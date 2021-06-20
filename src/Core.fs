module Tempo.Core

type Value<'V, 'S> =
    | Literal of 'V
    | Derived of ('S -> 'V)

type MapStateValue<'S1, 'S2, 'N when 'S1: equality and 'S2: equality> = ('S1 -> 'S2) * Template<'N, 'S2>

and MapStateApply =
    abstract member Apply<'S1, 'S2, 'N when 'S1: equality and 'S2: equality> : MapStateEvaluator<'S1> -> MapStateValue<'S1, 'S2, 'N>

and MapStateEvaluator<'S1 when 'S1: equality> =
    abstract member Eval : unit -> MapStateValue<'S1, 'S2, 'N>

and EmbedEvaluator<'N1, 'S when 'S: equality> =
    abstract member Eval<'N2, 'I1, 'I2> : unit -> (RenderNode<'N2, 'I2, 'S> * ('I1 -> 'I2) * Template<'N2, 'S>)

and Template<'N, 'S when 'S: equality> =
    | Node of 'N
    | Fragment of Template<'N, 'S> list
    | MapState of MapStateEvaluator<'S>
    | Embed of EmbedEvaluator<'N, 'S>
    // BindState
    // Sequence
    // Query
    // Lifecycle
    // MapImpl
    // MapAction

and RenderNode<'N, 'I, 'S when 'S: equality> = 'N -> 'I -> 'S -> View<'S, 'I>

and Render<'N, 'I, 'S when 'S: equality> = RenderNode<'N, 'I, 'S> -> Template<'N, 'S> -> 'I -> 'S -> View<'S, 'I>

and View<'S, 'I> =
    { Impl: 'I
      Change: 'S -> unit
      Destroy: unit -> unit }


// let inline makeMapState2 (value: MapStateValue< 'S1, 'S2, 'N>) : MapStateEvaluator<'S1> =
//     { new MapStateEvaluator< 'S1> with
//         override __.Eval<'S2, 'N when 'S2 : equality> (): MapStateValue<'S1, 'S2, 'N> = value //:> MapStateValue<'S1, 'a, 'b> 
//         }


// let inline makeMapState (value: MapStateValue< 'S1, 'S2, 'N>) : MapStateEvaluator< 'S1> =
//     { new MapStateEvaluator< 'S1> with
//         member __.Eval () = value }

// module MapStateOps =
//     let make (l : 'a list) : MapStateApply =
//         { new MapStateApply with
//             member __.Apply e = e.Eval ()
//         }

//     let ret<'S1, 'S2, 'N when 'S1 : equality and 'S2: equality> (v: MapStateValue<'S1, 'S2, 'N>) (applier : MapStateApply) : MapStateValue<'S1, 'S2, 'N> =
//         applier.Apply { new MapStateEvaluator<'S1> with
//             override this.Eval(): MapStateValue<'S1,'S2,'N> = v
//         }

let resolve<'V, 'S> (value: Value<'V, 'S>) (state: 'S) =
    match value with
    | Literal v -> v
    | Derived f -> f state

let rec render<'N, 'I, 'S when 'S: equality>
    (renderNode: RenderNode<'N, 'I, 'S>)
    (template: Template<'N, 'S>)
    (impl: 'I)
    (state: 'S)
    : View<'S, 'I> =
    match template with
    | Node n -> renderNode n impl state
    | Fragment ls ->
        let ls =
            List.map (fun i -> render renderNode i impl state) ls

        { Impl = impl
          Change = fun state -> List.iter (fun i -> i.Change state) ls
          Destroy = fun () -> List.iter (fun i -> i.Destroy()) ls }
    | MapState ev ->
        let (map, template) = ev.Eval()
        render renderNode template impl <| map state
    | Embed ev ->
        let (renderEmbedded, attach, template) = ev.Eval()
        let implEmbedded = attach impl
        render renderEmbedded template implEmbedded state
