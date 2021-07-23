namespace Tempo.Html

open Browser
open Tempo.Std
open Tempo.Value
open Tempo.Html.Template
open Tempo.Html.Render
open Browser.Types

[<AbstractClass; Sealed>]
type DSL =
    static member NSEl<'S, 'A, 'Q>
        (
            ns: string,
            name: string,
            children: Template<'S, 'A, 'Q> list
        ) : Template<'S, 'A, 'Q> =
        TElement
            { Name = name
              NS = Some ns
              Children = children }

    static member El<'S, 'A, 'Q>(name: string, children: Template<'S, 'A, 'Q> list) : Template<'S, 'A, 'Q> =
        TElement
            { Name = name
              NS = None
              Children = children }

    static member Text<'S, 'A, 'Q>(f: 'S -> string) : Template<'S, 'A, 'Q> = f |> Derived |> TText

    static member Text() : Template<string, 'A, 'Q> = DSL.Text id

    static member Text<'S, 'A, 'Q>(value: string) : Template<'S, 'A, 'Q> = value |> Literal |> TText

    static member AttrValue<'S, 'A, 'Q>(name: string, value: Value<'S, string option>) : Template<'S, 'A, 'Q> =
        TAttribute { Name = name; Value = value }

    static member AttrValue<'S, 'A, 'Q>(name: string, value: Value<'S, string>) : Template<'S, 'A, 'Q> =
        let value =
            Value.Map<'S, string, string option> Some value

        DSL.AttrValue(name, value)

    static member Attr<'S, 'A, 'Q>(name: string, value: string option) : Template<'S, 'A, 'Q> =
        DSL.AttrValue(name, (value |> Literal))

    static member Attr<'S, 'A, 'Q>(name: string, value: string) : Template<'S, 'A, 'Q> =
        DSL.AttrValue(name, (value |> Some |> Literal))

    static member Attr<'S, 'A, 'Q>(name: string, f: 'S -> string option) : Template<'S, 'A, 'Q> =
        DSL.AttrValue(name, (f |> Derived))

    static member Attr<'S, 'A, 'Q>(name: string, f: 'S -> string) : Template<'S, 'A, 'Q> =
        DSL.AttrValue(name, (f >> Some |> Derived))

    static member Attr<'S, 'A, 'Q>(name: string, value: bool) : Template<'S, 'A, 'Q> =
        DSL.AttrValue(name, ((if value then (Some name) else None) |> Literal))

    static member Attr<'S, 'A, 'Q>(name: string, f: 'S -> bool) : Template<'S, 'A, 'Q> =
        DSL.AttrValue(
            name,
            ((fun s -> if f s then (Some name) else None)
             |> Derived)
        )

    static member Attr<'S, 'A, 'Q>
        (
            name: string,
            predicate: 'S -> bool,
            whenTrue: string,
            whenFalse: string
        ) : Template<'S, 'A, 'Q> =
        DSL.Attr<'S, 'A, 'Q>(
            name,
            (fun (s: 'S) ->
                if (predicate s) then
                    whenTrue
                else
                    whenFalse)
        )

    static member Attr<'A, 'Q>(name: string, whenTrue: string, whenFalse: string) : Template<bool, 'A, 'Q> =
        DSL.Attr(name, (fun b -> if b then whenTrue else whenFalse))

    static member PropValue<'S, 'A, 'Q, 'T>(name: string, value: Value<'S, 'T option>) : Template<'S, 'A, 'Q> =
        makeProperty (name, value)

    static member Prop<'S, 'A, 'Q, 'T>(name: string, f: 'S -> 'T option) : Template<'S, 'A, 'Q> =
        DSL.PropValue(name, (f |> Derived))

    static member Prop<'S, 'A, 'Q, 'T>(name: string, f: 'S -> 'T) : Template<'S, 'A, 'Q> =
        DSL.PropValue(name, (f >> Some |> Derived))

    static member Prop<'S, 'A, 'Q, 'T>(name: string, value: 'T) : Template<'S, 'A, 'Q> =
        DSL.PropValue(name, (value |> Some |> Literal))

    static member On<'S, 'A, 'Q>(name: string, handler: THandlerPayload<'S, 'A> -> unit) : Template<'S, 'A, 'Q> =
        THandler { Name = name; Handler = handler }

    static member Send<'S, 'A, 'Q>(name: string, handler: THandlerSendPayload<'S> -> 'A) : Template<'S, 'A, 'Q> =
        THandler
            { Name = name
              Handler =
                  (fun { State = state
                         Dispatch = dispatch
                         Event = event
                         Element = element } ->
                      dispatch (
                          handler (
                              { State = state
                                Event = event
                                Element = element }
                          )
                      )) }

    static member inline Send<'S, 'A, 'Q>(name: string, action: 'A) : Template<'S, 'A, 'Q> =
        DSL.Send<'S, 'A, 'Q>(name, (fun (_: THandlerSendPayload<'S>) -> action))

    static member inline Send<'S, 'A, 'Q>(name: string, handler: unit -> 'A) : Template<'S, 'A, 'Q> =
        DSL.Send<'S, 'A, 'Q>(name, (fun (_: THandlerSendPayload<'S>) -> handler ()))

    static member inline Send<'S, 'A, 'Q>(name: string, handler: 'S -> 'A) : Template<'S, 'A, 'Q> =
        DSL.Send<'S, 'A, 'Q>(name, (fun ({ State = s }: THandlerSendPayload<'S>) -> handler s))

    static member inline Send<'S, 'A, 'Q>(name: string, handler: Event -> 'A) : Template<'S, 'A, 'Q> =
        DSL.Send<'S, 'A, 'Q>(name, (fun ({ Event = e }: THandlerSendPayload<'S>) -> handler e))

    static member inline SendTextInput<'S, 'A, 'Q>(name: string, handler: 'S -> string -> 'A) : Template<'S, 'A, 'Q> =
        DSL.Send<'S, 'A, 'Q>(name, (fun p -> handler p.State (p.Element :?> HTMLInputElement).value))

    static member SendNumberInput<'S, 'A, 'Q>(name: string, handler: 'S -> float -> 'A) : Template<'S, 'A, 'Q> =
        DSL.On<'S, 'A, 'Q>(
            name,
            (fun { Element = el
                   State = state
                   Dispatch = dispatch } ->
                let v = (el :?> HTMLInputElement).valueAsNumber

                let isFinite v =
                    not (System.Double.IsInfinity v)
                    && not (System.Double.IsNaN v)

                if isFinite v then
                    dispatch (handler state v))
        )

    static member inline SendToggleInput<'S, 'A, 'Q>(ev: string, handler: 'S -> bool -> 'A) : Template<'S, 'A, 'Q> =
        DSL.On<'S, 'A, 'Q>(
            ev,
            (fun { Dispatch = dispatch
                   State = state
                   Element = element } -> dispatch (handler state (element :?> HTMLInputElement).``checked``))
        )

    static member inline Fragment<'S, 'A, 'Q>(templates: Template<'S, 'A, 'Q> list) : Template<'S, 'A, 'Q> =
        TFragment templates

    static member MapState<'S1, 'S2, 'A, 'Q>(map: 'S1 -> 'S2, template: Template<'S2, 'A, 'Q>) : Template<'S1, 'A, 'Q> =
        makeTransform (
            (fun render ->
                fun state element reference dispatch ->
                    let view =
                        render (map (state)) element reference dispatch

                    let change =
                        Option.map (fun f -> f << map) view.Change

                    { Change = change
                      Request = view.Request
                      Destroy = view.Destroy }),
            template
        )

    static member MapAction<'S, 'A1, 'A2, 'Q>
        (
            map: 'A2 -> 'A1 option,
            template: Template<'S, 'A2, 'Q>
        ) : Template<'S, 'A1, 'Q> =
        makeTransform (
            (fun render ->
                fun state element reference dispatch ->
                    let mappedDispatch a =
                        match map a with
                        | Some a -> dispatch a
                        | None -> ()

                    render state element reference mappedDispatch),
            template
        )

    static member MapQuery<'S, 'A, 'Q1, 'Q2>
        (
            map: 'Q1 -> 'Q2 option,
            template: Template<'S, 'A, 'Q2>
        ) : Template<'S, 'A, 'Q1> =
        makeTransform<'S, 'S, 'A, 'A, 'Q1, 'Q2> (
            (fun (render: Render<'S, 'A, 'Q2>) ->
                fun state element reference dispatch ->
                    let view = render state element reference dispatch

                    let request =
                        Option.map
                            (fun r q ->
                                match map q with
                                | Some q -> r q
                                | None -> ())
                            view.Request

                    { Change = view.Change
                      Request = request
                      Destroy = view.Destroy }),
            template
        )

    static member inline MapQuery<'S, 'A, 'Q1, 'Q2>
        (
            map: 'Q1 -> 'Q2 option,
            templates: Template<'S, 'A, 'Q2> list
        ) : Template<'S, 'A, 'Q1> =
        DSL.MapQuery(map, DSL.Fragment templates)

    static member inline MapSA<'S1, 'S2, 'A1, 'A2, 'Q>
        (
            mapState: 'S1 -> 'S2,
            mapAction: 'A2 -> 'A1 option,
            template: Template<'S2, 'A2, 'Q>
        ) : Template<'S1, 'A1, 'Q> =
        DSL.MapState(mapState, DSL.MapAction(mapAction, template))

    static member inline MapSAQ<'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>
        (
            mapState: 'S1 -> 'S2,
            mapAction: 'A2 -> 'A1 option,
            mapQuery: 'Q1 -> 'Q2 option,
            template: Template<'S2, 'A2, 'Q2>
        ) : Template<'S1, 'A1, 'Q1> =
        DSL.MapSA(mapState, mapAction, DSL.MapQuery(mapQuery, template))

    static member inline MapSQ<'S1, 'S2, 'A, 'Q1, 'Q2>
        (
            mapState: 'S1 -> 'S2,
            mapQuery: 'Q1 -> 'Q2 option,
            template: Template<'S2, 'A, 'Q2>
        ) : Template<'S1, 'A, 'Q1> =
        DSL.MapState(mapState, DSL.MapQuery(mapQuery, template))

    static member inline MapAQ<'S, 'A1, 'A2, 'Q1, 'Q2>
        (
            mapAction: 'A2 -> 'A1 option,
            mapQuery: 'Q1 -> 'Q2 option,
            template: Template<'S, 'A2, 'Q2>
        ) : Template<'S, 'A1, 'Q1> =
        DSL.MapAction(mapAction, DSL.MapQuery(mapQuery, template))

    static member inline OneOf<'S, 'S1, 'S2, 'A, 'Q>
        (
            choose: 'S -> Choice<'S1, 'S2>,
            template1: Template<'S1, 'A, 'Q>,
            template2: Template<'S2, 'A, 'Q>
        ) : Template<'S, 'A, 'Q> =
        makeOneOf2 (choose, template1, template2)

    static member Maybe<'S1, 'S2, 'A, 'Q>
        (
            f: 'S1 -> 'S2 option,
            template: Template<'S2, 'A, 'Q>
        ) : Template<'S1, 'A, 'Q> =
        DSL.OneOf(
            (fun s ->
                match f s with
                | Some v -> Choice1Of2(v)
                | None -> Choice2Of2(())),
            template,
            DSL.Text ""
        )

    static member OneOf<'S, 'S1, 'S2, 'S3, 'A, 'Q>
        (
            f: 'S -> Choice<'S1, 'S2, 'S3>,
            template1: Template<'S1, 'A, 'Q>,
            template2: Template<'S2, 'A, 'Q>,
            template3: Template<'S3, 'A, 'Q>
        ) : Template<'S, 'A, 'Q> =
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
            template1: Template<'S1, 'A, 'Q>,
            template2: Template<'S2, 'A, 'Q>,
            template3: Template<'S3, 'A, 'Q>,
            template4: Template<'S4, 'A, 'Q>
        ) : Template<'S, 'A, 'Q> =
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
            template1: Template<'S1, 'A, 'Q>,
            template2: Template<'S2, 'A, 'Q>,
            template3: Template<'S3, 'A, 'Q>,
            template4: Template<'S4, 'A, 'Q>,
            template5: Template<'S5, 'A, 'Q>
        ) : Template<'S, 'A, 'Q> =
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
            template1: Template<'S1, 'A, 'Q>,
            template2: Template<'S2, 'A, 'Q>,
            template3: Template<'S3, 'A, 'Q>,
            template4: Template<'S4, 'A, 'Q>,
            template5: Template<'S5, 'A, 'Q>,
            template6: Template<'S6, 'A, 'Q>
        ) : Template<'S, 'A, 'Q> =
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
            template1: Template<'S1, 'A, 'Q>,
            template2: Template<'S2, 'A, 'Q>,
            template3: Template<'S3, 'A, 'Q>,
            template4: Template<'S4, 'A, 'Q>,
            template5: Template<'S5, 'A, 'Q>,
            template6: Template<'S6, 'A, 'Q>,
            template7: Template<'S7, 'A, 'Q>
        ) : Template<'S, 'A, 'Q> =
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
            trueTemplate: Template<'S, 'A, 'Q>,
            falseTemplate: Template<'S, 'A, 'Q>
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

    static member When<'S, 'A, 'Q>(predicate: 'S -> bool, template: Template<'S, 'A, 'Q>) =
        DSL.If(predicate, template, DSL.Fragment [])

    static member Unless<'S, 'A, 'Q>(predicate: 'S -> bool, template: Template<'S, 'A, 'Q>) =
        DSL.When(predicate >> not, template)

