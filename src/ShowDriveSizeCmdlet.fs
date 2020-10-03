namespace ShowSizeModule

open System.IO
open System.Management.Automation

[<Cmdlet(VerbsCommon.Show, "DriveSize")>]
type ShowDriveSizeCmdlet() =
    inherit PSCmdlet()

    override this.BeginProcessing() =
        DriveInfo.GetDrives()
        |> Seq.map DriveSize
        |> Seq.iter this.WriteObject
