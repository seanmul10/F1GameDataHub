using System.Globalization;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using F1.Common;
using F1Packets;
using F1Packets.F125;
using F126 = F1Packets.F126;
using Npgsql;

namespace F1GameDataHub.Web.Services;

public sealed class DumpImportJobService(IConfiguration configuration, JobStatusStore statusStore)
{
    private readonly string _connectionString =
        configuration["F1_DB_CONNECTION_STRING"]
        ?? Environment.GetEnvironmentVariable("F1_DB_CONNECTION_STRING")
        ?? "Host=localhost;Port=55432;Username=postgres;Password=postgres;Database=postgres";

    private Task? _importTask;
    private readonly object _gate = new();

    public bool TryStart(string dumpPath, bool resetBeforeImport, out string message)
    {
        if (!File.Exists(dumpPath))
        {
            message = $"Dump file does not exist: {dumpPath}";
            return false;
        }

        lock (_gate)
        {
            if (_importTask is { IsCompleted: false })
            {
                message = "Import is already running.";
                return false;
            }

            statusStore.SetImportStatus(JobRunState.Running, $"Starting import: {dumpPath}");
            _importTask = Task.Run(() => RunImportAsync(dumpPath, resetBeforeImport));
            message = "Import started.";
            return true;
        }
    }

    private async Task RunImportAsync(string dumpPath, bool resetBeforeImport)
    {
        try
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await EnsureReplaySchemaAsync(conn);
            if (resetBeforeImport)
            {
                await ResetReplayTablesAsync(conn);
            }

            var stats = await ImportDumpAsync(conn, dumpPath);
            statusStore.SetImportStatus(
                JobRunState.Completed,
                $"Completed. packets={stats.PacketsRead}, sessions={stats.SessionsInserted}, events={stats.EventsInserted}, telemetry_rows={stats.TelemetryRowsInserted}");
        }
        catch (Exception ex)
        {
            statusStore.SetImportStatus(JobRunState.Failed, $"Import failed: {ex.Message}");
        }
    }

    private async Task<ImportStats> ImportDumpAsync(NpgsqlConnection conn, string dumpPath)
    {
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
            if (stats.PacketsRead % 5000 == 0)
            {
                statusStore.SetImportStatus(JobRunState.Running, $"Imported {stats.PacketsRead} packets...");
            }

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
                    stats.TelemetryRowsInserted += await InsertTelemetryPacketAsync(conn, tx, packetBytes, header.PacketFormat, packetTimestamp);
                    break;
            }
        }

        await tx.CommitAsync();
        return stats;
    }

    private static async Task EnsureReplaySchemaAsync(NpgsqlConnection conn)
    {
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        var solutionRoot = Path.GetFullPath(Path.Combine(projectRoot, ".."));
        var schemaPath = Path.Combine(solutionRoot, "..", "Scripts", "Schema", "Replay", "001_replay_schema.sql");
        schemaPath = Path.GetFullPath(schemaPath);

        if (!File.Exists(schemaPath))
        {
            throw new FileNotFoundException($"Replay schema file not found at: {schemaPath}");
        }

        var sql = await File.ReadAllTextAsync(schemaPath);
        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task ResetReplayTablesAsync(NpgsqlConnection conn)
    {
        const string sql = """
TRUNCATE TABLE replay_telemetry;
TRUNCATE TABLE replay_events;
TRUNCATE TABLE replay_sessions;
""";
        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private static bool TryReadCommonHeader(byte[] packetBytes, int packetFormat, out long sessionUid, out int frameId, out int gameYear)
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

    private static async Task InsertSessionPacketAsync(
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

        const string sql = """
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
    formula = EXCLUDED.formula;
""";

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

    private static async Task InsertEventPacketAsync(
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        int packetFormat,
        long sessionUid,
        int frameId,
        int gameYear,
        DateTimeOffset packetTimestamp,
        string eventCode)
    {
        const string sql = """
INSERT INTO replay_events (
    session_uid, frame_id, packet_format, game_year, packet_timestamp, event_code, packet_json
)
VALUES (
    @session_uid, @frame_id, @packet_format, @game_year, @packet_timestamp, @event_code, @packet_json::jsonb
)
ON CONFLICT (session_uid, frame_id, event_code) DO NOTHING;
""";

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

    private static async Task<int> InsertTelemetryPacketAsync(
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
                    conn, tx, sessionUid, frameId, i, packetFormat, gameYear, packetTimestamp,
                    packet.CarTelemetryData[i].Throttle, packet.CarTelemetryData[i].Brake, packet.CarTelemetryData[i].Steer, packet.CarTelemetryData[i].Speed);
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
                    conn, tx, sessionUid, frameId, i, packetFormat, gameYear, packetTimestamp,
                    packet.CarTelemetryData[i].Throttle, packet.CarTelemetryData[i].Brake, packet.CarTelemetryData[i].Steer, packet.CarTelemetryData[i].Speed);
            }

            return inserted;
        }

        return 0;
    }

    private static async Task<int> InsertTelemetryRowAsync(
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
        const string sql = """
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
    speed = EXCLUDED.speed;
""";

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

    private static string? ReadEventCode(byte[] packetBytes, int packetFormat)
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

    private sealed class ImportStats
    {
        public int PacketsRead { get; set; }
        public int SessionsInserted { get; set; }
        public int EventsInserted { get; set; }
        public int TelemetryRowsInserted { get; set; }
    }
}