// static member Seq<'S1, 'S2, 'A, 'Q>
//     (
//         f: 'S1 -> 'S2 list,
//         template: Template<'S2, 'A, 'Q>
//     ) : Template<'S1, 'A, 'Q> =
//     iterator createGroupNode f template

// static member inline Transform<'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>
//     (
//         transformf: Render<'S2, 'A2, 'Q2> -> Render<'S1, 'A1, 'Q1>,
//         template: Template<'S2, 'A2, 'Q2>
//     ) : Template<'S1, 'A1, 'Q1> =
//     transform transformf template

// static member inline Lifecycle<'S, 'A, 'Q, 'P>
//     (
//         afterRender: LifecycleInitialPayload<'S, 'A> -> 'P,
//         beforeChange: LifecycleStatePayload<'S, 'A, 'P> -> (bool * 'P),
//         afterChange: LifecycleStatePayload<'S, 'A, 'P> -> 'P,
//         beforeDestroy: LifecyclePayload<'A, 'P> -> unit,
//         respond: 'Q -> LifecyclePayload<'A, 'P> -> 'P,
//         template: Template<'S, 'A, 'Q>
//     ) : Template<'S, 'A, 'Q> =
//     lifecycle afterRender beforeChange afterChange beforeDestroy respond template

// static member inline Lifecycle<'S, 'A, 'Q, 'P>
//     (
//         afterRender: LifecycleInitialPayload<'S, 'A> -> 'P,
//         afterChange: LifecycleStatePayload<'S, 'A, 'P> -> 'P,
//         beforeDestroy: LifecyclePayload<'A, 'P> -> unit,
//         respond: 'Q -> LifecyclePayload<'A, 'P> -> 'P,
//         template: Template<'S, 'A, 'Q>
//     ) : Template<'S, 'A, 'Q> =
//     DSL.Lifecycle(
//         afterRender,
//         (fun { Payload = payload } -> (true, payload)),
//         afterChange,
//         beforeDestroy,
//         respond,
//         template
//     )

