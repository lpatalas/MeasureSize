#Requires -PSEdition Core
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$buildVars = & (Join-Path $PSScriptRoot 'buildvars.ps1')

& (Join-Path $PSScriptRoot 'build.ps1')

$modulePath = Join-Path $buildVars.ModulePublishDir "$($buildVars.ModuleName).psd1"
pwsh.exe -NoExit -NoProfile -Command "Import-Module '$modulePath'"
