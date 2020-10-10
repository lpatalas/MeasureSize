#Requires -PSEdition Core,Desktop -Module Pester
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

BeforeAll {
    $tempDir = Join-Path $PSScriptRoot 'temp'

    if (Test-Path $tempDir) {
        Remove-Item -Path $tempDir -Force -Recurse
    }

    function EnsureParentExists($FilePath) {
        $parentPath = Split-Path $FilePath
        if ($parentPath -and (-not (Test-Path $parentPath))) {
            New-Item `
                -Path $parentPath `
                -Force `
                -ItemType Directory `
            | Out-Null
        }
    }

    function CreateTestFile($RelativePath, $SizeInBytes) {
        $fullPath = Join-Path $tempDir $RelativePath
        EnsureParentExists $fullPath
        $content = 'X' * $SizeInBytes
        Set-Content -Path $fullPath -Value $content -Encoding ascii -NoNewline
        Get-Item -LiteralPath $fullPath
    }
}

Describe 'Measure-ItemSize' {
    It 'should return correct size for specific file' {
        $testFile = CreateTestFile 'test.txt' 128
        $result = Measure-ItemSize $testFile
        $result.Item.FullName | Should -Be $testFile.FullName
        $result.Size.ToString() | Should -Be "$($testFile.Length)B"
    }

    It 'should return correct folder size' {
        $testFiles = @(
            CreateTestFile 'X\A.txt' 1
            CreateTestFile 'X\B.txt' 2
            CreateTestFile 'X\Y\C.txt' 3
            CreateTestFile 'X\Y\Z\D.txt' 4
        )
        $testFolderPath = $testFiles[0].Directory.FullName
        $result = Measure-ItemSize $testFolderPath
        $result.Size.ToString() | Should -Be '10B'
    }

    It 'should return error if input path is not valid' {
        $path = Join-Path $PSScriptRoot 'missing-file.txt'
        $result = @(Measure-ItemSize -Path $path -ErrorAction SilentlyContinue)
        $result | Should -HaveCount 0
        $Error[0].Exception | Should -BeOfType System.Management.Automation.ItemNotFoundException
    }

    It 'should return error if path is using provider other than FileSystem' {
        $path = 'Variable:\Error'
        $result = @(Measure-ItemSize -Path $path -ErrorAction SilentlyContinue)
        $result | Should -HaveCount 0
        $Error[0].Exception | Should -BeOfType System.NotSupportedException
    }
}
