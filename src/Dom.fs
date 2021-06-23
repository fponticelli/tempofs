module Tempo.Dom

open Browser
open Tempo.Core
open Tempo.Std
open Tempo.Dom.Helper
open Browser.Types

type DOMImplNode =
    | ElementI of HTMLElement
    | TextI of Text
    | RefI of Node * DOMImplNode list
    member this.GetNodes() : Types.Node list =
        match this with
        | ElementI el -> [ el :> Node ]
        | TextI t -> [ t :> Node ]
        | RefI (r, ls) ->
            List.concat (
                [ r ]
                :: (List.map (fun (i: DOMImplNode) -> i.GetNodes()) ls)
            )

    member this.HeadNode() : Types.Node =
        match this with
        | ElementI el -> el :> Node
        | TextI t -> t :> Node
        | RefI (r, _) -> r

    member this.InsertBefore(ref: DOMImplNode) =
        let toInsert = this.GetNodes()
        let refNode = ref.HeadNode()
        let parent = refNode.parentElement
        List.iter (fun item -> parent.insertBefore (item, refNode) |> ignore) toInsert

    member this.Remove() =
        let toRemove = this.GetNodes()
        List.iter remove toRemove

    member this.Append(child: DOMImplNode) =
        match this with
        | ElementI parent ->
            let toAppend = child.GetNodes()
            List.iter (fun item -> (parent.appendChild item |> ignore)) toAppend
        | TextI _
        | RefI _ -> child.InsertBefore this


type DOMImpl(node: DOMImplNode, doc: Document) =
    member this.Node = node
    member this.Doc = doc

    new(el: HTMLElement) = DOMImpl(ElementI el, ownerOrDocument el)
    new(el: HTMLElement, doc: Document) = DOMImpl(ElementI el, doc)
    new(text: Text) = DOMImpl(TextI text, ownerOrDocument text)
    new(text: Text, doc: Document) = DOMImpl(TextI text, doc)
    new(ref: Node, ls: DOMImplNode list) = DOMImpl(RefI(ref, ls), ownerOrDocument ref)
    new(ref: Node, ls: DOMImplNode list, doc: Document) = DOMImpl(RefI(ref, ls), doc)

    member this.Remove() = this.Node.Remove()

    member this.Append(child: DOMImpl) = this.Node.Append child.Node

type DOMTemplate<'S, 'A, 'Q> = Template<DOMNode<'S, 'A, 'Q>, DOMImpl, 'S, 'A, 'Q>

and DOMNode<'S, 'A, 'Q> =
    | DOMElement of DOMElement<'S, 'A, 'Q>
    | DOMText of Value<'S, string>

