namespace Tempo.Html

open Fable.Core
open Browser.Types

module Tools =
    [<Emit("$0 == null")>]
    let nullOrUndefined<'T> (v: 'T) : bool = jsNative

    [<Emit("$0 != null")>]
    let exists<'T> (v: 'T) : bool = jsNative

    [<Emit("$0[$1] = $2")>]
    let assign<'X, 'T> (target: 'X) (prop: string) (v: 'T) : unit = jsNative

    [<Emit("$0[$1] !== null")>]
    let hasProperty<'X> (target: 'X, prop: string) : bool = jsNative

    [<Emit("$0 instanceof HTMLElement")>]
    let isHTMLElement<'X> (target: 'X) : bool = jsNative

    [<Emit("$0")>]
    let unsafeCast<'T, 'F> (target: 'F) : 'T = jsNative

    let remove (n: Node) : unit =
        if isHTMLElement n then
            let el = n :?> HTMLElement

            if exists el && exists el.blur then
                assign el "onblur" null

        if exists n
           && exists n.ownerDocument
           && exists n.parentNode then
            n.parentNode.removeChild n |> ignore

    [<Emit("$0.ownerDocument || document")>]
    let ownerOrDocument (n: Node) : Document = jsNative
