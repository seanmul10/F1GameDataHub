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
