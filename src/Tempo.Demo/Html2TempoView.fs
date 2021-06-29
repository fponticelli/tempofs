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
            [ cls "flex h-full" ],
            [ DIV(
                [ cls "flex-auto h-full w-6/12"
                  MonacoEditorAttribute(
                      (fun s ->
                          {| value = "<a>Link</a>"
                             language = "html" |})
                  ) ]
              )
              DIV(
                  [ cls "flex-auto h-full w-6/12"
                    MonacoEditorAttribute(
                        (fun s ->
                            {| value = "let a = 2"
                               language = "fsharp" |})
                    ) ]
              ) ]
        )
