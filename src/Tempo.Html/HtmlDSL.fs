namespace Tempo.Html

open Tempo.Std
open Tempo.Core
open Tempo.Html
open Tempo.Html.Impl
open Browser.Types


module DSL =
    type MiddlewarePayload<'S, 'A> =
        { Current: 'S
          Previous: 'S
          Action: 'A
          Dispatch: 'A -> unit }

    [<AbstractClass; Sealed>]
    type HTML =
        static member MakeProgram<'S, 'A, 'Q>(template: HTMLTemplate<'S, 'A, 'Q>, el: HTMLElement) =
            let renderInstance = MakeHTMLRender()
            let f = renderInstance.Make template
            let parent = HTMLElementImpl(el)

            let render = f parent

            fun update middleware state ->
                let mutable localState = state

                let rec dispatch action =
                    let newState = update localState action
                    view.Change newState

                    middleware
                        { Previous = localState
                          Current = newState
                          Action = action
                          Dispatch = dispatch }

                    localState <- newState

                and view = render localState dispatch


                { Impl = view.Impl
                  Change = view.Change
                  Dispatch = dispatch
                  Destroy = view.Destroy
                  Query = view.Query }: ComponentView<'S, 'A, 'Q>

        static member El<'S, 'A, 'Q>
            (
                name: string,
                attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list,
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
                attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list
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
                attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list,
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
                attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list,
                value: string
            ) : HTMLTemplate<'S, 'A, 'Q> =
            HTML.El<'S, 'A, 'Q>(name, attributes, [ HTML.Text value ])

        static member inline El<'S, 'A, 'Q>
            (
                name: string,
                attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list,
                f: 'S -> string
            ) : HTMLTemplate<'S, 'A, 'Q> =
            HTML.El<'S, 'A, 'Q>(name, attributes, [ HTML.Text f ])

        static member Text() : HTMLTemplate<string, 'A, 'Q> =
            id |> Derived |> HTMLTemplateText |> Node

        static member Text<'S, 'A, 'Q>(value: string) : HTMLTemplate<'S, 'A, 'Q> =
            value |> Literal |> HTMLTemplateText |> Node

        static member Text<'S, 'A, 'Q>(f: 'S -> string) : HTMLTemplate<'S, 'A, 'Q> =
            f |> Derived |> HTMLTemplateText |> Node

        static member Attr<'S, 'A, 'Q>(name: string, value: string option) : HTMLTemplateAttribute<'S, 'A, 'Q> =
            attribute name (value |> Literal |> StringValue)

        static member Attr<'S, 'A, 'Q>(name: string, value: string) : HTMLTemplateAttribute<'S, 'A, 'Q> =
            attribute name (value |> Some |> Literal |> StringValue)

        static member Attr<'S, 'A, 'Q>(name: string, f: 'S -> string option) : HTMLTemplateAttribute<'S, 'A, 'Q> =
            attribute name (f |> Derived |> StringValue)

        static member Attr<'S, 'A, 'Q>(name: string, f: 'S -> string) : HTMLTemplateAttribute<'S, 'A, 'Q> =
            attribute name (f >> Some |> Derived |> StringValue)

        static member On<'S, 'A, 'Q>(name: string, action: 'A) : HTMLTemplateAttribute<'S, 'A, 'Q> =
            attribute name (makeTrigger (fun _ -> action) |> TriggerValue)

        static member On<'S, 'A, 'Q>(name: string, handler: unit -> 'A) : HTMLTemplateAttribute<'S, 'A, 'Q> =
            attribute name (makeTrigger (fun _ -> handler ()) |> TriggerValue)

        static member On<'S, 'A, 'Q, 'EL, 'E when 'E :> Event and 'EL :> Element>
            (
                name: string,
                handler: TriggerPayload<'S, 'E, 'EL> -> 'A
            ) : HTMLTemplateAttribute<'S, 'A, 'Q> =
            attribute name (makeTrigger handler |> TriggerValue)

        static member inline MapState<'S1, 'S2, 'A, 'Q>
            (
                f: 'S1 -> 'S2,
                template: HTMLTemplate<'S2, 'A, 'Q>
            ) : HTMLTemplate<'S1, 'A, 'Q> =
            (mapState f template)

        static member inline MapState<'S1, 'S2, 'A, 'Q>
            (
                f: 'S1 -> 'S2,
                templates: HTMLTemplate<'S2, 'A, 'Q> list
            ) : HTMLTemplate<'S1, 'A, 'Q> =
            (mapState f <| Fragment templates)

        static member inline MapAction<'S, 'A1, 'A2, 'Q>
            (
                f: 'A2 -> 'A1,
                template: HTMLTemplate<'S, 'A2, 'Q>
            ) : HTMLTemplate<'S, 'A1, 'Q> =
            mapAction f template

        static member inline MapAction<'S, 'A1, 'A2, 'Q>
            (
                f: 'A2 -> 'A1,
                templates: HTMLTemplate<'S, 'A2, 'Q> list
            ) : HTMLTemplate<'S, 'A1, 'Q> =
            mapAction f <| Fragment templates

        static member inline MapQuery<'S, 'A, 'Q1, 'Q2>
            (
                f: 'Q1 -> 'Q2,
                template: HTMLTemplate<'S, 'A, 'Q2>
            ) : HTMLTemplate<'S, 'A, 'Q1> =
            mapQuery f template

        static member inline MapQuery<'S, 'A, 'Q1, 'Q2>
            (
                f: 'Q1 -> 'Q2,
                templates: HTMLTemplate<'S, 'A, 'Q2> list
            ) : HTMLTemplate<'S, 'A, 'Q1> =
            mapQuery f <| Fragment templates

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

        static member If<'S, 'A, 'Q>
            (
                predicate: 'S -> bool,
                trueTemplate: HTMLTemplate<'S, 'A, 'Q>,
                falseTemplate: HTMLTemplate<'S, 'A, 'Q>
            ) =
            HTML.OneOf(
                (fun s ->
                    if predicate s then
                        Choice1Of2 s
                    else
                        Choice2Of2 s),
                trueTemplate,
                falseTemplate
            )

        static member When<'S, 'A, 'Q>(predicate: 'S -> bool, template: HTMLTemplate<'S, 'A, 'Q>) =
            HTML.If(predicate, template, Fragment [])

        static member Unless<'S, 'A, 'Q>(predicate: 'S -> bool, template: HTMLTemplate<'S, 'A, 'Q>) =
            HTML.When(predicate >> not, template)

        static member Seq<'S, 'S1, 'A, 'Q>
            (
                f: 'S -> 'S1 list,
                template: HTMLTemplate<'S1, 'A, 'Q>
            ) : HTMLTemplate<'S, 'A, 'Q> =
            Template<HTMLTemplateNode<'S, 'A, 'Q>, 'S, 'A, 'Q>.Iterator
                (packIterator<HTMLTemplateNode<'S, 'A, 'Q>, HTMLTemplateNode<'S1, 'A, 'Q>, 'S, 'S1, 'A, 'Q>
                 <| Iterator(f, template))
