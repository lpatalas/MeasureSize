namespace ShowSizeModule

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
        
    let rec calculateSize (fileSystemInfo: FileSystemInfo) =
        let calculateDirectorySize (directoryInfo: DirectoryInfo) =
            let fileSizes = 
                directoryInfo.EnumerateFiles()
                |> Seq.map (fun f -> f.Length)
            
            let dirSizes =
                directoryInfo.EnumerateDirectories()
                |> Seq.map (fun d -> calculateSize d)

            fileSizes |> Seq.append dirSizes |> Seq.sum
            
        match getFileSystemInfoKind fileSystemInfo with
        | File fileInfo ->
            fileInfo.Length
        | Directory dirInfo ->
            calculateDirectorySize dirInfo
            
