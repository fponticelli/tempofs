namespace Tempo.Html

open Browser.Types
open Browser.Dom
open Tempo.Html.Render
open Tempo.Html.Template
open Tempo.Update
open Tempo.View

type ProgramOptions<'S, 'A, 'Q> =
    { Template: Template<'S, 'A, 'Q>
      Container: Element
      Update: Update<'S, 'A>
      Middleware: Middleware<'S, 'A, 'Q> option
      State: 'S }

[<RequireQualifiedAccess>]
type Program =
    static member Make<'S, 'A, 'Q>
        ({ Template = template
           Update = update
           Middleware = middleware
           Container = container
           State = state' }: ProgramOptions<'S, 'A, 'Q>)
        =
        let mutable localState = state'
        let render = makeRender (template, true)

        let rec dispatch (action: 'A) =
            let newState = update localState action
            Option.iter (fun c -> c newState) view.Change

            Option.iter
                (fun m ->
                    m
                        { Previous = localState
                          Current = newState
                          Action = action
                          Dispatch = dispatch
                          Request = fun q -> Option.iter (fun r -> r q) view.Request })
                middleware

            localState <- newState

        and view : View<'S, 'Q> =
            render (localState, container, None, dispatch)

        { Change = view.Change
          Dispatch = dispatch
          Destroy = view.Destroy
          Request = view.Request }: ComponentView<'S, 'A, 'Q>

    static member MakeOnContentLoaded<'S, 'A, 'Q>
        (
            options: ProgramOptions<'S, 'A, 'Q>,
            callback: ComponentView<'S, 'A, 'Q> -> unit
        ) =
        let make () = Program.Make(options) |> callback

        window.addEventListener ("DOMContentLoaded", (fun _ -> make ()))
        |> ignore
