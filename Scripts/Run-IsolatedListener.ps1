[CmdletBinding()]
param(
    [int]$UdpPort = 20778,
    [int]$DbPort = 5432,
    [string]$DbHost = "localhost",
    [string]$DbUser = "postgres",
    [string]$DbPassword = "postgres",
    [string]$DbName = "postgres",
    [string]$ContainerName = "f1gamedatahub-isolated",
    [switch]$UseLocalPostgres,
    [switch]$KeepContainer
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Invoke-Docker {
    param([Parameter(Mandatory = $true)][string[]]$Args)
    & docker @Args
}

function Wait-ForPostgresContainer {
    param([Parameter(Mandatory = $true)][string]$Name)

    $deadline = (Get-Date).AddSeconds(45)
    while ((Get-Date) -lt $deadline) {
        $readyOutput = Invoke-Docker -Args @("exec", $Name, "pg_isready", "-U", "postgres")
        if ($LASTEXITCODE -eq 0 -and ($readyOutput -join "`n") -match "accepting connections") {
            return
        }

        Start-Sleep -Seconds 1
    }

    throw "Postgres container '$Name' did not become ready in time."
}

function Get-DockerStatusLine {
    param([Parameter(Mandatory = $true)][string]$Name)

    $status = Invoke-Docker -Args @(
        "exec",
        $Name,
        "psql", "-U", "postgres", "-d", "postgres", "-t", "-A", "-F", " | ",
        "-c",
        "SELECT
            (SELECT COUNT(*) FROM session_metadata) AS sessions,
            (SELECT COUNT(*) FROM participants) AS participants,
            (SELECT COUNT(*) FROM car_telemetry) AS telemetry_rows,
            COALESCE((SELECT MAX(frame_id) FROM car_telemetry), 0) AS latest_frame;"
    )

    $line = ($status -join "").Trim()
    if ([string]::IsNullOrWhiteSpace($line)) {
        return "0 | 0 | 0 | 0"
    }

    return $line
}

function Get-LocalStatusLine {
    param(
        [Parameter(Mandatory = $true)][string]$Host,
        [Parameter(Mandatory = $true)][int]$Port,
        [Parameter(Mandatory = $true)][string]$User,
        [Parameter(Mandatory = $true)][string]$Password,
        [Parameter(Mandatory = $true)][string]$Database
    )

    $psqlCmd = Get-Command psql -ErrorAction SilentlyContinue
    if ($null -eq $psqlCmd) {
        return $null
    }

    $previousPgPassword = [Environment]::GetEnvironmentVariable("PGPASSWORD", "Process")
    try {
        [Environment]::SetEnvironmentVariable("PGPASSWORD", $Password, "Process")
        $status = & psql -h $Host -p $Port -U $User -d $Database -t -A -F " | " -c @"
SELECT
    (SELECT COUNT(*) FROM session_metadata) AS sessions,
    (SELECT COUNT(*) FROM participants) AS participants,
    (SELECT COUNT(*) FROM car_telemetry) AS telemetry_rows,
    COALESCE((SELECT MAX(frame_id) FROM car_telemetry), 0) AS latest_frame;
"@
        if ($LASTEXITCODE -ne 0) {
            return $null
        }

        $line = ($status -join "").Trim()
        if ([string]::IsNullOrWhiteSpace($line)) {
            return "0 | 0 | 0 | 0"
        }

        return $line
    }
    finally {
        [Environment]::SetEnvironmentVariable("PGPASSWORD", $previousPgPassword, "Process")
    }
}

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$bootstrapProject = Join-Path $repoRoot "F1GameDataHub\DbSchemaBootstrap\DbSchemaBootstrap.csproj"
$listenerProject = Join-Path $repoRoot "F1GameDataHub\OvertakeTest\OvertakeTest.csproj"

$dockerCmd = Get-Command docker -ErrorAction SilentlyContinue
$dockerMode = (-not $UseLocalPostgres) -and ($null -ne $dockerCmd)

if (-not $UseLocalPostgres -and $null -eq $dockerCmd) {
    Write-Host "Docker not found. Falling back to local Postgres mode."
}

if ($dockerMode -and -not $PSBoundParameters.ContainsKey("DbPort")) {
    $DbPort = 55432
}

$dbConnectionString = "Host=$DbHost;Port=$DbPort;Username=$DbUser;Password=$DbPassword;Database=$DbName"

$originalDbEnv = [Environment]::GetEnvironmentVariable("F1_DB_CONNECTION_STRING", "Process")
$originalUdpEnv = [Environment]::GetEnvironmentVariable("F1_UDP_PORT", "Process")
$originalDisableDbEnv = [Environment]::GetEnvironmentVariable("F1_DISABLE_DB", "Process")
$listenerProcess = $null
$containerStarted = $false

try {
    if ($dockerMode) {
        $existingContainerId = (Invoke-Docker -Args @("ps", "-a", "-q", "-f", "name=^${ContainerName}$")) -join ""
        if (-not [string]::IsNullOrWhiteSpace($existingContainerId)) {
            throw "Container '$ContainerName' already exists. Remove it or pass a different -ContainerName."
        }

        Write-Host "Starting isolated Postgres container '$ContainerName' on localhost:$DbPort..."
        Invoke-Docker -Args @(
            "run", "-d",
            "--name", $ContainerName,
            "-e", "POSTGRES_USER=postgres",
            "-e", "POSTGRES_PASSWORD=postgres",
            "-e", "POSTGRES_DB=postgres",
            "-p", "${DbPort}:5432",
            "postgres:16-alpine"
        ) | Out-Null
        $containerStarted = $true
        Wait-ForPostgresContainer -Name $ContainerName
    }
    else {
        Write-Host "Using local Postgres on ${DbHost}:$DbPort ($DbName)."
    }

    [Environment]::SetEnvironmentVariable("F1_DB_CONNECTION_STRING", $dbConnectionString, "Process")
    [Environment]::SetEnvironmentVariable("F1_UDP_PORT", "$UdpPort", "Process")
    [Environment]::SetEnvironmentVariable("F1_DISABLE_DB", "false", "Process")

    Write-Host "Bootstrapping schema..."
    & dotnet run --project $bootstrapProject --nologo
    $dbOnline = $LASTEXITCODE -eq 0
    if (-not $dbOnline) {
        if ($dockerMode) {
            throw "Schema bootstrap failed."
        }

        Write-Host "Schema bootstrap failed for local Postgres. Continuing in parser-only mode (DB disabled)."
        [Environment]::SetEnvironmentVariable("F1_DISABLE_DB", "true", "Process")
    }

    Write-Host ""
    Write-Host "Starting listener..."
    Write-Host "Configure game UDP output port to: $UdpPort"
    Write-Host "Press 'Q' in this terminal to stop."
    Write-Host ""

    $listenerProcess = Start-Process `
        -FilePath "dotnet" `
        -ArgumentList @("run", "--project", $listenerProject, "--nologo") `
        -NoNewWindow `
        -PassThru

    $localStatusWarningShown = $false
    while (-not $listenerProcess.HasExited) {
        $statusLine = if (-not $dbOnline) {
            $null
        }
        elseif ($dockerMode) {
            Get-DockerStatusLine -Name $ContainerName
        }
        else {
            Get-LocalStatusLine -Host $DbHost -Port $DbPort -User $DbUser -Password $DbPassword -Database $DbName
        }

        if ($null -eq $statusLine) {
            if (-not $localStatusWarningShown) {
                if (-not $dbOnline) {
                    Write-Host "Database is disabled. Watching listener ingest logs only."
                }
                else {
                    Write-Host "Live DB counters unavailable (install psql for local counter queries). Using listener ingest logs only."
                }

                $localStatusWarningShown = $true
            }
        }
        else {
            Write-Host ("[{0}] sessions | participants | telemetry_rows | latest_frame => {1}" -f (Get-Date -Format "HH:mm:ss"), $statusLine)
        }

        Start-Sleep -Seconds 2
        if ([Console]::KeyAvailable) {
            $key = [Console]::ReadKey($true)
            if ($key.Key -eq [ConsoleKey]::Q) {
                break
            }
        }
    }
}
finally {
    if ($null -ne $listenerProcess -and -not $listenerProcess.HasExited) {
        Stop-Process -Id $listenerProcess.Id
    }

    [Environment]::SetEnvironmentVariable("F1_DB_CONNECTION_STRING", $originalDbEnv, "Process")
    [Environment]::SetEnvironmentVariable("F1_UDP_PORT", $originalUdpEnv, "Process")
    [Environment]::SetEnvironmentVariable("F1_DISABLE_DB", $originalDisableDbEnv, "Process")

    if ($dockerMode -and $containerStarted -and -not $KeepContainer) {
        Write-Host "Removing container '$ContainerName'..."
        Invoke-Docker -Args @("rm", "-f", $ContainerName) | Out-Null
    }
}
