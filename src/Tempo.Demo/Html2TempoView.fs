namespace Tempo.Demo.Html2Tempo

open Browser
open Tempo.Core
open Tempo.Demo.Utils.Monaco
open Tempo.Html
open Tempo.Demo.Utils.HtmlParser

open type Tempo.Html.DSL

module View =
    type Html2TempoAction =
        | HtmlPasted
        | HtmlChanged
        | SetFilterComments of bool

    type Html2TempoState = { FilterComments: bool }

    type Html2TempoQuery =
        | GetHtml of (string -> unit)
        | SetGenerated of string

    let update (state: Html2TempoState) (action: Html2TempoAction) =
        match action with
        | SetFilterComments on -> { state with FilterComments = on }
        | HtmlPasted
        | HtmlChanged -> state

    let mutable timerId : float option = None

    let middleware
        ({ Current = { FilterComments = filterComments }
           Previous = prev
           Action = action
           Query = query }: MiddlewarePayload<_, _, _>)
        =
        let transformRoundtrip filterComments =
            query (
                GetHtml(
                    (transformHtml filterComments)
                    >> SetGenerated
                    >> query
                )
            )

        match action with
        | HtmlPasted -> transformRoundtrip filterComments
        | HtmlChanged ->
            match timerId with
            | Some id -> window.clearTimeout id
            | None -> ()

            timerId <-
                Some
                <| window.setTimeout (
                    (fun _ ->
                        timerId <- None
                        transformRoundtrip filterComments),
                    200
                )
        | SetFilterComments _ -> transformRoundtrip filterComments

    let sample =
        """<div class="relative w-screen max-w-md">
    <div class="h-full flex flex-col py-6 bg-white shadow-xl overflow-y-scroll">
        <div class="px-4 sm:px-6">
        <h2 class="text-lg font-medium text-gray-900" id="slide-over-title">
            Panel title
        </h2>
        </div>
        <div class="mt-6 relative flex-1 px-4 sm:px-6">
            <!-- Replace with your content -->
            <div class="absolute inset-0 px-4 sm:px-6">
                <div class="h-full border-2 border-dashed border-gray-200" aria-hidden="true"></div>
            </div>
            <!-- /End replace -->
        </div>
    </div>
</div>
    """

    let bar : HTMLTemplate<Html2TempoState, Html2TempoAction, Html2TempoQuery> =
        El(
            "header",
            [ cls "bg-white shadow-sm lg:static lg:overflow-y-visible" ],
            [ DIV(
                  [ cls "max-w-7xl mx-auto px-4 sm:px-6 lg:px-8" ],
                  [ DIV(
                        [ cls "relative flex justify-between xl:grid xl:grid-cols-12 lg:gap-8" ],
                        [ DIV(
                              [ cls "min-w-0 flex-1 md:px-8 lg:px-0 xl:col-span-6" ],
                              [ DIV(
                                    [ cls
                                          "flex items-center px-6 py-4 md:max-w-3xl md:mx-auto lg:max-w-none lg:mx-0 xl:px-0" ],
                                    [ DIV(
                                          [ cls "w-full" ],
                                          [ DIV(
                                                [ cls "relative" ],
                                                [ DIV(
                                                      [ cls "flex items-center" ],
                                                      MapState(
                                                          (fun { FilterComments = v } -> v),
                                                          Fragment [ BUTTON(
                                                                         [ id "filter-comments"
                                                                           Attr("type", "button")
                                                                           On<_, Html2TempoAction, _>(
                                                                               "click",
                                                                               not >> SetFilterComments
                                                                           )
                                                                           cls (
                                                                               "relative inline-flex flex-shrink-0 h-6 w-11 border-2 border-transparent rounded-full cursor-pointer transition-colors ease-in-out duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                                                                           )
                                                                           cls (
                                                                               "bg-indigo-600",
                                                                               "bg-gray-200 focus:bg-gray-200"
                                                                           )
                                                                           Attr("role", "switch")
                                                                           aria ("checked", "true", "false") ],
                                                                         [ SPAN(
                                                                             [ cls "sr-only" ],
                                                                             [ Text("Filter-out comments") ]
                                                                           )
                                                                           SPAN(
                                                                               [ cls
                                                                                   "pointer-events-none relative inline-block h-5 w-5 rounded-full bg-white shadow transform ring-0 transition ease-in-out duration-200"
                                                                                 cls ("translate-x-5", "translate-x-0") ],
                                                                               [ SPAN(
                                                                                   [ cls
                                                                                       "absolute inset-0 h-full w-full flex items-center justify-center transition-opacity"
                                                                                     cls (
                                                                                         "opacity-0 ease-out duration-100",
                                                                                         "opacity-100 ease-in duration-200"
                                                                                     )
                                                                                     aria ("hidden", "true") ],
                                                                                   [ SVG(
                                                                                         [ cls "h-3 w-3 text-gray-400"
                                                                                           Attr("fill", "none")
                                                                                           Attr("viewBox", "0 0 12 12") ],
                                                                                         [ PATH(
                                                                                               [ Attr(
                                                                                                   "d",
                                                                                                   "M4 8l2-2m0 0l2-2M6 6L4 4m2 2l2 2"
                                                                                                 )
                                                                                                 Attr(
                                                                                                     "stroke",
                                                                                                     "currentColor"
                                                                                                 )
                                                                                                 Attr(
                                                                                                     "stroke-width",
                                                                                                     "2"
                                                                                                 )
                                                                                                 Attr(
                                                                                                     "stroke-linecap",
                                                                                                     "round"
                                                                                                 )
                                                                                                 Attr(
                                                                                                     "stroke-linejoin",
                                                                                                     "round"
                                                                                                 ) ]
                                                                                           ) ]
                                                                                     ) ]
                                                                                 )
                                                                                 SPAN(
                                                                                     [ cls
                                                                                         "absolute inset-0 h-full w-full flex items-center justify-center transition-opacity"
                                                                                       cls (
                                                                                           "opacity-100 ease-in duration-200",
                                                                                           "opacity-0 ease-out duration-100"
                                                                                       )
                                                                                       aria ("hidden", "true") ],
                                                                                     [ SVG(
                                                                                           [ cls
                                                                                               "h-3 w-3 text-indigo-600"
                                                                                             Attr(
                                                                                                 "fill",
                                                                                                 "currentColor"
                                                                                             )
                                                                                             Attr(
                                                                                                 "viewBox",
                                                                                                 "0 0 12 12"
                                                                                             ) ],
                                                                                           [ PATH(
                                                                                                 [ Attr(
                                                                                                       "d",
                                                                                                       "M3.707 5.293a1 1 0 00-1.414 1.414l1.414-1.414zM5 8l-.707.707a1 1 0 001.414 0L5 8zm4.707-3.293a1 1 0 00-1.414-1.414l1.414 1.414zm-7.414 2l2 2 1.414-1.414-2-2-1.414 1.414zm3.414 2l4-4-1.414-1.414-4 4 1.414 1.414z"
                                                                                                   ) ]
                                                                                             ) ]
                                                                                       ) ]
                                                                                 ) ]
                                                                           ) ]
                                                                     )
                                                                     El(
                                                                         "label",
                                                                         [ Attr("for", "filter-comments") ],
                                                                         [ SPAN(
                                                                               [ cls "ml-3" ],
                                                                               [ SPAN(
                                                                                     [ cls
                                                                                           "text-sm font-medium text-gray-900" ],
                                                                                     [ Text("Filter-out Comments") ]
                                                                                 ) ]
                                                                           ) ]
                                                                     ) ]
                                                      )
                                                  ) ]
                                            ) ]
                                      ) ]
                                ) ]
                          )

                         ]
                    ) ]
              ) ]
        )

    let template : HTMLTemplate<Html2TempoState, Html2TempoAction, Html2TempoQuery> =
        DIV(
            [ cls "flex flex-col h-screen" ],
            [ bar
              DIV(
                  [ cls "flex h-full" ],
                  [ DIV(
                      [ cls "flex-auto h-full w-6/12"
                        MonacoEditorAttribute(
                            (fun _ ->
                                {| value = sample
                                   language = "html"
                                   wordWrap = "on" |}),
                            (function
                            | OnPaste -> Some HtmlPasted
                            | OnChange -> Some HtmlChanged),
                            (fun request editor ->
                                match request with
                                | GetHtml f -> f (editor.getValue ())
                                | SetGenerated _ -> ())
                        ) ]
                    )
                    DIV(
                        [ cls "flex-auto h-full w-6/12"
                          MonacoEditorAttribute(
                              (fun { FilterComments = filterComments } ->
                                  {| value = transformHtml filterComments sample
                                     language = "fsharp"
                                     wordWrap = "on" |}),
                              (fun e -> None),
                              (fun request editor ->
                                  match request with
                                  | GetHtml _ -> ()
                                  | SetGenerated content -> editor.setValue content)
                          ) ]
                    ) ]
              ) ]
        )

    let comp = Component(update, middleware, template)
