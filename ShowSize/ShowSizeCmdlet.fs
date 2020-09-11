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

    [<Parameter(Mandatory = true, ValueFromPipeline = true)>]
    member val InputObject: obj = null with get, set

    override this.ProcessRecord() = 
        let fileSystemInfo = FileSystem.getFileSystemInfo this.InputObject
        let size = FileSystem.calculateSize fileSystemInfo
        this.WriteObject(ItemSize(fileSystemInfo, size))