module App

open Browser
open Tempo.Core
open Tempo.Html

open Tempo.Html.DSL

open type Tempo.Html.DSL.HTML

type Action =
    | Reset
    | Increment of int

type State = { Counter: int }
let makeState v = { Counter = v }
let state = { Counter = 0 }

let template : HTMLTemplate<State, Action, unit> =
    Fragment [ El(
                   "div",
                   [ Attr("class", (fun { Counter = counter } -> $"size-{System.Math.Max(1, counter)}")) ],
                   [ El("button", [ on ("click", Increment -10) ], Text "-10")
                     El("button", [ on ("click", Increment -1) ], Text "-")
                     El("button", [ on ("click", Increment 1) ], Text "+")
                     El("button", [ on ("click", Increment 10) ], Text "+10")
                     El("button", [ on ("click", Reset) ], Text "reset") ]
               )
               El(
                   "div",
                   [ Text "count: "
                     MapState(
                         (fun { Counter = counter } -> counter),
                         [ El(
                             "b",
                             [ Attr("style", "font-size: 32px;") ],
                             [ Text("(")
                               Text(fun s -> s.ToString())
                               Text(") ") ]
                           )
                           OneOf(
                               (fun v ->
                                   if v > 9 then (Choice1Of3 "Great!")
                                   else if v > 4 then (Choice2Of3 v)
                                   else (Choice3Of3())),
                               Text(),
                               Text(fun s -> $"{s} is a good number"),
                               Text("meh")
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

let render = MakeProgram(template, document.body)

let view = render update middleware state
