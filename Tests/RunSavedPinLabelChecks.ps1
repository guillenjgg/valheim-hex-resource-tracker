param([Parameter(Mandatory = $true)][string]$CscPath)
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$projectRoot = Join-Path $repoRoot 'HexResourceTracker'
$outputDirectory = Join-Path $projectRoot 'obj\SavedPinChecks'
New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
$outputFile = Join-Path $outputDirectory 'Checks.exe'
$sources = @(
    (Join-Path $PSScriptRoot 'SavedPinLabelServiceChecks.cs'),
    (Join-Path $projectRoot 'Core\PinManagers\PinLabelService.cs'),
    (Join-Path $projectRoot 'Core\PinManagers\SavedPinLabelService.cs'),
    (Join-Path $projectRoot 'Core\TrackedDungeons.cs'),
    (Join-Path $projectRoot 'Core\TrackedLocations.cs'),
    (Join-Path $projectRoot 'Models\TrackedDungeonDefinitionModel.cs')
)
& $CscPath /nologo "/out:$outputFile" @sources
if ($LASTEXITCODE -ne 0) { throw 'Label checks failed to compile.' }
& $outputFile
if ($LASTEXITCODE -ne 0) { throw 'Label checks failed.' }
