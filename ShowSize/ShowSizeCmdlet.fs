namespace ShowSizeModule

open System
open System.IO
open System.Management.Automation

type ItemSize(item: FileSystemInfo, size: int64) =
    member val Item = item
    member val Size = FormattableSize size

[<Cmdlet(VerbsCommon.Show, "Size")>]
type ShowSizeCmdlet() =
    inherit PSCmdlet()

    [<Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)>]
    member val Path = "" with get, set

    override this.ProcessRecord() =
        let itemPathOrDefault =
            if String.IsNullOrEmpty(this.Path) then
                "*"
            else
                this.Path

        let getItemSize info =
            ItemSize(info, FileSystem.calculateSize info)

        let getItemSizeFromPath =
            FileSystem.getFileSystemInfoForPath >> getItemSize

        let (resolvedPaths, provider) =
            this.SessionState.Path.GetResolvedProviderPathFromPSPath(itemPathOrDefault)

        if String.Equals(provider.Name, "FileSystem", StringComparison.Ordinal) then
            resolvedPaths
            |> Seq.map getItemSizeFromPath
            |> Seq.iter this.WriteObject
