module Tempo.Dom.Helper

open Browser.Types

let remove (n: Node): unit =
    n.parentElement.removeChild n |> ignore