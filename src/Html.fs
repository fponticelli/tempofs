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
                console.log child
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

                printfn $"Appending to Group {nodes.Length} nodes"

                List.iter (fun node -> parent.insertBefore (node, this.ref) |> ignore) nodes
            else
                failwith $"HTMLGroupImpl doesn't know how to append a child of type {child}"

        override this.Remove(child: Impl) =
            if hasProperty (child, "GetNodes") then
                let htmlChild = unsafeCast<HTMLImpl, Impl> (child)
                console.log ($"Before {this.children.Length}")
                this.children <- List.filter (fun c -> c <> child) this.children
                console.log ($"After {this.children.Length}")
                let ls = htmlChild.GetNodes()
                List.iter remove ls
            else
                failwith $"HTMLGroupImpl doesn't know how to append a child of type {child}"

    new(label: string) =
        counter <- counter + 1

        { ref = document.createComment $"{label}: {counter}"
          children = [] }

type HTMLTemplate<'S, 'A, 'Q> = Template<HTMLTemplateNode<'S, 'A, 'Q>, 'S, 'A, 'Q>

and HTMLTemplateNode<'S, 'A, 'Q> =
    | HTMLTemplateElement of HTMLTemplateElement<'S, 'A, 'Q>
    | HTMLTemplateText of Value<'S, string>

and HTMLTemplateElement<'S, 'A, 'Q> =
    { Name: string
      Attributes: HTMLTemplateAttribute<'S, 'A> list
      Children: HTMLTemplate<'S, 'A, 'Q> list }

and HTMLTemplateAttribute<'S, 'A> =
    { Name: string
      Value: HTMLTemplateAttributeValue<'S, 'A> }

and HTMLTemplateAttributeValue<'S, 'A> =
    | StringValue of Value<'S, string option>
    | TriggerValue of IHTMLTrigger<'S, 'A>

and IHTMLTrigger<'S, 'A> =
    abstract Accept : IHTMLTriggerInvoker<'S, 'A, 'R> -> 'R

and HTMLTrigger<'S, 'A, 'E when 'E :> Event>(handler) =
    member this.Handler : 'S -> 'E -> 'A = handler
    with
        interface IHTMLTrigger<'S, 'A> with
            member this.Accept f = f.Invoke<'E> this

and IHTMLTriggerInvoker<'S, 'A, 'R> =
    abstract Invoke<'E when 'E :> Event> : HTMLTrigger<'S, 'A, 'E> -> 'R

let inline attribute<'S, 'A> name value : HTMLTemplateAttribute<'S, 'A> = { Name = name; Value = value }

let packHTMLTrigger (trigger: HTMLTrigger<'S, 'A, 'E>) = trigger :> IHTMLTrigger<'S, 'A>

let unpackHTMLTrigger (trigger: IHTMLTrigger<'S, 'A>) (f: IHTMLTriggerInvoker<'S, 'A, 'R>) : 'R = trigger.Accept f

let makeTrigger<'S, 'A, 'E when 'E :> Event> (f: 'S -> 'E -> 'A) = packHTMLTrigger <| HTMLTrigger(f)

let applyStringAttribute (name: string) (el: HTMLElement) (s: string option) =
    match s with
    | Some s -> el.setAttribute (name, s)
    | None -> el.removeAttribute name

let derivedApplication ({ Name = name; Value = value }: HTMLTemplateAttribute<'S, 'A>) =
    match value with
    | StringValue (Derived f) ->
        Some
        <| fun el state -> applyStringAttribute name el (f state)
    | StringValue (Literal _) -> None
    | TriggerValue _ -> None

let applyAttribute (dispatch: 'A -> unit) (el: HTMLElement) (state: unit -> 'S) ({ Value = value; Name = name }: HTMLTemplateAttribute<'S, 'A>) =
    match value with
    | StringValue v ->
        applyStringAttribute name el
        <| Value.Resolve v (state ())
    | TriggerValue domTrigger ->
        unpackHTMLTrigger
            domTrigger
            { new IHTMLTriggerInvoker<'S, 'A, int> with
                override this.Invoke<'E when 'E :> Event>(t: HTMLTrigger<'S, 'A, 'E>) =
                    el.addEventListener (name, (fun e -> dispatch <| t.Handler(state ()) (e :?> 'E)))
                    0 }
        |> ignore

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

            printfn "DOMElement"
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
                printfn $"Text Node Derived: {htmlImpl.text.nodeValue}"
                parent.Append impl
                printfn "Text Node Derived (Appended)"

                { Impl = impl
                  Change = fun state -> htmlImpl.text.nodeValue <- f state // TODO use HTMLTextImpl methods
                  Destroy = fun () -> parent.Remove impl
                  Query = ignore }
            | Literal s ->
                let htmlImpl = HTMLTextImpl s
                let impl = htmlImpl :> Impl
                printfn "Text Node Literal"
                parent.Append impl
                printfn "Text Node Literal (Appended)"

                { Impl = impl
                  Change = ignore
                  Destroy = fun () -> parent.Remove impl
                  Query = ignore }
