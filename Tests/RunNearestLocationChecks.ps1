param([Parameter(Mandatory = $true)][string]$CscPath)
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$projectRoot = Join-Path $repoRoot 'HexResourceTracker'
$outputDirectory = Join-Path $projectRoot 'obj\NearestLocationChecks'
New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
$outputFile = Join-Path $outputDirectory 'Checks.exe'
$sources = @(
    (Join-Path $PSScriptRoot 'NearestLocationChecks.cs'),
    (Join-Path $projectRoot 'Core\PinManagers\PinLabelService.cs'),
    (Join-Path $projectRoot 'Core\PinManagers\LocationPinManager.cs'),
    (Join-Path $projectRoot 'Core\LocationSearchService.cs')
)
& $CscPath /nologo "/out:$outputFile" @sources
if ($LASTEXITCODE -ne 0) { throw 'Label checks failed to compile.' }
& $outputFile
if ($LASTEXITCODE -ne 0) { throw 'Label checks failed.' }
