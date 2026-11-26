param([Parameter(Mandatory = $true)][string]$CscPath)
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$projectRoot = Join-Path $repoRoot 'HexResourceTracker'
$outputDirectory = Join-Path $projectRoot 'obj\TrackedResourceAliasChecks'
New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
$outputFile = Join-Path $outputDirectory 'Checks.exe'
$sources = @(
    (Join-Path $PSScriptRoot 'TrackedResourceAliasChecks.cs'),
    (Join-Path $projectRoot 'Core\TrackedResources.cs'),
    (Join-Path $projectRoot 'Models\TrackedResourceDefinitionModel.cs'),
    (Join-Path $projectRoot 'Models\TrackedResourceTypeEnum.cs')
)
& $CscPath /nologo "/out:$outputFile" @sources
if ($LASTEXITCODE -ne 0) { throw 'Resource variant checks failed to compile.' }
& $outputFile
if ($LASTEXITCODE -ne 0) { throw 'Resource variant checks failed.' }