// static member inline Lifecycle<'S, 'A, 'Q, 'P>
//     (
//         afterRender: LifecycleInitialPayload<'S, 'A> -> 'P,
//         afterChange: LifecycleStatePayload<'S, 'A, 'P> -> 'P,
//         beforeDestroy: LifecyclePayload<'A, 'P> -> unit,
//         template: Template<'S, 'A, 'Q>
//     ) : Template<'S, 'A, 'Q> =
//     DSL.Lifecycle(afterRender, afterChange, beforeDestroy, (fun _ { Payload = payload } -> payload), template)

// static member CompareStates<'S, 'A, 'Q>
//     (
//         f: 'S -> 'S -> bool,
//         template: Template<'S, 'A, 'Q>
//     ) : Template<'S, 'A, 'Q> =
//     lifecycle
//         (fun { State = state } -> state)
//         (fun { State = newState; Payload = oldState } ->
//             if f newState oldState then
//                 (true, newState)
//             else
//                 (false, oldState))
//         (fun { State = state } -> state)
//         ignore
//         (fun _ { Payload = state } -> state)
//         template

// static member inline Filter<'S, 'A, 'Q>
//     (
//         f: 'S -> bool,
//         template: Template<'S, 'A, 'Q>
//     ) : Template<'S, 'A, 'Q> =
//     lifecycle ignore ((fun { State = s } -> (f s, ()))) ignore ignore (fun _ _ -> ()) template

