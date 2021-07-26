namespace Tempo.Html

open Browser.Types
open Tempo.Value
open Tempo.View
open Tempo.Browser
open Tempo.Html.Template

module Render =
    let rec makeEmptyRender _ : Render<'S, 'A, 'Q> =
        fun (_: 'S, _: Element, _: Node option, _: Dispatch<'A>) ->
            { Change = None
              Destroy = None
              Request = None }

    and makeElementRender
        (
            { Name = name
              NS = ns
              Children = children }: TElement<'S, 'A, 'Q>,
            isRoot
        ) : Render<'S, 'A, 'Q> =
        fun (state: 'S, container: Element, reference: Node option, dispatch: Dispatch<'A>) ->
            let element =
                Option.map (fun ns -> container.ownerDocument.createElementNS (ns, name)) ns
                |> Option.defaultWith (fun () -> container.ownerDocument.createElement (name) :> Element)

            // Any significant performance gain in puttin this after children views are rendered?
            // That would force libraries that rely on DOM characteristic to delay their assumptions.
            container.insertBefore (element, optionToMaybe reference)
            |> ignore

            let views =
                simplify children
                |> List.map (fun child -> (makeRender (child, false)) (state, element, None, dispatch))

            let view =
                { Change = None
                  Destroy =
                      if isRoot then
                          (Some(fun () -> remove (element)))
                      else
                          None
                  Request = None }

            mergeViews (view :: views)

    and makeFragmentRender (children: TFragment<'S, 'A, 'Q>, isRoot) : Render<'S, 'A, 'Q> =
        fun (state: 'S, container: Element, reference: Node option, dispatch: Dispatch<'A>) ->
            let views =
                simplify children
                |> List.map (fun child -> (makeRender (child, isRoot)) (state, container, reference, dispatch))

            mergeViews views

    and makeTextRender (value: TText<'S>, isRoot) : Render<'S, 'A, 'Q> =
        let makeDestroy node =
            if isRoot then
                (fun () -> remove node) |> Some
            else
                None

        match value with
        | Derived f ->
            fun (state: 'S, container: Element, reference: Node option, _: Dispatch<'A>) ->
                let v = f state

                let textNode =
                    container.ownerDocument.createTextNode (v)

                container.insertBefore (textNode, optionToMaybe reference)
                |> ignore

                let change s = textNode.nodeValue <- f s

                { Change = Some change
                  Destroy = makeDestroy (textNode)
                  Request = None }
        | Literal v ->
            fun (_: 'S, container: Element, reference: Node option, _: Dispatch<'A>) ->
                let textNode =
                    container.ownerDocument.createTextNode (v)

                container.insertBefore (textNode, optionToMaybe reference)
                |> ignore

                { Change = None
                  Destroy = makeDestroy (textNode)
                  Request = None }

    and makeAttributeRender ({ Name = name; Value = value }: TAttribute<'S>, isRoot) : Render<'S, 'A, 'Q> =
        let makeDestroy (el: Element) =
            if isRoot then
                (fun () -> el.removeAttribute (name)) |> Some
            else
                None

        match value with
        | Derived f ->
            fun (state: 'S, container: Element, _: Node option, _: Dispatch<'A>) ->
                let change s =
                    setAttributeOption (container, name, f s)

                change state

                { Change = Some change
                  Destroy = makeDestroy container
                  Request = None }
        | Literal v ->
            fun (_: 'S, container: Element, _: Node option, _: Dispatch<'A>) ->
                setAttributeOption (container, name, v)

                { Change = None
                  Destroy = makeDestroy container
                  Request = None }

    and makeStyleRender ({ Name = name; Value = value }: TStyle<'S>, isRoot) : Render<'S, 'A, 'Q> =
        let makeDestroy (el: Element) =
            if isRoot then
                (fun () -> deleteStyle (el, name)) |> Some
            else
                None

        match value with
        | Derived f ->
            fun (state: 'S, container: Element, _: Node option, _: Dispatch<'A>) ->
                let change s = setStyleOption (container, name, f s)
                change state

                { Change = Some change
                  Destroy = makeDestroy container
                  Request = None }
        | Literal v ->
            fun (_: 'S, container: Element, _: Node option, _: Dispatch<'A>) ->
                setStyleOption (container, name, v)

                { Change = None
                  Destroy = makeDestroy container
                  Request = None }

    and makeTransformRender (t: ITTransform<'S1, 'A1, 'Q1>, isRoot) : Render<'S1, 'A1, 'Q1> =
        unpackTransform
            t
            { new ITTransformInvoker<'S1, 'A1, 'Q1, Render<'S1, 'A1, 'Q1>> with
                member __.Invoke<'S2, 'A2, 'Q2>
                    (transform: TVTransform<'S1, 'S2, 'A1, 'A2, 'Q1, 'Q2>)
                    : Render<'S1, 'A1, 'Q1> =
                    let render =
                        makeRender<'S2, 'A2, 'Q2> (transform.Template, isRoot)

                    transform.Transform(render) }

    and makeOneOf2Render (t: ITOneOf2<'S, 'A, 'Q>, isRoot) : Render<'S, 'A, 'Q> =
        unpackOneOf2
            t
            { new ITOneOf2Invoker<'S, 'A, 'Q, Render<'S, 'A, 'Q>> with
                member __.Invoke<'S1, 'S2>(oneOf2: TVOneOf2<'S, 'S1, 'S2, 'A, 'Q>) : Render<'S, 'A, 'Q> =
                    fun (state': 'S, container: Element, reference: Node option, dispatch: Dispatch<'A>) ->
                        let ref =
                            container.ownerDocument.createTextNode ("") :> Node

                        let someRef = Some ref

                        container.insertBefore (ref, optionToMaybe reference)
                        |> ignore

                        let mutable assignament : Choice<View<'S1, 'Q>, View<'S2, 'Q>> =
                            match oneOf2.Choose state' with
                            | Choice1Of2 s ->
                                Choice1Of2(makeRender (oneOf2.Template1, true) (s, container, someRef, dispatch))
                            | Choice2Of2 s ->
                                Choice2Of2(makeRender (oneOf2.Template2, true) (s, container, someRef, dispatch))

                        let change (s: 'S) =
                            assignament <-
                                match (assignament, oneOf2.Choose s) with
                                | (Choice1Of2 v1) as c1, (Choice1Of2 s) ->
                                    Option.iter (fun f -> f s) v1.Change
                                    c1
                                | (Choice2Of2 v2) as c2, (Choice2Of2 s) ->
                                    Option.iter (fun f -> f s) v2.Change
                                    c2
                                | (Choice1Of2 v1), (Choice2Of2 s) ->
                                    Option.iter (fun f -> f ()) v1.Destroy
                                    Choice2Of2(makeRender (oneOf2.Template2, true) (s, container, someRef, dispatch))
                                | (Choice2Of2 v2), (Choice1Of2 s) ->
                                    Option.iter (fun f -> f ()) v2.Destroy
                                    Choice1Of2(makeRender (oneOf2.Template1, true) (s, container, someRef, dispatch))

                        let destroy () =
                            if isRoot then remove ref

                            match assignament with
                            | Choice1Of2 v1 -> Option.iter (fun f -> f ()) v1.Destroy
                            | Choice2Of2 v2 -> Option.iter (fun f -> f ()) v2.Destroy

                        let request q =
                            match assignament with
                            | Choice1Of2 v1 -> Option.iter (fun f -> f q) v1.Request
                            | Choice2Of2 v2 -> Option.iter (fun f -> f q) v2.Request

                        { Change = Some change
                          Destroy = Some destroy
                          Request = Some request } }

    and makePropertyRender (t: ITProperty<'S>, isRoot) : Render<'S, 'A, 'Q> =
        unpackProperty
            t
            { new ITPropertyInvoker<'S, Render<'S, 'A, 'Q>> with
                member __.Invoke<'V>(prop: TVProperty<'S, 'V>) : Render<'S, 'A, 'Q> =
                    let destroy el =
                        if isRoot then
                            (fun () -> deleteProperty (el, prop.Name)) |> Some
                        else
                            None

                    match prop.Value with
                    | Derived f ->
                        fun (state: 'S, container: Element, _: Node option, _: Dispatch<'A>) ->
                            let change s =
                                setPropertyOption (container, prop.Name, f s)

                            change state

                            { Change = Some change
                              Destroy = destroy container
                              Request = None }
                    | Literal v ->
                        fun (_: 'S, container: Element, _: Node option, _: Dispatch<'A>) ->
                            setPropertyOption (container, prop.Name, v)

                            { Change = None
                              Destroy = destroy container
                              Request = None } }

    and makeHandlerRender ({ Name = name; Handler = handler }: THandler<'S, 'A>, isRoot) : Render<'S, 'A, 'Q> =
        fun (state: 'S, container: Element, _: Node option, dispatch: Dispatch<'A>) ->
            let mutable localState = state
            let change s = localState <- s

            let listener (e: Event) =
                handler (
                    { Event = e
                      State = localState
                      Element = container
                      Dispatch = dispatch }
                )

            let destroy =
                if isRoot then
                    (fun () -> container.removeEventListener (name, listener))
                    |> Some
                else
                    None

            container.addEventListener (name, listener)

            { Change = Some change
              Destroy = destroy
              Request = None }

    and makeRespondRender (t: TRespond<'Q>, isRoot) : Render<'S, 'A, 'Q> =
        fun (_: 'S, container: Element, _: Node option, _: Dispatch<'A>) ->
            { Change = None
              Destroy = None
              Request = t container |> Some }

    and makeRender<'S, 'A, 'Q> (template: Template<'S, 'A, 'Q>, isRoot: bool) : Render<'S, 'A, 'Q> =
        match template with
        | TEmpty -> makeEmptyRender isRoot
        | TElement t -> makeElementRender (t, isRoot)
        | TFragment t -> makeFragmentRender (t, isRoot)
        | TText t -> makeTextRender (t, isRoot)
        | TAttribute t -> makeAttributeRender (t, isRoot)
        | TStyle t -> makeStyleRender (t, isRoot)
        | TTransform t -> makeTransformRender (t, isRoot)
        | TOneOf2 t -> makeOneOf2Render (t, isRoot)
        | TProperty t -> makePropertyRender (t, isRoot)
        | THandler t -> makeHandlerRender (t, isRoot)
        | TRespond t -> makeRespondRender (t, isRoot)
