namespace ShowSizeModule

open System.Collections.Generic
open System.IO
open System.Management.Automation

module FileSystem =
    let unwrapPSObject (input: obj) =
        match input with
        | :? PSObject as psObj -> psObj.BaseObject
        | _ -> input

    let getFileSystemInfoForPath path =
        let attrs = File.GetAttributes path
        if attrs.HasFlag(FileAttributes.Directory) then
            DirectoryInfo path :> FileSystemInfo
        else
            FileInfo path :> FileSystemInfo

    let getFileSystemInfo input =
        match (unwrapPSObject input) with
        | :? FileSystemInfo as fsi -> fsi
        | :? string as str -> getFileSystemInfoForPath str
        | _ -> invalidOp "Input is neither FileSystemInfo nor String"
        
    let calculateSize (fileSystemInfo: FileSystemInfo) =
        let calculateDirectorySize calculateItemCallback (directoryInfo: DirectoryInfo) =
            let fileSizes = 
                directoryInfo.EnumerateFiles()
                |> Seq.map (fun f -> f.Length)
            
            let dirSizes =
                directoryInfo.EnumerateDirectories()
                |> Seq.map (fun d -> calculateItemCallback d)

            fileSizes |> Seq.append dirSizes |> Seq.sum
            
        let rec calculateSizeImpl (sizeCache: IDictionary<string, int64>) (fileSystemInfo: FileSystemInfo) =
            match sizeCache.TryGetValue(fileSystemInfo.FullName) with
            | (true, size) ->
                size
            | (false, _) ->
                let size =
                    match fileSystemInfo with
                    | :? FileInfo as fileInfo ->
                        fileInfo.Length
                    | :? DirectoryInfo as dirInfo ->
                        calculateDirectorySize (calculateSizeImpl sizeCache) dirInfo
                    | _ ->
                        invalidOp "FileSystemInfo is neither FileInfo nor DirectoryInfo"

                sizeCache.Add(fileSystemInfo.FullName, size) |> ignore
                size

        calculateSizeImpl (Dictionary()) fileSystemInfo
            
