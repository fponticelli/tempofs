namespace Tempo.Html.UI

open Fable.Core
open Browser.Types

[<Erase; RequireQualifiedAccess>]
module JSe =
    [<Erase>]
    type ResizeObserverEntry =
        [<Emit("$0.contentRect")>]
        member _.contentRect : ClientRect = jsNative

    [<Erase>]
    type ResizeObserver private () =
        [<Emit("new ResizeObserver($0, {root: $1, rootMargin: $2, threshold: $3})")>]
        new(callback: ResizeObserverEntry [] -> ResizeObserver -> unit) = ResizeObserver()

        [<Emit("$0.disconnect()")>]
        member _.disconnect() : unit = jsNative

        [<Emit("$0.observe($1)")>]
        member _.observe(element: #Element) : unit = jsNative

        [<Emit("$0.unobserve($1)")>]
        member _.unobserve(element: #Element) : unit = jsNative