and DOMElement<'S, 'A, 'Q> =
    { Name: string
      Attributes: DOMAttribute<'S, 'A> list
      Children: DOMTemplate<'S, 'A, 'Q> list }

and DOMAttribute<'S, 'A> =
    { Name: string
      Value: DOMAttributeValue<'S, 'A> }

and DOMAttributeValue<'S, 'A> =
    | StringValue of Value<'S, string option>
    | TriggerValue of IDOMTrigger<'S, 'A>

and IDOMTrigger<'S, 'A> =
    abstract Accept : IDOMTriggerInvoker<'S, 'A, 'R> -> 'R

and DOMTrigger<'S, 'A, 'E when 'E :> Event>(handler) =
    member this.Handler : 'S -> 'E -> 'A = handler
    with
        interface IDOMTrigger<'S, 'A> with
            member this.Accept f = f.Invoke<'E> this

and IDOMTriggerInvoker<'S, 'A, 'R> =
    abstract Invoke<'E when 'E :> Event> : DOMTrigger<'S, 'A, 'E> -> 'R

let inline attribute<'S, 'A> name value : DOMAttribute<'S, 'A> = { Name = name; Value = value }

let packDOMTrigger (trigger: DOMTrigger<'S, 'A, 'E>) = trigger :> IDOMTrigger<'S, 'A>

let unpackDOMTrigger (trigger: IDOMTrigger<'S, 'A>) (f: IDOMTriggerInvoker<'S, 'A, 'R>) : 'R = trigger.Accept f

let makeTrigger<'S, 'A, 'E when 'E :> Event> (f: 'S -> 'E -> 'A) = packDOMTrigger <| DOMTrigger(f)

let applyStringAttribute (name: string) (el: HTMLElement) (s: string option) =
    match s with
    | Some s -> el.setAttribute (name, s)
    | None -> el.removeAttribute name

let derivedApplication ({ Name = name; Value = value }: DOMAttribute<'S, 'A>) =
    match value with
    | StringValue (Derived f) ->
        Some
        <| fun el state -> applyStringAttribute name el (f state)
    | StringValue (Literal _) -> None
    | TriggerValue _ -> None

let applyAttribute
    (dispatch: 'A -> unit)
    (el: HTMLElement)
    (state: unit -> 'S)
    ({ Value = value; Name = name }: DOMAttribute<'S, 'A>)
    =
    match value with
    | StringValue v ->
        applyStringAttribute name el
        <| Value.Resolve v (state ())
    | TriggerValue domTrigger ->
        unpackDOMTrigger
            domTrigger
            { new IDOMTriggerInvoker<'S, 'A, int> with
                override this.Invoke<'E when 'E :> Event>(t: DOMTrigger<'S, 'A, 'E>) =
                    el.addEventListener (name, (fun e -> dispatch <| t.Handler(state ()) (e :?> 'E)))
                    0 }
        |> ignore

type MakeDOMRender<'S, 'A, 'Q>(dispatch: 'A -> unit) =
    inherit MakeRender<DOMNode<'S, 'A, 'Q>, DOMImpl, 'S, 'A, 'Q>()

    override this.MakeNode(node) =
        match node with
        | DOMElement el -> this.MakeRenderDOMElement el
        | DOMText v -> this.MakeRenderDOMText v

    override this.MakeRef (parent: DOMImpl) (children: DOMImpl list) =
        let nodes =
            List.map (fun (child: DOMImpl) -> child.Node) children

        let ref = document.createTextNode ("")
        DOMImpl(ref, nodes, parent.Doc)

    override this.RemoveNode(node: DOMImpl) = node.Remove()
    override this.InsertBeforeNode (ref: DOMImpl) (toInsert: DOMImpl) = ref.Node.InsertBefore toInsert.Node
    override this.AppendNode (parent: DOMImpl) (child: DOMImpl) = parent.Append child

    member this.MakeRenderDOMElement(node: DOMElement<'S, 'A, 'Q>) : Render<DOMImpl, 'S, 'Q> =
        fun (parent: DOMImpl) (state: 'S) ->
            let mutable localState = state

            let el = document.createElement node.Name
            let impl = DOMImpl(el, parent.Doc)
            let getState () = localState

            List.iter (applyAttribute dispatch el getState) node.Attributes

            parent.Append impl

            let childViews =
                List.map (fun child -> this.Make child impl localState) node.Children

            let childUpdates =
                List.map (fun ({ Change = change }: View<_, _, _>) -> change) childViews

            let childDestroys =
                List.map (fun ({ Destroy = destroy }: View<_, _, _>) -> destroy) childViews

            let childQueries =
                List.map (fun ({ Query = query }: View<_, _, _>) -> query) childViews

            let attributeUpdates =
                List.filterMap derivedApplication node.Attributes
                |> List.map (fun f -> f el)

            let updates = attributeUpdates @ childUpdates

            let change =
                fun state ->
                    localState <- state
                    List.iter (fun change -> change localState) updates

            let destroy =
                fun () ->
                    remove el
                    List.iter (fun destroy -> destroy ()) childDestroys

            let query =
                fun (q: 'Q) -> List.iter (fun query -> query q) childQueries

            { Impl = DOMImpl(el, parent.Doc)
              Change = change
              Destroy = destroy
              Query = query }

    member this.MakeRenderDOMText(value: Value<'S, string>) : Render<DOMImpl, 'S, 'Q> =
        fun (parent: DOMImpl) (state: 'S) ->
            match value with
            | Derived f ->
                let n = parent.Doc.createTextNode <| f state
                let impl = DOMImpl(n, parent.Doc)
                parent.Append impl

                { Impl = impl
                  Change = fun state -> n.nodeValue <- f state
                  Destroy = impl.Remove
                  Query = ignore }
            | Literal s ->
                let n = parent.Doc.createTextNode s
                let impl = DOMImpl(n, parent.Doc)
                parent.Append impl

                { Impl = impl
                  Change = ignore
                  Destroy = impl.Remove
                  Query = ignore }
