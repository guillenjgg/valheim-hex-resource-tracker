param([Parameter(Mandatory = $true)][string]$CscPath)
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$projectRoot = Join-Path $repoRoot 'HexResourceTracker'
$outputDirectory = Join-Path $projectRoot 'obj\TrackedBiomesChecks'
New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
$outputFile = Join-Path $outputDirectory 'Checks.exe'
$sources = @(
    (Join-Path $PSScriptRoot 'TrackedBiomesChecks.cs'),
    (Join-Path $projectRoot 'Core\TrackedBiomes.cs'),
    (Join-Path $projectRoot 'Core\TrackedResources.cs'),
    (Join-Path $projectRoot 'Core\TrackedDungeons.cs'),
    (Join-Path $projectRoot 'Core\TrackedCreatures.cs'),
    (Join-Path $projectRoot 'Core\TrackedLocations.cs'),
    (Join-Path $projectRoot 'Core\CreaturePacks.cs'),
    (Join-Path $projectRoot 'Models\TrackedResourceDefinitionModel.cs'),
    (Join-Path $projectRoot 'Models\TrackedResourceTypeEnum.cs'),
    (Join-Path $projectRoot 'Models\TrackedDungeonDefinitionModel.cs'),
    (Join-Path $projectRoot 'Models\TrackedCreatureDefinitionModel.cs')
)
& $CscPath /nologo "/out:$outputFile" @sources
if ($LASTEXITCODE -ne 0) { throw 'Biome checks failed to compile.' }
& $outputFile
if ($LASTEXITCODE -ne 0) { throw 'Biome checks failed.' }