// static member inline WhenStateChanges<'S, 'A, 'Q when 'S: equality>
//     (
//         f: 'S -> bool,
//         template: Template<'S, 'A, 'Q>
//     ) : Template<'S, 'A, 'Q> =
//     DSL.CompareStates((<>), template)

// static member MakeCaptureSA<'S1, 'S2, 'S3, 'A1, 'A2, 'A3, 'Q1>
//     ()
//     : CaptureResult<HTMLTemplateNode<'S1, 'A1, 'Q1>, HTMLTemplateNode<'S2, 'A2, 'Q1>, HTMLTemplateNode<'S3, 'A3, 'Q1>, 'S1, 'S2, 'S3, 'A1, 'A2, 'A3, 'Q1> =
//     makeCaptureSA ()

// static member MakeCaptureState<'S1, 'S2, 'S3, 'A1, 'Q1>
//     ()
//     : CaptureStateResult<HTMLTemplateNode<'S1, 'A1, 'Q1>, HTMLTemplateNode<'S2, 'A1, 'Q1>, HTMLTemplateNode<'S3, 'A1, 'Q1>, 'S1, 'S2, 'S3, 'A1, 'Q1> =
//     makeCaptureState ()

// static member MakeCaptureAction<'S1, 'A1, 'A2, 'A3, 'Q1>
//     ()
//     : CaptureActionResult<HTMLTemplateNode<'S1, 'A1, 'Q1>, HTMLTemplateNode<'S1, 'A2, 'Q1>, HTMLTemplateNode<'S1, 'A3, 'Q1>, 'S1, 'A1, 'A2, 'A3, 'Q1> =
//     makeCaptureAction ()

