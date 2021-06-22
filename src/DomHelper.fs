module Tempo.Dom.Helper

open Fable.Core
open Browser.Types

let remove (n: Node) : unit = n.parentElement.removeChild n |> ignore

[<Emit("$0.ownerDocument || document")>]
let ownerOrDocument (el: HTMLElement) : Document = jsNative
