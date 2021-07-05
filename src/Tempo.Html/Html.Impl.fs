namespace Tempo.Html

open Browser
open Tempo.Core
open Tempo.Std
open Tempo.Html
open Tempo.Html.Tools
open Browser.Types

module Impl =
    [<AbstractClass>]
    type HTMLImpl() =
        abstract GetNodes : unit -> Node list

    type HTMLElementImpl =
        val element: Element
        inherit HTMLImpl
        override this.GetNodes() = [ this.element :> Node ]

        member this.SetAttribute(name: string, value: string option) =
            match value with
            | Some s -> this.element.setAttribute (name, s)
            | None -> this.element.removeAttribute (name)

        member this.SetProperty<'T>(name: string, value: 'T) : unit = assign this.element name value

        member this.SetHandler<'S, 'A, 'E, 'EL when 'E :> Event and 'EL :> Element>
            (name: string)
            (getState: unit -> 'S)
            (dispatch: 'A -> unit)
            (trigger: HTMLTrigger<_, _, 'E, 'EL>)
            =
            this.element.addEventListener (
                name,
                (fun e ->
                    trigger.Handler
                        { State = getState ()
                          Event = (e :?> 'E)
                          Element = this.element :?> 'EL }
                        dispatch)
            )

        interface Impl with
            override this.Append(child: Impl) =
                if hasProperty (child, "GetNodes") then
                    let child = unsafeCast<HTMLImpl, Impl> (child)
                    let nodes = child.GetNodes()
                    List.iter (this.element.appendChild >> ignore) nodes
                else
                    failwith $"HTMLElementImpl doesn't know how to append a child of type {child}"

            override this.Remove(child: Impl) =
                if hasProperty (child, "GetNodes") then
                    let child = unsafeCast<HTMLImpl, Impl> (child)
                    let ls = child.GetNodes()
                    List.iter remove ls
                else
                    failwith $"HTMLElementImpl doesn't know how to remove a child of type {child}"

        new(el: Element) = { element = el }
        new(name: string) = HTMLElementImpl(document.createElement name)
        new(ns: string, name: string) = HTMLElementImpl(document.createElementNS (ns, name))

    type HTMLTextImpl =
        val text: Text
        inherit HTMLImpl
        override this.GetNodes() = [ this.text :> Node ]

        interface Impl with
            override this.Append(child: Impl) =
                failwith "HTMLTextImpl does not support adding children"

            override this.Remove(child: Impl) =
                failwith "HTMLTextImpl does not support removing children"

        new(text: Text) = { text = text }
        new(value: string) = HTMLTextImpl(document.createTextNode value)

    let mutable private counter = 0

    type HTMLGroupImpl =
        val ref: Node
        val mutable children: Impl list
        inherit HTMLImpl

        override this.GetNodes() =
            this.ref
            :: (List.collect
                    (fun (child: Impl) ->
                        if hasProperty (child, "GetNodes") then
                            let child = unsafeCast<HTMLImpl, Impl> (child)
                            child.GetNodes()
                        else
                            failwith $"Group contains a foreign element {child}")
                    (List.rev this.children))

        interface Impl with
            override this.Append(child: Impl) =
                this.children <- child :: this.children

                if hasProperty (child, "GetNodes") then
                    let child = unsafeCast<HTMLImpl, Impl> (child)
                    let nodes = child.GetNodes()
                    let parent = this.ref.parentNode

                    List.iter (fun node -> parent.insertBefore (node, this.ref) |> ignore) nodes
                else
                    failwith $"HTMLGroupImpl doesn't know how to append a child of type {child}"

            override this.Remove(child: Impl) =
                if hasProperty (child, "GetNodes") then
                    let htmlChild = unsafeCast<HTMLImpl, Impl> (child)
                    this.children <- List.filter (fun c -> c <> child) this.children
                    let ls = htmlChild.GetNodes()
                    List.iter remove ls
                else
                    failwith $"HTMLGroupImpl doesn't know how to remove a child of type {child}"

        new(label: string) =
#if DEBUG
            counter <- counter + 1

            { ref = document.createComment $"{label}: {counter}"
              children = [] }
#else
            { ref = document.createTextNode ""
              children = [] }
#endif

    type LifecycleImpl<'S, 'A, 'Q> =
        { BeforeChange: 'S -> bool
          AfterChange: 'S -> unit
          BeforeDestroy: unit -> unit
          Dispatch: 'A -> unit
          Respond: 'Q -> unit }

    let inline attribute<'S, 'A, 'Q> name value : HTMLTemplateAttribute<'S, 'A, 'Q> =
        HTMLNamedAttribute { Name = name; Value = value }

    let packHTMLTrigger (trigger: HTMLTrigger<'S, 'A, 'E, 'EL>) = trigger :> IHTMLTrigger<'S, 'A>

    let unpackHTMLTrigger (trigger: IHTMLTrigger<'S, 'A>) (f: IHTMLTriggerInvoker<'S, 'A, 'R>) : 'R = trigger.Accept f

    let makeTrigger<'S, 'A, 'E, 'EL when 'E :> Event and 'EL :> Element> (f: TriggerPayload<'S, 'E, 'EL> -> Dispatch<'A> -> unit) = packHTMLTrigger <| HTMLTrigger(f)

    let packProperty (trigger: Property<'S, 'V>) = trigger :> IProperty<'S>

    let unpackProperty (trigger: IProperty<'S>) (f: IPropertyInvoker<'S, 'R>) : 'R = trigger.Accept f

    let inline property<'S, 'A, 'Q, 'V> (name: string) (value: Value<'S, 'V>) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        HTMLNamedAttribute
            { Name = name
              Value =
                  HTMLTemplateAttributeValue.Property
                  <| (packProperty <| Property(value)) }

    let packHTMLLifecycle (lifecycle: HTMLLifecycle<'S, 'A, 'Q, 'EL, 'P>) = lifecycle :> IHTMLLifecycle<'S, 'A, 'Q>

    let unpackHTMLLifecycle (lifecycle: IHTMLLifecycle<'S, 'A, 'Q>) (f: IHTMLLifecycleInvoker<'S, 'A, 'Q, 'R>) : 'R = lifecycle.Accept f

    // TODO this should be optimizable and wrapped in Transform without the need for special treatment
    let makeLifecycle<'S, 'A, 'Q, 'EL, 'P when 'EL :> Element> (afterRender: HTMLLifecycleInitialPayload<'S, 'A, 'EL> -> 'P) (beforeChange: HTMLLifecyclePayload<'S, 'A, 'EL, 'P> -> (bool * 'P)) (afterChange: HTMLLifecyclePayload<'S, 'A, 'EL, 'P> -> 'P) (beforeDestroy: HTMLLifecyclePayload<'S, 'A, 'EL, 'P> -> unit) (respond: 'Q -> HTMLLifecyclePayload<'S, 'A, 'EL, 'P> -> 'P) =
        packHTMLLifecycle
        <| HTMLLifecycle(afterRender, beforeChange, afterChange, beforeDestroy, respond)

    let lifecycleAttribute<'S, 'A, 'Q, 'EL, 'P when 'EL :> Element> (afterRender: HTMLLifecycleInitialPayload<'S, 'A, 'EL> -> 'P) (beforeChange: HTMLLifecyclePayload<'S, 'A, 'EL, 'P> -> (bool * 'P)) (afterChange: HTMLLifecyclePayload<'S, 'A, 'EL, 'P> -> 'P) (beforeDestroy: HTMLLifecyclePayload<'S, 'A, 'EL, 'P> -> unit) (respond: 'Q -> HTMLLifecyclePayload<'S, 'A, 'EL, 'P> -> 'P) =
        HTMLTemplateAttribute<'S, 'A, 'Q>.Lifecycle (makeLifecycle<'S, 'A, 'Q, 'EL, 'P> afterRender beforeChange afterChange beforeDestroy respond)

    let applyTrigger<'S, 'A> (name: string) (domTrigger: IHTMLTrigger<'S, 'A>) (impl: HTMLElementImpl) (dispatch: 'A -> unit) (getState: unit -> 'S) =
        unpackHTMLTrigger
            domTrigger
            { new IHTMLTriggerInvoker<'S, 'A, int> with
                override this.Invoke<'E, 'EL when 'E :> Event and 'EL :> Element>(trigger: HTMLTrigger<'S, 'A, 'E, 'EL>) =
                    impl.SetHandler name getState dispatch trigger

                    0 }
        |> ignore

    let applyProperty<'S> (name: string) (prop: IProperty<'S>) (impl: HTMLElementImpl) (state) =
        unpackProperty
            prop
            { new IPropertyInvoker<'S, int> with
                override this.Invoke<'V>(prop: Property<'S, 'V>) =
                    impl.SetProperty<'V>(name, Value.Resolve prop.Value state)
                    0 }
        |> ignore

    let extractDerivedProperty<'S> name (prop: IProperty<'S>) =
        unpackProperty
            prop
            { new IPropertyInvoker<'S, (HTMLElementImpl -> 'S -> unit) option> with
                override this.Invoke<'V>(prop: Property<'S, 'V>) =
                    match prop.Value with
                    | Derived f -> Some(fun impl state -> impl.SetProperty(name, f state))
                    | Literal _ -> None }

    let extractLifecycle<'S, 'A, 'Q> (lc: IHTMLLifecycle<'S, 'A, 'Q>) (dispatch: Dispatch<'A>) =
        unpackHTMLLifecycle
            lc
            { new IHTMLLifecycleInvoker<'S, 'A, 'Q, Element -> 'S -> LifecycleImpl<'S, 'A, 'Q>> with
                override this.Invoke<'EL, 'P when 'EL :> Element>(t: HTMLLifecycle<'S, 'A, 'Q, 'EL, 'P>) : Element -> 'S -> LifecycleImpl<'S, 'A, 'Q> =
                    fun (el: Element) (state: 'S) ->
                        let mutable payload : 'P =
                            t.AfterRender
                                { State = state
                                  Element = el :?> 'EL
                                  Dispatch = dispatch }

                        let beforeChange state =
                            let (result, newPayload) =
                                t.BeforeChange
                                    { State = state
                                      Element = el :?> 'EL
                                      Dispatch = dispatch
                                      Payload = payload }

                            payload <- newPayload
                            result

                        let afterChange state =
                            payload <-
                                t.AfterChange
                                    { State = state
                                      Element = el :?> 'EL
                                      Payload = payload
                                      Dispatch = dispatch }

                        let beforeDestroy () =
                            t.BeforeDestroy
                                { State = state
                                  Element = el :?> 'EL
                                  Payload = payload
                                  Dispatch = dispatch }

                        let respond query =
                            payload <-
                                t.Respond
                                    query
                                    { State = state
                                      Element = el :?> 'EL
                                      Payload = payload
                                      Dispatch = dispatch }

                        { BeforeChange = beforeChange
                          AfterChange = afterChange
                          BeforeDestroy = beforeDestroy
                          Respond = respond
                          Dispatch = dispatch } }



    let mergeLifecycles (ls: LifecycleImpl<'S, 'A, 'Q> list) =
        let merge a b =
            { BeforeChange =
                  fun s ->
                      let ra = a.BeforeChange s
                      let rb = b.BeforeChange s
                      ra || rb
              AfterChange =
                  fun s ->
                      a.AfterChange s
                      b.AfterChange s
              BeforeDestroy =
                  fun () ->
                      a.BeforeDestroy()
                      b.BeforeDestroy()
              Respond =
                  fun q ->
                      a.Respond q
                      b.Respond q
              Dispatch =
                  fun v ->
                      a.Dispatch v
                      b.Dispatch v }

        let start =
            { BeforeChange = fun _ -> true
              AfterChange = ignore
              BeforeDestroy = ignore
              Respond = ignore
              Dispatch = ignore }

        List.fold merge start ls

    let createGroupNode (label: string) = HTMLGroupImpl(label) :> Impl

    let aggregatedAttributes =
        [ "class", " "; "style", "; " ] |> Map.ofList

    // this fails at runtime if the list is empty
    let foldSelf<'T> (f: 'T -> 'T -> 'T) (ls: 'T list) =
        let head = ls.Head
        let tail = ls.Tail
        List.fold f head tail

    let combineAttributes<'S> (name: string) (va: Value<'S, string option>) (vb: Value<'S, string option>) : Value<'S, string option> =
        match Map.tryFind name aggregatedAttributes with
        | Some sep ->
            let combiner (a: string option) (b: string option) =
                Option.map2 (fun a b -> $"{a}{sep}{b}") a b

            Value.Combine<'S, string option, string option, string option>(combiner, va, vb)
        | None -> va

    let rec makeHTMLNodeRender<'S, 'A, 'Q> (make: Template<HTMLTemplateNode<'S, 'A, 'Q>, 'S, 'A, 'Q> -> Render<'S, 'A, 'Q>) (node: HTMLTemplateNode<'S, 'A, 'Q>) : Render<'S, 'A, 'Q> =
        match node with
        | HTMLTemplateElementNS (ns, el) -> makeRenderDOMElement (Some ns) el make
        | HTMLTemplateElement el -> makeRenderDOMElement (None) el make
        | HTMLTemplateText v -> makeRenderDOMText v

    and makeRenderDOMElement ns (node: HTMLTemplateElement<'S, 'A, 'Q>) make : Render<'S, 'A, 'Q> =
        fun (parent: Impl) (state: 'S) dispatch ->
            let mutable localState = state

            let htmlImpl =
                match ns with
                | Some ns -> HTMLElementImpl(ns, node.Name)
                | None -> HTMLElementImpl node.Name

            let impl = htmlImpl :> Impl
            let getState () : 'S = localState

            let namedAttributes =
                List.filterMap
                    (function
                    | HTMLNamedAttribute at -> Some at
                    | _ -> None)
                    node.Attributes

            let (attributes, properties, triggers) =
                List.fold
                    (fun (attributes, properties, triggers) { Name = name; Value = value } ->
                        match value with
                        | StringAttr v -> ((name, v) :: attributes, properties, triggers)
                        | Property v -> (attributes, (name, v) :: properties, triggers)
                        | Trigger v -> (attributes, properties, (name, v) :: triggers))
                    ([], [], [])
                    namedAttributes

            let groupedAttributes =
                List.groupBy (fun (name, _) -> name) attributes
                |> List.map (fun (name, ls) -> (name, List.map (fun (_, v) -> v) ls))

            let attributes =
                List.map (fun (name: string, ls: Value<'S, string option> list) -> (name, foldSelf (combineAttributes name) ls)) groupedAttributes

            // Apply Attributes
            List.iter (fun (name, value) -> htmlImpl.SetAttribute(name, (Value.Resolve value state))) attributes
            // Store Derived Attributes
            let attributeUpdates =
                List.filterMap
                    (fun (name, value) ->
                        match value with
                        | Derived f -> Some(name, f)
                        | Literal _ -> None)
                    attributes
                |> List.map (fun (name, f) -> (fun s -> htmlImpl.SetAttribute(name, f s)))

            // Apply Properties
            List.iter (fun (name, prop) -> applyProperty name prop htmlImpl state) properties
            // Store Derived Properties
            let propertyUpdates =
                List.filterMap (fun (name, prop) -> extractDerivedProperty name prop) properties
                |> List.map (fun f -> f htmlImpl)

            // Apply Triggers
            let callback (name, handler) =
                applyTrigger name handler htmlImpl dispatch getState

            List.iter callback triggers

            parent.Append impl

            // TODO use HTMLElementImpl methods
            let childViews =
                List.map (fun child -> make child impl localState dispatch) node.Children

            let { BeforeChange = beforeChange
                  AfterChange = afterChange
                  BeforeDestroy = beforeDestroy
                  Respond = respond } =
                List.filterMap
                    (function
                    | Lifecycle lc ->
                        extractLifecycle lc dispatch htmlImpl.element state
                        |> Some
                    | _ -> None)
                    node.Attributes
                |> mergeLifecycles

            let childUpdates =
                List.map (fun ({ Change = change }: View<_, _>) -> change) childViews

            let childDestroys =
                List.map (fun ({ Destroy = destroy }: View<_, _>) -> destroy) childViews

            let childQueries =
                List.map (fun ({ Query = query }: View<_, _>) -> query) childViews

            // TODO use HTMLElementImpl methods
            // let attributeUpdates =
            // namedAttributes
            // |> List.filterMap derivedApplication
            // |> List.map (fun f -> f htmlImpl.element)

            let updates =
                attributeUpdates @ propertyUpdates @ childUpdates

            let change =
                fun state ->
                    if beforeChange state then
                        localState <- state
                        List.iter (fun change -> change localState) updates
                        afterChange localState

            let destroy =
                fun () ->
                    beforeDestroy ()
                    parent.Remove(impl)
                    List.iter (fun destroy -> destroy ()) childDestroys

            let query =
                fun (q: 'Q) ->
                    List.iter (fun query -> query q) childQueries
                    respond q

            { Impl = impl
              Change = change
              Destroy = destroy
              Query = query }

    and makeRenderDOMText (value: Value<'S, string>) : Render<'S, 'A, 'Q> =
        fun (parent: Impl) (state: 'S) dispatch ->
            match value with
            | Derived f ->
                let htmlImpl = HTMLTextImpl(f state)
                let impl = htmlImpl :> Impl
                parent.Append impl

                { Impl = impl
                  Change = fun state -> htmlImpl.text.nodeValue <- f state // TODO use HTMLTextImpl methods
                  Destroy = fun () -> parent.Remove impl
                  Query = ignore }
            | Literal s ->
                let htmlImpl = HTMLTextImpl s
                let impl = htmlImpl :> Impl
                parent.Append impl

                { Impl = impl
                  Change = ignore
                  Destroy = fun () -> parent.Remove impl
                  Query = ignore }
