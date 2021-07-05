namespace Tempo.Html.UI

open Browser.Types
open Browser.Dom
open Tempo.Core
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

    type State<'S, 'A> =
        { Open: bool
          TriggerElement: Element
          OuterState: 'S
          OuterDispatch: Dispatch<'A> }

    type Coords = { X: float; Y: float }

    type Action<'S, 'A> =
        | Open
        | Close
        | Toggle
        | Reposition
        | SetOuterState of 'S
        | OuterActionTriggered of 'A

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
    static member MakeAttr<'S, 'A, 'Q>
        (
            panel: HTMLTemplate<'S, 'A, 'Q>,
            ?position: Popover.Position,
            ?triggeringEvents: string list,
            ?distance: float,
            ?container: Element,
            ?startOpen: 'S -> bool,
            ?closeOnAction: 'A -> bool
        ) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        let position =
            Option.defaultValue Popover.BottomLeft position

        let distance = Option.defaultValue 2.0 distance

        let triggeringEvents =
            Option.defaultValue [ "click" ] triggeringEvents

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

        let makeCloseOnClickOutsideImpl dispatch =
            let rec f (ev: Event) =
                remove ()

                match getProperty (ev, "key") with
                | None -> dispatch PopoverImpl.Action.Close
                | Some k when k = "Escape" || k = "Esc" -> dispatch PopoverImpl.Action.Close
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

        let makeCloseOnClickOutside isOpen dispatch =
            if isOpen then
                makeCloseOnClickOutsideImpl dispatch
            else
                ignore

        let makeReposition (el: Element) dispatch =
            let dispatchReposition _ =
                PopoverImpl.Action.Reposition |> dispatch

            let rObserver =
                JSe.ResizeObserver((fun _ _ -> dispatchReposition ()))

            let ancestors = collectElementAndAncestors el

            List.iter
                (fun (el: Element) ->
                    el.addEventListener ("scroll", dispatchReposition)
                    rObserver.observe (el))
                ancestors

            let remove () =
                rObserver.disconnect ()
                List.iter (fun (i: Element) -> i.removeEventListener ("scroll", dispatchReposition)) ancestors

            remove

        // TODO
        let mapped =
            MapSAQ(
                (fun ({ OuterState = state }: PopoverImpl.State<'S, 'A>) -> state),
                Some << PopoverImpl.OuterActionTriggered,
                (fun () -> None),
                panel
            )

        let applyPositioning (isOpen: bool) (trigger: Element) (panel: Element) =
            if isOpen then
                let { PopoverImpl.X = x; PopoverImpl.Y = y } = calcPosition trigger panel
                setStyle (panel, "left", $"{x}px")
                setStyle (panel, "top", $"{y}px")
            else
                ()

        let template =
            OneOf(
                (fun (s: PopoverImpl.State<'S, 'A>) ->
                    if s.Open then
                        Choice1Of2(s)
                    else
                        Choice2Of2(())),

                DIV(
                    [ Attr("style", "position: absolute")
                      Lifecycle(
                          (fun { State = ({ Open = isOpen
                                            TriggerElement = trigger }: PopoverImpl.State<'S, 'A>)
                                 Dispatch = dispatch
                                 Element = panel } ->
                              panel.addEventListener (
                                  "click",
                                  (fun event ->
                                      event.cancelBubble <- true
                                      event.preventDefault ())
                              )

                              applyPositioning isOpen trigger panel
                              makeCloseOnClickOutside isOpen dispatch),
                          afterChange =
                              (fun { State = { Open = isOpen
                                               TriggerElement = trigger }
                                     Dispatch = dispatch
                                     Element = panel
                                     Payload = manageClickDoc } ->
                                  applyPositioning isOpen trigger panel
                                  manageClickDoc ()
                                  makeCloseOnClickOutside isOpen dispatch),
                          beforeDestroy = (fun { Payload = manageClickDoc } -> manageClickDoc ())
                      ) ],
                    [ mapped ]
                ),
                Text ""
            )

        // Button/Control lifecycle
        Lifecycle<'S, 'A, 'Q, Element, _>(
            (fun { State = state
                   Dispatch = dispatch
                   Element = el } ->
                let render = MakeProgram(template, container)

                let update (state: PopoverImpl.State<'S, 'A>) (action: PopoverImpl.Action<'S, 'A>) =
                    match action with
                    | PopoverImpl.Toggle -> { state with Open = not state.Open }
                    | PopoverImpl.Open -> { state with Open = true }
                    | PopoverImpl.Close -> { state with Open = false }
                    | PopoverImpl.SetOuterState outs -> { state with OuterState = outs }
                    | PopoverImpl.OuterActionTriggered act ->
                        state.OuterDispatch act

                        if closeOnAction act then
                            { state with Open = false }
                        else
                            state
                    | PopoverImpl.Reposition -> state // triggers refresh of location

                let view =
                    render
                        update
                        ignore
                        { Open = startOpen state
                          TriggerElement = el
                          OuterState = state
                          OuterDispatch = dispatch }

                let dispatchOpen _ = view.Dispatch(PopoverImpl.Toggle)
                List.iter (fun te -> el.addEventListener (te, dispatchOpen)) triggeringEvents
                let removeReposition = makeReposition el view.Dispatch

                let destroy () =
                    removeReposition ()
                    List.iter (fun te -> el.removeEventListener (te, dispatchOpen)) triggeringEvents
                    view.Destroy()

                { view with Destroy = destroy }),
            afterChange =
                (fun { State = state; Payload = view } ->
                    view.Dispatch(PopoverImpl.SetOuterState state)
                    view),
            beforeDestroy = (fun { Payload = view } -> view.Destroy())
        )
