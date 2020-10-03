#Requires -PSEdition Core -Module PowerShellGet
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [String] $PSRepositoryName,

    [Parameter(Mandatory, ParameterSetName = "LocalPublish")]
    [switch] $LocalPublish,

    [Parameter(Mandatory, ParameterSetName = "OnlinePublish")]
    [String] $ApiKey
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$projectPath = Join-Path $PSScriptRoot 'src' 'MeasureSize.fsproj'
$moduleName = [System.IO.Path]::GetFileNameWithoutExtension($projectPath)
$artifactsDir = Join-Path $PSScriptRoot '.artifacts'
$rootPublishDir = Join-Path $artifactsDir 'publish'
$modulePublishDir = Join-Path $rootPublishDir $moduleName

Write-Host "Publishing '$ProjectPath' to intermediate directory $modulePublishDir" -ForegroundColor Cyan

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

$originalModulePath = $env:PSModulePath
try {
    $env:PSModulePath += ";$rootPublishDir"

    if ($LocalPublish) {
        Write-Host 'Running local publish'

        Publish-Module `
            -Path $modulePublishDir `
            -Repository $PSRepositoryName

        Write-Host 'Publish succeeded' -ForegroundColor Green
    }
    else {
        Write-Host 'Running Publish-Module ... -WhatIf' -ForegroundColor Cyan
        Publish-Module `
            -Path $modulePublishDir `
            -Repository $PSRepositoryName `
            -NuGetApiKey $ApiKey `
            -Verbose `
            -WhatIf

        if ($PSCmdlet.ShouldContinue("Publish module '$ModulePath' to repository '$PSRepositoryName'?", "Confirm Publish")) {
            Publish-Module `
                -Path $modulePublishDir `
                -Repository $PSRepositoryName `
                -NuGetApiKey $ApiKey

            Write-Host 'Publish succeeded' -ForegroundColor Green
        }
        else {
            Write-Host 'Publish was cancelled' -ForegroundColor Yellow
        }
    }
}
finally {
    $env:PSModulePath = $originalModulePath
}
