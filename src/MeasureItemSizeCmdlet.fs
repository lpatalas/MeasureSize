namespace MeasureSizeModule

open System
open System.IO
open System.Management.Automation

type ResolvedPath =
| ValidPath of string
| InvalidPath of ItemNotFoundException
| UnsupportedProvider of string

[<Cmdlet(VerbsDiagnostic.Measure, "ItemSize")>]
[<OutputType(typeof<ItemSize>)>]
type MeasureItemSizeCmdlet() =
    inherit PSCmdlet()

    let mapResolvedPath mapping resolvedPath =
        match resolvedPath with
        | ValidPath path -> ValidPath (mapping path)
        | _ -> resolvedPath

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
        |> Seq.collect this.TryResolvePath
        |> Seq.iter this.ProcessPath

    member private this.TryResolvePath path =
        try
            let (resolvedPaths, provider) =
                this.SessionState.Path.GetResolvedProviderPathFromPSPath path

            if String.Equals(provider.Name, "FileSystem", StringComparison.Ordinal) then
                resolvedPaths
                |> Seq.map ValidPath
            else
                seq { UnsupportedProvider provider.Name }
        with
        :? ItemNotFoundException as ex -> seq { InvalidPath ex }

    member private this.ProcessPath resolvedPath =
        match resolvedPath with
        | ValidPath path ->
            path
            |> FileSystem.getItemSize
            |> this.WriteObject
        | InvalidPath ex ->
            this.WriteError(
                ErrorRecord (
                    ex,
                    "Path is invalid",
                    ErrorCategory.InvalidArgument,
                    this))
        | UnsupportedProvider providerName ->
            this.WriteError(
                ErrorRecord (
                    NotSupportedException(sprintf "Provider not supported: %s" providerName),
                    "Path is invalid",
                    ErrorCategory.InvalidArgument,
                    this))
