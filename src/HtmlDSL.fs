module Tempo.Html.DSL

open Tempo.Std
open Tempo.Core
open Tempo.Html
open Browser.Types

type MiddlewarePayload<'S, 'A> =
    { Current: 'S
      Old: 'S
      Action: 'A
      Dispatch: 'A -> unit }

[<AbstractClass; Sealed>]
type HTML =
    static member MakeProgram<'S, 'A, 'Q>(template: HTMLTemplate<'S, 'A, 'Q>, el: HTMLElement, ?audit: 'A -> unit) =
        let mutable invokes = Option.toList audit
        let dispatch action = List.iter (fun f -> f action) invokes

        let renderInstance = MakeHTMLRender(dispatch)
        let f = renderInstance.Make template
        let parent = HTMLElementImpl(el)

        let render = f parent

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
              Query = view.Query }: ComponentView<'S, 'A, 'Q>

    static member El<'S, 'A, 'Q>
        (
            name: string,
            attributes: HTMLTemplateAttribute<'S, 'A> list,
            children: HTMLTemplate<'S, 'A, 'Q> list
        ) : HTMLTemplate<'S, 'A, 'Q> =
        Node
        <| HTMLTemplateElement
            { Name = name
              Attributes = attributes
              Children = children }

    static member inline El<'S, 'A, 'Q>
        (
            name: string,
            attributes: HTMLTemplateAttribute<'S, 'A> list
        ) : HTMLTemplate<'S, 'A, 'Q> =
        HTML.El<'S, 'A, 'Q>(name, attributes, [])

    static member inline El<'S, 'A, 'Q>
        (
            name: string,
            children: HTMLTemplate<'S, 'A, 'Q> list
        ) : HTMLTemplate<'S, 'A, 'Q> =
        HTML.El<'S, 'A, 'Q>(name, [], children)

    static member inline El<'S, 'A, 'Q>
        (
            name: string,
            attributes: HTMLTemplateAttribute<'S, 'A> list,
            child: HTMLTemplate<'S, 'A, 'Q>
        ) : HTMLTemplate<'S, 'A, 'Q> =
        HTML.El<'S, 'A, 'Q>(name, attributes, [ child ])

    static member inline El<'S, 'A, 'Q>(name: string, child: HTMLTemplate<'S, 'A, 'Q>) : HTMLTemplate<'S, 'A, 'Q> =
        HTML.El<'S, 'A, 'Q>(name, [], [ child ])

    static member inline El<'S, 'A, 'Q>(name: string) : HTMLTemplate<'S, 'A, 'Q> = HTML.El<'S, 'A, 'Q>(name, [], [])

    static member inline El<'S, 'A, 'Q>(name: string, value: string) : HTMLTemplate<'S, 'A, 'Q> =
        HTML.El<'S, 'A, 'Q>(name, [], [ HTML.Text value ])

    static member inline El<'S, 'A, 'Q>(name: string, f: 'S -> string) : HTMLTemplate<'S, 'A, 'Q> =
        HTML.El<'S, 'A, 'Q>(name, [], [ HTML.Text f ])

    static member inline El<'S, 'A, 'Q>
        (
            name: string,
            attributes: HTMLTemplateAttribute<'S, 'A> list,
            value: string
        ) : HTMLTemplate<'S, 'A, 'Q> =
        HTML.El<'S, 'A, 'Q>(name, attributes, [ HTML.Text value ])

    static member inline El<'S, 'A, 'Q>
        (
            name: string,
            attributes: HTMLTemplateAttribute<'S, 'A> list,
            f: 'S -> string
        ) : HTMLTemplate<'S, 'A, 'Q> =
        HTML.El<'S, 'A, 'Q>(name, attributes, [ HTML.Text f ])

    static member Text() : HTMLTemplate<string, 'A, 'Q> =
        id |> Derived |> HTMLTemplateText |> Node

    static member Text<'S, 'A, 'Q>(value: string) : HTMLTemplate<'S, 'A, 'Q> =
        value |> Literal |> HTMLTemplateText |> Node

    static member Text<'S, 'A, 'Q>(f: 'S -> string) : HTMLTemplate<'S, 'A, 'Q> =
        f |> Derived |> HTMLTemplateText |> Node

    static member Attr<'S, 'A>(name: string, value: string option) : HTMLTemplateAttribute<'S, 'A> =
        attribute name (value |> Literal |> StringValue)

    static member Attr<'S, 'A>(name: string, value: string) : HTMLTemplateAttribute<'S, 'A> =
        attribute name (value |> String.ToOption |> Literal |> StringValue)

    static member Attr<'S, 'A>(name: string, f: 'S -> string option) : HTMLTemplateAttribute<'S, 'A> =
        attribute name (f |> Derived |> StringValue)

    static member Attr<'S, 'A>(name: string, f: 'S -> string) : HTMLTemplateAttribute<'S, 'A> =
        attribute name (f >> String.ToOption |> Derived |> StringValue)

    static member on<'S, 'A>(name: string, action: 'A) : HTMLTemplateAttribute<'S, 'A> =
        attribute name (makeTrigger (fun _ _ -> action) |> TriggerValue)

    static member On<'S, 'A>(name: string, handler: unit -> 'A) : HTMLTemplateAttribute<'S, 'A> =
        attribute
            name
            (makeTrigger (fun _ _ -> handler ())
             |> TriggerValue)

    static member On<'S, 'A, 'E when 'E :> Event>(name: string, handler: 'E -> 'A) : HTMLTemplateAttribute<'S, 'A> =
        attribute name (makeTrigger (fun _ e -> handler e) |> TriggerValue)

    static member On<'S, 'A, 'E when 'E :> Event>
        (
            name: string,
            handler: 'S -> 'E -> 'A
        ) : HTMLTemplateAttribute<'S, 'A> =
        attribute name (makeTrigger handler |> TriggerValue)

    static member On<'S, 'A>(name: string, handler: 'S -> 'A) : HTMLTemplateAttribute<'S, 'A> =
        attribute name (makeTrigger (fun s _ -> handler s) |> TriggerValue)

    static member MapState<'S1, 'S2, 'A, 'Q>(f: 'S1 -> 'S2, template: HTMLTemplate<'S2, 'A, 'Q>) =
        Template<HTMLTemplateNode<'S1, 'A, 'Q>, 'S1, 'A, 'Q>.MapState
            (packMapState<HTMLTemplateNode<'S1, 'A, 'Q>, HTMLTemplateNode<'S2, 'A, 'Q>, 'S1, 'S2, 'A, 'Q>
             <| MapState<HTMLTemplateNode<'S1, 'A, 'Q>, HTMLTemplateNode<'S2, 'A, 'Q>, 'S1, 'S2, 'A, 'Q>(f, template))

    static member MapState<'S1, 'S2, 'A, 'Q>(f: 'S1 -> 'S2, templates: HTMLTemplate<'S2, 'A, 'Q> list) =
        HTML.MapState(f, Fragment templates)


    static member OneOf<'S, 'S1, 'S2, 'A, 'Q>
        (
            f: 'S -> Choice<'S1, 'S2>,
            template1: HTMLTemplate<'S1, 'A, 'Q>,
            template2: HTMLTemplate<'S2, 'A, 'Q>
        ) : HTMLTemplate<'S, 'A, 'Q> =
        Template<HTMLTemplateNode<'S, 'A, 'Q>, 'S, 'A, 'Q>.OneOf2
            (packOneOf2<HTMLTemplateNode<'S, 'A, 'Q>, HTMLTemplateNode<'S1, 'A, 'Q>, HTMLTemplateNode<'S2, 'A, 'Q>, 'S, 'S1, 'S2, 'A, 'Q>
             <| OneOf2(f, template1, template2))


    static member OneOf<'S, 'S1, 'S2, 'S3, 'A, 'Q>
        (
            f: 'S -> Choice<'S1, 'S2, 'S3>,
            template1: HTMLTemplate<'S1, 'A, 'Q>,
            template2: HTMLTemplate<'S2, 'A, 'Q>,
            template3: HTMLTemplate<'S3, 'A, 'Q>
        ) : HTMLTemplate<'S, 'A, 'Q> =
        HTML.OneOf(
            (fun s ->
                match f s with
                | Choice1Of3 c -> Choice1Of2 c
                | Choice2Of3 c -> Choice2Of2 <| Choice1Of2 c
                | Choice3Of3 c -> Choice2Of2 <| Choice2Of2 c),
            template1,
            HTML.OneOf<Choice<'S2, 'S3>, 'S2, 'S3, 'A, 'Q>(id, template2, template3)
        )


    static member OneOf<'S, 'S1, 'S2, 'S3, 'S4, 'A, 'Q>
        (
            f: 'S -> Choice<'S1, 'S2, 'S3, 'S4>,
            template1: HTMLTemplate<'S1, 'A, 'Q>,
            template2: HTMLTemplate<'S2, 'A, 'Q>,
            template3: HTMLTemplate<'S3, 'A, 'Q>,
            template4: HTMLTemplate<'S4, 'A, 'Q>
        ) : HTMLTemplate<'S, 'A, 'Q> =
        HTML.OneOf(
            (fun s ->
                match f s with
                | Choice1Of4 c -> Choice1Of2 <| Choice1Of2 c
                | Choice2Of4 c -> Choice1Of2 <| Choice2Of2 c
                | Choice3Of4 c -> Choice2Of2 <| Choice1Of2 c
                | Choice4Of4 c -> Choice2Of2 <| Choice2Of2 c),
            HTML.OneOf<Choice<'S1, 'S2>, 'S1, 'S2, 'A, 'Q>(id, template1, template2),
            HTML.OneOf<Choice<'S3, 'S4>, 'S3, 'S4, 'A, 'Q>(id, template3, template4)
        )

    static member OneOf<'S, 'S1, 'S2, 'S3, 'S4, 'S5, 'A, 'Q>
        (
            f: 'S -> Choice<'S1, 'S2, 'S3, 'S4, 'S5>,
            template1: HTMLTemplate<'S1, 'A, 'Q>,
            template2: HTMLTemplate<'S2, 'A, 'Q>,
            template3: HTMLTemplate<'S3, 'A, 'Q>,
            template4: HTMLTemplate<'S4, 'A, 'Q>,
            template5: HTMLTemplate<'S5, 'A, 'Q>
        ) : HTMLTemplate<'S, 'A, 'Q> =
        HTML.OneOf(
            (fun s ->
                match f s with
                | Choice1Of5 c -> Choice1Of2 <| Choice1Of2 c
                | Choice2Of5 c -> Choice1Of2 <| Choice2Of2 c
                | Choice3Of5 c -> Choice2Of2 <| Choice1Of3 c
                | Choice4Of5 c -> Choice2Of2 <| Choice2Of3 c
                | Choice5Of5 c -> Choice2Of2 <| Choice3Of3 c),
            HTML.OneOf<Choice<'S1, 'S2>, 'S1, 'S2, 'A, 'Q>(id, template1, template2),
            HTML.OneOf<Choice<'S3, 'S4, 'S5>, 'S3, 'S4, 'S5, 'A, 'Q>(id, template3, template4, template5)
        )

    static member OneOf<'S, 'S1, 'S2, 'S3, 'S4, 'S5, 'S6, 'A, 'Q>
        (
            f: 'S -> Choice<'S1, 'S2, 'S3, 'S4, 'S5, 'S6>,
            template1: HTMLTemplate<'S1, 'A, 'Q>,
            template2: HTMLTemplate<'S2, 'A, 'Q>,
            template3: HTMLTemplate<'S3, 'A, 'Q>,
            template4: HTMLTemplate<'S4, 'A, 'Q>,
            template5: HTMLTemplate<'S5, 'A, 'Q>,
            template6: HTMLTemplate<'S6, 'A, 'Q>
        ) : HTMLTemplate<'S, 'A, 'Q> =
        HTML.OneOf(
            (fun s ->
                match f s with
                | Choice1Of6 c -> Choice1Of2 <| Choice1Of3 c
                | Choice2Of6 c -> Choice1Of2 <| Choice2Of3 c
                | Choice3Of6 c -> Choice1Of2 <| Choice3Of3 c
                | Choice4Of6 c -> Choice2Of2 <| Choice1Of3 c
                | Choice5Of6 c -> Choice2Of2 <| Choice2Of3 c
                | Choice6Of6 c -> Choice2Of2 <| Choice3Of3 c),
            HTML.OneOf<Choice<'S1, 'S2, 'S3>, 'S1, 'S2, 'S3, 'A, 'Q>(id, template1, template2, template3),
            HTML.OneOf<Choice<'S4, 'S5, 'S6>, 'S4, 'S5, 'S6, 'A, 'Q>(id, template4, template5, template6)
        )

    static member OneOf<'S, 'S1, 'S2, 'S3, 'S4, 'S5, 'S6, 'S7, 'A, 'Q>
        (
            f: 'S -> Choice<'S1, 'S2, 'S3, 'S4, 'S5, 'S6, 'S7>,
            template1: HTMLTemplate<'S1, 'A, 'Q>,
            template2: HTMLTemplate<'S2, 'A, 'Q>,
            template3: HTMLTemplate<'S3, 'A, 'Q>,
            template4: HTMLTemplate<'S4, 'A, 'Q>,
            template5: HTMLTemplate<'S5, 'A, 'Q>,
            template6: HTMLTemplate<'S6, 'A, 'Q>,
            template7: HTMLTemplate<'S7, 'A, 'Q>
        ) : HTMLTemplate<'S, 'A, 'Q> =
        HTML.OneOf(
            (fun s ->
                match f s with
                | Choice1Of7 c -> Choice1Of2 <| Choice1Of3 c
                | Choice2Of7 c -> Choice1Of2 <| Choice2Of3 c
                | Choice3Of7 c -> Choice1Of2 <| Choice3Of3 c
                | Choice4Of7 c -> Choice2Of2 <| Choice1Of4 c
                | Choice5Of7 c -> Choice2Of2 <| Choice2Of4 c
                | Choice6Of7 c -> Choice2Of2 <| Choice3Of4 c
                | Choice7Of7 c -> Choice2Of2 <| Choice4Of4 c),
            HTML.OneOf<Choice<'S1, 'S2, 'S3>, 'S1, 'S2, 'S3, 'A, 'Q>(id, template1, template2, template3),
            HTML.OneOf<Choice<'S4, 'S5, 'S6, 'S7>, 'S4, 'S5, 'S6, 'S7, 'A, 'Q>(
                id,
                template4,
                template5,
                template6,
                template7
            )
        )