// static member Component<'S, 'A, 'Q>
//     (
//         update: 'S -> 'A -> 'S,
//         middleware: MiddlewarePayload<'S, 'A, 'Q> -> unit,
//         template: Template<'S, 'A, 'Q>
//     ) : Template<'S, 'A, 'Q> =
//     comp update middleware template

// static member Component<'S, 'A, 'Q>
//     (
//         update: 'S -> 'A -> 'S,
//         template: Template<'S, 'A, 'Q>
//     ) : Template<'S, 'A, 'Q> =
//     comp update ignore template

// static member Portal<'S, 'A, 'Q>(selector: string, template: Template<'S, 'A, 'Q>) : Template<'S, 'A, 'Q> =
//     transform
//         (fun render2 ->
//             fun impl state dispatch ->
//                 // TODO this will fail at runtime if parent doesn't exist
//                 let parent = document.querySelector selector
//                 let parent = HTMLElementImpl(parent) :> Impl

//                 // render in source parent to not break the flow
//                 let view = render2 impl state dispatch
//                 // attach to physical parent
//                 parent.Append view.Impl
//                 view)
//         template

// // fsharplint:disable
// static member inline cls(text: string) = DSL.Attr("class", text)
// static member inline cls(f: 'S -> string) = DSL.Attr("class", f)
// static member inline cls(f: 'S -> string option) = DSL.cls (Option.defaultValue "" << f)
// static member inline cls(whenTrue: string, whenFalse: string) = DSL.Attr("class", whenTrue, whenFalse)

