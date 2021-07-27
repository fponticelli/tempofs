namespace Tempo

open Tempo.Std
open Tempo.Std.Function

module View =
    type Dispatch<'A> = 'A -> unit

    type View<'S, 'Q> =
        { Change: option<'S -> unit>
          Destroy: option<unit -> unit>
          Request: option<'Q -> unit> }

    and ComponentView<'S, 'A, 'Q> =
        { Change: option<'S -> unit>
          Destroy: option<unit -> unit>
          Request: option<'Q -> unit>
          Dispatch: Dispatch<'A> }

    let empty =
        { Change = None
          Destroy = None
          Request = None }

    let mergeViews<'S, 'Q> (ls: View<'S, 'Q> list) : View<'S, 'Q> =
        List.fold
            (fun (a: View<'S, 'Q>) (b: View<'S, 'Q>) ->
                { Change = Option.merge (fun a b -> F.MergeEffects(a, b)) a.Change b.Change
                  Request = Option.merge (fun a b -> F.MergeEffects(a, b)) a.Request b.Request
                  Destroy = Option.merge (fun a b -> F.MergeEffects(a, b)) a.Destroy b.Destroy })
            empty
            ls

    let mergeChange<'S, 'Q> (change: 'S -> unit, view: View<'S, 'Q>) : View<'S, 'Q> =
        mergeViews (
            [ { Change = Some change
                Request = None
                Destroy = None }
              view ]
        )
