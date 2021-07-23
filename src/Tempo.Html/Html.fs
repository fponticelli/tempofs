// namespace Tempo.Html

// open Tempo.Core
// open Browser.Types

// type HTMLTemplate<'S, 'A, 'Q> = Template<HTMLTemplateNode<'S, 'A, 'Q>, 'S, 'A, 'Q>

// and HTMLTemplateNode<'S, 'A, 'Q> =
//     | HTMLTemplateElementNS of string * HTMLTemplateElement<'S, 'A, 'Q>
//     | HTMLTemplateElement of HTMLTemplateElement<'S, 'A, 'Q>
//     | HTMLTemplateText of Value<'S, string>

// and HTMLTemplateElement<'S, 'A, 'Q> =
//     { Name: string
//       Attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list
//       Children: HTMLTemplate<'S, 'A, 'Q> list }

// and HTMLNamedAttribute<'S, 'A, 'Q> =
//     { Name: string
//       Value: HTMLTemplateAttributeValue<'S, 'A, 'Q> }

// and HTMLTemplateAttribute<'S, 'A, 'Q> =
//     | HTMLNamedAttribute of HTMLNamedAttribute<'S, 'A, 'Q>
//     | Lifecycle of IHTMLLifecycle<'S, 'A, 'Q>

// and HTMLTemplateAttributeValue<'S, 'A, 'Q> =
//     | StringAttr of Value<'S, string option>
//     | Property of IProperty<'S>
//     | Trigger of IHTMLTrigger<'S, 'A>

// and IProperty<'S> =
//     abstract Accept : IPropertyInvoker<'S, 'R> -> 'R

// and Property<'S, 'V>(value) =
//     member this.Value : Value<'S, 'V> = value
//     with
//         interface IProperty<'S> with
//             member this.Accept f = f.Invoke<'V> this

// and IPropertyInvoker<'S, 'R> =
//     abstract Invoke<'V> : Property<'S, 'V> -> 'R

// and IHTMLTrigger<'S, 'A> =
//     abstract Accept : IHTMLTriggerInvoker<'S, 'A, 'R> -> 'R

// and TriggerPayload<'S, 'EL, 'E when 'E :> Event and 'EL :> Element> = { State: 'S; Event: 'E; Element: 'EL }

// and HTMLTrigger<'S, 'A, 'EL, 'E when 'E :> Event and 'EL :> Element>(handler) =
//     member this.Handler : TriggerPayload<'S, 'EL, 'E> -> Dispatch<'A> -> unit = handler
//     with
//         interface IHTMLTrigger<'S, 'A> with
//             member this.Accept f = f.Invoke<'EL, 'E> this

// and IHTMLTriggerInvoker<'S, 'A, 'R> =
//     abstract Invoke<'EL, 'E when 'E :> Event and 'EL :> Element> : HTMLTrigger<'S, 'A, 'EL, 'E> -> 'R


// and IHTMLLifecycle<'S, 'A, 'Q> =
//     abstract Accept : IHTMLLifecycleInvoker<'S, 'A, 'Q, 'R> -> 'R

// and HTMLLifecycleInitialPayload<'S, 'A, 'EL when 'EL :> Element> =
//     { State: 'S
//       Element: 'EL
//       Dispatch: 'A -> unit }

// and HTMLLifecycleStatePayload<'S, 'A, 'EL, 'P when 'EL :> Element> =
//     { State: 'S
//       Element: 'EL
//       Payload: 'P
//       Dispatch: 'A -> unit }

// and HTMLLifecyclePayload<'A, 'EL, 'P when 'EL :> Element> =
//     { Element: 'EL
//       Payload: 'P
//       Dispatch: 'A -> unit }

