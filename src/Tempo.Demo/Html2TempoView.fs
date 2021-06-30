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

    type Html2TempoState = unit

    type Html2TempoQuery =
        | GetHtml of (string -> unit)
        | SetGenerated of string

    let update (state: Html2TempoState) (action: Html2TempoAction) = state

    let mutable timerId : float option = None

    let middleware
        ({ Current = current
           Previous = prev
           Action = action
           Query = query }: MiddlewarePayload<_, _, _>)
        =
        let transformRoundtrip () =
            query (GetHtml(transformHtml >> SetGenerated >> query))

        match action with
        | HtmlPasted -> transformRoundtrip ()
        | HtmlChanged ->
            match timerId with
            | Some id -> window.clearTimeout id
            | None -> ()

            timerId <-
                Some
                <| window.setTimeout (
                    (fun _ ->
                        timerId <- None
                        transformRoundtrip ()),
                    200
                )

        console.log $"Html2TempoQuery Action: {action}, State: {current}, Previous {prev}"

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

    let template : HTMLTemplate<Html2TempoState, Html2TempoAction, Html2TempoQuery> =
        DIV(
            [ cls "flex h-full" ],
            [ DIV(
                [ cls "flex-auto h-full w-6/12"
                  MonacoEditorAttribute(
                      (fun () ->
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
                        (fun () ->
                            {| value = transformHtml sample
                               language = "fsharp"
                               wordWrap = "on" |}),
                        (fun e -> None),
                        (fun request editor ->
                            match request with
                            | GetHtml _ -> ()
                            | SetGenerated content ->
                                console.log content
                                editor.setValue content)
                    ) ]
              ) ]
        )

    let comp = Component(update, middleware, template)
