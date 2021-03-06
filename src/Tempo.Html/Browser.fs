namespace Tempo

open Fable.Core
open Browser.Types
open Browser.Dom

module Browser =
    [<Emit("$0 == null")>]
    let isNullOrUndefined<'T> (v: 'T) : bool = jsNative

    [<Emit("$0 != null")>]
    let exists<'T> (v: 'T) : bool = jsNative

    [<Emit("$0[$1] = $2")>]
    let assign<'X, 'T> (target: 'X) (prop: string) (v: 'T) : unit = jsNative

    [<Emit("$0[$1] = $2")>]
    let setProperty<'X, 'T> (target: 'X, prop: string, v: 'T) : unit = jsNative

    [<Emit("delete $0[$1]")>]
    let deleteProperty<'X, 'T> (target: 'X, prop: string) : unit = jsNative

    let setPropertyOption<'X, 'T> (target: 'X, prop: string, v: 'T option) : unit =
        match v with
        | Some v -> setProperty (target, prop, v)
        | None -> deleteProperty (target, prop)

    let setAttributeOption<'T> (target: Element, attr: string, v: string option) : unit =
        match v with
        | Some v -> target.setAttribute (attr, v)
        | None -> target.removeAttribute (attr)

    [<Emit("$0.style[$1] = $2")>]
    let setStyle (target: Element, prop: string, v: string) : unit = jsNative

    [<Emit("delete $0.style[$1]")>]
    let deleteStyle (target: Element, prop: string) : unit = jsNative

    let setStyleOption (target: Element, prop: string, v: string option) : unit =
        match v with
        | Some v -> setStyle (target, prop, v)
        | None -> deleteStyle (target, prop)

    [<Emit("$0[$1] !== null")>]
    let hasProperty<'X> (target: 'X, prop: string) : bool = jsNative

    [<Emit("$0[$1]")>]
    let getProperty<'X, 'Y> (target: 'X, prop: string) : 'Y option = jsNative

    [<Emit("$0[$1]")>]
    let getKnownProperty<'X, 'Y> (target: 'X, prop: string) : 'Y = jsNative

    [<Emit("$0 instanceof HTMLElement")>]
    let isHTMLElement<'X> (target: 'X) : bool = jsNative

    [<Emit("$0")>]
    let unsafeCast<'T, 'F> (target: 'F) : 'T = jsNative

    [<Emit("Array.prototype.slice.call($0)")>]
    let nodeListToArray<'T when 'T :> Element> (nl: NodeListOf<'T>) : 'T array = jsNative

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

    let collectElementAndAncestors (el: Element) : Element list =
        let rec go (el: Element) acc =
            if isNull el then
                acc
            else
                go el.parentElement <| el :: acc

        go el []

    [<Emit("btoa($0)")>]
    let toBase64String (s: string) : string = jsNative

    [<Emit("atob($0)")>]
    let fromBase64String (s: string) : string = jsNative

    let rec hasSpecifiedAncestor (element: Element) (ancestor: Element) : bool =
        if isNull element then
            false
        else if element = ancestor then
            true
        else
            hasSpecifiedAncestor element.parentElement ancestor

    let rec targetHasSpecifiedAncestor (target: EventTarget) (ancestor: Element) : bool =
        if hasProperty (target, "tagName") then
            hasSpecifiedAncestor (target :?> Element) ancestor
        else
            false

    [<Emit("new Event($0)")>]
    let rec createEvent (name: string) : Browser.Types.Event = jsNative

    [<Emit("$0")>]
    let optionToMaybe<'T> (v: 'T option) : 'T = jsNative

    [<Emit("performance.now")>]
    let performanceNow: unit -> float = jsNative

    let focusableSelector =
        String.concat
            ","
            [ "[contentEditable=true]"
              "[tabindex]"
              "a[href]"
              "area[href]"
              "button:not([disabled])"
              "iframe"
              "input:not([disabled])"
              "select:not([disabled])"
              "textarea:not([disabled])" ]

    let getFocusable (container: Element) : Element array =
        nodeListToArray (container.querySelectorAll focusableSelector)

    let isMac =
        getKnownProperty (getKnownProperty (window, "navigator"), "platform") = "MacIntel"
