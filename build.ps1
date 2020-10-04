#Requires -PSEdition Core -Modules platyPS
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$workspaceRoot = $PSScriptRoot
$projectPath = Join-Path $PSScriptRoot 'src' 'MeasureSize.fsproj'
$moduleName = [System.IO.Path]::GetFileNameWithoutExtension($projectPath)
$artifactsDir = Join-Path $PSScriptRoot '.artifacts'
$rootPublishDir = Join-Path $artifactsDir 'publish'
$modulePublishDir = Join-Path $rootPublishDir $moduleName

if (Test-Path $modulePublishDir) {
    Write-Host "Removing existing directory $modulePublishDir"
    Remove-Item $modulePublishDir -Recurse
}

dotnet publish `
    --configuration Release `
    --output $modulePublishDir `
    $ProjectPath

if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with error code $LASTEXITCODE"
}

$docsPath = Join-Path $workspaceRoot 'docs'
New-ExternalHelp -Path $docsPath -OutputPath $modulePublishDir -Force `
    | ForEach-Object { Write-Host "Generated help file: $_" }
