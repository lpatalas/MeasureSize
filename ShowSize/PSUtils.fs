module ShowSizeModule.PSUtils

open System.Management.Automation

let unwrapPSObject (input: obj) =
    match input with
    | :? PSObject as psObj -> psObj.BaseObject
    | _ -> input
