module Tempo.Dom.DSL

open Tempo.Std
open Tempo.Core
open Tempo.Dom
open Browser.Types

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

    static member inline El<'S, 'A, 'Q>
        (
            name: string,
            children: DOMTemplate<'S, 'A, 'Q> list
        ) : DOMTemplate<'S, 'A, 'Q> =
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
