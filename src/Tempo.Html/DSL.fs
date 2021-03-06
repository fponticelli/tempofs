namespace Tempo.Html

open Browser
open Tempo.Std
open Tempo.Update
open Tempo.View
open Tempo.Value
open Tempo.Browser
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

type TransitionStateTime =
    { Delta: float
      Start: float
      Timestamp: float }

type TransitionStateOptions<'S> =
    { Step: int
      StartState: 'S
      EndState: 'S
      CurrentState: 'S
      Delta: float
      Start: float
      Timestamp: float }

type ControlChangeOptions<'S1, 'S2> =
    { PreviousState: 'S1
      CurrentState: 'S1
      Change: 'S2 -> unit }

type ControlRenderOptions<'S1, 'S2> = { State: 'S1; Change: 'S2 -> unit }

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

    static member inline valueAsNumber<'S, 'A, 'Q>(f: 'S -> float) : Template<'S, 'A, 'Q> =
        DSL.Prop<'S, 'A, 'Q, float>("valueAsNumber", f)

    static member inline valueAsNumber<'S, 'A, 'Q>(f: 'S -> int) : Template<'S, 'A, 'Q> =
        DSL.Prop<'S, 'A, 'Q, int>("valueAsNumber", f)

    static member inline valueAsText<'S, 'A, 'Q>(f: 'S -> string) : Template<'S, 'A, 'Q> =
        DSL.Prop<'S, 'A, 'Q, string>("value", f)

    static member inline isChecked<'S, 'A, 'Q>(f: 'S -> bool) : Template<'S, 'A, 'Q> =
        DSL.Prop<'S, 'A, 'Q, bool>("checked", f)

    static member StyleValue<'S, 'A, 'Q>(name: string, value: Value<'S, string option>) : Template<'S, 'A, 'Q> =
        TStyle { Name = name; Value = value }

    static member inline Style<'S, 'A, 'Q>(name: string, value: 'S -> string option) : Template<'S, 'A, 'Q> =
        DSL.StyleValue(name, Derived value)

    static member inline Style<'S, 'A, 'Q>(name: string, value: 'S -> string) : Template<'S, 'A, 'Q> =
        DSL.StyleValue(name, Value.Map Some (Derived value))

    static member inline Style<'S, 'A, 'Q>(name: string, value: string) : Template<'S, 'A, 'Q> =
        DSL.StyleValue(name, Literal(Some value))

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

    static member inline Click<'S, 'A, 'Q>(f: 'S -> 'A) : Template<'S, 'A, 'Q> =
        DSL.Send<'S, 'A, 'Q>("click", (fun ({ State = state }: SendPayload<'S>) -> f state))

    static member inline ClickLinkAction<'S, 'A, 'Q>(action: 'A) : Template<'S, 'A, 'Q> =
        DSL.On<'S, 'A, 'Q>(
            "click",
            (fun ({ Event = e
                    Element = el
                    Dispatch = dispatch }) ->
                let a = el :?> HTMLAnchorElement
                let e = e :?> MouseEvent

                let ctrlKey = if isMac then e.metaKey else e.ctrlKey

                if not ctrlKey then
                    e.preventDefault ()
                    history.pushState (null, a.title, a.href)
                    dispatch action)
        )

    static member inline ClickLinkState<'S, 'A, 'Q>(handler: 'S -> 'A) : Template<'S, 'A, 'Q> =
        DSL.Send<'S, 'A, 'Q>(
            "click",
            (fun ({ Event = e; State = s; Element = el }: SendPayload<'S>) ->
                let a = el :?> HTMLAnchorElement
                history.pushState (null, a.title, a.href)
                e.preventDefault ()
                handler s)
        )

    static member inline SendAction<'S, 'A, 'Q>(name: string, action: 'A) : Template<'S, 'A, 'Q> =
        DSL.Send<'S, 'A, 'Q>(name, (fun (_: SendPayload<'S>) -> action))

    static member inline ClickAction<'S, 'A, 'Q>(action: 'A) : Template<'S, 'A, 'Q> =
        DSL.Send<'S, 'A, 'Q>("click", (fun (_: SendPayload<'S>) -> action))

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
            template,
            false
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
            template,
            false
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
            template,
            false
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

    static member inline Maybe<'S, 'A, 'Q>(template: Template<'S, 'A, 'Q>) : Template<'S option, 'A, 'Q> =
        DSL.Maybe(id, template)

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

    static member inline ForEachArr<'S, 'A, 'Q>(template: Template<'S, 'A, 'Q>) : Template<'S [], 'A, 'Q> =
        forEachArray (template)

    static member inline ForEachArr<'S1, 'S2, 'A, 'Q>
        (
            map: 'S1 -> 'S2 [],
            template: Template<'S2, 'A, 'Q>
        ) : Template<'S1, 'A, 'Q> =
        DSL.MapState(map, forEachArray (template))

    static member inline Transform<'S1, 'A1, 'Q1, 'S2, 'A2, 'Q2>
        (
            transform: Render<'S2, 'A2, 'Q2> -> Render<'S1, 'A1, 'Q1>,
            template: Template<'S2, 'A2, 'Q2>,
            forceRoot: bool
        ) =
        makeTransform (transform, template, forceRoot)

    static member inline Transform<'S1, 'A1, 'Q1, 'S2, 'A2, 'Q2>
        (
            transform: Render<'S2, 'A2, 'Q2> -> Render<'S1, 'A1, 'Q1>,
            template: Template<'S2, 'A2, 'Q2>
        ) =
        makeTransform (transform, template, false)

    static member inline Transform<'S1, 'A1, 'Q1, 'S2, 'A2, 'Q2>(transform) =
        makeTransform (transform, DSL.Empty(), false)

    static member Portal<'S, 'A, 'Q>(selector: string, template: Template<'S, 'A, 'Q>) : Template<'S, 'A, 'Q> =
        DSL.Transform(
            (fun render ->
                (fun (state, _, _, dispatch: Dispatch<'A>) ->
                    let parent = document.querySelector selector // TODO this will fail at runtime if parent doesn't exist
                    render (state, parent, None, dispatch))),
            template
        )

    static member ControlRender<'S1, 'S2, 'A, 'Q>
        (
            makeController: unit -> (ControlRenderOptions<'S1, 'S2> -> unit),
            template: Template<'S2, 'A, 'Q>
        ) : Template<'S1, 'A, 'Q> =
        DSL.Transform(
            (fun (render: Render<'S2, 'A, 'Q>) ->
                fun (state, container, reference, dispatch) ->
                    let mutable destroyed = false
                    let mutable view: View<'S2, 'Q> option = None
                    let mutable localState = state

                    let ref =
                        container.ownerDocument.createTextNode ("") :> Node

                    container.insertBefore (ref, optionToMaybe reference)
                    |> ignore

                    let controller = makeController ()

                    controller
                        { State = localState
                          Change =
                              fun (s) ->
                                  if
                                      not
                                          (
                                              destroyed
                                              || isNullOrUndefined container.ownerDocument
                                          )
                                  then
                                      match view with
                                      | None -> view <- Some(render (s, container, Some ref, dispatch))
                                      | Some view -> Option.iter (fun c -> c s) view.Change }

                    let change s =
                        localState <- s

                        Option.iter
                            (fun (view: View<_, _>) ->
                                Option.iter
                                    (fun c ->
                                        let change v = if not destroyed then c v
                                        controller { State = localState; Change = change })
                                    view.Change)
                            view

                    let destroy () =
                        destroyed <- true
                        Option.iter (fun (view: View<_, _>) -> Option.iter (fun d -> d ()) view.Destroy) view

                    let request q =
                        Option.iter (fun (view: View<_, _>) -> Option.iter (fun r -> r q) view.Request) view

                    { Change = Some change
                      Destroy = Some destroy
                      Request = Some request }),
            template,
            true
        )

    static member ControlChange<'S1, 'S2, 'A, 'Q>
        (
            mapRender: 'S1 -> 'S2,
            makeController: unit -> (ControlChangeOptions<'S1, 'S2> -> unit),
            template: Template<'S2, 'A, 'Q>
        ) : Template<'S1, 'A, 'Q> =
        DSL.Transform(
            (fun (render: Render<'S2, 'A, 'Q>) ->
                fun (state, container, reference, dispatch) ->
                    let mutable destroyed = false
                    let mutable localState = state

                    let view =
                        render (mapRender localState, container, reference, dispatch)

                    let controller = makeController ()

                    let change =
                        Option.map
                            (fun c ->
                                let change v = if not destroyed then c v

                                fun currentState ->
                                    controller
                                        { CurrentState = currentState
                                          PreviousState = localState
                                          Change = change }

                                    localState <- currentState)
                            view.Change

                    let destroy () =
                        destroyed <- true
                        Option.iter (fun d -> d ()) view.Destroy

                    { Change = change
                      Destroy = Some destroy
                      Request = view.Request }),
            template
        )

    static member ControlChange<'S, 'A, 'Q>
        (
            makeController: unit -> (ControlChangeOptions<'S, 'S> -> unit),
            template: Template<'S, 'A, 'Q>
        ) : Template<'S, 'A, 'Q> =
        DSL.ControlChange(id, makeController, template)

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
            (fun _ ->
                (fun { CurrentState = curr
                       PreviousState = prev
                       Change = change } ->
                    if (predicate curr prev) then
                        change curr)),
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

    static member TransitionState<'S, 'A, 'Q>
        (
            makeTriggerNext: unit -> ((TransitionStateTime -> unit) -> (unit -> unit)),
            interpolate: TransitionStateOptions<'S> -> ('S * bool),
            template: Template<'S, 'A, 'Q>
        ) : Template<'S, 'A, 'Q> =
        let makeController () =
            let triggerNext = makeTriggerNext ()
            let mutable cancel = None

            let controller
                { CurrentState = curr
                  PreviousState = prev
                  Change = change }
                =
                let startState = prev
                let step = 0
                Option.iter (fun c -> c ()) cancel
                let mutable current = None

                let rec nextCallback (time: TransitionStateTime) : unit =
                    let payload =
                        { Step = step
                          StartState = startState
                          EndState = curr
                          CurrentState = Option.defaultValue startState current
                          Start = time.Start
                          Delta = time.Delta
                          Timestamp = time.Timestamp }

                    let (stateToApply, isDone) = interpolate payload
                    current <- Some stateToApply
                    change stateToApply

                    if not isDone then
                        cancel <- triggerNext nextCallback |> Some
                    else
                        cancel <- None

                cancel <- triggerNext nextCallback |> Some

            controller

        DSL.ControlChange(makeController, template)

    static member TransitionState<'S, 'A, 'Q>
        (
            interpolate: TransitionStateOptions<'S> -> ('S * bool),
            template: Template<'S, 'A, 'Q>
        ) : Template<'S, 'A, 'Q> =
        let makeTriggerNext () =
            let mutable start = None

            let triggerNext callback =
                if Option.isNone start then
                    start <- Some(performanceNow ())

                let current = performanceNow ()

                let frameCallback (timestamp: float) =
                    callback
                        { Delta = timestamp - current
                          Start = Option.get start
                          Timestamp = timestamp }

                let cancelId =
                    window.requestAnimationFrame frameCallback

                fun () ->
                    start <- None
                    window.cancelAnimationFrame cancelId

            triggerNext

        DSL.TransitionState(makeTriggerNext, interpolate, template)

    // fsharplint:disable
    static member inline cls(text: string) = DSL.Attr("class", text)
    static member inline cls(f: 'S -> string) = DSL.Attr("class", f)
    static member inline cls(f: 'S -> string option) = DSL.cls (Option.defaultValue "" << f)
    static member inline cls(whenTrue: string, whenFalse: string) = DSL.Attr("class", whenTrue, whenFalse)

    static member inline cls(predicate: 'S -> bool, whenTrue: string, whenFalse: string) =
        DSL.Attr("class", predicate, whenTrue, whenFalse)

    static member inline href(text: string) = DSL.Attr("href", text)
    static member inline href(f: 'S -> string) = DSL.Attr("href", f)

    static member inline src(text: string) = DSL.Attr("src", text)
    static member inline src(f: 'S -> string) = DSL.Attr("src", f)

    static member inline elId(text: string) = DSL.Attr("id", text)
    static member inline elId(f: 'S -> string) = DSL.Attr("id", f)
    static member inline elId(whenTrue: string, whenFalse: string) = DSL.Attr("id", whenTrue, whenFalse)

    static member inline aria(name: string, text: string) = DSL.Attr($"aria-{name}", text)
    static member inline aria(name: string, f: 'S -> string) = DSL.Attr($"aria-{name}", f)

    static member inline aria(name: string, whenTrue: string, whenFalse: string) =
        DSL.Attr($"aria-{name}", whenTrue, whenFalse)

    static member inline innerHTML<'S, 'A, 'Q>(html: Value<'S, string option>) : Template<'S, 'A, 'Q> =
        DSL.PropValue("innerHTML", html)

    static member inline innerHTML<'S, 'A, 'Q>(html: Value<'S, string>) : Template<'S, 'A, 'Q> =
        DSL.PropValue("innerHTML", html)

    static member inline innerHTML<'S, 'A, 'Q>(html: 'S -> string option) : Template<'S, 'A, 'Q> =
        DSL.innerHTML (html |> Derived)

    static member inline innerHTML<'S, 'A, 'Q>(html: 'S -> string) : Template<'S, 'A, 'Q> =
        DSL.innerHTML (html |> Derived)
    // fsharplint:enable

    static member inline DIV<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("div", children)
    static member inline UL<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("ul", children)
    static member inline OL<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("ol", children)
    static member inline LI<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("li", children)
    static member inline DL<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("dl", children)
    static member inline DT<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("dt", children)
    static member inline DD<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("dd", children)
    static member inline MAIN<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("main", children)
    static member inline ASIDE<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("aside", children)
    static member inline BUTTON<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("button", children)
    static member inline IMG<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("img", children)
    static member inline SPAN<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("span", children)
    static member inline A<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("a", children)
    static member inline P<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("p", children)
    static member inline ARTICLE<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("article", children)
    static member inline NAV<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("nav", children)
    static member inline H1<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("h1", children)
    static member inline H2<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("h2", children)
    static member inline H3<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("h3", children)
    static member inline H4<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("h4", children)
    static member inline H5<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("h5", children)
    static member inline H6<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("h6", children)
    static member inline TABLE<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("table", children)
    static member inline THEAD<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("thead", children)
    static member inline TBODY<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("tbody", children)
    static member inline TFOOT<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("tfoot", children)
    static member inline TR<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("tr", children)
    static member inline TD<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("td", children)
    static member inline TH<'S, 'A, 'Q>(children: Template<'S, 'A, 'Q> list) = DSL.El("th", children)

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

    static member MakeSelect<'S, 'Q>
        (
            options: SelectOption<'S> list,
            isSelected: 'S -> 'S -> bool,
            children: Template<'S, 'S, 'Q> list
        ) : Template<'S, 'S, 'Q> =
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
            | OptionGroup (label, values) ->
                makeOptionGroup label (List.map (fun (label, v) -> (label, isSelected v)) values)

        let values =
            List.fold
                (fun acc opt ->
                    match opt with
                    | OptionValue (_, value) -> acc @ [ value ]
                    | OptionGroup (_, values) -> acc @ List.map (fun (_, value) -> value) values)
                []
                options

        DSL.SELECT(
            DSL.Send<'S, 'S, 'Q>(
                "change",
                (fun { Element = el } ->
                    let index = (el :?> HTMLSelectElement).selectedIndex
                    List.item index values)
            )
            :: (children @ List.map makeOption options)
        )

    static member inline Lazy(f: unit -> Template<'S, 'A, 'Q>) : Template<'S, 'A, 'Q> = lazyt f
