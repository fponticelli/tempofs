module App

open Browser
open Tempo.Core
open Tempo.Html

open Tempo.Html.DSL

open type Tempo.Html.DSL.HTML

type Action =
    | Reset
    | Increment of int
    | Set of int

type State = { Counter: int }
let makeState v = { Counter = v }
let state = { Counter = 0 }

let template : HTMLTemplate<State, Action, unit> =
    El(
        "sp-theme",
        [ Attr("scale", "smallest")
          Attr("color", "light")
          Attr(
              "style",
              "background: var(--spectrum-global-color-gray-75); padding: var(--spectrum-global-dimension-size-400);"
          ) ],
        [

          El(
              "div",
              [ Attr("style", "display: flex; justify-content: space-between;") ],
              [ El(
                  "sp-action-group",
                  [ Attr("compact", "compact") ],
                  [ El("sp-action-button", [ On("click", Increment -10) ], Text "-10")
                    El("sp-action-button", [ On("click", Increment -1) ], Text "-")
                    El("sp-action-button", [ On("click", Increment 1) ], Text "+")
                    El("sp-action-button", [ On("click", Increment 10) ], Text "+10") ]
                )
                El(
                    "sp-number-field",
                    [ On<_, _, Types.HTMLInputElement, _>(
                        "input",
                        (fun { Element = el } -> Set(el.value :> obj :?> int))
                      )
                      Attr("value", (fun { Counter = c } -> $"{c}")) ]
                )
                El("sp-textfield", [ On("change", Increment -1) ])
                El("sp-action-button", [ On("click", Reset) ], Text "reset") ]
          )
          El(
              "sp-slider",
              [ On<_, _, Types.HTMLInputElement, _>("input", (fun { Element = el } -> Set(el.value :> obj :?> int)))
                Attr("value", (fun { Counter = c } -> $"{c}"))
                Attr("label", "Slider")
                Attr("variant", "tick")
                Attr("tick-labels", "tick-labels")
                Attr("tick-step", "10") ]
          )
          El(
              "div",
              [ Attr("style", "padding: 20px 0") ],
              [ El(
                    "sp-card",
                    [ Attr("heading", (fun { Counter = counter } -> $"counting ... {counter}"))
                      Attr("subheading", "No Preview") ],
                    [ El(
                          "div",
                          [ Attr("slot", "footer") ],
                          [

                            El(
                                "sp-progress-circle",
                                [ Attr("label", "Done?")
                                  Attr(
                                      "progress",
                                      (fun { Counter = c } ->
                                          if c >= 0 && c <= 100 then
                                              Some $"{c}"
                                          else
                                              None)
                                  )
                                  Attr(
                                      "indeterminate",
                                      (fun { Counter = c } ->
                                          if c < 0 || c > 100 then
                                              Some ""
                                          else
                                              None)
                                  )
                                  Attr("size", "large") ]
                            )

                           ]
                      ) ]
                ) ]
          )
          El(
              "div",
              [ Text "count: "
                MapState(
                    (fun { Counter = counter } -> counter),
                    [ El("b", [ Attr("style", "font-size: 32px;") ], Text(fun s -> s.ToString()))
                      OneOf(
                          (fun v ->
                              if v > 9 then (Choice1Of3 " ... Great!")
                              else if v > 4 then (Choice2Of3 v)
                              else (Choice3Of3())),
                          Text(),
                          Text(fun s -> $" ... {s} is a good number"),
                          Text(" ... meh")
                      ) ]
                ) ]
          )
          El("ul", Seq((fun { Counter = counter } -> [ 1 .. counter ]), El("li", Text(fun i -> $"Item: {i}")))) ]
    )

let update state action =
    match action with
    | Increment by ->
        { state with
              Counter = state.Counter + by }
    | Reset -> { state with Counter = 0 }
    | Set v -> { state with Counter = v }

let middleware
    { Current = current
      Old = old
      Dispatch = dispatch
      Action = action }
    =
    console.log $"Action: {action}, State: {current}, Previous {old}"

let render = MakeProgram(template, document.body)

let view = render update middleware state