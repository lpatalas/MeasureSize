namespace ShowSizeModule

open System.Collections.Generic
open System.IO

type FileSystemInfoKind =
    | Directory of DirectoryInfo
    | File of FileInfo

module FileSystem =
    let getFileSystemInfoKind (input: FileSystemInfo) =
        match input with
        | :? DirectoryInfo as directoryInfo -> Directory directoryInfo
        | :? FileInfo as fileInfo -> File fileInfo
        | _ -> invalidOp "FileSystemInfo is neither FileInfo nor DirectoryInfo"

    let getFileSystemInfoForPath path =
        let attrs = File.GetAttributes path
        if attrs.HasFlag(FileAttributes.Directory) then
            DirectoryInfo path :> FileSystemInfo
        else
            FileInfo path :> FileSystemInfo
        
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
                    match getFileSystemInfoKind fileSystemInfo with
                    | File fileInfo ->
                        fileInfo.Length
                    | Directory dirInfo ->
                        calculateDirectorySize (calculateSizeImpl sizeCache) dirInfo

                sizeCache.Add(fileSystemInfo.FullName, size) |> ignore
                size

        calculateSizeImpl (Dictionary()) fileSystemInfo
            
