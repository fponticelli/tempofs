namespace Tempo.Demo.Utils

open Fable.Core
open Fable.Core.JsInterop
open Browser.Dom

module Monaco =
    [<Import("*", from = "monaco-editor")>]
    let monaco : obj = jsNative

    let a () =
        console.log (monaco)
        1
