using System.Globalization;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using F1.Common;
using F1Packets;
using F1Packets.F125;
using F126 = F1Packets.F126;
using Npgsql;

var argsList = args.ToList();
if (argsList.Count == 0)
{
    PrintUsage();
    return 1;
}

var connectionString =
    Environment.GetEnvironmentVariable("F1_DB_CONNECTION_STRING")
    ?? "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=postgres";

await using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();

var command = argsList[0].ToLowerInvariant();

switch (command)
{
    case "init-schema":
        await EnsureReplaySchemaAsync(conn);
        Console.WriteLine("Replay schema initialized.");
        return 0;

    case "import":
        await EnsureReplaySchemaAsync(conn);
        await HandleImportAsync(conn, argsList.Skip(1).ToList());
        return 0;

    case "flashback":
        await HandleFlashbackAsync(conn, argsList.Skip(1).ToList());
        return 0;

    default:
        PrintUsage();
        return 1;
}

static async Task HandleImportAsync(NpgsqlConnection conn, List<string> importArgs)
{
    var reset = importArgs.Contains("--reset", StringComparer.OrdinalIgnoreCase);
    var dumpPaths = importArgs.Where(arg => !arg.StartsWith("--", StringComparison.Ordinal)).ToList();

    if (dumpPaths.Count == 0)
    {
        throw new ArgumentException("No dump file paths were provided for import.");
    }

    if (reset)
    {
        await ResetReplayTablesAsync(conn);
        Console.WriteLine("Replay tables truncated.");
    }

    foreach (var dumpPath in dumpPaths)
    {
        var stats = await ImportDumpAsync(conn, dumpPath);
        Console.WriteLine(
            $"Imported {dumpPath} | packets={stats.PacketsRead}, sessions={stats.SessionsInserted}, events={stats.EventsInserted}, telemetry_rows={stats.TelemetryRowsInserted}"
        );
    }
}

static async Task HandleFlashbackAsync(NpgsqlConnection conn, List<string> flashbackArgs)
{
    if (flashbackArgs.Count != 2)
    {
        throw new ArgumentException("flashback requires <session_uid> <frame_id>.");
    }

    if (!long.TryParse(flashbackArgs[0], out var sessionUid))
    {
        throw new ArgumentException("Invalid session_uid.");
    }

    if (!int.TryParse(flashbackArgs[1], out var frameId))
    {
        throw new ArgumentException("Invalid frame_id.");
    }

    await using var tx = await conn.BeginTransactionAsync();

    var telemetryDeleted = await DeleteAfterFrameAsync(conn, tx, "replay_telemetry", sessionUid, frameId);
    var eventsDeleted = await DeleteAfterFrameAsync(conn, tx, "replay_events", sessionUid, frameId);
    var sessionsDeleted = await DeleteAfterFrameAsync(conn, tx, "replay_sessions", sessionUid, frameId);

    await tx.CommitAsync();

    Console.WriteLine(
        $"Flashback applied for session {sessionUid} at frame {frameId} | telemetry_deleted={telemetryDeleted}, events_deleted={eventsDeleted}, sessions_deleted={sessionsDeleted}"
    );
}

static async Task<int> DeleteAfterFrameAsync(
    NpgsqlConnection conn,
    NpgsqlTransaction tx,
    string tableName,
    long sessionUid,
    int frameId)
{
    var sql = $"DELETE FROM {tableName} WHERE session_uid = @session_uid AND frame_id > @frame_id;";
    await using var cmd = new NpgsqlCommand(sql, conn, tx);
    cmd.Parameters.AddWithValue("session_uid", sessionUid);
    cmd.Parameters.AddWithValue("frame_id", frameId);
    return await cmd.ExecuteNonQueryAsync();
}

static async Task EnsureReplaySchemaAsync(NpgsqlConnection conn)
{
    var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
    var repoRoot = Path.GetFullPath(Path.Combine(projectRoot, "..", ".."));
    var schemaPath = Path.Combine(repoRoot, "Scripts", "Schema", "Replay", "001_replay_schema.sql");

    if (!File.Exists(schemaPath))
    {
        throw new FileNotFoundException($"Replay schema file not found at: {schemaPath}");
    }

    var sql = await File.ReadAllTextAsync(schemaPath);
    await using var cmd = new NpgsqlCommand(sql, conn);
    await cmd.ExecuteNonQueryAsync();
}

