module Tempo.Dom

open Browser
open Tempo.Core
open Tempo.Utils
open Tempo.Dom.Helper
open Browser.Types

type DOMImpl(el: HTMLElement, doc: Document option) =
    member this.Element = el

    member this.Doc =
        Option.defaultValue (ownerOrDocument this.Element) doc

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


let rec makeRenderDOMNode<'S, 'A, 'Q> (dispatch: 'A -> unit) (node: DOMNode<'S, 'A, 'Q>) : Render<DOMImpl, 'S, 'Q> =
    match node with
    | DOMElement el -> makeRenderDOMElement el dispatch
    | DOMText v -> makeRenderDOMText v

and makeRenderDOMElement (node: DOMElement<'S, 'A, 'Q>) (dispatch: 'A -> unit) : Render<DOMImpl, 'S, 'Q> =
    fun (parent: DOMImpl) (state: 'S) ->
        let mutable localState = state

        let el = document.createElement node.Name
        let impl = DOMImpl(el, Some parent.Doc)
        let getState () = localState

        List.iter (applyAttribute dispatch el getState) node.Attributes

        let childRender = makeRender (makeRenderDOMNode dispatch)

        parent.Element.appendChild el |> ignore

        let childViews =
            List.map (fun child -> childRender child impl localState) node.Children

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

        { Impl = Some impl
          Change = change
          Destroy = destroy
          Query = query }

and makeRenderDOMText (value: Value<'S, string>) : Render<DOMImpl, 'S, 'Q> =
    fun (impl: DOMImpl) (state: 'S) ->
        let (change, destroy) =
            match value with
            | Derived f ->
                let n = impl.Doc.createTextNode <| f state
                impl.Element.appendChild n |> ignore
                ((fun state -> n.nodeValue <- f state), (fun () -> remove n))
            | Literal s ->
                let n = impl.Doc.createTextNode s
                impl.Element.appendChild n |> ignore
                (ignore, (fun () -> remove n))

        { Impl = None
          Change = change
          Destroy = destroy
          Query = ignore }

type MiddlewarePayload<'S, 'A> =
    { Current: 'S
      Old: 'S
      Action: 'A
      Dispatch: 'A -> unit }

type DOM =
    static member Make<'S, 'A, 'Q>
        (
            template: DOMTemplate<'S, 'A, 'Q>,
            el: HTMLElement,
            ?audit: 'A -> unit,
            ?doc: Document
        ) =
        let mutable invokes = Option.toList audit
        let dispatch action = List.iter (fun f -> f action) invokes

        let f =
            makeRender<DOMNode<'S, 'A, 'Q>, DOMImpl, 'S, 'A, 'Q> (makeRenderDOMNode dispatch) template

        let render = f <| DOMImpl(el, doc)

        fun update middleware state ->
            let mutable localState = state
            let view = render localState

            let updateDispatch action =
                let newState = update localState action
                view.Change newState

                middleware
                    { Old = localState
                      Current = newState
                      Action = action
                      Dispatch = dispatch }

                localState <- newState

            invokes <- updateDispatch :: invokes

            { Impl = None
              Change = view.Change
              Dispatch = dispatch
              Destroy = view.Destroy
              Query = view.Query }: ComponentView<HTMLElement, 'S, 'A, 'Q>

    static member El<'S, 'A, 'Q>
        (
            name: string,
            attributes: DOMAttribute<'S, 'A> list,
            children: DOMTemplate<'S, 'A, 'Q> list
        ) : DOMTemplate<'S, 'A, 'Q> =
        Node
        <| DOMElement
            { Name = name
              Attributes = attributes
              Children = children }

    static member inline El<'S, 'A, 'Q>(name: string, attributes: DOMAttribute<'S, 'A> list) : DOMTemplate<'S, 'A, 'Q> =
        DOM.El<'S, 'A, 'Q>(name, attributes, [])

    static member El<'S, 'A, 'Q>(name: string, children: DOMTemplate<'S, 'A, 'Q> list) : DOMTemplate<'S, 'A, 'Q> =
        DOM.El<'S, 'A, 'Q>(name, [], children)

    static member Text<'S, 'A, 'Q>(value: string) : DOMTemplate<'S, 'A, 'Q> = value |> Literal |> DOMText |> Node
    static member Text<'S, 'A, 'Q>(f: 'S -> string) : DOMTemplate<'S, 'A, 'Q> = f |> Derived |> DOMText |> Node

    static member Attr<'S, 'A>(name: string, value: string option) : DOMAttribute<'S, 'A> =
        attribute name (value |> Literal |> StringValue)

    static member Attr<'S, 'A>(name: string, value: string) : DOMAttribute<'S, 'A> =
        attribute name (value |> String.ToOption |> Literal |> StringValue)

    static member Attr<'S, 'A>(name: string, f: 'S -> string option) : DOMAttribute<'S, 'A> =
        attribute name (f |> Derived |> StringValue)

    static member Attr<'S, 'A>(name: string, f: 'S -> string) : DOMAttribute<'S, 'A> =
        attribute name (f >> String.ToOption |> Derived |> StringValue)

    static member On<'S, 'A>(name: string, action: 'A) : DOMAttribute<'S, 'A> =
        attribute name (makeTrigger (fun _ _ -> action) |> TriggerValue)

    static member On<'S, 'A>(name: string, handler: unit -> 'A) : DOMAttribute<'S, 'A> =
        attribute
            name
            (makeTrigger (fun _ _ -> handler ())
             |> TriggerValue)

    static member On<'S, 'A, 'E when 'E :> Event>(name: string, handler: 'E -> 'A) : DOMAttribute<'S, 'A> =
        attribute name (makeTrigger (fun _ e -> handler e) |> TriggerValue)

    static member On<'S, 'A, 'E when 'E :> Event>(name: string, handler: 'S -> 'E -> 'A) : DOMAttribute<'S, 'A> =
        attribute name (makeTrigger handler |> TriggerValue)

    static member On<'S, 'A>(name: string, handler: 'S -> 'A) : DOMAttribute<'S, 'A> =
        attribute name (makeTrigger (fun s _ -> handler s) |> TriggerValue)
