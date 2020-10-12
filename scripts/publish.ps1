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

$buildVars = & (Join-Path $PSScriptRoot 'buildvars.ps1')

& (Join-Path $PSScriptRoot 'build.ps1')

$originalModulePath = $env:PSModulePath
try {
    $env:PSModulePath += ";$($buildVars.RootPublishDir)"

    if ($LocalPublish) {
        Write-Host 'Running local publish'

        Publish-Module `
            -Path $buildVars.ModulePublishDir `
            -Repository $PSRepositoryName

        Write-Host 'Publish succeeded' -ForegroundColor Green
    }
    else {
        Write-Host 'Running Publish-Module ... -WhatIf' -ForegroundColor Cyan
        Publish-Module `
            -Path $buildVars.ModulePublishDir `
            -Repository $PSRepositoryName `
            -NuGetApiKey $ApiKey `
            -Verbose `
            -WhatIf

        if ($PSCmdlet.ShouldContinue("Publish module '$($buildVars.ModulePublishDir)' to repository '$PSRepositoryName'?", "Confirm Publish")) {
            Publish-Module `
                -Path $buildVars.ModulePublishDir `
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
