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
        val element: HTMLElement
        inherit HTMLImpl
        override this.GetNodes() = [ this.element :> Node ]

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

        new(el: HTMLElement) = { element = el }
        new(name: string) = HTMLElementImpl(document.createElement name)

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
                    failwith $"HTMLGroupImpl doesn't know how to append a child of type {child}"

        new(label: string) =
#if DEBUG
            counter <- counter + 1

            { ref = document.createComment $"{label}: {counter}"
              children = [] }
#else
            { ref = document.createTextNode ""
              children = [] }
#endif

    type LifecycleImpl<'S, 'Q> =
        { BeforeChange: 'S -> Element -> bool
          AfterChange: 'S -> Element -> unit
          BeforeDestroy: 'S -> Element -> unit
          Respond: 'Q -> 'S -> Element -> unit }

    let inline attribute<'S, 'A, 'Q> name value : HTMLTemplateAttribute<'S, 'A, 'Q> = { Name = name; Value = value }

    let packHTMLTrigger (trigger: HTMLTrigger<'S, 'A, 'E, 'EL>) = trigger :> IHTMLTrigger<'S, 'A>

    let unpackHTMLTrigger (trigger: IHTMLTrigger<'S, 'A>) (f: IHTMLTriggerInvoker<'S, 'A, 'R>) : 'R = trigger.Accept f

    let makeTrigger<'S, 'A, 'E, 'EL when 'E :> Event and 'EL :> Element> (f: TriggerPayload<'S, 'E, 'EL> -> 'A) = packHTMLTrigger <| HTMLTrigger(f)

    let packProperty (trigger: Property<'S, 'V>) = trigger :> IProperty<'S>

    let unpackProperty (trigger: IProperty<'S>) (f: IPropertyInvoker<'S, 'R>) : 'R = trigger.Accept f

    let inline property<'S, 'A, 'Q, 'V> (name: string) (value: Value<'S, 'V>) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        { Name = name
          Value =
              HTMLTemplateAttributeValue.Property
              <| (packProperty <| Property(name, value)) }

    let packHTMLLifecycle (lifecycle: HTMLLifecycle<'S, 'Q, 'EL, 'P>) = lifecycle :> IHTMLLifecycle<'S, 'Q>

    let unpackHTMLLifecycle (lifecycle: IHTMLLifecycle<'S, 'Q>) (f: IHTMLLifecycleInvoker<'S, 'Q, 'R>) : 'R = lifecycle.Accept f

    let makeLifecycle<'S, 'Q, 'EL, 'P when 'EL :> Element> (afterRender: HTMLLifecycleInitialPayload<'S, 'EL> -> 'P, beforeChange: HTMLLifecyclePayload<'S, 'EL, 'P> -> (bool * 'P), afterChange: HTMLLifecyclePayload<'S, 'EL, 'P> -> 'P, beforeDestroy: HTMLLifecyclePayload<'S, 'EL, 'P> -> unit, respond: 'Q -> HTMLLifecyclePayload<'S, 'EL, 'P> -> 'P) =
        packHTMLLifecycle
        <| HTMLLifecycle(afterRender, beforeChange, afterChange, beforeDestroy, respond)

    let applyStringAttribute (name: string) (el: HTMLElement) (s: string option) =
        match s with
        | Some s -> el.setAttribute (name, s)
        | None -> el.removeAttribute name

    let applyTrigger domTrigger name el dispatch state =
        unpackHTMLTrigger
            domTrigger
            { new IHTMLTriggerInvoker<'S, 'A, int> with
                override this.Invoke<'E, 'EL when 'E :> Event and 'EL :> Element>(t: HTMLTrigger<'S, 'A, 'E, 'EL>) =
                    let el = (el :> Element :?> 'EL)

                    el.addEventListener (
                        name,
                        (fun e ->
                            dispatch
                            <| t.Handler(
                                { State = state ()
                                  Event = (e :?> 'E)
                                  Element = el }
                            ))
                    )

                    0 }
        |> ignore

    let applyProperty<'S> (prop: IProperty<'S>) (el: HTMLElement) (state) =
        unpackProperty
            prop
            { new IPropertyInvoker<'S, int> with
                override this.Invoke<'V>(prop: Property<'S, 'V>) =
                    assign el prop.Name
                    <| Value.Resolve prop.Value state

                    0 }
        |> ignore

    let extractDerivedProperty<'S> (prop: IProperty<'S>) =
        unpackProperty
            prop
            { new IPropertyInvoker<'S, (HTMLElement -> 'S -> unit) option> with
                override this.Invoke<'V>(prop: Property<'S, 'V>) =
                    match prop.Value with
                    | Derived f -> Some(fun el state -> assign el prop.Name (f state))
                    | Literal _ -> None }

    let derivedApplication ({ Name = name; Value = value }: HTMLTemplateAttribute<'S, 'A, 'Q>) =
        match value with
        | StringAttr (Derived f) ->
            Some
            <| fun el state -> applyStringAttribute name el (f state)
        | Property prop -> extractDerivedProperty prop // TODO
        | StringAttr (Literal _) -> None
        | Trigger _ -> None
        | Lifecycle _ -> None

    let applyAttribute (dispatch: 'A -> unit) (el: HTMLElement) (state: unit -> 'S) ({ Value = value; Name = name }: HTMLTemplateAttribute<'S, 'A, 'Q>) =
        match value with
        | StringAttr v ->
            applyStringAttribute name el
            <| Value.Resolve v (state ())
        | Property prop -> applyProperty prop el (state ())
        | Trigger domTrigger -> applyTrigger domTrigger name el dispatch state
        | Lifecycle _ -> // TODO
            ()

    let extractLifecycle lc =
        unpackHTMLLifecycle
            lc
            { new IHTMLLifecycleInvoker<'S, 'Q, Element -> 'S -> LifecycleImpl<'S, 'Q>> with
                override this.Invoke<'EL, 'P when 'EL :> Element>(t: HTMLLifecycle<'S, 'Q, 'EL, 'P>) : Element -> 'S -> LifecycleImpl<'S, 'Q> =
                    fun (el: Element) (state: 'S) ->
                        let mutable payload : 'P =
                            t.AfterRender { State = state; Element = el :?> 'EL }

                        let beforeChange state (el: Element) =
                            let (result, newPayload) =
                                t.BeforeChange
                                    { State = state
                                      Element = el :?> 'EL
                                      Payload = payload }

                            payload <- newPayload
                            result

                        let afterChange state (el: Element) =
                            payload <-
                                t.AfterChange
                                    { State = state
                                      Element = el :?> 'EL
                                      Payload = payload }

                        let beforeDestroy state (el: Element) =
                            t.BeforeDestroy
                                { State = state
                                  Element = el :?> 'EL
                                  Payload = payload }

                        let respond query state (el: Element) =
                            payload <-
                                t.Respond
                                    query
                                    { State = state
                                      Element = el :?> 'EL
                                      Payload = payload }

                        { BeforeChange = beforeChange
                          AfterChange = afterChange
                          BeforeDestroy = beforeDestroy
                          Respond = respond } }



    let mergeLifecycles ls =
        let merge a b =
            { BeforeChange =
                  fun s el ->
                      let ra = a.BeforeChange s el
                      let rb = b.BeforeChange s el
                      ra || rb
              AfterChange =
                  fun s el ->
                      a.AfterChange s el
                      b.AfterChange s el
              BeforeDestroy =
                  fun s el ->
                      a.BeforeDestroy s el
                      b.BeforeDestroy s el
              Respond =
                  fun q s el ->
                      a.Respond q s el
                      b.Respond q s el }

        let start =
            { BeforeChange = fun _ _ -> true
              AfterChange = fun _ _ -> ()
              BeforeDestroy = fun _ _ -> ()
              Respond = fun _ _ _ -> () }

        List.fold (fun acc curr -> acc) start ls

    let createGroupNode (label: string) = HTMLGroupImpl(label) :> Impl

    let rec makeHTMLNodeRender<'S, 'A, 'Q> (make: Template<HTMLTemplateNode<'S, 'A, 'Q>, 'S, 'A, 'Q> -> Render<'S, 'A, 'Q>) (node: HTMLTemplateNode<'S, 'A, 'Q>) : Render<'S, 'A, 'Q> =
        match node with
        | HTMLTemplateElement el -> makeRenderDOMElement el make
        | HTMLTemplateText v -> makeRenderDOMText v

    and makeRenderDOMElement (node: HTMLTemplateElement<'S, 'A, 'Q>) make : Render<'S, 'A, 'Q> =
        fun (parent: Impl) (state: 'S) dispatch ->
            let mutable localState = state

            let htmlImpl = HTMLElementImpl node.Name
            let impl = htmlImpl :> Impl
            let getState () = localState

            // TODO use HTMLElementImpl methods
            List.iter (applyAttribute dispatch htmlImpl.element getState) node.Attributes
            parent.Append impl

            // TODO use HTMLElementImpl methods
            let childViews =
                List.map (fun child -> make child impl localState dispatch) node.Children

            let childUpdates =
                List.map (fun ({ Change = change }: View<_, _>) -> change) childViews

            let childDestroys =
                List.map (fun ({ Destroy = destroy }: View<_, _>) -> destroy) childViews

            let childQueries =
                List.map (fun ({ Query = query }: View<_, _>) -> query) childViews

            // TODO use HTMLElementImpl methods
            let attributeUpdates =
                List.filterMap derivedApplication node.Attributes
                |> List.map (fun f -> f htmlImpl.element)

            let updates = attributeUpdates @ childUpdates

            let change =
                fun state ->
                    localState <- state
                    List.iter (fun change -> change localState) updates

            let destroy =
                fun () ->
                    parent.Remove(impl)
                    List.iter (fun destroy -> destroy ()) childDestroys

            let query =
                fun (q: 'Q) -> List.iter (fun query -> query q) childQueries

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
