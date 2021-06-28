namespace Tempo.Demo.Html2Tempo

open Tempo.Demo.Utils.Monaco
open Tempo.Html

open type Tempo.Html.DSL.HTML

module View =
    type Html2TempoAction = | Unknwon
    type Html2TempoState = { Value: unit }
    type Html2TempoQuery = unit
    let update (state: Html2TempoState) (action: Html2TempoAction) = state

    let template : HTMLTemplate<Html2TempoState, Html2TempoAction, Html2TempoQuery> =
        DIV(
            [ cls "flex h-screen" ],
            [ DIV([ cls "flex-auto" ], [ Text "Panel 1" ])
              DIV(
                  [ cls "flex-auto" ],
                  [ MapState(
                        (fun (s: Html2TempoState) -> { Value = "hello" }: MonacoState),
                        MapAction((fun _ -> None), MonacoEditor())
                    ) ]
              ) ]
        )
