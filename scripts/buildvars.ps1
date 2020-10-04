$moduleName = 'MeasureSize'
$workspaceDir = Resolve-Path (Join-Path $PSScriptRoot '..')

[pscustomobject]@{
    ModuleName = $moduleName
    WorkspaceDir = $workspaceDir
    DocsDir = Join-Path $workspaceDir 'docs'
    ProjectPath = Join-Path $workspaceDir 'src' "$moduleName.fsproj"
    RootPublishDir = Join-Path $workspaceDir '.publish'
    ModulePublishDir = Join-Path $workspaceDir '.publish' $moduleName
}
