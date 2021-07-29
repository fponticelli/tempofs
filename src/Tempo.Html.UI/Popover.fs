namespace Tempo.Html.UI

open Browser.Types
open Browser.Dom
open Tempo.Browser
open Tempo.View
open Tempo.Html.Template
open Tempo.Html
open Tempo.Update

open type Tempo.Html.DSL

module Popover =
    type Position =
        | Centered
        | Top
        | Bottom
        | Left
        | Right
        | TopLeft
        | TopRight
        | BottomLeft
        | BottomRight
        | LeftTop
        | LeftBottom
        | RightTop
        | RightBottom

module private PopoverImpl =
    type Payload<'S, 'A, 'Q> =
        { mutable State: 'S
          mutable MaybeView: ComponentView<'S, 'A, 'Q> option }

    type Position = Popover.Position
    type Coords = { X: float; Y: float }

    let makeCalculatePosition (position: Position) (distance: float) =
        match position with
        | Position.Centered ->
            (fun (ref: ClientRect) (target: ClientRect) ->
                let x =
                    ref.left + (ref.width - target.width) / 2.0

                let y =
                    ref.top + (ref.height - target.height) / 2.0

                { X = x; Y = y })
        | Position.Top ->
            (fun (ref: ClientRect) (target: ClientRect) ->
                let x =
                    ref.left + (ref.width - target.width) / 2.0

                let y = ref.top - target.height - distance
                { X = x; Y = y })
        | Position.Bottom ->
            (fun (ref: ClientRect) (target: ClientRect) ->
                let x =
                    ref.left + (ref.width - target.width) / 2.0

                let y = ref.bottom + distance
                { X = x; Y = y })
        | Position.Left ->
            (fun (ref: ClientRect) (target: ClientRect) ->
                let x = ref.left - target.width - distance

                let y =
                    ref.top + (ref.height - target.height) / 2.0

                { X = x; Y = y })
        | Position.Right ->
            (fun (ref: ClientRect) (target: ClientRect) ->
                let x = ref.right + distance

                let y =
                    ref.top + (ref.height - target.height) / 2.0

                { X = x; Y = y })
        | Position.TopLeft ->
            (fun (ref: ClientRect) (target: ClientRect) ->
                let x = ref.left
                let y = ref.top - target.height - distance
                { X = x; Y = y })
        | Position.TopRight ->
            (fun (ref: ClientRect) (target: ClientRect) ->
                let x = ref.right - target.width
                let y = ref.top - target.height - distance
                { X = x; Y = y })
        | Position.BottomLeft ->
            (fun (ref: ClientRect) (target: ClientRect) ->
                let x = ref.left
                let y = ref.bottom + distance
                { X = x; Y = y })
        | Position.BottomRight ->
            (fun (ref: ClientRect) (target: ClientRect) ->
                let x = ref.right - target.width
                let y = ref.bottom + distance
                { X = x; Y = y })
        | Position.LeftTop ->
            (fun (ref: ClientRect) (target: ClientRect) ->
                let x = ref.left - target.width - distance
                let y = ref.top
                { X = x; Y = y })
        | Position.LeftBottom ->
            (fun (ref: ClientRect) (target: ClientRect) ->
                let x = ref.left - target.width - distance
                let y = ref.bottom - target.height
                { X = x; Y = y })
        | Position.RightTop ->
            (fun (ref: ClientRect) (target: ClientRect) ->
                let x = ref.right + distance
                let y = ref.top
                { X = x; Y = y })
        | Position.RightBottom ->
            (fun (ref: ClientRect) (target: ClientRect) ->
                let x = ref.right + distance
                let y = ref.bottom - target.height
                { X = x; Y = y })

type Popover =
    static member Popover<'S, 'A, 'Q>
        (
            panel: Template<'S, 'A, 'Q>,
            ?position: Popover.Position,
            ?triggeringEvents: string list,
            ?closingEvents: string list,
            ?distance: float,
            ?container: Element,
            ?startOpen: 'S -> bool,
            ?closeOnAction: 'A -> bool
        ) : Template<'S, 'A, 'Q> =
        let position =
            Option.defaultValue Popover.BottomLeft position

        let distance = Option.defaultValue 2.0 distance

        let triggeringEvents =
            Option.defaultValue [ "click" ] triggeringEvents

        let closingEvents =
            Option.defaultValue [ "mousedown"; "keyup" ] closingEvents

        let container =
            Option.defaultValue (document.body :> Element) container

        let startOpen =
            Option.defaultValue (fun _ -> false) startOpen

        let closeOnAction =
            Option.defaultValue (fun _ -> true) closeOnAction

        let calcPosition =
            PopoverImpl.makeCalculatePosition position distance

        let calcPosition (ref: Element) (target: Element) =
            calcPosition (ref.getBoundingClientRect ()) (target.getBoundingClientRect ())

        let applyPositioning (trigger: Element) (panel: Element) =
            let { PopoverImpl.X = x; PopoverImpl.Y = y } = calcPosition trigger panel
            setStyle (panel, "left", $"{x}px")
            setStyle (panel, "top", $"{y}px")

        let wireReposition (button: Element) (container: Element) =
            let apply () = applyPositioning button container

            let rObserver =
                JSe.ResizeObserver((fun _ _ -> apply ()))

            let ancestors = collectElementAndAncestors button

            List.iter
                (fun (el: Element) ->
                    el.addEventListener ("scroll", (fun _ -> apply ()))
                    rObserver.observe (el))
                ancestors

            let remove () =
                rObserver.disconnect ()
                List.iter (fun (i: Element) -> i.removeEventListener ("scroll", (fun _ -> apply ()))) ancestors

            remove

        let makePanelView (payload: PopoverImpl.Payload<'S, 'A, 'Q>) (dispatch: Dispatch<'A>) (parent: Element) =
            let update (s: 'S) (_: 'A) = s
            let middleware ({ Action = action }: MiddlewarePayload<'S, 'A, 'Q>) = dispatch action

            let container = document.createElement ("div")
            setStyle (container, "position", "absolute")
            (parent.appendChild container) |> ignore

            let view =
                Program.Make(
                    { Template = panel
                      Container = container
                      State = payload.State
                      Update = update
                      Middleware = Some middleware }
                )

            (container,
             { view with
                   Destroy =
                       Some
                           (fun () ->
                               remove container
                               Option.iter (fun d -> d ()) view.Destroy) })

        // Button/Control lifecycle
        DSL.Lifecycle<'S, 'A, 'Q, PopoverImpl.Payload<'S, 'A, 'Q>>(
            (fun { State = state
                   Dispatch = dispatch
                   Element = button } ->

                let payload: PopoverImpl.Payload<'S, 'A, 'Q> = { State = state; MaybeView = None }

                let rec openPopover (ev: Event) =
                    (document.activeElement :?> HTMLElement).blur ()
                    ev.cancelBubble <- true

                    let (containerEl, view) = makePanelView payload dispatch container
                    applyPositioning button containerEl
                    let removeWiring = wireReposition button containerEl

                    let rec closePopover (ev: Event) =
                        if not (targetHasSpecifiedAncestor ev.target containerEl) then
                            removeWiring ()
                            List.iter (fun te -> button.addEventListener (te, openPopover)) triggeringEvents
                            List.iter (fun ce -> document.removeEventListener (ce, closePopover)) closingEvents
                            payload.MaybeView <- None
                            (button :?> HTMLElement).focus ()
                            Option.iter (fun d -> d ()) view.Destroy

                    List.iter (fun te -> button.removeEventListener (te, openPopover)) triggeringEvents
                    List.iter (fun ce -> document.addEventListener (ce, closePopover)) closingEvents
                    button.addEventListener ("click", closePopover)

                    payload.MaybeView <- Some view

                    Array.tryItem 0 (getFocusable (containerEl))
                    |> Option.iter (fun (el) -> (el :?> HTMLElement).focus ())

                List.iter (fun te -> button.addEventListener (te, openPopover)) triggeringEvents
                payload),
            (fun { State = state; Payload = p } ->
                p.State <- state

                Option.bind (fun (view: ComponentView<_, _, _>) -> view.Change) p.MaybeView
                |> Option.iter (fun change -> change state)

                p),
            (fun { Payload = p } ->
                Option.bind (fun view -> view.Destroy) p.MaybeView
                |> Option.iter (fun destroy -> destroy ())),
            (fun _ _ -> ())
        )
