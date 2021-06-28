namespace Tempo.Demo.Html2Tempo

open Tempo.Demo.Utils.Monaco
open Tempo.Html

open type Tempo.Html.DSL.HTML

module View =
    type Action = | Unknwon
    type State = { Value: unit }
    type Query = unit
    let update (state: State) (action: Action) = state

    let init () : State =
        a () |> ignore
        { Value = () }

    let template : HTMLTemplate<State, Action, Query> =
        DIV(
            [ DIV([ Text "Panel 1" ])
              DIV([ Text "Panel 2" ]) ]
        )
