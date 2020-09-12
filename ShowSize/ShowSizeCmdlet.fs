namespace ShowSizeModule

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
            this.ProcessPipelineInput(this.InputObject)
        else
            this.ProcessPath(this.Path)

    member private this.ProcessPipelineInput(input: obj) =
        let maybeFileSystemInfo = 
            match (PSUtils.unwrapPSObject input) with
            | :? FileSystemInfo as fsi -> Ok fsi
            | :? string as str -> Ok (FileSystem.getFileSystemInfoForPath str)
            | _ -> Error (sprintf "Input is neither FileSystemInfo nor String: %O" input)

        match maybeFileSystemInfo with
        | Ok fileSystemInfo ->
            let size = FileSystem.calculateSize fileSystemInfo
            this.WriteObject(ItemSize(fileSystemInfo, size))
        | Error message ->
            this.WriteWarning(message)

    member private this.ProcessPath(path: string) =
        let (resolvedPaths, _) = this.SessionState.Path.GetResolvedProviderPathFromPSPath(this.Path)

        let items =
            if resolvedPaths.Count = 1 then
                let itemInfo = FileSystem.getFileSystemInfoForPath resolvedPaths.[0]
                match FileSystem.getFileSystemInfoKind itemInfo with
                | Directory directoryInfo -> directoryInfo.EnumerateFileSystemInfos()
                | File fileInfo -> seq { fileInfo :> FileSystemInfo }
            else
                resolvedPaths
                |> Seq.map FileSystem.getFileSystemInfoForPath

        let getItemSize info =
            ItemSize(info, FileSystem.calculateSize info)

        items
        |> Seq.map getItemSize
        |> Seq.iter this.WriteObject