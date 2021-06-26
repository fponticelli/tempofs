module Tempo.Html

open Browser
open Tempo.Core
open Tempo.Std
open Tempo.Html.Helper
open Browser.Types

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


type HTMLTemplate<'S, 'A, 'Q> = Template<HTMLTemplateNode<'S, 'A, 'Q>, 'S, 'A, 'Q>

and HTMLTemplateNode<'S, 'A, 'Q> =
    | HTMLTemplateElement of HTMLTemplateElement<'S, 'A, 'Q>
    | HTMLTemplateText of Value<'S, string>

and HTMLTemplateElement<'S, 'A, 'Q> =
    { Name: string
      Attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list
      Children: HTMLTemplate<'S, 'A, 'Q> list }

and HTMLTemplateAttribute<'S, 'A, 'Q> =
    { Name: string
      Value: HTMLTemplateAttributeValue<'S, 'A, 'Q> }

and HTMLTemplateAttributeValue<'S, 'A, 'Q> =
    | StringValue of Value<'S, string option>
    | TriggerValue of IHTMLTrigger<'S, 'A>
    | LifecycleValue of ILifecycleValue<'S, 'Q>

and IHTMLTrigger<'S, 'A> =
    abstract Accept : IHTMLTriggerInvoker<'S, 'A, 'R> -> 'R

and TriggerPayload<'S, 'E, 'EL when 'E :> Event and 'EL :> Element> = { State: 'S; Event: 'E; Element: 'EL }

and HTMLTrigger<'S, 'A, 'E, 'EL when 'E :> Event and 'EL :> Element>(handler) =
    member this.Handler : TriggerPayload<'S, 'E, 'EL> -> 'A = handler
    with
        interface IHTMLTrigger<'S, 'A> with
            member this.Accept f = f.Invoke<'E, 'EL> this

and IHTMLTriggerInvoker<'S, 'A, 'R> =
    abstract Invoke<'E, 'EL when 'E :> Event and 'EL :> Element> : HTMLTrigger<'S, 'A, 'E, 'EL> -> 'R


and ILifecycleValue<'S, 'Q> =
    abstract Accept : ILifecycleValueInvoker<'S, 'Q, 'R> -> 'R

and LifecycleValueInitialPayload<'S, 'Q, 'EL when 'EL :> Element> = { State: 'S; Element: 'EL }

and LifecycleValuePayload<'S, 'Q, 'EL, 'P when 'EL :> Element> =
    { State: 'S
      Element: 'EL
      Payload: 'P }

and LifecycleValue<'S, 'Q, 'EL, 'P when 'EL :> Element>(afterRender, beforeChange, afterChange, beforeDestroy, respond) =
    member this.AfterRender : LifecycleValueInitialPayload<'S, 'Q, 'EL> -> 'P = afterRender
    member this.BeforeChange : LifecycleValuePayload<'S, 'Q, 'EL, 'P> -> (bool * 'P) = beforeChange
    member this.AfterChange : LifecycleValuePayload<'S, 'Q, 'EL, 'P> -> 'P = afterChange
    member this.BeforeDestroy : LifecycleValuePayload<'S, 'Q, 'EL, 'P> -> 'P = beforeDestroy
    member this.Respond : LifecycleValuePayload<'S, 'Q, 'EL, 'P> -> 'P = respond
    with
        interface ILifecycleValue<'S, 'Q> with
            member this.Accept f = f.Invoke<'EL, 'P> this

and ILifecycleValueInvoker<'S, 'Q, 'R> =
    abstract Invoke<'EL, 'P when 'EL :> Element> : LifecycleValue<'S, 'Q, 'EL, 'P> -> 'R

let inline attribute<'S, 'A, 'Q> name value : HTMLTemplateAttribute<'S, 'A, 'Q> = { Name = name; Value = value }

let packHTMLTrigger (trigger: HTMLTrigger<'S, 'A, 'E, 'EL>) = trigger :> IHTMLTrigger<'S, 'A>

let unpackHTMLTrigger (trigger: IHTMLTrigger<'S, 'A>) (f: IHTMLTriggerInvoker<'S, 'A, 'R>) : 'R = trigger.Accept f

let makeTrigger<'S, 'A, 'E, 'EL when 'E :> Event and 'EL :> Element> (f: TriggerPayload<'S, 'E, 'EL> -> 'A) = packHTMLTrigger <| HTMLTrigger(f)

let applyStringAttribute (name: string) (el: HTMLElement) (s: string option) =
    match s with
    | Some s -> el.setAttribute (name, s)
    | None -> el.removeAttribute name

let derivedApplication ({ Name = name; Value = value }: HTMLTemplateAttribute<'S, 'A, 'Q>) =
    match value with
    | StringValue (Derived f) ->
        Some
        <| fun el state -> applyStringAttribute name el (f state)
    | StringValue (Literal _) -> None
    | TriggerValue _ -> None
    | LifecycleValue _ -> None

let applyAttribute (dispatch: 'A -> unit) (el: HTMLElement) (state: unit -> 'S) ({ Value = value; Name = name }: HTMLTemplateAttribute<'S, 'A, 'Q>) =
    match value with
    | StringValue v ->
        applyStringAttribute name el
        <| Value.Resolve v (state ())
    | TriggerValue domTrigger ->
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
    | LifecycleValue lc -> failwith "ainono"

type MakeHTMLRender<'S, 'A, 'Q>(dispatch: 'A -> unit) =
    inherit MakeRender<HTMLTemplateNode<'S, 'A, 'Q>, 'S, 'A, 'Q>()

    override this.MakeNodeRender(node) =
        match node with
        | HTMLTemplateElement el -> this.MakeRenderDOMElement el
        | HTMLTemplateText v -> this.MakeRenderDOMText v

    override this.CreateGroupNode(label: string) = HTMLGroupImpl(label) :> Impl

    member this.MakeRenderDOMElement(node: HTMLTemplateElement<'S, 'A, 'Q>) : Render<'S, 'Q> =
        fun (parent: Impl) (state: 'S) ->
            let mutable localState = state

            let htmlImpl = HTMLElementImpl node.Name
            let impl = htmlImpl :> Impl
            let getState () = localState

            // TODO use HTMLElementImpl methods
            List.iter (applyAttribute dispatch htmlImpl.element getState) node.Attributes
            parent.Append impl

            // TODO use HTMLElementImpl methods
            let childViews =
                List.map (fun child -> this.Make child impl localState) node.Children

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

    member this.MakeRenderDOMText(value: Value<'S, string>) : Render<'S, 'Q> =
        fun (parent: Impl) (state: 'S) ->
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
