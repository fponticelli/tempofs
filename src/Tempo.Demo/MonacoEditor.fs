namespace Tempo.Demo.Utils

open Fable.Core
open Browser.Types
open Tempo.Html
open Tempo.Html.Impl

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


    [<AbstractClass>]
    type MonacoEditorClass =
        // [<CompiledName("create")>]
        // abstract create : HTMLElement * MonacoEditorOptions -> MonacoEditorInstance
        [<Emit("$0.create($1, $2)")>]
        member this.create(element: HTMLElement, options: MonacoEditorOptions) : MonacoEditorInstance = jsNative

    type MonacoModule = { editor: MonacoEditorClass }

    [<Import("*", from = "monaco-editor")>]
    let monaco : MonacoModule = jsNative

    let MonacoEditorAttribute<'S, 'A, 'Q>
        (
            mapToOptions: 'S -> MonacoEditorOptions,
            mapAction: MonacoEvent -> 'A option,
            respond: 'Q -> MonacoEditorInstance -> unit
        ) : HTMLTemplateAttribute<'S, 'A, 'Q> =
        lifecycleAttribute<'S, 'A, 'Q, _, MonacoEditorInstance>
            (fun { Element = element
                   State = state
                   Dispatch = dispatch } ->
                let editor =
                    monaco.editor.create (element, mapToOptions state)

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

                editor)

            (fun { Payload = editor } -> (true, editor))

            (fun { Payload = editor } -> editor)
            (fun { Payload = editor } -> editor.dispose ())
            (fun q { Payload = editor } ->
                respond q editor
                editor)
