module Tempo.Core

type Value<'V, 'S> =
    | Literal of 'V
    | Derived of ('S -> 'V)


module Value =
    let resolve<'V, 'S> (value: Value<'V, 'S>) (state: 'S) =
        match value with
        | Literal v -> v
        | Derived f -> f state

    let literal v = Literal v
    let derived f = Derived f
    let ofState = Derived id

type Template<'N, 'S, 'A, 'Q> =
    | Node of 'N
    | Fragment of Template<'N, 'S, 'A, 'Q> list
    | MapState of IMapState<'N, 'S, 'A, 'Q>

and View<'S, 'Q, 'I> =
    { Impl: 'I option
      Change: 'S -> unit
      Destroy: unit -> unit
      Query: 'Q -> unit }

and IMapState<'N, 'S, 'A, 'Q> =
    abstract Accept : IMapStateInvoker<'N, 'S, 'A, 'Q, 'R> -> 'R

and MapState<'N, 'S, 'S2, 'A, 'Q>(m, t, mrn) =
    member this.MapF : 'S -> 'S2 = m
    member this.Template : Template<'N, 'S2, 'A, 'Q> = t
    member this.MakeRenderNode : MakeNodeRender<'N, 'S2, 'Q, 'I> = mrn
    with
        interface IMapState<'N, 'S, 'A, 'Q> with
            member this.Accept f = f.Invoke<'S2> this

and IMapStateInvoker<'N, 'S, 'A, 'Q, 'R> =
    abstract Invoke<'S2> : MapState<'N, 'S, 'S2, 'A, 'Q> -> 'R

and Render<'S, 'Q, 'I> = 'I -> 'S -> View<'S, 'Q, 'I>

and MakeNodeRender<'N, 'S, 'Q, 'I> = 'N -> Render<'S, 'Q, 'I>

let packMapState (mapState: MapState<'N, 'S, 'S2, 'A, 'Q>) = mapState :> IMapState<'N, 'S, 'A, 'Q>

let unpackMapState (mapState: IMapState<'N, 'S, 'A, 'Q>) (f: IMapStateInvoker<'N, 'S, 'A, 'Q, 'R>) : 'R =
    mapState.Accept f

let makeMapState<'N, 'S, 'S2, 'A, 'Q>
    (makeNodeRender: MakeNodeRender<'N, 'S2, 'Q, 'I>)
    (f: 'S -> 'S2)
    (t: Template<'N, 'S2, 'A, 'Q>)
    =
    packMapState
    <| MapState<'N, 'S, 'S2, 'A, 'Q>(f, t, makeNodeRender)

let rec makeRender<'N, 'S, 'A, 'Q, 'I>
    (makeNodeRender: MakeNodeRender<'N, 'S, 'Q, 'I>)
    (template: Template<'N, 'S, 'A, 'Q>)
    : Render<'S, 'Q, 'I> =
    match template with
    | Node n -> makeNodeRender n
    | Fragment ls ->
        let fs = List.map (makeRender makeNodeRender) ls

        fun (i: 'I) (s: 'S) ->
            let views = List.map (fun f -> f i s) fs

            { Impl = None
              Change = fun s -> List.iter (fun i -> i.Change s) views
              Destroy = fun () -> List.iter (fun i -> i.Destroy()) views
              Query = fun q -> List.iter (fun i -> i.Query q) views }
    | MapState mapState -> makeRenderMapState mapState

and makeRenderMapState<'N, 'S1, 'S2, 'A, 'Q, 'I> (mapState: IMapState<'N, 'S1, 'A, 'Q>) : Render<'S1, 'Q, 'I> =
    unpackMapState
        mapState
        { new IMapStateInvoker<'N, 'S1, 'A, 'Q, Render<'S1, 'Q, 'I>> with
            member __.Invoke<'S2>(mapState: MapState<'N, 'S1, 'S2, 'A, 'Q>) : Render<'S1, 'Q, 'I> =
                let t = mapState.Template

                let render : Render<'S1, 'Q, 'I> =
                    (makeRender :?> Render<'S1, 'Q, 'I>) mapState.MakeRenderNode t

                fun (i: 'I) (s: 'S1) -> render i (mapState.MapF s) }

// type MapStateValue<'S1, 'S2, 'N when 'S1: equality and 'S2: equality> = ('S1 -> 'S2) * Template<'N, 'S2>

// and MapStateApply =
//     abstract member Apply<'S1, 'S2, 'N when 'S1: equality and 'S2: equality> : MapStateEvaluator<'S1> -> MapStateValue<'S1, 'S2, 'N>

// and MapStateEvaluator<'S1 when 'S1: equality> =
//     abstract member Eval : unit -> MapStateValue<'S1, 'S2, 'N>

// and EmbedEvaluator<'N1, 'S when 'S: equality> =
//     abstract member Eval<'N2, 'I1, 'I2> : unit -> (RenderNode<'N2, 'I2, 'S> * ('I1 -> 'I2) * Template<'N2, 'S>)

// and Template<'N, 'S when 'S: equality> =
//     | Node of 'N
//     | Fragment of Template<'N, 'S> list
//     | MapState of MapStateEvaluator<'S>
//     | Embed of EmbedEvaluator<'N, 'S>
//     // BindState
//     // Sequence
//     // Query
//     // Lifecycle
//     // MapImpl
//     // MapAction

// and RenderNode<'N, 'I, 'S when 'S: equality> = 'N -> 'I -> 'S -> View<'S, 'I>

// and Render<'N, 'I, 'S when 'S: equality> = RenderNode<'N, 'I, 'S> -> Template<'N, 'S> -> 'I -> 'S -> View<'S, 'I>

// and View<'S, 'I> =
//     { Impl: 'I
//       Change: 'S -> unit
//       Destroy: unit -> unit }


// // let inline makeMapState2 (value: MapStateValue< 'S1, 'S2, 'N>) : MapStateEvaluator<'S1> =
// //     { new MapStateEvaluator< 'S1> with
// //         override __.Eval<'S2, 'N when 'S2 : equality> (): MapStateValue<'S1, 'S2, 'N> = value //:> MapStateValue<'S1, 'a, 'b>
// //         }


// // let inline makeMapState (value: MapStateValue< 'S1, 'S2, 'N>) : MapStateEvaluator< 'S1> =
// //     { new MapStateEvaluator< 'S1> with
// //         member __.Eval () = value }

// // module MapStateOps =
// //     let make (l : 'a list) : MapStateApply =
// //         { new MapStateApply with
// //             member __.Apply e = e.Eval ()
// //         }

// //     let ret<'S1, 'S2, 'N when 'S1 : equality and 'S2: equality> (v: MapStateValue<'S1, 'S2, 'N>) (applier : MapStateApply) : MapStateValue<'S1, 'S2, 'N> =
// //         applier.Apply { new MapStateEvaluator<'S1> with
// //             override this.Eval(): MapStateValue<'S1,'S2,'N> = v
// //         }

// let rec render<'N, 'I, 'S when 'S: equality>
//     (renderNode: RenderNode<'N, 'I, 'S>)
//     (template: Template<'N, 'S>)
//     (impl: 'I)
//     (state: 'S)
//     : View<'S, 'I> =
//     match template with
//     | Node n -> renderNode n impl state
//     | Fragment ls ->
//         let ls =
//             List.map (fun i -> render renderNode i impl state) ls

//         { Impl = impl
//           Change = fun state -> List.iter (fun i -> i.Change state) ls
//           Destroy = fun () -> List.iter (fun i -> i.Destroy()) ls }
//     | MapState ev ->
//         let (map, template) = ev.Eval()
//         render renderNode template impl <| map state
//     | Embed ev ->
//         let (renderEmbedded, attach, template) = ev.Eval()
//         let implEmbedded = attach impl
//         render renderEmbedded template implEmbedded state
