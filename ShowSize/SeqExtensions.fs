namespace ShowSizeModule

module Seq =
    let chooseCollect func seq =
        seq
        |> Seq.choose func
        |> Seq.collect id
