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

[<AbstractClass; Sealed>]
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

        let renderInstance = MakeDOMRender(dispatch)
        let f = renderInstance.Make template

        let render =
            f
            <| match doc with
               | Some doc -> DOMImpl(el, doc)
               | None -> DOMImpl(el)

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

            { Impl = view.Impl
              Change = view.Change
              Dispatch = dispatch
              Destroy = view.Destroy
              Query = view.Query }: ComponentView<DOMImpl, 'S, 'A, 'Q>

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

    static member Text() : DOMTemplate<string, 'A, 'Q> = id |> Derived |> DOMText |> Node
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

    static member on<'S, 'A>(name: string, action: 'A) : DOMAttribute<'S, 'A> =
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

    static member MapState<'S1, 'S2, 'A, 'Q>(f: 'S1 -> 'S2, template: DOMTemplate<'S2, 'A, 'Q>) =
        Template<DOMNode<'S1, 'A, 'Q>, DOMImpl, 'S1, 'A, 'Q>.MapState
            (packMapState<DOMNode<'S1, 'A, 'Q>, DOMNode<'S2, 'A, 'Q>, DOMImpl, 'S1, 'S2, 'A, 'Q>
             <| MapState<DOMNode<'S1, 'A, 'Q>, DOMNode<'S2, 'A, 'Q>, DOMImpl, 'S1, 'S2, 'A, 'Q>(f, template))


    static member OneOf<'S, 'S1, 'S2, 'A, 'Q>
        (
            f: 'S -> Choice<'S1, 'S2>,
            template1: DOMTemplate<'S1, 'A, 'Q>,
            template2: DOMTemplate<'S2, 'A, 'Q>
        ) =
        Template<DOMNode<'S, 'A, 'Q>, DOMImpl, 'S, 'A, 'Q>.OneOf2
            (packOneOf2<DOMNode<'S, 'A, 'Q>, DOMNode<'S1, 'A, 'Q>, DOMNode<'S2, 'A, 'Q>, DOMImpl, 'S, 'S1, 'S2, 'A, 'Q>
             <| OneOf2(f, template1, template2))
