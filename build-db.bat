@echo off
setlocal ENABLEDELAYEDEXPANSION

REM --- Config ---
set PGUSER=postgres
set PGDATABASE=postgres
set PGPASSWORD=postgres
set PGHOST=localhost
set PGPORT=5432

REM --- Base directory (same as batch file) ---
set BASEDIR=%~dp0

REM Export PGPASSWORD so psql uses it automatically (works on Windows)
set PGPASSWORD=%PGPASSWORD%

echo =============================================
echo Run started at %date% %time%
echo =============================================
echo.

REM Run schema scripts
echo Running schema scripts...
for %%f in ("%BASEDIR%Scripts\Schema\*.sql") do (
    echo Running %%~nxf...
    psql -h %PGHOST% -p %PGPORT% -U %PGUSER% -d %PGDATABASE% -v ON_ERROR_STOP=1 -f "%%f"
    set EXITCODE=!ERRORLEVEL!
    if not !EXITCODE! == 0 (
        echo ERROR running %%~nxf
        pause
        exit /b !EXITCODE!
    )
)

REM Run static data scripts
echo Running static data scripts...
for %%f in ("%BASEDIR%Scripts\StaticData\*.sql") do (
    echo Running %%~nxf...
    psql -h %PGHOST% -p %PGPORT% -U %PGUSER% -d %PGDATABASE% -v ON_ERROR_STOP=1 -f "%%f"
    set EXITCODE=!ERRORLEVEL!
    if not !EXITCODE! == 0 (
        echo ERROR running %%~nxf
        pause
        exit /b !EXITCODE!
    )
)

echo All scripts executed successfully.
endlocal
pause
