namespace Tempo.Html.UI

open Browser.Types
open Browser.Dom
open Tempo.Html
open Tempo.Html.Tools

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
    type Position = Popover.Position
    type PopoverState = { Open: bool; Rect: ClientRect }
    type Coords = { X: float; Y: float }

    type PopoverAction =
        | Open
        | Close
        | Reposition of ClientRect

    type PopoverPayload =
        { ManageClickDoc: unit -> unit
          RemoveRepositionHandlers: unit -> unit }

    let makeCalculatePosition (position: Position) (distance: float) =
        match position with
        | Position.Centered ->
            (fun (reference: ClientRect) (target: ClientRect) ->
                let x =
                    reference.left
                    + (reference.width - target.width) / 2.0

                let y =
                    reference.top
                    + (reference.height - target.height) / 2.0

                { X = x; Y = y })
        | Position.Top ->
            (fun (reference: ClientRect) (target: ClientRect) ->
                let x =
                    reference.left
                    + (reference.width - target.width) / 2.0

                let y = reference.top - target.height - distance
                { X = x; Y = y })
        | Position.Bottom ->
            (fun (reference: ClientRect) (target: ClientRect) ->
                let x =
                    reference.left
                    + (reference.width - target.width) / 2.0

                let y = reference.bottom + distance
                { X = x; Y = y })
        | Position.Left ->
            (fun (reference: ClientRect) (target: ClientRect) ->
                let x = reference.left - target.width - distance

                let y =
                    reference.top
                    + (reference.height - target.height) / 2.0

                { X = x; Y = y })
        | Position.Right ->
            (fun (reference: ClientRect) (target: ClientRect) ->
                let x = reference.right + distance

                let y =
                    reference.top
                    + (reference.height - target.height) / 2.0

                { X = x; Y = y })
        | Position.TopLeft ->
            (fun (reference: ClientRect) (target: ClientRect) ->
                let x = reference.left
                let y = reference.top - target.height - distance
                { X = x; Y = y })
        | Position.TopRight ->
            (fun (reference: ClientRect) (target: ClientRect) ->
                let x = reference.right - target.width
                let y = reference.top - target.height - distance
                { X = x; Y = y })
        | Position.BottomLeft ->
            (fun (reference: ClientRect) (target: ClientRect) ->
                let x = reference.left
                let y = reference.bottom + distance
                { X = x; Y = y })
        | Position.BottomRight ->
            (fun (reference: ClientRect) (target: ClientRect) ->
                let x = reference.right - target.width
                let y = reference.bottom + distance
                { X = x; Y = y })
        | Position.LeftTop ->
            (fun (reference: ClientRect) (target: ClientRect) ->
                let x = reference.left - target.width - distance
                let y = reference.top
                { X = x; Y = y })
        | Position.LeftBottom ->
            (fun (reference: ClientRect) (target: ClientRect) ->
                let x = reference.left - target.width - distance
                let y = reference.bottom - target.height
                { X = x; Y = y })
        | Position.RightTop ->
            (fun (reference: ClientRect) (target: ClientRect) ->
                let x = reference.right + distance
                let y = reference.top
                { X = x; Y = y })
        | Position.RightBottom ->
            (fun (reference: ClientRect) (target: ClientRect) ->
                let x = reference.right + distance
                let y = reference.bottom - target.height
                { X = x; Y = y })

