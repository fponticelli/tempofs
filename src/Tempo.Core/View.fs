namespace Tempo

open Tempo.Std
open Tempo.Std.Function

module View =
    type View2<'S, 'Q> =
        { Change: option<'S -> unit>
          Destroy: option<unit -> unit>
          Request: option<'Q -> unit> }

    and ComponentView2<'S, 'A, 'Q> =
        { Change: option<'S -> unit>
          Destroy: option<unit -> unit>
          Request: option<'Q -> unit>
          Dispatch: 'A -> unit }

    let empty =
        { Change = None
          Destroy = None
          Request = None }

    let mergeViews<'S, 'Q> (ls: View2<'S, 'Q> list) : View2<'S, 'Q> =
        List.fold
            (fun (a: View2<'S, 'Q>) (b: View2<'S, 'Q>) ->
                { Change = Option.merge (fun a b -> F.MergeEffects(a, b)) a.Change b.Change
                  Request = Option.merge (fun a b -> F.MergeEffects(a, b)) a.Request b.Request
                  Destroy = Option.merge (fun a b -> F.MergeEffects(a, b)) a.Destroy b.Destroy })
            empty
            ls
