module Tempo.Dom

// open Browser
// open Tempo.Core
// open Tempo.Dom.Helper
// open Browser.Types

// type DOMTemplate<'S when 'S: equality> = Template<DOMNode<'S>, 'S>

// and DOMElement<'S when 'S: equality> =
//     { Name: string
//       Attributes: DOMAttribute<'S> list
//       Children: DOMTemplate<'S> list }

// and DOMAttributeValue<'S> =
//     | StringValue of Value<string option, 'S>
//     | TriggerValue of DOMTrigger<'S>

// and DOMAttribute<'S> =
//     { Name: string
//       Value: DOMAttributeValue<'S> }

// and DOMTrigger<'S> =
//     abstract Accept : DOMTriggerFunc<'R> -> 'R

// and DOMTrigger<'S, 'E, 'A when 'E :> Types.Event>(handler: 'S -> 'E -> 'A) =
//     // member this.Apply name (el: HTMLElement) (dispatch: 'A -> unit) (state: 'S) =
//     //     el.addEventListener(name, fun (e: Types.Event) -> dispatch <| this.Handler state (e :?> 'E))
//     member this.Handler = handler
//     with
//         interface DOMTrigger<'S> with
//             member this.Accept f = f.Invoke<'S, 'E, 'A> this

// and DOMTriggerFunc<'R> =
//     abstract Invoke<'S, 'E, 'A when 'E :> Types.Event> : DOMTrigger<'S, 'E, 'A> -> 'R

// and DOMNode<'S when 'S: equality> =
//     | DOMElement of DOMElement<'S>
//     | DOMText of Value<string, 'S>
// // Portal
// // Namespace / SVG


// let makeTrigger (name: string) (f: 'S -> 'E -> 'A) =
//     let t = new DOMTrigger<'S, 'E, 'A>(f)

//     { Name = name
//       Value = TriggerValue <| (t :> DOMTrigger<'S>) }

// let unpackTrigger (f: DOMTriggerFunc<'R>) (trigger: DOMTrigger<'S>) : 'R = trigger.Accept f

// let isAttributeDerived ({ Value = value }: DOMAttribute<_>) =
//     match value with
//     | StringValue (Derived _) -> true
//     | StringValue (Literal _) -> false
//     | TriggerValue _ -> false

// let applyAttribute (el: HTMLElement) (state: 'S) ({ Value = value; Name = name }: DOMAttribute<'S>) =
//     match value with
//     | StringValue v ->
//         match resolve v state with
//         | Some v -> el.setAttribute (name, v)
//         | None -> el.removeAttribute (name)
//     | TriggerValue _ -> ()

// let applyAttributeF (el: HTMLElement) ({ Value = value; Name = name }: DOMAttribute<'S>) : ('S -> unit) =
//     match value with
//     | StringValue v ->
//         fun state ->
//             match resolve v state with
//             | Some v -> el.setAttribute (name, v)
//             | None -> el.removeAttribute (name)
//     | TriggerValue _ -> fun _ -> ()

// // let applyTrigger (el: HTMLElement) (state: unit -> 'S) (dispatch: 'A -> unit) (name: string) (trigger: DOMTrigger<'S>) =
// //     let f =
// //         unpackTrigger
// //             trigger
// //             ({ new DOMTriggerFunc<'S -> 'E -> 'A> with
// //                 override this.Invoke t = t.Handler })

// //     el.addEventListener (name, (fun e -> f (state ()) e))

// let rec renderDOMElement<'S, 'E, 'A when 'S: equality and 'E :> Browser.Types.Event>
//     (dispatch: 'A -> unit)
//     (node: DOMElement<'S>)
//     (impl: HTMLElement)
//     (state: 'S)
//     : View<'S, HTMLElement> =
//     let mutable localState = state
//     let el = document.createElement node.Name

//     let (derived, _) =
//         List.partition isAttributeDerived node.Attributes

//     let derived = List.map (applyAttributeF el) derived

//     let unpackHandlerF =
//         unpackTrigger
//             { new DOMTriggerFunc<'S -> 'E -> 'A> with
//                 override this.Invoke(a: DOMTrigger<'S, 'E, 'A>) = a.Handler }

//     let triggers =
//         List.fold
//             (fun acc (curr: DOMAttribute<'S>) ->
//                 match curr with
//                 | { Name = name
//                     Value = TriggerValue trigger } -> (name, trigger) :: acc
//                 | _ -> acc)
//             []
//             node.Attributes
//         |> List.map (fun (name, trigger) -> (name, unpackHandlerF trigger))
//         |> List.map
//             (fun (name, f) -> el.addEventListener (name, (fun (e: Types.Event) -> dispatch <| f state (e :?> 'E))))

//     List.iter (fun a -> applyAttributeF el a localState) node.Attributes

//     let children =
//         List.map (fun c -> render (renderDOMNode dispatch) c el localState) node.Children

//     impl.appendChild el |> ignore

//     { Impl = el
//       Change =
//           (fun state ->
//               localState <- state
//               List.iter (fun f -> f localState) derived
//               List.iter (fun v -> v.Change localState) children)
//       Destroy =
//           (fun () ->
//               remove el
//               List.iter (fun v -> v.Destroy()) children) }

// and renderDOMText (node: Value<string, 'S>) (impl: HTMLElement) (state: 'S) : View<'S, HTMLElement> =
//     let s = resolve node state
//     let n = document.createTextNode s

//     { Impl = impl
//       Change = (fun state -> n.nodeValue <- resolve node state)
//       Destroy = (fun () -> remove n) }

// and renderDOMNode (dispatch: 'A -> unit) (node: DOMNode<'S>) (impl: HTMLElement) (state: 'S) : View<'S, HTMLElement> =
//     match node with
//     | DOMElement el -> renderDOMElement dispatch el impl state
//     | DOMText v -> renderDOMText v impl state

// let renderDOM
//     (dispatch: 'A -> unit)
//     (template: DOMTemplate<'S>)
//     (parent: HTMLElement)
//     (state: 'S)
//     : View<'S, HTMLElement> =
//     render (renderDOMNode dispatch) template parent state
