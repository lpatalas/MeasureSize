[CmdletBinding()]
param(
    [switch] $NoBuild
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$buildVars = & (Join-Path $PSScriptRoot 'buildvars.ps1')

if (-not $NoBuild) {
    & (Join-Path $PSScriptRoot 'build.ps1')
}

$command = @(
    "Import-Module $($buildVars.ModuleDefinitionPath)"
    "Invoke-Pester $(Join-Path $buildVars.TestDir '*.Tests.ps1') -Show All"
) -join ';'

pwsh.exe -NoLogo -NoProfile -Command $command
