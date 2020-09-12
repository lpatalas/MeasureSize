namespace ShowSizeModule

open System.Collections.Generic
open System.IO
open System.Management.Automation

type ItemSize(item: FileSystemInfo, size: int64) =
    member val Item = item
    member val Size = FormattableSize size

[<Cmdlet(VerbsCommon.Show, "Size")>]
type ShowSizeCmdlet() =
    inherit PSCmdlet()

    [<Parameter(Mandatory = true, ParameterSetName = "Input", ValueFromPipeline = true)>]
    member val InputObject: obj = null with get, set

    [<Parameter(Mandatory = true, ParameterSetName = "Path", Position = 0)>]
    member val Path: string = "" with get, set

    override this.ProcessRecord() = 
        if this.ParameterSetName = "Input" then
            let fileSystemInfo = FileSystem.getFileSystemInfo this.InputObject
            let size = FileSystem.calculateSize fileSystemInfo
            this.WriteObject(ItemSize(fileSystemInfo, size))
        else
            let (resolvedPaths, _) = this.SessionState.Path.GetResolvedProviderPathFromPSPath(this.Path)

            let items =
                if resolvedPaths.Count = 1 then
                    let itemInfo = FileSystem.getFileSystemInfoForPath resolvedPaths.[0]
                    match itemInfo with
                    | :? FileInfo as fileInfo -> seq { fileInfo :> FileSystemInfo }
                    | :? DirectoryInfo as directoryInfo -> directoryInfo.EnumerateFileSystemInfos()
                    | _ -> invalidOp "FileSystemInfo is neither FileInfo nor DirectoryInfo"
                else
                    resolvedPaths
                    |> Seq.map FileSystem.getFileSystemInfoForPath

            items
            |> Seq.map (fun info -> ItemSize(info, FileSystem.calculateSize info))
            |> Seq.iter this.WriteObject