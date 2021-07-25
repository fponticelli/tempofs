namespace Tempo.Demo.Utils

open Fable.Core
open Browser.Types
open Browser.Dom
open Tempo.Html.Template

open type Tempo.Html.DSL

module Monaco =
    type MonacoEditorOptions =
        {| value: string
           language: string
           wordWrap: string |}

    type MonacoEvent =
        | OnPaste
        | OnChange

    [<AbstractClass>]
    type MonacoEditorInstance =
        [<Emit("$0.onDidPaste($1)")>]
        member this.onDidPaste(listener: unit -> unit) : unit = jsNative

        [<Emit("$0.onDidChangeModelContent($1)")>]
        member this.onDidChangeModelContent(listener: unit -> unit) : unit = jsNative

        [<Emit("$0.getValue()")>]
        member this.getValue() : string = jsNative

        [<Emit("$0.setValue($1)")>]
        member this.setValue(value: string) : unit = jsNative

        [<Emit("$0.dispose()")>]
        member this.dispose() : unit = jsNative

        [<Emit("$0.layout()")>]
        member this.layout() : unit = jsNative


    [<AbstractClass>]
    type MonacoEditorClass =
        [<Emit("$0.create($1, $2)")>]
        member this.create(element: HTMLElement, options: MonacoEditorOptions) : MonacoEditorInstance = jsNative

    type MonacoModule = { editor: MonacoEditorClass }

    [<Import("*", from = "monaco-editor")>]
    let monaco : MonacoModule = jsNative

    let MonacoEditor<'S, 'A, 'Q>
        (
            mapToOptions: 'S -> MonacoEditorOptions,
            mapAction: MonacoEvent -> 'A option,
            respond: 'Q -> MonacoEditorInstance -> unit
        ) : Template<'S, 'A, 'Q> =
        Lifecycle<'S, 'A, 'Q, MonacoEditorInstance>(
            onMount =
                (fun { Element = element
                       State = state
                       Dispatch = dispatch } ->

                    let editor =
                        monaco.editor.create (element :?> HTMLElement, mapToOptions state)

                    editor.onDidChangeModelContent
                        (fun () ->
                            match mapAction OnChange with
                            | Some a -> dispatch a
                            | None -> ())

                    editor.onDidPaste
                        (fun () ->
                            match mapAction OnPaste with
                            | Some a -> dispatch a
                            | None -> ())

                    // refresh layout sizing after dom is settled, could be avoided by inserting node BEFORE children
                    // window.setImmediate (fun () -> editor.layout ())
                    // |> ignore

                    editor),
            onRemove = (fun { Payload = editor } -> editor.dispose ()),
            respond = (fun editor q -> respond q editor)
        )
