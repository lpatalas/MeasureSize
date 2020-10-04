#Requires -PSEdition Core
[CmdletBinding()]
param()

& (Join-Path $PSScriptRoot 'build.ps1')

$modulePath = Join-Path $PSScriptRoot '.artifacts' 'publish' 'MeasureSize' 'MeasureSize.psd1'
pwsh.exe -NoExit -NoProfile -Command "Import-Module '$modulePath'"
