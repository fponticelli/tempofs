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
    | OneOf2 of IOneOf2<'N, 'I, 'S, 'A, 'Q>

// BindState
// Lifecycle
// Capture capture/release state
// Sequence
// MapAction
// MapImpl
// MapQuery
// Adapter???
// Lazy???
// When???/If/Else

and ComponentView<'I, 'S, 'A, 'Q> =
    { Impl: 'I
      Dispatch: 'A -> unit
      Change: 'S -> unit
      Destroy: unit -> unit
      Query: 'Q -> unit }

and View<'I, 'S, 'Q> =
    { Impl: 'I
      Change: 'S -> unit
      Destroy: unit -> unit
      Query: 'Q -> unit }

and IMapState<'N, 'I, 'S, 'A, 'Q> =
    abstract Accept : IMapStateInvoker<'N, 'I, 'S, 'A, 'Q, 'R> -> 'R

and MapState<'N, 'N2, 'I, 'S, 'S2, 'A, 'Q>(m, t) =
    member this.MapF : 'S -> 'S2 = m
    member this.Template : Template<'N2, 'I, 'S2, 'A, 'Q> = t
    with
        interface IMapState<'N, 'I, 'S, 'A, 'Q> with
            member this.Accept f = f.Invoke<'N2, 'S2> this

and IMapStateInvoker<'N, 'I, 'S, 'A, 'Q, 'R> =
    abstract Invoke<'N2, 'S2> : MapState<'N, 'N2, 'I, 'S, 'S2, 'A, 'Q> -> 'R

and IOneOf2<'N, 'I, 'S, 'A, 'Q> =
    abstract Accept : IOneOf2Invoker<'N, 'I, 'S, 'A, 'Q, 'R> -> 'R

and OneOf2<'N, 'N1, 'N2, 'I, 'S, 'S1, 'S2, 'A, 'Q>(m, c1, c2) =
    member this.MapF : 'S -> Choice<'S1, 'S2> = m
    member this.Template1 : Template<'N1, 'I, 'S1, 'A, 'Q> = c1
    member this.Template2 : Template<'N2, 'I, 'S2, 'A, 'Q> = c2
    with
        interface IOneOf2<'N, 'I, 'S, 'A, 'Q> with
            member this.Accept f = f.Invoke<'N1, 'N2, 'S1, 'S2> this

and IOneOf2Invoker<'N, 'I, 'S, 'A, 'Q, 'R> =
    abstract Invoke<'N1, 'N2, 'S1, 'S2> : OneOf2<'N, 'N1, 'N2, 'I, 'S, 'S1, 'S2, 'A, 'Q> -> 'R

and Render<'I, 'S, 'Q> = 'I -> 'S -> View<'I, 'S, 'Q>

and MakeNodeRender<'N, 'I, 'S, 'Q> = 'N -> Render<'I, 'S, 'Q>

let packMapState<'N, 'N2, 'I, 'S1, 'S2, 'A, 'Q> (mapState: MapState<'N, 'N2, 'I, 'S1, 'S2, 'A, 'Q>) =
    mapState :> IMapState<'N, 'I, 'S1, 'A, 'Q>

let unpackMapState (mapState: IMapState<'N, 'I, 'S, 'A, 'Q>) (f: IMapStateInvoker<'N, 'I, 'S, 'A, 'Q, 'R>) : 'R =
    mapState.Accept f

let packOneOf2<'N, 'N1, 'N2, 'I, 'S, 'S1, 'S2, 'A, 'Q> (oneOf2: OneOf2<'N, 'N1, 'N2, 'I, 'S, 'S1, 'S2, 'A, 'Q>) =
    oneOf2 :> IOneOf2<'N, 'I, 'S, 'A, 'Q>

let unpackOneOf2 (oneOf2: IOneOf2<'N, 'I, 'S, 'A, 'Q>) (f: IOneOf2Invoker<'N, 'I, 'S, 'A, 'Q, 'R>) : 'R =
    oneOf2.Accept f

[<AbstractClass>]
type MakeRender<'N, 'I, 'S, 'A, 'Q>() =
    member this.Make(template: Template<'N, 'I, 'S, 'A, 'Q>) : 'I -> 'S -> View<'I, 'S, 'Q> =
        match template with
        | Node n -> this.MakeNode n
        | Fragment ls -> this.MakeFragment ls
        | MapState mapState -> this.MakeMapState mapState
        | OneOf2 oneOf2 -> this.MakeOneOf2 oneOf2

    abstract MakeNode : 'N -> ('I -> 'S -> View<'I, 'S, 'Q>)
    abstract MakeRef : 'I -> ('I list) -> 'I
    abstract AppendNode : 'I -> 'I -> unit
    abstract RemoveNode : 'I -> unit
    abstract InsertBeforeNode : 'I -> 'I -> unit

    // TODO super cheating!
    member this.MakeRenderS<'N2, 'S2>() : MakeRender<'N2, 'I, 'S2, 'A, 'Q> =
        this :> obj :?> MakeRender<'N2, 'I, 'S2, 'A, 'Q>

    member this.MakeFragment<'S, 'A, 'Q>(templates: Template<'N, 'I, 'S, 'A, 'Q> list) =
        let fs = List.map (this.Make) templates

        fun (parent: 'I) (s: 'S) ->
            let impl = this.MakeRef parent []
            this.AppendNode parent impl
            let views = List.map (fun f -> f impl s) fs
            // let nodes = List.map (fun v -> v.Impl) views

            { Impl = impl
              Change = fun s -> List.iter (fun i -> i.Change s) views
              Destroy = fun () -> List.iter (fun i -> i.Destroy()) views
              Query = fun q -> List.iter (fun i -> i.Query q) views }

    member this.MakeMapState(mapState: IMapState<'N, 'I, 'S, 'A, 'Q>) : 'I -> 'S -> View<'I, 'S, 'Q> =
        unpackMapState
            mapState
            { new IMapStateInvoker<'N, 'I, 'S, 'A, 'Q, Render<'I, 'S, 'Q>> with
                member __.Invoke<'N2, 'S2>(mapState: MapState<'N, 'N2, 'I, 'S, 'S2, 'A, 'Q>) : Render<'I, 'S, 'Q> =
                    let render =
                        (this.MakeRenderS<'N2, 'S2>())
                            .Make mapState.Template

                    fun (i: 'I) (s: 'S) ->
                        let view = render i (mapState.MapF s)

                        { Impl = view.Impl
                          Query = view.Query
                          Destroy = view.Destroy
                          Change = fun s1 -> view.Change <| mapState.MapF s1 } }

    member this.MakeOneOf2(oneOf2: IOneOf2<'N, 'I, 'S, 'A, 'Q>) : 'I -> 'S -> View<'I, 'S, 'Q> =
        unpackOneOf2
            oneOf2
            { new IOneOf2Invoker<'N, 'I, 'S, 'A, 'Q, Render<'I, 'S, 'Q>> with
                member __.Invoke<'N1, 'N2, 'S1, 'S2>
                    (oneOf2: OneOf2<'N, 'N1, 'N2, 'I, 'S, 'S1, 'S2, 'A, 'Q>)
                    : Render<'I, 'S, 'Q> =
                    let render1 =
                        (this.MakeRenderS<'N1, 'S1>())
                            .Make oneOf2.Template1

                    let render2 =
                        (this.MakeRenderS<'N2, 'S2>())
                            .Make oneOf2.Template2

                    fun (parent: 'I) (s: 'S) ->
                        let mutable views = None

                        match oneOf2.MapF s with
                        | Choice1Of2 s1 ->
                            let view1 = render1 parent s1

                            ignore ()
                        | Choice2Of2 s2 -> ignore ()

                        { Impl = parent
                          Change = ignore
                          Query = ignore
                          Destroy = ignore } }
