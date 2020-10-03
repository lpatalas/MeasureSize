namespace ShowSizeModule

open System.IO
open System.Management.Automation

[<Cmdlet(VerbsDiagnostic.Measure, "DriveSize")>]
type MeasureDriveSizeCmdlet() =
    inherit PSCmdlet()

    override this.BeginProcessing() =
        DriveInfo.GetDrives()
        |> Seq.map DriveSize
        |> Seq.iter this.WriteObject
