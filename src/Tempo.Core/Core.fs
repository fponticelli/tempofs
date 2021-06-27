namespace Tempo

module Core =
    type Value<'S, 'V> =
        | Literal of 'V
        | Derived of ('S -> 'V)
        static member Of<'V>(v: 'V) = Literal v
        static member Of<'S, 'V>(f: 'S -> 'V) = Derived f
        static member Of<'S>() = Derived id<'S>

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
        | Map of IMap<'N, 'S, 'A, 'Q>
        | Lifecycle of ILifecycle<'N, 'S, 'A, 'Q>
        | OneOf2 of IOneOf2<'N, 'S, 'A, 'Q>
        | Iterator of IIterator<'N, 'S, 'A, 'Q>

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

    and IMap<'N, 'S, 'A, 'Q> =
        abstract Accept : IMapInvoker<'N, 'S, 'A, 'Q, 'R> -> 'R

    and Map<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>(state, action, query, template) =
        member this.MapStateF : 'S1 -> 'S2 = state
        member this.MapActionF : 'A2 -> 'A1 = action
        member this.MapQueryF : 'Q1 -> 'Q2 = query
        member this.Template : Template<'N2, 'S2, 'A2, 'Q2> = template
        with
            interface IMap<'N1, 'S1, 'A1, 'Q1> with
                member this.Accept f = f.Invoke<'N2, 'S2, 'A2, 'Q2> this

    and IMapInvoker<'N1, 'S1, 'A1, 'Q1, 'R> =
        abstract Invoke<'N2, 'S2, 'A2, 'Q2> : Map<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2> -> 'R

    and ILifecycle<'N, 'S, 'A, 'Q> =
        abstract Accept : ILifecycleInvoker<'N, 'S, 'A, 'Q, 'R> -> 'R

    and Lifecycle<'N, 'S, 'A, 'Q, 'P>(afterRender, beforeChange, afterChange, beforeDestroy, respond, template) =
        member this.AfterRender : 'S -> 'P = afterRender
        member this.BeforeChange : 'S -> 'P -> bool = beforeChange
        member this.AfterChange : 'S -> 'P -> 'P = afterChange
        member this.BeforeDestroy : 'P -> unit = beforeDestroy
        member this.Respond : 'Q -> 'P -> unit = respond
        member this.Template : Template<'N, 'S, 'A, 'Q> = template
        with
            interface ILifecycle<'N, 'S, 'A, 'Q> with
                member this.Accept f = f.Invoke<'P> this

    and ILifecycleInvoker<'N, 'S, 'A, 'Q, 'R> =
        abstract Invoke<'P> : Lifecycle<'N, 'S, 'A, 'Q, 'P> -> 'R

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

    and IIterator<'N, 'S, 'A, 'Q> =
        abstract Accept : IIteratorInvoker<'N, 'S, 'A, 'Q, 'R> -> 'R

    and Iterator<'N, 'N1, 'S, 'S1, 'A, 'Q>(f, template) =
        member this.MapF : 'S -> 'S1 list = f
        member this.Template : Template<'N1, 'S1, 'A, 'Q> = template
        with
            interface IIterator<'N, 'S, 'A, 'Q> with
                member this.Accept f = f.Invoke<'N1, 'S1> this

    and IIteratorInvoker<'N, 'S, 'A, 'Q, 'R> =
        abstract Invoke<'N1, 'S1> : Iterator<'N, 'N1, 'S, 'S1, 'A, 'Q> -> 'R

    and Dispatch<'A> = 'A -> unit

    and Render<'S, 'A, 'Q> = Impl -> 'S -> Dispatch<'A> -> View<'S, 'Q>

    and MakeNodeRender<'N, 'S, 'A, 'Q> = 'N -> Render<'S, 'A, 'Q>

    let packMap<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2> (map: Map<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>) =
        map :> IMap<'N1, 'S1, 'A1, 'Q1>

    let unpackMap (map: IMap<'N, 'S, 'A, 'Q>) (f: IMapInvoker<'N, 'S, 'A, 'Q, 'R>) : 'R = map.Accept f

    let packLifecycle<'N, 'S, 'A, 'Q, 'P> (map: Lifecycle<'N, 'S, 'A, 'Q, 'P>) = map :> ILifecycle<'N, 'S, 'A, 'Q>

    let unpackLifecycle (map: ILifecycle<'N, 'S, 'A, 'Q>) (f: ILifecycleInvoker<'N, 'S, 'A, 'Q, 'R>) : 'R = map.Accept f

    let packOneOf2<'N, 'N1, 'N2, 'S, 'S1, 'S2, 'A, 'Q> (oneOf2: OneOf2<'N, 'N1, 'N2, 'S, 'S1, 'S2, 'A, 'Q>) =
        oneOf2 :> IOneOf2<'N, 'S, 'A, 'Q>

    let unpackOneOf2 (oneOf2: IOneOf2<'N, 'S, 'A, 'Q>) (f: IOneOf2Invoker<'N, 'S, 'A, 'Q, 'R>) : 'R = oneOf2.Accept f

    let packIterator<'N, 'N1, 'S, 'S1, 'A, 'Q> (iterator: Iterator<'N, 'N1, 'S, 'S1, 'A, 'Q>) =
        iterator :> IIterator<'N, 'S, 'A, 'Q>

    let unpackIterator (iterator: IIterator<'N, 'S, 'A, 'Q>) (f: IIteratorInvoker<'N, 'S, 'A, 'Q, 'R>) : 'R =
        iterator.Accept f

    type ChoiceAssignament<'A, 'B> =
        | FirstOnly of 'A
        | SecondOnly of 'B
        | FirstAndSecond of 'A * 'B
        | SecondAndFirst of 'A * 'B

    [<AbstractClass>]
    type MakeRender<'N, 'S, 'A, 'Q>() =
        member this.Make(template: Template<'N, 'S, 'A, 'Q>) : Impl -> 'S -> Dispatch<'A> -> View<'S, 'Q> =
            match template with
            | Node n -> this.MakeNodeRender n
            | Fragment ls -> this.MakeFragmentRender ls
            | Map map -> this.MakeMapRender map
            | Lifecycle lifecycle -> this.MakeLifecycleRender lifecycle
            | OneOf2 oneOf2 -> this.MakeOneOf2Render oneOf2
            | Iterator iterator -> this.MakeIteratorRender iterator

        abstract MakeNodeRender : 'N -> (Impl -> 'S -> Dispatch<'A> -> View<'S, 'Q>)
        abstract CreateGroupNode : string -> Impl

        // TODO super cheating!
        member this.MakeRenderS<'N2, 'S2, 'A2, 'Q2>() : MakeRender<'N2, 'S2, 'A2, 'Q2> =
            this :> obj :?> MakeRender<'N2, 'S2, 'A2, 'Q2>
        // abstract MakeRenderS<'N2, 'S2, 'A2, 'Q2> : unit -> MakeRender<'N2, 'S2, 'A2, 'Q2>

        member this.MakeFragmentRender<'S, 'A, 'Q>(templates: Template<'N, 'S, 'A, 'Q> list) =
            let fs = List.map (this.Make) templates

            fun (parent: Impl) (s: 'S) (dispatch) ->
                let group = this.CreateGroupNode("Fragment")
                parent.Append group

                let views =
                    List.map (fun render -> render group s dispatch) (fs)

                { Impl = group
                  Change = fun s -> List.iter (fun i -> i.Change s) views
                  Destroy =
                      fun () ->
                          parent.Remove(group) // TODO this tries to remove nodes in the iterator twice
                          List.iter (fun i -> i.Destroy()) views
                  Query = fun q -> List.iter (fun i -> i.Query q) views }

        member this.MakeMapRender<'N2, 'S2, 'A2, 'Q2>
            (map: IMap<'N, 'S, 'A, 'Q>)
            : Impl -> 'S -> Dispatch<'A> -> View<'S, 'Q> =
            unpackMap
                map
                { new IMapInvoker<'N, 'S, 'A, 'Q, Render<'S, 'A, 'Q>> with
                    member __.Invoke<'N2, 'S2, 'A2, 'Q2>
                        (map: Map<'N, 'N2, 'S, 'S2, 'A, 'A2, 'Q, 'Q2>)
                        : Render<'S, 'A, 'Q> =
                        let render =
                            (this.MakeRenderS<'N2, 'S2, 'A2, 'Q2>())
                                .Make map.Template

                        fun (parent: Impl) (s: 'S) dispatch ->
                            let dispatch2 a = dispatch <| map.MapActionF a

                            let view =
                                render parent (map.MapStateF s) dispatch2

                            let query = map.MapQueryF >> view.Query
                            let destroy = view.Destroy
                            let change = map.MapStateF >> view.Change

                            { Impl = parent
                              Query = query
                              Destroy = destroy
                              Change = change } }

        member this.MakeLifecycleRender<'P>
            (lifecycle: ILifecycle<'N, 'S, 'A, 'Q>)
            : Impl -> 'S -> Dispatch<'A> -> View<'S, 'Q> =
            unpackLifecycle
                lifecycle
                { new ILifecycleInvoker<'N, 'S, 'A, 'Q, Render<'S, 'A, 'Q>> with
                    member __.Invoke<'P>(lifecycle: Lifecycle<'N, 'S, 'A, 'Q, 'P>) : Render<'S, 'A, 'Q> =
                        let render = this.Make lifecycle.Template

                        fun (impl: Impl) (state: 'S) (dispatch: Dispatch<'A>) ->
                            let view = render impl state dispatch
                            let mutable payload = lifecycle.AfterRender state

                            let query q =
                                lifecycle.Respond q payload
                                view.Query q

                            let destroy () =
                                lifecycle.BeforeDestroy payload
                                view.Destroy()

                            let change state =
                                if not <| lifecycle.BeforeChange state payload then
                                    view.Change state
                                    payload <- lifecycle.AfterChange state payload

                            { Impl = impl
                              Change = change
                              Query = query
                              Destroy = destroy } }

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
                            let group = this.CreateGroupNode("OneOf2")
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

        member this.MakeIteratorRender
            (iterator: IIterator<'N, 'S, 'A, 'Q>)
            : Impl -> 'S -> Dispatch<'A> -> View<'S, 'Q> =
            unpackIterator
                iterator
                { new IIteratorInvoker<'N, 'S, 'A, 'Q, Render<'S, 'A, 'Q>> with
                    member __.Invoke<'N2, 'S2>(iterator: Iterator<'N, 'N2, 'S, 'S2, 'A, 'Q>) : Render<'S, 'A, 'Q> =
                        let render =
                            (this.MakeRenderS<'N2, 'S2, 'A, 'Q>())
                                .Make iterator.Template

                        fun (parent: Impl) (s: 'S) dispatch ->
                            let group = this.CreateGroupNode("Iterator")
                            parent.Append group
                            let ls = iterator.MapF s

                            let mutable views =
                                List.map (fun state -> render group state dispatch) ls

                            let query =
                                fun q -> List.iter (fun view -> view.Query q) views

                            let change =
                                fun (s: 'S) ->
                                    let states = iterator.MapF s

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
                              Change = change } }

    let map<'N1, 'N2, 'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>
        (state: 'S1 -> 'S2)
        (action: 'A2 -> 'A1)
        (query: 'Q1 -> 'Q2)
        (template: Template<'N2, 'S2, 'A2, 'Q2>)
        =
        Template<'N1, 'S1, 'A1, 'Q1>.Map (packMap <| Map(state, action, query, template))

    let mapState<'N1, 'N2, 'S1, 'S2, 'A, 'Q> (f: 'S1 -> 'S2) (template: Template<'N2, 'S2, 'A, 'Q>) =
        Template<'N1, 'S1, 'A, 'Q>.Map (packMap <| Map(f, id, id, template))

    let mapAction<'N1, 'N2, 'S, 'A1, 'A2, 'Q> (f: 'A2 -> 'A1) (template: Template<'N2, 'S, 'A2, 'Q>) =
        Template<'N1, 'S, 'A1, 'Q>.Map (packMap <| Map(id, f, id, template))

    let mapQuery<'N1, 'N2, 'S, 'A, 'Q1, 'Q2> (f: 'Q1 -> 'Q2) (template: Template<'N2, 'S, 'A, 'Q2>) =
        Template<'N1, 'S, 'A, 'Q1>.Map (packMap <| Map(id, id, f, template))

    let lifecycle
        (afterRender: 'S -> 'P)
        (beforeChange: 'S -> 'P -> bool)
        (afterChange: 'S -> 'P -> 'P)
        (beforeDestroy: 'P -> unit)
        (respond: 'Q -> 'P -> unit)
        (template: Template<'N, 'S, 'A, 'Q>)
        =
        Template<'N, 'S, 'A, 'Q>.Lifecycle
            (packLifecycle
             <| Lifecycle(afterRender, beforeChange, afterChange, beforeDestroy, respond, template))
