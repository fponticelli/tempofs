namespace Tempo.Html

open Browser
open Tempo.Std
open Tempo.Update
open Tempo.View
open Tempo.Value
open Tempo.Html.Template
open Browser.Types

type LifecycleMount<'S, 'A> =
    { Element: Element
      State: 'S
      Dispatch: Dispatch<'A> }

type LifecycleChange<'S, 'A, 'P> =
    { Element: Element
      State: 'S
      Dispatch: Dispatch<'A>
      Payload: 'P }

type LifecycleDestroy<'A, 'P> =
    { Element: Element
      Dispatch: Dispatch<'A>
      Payload: 'P }

type MapActionPayload<'S, 'A> = { State: 'S; Action: 'A }


type SelectOption<'S> =
    | OptionValue of label: Value<'S, string> * value: 'S
    | OptionGroup of string * (Value<'S, string> * 'S) list

[<AbstractClass; Sealed>]
type DSL =
    static member Empty<'S, 'A, 'Q>() : Template<'S, 'A, 'Q> = TEmpty

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

    static member Text<'S, 'A, 'Q>(v: Value<'S, string>) : Template<'S, 'A, 'Q> = v |> TText

    static member Text<'S, 'A, 'Q>(f: 'S -> string) : Template<'S, 'A, 'Q> = f |> Derived |> DSL.Text

    static member Text() : Template<string, 'A, 'Q> = DSL.Text id

    static member Text<'S, 'A, 'Q>(value: string) : Template<'S, 'A, 'Q> = value |> Literal |> DSL.Text

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

    static member PropValue<'S, 'A, 'Q>(name: string, value: Value<'S, string>) : Template<'S, 'A, 'Q> =
        makeProperty (name, Value.Map Some value)

    static member PropValue<'S, 'A, 'Q>(name: string, value: Value<'S, float>) : Template<'S, 'A, 'Q> =
        makeProperty (name, Value.Map Some value)

    static member PropValue<'S, 'A, 'Q>(name: string, value: Value<'S, int>) : Template<'S, 'A, 'Q> =
        makeProperty (name, Value.Map Some value)

    static member PropValue<'S, 'A, 'Q>(name: string, value: Value<'S, bool>) : Template<'S, 'A, 'Q> =
        makeProperty (name, Value.Map Some value)

    static member Prop<'S, 'A, 'Q, 'T>(name: string, f: 'S -> 'T option) : Template<'S, 'A, 'Q> =
        DSL.PropValue(name, (f |> Derived))

    static member Prop<'S, 'A, 'Q, 'T>(name: string, f: 'S -> 'T) : Template<'S, 'A, 'Q> =
        DSL.PropValue(name, (f >> Some |> Derived))

    static member Prop<'S, 'A, 'Q>(name: string, value: string) : Template<'S, 'A, 'Q> =
        DSL.PropValue(name, (value |> Some |> Literal))

    static member Prop<'S, 'A, 'Q>(name: string, value: float) : Template<'S, 'A, 'Q> =
        DSL.PropValue(name, (value |> Some |> Literal))

    static member Prop<'S, 'A, 'Q>(name: string, value: int) : Template<'S, 'A, 'Q> =
        DSL.PropValue(name, (value |> Some |> Literal))

    static member Prop<'S, 'A, 'Q>(name: string, value: bool) : Template<'S, 'A, 'Q> =
        DSL.PropValue(name, (value |> Some |> Literal))

    static member On<'S, 'A, 'Q>(name: string, handler: OnPayload<'S, 'A> -> unit) : Template<'S, 'A, 'Q> =
        THandler { Name = name; Handler = handler }

    static member Send<'S, 'A, 'Q>(name: string, handler: SendPayload<'S> -> 'A) : Template<'S, 'A, 'Q> =
        DSL.On(
            name,
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
                ))
        )

    static member inline SendAction<'S, 'A, 'Q>(name: string, action: 'A) : Template<'S, 'A, 'Q> =
        DSL.Send<'S, 'A, 'Q>(name, (fun (_: SendPayload<'S>) -> action))

    static member inline SendState<'S, 'A, 'Q>(name: string, handler: 'S -> 'A) : Template<'S, 'A, 'Q> =
        DSL.Send<'S, 'A, 'Q>(name, (fun ({ State = s }: SendPayload<'S>) -> handler s))

    static member inline SendEvent<'S, 'A, 'Q>(name: string, handler: Event -> 'A) : Template<'S, 'A, 'Q> =
        DSL.Send<'S, 'A, 'Q>(name, (fun ({ Event = e }: SendPayload<'S>) -> handler e))

    static member inline SendElement<'S, 'A, 'Q>(name: string, handler: Element -> 'A) : Template<'S, 'A, 'Q> =
        DSL.Send<'S, 'A, 'Q>(name, (fun ({ Element = e }: SendPayload<'S>) -> handler e))

    static member inline SendTextInput<'S, 'A, 'Q>(name: string, handler: 'S -> string -> 'A) : Template<'S, 'A, 'Q> =
        DSL.Send<'S, 'A, 'Q>(
            name,
            (fun ({ State = state; Element = element }: SendPayload<'S>) ->
                handler state (element :?> HTMLInputElement).value)
        )

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
                fun (state, element, reference, dispatch) ->
                    let view =
                        render (map (state), element, reference, dispatch)

                    let change =
                        Option.map (fun f -> f << map) view.Change

                    { Change = change
                      Request = view.Request
                      Destroy = view.Destroy }),
            template
        )

    static member MapAction<'S, 'A1, 'A2, 'Q>
        (
            map: MapActionPayload<'S, 'A2> -> 'A1 option,
            template: Template<'S, 'A2, 'Q>
        ) : Template<'S, 'A1, 'Q> =
        makeTransform (
            (fun render ->
                fun (state, element, reference, dispatch) ->
                    let mutable localState = state

                    let mappedDispatch (a: 'A2) =
                        match map { State = localState; Action = a } with
                        | Some a -> dispatch a
                        | None -> ()

                    let view =
                        render (state, element, reference, mappedDispatch)

                    let change s = localState <- s
                    mergeChange (change, view)),
            template
        )

    static member MapQuery<'S, 'A, 'Q1, 'Q2>
        (
            map: 'Q1 -> 'Q2 option,
            template: Template<'S, 'A, 'Q2>
        ) : Template<'S, 'A, 'Q1> =
        makeTransform<'S, 'S, 'A, 'A, 'Q1, 'Q2> (
            (fun (render: Render<'S, 'A, 'Q2>) ->
                fun (state, element, reference, dispatch) ->
                    let view =
                        render (state, element, reference, dispatch)

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
            mapAction: MapActionPayload<'S1, 'A2> -> 'A1 option,
            template: Template<'S2, 'A2, 'Q>
        ) : Template<'S1, 'A1, 'Q> =
        DSL.MapAction(mapAction, DSL.MapState(mapState, template))

    static member inline MapSAQ<'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>
        (
            mapState: 'S1 -> 'S2,
            mapAction: MapActionPayload<'S1, 'A2> -> 'A1 option,
            mapQuery: 'Q1 -> 'Q2 option,
            template: Template<'S2, 'A2, 'Q2>
        ) : Template<'S1, 'A1, 'Q1> =
        DSL.MapQuery(mapQuery, DSL.MapSA(mapState, mapAction, template))

    static member inline MapSQ<'S1, 'S2, 'A, 'Q1, 'Q2>
        (
            mapState: 'S1 -> 'S2,
            mapQuery: 'Q1 -> 'Q2 option,
            template: Template<'S2, 'A, 'Q2>
        ) : Template<'S1, 'A, 'Q1> =
        DSL.MapState(mapState, DSL.MapQuery(mapQuery, template))

    static member inline MapAQ<'S, 'A1, 'A2, 'Q1, 'Q2>
        (
            mapAction: MapActionPayload<'S, 'A2> -> 'A1 option,
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
            DSL.Empty()
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
        DSL.If(predicate, template, DSL.Empty())

    static member Unless<'S, 'A, 'Q>(predicate: 'S -> bool, template: Template<'S, 'A, 'Q>) =
        DSL.When(predicate >> not, template)

    static member inline ForEach<'S, 'A, 'Q>(template: Template<'S, 'A, 'Q>) : Template<'S list, 'A, 'Q> =
        forEach (template)

    static member inline ForEach<'S1, 'S2, 'A, 'Q>
        (
            map: 'S1 -> 'S2 list,
            template: Template<'S2, 'A, 'Q>
        ) : Template<'S1, 'A, 'Q> =
        DSL.MapState(map, forEach (template))

    static member inline Transform<'S1, 'A1, 'Q1, 'S2, 'A2, 'Q2>
        (
            transform: Render<'S2, 'A2, 'Q2> -> Render<'S1, 'A1, 'Q1>,
            template: Template<'S2, 'A2, 'Q2>
        ) =
        makeTransform (transform, template)

    static member inline Transform<'S1, 'A1, 'Q1, 'S2, 'A2, 'Q2>(transform) = makeTransform (transform, DSL.Empty())

    static member Portal<'S, 'A, 'Q>(selector: string, template: Template<'S, 'A, 'Q>) : Template<'S, 'A, 'Q> =
        DSL.Transform(
            (fun render ->
                (fun (state, _, _, dispatch: Dispatch<'A>) ->
                    let parent = document.querySelector selector // TODO this will fail at runtime if parent doesn't exist
                    render (state, parent, None, dispatch))),
            template
        )

    static member ControlChange<'S, 'A, 'Q>
        (
            controller: 'S -> 'S -> ('S -> unit) -> unit,
            template: Template<'S, 'A, 'Q>
        ) =
        DSL.Transform(
            (fun render ->
                fun (state, container, reference, dispatch) ->
                    let mutable localState = state

                    let view =
                        render (localState, container, reference, dispatch)

                    let change =
                        Option.map
                            (fun c ->
                                fun currState ->
                                    controller currState localState c
                                    localState <- currState)
                            view.Change

                    { Change = change
                      Destroy = view.Destroy
                      Request = view.Request }),
            template
        )

    static member ControlDestroy<'S, 'A, 'Q>(controller: (unit -> unit) -> unit, template: Template<'S, 'A, 'Q>) =
        DSL.Transform(
            (fun render ->
                fun (state, container, reference, dispatch) ->
                    let view =
                        render (state, container, reference, dispatch)

                    let destroy =
                        Option.map (fun d -> fun () -> controller d) view.Destroy

                    { Change = view.Change
                      Destroy = destroy
                      Request = view.Request }),
            template
        )

    static member FilterChange<'S, 'A, 'Q>(predicate: 'S -> 'S -> bool, template: Template<'S, 'A, 'Q>) =
        DSL.ControlChange(
            (fun curr prev change ->
                if (predicate curr prev) then
                    change curr),
            template
        )

    static member inline FilterChange<'S, 'A, 'Q when 'S: equality>(template: Template<'S, 'A, 'Q>) =
        DSL.FilterChange((<>), template)

    static member inline Respond<'S, 'A, 'Q>(responder: Element -> 'Q -> unit) : Template<'S, 'A, 'Q> =
        respond responder

    static member OnMount<'S, 'A, 'Q>(handler: LifecycleMount<'S, 'A> -> unit) =
        DSL.Transform(
            (fun render ->
                (fun (state, container, _, dispatch) ->
                    handler
                        { State = state
                          Element = container
                          Dispatch = dispatch }

                    { Change = None
                      Destroy = None
                      Request = None })),
            DSL.Empty()
        )

    static member OnUpdate<'S, 'A, 'Q>(handler: 'S -> Element -> unit) =
        DSL.Transform(
            (fun render ->
                (fun (state, container, _, dispatch) ->
                    handler state container

                    { Change = Some(fun s -> handler s container)
                      Destroy = None
                      Request = None })),
            DSL.Empty()
        )

    static member OnRemove<'S, 'A, 'Q>(handler: Element -> unit) =
        DSL.Transform(
            (fun render ->
                (fun (_, container, _, dispatch) ->
                    { Change = None
                      Destroy = None
                      Request = Some(fun () -> handler container) })),
            DSL.Empty()
        )

    static member Lifecycle<'S, 'A, 'Q, 'P>
        (
            onMount: LifecycleMount<'S, 'A> -> 'P,
            onChange: LifecycleChange<'S, 'A, 'P> -> 'P,
            onRemove: LifecycleDestroy<'A, 'P> -> unit,
            respond: 'P -> 'Q -> unit
        ) : Template<'S, 'A, 'Q> =
        DSL.Transform(
            (fun render ->
                (fun (state, container, _, dispatch) ->
                    let mutable payload =
                        onMount
                            { Element = container
                              State = state
                              Dispatch = dispatch }

                    let change s =
                        payload <-
                            onChange
                                { Element = container
                                  State = s
                                  Payload = payload
                                  Dispatch = dispatch }

                    let destroy () =
                        onRemove
                            { Element = container
                              Payload = payload
                              Dispatch = dispatch }

                    let request q = respond payload q

                    { Change = Some change
                      Destroy = Some destroy
                      Request = Some request })),
            DSL.Empty()
        )

    static member Lifecycle<'S, 'A, 'Q, 'P>
        (
            onMount: LifecycleMount<'S, 'A> -> 'P,
            onChange: LifecycleChange<'S, 'A, 'P> -> 'P,
            onRemove: LifecycleDestroy<'A, 'P> -> unit
        ) : Template<'S, 'A, 'Q> =
        DSL.Lifecycle(onMount, onChange, onRemove, (fun _ _ -> ()))

    static member Lifecycle<'S, 'A, 'Q, 'P>
        (
            onMount: LifecycleMount<'S, 'A> -> 'P,
            onRemove: LifecycleDestroy<'A, 'P> -> unit,
            respond: 'P -> 'Q -> unit
        ) : Template<'S, 'A, 'Q> =
        DSL.Lifecycle(onMount, (fun { Payload = payload } -> payload), onRemove, respond)

    static member Lifecycle<'S, 'A, 'Q, 'P>
        (
            onMount: LifecycleMount<'S, 'A> -> 'P,
            onRemove: LifecycleDestroy<'A, 'P> -> unit
        ) : Template<'S, 'A, 'Q> =
        DSL.Lifecycle(onMount, (fun { Payload = payload } -> payload), onRemove, (fun _ _ -> ()))


    static member MakeCaptureState<'S1, 'S2, 'S3, 'A1, 'A2, 'A3, 'Q>() =
        let mutable localState1 = None
        let mutable localDispatch1 = None

        let hold (template: Template<'S1, 'A1, 'Q>) =
            DSL.Transform(
                (fun render ->
                    (fun (state, container, reference, dispatch) ->
                        localDispatch1 <- Some dispatch
                        localState1 <- Some state

                        let view =
                            render (state, container, reference, dispatch)


                        let change s =
                            localState1 <- Some s
                            Option.iter (fun c -> c s) view.Change

                        { Change = Some change
                          Destroy = view.Destroy
                          Request = view.Request })),
                template
            )

        let release
            (
                mergestates: 'S1 -> 'S2 -> 'S3,
                propagateAction: 'A3 -> 'A1 option,
                template: Template<'S3, 'A3, 'Q>
            ) : Template<'S2, 'A2, 'Q> =
            DSL.Transform(
                (fun render ->
                    (fun (state, container, reference, dispatch) ->
                        let mappedDispatch (a: 'A3) =
                            match (propagateAction a, localDispatch1) with
                            | (Some a, Some dispatch) -> dispatch a
                            | _ -> ()

                        let view =
                            render (mergestates (Option.get localState1) state, container, reference, mappedDispatch)

                        let change s =
                            Option.iter (fun c -> c (mergestates (Option.get localState1) s)) view.Change

                        { Change = Some change
                          Destroy = view.Destroy
                          Request = view.Request })),
                template
            )

        (hold, release)

    static member inline Component<'S, 'A, 'Q>
        (
            update: Update<'S, 'A>,
            middleware: Middleware<'S, 'A, 'Q>,
            template: Template<'S, 'A, 'Q>
        ) : Template<'S, 'A, 'Q> =
        ``component`` update middleware template

    static member inline Component<'S, 'A, 'Q>
        (
            update: Update<'S, 'A>,
            template: Template<'S, 'A, 'Q>
        ) : Template<'S, 'A, 'Q> =
        DSL.Component(update, ignore, template)

    // fsharplint:disable
    static member inline cls(text: string) = DSL.Attr("class", text)
    static member inline cls(f: 'S -> string) = DSL.Attr("class", f)
    static member inline cls(f: 'S -> string option) = DSL.cls (Option.defaultValue "" << f)
    static member inline cls(whenTrue: string, whenFalse: string) = DSL.Attr("class", whenTrue, whenFalse)

    static member inline cls(predicate: 'S -> bool, whenTrue: string, whenFalse: string) =
        DSL.Attr("class", predicate, whenTrue, whenFalse)

    static member inline elId(text: string) = DSL.Attr("id", text)
    static member inline elId(f: 'S -> string) = DSL.Attr("id", f)
    static member inline elId(whenTrue: string, whenFalse: string) = DSL.Attr("id", whenTrue, whenFalse)

    static member inline aria(name: string, text: string) = DSL.Attr($"aria-{name}", text)
    static member inline aria(name: string, f: 'S -> string) = DSL.Attr($"aria-{name}", f)

    static member inline aria(name: string, whenTrue: string, whenFalse: string) =
        DSL.Attr($"aria-{name}", whenTrue, whenFalse)

    static member inline innerHTML<'S, 'A, 'Q>(html: Value<'S, string option>) : Template<'S, 'A, 'Q> = DSL.PropValue("innerHTML", html)

    static member inline innerHTML<'S, 'A, 'Q>(html: Value<'S, string>) : Template<'S, 'A, 'Q> = DSL.PropValue("innerHTML", html)

    static member inline innerHTML<'S, 'A, 'Q>(html: 'S -> string option) : Template<'S, 'A, 'Q> = DSL.innerHTML (html |> Derived)

    static member inline innerHTML<'S, 'A, 'Q>(html: 'S -> string) : Template<'S, 'A, 'Q> = DSL.innerHTML (html |> Derived)
    // fsharplint:enable

    static member inline DIV<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("div", children)
    static member inline MAIN<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("main", children)
    static member inline ASIDE<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("aside", children)
    static member inline BUTTON<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("button", children)
    static member inline IMG<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("img", children)
    static member inline SPAN<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("span", children)

    static member inline Svg<'S, 'A, 'Q>(name: string, children: Template<'S, 'A, 'Q> list) =
        DSL.NSEl("http://www.w3.org/2000/svg", name, children)

    static member inline SVG<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.Svg("svg", children)
    static member inline PATH<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.Svg("path", children)
    static member inline INPUT<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("input", children)
    static member inline TEXTAREA<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("textarea", children)
    // fsharplint:disable
    static member inline INPUT_TEXT<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) =
        DSL.El("input", DSL.Attr("type", "text") :: children)

    static member inline INPUT_NUMBER<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) =
        DSL.El("input", DSL.Attr("type", "number") :: children)

    static member inline INPUT_CHECKBOX<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) =
        DSL.El("input", DSL.Attr("type", "checkbox") :: children)

    static member inline INPUT_RANGE<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) =
        DSL.El("input", DSL.Attr("type", "range") :: children)
    // fsharplint:enable

    static member inline SELECT<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("select", children)

    static member MakeSelect<'S, 'Q>(options: SelectOption<'S> list, isSelected: 'S -> 'S -> bool, children: Template<'S, 'S, 'Q> list) : Template<'S, 'S, 'Q> =
        let makeOptionOption (label: Value<'S, string>, isSelected: 'S -> bool) =
            DSL.El(
                "option",
                [ DSL.Prop("selected", isSelected)
                  DSL.Text label ]
            )

        let makeOptionGroup (label: string) (values) =
            DSL.El(
                "optgroup",
                DSL.Attr("label", label)
                :: (List.map makeOptionOption values)
            )

        let makeOption option =
            match option with
            | OptionValue (label, value) -> makeOptionOption (label, (isSelected value))
            | OptionGroup (label, values) -> makeOptionGroup label (List.map (fun (label, v) -> (label, isSelected v)) values)

        let values =
            List.fold
                (fun acc opt ->
                    match opt with
                    | OptionValue (_, value) -> acc @ [ value ]
                    | OptionGroup (_, values) -> acc @ List.map (fun (_, value) -> value) values)
                []
                options

        DSL.SELECT(
            DSL.SendElement<'S, 'S, 'Q>(
                "change",
                (fun el ->
                    let index = (el :?> HTMLSelectElement).selectedIndex
                    console.log (index)
                    List.item index values)
            )
            :: (children @ List.map makeOption options)
        )
