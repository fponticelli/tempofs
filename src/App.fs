module App

open Browser
open Tempo.Dom
open Tempo.Dom.DSL
open Tempo.Core

type Action =
    | Reset
    | Increment of int

type State = { Counter: int }
let makeState v = { Counter = v }
let state = { Counter = 0 }

let template : DOMTemplate<State, Action, unit> =
    Fragment [ DOM.El(
                   "div",
                   [ DOM.Attr("class", (fun { Counter = counter } -> $"size-{System.Math.Max(1, counter)}")) ],
                   [ DOM.El("button", [ DOM.on ("click", Increment -10) ], [ DOM.Text "-10" ])
                     DOM.El("button", [ DOM.on ("click", Increment -1) ], [ DOM.Text "-" ])
                     DOM.El("button", [ DOM.on ("click", Increment 1) ], [ DOM.Text "+" ])
                     DOM.El("button", [ DOM.on ("click", Increment 10) ], [ DOM.Text "+10" ])
                     DOM.El("button", [ DOM.on ("click", Reset) ], [ DOM.Text "reset" ]) ]
               )
               DOM.El(
                   "div",
                   [ DOM.Text "count: "
                     DOM.MapState(
                         (fun { Counter = counter } -> counter),
                         Fragment [ DOM.Text(fun s -> s.ToString())
                                    DOM.OneOf(
                                        (fun v ->
                                            if v > 10 then
                                                (Choice1Of2 "Awesome!")
                                            else
                                                (Choice2Of2())),
                                        DOM.Text(),
                                        DOM.Text("not so great")
                                    ) ]
                     ) ]
               ) ]

let update state action =
    match action with
    | Increment by ->
        { state with
              Counter = state.Counter + by }
    | Reset -> { state with Counter = 0 }

let middleware
    { Current = current
      Old = old
      Dispatch = dispatch
      Action = action }
    =
    console.log $"Action: {action}, State: {current}"

let render = DOM.Make(template, document.body)

let view = render update middleware state
