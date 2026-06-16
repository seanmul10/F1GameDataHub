using Npgsql;

namespace F1GameDataHub.Web.Services;

public sealed record SessionSummaryItem(
    long SessionUid,
    DateTimeOffset SessionStartTimeUtc,
    string SessionType,
    int? TrackId,
    string TrackLabel);

public sealed class SessionSummaryService(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration["F1_DB_CONNECTION_STRING"]
        ?? Environment.GetEnvironmentVariable("F1_DB_CONNECTION_STRING")
        ?? "Host=localhost;Port=55432;Username=postgres;Password=postgres;Database=postgres";

    public async Task<IReadOnlyList<SessionSummaryItem>> GetSessionsAsync(int limit, CancellationToken cancellationToken = default)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync(cancellationToken);

        var trackColumn = await ResolveTrackDisplayColumnAsync(conn, cancellationToken);
        var query = BuildSessionsQuery(trackColumn);

        await using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("limit", limit);

        var sessions = new List<SessionSummaryItem>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var sessionUid = reader.GetInt64(0);
            var sessionStart = reader.GetFieldValue<DateTime>(1);
            var sessionType = reader.GetString(2);
            int? trackId = reader.IsDBNull(3) ? null : reader.GetInt32(3);
            var trackLabel = reader.GetString(4);

            sessions.Add(
                new SessionSummaryItem(
                    sessionUid,
                    new DateTimeOffset(DateTime.SpecifyKind(sessionStart, DateTimeKind.Utc)),
                    sessionType,
                    trackId,
                    trackLabel));
        }

        return sessions;
    }

    public async Task<SessionSummaryItem?> GetSessionAsync(long sessionUid, CancellationToken cancellationToken = default)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync(cancellationToken);

        var trackColumn = await ResolveTrackDisplayColumnAsync(conn, cancellationToken);
        var query = BuildSingleSessionQuery(trackColumn);

        await using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("session_uid", sessionUid);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var sessionStart = reader.GetFieldValue<DateTime>(1);
        return new SessionSummaryItem(
            reader.GetInt64(0),
            new DateTimeOffset(DateTime.SpecifyKind(sessionStart, DateTimeKind.Utc)),
            reader.GetString(2),
            reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
            reader.GetString(4));
    }

    private static async Task<string?> ResolveTrackDisplayColumnAsync(NpgsqlConnection conn, CancellationToken cancellationToken)
    {
        const string sql = """
SELECT column_name
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name = 'tracks'
  AND column_name IN ('name', 'type')
ORDER BY CASE WHEN column_name = 'name' THEN 0 ELSE 1 END
LIMIT 1;
""";

        await using var cmd = new NpgsqlCommand(sql, conn);
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return result as string;
    }

    private static string BuildSessionsQuery(string? trackColumn)
    {
        if (trackColumn is null)
        {
            return """
SELECT
    sm.session_uid,
    sm.session_start_time,
    sm.session_type,
    sm.track_id,
    CONCAT('Track ', sm.track_id::text) AS track_label
FROM session_metadata sm
ORDER BY sm.session_start_time DESC
LIMIT @limit
""";
        }

        return $"""
SELECT
    sm.session_uid,
    sm.session_start_time,
    sm.session_type,
    sm.track_id,
    COALESCE(t.{trackColumn}, CONCAT('Track ', sm.track_id::text)) AS track_label
FROM session_metadata sm
LEFT JOIN tracks t ON t.id = sm.track_id
ORDER BY sm.session_start_time DESC
LIMIT @limit
""";
    }

    private static string BuildSingleSessionQuery(string? trackColumn)
    {
        if (trackColumn is null)
        {
            return """
SELECT
    sm.session_uid,
    sm.session_start_time,
    sm.session_type,
    sm.track_id,
    CONCAT('Track ', sm.track_id::text) AS track_label
FROM session_metadata sm
WHERE sm.session_uid = @session_uid
LIMIT 1
""";
        }

        return $"""
SELECT
    sm.session_uid,
    sm.session_start_time,
    sm.session_type,
    sm.track_id,
    COALESCE(t.{trackColumn}, CONCAT('Track ', sm.track_id::text)) AS track_label
FROM session_metadata sm
LEFT JOIN tracks t ON t.id = sm.track_id
WHERE sm.session_uid = @session_uid
LIMIT 1
""";
    }
}