// static member inline cls(predicate: 'S -> bool, whenTrue: string, whenFalse: string) =
//     DSL.Attr("class", predicate, whenTrue, whenFalse)

// static member inline elId(text: string) = DSL.Attr("id", text)
// static member inline elId(f: 'S -> string) = DSL.Attr("id", f)
// static member inline elId(whenTrue: string, whenFalse: string) = DSL.Attr("id", whenTrue, whenFalse)

// static member inline aria(name: string, text: string) = DSL.Attr($"aria-{name}", text)
// static member inline aria(name: string, f: 'S -> string) = DSL.Attr($"aria-{name}", f)

// static member inline aria(name: string, whenTrue: string, whenFalse: string) =
//     DSL.Attr($"aria-{name}", whenTrue, whenFalse)

// static member inline innerHTML<'S, 'A, 'Q>(html: string) : Template<'S, 'A, 'Q> =
//     lifecycleAttribute (fun { Element = el } -> el.innerHTML <- html) (fun _ -> (true, ())) ignore ignore (fun _ _ -> ())

// static member inline innerHTML<'S, 'A, 'Q>(f: 'S -> string) : Template<'S, 'A, 'Q> =
//     lifecycleAttribute
//         (fun { Element = el; State = state } ->
//             let html = f state
//             el.innerHTML <- html
//             html)
//         (fun { Payload = old } -> (true, old))
//         (fun { Payload = old } -> old)
//         ignore
//         (fun _ { Payload = old } -> old)

// static member inline innerHTML<'A, 'Q>(html: string -> string) : Template<string, 'A, 'Q> = DSL.innerHTML<string, 'A, 'Q> id
// // fsharplint:enable

// static member inline DIV<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list, children: Template<'S, 'A, 'Q> list) = DSL.El("div", attributes, children)

// static member inline DIV<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list) = DSL.DIV(attributes, [])

// static member inline DIV<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.DIV([], children)
// static member inline DIV<'S, 'A, 'Q>(child: Template<'S, 'A, 'Q>) = DSL.DIV([], [ child ])

// static member inline DIV<'S, 'A, 'Q>() = DSL.DIV([], [])

// static member inline DIV<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list, child: Template<'S, 'A, 'Q>) = DSL.DIV(attributes, [ child ])

// static member inline MAIN<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list, children: Template<'S, 'A, 'Q> list) = DSL.El("main", attributes, children)

// static member inline MAIN<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list) = DSL.MAIN(attributes, [])

// static member inline MAIN<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.MAIN([], children)
// static member inline MAIN<'S, 'A, 'Q>(child: Template<'S, 'A, 'Q>) = DSL.MAIN([], [ child ])

// static member inline MAIN<'S, 'A, 'Q>() = DSL.MAIN([], [])

// static member inline MAIN<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list, child: Template<'S, 'A, 'Q>) = DSL.MAIN(attributes, [ child ])

// static member inline ASIDE<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list, children: Template<'S, 'A, 'Q> list) = DSL.El("aside", attributes, children)

// static member inline ASIDE<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list) = DSL.ASIDE(attributes, [])

// static member inline ASIDE<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.ASIDE([], children)
// static member inline ASIDE<'S, 'A, 'Q>(child: Template<'S, 'A, 'Q>) = DSL.ASIDE([], [ child ])

// static member inline ASIDE<'S, 'A, 'Q>() = DSL.ASIDE([], [])

// static member inline ASIDE<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list, child: Template<'S, 'A, 'Q>) = DSL.ASIDE(attributes, [ child ])