static async Task ResetReplayTablesAsync(NpgsqlConnection conn)
{
    const string sql = @"
TRUNCATE TABLE replay_telemetry;
TRUNCATE TABLE replay_events;
TRUNCATE TABLE replay_sessions;
";
    await using var cmd = new NpgsqlCommand(sql, conn);
    await cmd.ExecuteNonQueryAsync();
}

static async Task<ImportStats> ImportDumpAsync(NpgsqlConnection conn, string dumpPath)
{
    if (!File.Exists(dumpPath))
    {
        throw new FileNotFoundException($"Dump file not found: {dumpPath}");
    }

    await using var tx = await conn.BeginTransactionAsync();

    using var fileStream = new FileStream(dumpPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    using Stream inputStream = dumpPath.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)
        ? new GZipStream(fileStream, CompressionMode.Decompress)
        : fileStream;
    using var reader = new BinaryReader(inputStream, Encoding.UTF8, leaveOpen: false);

    var stats = new ImportStats();

    while (true)
    {
        int timestampLength;
        try
        {
            timestampLength = reader.ReadInt32();
        }
        catch (EndOfStreamException)
        {
            break;
        }

        if (timestampLength <= 0 || timestampLength > 256)
        {
            throw new InvalidDataException($"Invalid timestamp length: {timestampLength}");
        }

        var timestampBytes = reader.ReadBytes(timestampLength);
        var timestampRaw = Encoding.UTF8.GetString(timestampBytes);
        var packetTimestamp = DateTimeOffset.Parse(timestampRaw, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        var packetLength = reader.ReadInt32();
        if (packetLength <= 0)
        {
            continue;
        }

        var packetBytes = reader.ReadBytes(packetLength);
        if (packetBytes.Length != packetLength)
        {
            break;
        }

        stats.PacketsRead++;

        if (!PacketInspector.TryReadHeader(packetBytes, out var header))
        {
            continue;
        }

        var packetId = (F1PacketId)header.PacketId;
        switch (packetId)
        {
            case F1PacketId.Session:
                if (TryReadCommonHeader(packetBytes, header.PacketFormat, out var sessionUid, out var frameId, out var gameYear))
                {
                    await InsertSessionPacketAsync(conn, tx, header.PacketFormat, sessionUid, frameId, gameYear, packetTimestamp, packetBytes);
                    stats.SessionsInserted++;
                }

                break;

            case F1PacketId.Event:
                if (TryReadCommonHeader(packetBytes, header.PacketFormat, out sessionUid, out frameId, out gameYear))
                {
                    var eventCode = ReadEventCode(packetBytes, header.PacketFormat) ?? "UNKN";
                    await InsertEventPacketAsync(conn, tx, header.PacketFormat, sessionUid, frameId, gameYear, packetTimestamp, eventCode);
                    stats.EventsInserted++;
                }

                break;

            case F1PacketId.CarTelemetry:
                var telemetryRows = await InsertTelemetryPacketAsync(conn, tx, packetBytes, header.PacketFormat, packetTimestamp);
                stats.TelemetryRowsInserted += telemetryRows;
                break;
        }
    }

    await tx.CommitAsync();
    return stats;
}

static async Task InsertSessionPacketAsync(
    NpgsqlConnection conn,
    NpgsqlTransaction tx,
    int packetFormat,
    long sessionUid,
    int frameId,
    int gameYear,
    DateTimeOffset packetTimestamp,
    byte[] packetBytes)
{
    int? trackId = null;
    string? sessionType = null;
    string? sessionLength = null;
    string? formula = null;

    if (packetFormat == PacketFormats.F125)
    {
        var packet = Utils.ReadFromBytes<PacketSessionData>(packetBytes);
        trackId = packet.TrackId;
        sessionType = packet.SessionType.ToString();
        sessionLength = packet.SessionLength.ToString();
        formula = packet.Formula.ToString();
    }

    const string sql = @"
INSERT INTO replay_sessions (
    session_uid, frame_id, packet_format, game_year, packet_timestamp,
    track_id, session_type, session_length, formula, packet_json
)
VALUES (
    @session_uid, @frame_id, @packet_format, @game_year, @packet_timestamp,
    @track_id, @session_type, @session_length, @formula, @packet_json::jsonb
)
ON CONFLICT (session_uid, frame_id)
DO UPDATE SET
    packet_timestamp = EXCLUDED.packet_timestamp,
    track_id = EXCLUDED.track_id,
    session_type = EXCLUDED.session_type,
    session_length = EXCLUDED.session_length,
    formula = EXCLUDED.formula;";

    await using var cmd = new NpgsqlCommand(sql, conn, tx);
    cmd.Parameters.AddWithValue("session_uid", sessionUid);
    cmd.Parameters.AddWithValue("frame_id", frameId);
    cmd.Parameters.AddWithValue("packet_format", packetFormat);
    cmd.Parameters.AddWithValue("game_year", gameYear);
    cmd.Parameters.AddWithValue("packet_timestamp", packetTimestamp.UtcDateTime);
    cmd.Parameters.AddWithValue("track_id", (object?)trackId ?? DBNull.Value);
    cmd.Parameters.AddWithValue("session_type", (object?)sessionType ?? DBNull.Value);
    cmd.Parameters.AddWithValue("session_length", (object?)sessionLength ?? DBNull.Value);
    cmd.Parameters.AddWithValue("formula", (object?)formula ?? DBNull.Value);
    cmd.Parameters.AddWithValue("packet_json", "{}");
    await cmd.ExecuteNonQueryAsync();
}

static async Task InsertEventPacketAsync(
    NpgsqlConnection conn,
    NpgsqlTransaction tx,
    int packetFormat,
    long sessionUid,
    int frameId,
    int gameYear,
    DateTimeOffset packetTimestamp,
    string eventCode)
{
    const string sql = @"
INSERT INTO replay_events (
    session_uid, frame_id, packet_format, game_year, packet_timestamp, event_code, packet_json
)
VALUES (
    @session_uid, @frame_id, @packet_format, @game_year, @packet_timestamp, @event_code, @packet_json::jsonb
)
ON CONFLICT (session_uid, frame_id, event_code) DO NOTHING;";

    await using var cmd = new NpgsqlCommand(sql, conn, tx);
    cmd.Parameters.AddWithValue("session_uid", sessionUid);
    cmd.Parameters.AddWithValue("frame_id", frameId);
    cmd.Parameters.AddWithValue("packet_format", packetFormat);
    cmd.Parameters.AddWithValue("game_year", gameYear);
    cmd.Parameters.AddWithValue("packet_timestamp", packetTimestamp.UtcDateTime);
    cmd.Parameters.AddWithValue("event_code", eventCode);
    cmd.Parameters.AddWithValue("packet_json", "{}");
    await cmd.ExecuteNonQueryAsync();
}

static async Task<int> InsertTelemetryPacketAsync(
    NpgsqlConnection conn,
    NpgsqlTransaction tx,
    byte[] packetBytes,
    int packetFormat,
    DateTimeOffset packetTimestamp)
{
    if (packetFormat == PacketFormats.F125)
    {
        var packet = Utils.ReadFromBytes<PacketCarTelemetryData>(packetBytes);
        var sessionUid = unchecked((long)packet.Header.SessionUid);
        var frameId = (int)packet.Header.FrameIdentifier;
        var gameYear = packet.Header.GameYear;

        var inserted = 0;
        for (var i = 0; i < PacketConstants.MaxNumCarsInUdpData; i++)
        {
            inserted += await InsertTelemetryRowAsync(
                conn,
                tx,
                sessionUid,
                frameId,
                i,
                packetFormat,
                gameYear,
                packetTimestamp,
                packet.CarTelemetryData[i].Throttle,
                packet.CarTelemetryData[i].Brake,
                packet.CarTelemetryData[i].Steer,
                packet.CarTelemetryData[i].Speed);
        }

        return inserted;
    }

    if (packetFormat == PacketFormats.F126)
    {
        var packet = Utils.ReadFromBytes<F126.PacketCarTelemetryData>(packetBytes);
        var sessionUid = unchecked((long)packet.Header.SessionUid);
        var frameId = (int)packet.Header.FrameIdentifier;
        var gameYear = packet.Header.GameYear;

        var inserted = 0;
        for (var i = 0; i < F126.PacketConstants.MaxNumCarsInUdpData; i++)
        {
            inserted += await InsertTelemetryRowAsync(
                conn,
                tx,
                sessionUid,
                frameId,
                i,
                packetFormat,
                gameYear,
                packetTimestamp,
                packet.CarTelemetryData[i].Throttle,
                packet.CarTelemetryData[i].Brake,
                packet.CarTelemetryData[i].Steer,
                packet.CarTelemetryData[i].Speed);
        }

        return inserted;
    }

    return 0;
}

static async Task<int> InsertTelemetryRowAsync(
    NpgsqlConnection conn,
    NpgsqlTransaction tx,
    long sessionUid,
    int frameId,
    int driverIndex,
    int packetFormat,
    int gameYear,
    DateTimeOffset packetTimestamp,
    float throttle,
    float brake,
    float steering,
    int speed)
{
    const string sql = @"
INSERT INTO replay_telemetry (
    session_uid, frame_id, driver_index, packet_format, game_year, packet_timestamp, throttle, brake, steering, speed
)
VALUES (
    @session_uid, @frame_id, @driver_index, @packet_format, @game_year, @packet_timestamp, @throttle, @brake, @steering, @speed
)
ON CONFLICT (session_uid, frame_id, driver_index)
DO UPDATE SET
    packet_timestamp = EXCLUDED.packet_timestamp,
    throttle = EXCLUDED.throttle,
    brake = EXCLUDED.brake,
    steering = EXCLUDED.steering,
    speed = EXCLUDED.speed;";

    await using var cmd = new NpgsqlCommand(sql, conn, tx);
    cmd.Parameters.AddWithValue("session_uid", sessionUid);
    cmd.Parameters.AddWithValue("frame_id", frameId);
    cmd.Parameters.AddWithValue("driver_index", driverIndex);
    cmd.Parameters.AddWithValue("packet_format", packetFormat);
    cmd.Parameters.AddWithValue("game_year", gameYear);
    cmd.Parameters.AddWithValue("packet_timestamp", packetTimestamp.UtcDateTime);
    cmd.Parameters.AddWithValue("throttle", throttle);
    cmd.Parameters.AddWithValue("brake", brake);
    cmd.Parameters.AddWithValue("steering", steering);
    cmd.Parameters.AddWithValue("speed", speed);
    return await cmd.ExecuteNonQueryAsync();
}

static string? ReadEventCode(byte[] packetBytes, int packetFormat)
{
    var headerSize = packetFormat == PacketFormats.F126
        ? Marshal.SizeOf<F126.PacketHeader>()
        : Marshal.SizeOf<PacketHeader>();

    if (packetBytes.Length < headerSize + 4)
    {
        return null;
    }

    var code = Encoding.ASCII.GetString(packetBytes, headerSize, 4).Trim('\0', ' ');
    return string.IsNullOrWhiteSpace(code) ? null : code;
}

static bool TryReadCommonHeader(byte[] packetBytes, int packetFormat, out long sessionUid, out int frameId, out int gameYear)
{
    if (packetFormat == PacketFormats.F125)
    {
        var header = Utils.ReadFromBytes<PacketHeader>(packetBytes);
        sessionUid = unchecked((long)header.SessionUid);
        frameId = (int)header.FrameIdentifier;
        gameYear = header.GameYear;
        return true;
    }

    if (packetFormat == PacketFormats.F126)
    {
        var header = Utils.ReadFromBytes<F126.PacketHeader>(packetBytes);
        sessionUid = unchecked((long)header.SessionUid);
        frameId = (int)header.FrameIdentifier;
        gameYear = header.GameYear;
        return true;
    }

    sessionUid = 0;
    frameId = 0;
    gameYear = 0;
    return false;
}

static void PrintUsage()
{
    Console.WriteLine("PacketDumpImporter usage:");
    Console.WriteLine("  init-schema");
    Console.WriteLine("  import [--reset] <dumpPath1.gz> [dumpPath2.gz ...]");
    Console.WriteLine("  flashback <session_uid> <frame_id>");
}

file sealed class ImportStats
{
    public int PacketsRead { get; set; }
    public int SessionsInserted { get; set; }
    public int EventsInserted { get; set; }
    public int TelemetryRowsInserted { get; set; }
}
