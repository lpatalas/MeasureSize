namespace MeasureSizeModule

open System.IO

type FileSystemInfoKind =
    | Directory of DirectoryInfo
    | File of FileInfo

type ItemSize(item: FileSystemInfo, size: int64) =
    member val Item = item
    member val Size = FormattableSize size

type DriveSize(driveInfo: DriveInfo) =
    member val Drive = driveInfo
    member val UsedSpace = FormattableSize (driveInfo.TotalSize - driveInfo.TotalFreeSpace)
    member val FreeSpace = FormattableSize driveInfo.TotalFreeSpace
    member val TotalSize = FormattableSize driveInfo.TotalSize

    member this.UsedPercentage =
        this.UsedSpace.ToDecimal() / this.TotalSize.ToDecimal()

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
                |> Seq.map calculateSize

            fileSizes |> Seq.append dirSizes |> Seq.sum

        match getFileSystemInfoKind fileSystemInfo with
        | File fileInfo ->
            fileInfo.Length
        | Directory dirInfo ->
            calculateDirectorySize dirInfo

    let getItemSize path =
        let info = getFileSystemInfoForPath path
        ItemSize(info, calculateSize info)

