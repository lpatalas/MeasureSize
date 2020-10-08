namespace MeasureSizeModule

open System
open System.Management.Automation

[<Cmdlet(VerbsDiagnostic.Measure, "ItemSize")>]
[<OutputType(typeof<ItemSize>)>]
type MeasureItemSizeCmdlet() =
    inherit PSCmdlet()

    [<Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)>]
    member val Path: string[] = Array.empty with get, set

    override this.ProcessRecord() =
        let pathOrDefault =
            if Array.isEmpty this.Path then
                [| "*" |]
            else
                this.Path

        pathOrDefault
        |> Seq.distinct
        |> Seq.chooseCollect this.TryResolvePath
        |> Seq.map FileSystem.getItemSize
        |> Seq.iter this.WriteObject

    member private this.TryResolvePath path =
        let (resolvedPaths, provider) =
            this.SessionState.Path.GetResolvedProviderPathFromPSPath path

        if String.Equals(provider.Name, "FileSystem", StringComparison.Ordinal) then
            Some resolvedPaths
        else
            None