type Popover =
    static member Make<'S, 'A, 'Q>
        (
            position: Popover.Position,
            trigger: HTMLTemplate<'S, 'A, 'Q>,
            panel: HTMLTemplate<'S, 'A, 'Q>,
            buttonClass: string
        ) : HTMLTemplate<'S, 'A, 'Q> =
        Popover.Make(position, 2.0, trigger, panel, buttonClass)

    static member Make<'S, 'A, 'Q>
        (
            position: Popover.Position,
            distance: float,
            trigger: HTMLTemplate<'S, 'A, 'Q>,
            panel: HTMLTemplate<'S, 'A, 'Q>,
            buttonClass: string
        ) : HTMLTemplate<'S, 'A, 'Q> =
        let (hold, release) = MakeCaptureSA()

        let calcPosition =
            PopoverImpl.makeCalculatePosition position distance

        let calcPosition ref (target: HTMLElement) =
            calcPosition ref (target.getBoundingClientRect ())

        let makeCloseOnClickOutsideImpl (el: EventTarget) dispatch =
            let rec f (ev: Event) =
                remove ()

                match getProperty (ev, "key") with
                | None -> dispatch PopoverImpl.PopoverAction.Close
                | Some k when k = "Escape" || k = "Esc" -> dispatch PopoverImpl.PopoverAction.Close
                | Some _ -> ()

            and remove () =
                document.removeEventListener ("keyup", f)
                document.removeEventListener ("click", f)

            window.setTimeout (
                (fun () ->
                    document.addEventListener ("keyup", f)
                    document.addEventListener ("click", f)),
                0
            )
            |> ignore

            remove

        let makeCloseOnClickOutside isOpen el dispatch =
            if isOpen then
                makeCloseOnClickOutsideImpl el dispatch
            else
                ignore

        let makeReposition (el: HTMLElement) dispatch =
            let update _ =
                let rect = el.getBoundingClientRect ()

                rect
                |> PopoverImpl.PopoverAction.Reposition
                |> dispatch

            let rObserver =
                JSe.ResizeObserver((fun _ _ -> update ()))

            let ancestors = collectElementAndAncestors el

            List.iter
                (fun (el: Element) ->
                    el.addEventListener ("scroll", update)
                    rObserver.observe (el))
                ancestors

            let remove () =
                rObserver.disconnect ()
                List.iter (fun (i: Element) -> i.removeEventListener ("scroll", update)) ancestors

            remove

        hold (
            MapSA<'S, PopoverImpl.PopoverState, 'A, PopoverImpl.PopoverAction, 'Q>(
                (fun _ ->
                    { Open = false
                      Rect = document.body.getBoundingClientRect () }),
                (fun (_: PopoverImpl.PopoverAction) -> None),
                Component<PopoverImpl.PopoverState, PopoverImpl.PopoverAction, _>(
                    (fun (s: PopoverImpl.PopoverState) (a: PopoverImpl.PopoverAction) ->
                        match a with
                        | PopoverImpl.PopoverAction.Open -> { s with Open = true }
                        | PopoverImpl.PopoverAction.Close -> { s with Open = false }
                        | PopoverImpl.PopoverAction.Reposition rect -> { s with Rect = rect }),
                    Fragment [ BUTTON(
                                   [ Lifecycle(
                                       (fun { State = { PopoverImpl.PopoverState.Open = isOpen }
                                              Dispatch = dispatch
                                              Element = el } ->

                                           let manageClickDoc =
                                               makeCloseOnClickOutside isOpen el dispatch // after render

                                           let removeRepositionHandlers = makeReposition el dispatch

                                           { PopoverImpl.PopoverPayload.ManageClickDoc = manageClickDoc
                                             PopoverImpl.PopoverPayload.RemoveRepositionHandlers =
                                                 removeRepositionHandlers }),
                                       (fun { State = { Open = isOpen }
                                              Payload = payload
                                              Dispatch = dispatch
                                              Element = el } ->
                                           payload.ManageClickDoc()

                                           let manageClickDoc =
                                               makeCloseOnClickOutside isOpen el dispatch

                                           { payload with
                                                 ManageClickDoc = manageClickDoc }), // after change
                                       (fun { Payload = payload } ->
                                           payload.ManageClickDoc()
                                           payload.RemoveRepositionHandlers()) // before destroy
                                     )
                                     On<PopoverImpl.PopoverState, PopoverImpl.PopoverAction, _, HTMLElement, Event>(
                                         "click",
                                         (fun { State = { Open = isOpen } } ->
                                             if isOpen then
                                                 PopoverImpl.PopoverAction.Close
                                             else
                                                 PopoverImpl.PopoverAction.Open)
                                     )
                                     Attr("type", "button")
                                     cls buttonClass
                                     aria ("expanded", "true")
                                     aria ("haspopup", "true") ],
                                   [ release ((fun s _ -> s), Choice1Of3 >> Some, trigger) ]
                               )
                               Portal(
                                   "body",
                                   DIV(
                                       [ DispatchOn(
                                           "click",
                                           (fun { Event = event } _ ->
                                               event.cancelBubble <- true
                                               event.preventDefault ())
                                         )
                                         Attr(
                                             "style",
                                             (fun { PopoverImpl.PopoverState.Open = isOpen } ->
                                                 if isOpen then
                                                     "position: absolute"
                                                 else
                                                     "display: none")
                                         )
                                         Lifecycle(
                                             (fun { State = { Rect = reference }
                                                    Element = target } ->
                                                 let { PopoverImpl.Coords.X = x
                                                       PopoverImpl.Coords.Y = y } =
                                                     calcPosition reference target

                                                 setStyle (target, "top", $"{y}px")
                                                 setStyle (target, "left", $"{x}px")),
                                             (fun { State = { Rect = reference }
                                                    Element = target } ->
                                                 let { PopoverImpl.Coords.X = x
                                                       PopoverImpl.Coords.Y = y } =
                                                     calcPosition reference target

                                                 setStyle (target, "top", $"{y}px")
                                                 setStyle (target, "left", $"{x}px")),
                                             ignore
                                         ) ],
                                       [ release ((fun s _ -> s), Choice1Of3 >> Some, panel) ]
                                   )
                               ) ]
                )
            )
        )
