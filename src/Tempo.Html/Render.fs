namespace Tempo.Html

open Browser.Types
open Tempo.Value
open Tempo.View
open Tempo.Html.Tools
open Tempo.Html.Template

module Render =
    let rec makeEmptyRender _ : Render<'S, 'A, 'Q> =
        fun (_: 'S) (_: Element) (_: Node option) (dispatch: Dispatch<'A>) ->
            { Change = None
              Destroy = None
              Request = None }

    and makeElementRender
        ({ Name = name
           NS = ns
           Children = children }: TElement<'S, 'A, 'Q>)
        isRoot
        : Render<'S, 'A, 'Q> =
        fun (state: 'S) (container: Element) (reference: Node option) (dispatch: Dispatch<'A>) ->
            let element =
                Option.map (fun ns -> container.ownerDocument.createElementNS (ns, name)) ns
                |> Option.defaultWith (fun () -> container.ownerDocument.createElement (name) :> Element)

            let views =
                List.map (fun child -> (makeRender child false) state element None dispatch) children

            container.insertBefore (element, optionToMaybe reference)
            |> ignore

            let view =
                { Change = None
                  Destroy =
                      if isRoot then
                          (Some(fun () -> remove (element)))
                      else
                          None
                  Request = None }

            mergeViews (view :: views)

    and makeFragmentRender ({ Children = children }: TFragment<'S, 'A, 'Q>) isRoot : Render<'S, 'A, 'Q> =
        fun (state: 'S) (container: Element) (reference: Node option) (dispatch: Dispatch<'A>) ->
            let views =
                List.map (fun child -> (makeRender child false) state container reference dispatch) children

            mergeViews views

    and makeTextRender ({ Value = value }: TText<'S>) isRoot : Render<'S, 'A, 'Q> =
        let makeDestroy node =
            if isRoot then
                (fun () -> remove node) |> Some
            else
                None

        match value with
        | Derived f ->
            fun (state: 'S) (container: Element) (reference: Node option) (dispatch: Dispatch<'A>) ->
                let v = f state

                let textNode =
                    container.ownerDocument.createTextNode (v)

                container.insertBefore (container, optionToMaybe reference)
                |> ignore

                let change s = textNode.nodeValue <- f s

                { Change = Some change
                  Destroy = makeDestroy (textNode)
                  Request = None }
        | Literal v ->
            fun (state: 'S) (container: Element) (reference: Node option) (dispatch: Dispatch<'A>) ->
                let textNode =
                    container.ownerDocument.createTextNode (v)

                container.insertBefore (container, optionToMaybe reference)
                |> ignore

                { Change = None
                  Destroy = makeDestroy (textNode)
                  Request = None }

    and makeAttributeRender ({ Name = name; Value = value }: TAttribute<'S>) isRoot : Render<'S, 'A, 'Q> =
        let makeDestroy (el: Element) =
            if isRoot then
                (fun () -> el.removeAttribute (name)) |> Some
            else
                None

        match value with
        | Derived f ->
            fun (state: 'S) (container: Element) (reference: Node option) (dispatch: Dispatch<'A>) ->
                let change s =
                    setAttributeOption (container, name, f s)

                change state

                { Change = Some change
                  Destroy = makeDestroy (container)
                  Request = None }
        | Literal v ->
            fun (state: 'S) (container: Element) (reference: Node option) (dispatch: Dispatch<'A>) ->
                setAttributeOption (container, name, v)

                { Change = None
                  Destroy = makeDestroy (container)
                  Request = None }

    and makeStyleRender ({ Name = name; Value = value }: TStyle<'S>) isRoot : Render<'S, 'A, 'Q> =
        let makeDestroy (el: Element) =
            if isRoot then
                (fun () -> deleteStyle (el, name)) |> Some
            else
                None

        match value with
        | Derived f ->
            fun (state: 'S) (container: Element) (reference: Node option) (dispatch: Dispatch<'A>) ->
                let change s = setStyleOption (container, name, f s)
                change state

                { Change = Some change
                  Destroy = makeDestroy (container)
                  Request = None }
        | Literal v ->
            fun (state: 'S) (container: Element) (reference: Node option) (dispatch: Dispatch<'A>) ->
                setStyleOption (container, name, v)

                { Change = None
                  Destroy = makeDestroy (container)
                  Request = None }

    and makeTransformRender (t: ITTransform<'S, 'A, 'Q>) isRoot : Render<'S, 'A, 'Q> =
        unpackTransform
            t
            { new ITTransformInvoker<'S, 'A, 'Q, Render<'S, 'A, 'Q>> with
                member __.Invoke<'S2, 'A2, 'Q2>(transform: TTransform<'S, 'S2, 'A, 'A2, 'Q, 'Q2>) : Render<'S, 'A, 'Q> =
                    let render = makeRender (transform.Template) isRoot
                    transform.Transform(render) }

    and makeOneOf2Render (t: ITOneOf2<'S, 'A, 'Q>) isRoot : Render<'S, 'A, 'Q> =
        unpackOneOf2
            t
            { new ITOneOf2Invoker<'S, 'A, 'Q, Render<'S, 'A, 'Q>> with
                member __.Invoke<'S1, 'S2>(oneOf2: TOneOf2<'S, 'S1, 'S2, 'A, 'Q>) : Render<'S, 'A, 'Q> =
                    fun (state: 'S) (container: Element) (reference: Node option) (dispatch: Dispatch<'A>) ->
                        let ref =
                            container.ownerDocument.createTextNode ("") :> Node

                        container.insertBefore (ref, optionToMaybe reference)
                        |> ignore

                        let mutable assignament : Choice<View2<'S1, 'Q>, View2<'S2, 'Q>> =
                            match oneOf2.Choose state with
                            | Choice1Of2 s ->
                                Choice1Of2(makeRender (oneOf2.Template1) true s container (Some ref) dispatch)
                            | Choice2Of2 s ->
                                Choice2Of2(makeRender (oneOf2.Template2) true s container (Some ref) dispatch)

                        let change s =
                            assignament <-
                                match (assignament, oneOf2.Choose s) with
                                | (Choice1Of2 v) as c, (Choice1Of2 s) ->
                                    Option.iter (fun f -> f s) v.Change
                                    c
                                | (Choice2Of2 v) as c, (Choice2Of2 s) ->
                                    Option.iter (fun f -> f s) v.Change
                                    c
                                | (Choice1Of2 v), (Choice2Of2 s) ->
                                    Option.iter (fun f -> f ()) v.Destroy
                                    Choice2Of2(makeRender (oneOf2.Template2) true s container (Some ref) dispatch)
                                | (Choice2Of2 v), (Choice1Of2 s) ->
                                    Option.iter (fun f -> f ()) v.Destroy
                                    Choice1Of2(makeRender (oneOf2.Template1) true s container (Some ref) dispatch)

                        let destroy () =
                            match assignament with
                            | Choice1Of2 v -> Option.iter (fun f -> f ()) v.Destroy
                            | Choice2Of2 v -> Option.iter (fun f -> f ()) v.Destroy

                        let request q =
                            match assignament with
                            | Choice1Of2 v -> Option.iter (fun f -> f q) v.Request
                            | Choice2Of2 v -> Option.iter (fun f -> f q) v.Request

                        { Change = Some change
                          Destroy = Some destroy
                          Request = Some request } }

    and makePropertyRender (t: ITProperty<'S>) isRoot : Render<'S, 'A, 'Q> =
        unpackProperty
            t
            { new ITPropertyInvoker<'S, Render<'S, 'A, 'Q>> with
                member __.Invoke<'V>(prop: TProperty<'S, 'V>) : Render<'S, 'A, 'Q> =
                    let destroy el =
                        if isRoot then
                            (fun () -> deleteProperty (el, prop.Name)) |> Some
                        else
                            None

                    match prop.Value with
                    | Derived f ->
                        fun (state: 'S) (container: Element) (reference: Node option) (dispatch: Dispatch<'A>) ->
                            let change s =
                                setPropertyOption (container, prop.Name, f s)

                            change state

                            { Change = Some change
                              Destroy = destroy container
                              Request = None }
                    | Literal v ->
                        fun (state: 'S) (container: Element) (reference: Node option) (dispatch: Dispatch<'A>) ->
                            setPropertyOption (container, prop.Name, v)

                            { Change = None
                              Destroy = destroy container
                              Request = None } }

    and makeHandlerRender ({ Name = name; Handler = handler }: THandler<'S, 'A>) isRoot : Render<'S, 'A, 'Q> =
        fun (state: 'S) (container: Element) (reference: Node option) (dispatch: Dispatch<'A>) ->
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

    and makeRespondRender (t: TRespond<'Q>) isRoot : Render<'S, 'A, 'Q> =
        fun (_: 'S) (container: Element) (_: Node option) (dispatch: Dispatch<'A>) ->
            { Change = None
              Destroy = None
              Request = t container |> Some }

    and makeRender<'S, 'A, 'Q> (template: TTemplate<'S, 'A, 'Q>) (isRoot: bool) : Render<'S, 'A, 'Q> =
        match template with
        | TEmpty -> makeEmptyRender isRoot
        | TElement t -> makeElementRender t isRoot
        | TFragment t -> makeFragmentRender t isRoot
        | TText t -> makeTextRender t isRoot
        | TAttribute t -> makeAttributeRender t isRoot
        | TStyle t -> makeStyleRender t isRoot
        | TTransform t -> makeTransformRender t isRoot
        | TOneOf2 t -> makeOneOf2Render t isRoot
        | TProperty t -> makePropertyRender t isRoot
        | THandler t -> makeHandlerRender t isRoot
        | TRespond t -> makeRespondRender t isRoot
