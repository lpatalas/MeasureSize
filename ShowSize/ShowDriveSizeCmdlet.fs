namespace ShowSizeModule

open System.IO
open System.Management.Automation

type DriveSize(driveInfo: DriveInfo) =
    member val Drive = driveInfo
    member val UsedSpace = FormattableSize (driveInfo.TotalSize - driveInfo.TotalFreeSpace)
    member val TotalSize = FormattableSize driveInfo.TotalSize

    member this.UsedPercentage =
        this.UsedSpace.ToDecimal() / this.TotalSize.ToDecimal()

[<Cmdlet(VerbsCommon.Show, "DriveSize")>]
type ShowDriveSizeCmdlet() =
    inherit PSCmdlet()

    override this.BeginProcessing() =
        DriveInfo.GetDrives()
        |> Seq.map DriveSize
        |> Seq.iter this.WriteObject
