namespace Tempo.Html

open Browser.Types
open Tempo.Value
open Tempo.View
open Tempo.Browser

module Template =
    type Dispatch<'A> = 'A -> unit

    type Render<'S, 'A, 'Q> = 'S -> Element -> Node option -> Dispatch<'A> -> View<'S, 'Q>

    type TElement<'S, 'A, 'Q> =
        { Name: string
          NS: string option
          Children: Template<'S, 'A, 'Q> list }

    and TFragment<'S, 'A, 'Q> = Template<'S, 'A, 'Q> list

    and TText<'S> = Value<'S, string>

    and TAttribute<'S> =
        { Name: string
          Value: Value<'S, string option> }

    and ITProperty<'S> =
        abstract Accept : ITPropertyInvoker<'S, 'R> -> 'R

    and TVProperty<'S, 'V>(name, value) =
        member this.Name : string = name
        member this.Value : Value<'S, 'V option> = value
        with
            interface ITProperty<'S> with
                member this.Accept f = f.Invoke<'V> this

    and ITPropertyInvoker<'S, 'R> =
        abstract Invoke<'V> : TVProperty<'S, 'V> -> 'R

    and TStyle<'S> =
        { Name: string
          Value: Value<'S, string option> }

    and THandlerPayload<'S, 'A> =
        { State: 'S
          Event: Event
          Element: Element
          Dispatch: Dispatch<'A> }

    and THandlerSendPayload<'S> =
        { State: 'S
          Event: Event
          Element: Element }

    and THandler<'S, 'A> =
        { Name: string
          Handler: THandlerPayload<'S, 'A> -> unit }

    and ITTransform<'S, 'A, 'Q> =
        abstract Accept : ITTransformInvoker<'S, 'A, 'Q, 'R> -> 'R

    and TVTransform<'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>(transform, template) =
        member this.Transform : Render<'S2, 'A2, 'Q2> -> Render<'S1, 'A1, 'Q1> = transform
        member this.Template : Template<'S2, 'A2, 'Q2> = template
        with
            interface ITTransform<'S1, 'A1, 'Q1> with
                member this.Accept f = f.Invoke<'S2, 'A2, 'Q2> this

    and ITTransformInvoker<'S1, 'A1, 'Q1, 'R> =
        abstract Invoke<'S2, 'A2, 'Q2> : TVTransform<'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2> -> 'R

    and ITOneOf2<'S, 'A, 'Q> =
        abstract Accept : ITOneOf2Invoker<'S, 'A, 'Q, 'R> -> 'R

    and TVOneOf2<'S, 'S1, 'S2, 'A, 'Q>(m, t1, t2) =
        member this.Choose : 'S -> Choice<'S1, 'S2> = m
        member this.Template1 : Template<'S1, 'A, 'Q> = t1
        member this.Template2 : Template<'S2, 'A, 'Q> = t2
        with
            interface ITOneOf2<'S, 'A, 'Q> with
                member this.Accept f = f.Invoke<'S1, 'S2> this

    and ITOneOf2Invoker<'S, 'A, 'Q, 'R> =
        abstract Invoke<'S1, 'S2> : TVOneOf2<'S, 'S1, 'S2, 'A, 'Q> -> 'R

    and TRespond<'Q> = Element -> 'Q -> unit

    and Template<'S, 'A, 'Q> =
        | TEmpty
        | TElement of TElement<'S, 'A, 'Q>
        | TFragment of TFragment<'S, 'A, 'Q>
        | TText of TText<'S>
        | TAttribute of TAttribute<'S>
        | TStyle of TStyle<'S>
        | TTransform of ITTransform<'S, 'A, 'Q>
        | TOneOf2 of ITOneOf2<'S, 'A, 'Q>
        | TProperty of ITProperty<'S>
        | THandler of THandler<'S, 'A>
        | TRespond of TRespond<'Q>

    let private aggregatedAttributes =
        [ "class", " "; "style", "; " ] |> Map.ofList

    // this fails at runtime if the list is empty
    let private foldSelf<'T> (f: 'T -> 'T -> 'T) (ls: 'T list) =
        let head = ls.Head
        let tail = ls.Tail
        List.fold f head tail

    let simplify (ls: Template<'S, 'A, 'Q> list) : Template<'S, 'A, 'Q> list =
        let flattened =
            List.collect
                (function
                | TFragment ls -> ls
                | TEmpty -> []
                | other -> [ other ])
                ls

        let (toMerge, ls) =
            List.fold
                (fun (map, ls) curr ->
                    match curr with
                    | TAttribute { Name = name; Value = value } when Map.containsKey name aggregatedAttributes ->
                        let existing =
                            Map.tryFind name map |> Option.defaultValue []

                        (Map.add name (value :: existing) map, ls)
                    | other -> (map, other :: ls))
                (Map.empty, [])
                flattened

        let append =
            Map.fold
                (fun acc name ls ->
                    let sep = Map.find name aggregatedAttributes

                    let value =
                        foldSelf
                            (fun acc value -> Value.Combine(Option.map2 (fun a b -> $"{a}{sep}{b}"), value, acc))
                            ls

                    (name, value) :: acc)
                []
                toMerge
            |> List.map (fun (name, value) -> TAttribute { Name = name; Value = value })

        append @ (List.rev ls)

    let packTransform<'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2> (t: TVTransform<'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>) = t :> ITTransform<'S1, 'A1, 'Q1>

    let makeTransform<'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2> (transform: Render<'S2, 'A2, 'Q2> -> Render<'S1, 'A1, 'Q1>, template: Template<'S2, 'A2, 'Q2>) : Template<'S1, 'A1, 'Q1> =
        TTransform(packTransform (TVTransform(transform, template)))

    let unpackTransform (t: ITTransform<'S, 'A, 'Q>) (f: ITTransformInvoker<'S, 'A, 'Q, 'R>) : 'R = t.Accept f

    let packOneOf2<'S, 'S1, 'S2, 'A, 'Q> (t: TVOneOf2<'S, 'S1, 'S2, 'A, 'Q>) = t :> ITOneOf2<'S, 'A, 'Q>

    let makeOneOf2<'S, 'S1, 'S2, 'A, 'Q> (choose: 'S -> Choice<'S1, 'S2>, template1: Template<'S1, 'A, 'Q>, template2: Template<'S2, 'A, 'Q>) : Template<'S, 'A, 'Q> =
        TOneOf2(packOneOf2 (TVOneOf2(choose, template1, template2)))

    let unpackOneOf2 (t: ITOneOf2<'S, 'A, 'Q>) (f: ITOneOf2Invoker<'S, 'A, 'Q, 'R>) : 'R = t.Accept f

    let packProperty<'S, 'V> (t: TVProperty<'S, 'V>) = t :> ITProperty<'S>

    let unpackProperty (t: ITProperty<'S>) (f: ITPropertyInvoker<'S, 'R>) : 'R = t.Accept f

    let makeProperty<'S, 'A, 'Q, 'V> (name: string, value: Value<'S, 'V option>) : Template<'S, 'A, 'Q> =
        TProperty(packProperty (TVProperty(name, value)))

    let forEach<'S, 'A, 'Q> (template: Template<'S, 'A, 'Q>) : Template<'S seq, 'A, 'Q> =
        makeTransform (
            (fun render ->
                (fun (states: 'S seq) (container: Element) (reference: Node option) dispatch ->
                    let ref =
                        container.ownerDocument.createTextNode ("") :> Node

                    let maybeRef = ref |> Some

                    container.insertBefore (ref, optionToMaybe reference)
                    |> ignore

                    let mutable views =
                        Seq.map (fun state -> render state container maybeRef dispatch) states

                    let change states =
                        let min =
                            System.Math.Min(Seq.length views, Seq.length states)

                        Seq.zip views states
                        |> Seq.iter (fun (view, state) -> Option.iter (fun c -> c state) view.Change)

                        Seq.skip min views
                        |> Seq.iter (fun view -> Option.iter (fun d -> d ()) view.Destroy)

                        views <- Seq.take min views

                        let newViews =
                            Seq.skip min states
                            |> Seq.map (fun state -> render state container maybeRef dispatch)

                        views <- Seq.concat [ views; newViews ]

                    let request q =
                        Seq.iter (fun (view: View<'S, 'Q>) -> Option.iter (fun r -> r q) view.Request) views

                    let destroy () =
                        Seq.iter (fun (view: View<'S, 'Q>) -> Option.iter (fun d -> d ()) view.Destroy) views

                    { Change = Some change
                      Destroy = Some destroy
                      Request = Some request })),
            template
        )