// static member inline BUTTON<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list, children: Template<'S, 'A, 'Q> list) = DSL.El("button", attributes, children)

// static member inline BUTTON<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list) = DSL.BUTTON(attributes, [])

// static member inline BUTTON<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.BUTTON([], children)

// static member inline BUTTON<'S, 'A, 'Q>() = DSL.BUTTON([], [])

// static member inline BUTTON<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list, child: Template<'S, 'A, 'Q>) = DSL.BUTTON(attributes, [ child ])
// static member inline BUTTON<'S, 'A, 'Q>(child: Template<'S, 'A, 'Q>) = DSL.BUTTON([], [ child ])

// static member inline IMG<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list) = DSL.El("img", attributes, [])

// static member inline IMG<'S, 'A, 'Q>() = DSL.IMG([])

// static member inline SPAN<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list, children: Template<'S, 'A, 'Q> list) = DSL.El("span", attributes, children)

// static member inline SPAN<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list) = DSL.SPAN(attributes, [])

// static member inline SPAN<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.SPAN([], children)
// static member inline SPAN<'S, 'A, 'Q>(child: Template<'S, 'A, 'Q>) = DSL.SPAN([], [ child ])

// static member inline SPAN<'S, 'A, 'Q>() = DSL.SPAN([], [])

// static member inline SPAN<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list, child: Template<'S, 'A, 'Q>) = DSL.SPAN(attributes, [ child ])

// static member inline SVG<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list, children: Template<'S, 'A, 'Q> list) =
//     DSL.NSEl("http://www.w3.org/2000/svg", "svg", attributes, children)

// static member inline SVG<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list) = DSL.SVG(attributes, [])

// static member inline SVG<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.SVG([], children)
// static member inline SVG<'S, 'A, 'Q>(child: Template<'S, 'A, 'Q>) = DSL.SVG([], [ child ])

// static member inline SVG<'S, 'A, 'Q>() = DSL.SVG([], [])

// static member inline SVG<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list, child: Template<'S, 'A, 'Q>) = DSL.SVG(attributes, [ child ])

// static member inline PATH<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list, children: Template<'S, 'A, 'Q> list) =
//     DSL.NSEl("http://www.w3.org/2000/svg", "path", attributes, children)

// static member inline PATH<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list) = DSL.PATH(attributes, [])

// static member inline PATH<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.PATH([], children)

// static member inline PATH<'S, 'A, 'Q>() = DSL.PATH([], [])

// static member inline PATH<'S, 'A, 'Q>(attributes: Template<'S, 'A, 'Q> list, child: Template<'S, 'A, 'Q>) = DSL.PATH(attributes, [ child ])

// static member inline INPUT<'S, 'A, 'Q>(attrs: Template<'S, 'A, 'Q> list) = DSL.El("input", attrs, [])

// static member inline TEXTAREA<'S, 'A, 'Q>(attrs: Template<'S, 'A, 'Q> list) = DSL.El("textarea", attrs, [])

// // fsharplint:disable
// static member inline INPUT_TEXT<'S, 'A, 'Q>(attrs: Template<'S, 'A, 'Q> list) =
//     DSL.El("input", DSL.Attr("type", "text") :: attrs, [])

// static member inline INPUT_NUMBER<'S, 'A, 'Q>(attrs: Template<'S, 'A, 'Q> list) =
//     DSL.El("input", DSL.Attr("type", "number") :: attrs, [])

// static member inline INPUT_CHECKBOX<'S, 'A, 'Q>(attrs: Template<'S, 'A, 'Q> list) =
//     DSL.El("input", DSL.Attr("type", "checkbox") :: attrs, [])

// static member inline INPUT_RANGE<'S, 'A, 'Q>(attrs: Template<'S, 'A, 'Q> list) =
//     DSL.El("input", DSL.Attr("type", "range") :: attrs, [])
// // fsharplint:enable

// static member inline SELECT<'S, 'A, 'Q>(attrs: Template<'S, 'A, 'Q> list, children: Template<'S, 'A, 'Q> list) = DSL.El("select", attrs, children)
