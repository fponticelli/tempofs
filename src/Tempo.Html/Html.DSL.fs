namespace Tempo.Html

open Browser
open Tempo.Std
open Tempo.Core
open Tempo.Html
open Tempo.Html.Impl
open Browser.Types

[<AbstractClass; Sealed>]
type DSL =
    static member private MakeRender<'S, 'A, 'Q>() =
        MakeRender<HTMLTemplateNode<'S, 'A, 'Q>, 'S, 'A, 'Q>(makeHTMLNodeRender, createGroupNode)

    static member MakeProgram<'S, 'A, 'Q>(template: HTMLTemplate<'S, 'A, 'Q>, el: HTMLElement) =
        let renderInstance = DSL.MakeRender()

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
                      Dispatch = dispatch
                      Query = view.Query }

                localState <- newState

            and view = render localState dispatch

            { Impl = view.Impl
              Change = view.Change
              Dispatch = dispatch
              Destroy = view.Destroy
              Query = view.Query }: ComponentView<'S, 'A, 'Q>

    static member MakeProgramOnContentLoaded<'S, 'A, 'Q>
        (
            template: HTMLTemplate<'S, 'A, 'Q>,
            selector: string,
            f: (ComponentView<'S, 'A, 'Q> -> unit)
        ) =
        fun update middleware state ->
            window.addEventListener (
                "DOMContentLoaded",
                fun _ ->
                    let el =
                        document.querySelector selector :?> HTMLElement

                    let render = DSL.MakeProgram(template, el)
                    render update middleware state |> f
            )
            |> ignore

    static member NSEl<'S, 'A, 'Q>
        (
            ns: string,
            name: string,
            attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list,
            children: HTMLTemplate<'S, 'A, 'Q> list
        ) : HTMLTemplate<'S, 'A, 'Q> =
        Node
        <| HTMLTemplateElementNS(
            ns,
            { Name = name
              Attributes = attributes
              Children = children }
        )

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
        DSL.El<'S, 'A, 'Q>(name, attributes, [])

    static member inline El<'S, 'A, 'Q>
        (
            name: string,
            children: HTMLTemplate<'S, 'A, 'Q> list
        ) : HTMLTemplate<'S, 'A, 'Q> =
        DSL.El<'S, 'A, 'Q>(name, [], children)

    static member inline El<'S, 'A, 'Q>
        (
            name: string,
            attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list,
            child: HTMLTemplate<'S, 'A, 'Q>
        ) : HTMLTemplate<'S, 'A, 'Q> =
        DSL.El<'S, 'A, 'Q>(name, attributes, [ child ])

    static member inline El<'S, 'A, 'Q>(name: string, child: HTMLTemplate<'S, 'A, 'Q>) : HTMLTemplate<'S, 'A, 'Q> =
        DSL.El<'S, 'A, 'Q>(name, [], [ child ])

    static member inline El<'S, 'A, 'Q>(name: string) : HTMLTemplate<'S, 'A, 'Q> = DSL.El<'S, 'A, 'Q>(name, [], [])

    static member inline El<'S, 'A, 'Q>(name: string, value: string) : HTMLTemplate<'S, 'A, 'Q> =
        DSL.El<'S, 'A, 'Q>(name, [], [ DSL.Text value ])

    static member inline El<'S, 'A, 'Q>(name: string, f: 'S -> string) : HTMLTemplate<'S, 'A, 'Q> =
        DSL.El<'S, 'A, 'Q>(name, [], [ DSL.Text f ])

    static member inline El<'S, 'A, 'Q>
        (
            name: string,
            attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list,
            value: string
        ) : HTMLTemplate<'S, 'A, 'Q> =
        DSL.El<'S, 'A, 'Q>(name, attributes, [ DSL.Text value ])

    static member inline El<'S, 'A, 'Q>
        (
            name: string,
            attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list,
            f: 'S -> string
        ) : HTMLTemplate<'S, 'A, 'Q> =
        DSL.El<'S, 'A, 'Q>(name, attributes, [ DSL.Text f ])

    static member Text() : HTMLTemplate<string, 'A, 'Q> =
        id |> Derived |> HTMLTemplateText |> Node

    static member Text<'S, 'A, 'Q>(value: string) : HTMLTemplate<'S, 'A, 'Q> =
        value |> Literal |> HTMLTemplateText |> Node

    static member Text<'S, 'A, 'Q>(f: 'S -> string) : HTMLTemplate<'S, 'A, 'Q> =
        f |> Derived |> HTMLTemplateText |> Node

    static member Attr<'S, 'A, 'Q>(name: string, value: string option) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        attribute name (value |> Literal |> StringAttr)

    static member Attr<'S, 'A, 'Q>(name: string, value: string) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        attribute name (value |> Some |> Literal |> StringAttr)

    static member Attr<'S, 'A, 'Q>(name: string, f: 'S -> string option) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        attribute name (f |> Derived |> StringAttr)

    static member Attr<'S, 'A, 'Q>(name: string, f: 'S -> string) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        attribute name (f >> Some |> Derived |> StringAttr)

    static member Attr<'S, 'A, 'Q>(name: string, value: bool) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        attribute
            name
            ((if value then (Some name) else None)
             |> Literal
             |> StringAttr)

    static member Attr<'S, 'A, 'Q>(name: string, f: 'S -> bool) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        attribute
            name
            ((fun s -> if f s then (Some name) else None)
             |> Derived
             |> StringAttr)

    static member Attr<'A, 'Q>
        (
            name: string,
            whenTrue: string,
            whenFalse: string
        ) : HTMLTemplateAttribute<bool, 'A, 'Q> =
        DSL.Attr(name, (fun b -> if b then whenTrue else whenFalse))

    static member Prop<'S, 'A, 'Q, 'T>(name: string, value: 'T) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        property name (value |> Literal)

    static member Prop<'S, 'A, 'Q, 'T>(name: string, f: 'S -> 'T) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        property name (f >> Some |> Derived)

    static member On<'S, 'A, 'Q, 'EL, 'E when 'E :> Event and 'EL :> Element>
        (
            name: string,
            handler: TriggerPayload<'S, 'E, 'EL> -> 'A
        ) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        attribute name (makeTrigger handler |> Trigger)

    static member inline On<'S, 'A, 'Q>(name: string, action: 'A) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        DSL.On<'S, 'A, 'Q, _, _>(name, (fun _ -> action))

    static member inline On<'S, 'A, 'Q>(name: string, handler: unit -> 'A) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        DSL.On<'S, 'A, 'Q, _, _>(name, (fun _ -> handler ()))

    static member inline On<'S, 'A, 'Q>(name: string, handler: 'S -> 'A) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        DSL.On<'S, 'A, 'Q, _, _>(name, (fun { State = s } -> handler s))

    static member inline On<'S, 'A, 'Q, 'E when 'E :> Event>
        (
            name: string,
            handler: 'E -> 'A
        ) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        DSL.On<'S, 'A, 'Q, _, 'E>(name, (fun { Event = e } -> handler e))

    static member inline Map<'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>
        (
            impl: Impl -> Impl,
            state: 'S1 -> 'S2,
            action: 'A2 -> 'A1 option,
            query: 'Q1 -> 'Q2,
            template: HTMLTemplate<'S2, 'A2, 'Q2>
        ) : HTMLTemplate<'S1, 'A1, 'Q1> =
        map impl state action query template

    static member inline MapState<'S1, 'S2, 'A, 'Q>
        (
            f: 'S1 -> 'S2,
            template: HTMLTemplate<'S2, 'A, 'Q>
        ) : HTMLTemplate<'S1, 'A, 'Q> =
        DSL.Map(id, f, Some, id, template)

    static member inline MapState<'S1, 'S2, 'A, 'Q>
        (
            f: 'S1 -> 'S2,
            templates: HTMLTemplate<'S2, 'A, 'Q> list
        ) : HTMLTemplate<'S1, 'A, 'Q> =
        DSL.MapState(f, Fragment templates)

    static member inline MapAction<'S, 'A1, 'A2, 'Q>
        (
            f: 'A2 -> 'A1 option,
            template: HTMLTemplate<'S, 'A2, 'Q>
        ) : HTMLTemplate<'S, 'A1, 'Q> =
        DSL.Map(id, id, f, id, template)

    static member inline MapAction<'S, 'A1, 'A2, 'Q>
        (
            f: 'A2 -> 'A1 option,
            templates: HTMLTemplate<'S, 'A2, 'Q> list
        ) : HTMLTemplate<'S, 'A1, 'Q> =
        DSL.MapAction(f, Fragment templates)

    static member inline MapQuery<'S, 'A, 'Q1, 'Q2>
        (
            f: 'Q1 -> 'Q2,
            template: HTMLTemplate<'S, 'A, 'Q2>
        ) : HTMLTemplate<'S, 'A, 'Q1> =
        DSL.Map(id, id, Some, f, template)

    static member inline MapQuery<'S, 'A, 'Q1, 'Q2>
        (
            f: 'Q1 -> 'Q2,
            templates: HTMLTemplate<'S, 'A, 'Q2> list
        ) : HTMLTemplate<'S, 'A, 'Q1> =
        DSL.MapQuery(f, Fragment templates)

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
        DSL.OneOf(
            (fun s ->
                match f s with
                | Choice1Of3 c -> Choice1Of2 c
                | Choice2Of3 c -> Choice2Of2 <| Choice1Of2 c
                | Choice3Of3 c -> Choice2Of2 <| Choice2Of2 c),
            template1,
            DSL.OneOf<Choice<'S2, 'S3>, 'S2, 'S3, 'A, 'Q>(id, template2, template3)
        )


    static member OneOf<'S, 'S1, 'S2, 'S3, 'S4, 'A, 'Q>
        (
            f: 'S -> Choice<'S1, 'S2, 'S3, 'S4>,
            template1: HTMLTemplate<'S1, 'A, 'Q>,
            template2: HTMLTemplate<'S2, 'A, 'Q>,
            template3: HTMLTemplate<'S3, 'A, 'Q>,
            template4: HTMLTemplate<'S4, 'A, 'Q>
        ) : HTMLTemplate<'S, 'A, 'Q> =
        DSL.OneOf(
            (fun s ->
                match f s with
                | Choice1Of4 c -> Choice1Of2 <| Choice1Of2 c
                | Choice2Of4 c -> Choice1Of2 <| Choice2Of2 c
                | Choice3Of4 c -> Choice2Of2 <| Choice1Of2 c
                | Choice4Of4 c -> Choice2Of2 <| Choice2Of2 c),
            DSL.OneOf<Choice<'S1, 'S2>, 'S1, 'S2, 'A, 'Q>(id, template1, template2),
            DSL.OneOf<Choice<'S3, 'S4>, 'S3, 'S4, 'A, 'Q>(id, template3, template4)
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
        DSL.OneOf(
            (fun s ->
                match f s with
                | Choice1Of5 c -> Choice1Of2 <| Choice1Of2 c
                | Choice2Of5 c -> Choice1Of2 <| Choice2Of2 c
                | Choice3Of5 c -> Choice2Of2 <| Choice1Of3 c
                | Choice4Of5 c -> Choice2Of2 <| Choice2Of3 c
                | Choice5Of5 c -> Choice2Of2 <| Choice3Of3 c),
            DSL.OneOf<Choice<'S1, 'S2>, 'S1, 'S2, 'A, 'Q>(id, template1, template2),
            DSL.OneOf<Choice<'S3, 'S4, 'S5>, 'S3, 'S4, 'S5, 'A, 'Q>(id, template3, template4, template5)
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
        DSL.OneOf(
            (fun s ->
                match f s with
                | Choice1Of6 c -> Choice1Of2 <| Choice1Of3 c
                | Choice2Of6 c -> Choice1Of2 <| Choice2Of3 c
                | Choice3Of6 c -> Choice1Of2 <| Choice3Of3 c
                | Choice4Of6 c -> Choice2Of2 <| Choice1Of3 c
                | Choice5Of6 c -> Choice2Of2 <| Choice2Of3 c
                | Choice6Of6 c -> Choice2Of2 <| Choice3Of3 c),
            DSL.OneOf<Choice<'S1, 'S2, 'S3>, 'S1, 'S2, 'S3, 'A, 'Q>(id, template1, template2, template3),
            DSL.OneOf<Choice<'S4, 'S5, 'S6>, 'S4, 'S5, 'S6, 'A, 'Q>(id, template4, template5, template6)
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
        DSL.OneOf(
            (fun s ->
                match f s with
                | Choice1Of7 c -> Choice1Of2 <| Choice1Of3 c
                | Choice2Of7 c -> Choice1Of2 <| Choice2Of3 c
                | Choice3Of7 c -> Choice1Of2 <| Choice3Of3 c
                | Choice4Of7 c -> Choice2Of2 <| Choice1Of4 c
                | Choice5Of7 c -> Choice2Of2 <| Choice2Of4 c
                | Choice6Of7 c -> Choice2Of2 <| Choice3Of4 c
                | Choice7Of7 c -> Choice2Of2 <| Choice4Of4 c),
            DSL.OneOf<Choice<'S1, 'S2, 'S3>, 'S1, 'S2, 'S3, 'A, 'Q>(id, template1, template2, template3),
            DSL.OneOf<Choice<'S4, 'S5, 'S6, 'S7>, 'S4, 'S5, 'S6, 'S7, 'A, 'Q>(
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
        DSL.OneOf(
            (fun s ->
                if predicate s then
                    Choice1Of2 s
                else
                    Choice2Of2 s),
            trueTemplate,
            falseTemplate
        )

    static member When<'S, 'A, 'Q>(predicate: 'S -> bool, template: HTMLTemplate<'S, 'A, 'Q>) =
        DSL.If(predicate, template, Fragment [])

    static member Unless<'S, 'A, 'Q>(predicate: 'S -> bool, template: HTMLTemplate<'S, 'A, 'Q>) =
        DSL.When(predicate >> not, template)

    static member Seq<'S1, 'S2, 'A, 'Q>
        (
            f: 'S1 -> 'S2 list,
            template: HTMLTemplate<'S2, 'A, 'Q>
        ) : HTMLTemplate<'S1, 'A, 'Q> =
        iterator createGroupNode f template

    static member inline Lifecycle<'S, 'A, 'Q, 'P>
        (
            afterRender: 'S -> 'P,
            beforeChange: 'S -> 'P -> bool,
            afterChange: 'S -> 'P -> 'P,
            beforeDestroy: 'P -> unit,
            respond: 'Q -> 'P -> 'P,
            template: HTMLTemplate<'S, 'A, 'Q>
        ) : HTMLTemplate<'S, 'A, 'Q> =
        lifecycle afterRender beforeChange afterChange beforeDestroy respond template

    static member inline Lifecycle<'S, 'A, 'Q, 'EL, 'P when 'EL :> Element>
        (
            afterRender: HTMLLifecycleInitialPayload<'S, 'A, 'EL> -> 'P,
            beforeChange: HTMLLifecyclePayload<'S, 'A, 'EL, 'P> -> (bool * 'P),
            afterChange: HTMLLifecyclePayload<'S, 'A, 'EL, 'P> -> 'P,
            beforeDestroy: HTMLLifecyclePayload<'S, 'A, 'EL, 'P> -> unit,
            respond: 'Q -> HTMLLifecyclePayload<'S, 'A, 'EL, 'P> -> 'P
        ) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        lifecycleAttribute afterRender beforeChange afterChange beforeDestroy respond

    static member CompareStates<'S, 'A, 'Q>
        (
            f: 'S -> 'S -> bool,
            template: HTMLTemplate<'S, 'A, 'Q>
        ) : HTMLTemplate<'S, 'A, 'Q> =
        lifecycle id f (fun state _ -> state) ignore (fun _ p -> p) template

    static member inline Filter<'S, 'A, 'Q>
        (
            f: 'S -> bool,
            template: HTMLTemplate<'S, 'A, 'Q>
        ) : HTMLTemplate<'S, 'A, 'Q> =
        lifecycle ignore ((fun s _ -> f s)) (fun _ _ -> ()) ignore (fun _ _ -> ()) template

    static member inline WhenStateChanges<'S, 'A, 'Q when 'S: equality>
        (
            f: 'S -> bool,
            template: HTMLTemplate<'S, 'A, 'Q>
        ) : HTMLTemplate<'S, 'A, 'Q> =
        DSL.CompareStates((=), template)

    static member Component<'S, 'A, 'Q>
        (
            update: 'S -> 'A -> 'S,
            middleware: MiddlewarePayload<'S, 'A, 'Q> -> unit,
            template: HTMLTemplate<'S, 'A, 'Q>
        ) : HTMLTemplate<'S, 'A, 'Q> =
        comp update middleware template

    static member Portal<'S, 'A, 'Q>(selector: string, template: HTMLTemplate<'S, 'A, 'Q>) : HTMLTemplate<'S, 'A, 'Q> =
        transform
            (fun render2 ->
                fun impl state dispatch ->
                    // TODO this will fail at runtime if parent doesn't exist
                    let parent = document.querySelector selector
                    let parent = HTMLElementImpl(parent) :> Impl

                    // render in source parent to not break the flow
                    let view = render2 impl state dispatch
                    // attach to physical parent
                    parent.Append view.Impl
                    view)
            template

    static member inline cls(text: string) = DSL.Attr("class", text)
    static member inline cls(f: 'S -> string) = DSL.Attr("class", f)
    static member inline cls(whenTrue: string, whenFalse: string) = DSL.Attr("class", whenTrue, whenFalse)

    static member inline id(text: string) = DSL.Attr("id", text)
    static member inline id(f: 'S -> string) = DSL.Attr("id", f)
    static member inline id(whenTrue: string, whenFalse: string) = DSL.Attr("id", whenTrue, whenFalse)

    static member inline aria(name: string, text: string) = DSL.Attr($"aria-{name}", text)
    static member inline aria(name: string, f: 'S -> string) = DSL.Attr($"aria-{name}", f)

    static member inline aria(name: string, whenTrue: string, whenFalse: string) =
        DSL.Attr($"aria-{name}", whenTrue, whenFalse)

    static member inline innerHTML<'S, 'A, 'Q>(html: string) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        lifecycleAttribute (fun { Element = el } -> el.innerHTML <- html) (fun _ -> (true, ())) ignore ignore (fun _ _ -> ())

    static member inline innerHTML<'S, 'A, 'Q>(f: 'S -> string) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        lifecycleAttribute
            (fun { Element = el; State = state } ->
                let html = f state
                el.innerHTML <- html
                html)
            (fun { Payload = old } -> (true, old))
            (fun { Payload = old } -> old)
            ignore
            (fun _ { Payload = old } -> old)

    static member inline innerHTML<'A, 'Q>(html: string -> string) : HTMLTemplateAttribute<string, 'A, 'Q> = DSL.innerHTML<string, 'A, 'Q> id

    static member inline DIV(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list, children: HTMLTemplate<'S, 'A, 'Q> list) = DSL.El("div", attributes, children)

    static member inline DIV(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list) = DSL.DIV(attributes, [])

    static member inline DIV(children: HTMLTemplate<'S, 'A, 'Q> list) = DSL.DIV([], children)
    static member inline DIV(child: HTMLTemplate<'S, 'A, 'Q>) = DSL.DIV([], [ child ])

    static member inline DIV() = DSL.DIV([], [])

    static member inline DIV(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list, child: HTMLTemplate<'S, 'A, 'Q>) = DSL.DIV(attributes, [ child ])

    static member inline BUTTON(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list, children: HTMLTemplate<'S, 'A, 'Q> list) = DSL.El("button", attributes, children)

    static member inline BUTTON(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list) = DSL.BUTTON(attributes, [])

    static member inline BUTTON(children: HTMLTemplate<'S, 'A, 'Q> list) = DSL.BUTTON([], children)

    static member inline BUTTON() = DSL.BUTTON([], [])

    static member inline BUTTON(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list, child: HTMLTemplate<'S, 'A, 'Q>) = DSL.BUTTON(attributes, [ child ])
    static member inline BUTTON(child: HTMLTemplate<'S, 'A, 'Q>) = DSL.BUTTON([], [ child ])

    static member inline IMG(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list) = DSL.El("img", attributes, [])

    static member inline IMG() = DSL.IMG([])

    static member inline SPAN(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list, children: HTMLTemplate<'S, 'A, 'Q> list) = DSL.El("span", attributes, children)

    static member inline SPAN(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list) = DSL.SPAN(attributes, [])

    static member inline SPAN(children: HTMLTemplate<'S, 'A, 'Q> list) = DSL.SPAN([], children)
    static member inline SPAN(child: HTMLTemplate<'S, 'A, 'Q>) = DSL.SPAN([], [ child ])

    static member inline SPAN() = DSL.SPAN([], [])

    static member inline SPAN(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list, child: HTMLTemplate<'S, 'A, 'Q>) = DSL.SPAN(attributes, [ child ])

    static member inline SVG(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list, children: HTMLTemplate<'S, 'A, 'Q> list) =
        DSL.NSEl("http://www.w3.org/2000/svg", "svg", attributes, children)

    static member inline SVG(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list) = DSL.SVG(attributes, [])

    static member inline SVG(children: HTMLTemplate<'S, 'A, 'Q> list) = DSL.SVG([], children)
    static member inline SVG(child: HTMLTemplate<'S, 'A, 'Q>) = DSL.SVG([], [ child ])

    static member inline SVG() = DSL.SVG([], [])

    static member inline SVG(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list, child: HTMLTemplate<'S, 'A, 'Q>) = DSL.SVG(attributes, [ child ])

    static member inline PATH(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list, children: HTMLTemplate<'S, 'A, 'Q> list) =
        DSL.NSEl("http://www.w3.org/2000/svg", "path", attributes, children)

    static member inline PATH(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list) = DSL.PATH(attributes, [])

    static member inline PATH(children: HTMLTemplate<'S, 'A, 'Q> list) = DSL.PATH([], children)

    static member inline PATH() = DSL.PATH([], [])

    static member inline PATH(attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list, child: HTMLTemplate<'S, 'A, 'Q>) = DSL.PATH(attributes, [ child ])
