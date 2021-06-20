module Tempo.Core

type Value<'V, 'S> =
    | Literal of 'V
    | Derived of ('S -> 'V)

(*
    TypeParamenters:
    'S -> State
    'A -> Action
    'Q -> Query
    'N -> Node for Virtual DOM (like HTML Element or Text Node descriptions)
    'I -> Implementation of the Virtual DOM (HTMLElement or TextNode instances)
*)

and Template<'N, 'S, 'A, 'Q when 'S: equality> =
    | Node of 'N
    | Fragment of Template<'N, 'S, 'A, 'Q> list
    // | MapState of MapStateEvaluator<'S>
    // MapAction
    // MapImpl
    // BindState
    // Query
    // Lifecycle
    // Capture capture/release state
    // Sequence
    // Component hold state, has update cycle
    // Adapter???
    // Lazy???
    // When???/If/Else

and RenderNode<'N, 'I, 'S when 'S: equality> = 'N -> 'I -> 'S -> View<'S, 'I>

and Render<'N, 'I, 'S when 'S: equality> = RenderNode<'N, 'I, 'S> -> Template<'N, 'S> -> 'I -> 'S -> View<'S, 'I>

and View<'S, 'I> =
    { Impl: 'I
      Change: 'S -> unit
      Destroy: unit -> unit }

let resolve<'V, 'S> (value: Value<'V, 'S>) (state: 'S) =
    match value with
    | Literal v -> v
    | Derived f -> f state

let rec render<'N, 'I, 'S when 'S: equality>
    (renderNode: RenderNode<'N, 'I, 'S>)
    (template: Template<'N, 'S>)
    (impl: 'I)
    (state: 'S)
    : View<'S, 'I> =
    match template with
    | Node n -> renderNode n impl state
    | Fragment ls ->
        let ls =
            List.map (fun i -> render renderNode i impl state) ls

        { Impl = impl
          Change = fun state -> List.iter (fun i -> i.Change state) ls
          Destroy = fun () -> List.iter (fun i -> i.Destroy()) ls }
    // | MapState ev ->
    //     let (map, template) = ev.Eval()
    //     render renderNode template impl <| map state
    // | Embed ev ->
    //     let (renderEmbedded, attach, template) = ev.Eval()
    //     let implEmbedded = attach impl
    //     render renderEmbedded template implEmbedded state
