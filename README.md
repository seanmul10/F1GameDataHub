# F1GameDataHub
F1GameDataHub is an application for capturing, parsing, and analysing telemetry data from the F1 game series. It reads UDP packets sent from the game and provides tools to visualise driver performance, track results, and support league management.

Current UDP packet support includes packet formats **2025** (base F1 25 telemetry) and **2026** (F1 25 2026 Season Pack telemetry).

For an isolated local ingestion test (separate Postgres container + live row counters):

**Command Prompt (Batch):**
```cmd
.\Scripts\Run-IsolatedListener.bat
```

**Or PowerShell:**
```powershell
powershell -ExecutionPolicy Bypass -File .\Scripts\Run-IsolatedListener.ps1
```

If Docker is unavailable, both scripts automatically fall back to local Postgres mode. If local Postgres is also unavailable, the app runs in parser-only mode (ingesting packets without database writes).

## Packet dump replay importer prototype

`PacketDumpImporter` provides a standalone path for loading recorded `.gz` packet dumps directly into replay tables.

1. Initialize replay schema:
```powershell
dotnet run --project .\F1GameDataHub\PacketDumpImporter\PacketDumpImporter.csproj -- init-schema
```

2. Import one or more dumps (optionally reset replay tables first):
```powershell
dotnet run --project .\F1GameDataHub\PacketDumpImporter\PacketDumpImporter.csproj -- import --reset C:\path\to\dump1.gz C:\path\to\dump2.gz
```

3. Apply flashback rollback for a session:
```powershell
dotnet run --project .\F1GameDataHub\PacketDumpImporter\PacketDumpImporter.csproj -- flashback <session_uid> <frame_id>
```

### Replay reset/re-import workflow

Use this flow when iterating on schema/import logic:

1. Recreate/ensure replay schema:
```powershell
dotnet run --project .\F1GameDataHub\PacketDumpImporter\PacketDumpImporter.csproj -- init-schema
```
2. Import from a clean replay state:
```powershell
dotnet run --project .\F1GameDataHub\PacketDumpImporter\PacketDumpImporter.csproj -- import --reset C:\path\to\dump.gz
```
3. Validate imported metrics:
```sql
SELECT COUNT(*) FROM replay_sessions;
SELECT COUNT(*) FROM replay_events;
SELECT COUNT(*) FROM replay_telemetry;
SELECT session_uid, MIN(frame_id), MAX(frame_id), COUNT(*)
FROM replay_telemetry
GROUP BY session_uid
ORDER BY COUNT(*) DESC
LIMIT 5;
```

### Flashback handling

Flashback is applied by deleting rows where `frame_id > X` for a given `session_uid` across:
- `replay_sessions`
- `replay_events`
- `replay_telemetry`

This keeps replay state consistent after rewinds.

### Performance notes (prototype)

- Tables are keyed/indexed by `(session_uid, frame_id)` to support replay scans and flashback deletes.
- Hypertable creation is attempted when TimescaleDB functions are available; otherwise schema still works on plain Postgres.
- Prefer importing with `--reset` during iteration to avoid table-growth skewing measurements.
- For large dumps, run imports in background and validate using aggregate queries rather than row-by-row inspection.

## Razor UX shell (session browser + import + live listen)

Run the web app:

```powershell
dotnet run --project .\F1GameDataHub\F1GameDataHub.Web\F1GameDataHub.Web.csproj
```

From the home page you can:
- Select a saved session and view a summary (track/date/type).
- Start a `.gz` dump import in a background job.
- Start/stop live packet listening in a background job.

Default DB target is:
- `Host=localhost;Port=55432;Username=postgres;Password=postgres;Database=postgres`
