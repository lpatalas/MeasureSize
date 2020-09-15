namespace ShowSizeModule

open System
open System.Collections.Generic
open System.IO
open System.Management.Automation

type ItemSize(item: FileSystemInfo, size: int64) =
    member val Item = item
    member val Size = FormattableSize size

[<Cmdlet(VerbsCommon.Show, "Size")>]
type ShowSizeCmdlet() =
    inherit PSCmdlet()

    [<Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)>]
    member val Path: string = "" with get, set

    override this.ProcessRecord() =
        let getItemSize info =
            ItemSize(info, FileSystem.calculateSize info)

        let actualPath =
            if String.IsNullOrEmpty(this.Path) then
                "*"
            else
                this.Path

        let (resolvedPaths, _) = this.SessionState.Path.GetResolvedProviderPathFromPSPath(actualPath)

        resolvedPaths
        |> Seq.map FileSystem.getFileSystemInfoForPath
        |> Seq.map getItemSize
        |> Seq.iter this.WriteObject
