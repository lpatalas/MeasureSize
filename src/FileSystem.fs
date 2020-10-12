namespace MeasureSizeModule

open System
open System.IO

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
    let (|Directory|File|) (path: string) =
        let attrs = File.GetAttributes path
        if attrs.HasFlag(FileAttributes.Directory) then
            Directory (DirectoryInfo path)
        else
            File (FileInfo path)

    let calculateSize path =
        let calculateFileSize (fileInfo: FileInfo) =
            try
                fileInfo.Length
            with
            | :? UnauthorizedAccessException -> 0L

        let rec calculateDirectorySize (directoryInfo: DirectoryInfo) =
            try
                let fileSizes =
                    directoryInfo.EnumerateFiles()
                    |> Seq.map calculateFileSize

                let dirSizes =
                    directoryInfo.EnumerateDirectories()
                    |> Seq.map calculateDirectorySize

                fileSizes |> Seq.append dirSizes |> Seq.sum
            with
            | :? UnauthorizedAccessException -> 0L

        match path with
        | File fileInfo -> ItemSize(fileInfo, calculateFileSize fileInfo)
        | Directory dirInfo -> ItemSize(dirInfo, calculateDirectorySize dirInfo)
