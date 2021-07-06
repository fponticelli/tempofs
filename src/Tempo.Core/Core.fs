namespace Tempo

module Core =
    type Value<'S, 'V> =
        | Literal of 'V
        | Derived of ('S -> 'V)
        static member Of<'V>(v: 'V) = Literal v
        static member Of<'S, 'V>(f: 'S -> 'V) = Derived f
        static member Of<'S>() = Derived id<'S>

        static member Resolve<'S, 'V> (v: Value<'S, 'V>) (s: 'S) =
            match v with
            | Literal v -> v
            | Derived f -> f s

        static member Map<'S, 'V1, 'V2> (map: 'V1 -> 'V2) (v: Value<'S, 'V1>) : Value<'S, 'V2> =
            match v with
            | Literal v -> Literal <| map v
            | Derived f -> Derived(f >> map)

        static member inline MapOption<'S, 'V1, 'V2>
            (map: 'V1 -> 'V2)
            (v: Value<'S, 'V1 option>)
            : Value<'S, 'V2 option> =
            Value.Map<'S, 'V1 option, 'V2 option>(fun v -> Option.map map v) v

        static member Combine<'S, 'A, 'B, 'C>(f: 'A -> 'B -> 'C, va: Value<'S, 'A>, vb: Value<'S, 'B>) : Value<'S, 'C> =
            match (va, vb) with
            | (Literal a, Literal b) -> Literal <| f a b
            | (Derived fa, Derived fb) -> Derived <| fun s -> f (fa s) (fb s)
            | (Literal a, Derived fb) -> Derived <| fun s -> f a (fb s)
            | (Derived fa, Literal b) -> Derived <| fun s -> f (fa s) b

        static member Sequence<'S, 'V>(ls: List<Value<'S, 'V>>) : Value<'S, List<'V>> =
            Derived(fun s -> List.map (fun v -> Value.Resolve v s) ls)

    type Impl =
        abstract Append : Impl -> unit
        abstract Remove : Impl -> unit

    type Template<'N, 'S, 'A, 'Q> =
        | Node of 'N
        | Fragment of Template<'N, 'S, 'A, 'Q> list
        | Transform of ITransform<'N, 'S, 'A, 'Q>
        | OneOf2 of IOneOf2<'N, 'S, 'A, 'Q>

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

    and Update<'S, 'A> = 'S -> 'A -> 'S

    and MiddlewarePayload<'S, 'A, 'Q> =
        { Current: 'S
          Previous: 'S
          Action: 'A
          Dispatch: 'A -> unit
          Query: 'Q -> unit }

    and Middleware<'S, 'A, 'Q> = MiddlewarePayload<'S, 'A, 'Q> -> unit

    and ITransform<'N, 'S, 'A, 'Q> =
        abstract Accept : ITransformInvoker<'N, 'S, 'A, 'Q, 'R> -> 'R

    and Transform<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>(transform, template) =
        member this.Transform : Render<'S2, 'A2, 'Q2> -> Render<'S1, 'A1, 'Q1> = transform
        member this.Template : Template<'N2, 'S2, 'A2, 'Q2> = template
        with
            interface ITransform<'N1, 'S1, 'A1, 'Q1> with
                member this.Accept f = f.Invoke<'N2, 'S2, 'A2, 'Q2> this

    and ITransformInvoker<'N1, 'S1, 'A1, 'Q1, 'R> =
        abstract Invoke<'N2, 'S2, 'A2, 'Q2> : Transform<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2> -> 'R

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

    and Dispatch<'A> = 'A -> unit

    and Render<'S, 'A, 'Q> = Impl -> 'S -> Dispatch<'A> -> View<'S, 'Q>

    and MakeNodeRender<'N, 'S, 'A, 'Q> = 'N -> Render<'S, 'A, 'Q>

    let packTransform<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>
        (transform: Transform<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>)
        =
        transform :> ITransform<'N1, 'S1, 'A1, 'Q1>

    let unpackTransform (transform: ITransform<'N, 'S, 'A, 'Q>) (f: ITransformInvoker<'N, 'S, 'A, 'Q, 'R>) : 'R =
        transform.Accept f

    let packOneOf2<'N, 'N1, 'N2, 'S, 'S1, 'S2, 'A, 'Q> (oneOf2: OneOf2<'N, 'N1, 'N2, 'S, 'S1, 'S2, 'A, 'Q>) =
        oneOf2 :> IOneOf2<'N, 'S, 'A, 'Q>

    let unpackOneOf2 (oneOf2: IOneOf2<'N, 'S, 'A, 'Q>) (f: IOneOf2Invoker<'N, 'S, 'A, 'Q, 'R>) : 'R = oneOf2.Accept f

    type ChoiceAssignament<'A, 'B> =
        | FirstOnly of 'A
        | SecondOnly of 'B
        | FirstAndSecond of 'A * 'B
        | SecondAndFirst of 'A * 'B

    and MakeRender<'N, 'S, 'A, 'Q>
        (
            makeNodeRender: (Template<'N, 'S, 'A, 'Q> -> Render<'S, 'A, 'Q>) -> 'N -> Render<'S, 'A, 'Q>,
            createGroupNode: string -> Impl
        ) =
        member this.Make(template: Template<'N, 'S, 'A, 'Q>) : Impl -> 'S -> Dispatch<'A> -> View<'S, 'Q> =
            match template with
            | Node n -> makeNodeRender this.Make n
            | Fragment ls -> this.MakeFragmentRender ls
            | Transform map -> this.MakeTransformRender map
            | OneOf2 oneOf2 -> this.MakeOneOf2Render oneOf2

        // TODO super cheating!
        member this.MakeRenderS<'N2, 'S2, 'A2, 'Q2>() : MakeRender<'N2, 'S2, 'A2, 'Q2> =
            this :> obj :?> MakeRender<'N2, 'S2, 'A2, 'Q2>

        member this.MakeFragmentRender<'S, 'A, 'Q>(templates: Template<'N, 'S, 'A, 'Q> list) =
            let fs = List.map (this.Make) templates

            fun (parent: Impl) (s: 'S) (dispatch) ->
                let group = createGroupNode ("Fragment")
                parent.Append group

                let views =
                    List.map (fun render -> render group s dispatch) (fs)

                { Impl = group
                  Change = fun s -> List.iter (fun i -> i.Change s) views
                  Destroy =
                      fun () ->
                          parent.Remove(group) // TODO this tries to remove nodes in the iterator twice
                          List.iter (fun i -> i.Destroy()) views
                  Query = fun q -> List.iter (fun (i: View<'S, 'Q>) -> i.Query q) views }

        member this.MakeTransformRender<'N2, 'S2, 'A2, 'Q2>
            (transform: ITransform<'N, 'S, 'A, 'Q>)
            : Impl -> 'S -> Dispatch<'A> -> View<'S, 'Q> =
            unpackTransform
                transform
                { new ITransformInvoker<'N, 'S, 'A, 'Q, Render<'S, 'A, 'Q>> with
                    member __.Invoke<'N2, 'S2, 'A2, 'Q2>
                        (transform: Transform<'N, 'N2, 'S, 'S2, 'A, 'A2, 'Q, 'Q2>)
                        : Render<'S, 'A, 'Q> =

                        let render2 =
                            (this.MakeRenderS<'N2, 'S2, 'A2, 'Q2>())
                                .Make transform.Template

                        fun impl state dispatch -> (transform.Transform render2) impl state dispatch }

        member this.MakeOneOf2Render(oneOf2: IOneOf2<'N, 'S, 'A, 'Q>) : Impl -> 'S -> Dispatch<'A> -> View<'S, 'Q> =
            unpackOneOf2
                oneOf2
                { new IOneOf2Invoker<'N, 'S, 'A, 'Q, Render<'S, 'A, 'Q>> with
                    member __.Invoke<'N1, 'N2, 'S1, 'S2>
                        (oneOf2: OneOf2<'N, 'N1, 'N2, 'S, 'S1, 'S2, 'A, 'Q>)
                        : Render<'S, 'A, 'Q> =
                        let render1 =
                            (this.MakeRenderS<'N1, 'S1, 'A, 'Q>())
                                .Make oneOf2.Template1

                        let render2 =
                            (this.MakeRenderS<'N2, 'S2, 'A, 'Q>())
                                .Make oneOf2.Template2

                        fun (parent: Impl) (s: 'S) dispatch ->
                            let group = createGroupNode ("OneOf2")
                            parent.Append group

                            let mutable assignament =
                                match oneOf2.MapF s with
                                | Choice1Of2 s1 ->
                                    let view1 = render1 group s1 dispatch
                                    FirstOnly view1
                                | Choice2Of2 s2 ->
                                    let view2 = render2 group s2 dispatch
                                    SecondOnly view2

                            let change state =

                                match (assignament, oneOf2.MapF state) with
                                | (FirstOnly view1, Choice1Of2 s1) -> view1.Change s1
                                | (FirstAndSecond (view1, _), Choice1Of2 s1) -> view1.Change s1

                                | (SecondOnly view2, Choice2Of2 s2) -> view2.Change s2
                                | (SecondAndFirst (_, view2), Choice2Of2 s2) -> view2.Change s2

                                | (FirstOnly view1, Choice2Of2 s2) ->
                                    let view2 = render2 group s2 dispatch
                                    group.Remove view1.Impl
                                    assignament <- SecondAndFirst(view1, view2)
                                | (FirstAndSecond (view1, view2), Choice2Of2 s2) ->
                                    group.Append view2.Impl
                                    view2.Change s2
                                    group.Remove view1.Impl
                                    assignament <- SecondAndFirst(view1, view2)
                                | (SecondOnly view2, Choice1Of2 s1) ->
                                    let view1 = render1 group s1 dispatch
                                    group.Remove view2.Impl
                                    assignament <- FirstAndSecond(view1, view2)
                                | (SecondAndFirst (view1, view2), Choice1Of2 s1) ->
                                    group.Append view1.Impl
                                    view1.Change s1
                                    group.Remove view2.Impl
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

    let transform<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>
        (transform: Render<'S2, 'A2, 'Q2> -> Render<'S1, 'A1, 'Q1>)
        (template: Template<'N2, 'S2, 'A2, 'Q2>)
        =
        Template<'N1, 'S1, 'A1, 'Q1>.Transform (packTransform <| Transform(transform, template))

    let map<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>
        (mapImpl: Impl -> Impl)
        (stateMap: 'S1 -> 'S2)
        (actionMap: 'A2 -> 'A1 option)
        (queryMap: 'Q1 -> 'Q2 option)
        (template: Template<'N2, 'S2, 'A2, 'Q2>)
        =
        transform<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>
            (fun render2 ->
                (fun impl state dispatch ->
                    let state2 = stateMap state

                    let dispatch2 action2 =
                        match actionMap action2 with
                        | Some a -> dispatch a
                        | None -> ()

                    let impl2 = mapImpl impl

                    let view = render2 impl2 state2 dispatch2

                    { Impl = impl2
                      Change = fun s1 -> view.Change(stateMap s1)
                      Query =
                          fun q1 ->
                              match queryMap q1 with
                              | Some q -> view.Query(q)
                              | None -> ()
                      Destroy = view.Destroy }))
            template

    let inline mapState<'N1, 'N2, 'S1, 'S2, 'A, 'Q> (f: 'S1 -> 'S2) (template: Template<'N2, 'S2, 'A, 'Q>) =
        map<'N1, 'N2, 'S1, 'S2, 'A, 'A, 'Q, 'Q> id f Some Some template

    let inline mapAction<'N1, 'N2, 'S, 'A1, 'A2, 'Q> (f: 'A2 -> 'A1 option) (template: Template<'N2, 'S, 'A2, 'Q>) =
        map<'N1, 'N2, 'S, 'S, 'A1, 'A2, 'Q, 'Q> id id f Some template

    let inline mapQuery<'N1, 'N2, 'S, 'A, 'Q1, 'Q2> (f: 'Q1 -> 'Q2 option) (template: Template<'N2, 'S, 'A, 'Q2>) =
        map<'N1, 'N2, 'S, 'S, 'A, 'A, 'Q1, 'Q2> id id Some f template

    let inline mapSA<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q>
        (mapState: 'S1 -> 'S2)
        (mapAction: 'A2 -> 'A1 option)
        (template: Template<'N2, 'S2, 'A2, 'Q>)
        =
        map<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q, 'Q> id mapState mapAction Some template

    let inline mapSAQ<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>
        (mapState: 'S1 -> 'S2)
        (mapAction: 'A2 -> 'A1 option)
        (mapQuery: 'Q1 -> 'Q2 option)
        (template: Template<'N2, 'S2, 'A2, 'Q2>)
        =
        map<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2> id mapState mapAction mapQuery template

    let inline mapSQ<'N1, 'N2, 'S1, 'S2, 'A, 'Q1, 'Q2>
        (mapState: 'S1 -> 'S2)
        (mapQuery: 'Q1 -> 'Q2 option)
        (template: Template<'N2, 'S2, 'A, 'Q2>)
        =
        map<'N1, 'N2, 'S1, 'S2, 'A, 'A, 'Q1, 'Q2> id mapState Some mapQuery template

    let inline mapAQ<'N1, 'N2, 'S, 'A1, 'A2, 'Q1, 'Q2>
        (mapAction: 'A2 -> 'A1 option)
        (mapQuery: 'Q1 -> 'Q2 option)
        (template: Template<'N2, 'S, 'A2, 'Q2>)
        =
        map<'N1, 'N2, 'S, 'S, 'A1, 'A2, 'Q1, 'Q2> id id mapAction mapQuery template

    type CatchF<'N1, 'S1, 'A1, 'Q1> = Template<'N1, 'S1, 'A1, 'Q1> -> Template<'N1, 'S1, 'A1, 'Q1>

    type ReleaseF<'N2, 'N3, 'S1, 'S2, 'S3, 'A1, 'A2, 'A3, 'Q1> =
        ('S1 -> 'S2 -> 'S3) * ('A3 -> Choice<'A1, 'A2, 'A1 * 'A2> option) * Template<'N3, 'S3, 'A3, 'Q1> -> Template<'N2, 'S2, 'A2, 'Q1>

    type CaptureResult<'N1, 'N2, 'N3, 'S1, 'S2, 'S3, 'A1, 'A2, 'A3, 'Q1> =
        CatchF<'N1, 'S1, 'A1, 'Q1> * ReleaseF<'N2, 'N3, 'S1, 'S2, 'S3, 'A1, 'A2, 'A3, 'Q1>

    let makeCaptureSA<'N1, 'N2, 'N3, 'S1, 'S2, 'S3, 'A1, 'A2, 'A3, 'Q1>
        ()
        : CaptureResult<'N1, 'N2, 'N3, 'S1, 'S2, 'S3, 'A1, 'A2, 'A3, 'Q1> =
        let mutable localState = None
        let mutable localDispatch = None

        let catch (template1: Template<'N1, 'S1, 'A1, 'Q1>) : Template<'N1, 'S1, 'A1, 'Q1> =
            transform<'N1, 'N1, 'S1, 'S1, 'A1, 'A1, 'Q1, 'Q1>
                (fun render ->
                    (fun impl state dispatch ->
                        localState <- Some state
                        localDispatch <- Some dispatch
                        let view = render impl state dispatch

                        { view with
                              Change =
                                  fun s ->
                                      localState <- Some s
                                      view.Change s
                              Destroy =
                                  fun () ->
                                      localState <- None
                                      localDispatch <- None
                                      view.Destroy() }))
                template1

        let release
            (
                mergeState: 'S1 -> 'S2 -> 'S3,
                mapAction: 'A3 -> Choice<'A1, 'A2, 'A1 * 'A2> option,
                template3: Template<'N3, 'S3, 'A3, 'Q1>
            ) : Template<'N2, 'S2, 'A2, 'Q1> =
            mapSA
                (fun s2 ->
                    let s1 = Option.get localState
                    mergeState s1 s2)
                (fun (a3: 'A3) ->
                    match mapAction a3 with
                    | Some (Choice1Of3 (a1)) ->
                        let d1 = Option.get localDispatch
                        d1 a1
                        None
                    | Some (Choice2Of3 (a2)) -> Some a2
                    | Some (Choice3Of3 (a1, a2)) ->
                        let d1 = Option.get localDispatch
                        d1 a1
                        Some a2
                    | None -> None)
                template3

        (catch, release)

    type ReleaseActionF<'N2, 'N3, 'S1, 'A1, 'A2, 'A3, 'Q1> =
        ('A3 -> Choice<'A1, 'A2, 'A1 * 'A2> option) * Template<'N3, 'S1, 'A3, 'Q1> -> Template<'N2, 'S1, 'A2, 'Q1>

    type CaptureActionResult<'N1, 'N2, 'N3, 'S1, 'A1, 'A2, 'A3, 'Q1> =
        CatchF<'N1, 'S1, 'A1, 'Q1> * ReleaseActionF<'N2, 'N3, 'S1, 'A1, 'A2, 'A3, 'Q1>

    let makeCaptureAction<'N1, 'N2, 'N3, 'S1, 'A1, 'A2, 'A3, 'Q1>
        ()
        : CaptureActionResult<'N1, 'N2, 'N3, 'S1, 'A1, 'A2, 'A3, 'Q1> =
        let (catch, release) =
            makeCaptureSA<'N1, 'N2, 'N3, 'S1, 'S1, 'S1, 'A1, 'A2, 'A3, 'Q1> ()

        (catch,
         (fun (mapActionF: 'A3 -> Choice<'A1, 'A2, 'A1 * 'A2> option, template) ->
             release ((fun _ s -> s), mapActionF, template)))

    type ReleaseStateF<'N2, 'N3, 'S1, 'S2, 'S3, 'A1, 'Q1> =
        ('S1 -> 'S2 -> 'S3) * Template<'N3, 'S3, 'A1, 'Q1> -> Template<'N2, 'S2, 'A1, 'Q1>

    type CaptureStateResult<'N1, 'N2, 'N3, 'S1, 'S2, 'S3, 'A1, 'Q1> =
        CatchF<'N1, 'S1, 'A1, 'Q1> * ReleaseStateF<'N2, 'N3, 'S1, 'S2, 'S3, 'A1, 'Q1>

    let makeCaptureState<'N1, 'N2, 'N3, 'S1, 'S2, 'S3, 'A1, 'Q1>
        ()
        : CaptureStateResult<'N1, 'N2, 'N3, 'S1, 'S2, 'S3, 'A1, 'Q1> =
        let (catch, release) =
            makeCaptureSA<'N1, 'N2, 'N3, 'S1, 'S2, 'S3, 'A1, 'A1, 'A1, 'Q1> ()

        (catch, (fun (mapStateF: 'S1 -> 'S2 -> 'S3, template) -> release (mapStateF, Some << Choice2Of3, template)))

    let lifecycle<'N, 'S, 'A, 'Q, 'P>
        (afterRender: 'S -> 'P)
        (beforeChange: 'S -> 'P -> bool)
        (afterChange: 'S -> 'P -> 'P)
        (beforeDestroy: 'P -> unit)
        (respond: 'Q -> 'P -> 'P)
        (template: Template<'N, 'S, 'A, 'Q>)
        : Template<'N, 'S, 'A, 'Q> =
        transform<'N, 'N, 'S, 'S, 'A, 'A, 'Q, 'Q>
            (fun render ->
                (fun impl state dispatch ->
                    let view = render impl state dispatch
                    let mutable payload = afterRender state

                    { Impl = impl
                      Change =
                          fun s ->
                              if beforeChange s payload then
                                  view.Change s
                                  payload <- afterChange s payload
                      Query =
                          fun q ->
                              view.Query q
                              payload <- respond q payload
                      Destroy =
                          fun () ->
                              beforeDestroy payload
                              view.Destroy() }))
            template

    let iterator<'N1, 'N2, 'S1, 'S2, 'A, 'Q>
        (createGroupNode: string -> Impl)
        (map: 'S1 -> 'S2 list)
        (template: Template<'N2, 'S2, 'A, 'Q>)
        : Template<'N1, 'S1, 'A, 'Q> =
        transform
            (fun render ->
                fun (parent: Impl) (s: 'S1) dispatch ->
                    let group = createGroupNode "Iterator"
                    parent.Append group
                    let ls = map s

                    let mutable views =
                        List.map (fun state -> render group state dispatch) ls

                    let query =
                        fun q -> List.iter (fun (view: View<'S2, 'Q>) -> view.Query q) views

                    let change =
                        fun (s: 'S1) ->
                            let states = map s

                            let min =
                                System.Math.Min(views.Length, states.Length)

                            List.zip views states
                            |> List.iter (fun (view, state) -> view.Change state)

                            List.skip min views
                            |> List.iter (fun view -> view.Destroy())

                            views <- List.take min views

                            let newViews =
                                List.skip min states
                                |> List.map (fun state -> render group state dispatch)

                            views <- views @ newViews

                    let destroy =
                        fun () -> List.iter (fun view -> view.Destroy()) views

                    { Impl = group
                      Query = query
                      Destroy = destroy
                      Change = change })
            template

    let comp<'N, 'S, 'A, 'Q>
        (update: Update<'S, 'A>)
        (middleware: Middleware<'S, 'A, 'Q>)
        (template: Template<'N, 'S, 'A, 'Q>)
        : Template<'N, 'S, 'A, 'Q> =
        transform<'N, 'N, 'S, 'S, 'A, 'A, 'Q, 'Q>
            (fun render ->
                (fun impl state outerDispatch ->
                    let mutable localState = state

                    let rec dispatch a =
                        let curr = update localState a
                        view.Change curr

                        middleware
                            { Current = curr
                              Previous = localState
                              Action = a
                              Dispatch = dispatch
                              Query = view.Query }

                        localState <- curr
                        outerDispatch a

                    and view = render impl localState dispatch

                    { Impl = impl
                      Change = fun s -> view.Change s
                      Query = fun q -> view.Query q
                      Destroy = fun () -> view.Destroy() }))
            template
