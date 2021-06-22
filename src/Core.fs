module Tempo.Core

type Value<'S, 'V> =
    | Literal of 'V
    | Derived of ('S -> 'V)
    static member Val<'V>(v: 'V) = Literal v
    static member Val<'S, 'V>(f: 'S -> 'V) = Derived f
    static member Val<'S>() = Derived id<'S>

    static member Resolve (v: Value<'S, 'V>) (s: 'S) =
        match v with
        | Literal v -> v
        | Derived f -> f s

    static member Map m v =
        match v with
        | Literal v -> Literal <| m v
        | Derived f -> Derived(f >> m)

type Template<'N, 'I, 'S, 'A, 'Q> =
    | Node of 'N
    | Fragment of Template<'N, 'I, 'S, 'A, 'Q> list
    | MapState of IMapState<'N, 'I, 'S, 'A, 'Q>

// | MapAction
// MapImpl
// BindState
// Query
// Lifecycle
// Capture capture/release state
// Sequence
// Component hold state, has update cycle
// Adapter???
// Lazy???
// When???/If/Else

and ComponentView<'I, 'S, 'A, 'Q> =
    { Impl: 'I option
      Dispatch: 'A -> unit
      Change: 'S -> unit
      Destroy: unit -> unit
      Query: 'Q -> unit }

and View<'I, 'S, 'Q> =
    { Impl: 'I option
      Change: 'S -> unit
      Destroy: unit -> unit
      Query: 'Q -> unit }

and IMapState<'N, 'I, 'S, 'A, 'Q> =
    abstract Accept : IMapStateInvoker<'N, 'I, 'S, 'A, 'Q, 'R> -> 'R

and MapState<'N, 'I, 'S, 'S2, 'A, 'Q>(m, t, mrn) =
    member this.MapF : 'S -> 'S2 = m
    member this.Template : Template<'N, 'I, 'S2, 'A, 'Q> = t
    member this.MakeRenderNode : MakeNodeRender<'N, 'I, 'S2, 'Q> = mrn
    with
        interface IMapState<'N, 'I, 'S, 'A, 'Q> with
            member this.Accept f = f.Invoke<'S2> this

and IMapStateInvoker<'N, 'I, 'S, 'A, 'Q, 'R> =
    abstract Invoke<'S2> : MapState<'N, 'I, 'S, 'S2, 'A, 'Q> -> 'R

and Render<'I, 'S, 'Q> = 'I -> 'S -> View<'I, 'S, 'Q>

and MakeNodeRender<'N, 'I, 'S, 'Q> = 'N -> Render<'I, 'S, 'Q>

let packMapState (mapState: MapState<'N, 'I, 'S, 'S2, 'A, 'Q>) =
    mapState :> IMapState<'N, 'I, 'S, 'A, 'Q>

let unpackMapState (mapState: IMapState<'N, 'I, 'S, 'A, 'Q>) (f: IMapStateInvoker<'N, 'I, 'S, 'A, 'Q, 'R>) : 'R =
    mapState.Accept f

let makeMapState<'N, 'I, 'S, 'S2, 'A, 'Q>
    (makeNodeRender: MakeNodeRender<'N, 'I, 'S2, 'Q>)
    (f: 'S -> 'S2)
    (t: Template<'N, 'I, 'S2, 'A, 'Q>)
    =
    packMapState
    <| MapState<'N, 'I, 'S, 'S2, 'A, 'Q>(f, t, makeNodeRender)

let rec makeRender<'N, 'I, 'S, 'A, 'Q>
    (makeNodeRender: MakeNodeRender<'N, 'I, 'S, 'Q>)
    (template: Template<'N, 'I, 'S, 'A, 'Q>)
    : Render<'I, 'S, 'Q> =
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

and makeRenderMapState<'N, 'I, 'S1, 'S2, 'A, 'Q> (mapState: IMapState<'N, 'I, 'S1, 'A, 'Q>) : Render<'I, 'S1, 'Q> =
    unpackMapState
        mapState
        { new IMapStateInvoker<'N, 'I, 'S1, 'A, 'Q, Render<'I, 'S1, 'Q>> with
            member __.Invoke<'S2>(mapState: MapState<'N, 'I, 'S1, 'S2, 'A, 'Q>) : Render<'I, 'S1, 'Q> =
                let t = mapState.Template

                let render =
                    makeRender<'N, 'I, 'S2, 'A, 'Q> mapState.MakeRenderNode t

                fun (i: 'I) (s: 'S1) ->
                    let view = render i (mapState.MapF s)

                    { Impl = view.Impl
                      Query = view.Query
                      Destroy = view.Destroy
                      Change = fun s1 -> view.Change <| mapState.MapF s1 } }
