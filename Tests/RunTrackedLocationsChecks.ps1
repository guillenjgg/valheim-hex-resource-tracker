param([Parameter(Mandatory = $true)][string]$CscPath)
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$projectRoot = Join-Path $repoRoot 'HexResourceTracker'
$outputDirectory = Join-Path $projectRoot 'obj\TrackedLocationsChecks'
New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
$outputFile = Join-Path $outputDirectory 'Checks.exe'
$sources = @(
    (Join-Path $PSScriptRoot 'TrackedLocationsChecks.cs'),
    (Join-Path $projectRoot 'Core\TrackedLocations.cs')
)
& $CscPath /nologo "/out:$outputFile" @sources
if ($LASTEXITCODE -ne 0) { throw 'Location group checks failed to compile.' }
& $outputFile
if ($LASTEXITCODE -ne 0) { throw 'Location group checks failed.' }