// and HTMLLifecycle<'S, 'A, 'Q, 'EL, 'P when 'EL :> Element>
//     (
//         afterRender,
//         beforeChange,
//         afterChange,
//         beforeDestroy,
//         respond
//     ) =
//     member this.AfterRender : HTMLLifecycleInitialPayload<'S, 'A, 'EL> -> 'P = afterRender
//     member this.BeforeChange : HTMLLifecycleStatePayload<'S, 'A, 'EL, 'P> -> (bool * 'P) = beforeChange
//     member this.AfterChange : HTMLLifecycleStatePayload<'S, 'A, 'EL, 'P> -> 'P = afterChange
//     member this.BeforeDestroy : HTMLLifecyclePayload<'A, 'EL, 'P> -> unit = beforeDestroy
//     member this.Respond : 'Q -> HTMLLifecyclePayload<'A, 'EL, 'P> -> 'P = respond
//     with
//         interface IHTMLLifecycle<'S, 'A, 'Q> with
//             member this.Accept f = f.Invoke<'EL, 'P> this

// and IHTMLLifecycleInvoker<'S, 'A, 'Q, 'R> =
//     abstract Invoke<'EL, 'P when 'EL :> Element> : HTMLLifecycle<'S, 'A, 'Q, 'EL, 'P> -> 'R


// type HTMLLifecycle =
//     static member MapState<'S1, 'S2, 'A, 'Q, 'EL, 'P when 'EL :> Element>
//         (
//             mapState: 'S1 -> 'S2,
//             src: HTMLLifecycle<'S2, 'A, 'Q, 'EL, 'P>
//         ) : HTMLLifecycle<'S1, 'A, 'Q, 'EL, 'P> =
//         HTMLLifecycle<'S1, 'A, 'Q, 'EL, 'P>(
//             afterRender =
//                 (fun (payload: HTMLLifecycleInitialPayload<'S1, 'A, 'EL>) ->
//                     src.AfterRender
//                         { State = mapState payload.State
//                           Element = payload.Element
//                           Dispatch = payload.Dispatch }),
//             beforeChange =
//                 (fun (payload: HTMLLifecycleStatePayload<'S1, 'A, 'EL, 'P>) ->
//                     src.BeforeChange
//                         { State = mapState payload.State
//                           Element = payload.Element
//                           Payload = payload.Payload
//                           Dispatch = payload.Dispatch }),
//             afterChange =
//                 (fun (payload: HTMLLifecycleStatePayload<'S1, 'A, 'EL, 'P>) ->
//                     src.AfterChange
//                         { State = mapState payload.State
//                           Element = payload.Element
//                           Payload = payload.Payload
//                           Dispatch = payload.Dispatch }),
//             beforeDestroy =
//                 (fun (payload: HTMLLifecyclePayload<'A, 'EL, 'P>) ->
//                     src.BeforeDestroy
//                         { Element = payload.Element
//                           Payload = payload.Payload
//                           Dispatch = payload.Dispatch }),
//             respond =
//                 (fun (q: 'Q) (payload: HTMLLifecyclePayload<'A, 'EL, 'P>) ->
//                     src.Respond
//                         q
//                         { Element = payload.Element
//                           Payload = payload.Payload
//                           Dispatch = payload.Dispatch })
//         )

// //     static member MapState<'S1, 'S2, 'A, 'Q, 'R>
// //         (
// //             map: 'S1 -> 'S2,
// //             inv: IHTMLLifecycleInvoker<'S2, 'A, 'Q, 'R>
// //         ) : IHTMLLifecycleInvoker<'S1, 'A, 'Q, 'R> =
// //         failwith ""

// //     static member MapState<'S1, 'S2, 'A, 'Q>
// //         (
// //             map: 'S2 -> 'S1,
// //             lifecycle: IHTMLLifecycle<'S1, 'A, 'Q>
// //         ) : IHTMLLifecycle<'S2, 'A, 'Q> =
// //         { new IHTMLLifecycle<'S1, 'A, 'Q> with
// //             member this.Accept(f) =
// //                 (HTMLLifecycle.MapState(map, f))
// //                     .Invoke<'EL, 'P> this }
