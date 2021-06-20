module App

open Browser
open Tempo.Dom
open Tempo.Core

type Action =
    | Increment
    | Decrement

type State = { Counter: int }
let makeState v = { Counter = v }
let state = { Counter = 0 }

let trigger =
    { new DOMTriggerEvaluator<State> with
        member __.Eval() = //: DOMTriggerF<State, Types.MouseEvent, Action> =
            (fun s e -> Increment)  //:> DOMTriggerF<State, 'a, 'b>
            }

let template =
    Node
    <| DOMElement
        { Name = "div"
          Attributes = []
          Triggers = [] // [ { Name = "click"; Action = trigger } ]
          Children =
              [ Derived(fun { Counter = counter } -> $"count: {counter}")
                |> DOMText
                |> Node ] }

let view =
    renderDOM (fun (v: Action) -> (console.log v)) template document.body state

let eq = makeState 1 = makeState 1
console.log eq