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

type Impl =
    abstract Append : Impl -> unit
    abstract Remove : Impl -> unit

type Template<'N, 'S, 'A, 'Q> =
    | Node of 'N
    | Fragment of Template<'N, 'S, 'A, 'Q> list
    | MapState of IMapState<'N, 'S, 'A, 'Q>
    | OneOf2 of IOneOf2<'N, 'S, 'A, 'Q>

// Lifecycle
// Capture capture/release state
// Sequence
// MapAction
// MapImpl
// MapQuery
// Adapter???
// Lazy???
// When???/If/Else

and ComponentView<'S, 'A, 'Q> =
    { Impl: Impl
      Dispatch: 'A -> unit
      Change: 'S -> unit
      Destroy: unit -> unit
      Query: 'Q -> unit }

and View<'S, 'Q> =
    { Impl: Impl
      Change: 'S -> unit
      Destroy: unit -> unit
      Query: 'Q -> unit }

and IMapState<'N, 'S, 'A, 'Q> =
    abstract Accept : IMapStateInvoker<'N, 'S, 'A, 'Q, 'R> -> 'R

and MapState<'N, 'N2, 'S, 'S2, 'A, 'Q>(m, t) =
    member this.MapF : 'S -> 'S2 = m
    member this.Template : Template<'N2, 'S2, 'A, 'Q> = t
    with
        interface IMapState<'N, 'S, 'A, 'Q> with
            member this.Accept f = f.Invoke<'N2, 'S2> this

and IMapStateInvoker<'N, 'S, 'A, 'Q, 'R> =
    abstract Invoke<'N2, 'S2> : MapState<'N, 'N2, 'S, 'S2, 'A, 'Q> -> 'R

and IOneOf2<'N, 'S, 'A, 'Q> =
    abstract Accept : IOneOf2Invoker<'N, 'S, 'A, 'Q, 'R> -> 'R

and OneOf2<'N, 'N1, 'N2, 'S, 'S1, 'S2, 'A, 'Q>(m, c1, c2) =
    member this.MapF : 'S -> Choice<'S1, 'S2> = m
    member this.Template1 : Template<'N1, 'S1, 'A, 'Q> = c1
    member this.Template2 : Template<'N2, 'S2, 'A, 'Q> = c2
    with
        interface IOneOf2<'N, 'S, 'A, 'Q> with
            member this.Accept f = f.Invoke<'N1, 'N2, 'S1, 'S2> this

and IOneOf2Invoker<'N, 'S, 'A, 'Q, 'R> =
    abstract Invoke<'N1, 'N2, 'S1, 'S2> : OneOf2<'N, 'N1, 'N2, 'S, 'S1, 'S2, 'A, 'Q> -> 'R

and Render<'S, 'Q> = Impl -> 'S -> View<'S, 'Q>

and MakeNodeRender<'N, 'S, 'Q> = 'N -> Render<'S, 'Q>

let packMapState<'N, 'N2, 'S1, 'S2, 'A, 'Q> (mapState: MapState<'N, 'N2, 'S1, 'S2, 'A, 'Q>) =
    mapState :> IMapState<'N, 'S1, 'A, 'Q>

let unpackMapState (mapState: IMapState<'N, 'S, 'A, 'Q>) (f: IMapStateInvoker<'N, 'S, 'A, 'Q, 'R>) : 'R =
    mapState.Accept f

let packOneOf2<'N, 'N1, 'N2, 'S, 'S1, 'S2, 'A, 'Q> (oneOf2: OneOf2<'N, 'N1, 'N2, 'S, 'S1, 'S2, 'A, 'Q>) =
    oneOf2 :> IOneOf2<'N, 'S, 'A, 'Q>

let unpackOneOf2 (oneOf2: IOneOf2<'N, 'S, 'A, 'Q>) (f: IOneOf2Invoker<'N, 'S, 'A, 'Q, 'R>) : 'R = oneOf2.Accept f

type ChoiceAssignament<'A, 'B> =
    | FirstOnly of 'A
    | SecondOnly of 'B
    | FirstAndSecond of 'A * 'B
    | SecondAndFirst of 'A * 'B

[<AbstractClass>]
type MakeRender<'N, 'S, 'A, 'Q>() =
    member this.Make(template: Template<'N, 'S, 'A, 'Q>) : Impl -> 'S -> View<'S, 'Q> =
        match template with
        | Node n -> this.MakeNodeRender n
        | Fragment ls -> this.MakeFragmentRender ls
        | MapState mapState -> this.MakeMapStateRender mapState
        | OneOf2 oneOf2 -> this.MakeOneOf2Render oneOf2

    abstract MakeNodeRender : 'N -> (Impl -> 'S -> View<'S, 'Q>)
    abstract CreateGroupNode : string -> Impl

    // TODO super cheating!
    member this.MakeRenderS<'N2, 'S2>() : MakeRender<'N2, 'S2, 'A, 'Q> =
        this :> obj :?> MakeRender<'N2, 'S2, 'A, 'Q>

    member this.MakeFragmentRender<'S, 'A, 'Q>(templates: Template<'N, 'S, 'A, 'Q> list) =
        let fs = List.map (this.Make) templates

        fun (parent: Impl) (s: 'S) ->
            let group = this.CreateGroupNode("Fragment")
            printfn "Make Fragment"
            parent.Append group // TODO move after views?
            printfn "Make Fragment (Appended)"
            // this.AppendNode parent ref
            let views =
                List.map (fun render -> render group s) (fs)


            { Impl = group
              Change = fun s -> List.iter (fun i -> i.Change s) views
              Destroy =
                  fun () ->
                      parent.Remove(group) // TODO this tries to remove nodes in the collection twice
                      List.iter (fun i -> i.Destroy()) views
              Query = fun q -> List.iter (fun i -> i.Query q) views }

    member this.MakeMapStateRender(mapState: IMapState<'N, 'S, 'A, 'Q>) : Impl -> 'S -> View<'S, 'Q> =
        unpackMapState
            mapState
            { new IMapStateInvoker<'N, 'S, 'A, 'Q, Render<'S, 'Q>> with
                member __.Invoke<'N2, 'S2>(mapState: MapState<'N, 'N2, 'S, 'S2, 'A, 'Q>) : Render<'S, 'Q> =
                    let render =
                        (this.MakeRenderS<'N2, 'S2>())
                            .Make mapState.Template

                    fun (parent: Impl) (s: 'S) ->
                        let group = this.CreateGroupNode("MapState")
                        printfn "Make MapState"
                        parent.Append group // TODO move after view?
                        printfn "Make MapState (Appended)"
                        let view = render group (mapState.MapF s)
                        printfn "MakeMapState"
                        // this.AppendNode parent view.Impl
                        parent.Append view.Impl

                        { Impl = group
                          Query = view.Query
                          Destroy = view.Destroy
                          Change = fun s1 -> view.Change <| mapState.MapF s1 } }

    member this.MakeOneOf2Render(oneOf2: IOneOf2<'N, 'S, 'A, 'Q>) : Impl -> 'S -> View<'S, 'Q> =
        unpackOneOf2
            oneOf2
            { new IOneOf2Invoker<'N, 'S, 'A, 'Q, Render<'S, 'Q>> with
                member __.Invoke<'N1, 'N2, 'S1, 'S2>
                    (oneOf2: OneOf2<'N, 'N1, 'N2, 'S, 'S1, 'S2, 'A, 'Q>)
                    : Render<'S, 'Q> =
                    let render1 =
                        (this.MakeRenderS<'N1, 'S1>())
                            .Make oneOf2.Template1

                    let render2 =
                        (this.MakeRenderS<'N2, 'S2>())
                            .Make oneOf2.Template2

                    fun (parent: Impl) (s: 'S) ->
                        let group = this.CreateGroupNode("OneOf2")
                        printfn "MakeOneOf2"
                        parent.Append group // TODO move after views?
                        printfn "MakeOneOf2 (Appended)"
                        // this.AppendNode parent ref

                        let mutable assignament =
                            match oneOf2.MapF s with
                            | Choice1Of2 s1 ->
                                printfn "MakeOneOf2 Render1"
                                let view1 = render1 group s1
                                // group.Append view1.Impl
                                printfn "MakeOneOf2 Render1 (Appended)"
                                // this.InsertBeforeNode group view1.Impl
                                FirstOnly view1
                            | Choice2Of2 s2 ->
                                printfn "MakeOneOf2 Render2"
                                let view2 = render2 group s2
                                // group.Append view2.Impl
                                printfn "MakeOneOf2 Render2 (Appended)"
                                // this.InsertBeforeNode group view2.Impl
                                SecondOnly view2

                        let change state =
                            printfn "MaoneOf2.Change"

                            match (assignament, oneOf2.MapF state) with
                            | (FirstOnly view1, Choice1Of2 s1) ->
                                printfn "FirstOnly view1, Choice1Of2 s1"
                                view1.Change s1
                            | (FirstAndSecond (view1, _), Choice1Of2 s1) ->
                                printfn "FirstAndSecond (view1, _), Choice1Of2 s1"
                                view1.Change s1

                            | (SecondOnly view2, Choice2Of2 s2) ->
                                printfn "SecondOnly view2, Choice2Of2 s2"
                                view2.Change s2
                            | (SecondAndFirst (_, view2), Choice2Of2 s2) ->
                                printfn "SecondAndFirst (_, view2), Choice2Of2 s2"
                                view2.Change s2

                            | (FirstOnly view1, Choice2Of2 s2) ->
                                printfn "MakeOneOf2 FirstOnly view1, Choice2Of2 s2"
                                let view2 = render2 group s2
                                // group.Append view2.Impl
                                printfn "MakeOneOf2 FirstOnly view1, Choice2Of2 s2 (Appended)"
                                group.Remove view1.Impl
                                printfn "MakeOneOf2 FirstOnly view1, Choice2Of2 s2 (After Remove)"
                                assignament <- SecondAndFirst(view1, view2)
                            | (FirstAndSecond (view1, view2), Choice2Of2 s2) ->
                                printfn "MakeOneOf2 FirstAndSecond (view1, view2), Choice2Of2 s2"
                                view2.Change s2
                                group.Append view2.Impl
                                printfn "MakeOneOf2 FirstAndSecond (view1, view2), Choice2Of2 s2 (Appended)"
                                group.Remove view1.Impl
                                printfn "MakeOneOf2 FirstAndSecond (view1, view2), Choice2Of2 s2 (After Remove)"
                                assignament <- SecondAndFirst(view1, view2)
                            | (SecondOnly view2, Choice1Of2 s1) ->
                                printfn "MakeOneOf2 SecondOnly view2, Choice1Of2 s1"
                                Browser.Dom.console.log (render1, group, s1)
                                let view1 = render1 group s1
                                Browser.Dom.console.log (view1)
                                // group.Append view1.Impl
                                printfn "MakeOneOf2 SecondOnly view2, Choice1Of2 s1 (Appended)"
                                group.Remove view2.Impl
                                printfn "MakeOneOf2 SecondOnly view2, Choice1Of2 s1 (After Remove)"
                                assignament <- FirstAndSecond(view1, view2)
                            | (SecondAndFirst (view1, view2), Choice1Of2 s1) ->
                                printfn "MakeOneOf2 SecondAndFirst (view1, view2), Choice1Of2 s1"
                                view1.Change s1
                                group.Append view1.Impl
                                printfn "MakeOneOf2 SecondAndFirst (view1, view2), Choice1Of2 s1 (Appended)"
                                group.Remove view2.Impl
                                printfn "MakeOneOf2 SecondAndFirst (view1, view2), Choice1Of2 s1 (After Remove)"
                                assignament <- FirstAndSecond(view1, view2)

                        let query q =
                            match assignament with
                            | FirstAndSecond (view1, _)
                            | FirstOnly view1 -> view1.Query q
                            | SecondAndFirst (_, view2)
                            | SecondOnly view2 -> view2.Query q

                        let destroy q =
                            parent.Remove(group)

                            match assignament with
                            | FirstAndSecond (view1, view2) ->
                                view1.Destroy()
                                view2.Destroy()
                            | FirstOnly view1 -> view1.Destroy()
                            | SecondAndFirst (view1, view2) ->
                                view2.Destroy()
                                view1.Destroy()
                            | SecondOnly view2 -> view2.Destroy()

                        { Impl = group
                          Change = change
                          Query = query
                          Destroy = destroy } }
