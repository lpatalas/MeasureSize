#Requires -PSEdition Core -Modules platyPS
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$buildVars = & (Join-Path $PSScriptRoot 'buildvars.ps1')

if (Test-Path $buildVars.ModulePublishDir) {
    Write-Host "Removing existing directory $($buildVars.ModulePublishDir)"
    Remove-Item $buildVars.ModulePublishDir -Recurse
}

dotnet publish `
    --configuration Release `
    --output $buildVars.ModulePublishDir `
    $buildVars.ProjectPath

if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with error code $LASTEXITCODE"
}

New-ExternalHelp -Path $buildVars.DocsDir -OutputPath $buildVars.ModulePublishDir -Force `
    | ForEach-Object { Write-Host "Generated help file: $_" }
