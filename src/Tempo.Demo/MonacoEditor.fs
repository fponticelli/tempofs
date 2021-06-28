namespace Tempo.Demo.Utils

open Fable.Core
open Fable.Core.JsInterop
open Browser.Dom
open Browser.Types
open Tempo.Html

open type Tempo.Html.DSL.HTML

module Monaco =
    type MonacoAction = | Unknwon
    type MonacoState = { Value: string }

    type MonacoEditorOptions = { value: string; language: string }

    type MonacoEditorInstance = { update: (string -> unit) }

    type MonacoEditorClass =
        { create: ((HTMLElement * MonacoEditorOptions) -> MonacoEditorInstance) }

    type MonacoModule = { editor: MonacoEditorClass }

    [<Import("*", from = "monaco-editor")>]
    let monaco : MonacoModule = jsNative

    let MonacoEditor () : HTMLTemplate<MonacoState, MonacoAction, _> =
        DIV(
            [ Lifecycle(
                  (fun { Element = element; State = state } ->
                      let editor =
                          monaco.editor.create (
                              element,
                              { value = state.Value
                                language = "fsharp" }
                          )

                      editor), // afterRender: HTMLLifecycleInitialPayload<'S, 'EL> -> 'P,
                  (fun { Payload = editor } -> (true, editor)), // beforeChange: HTMLLifecyclePayload<'S, 'EL, 'P> -> (bool * 'P),
                  (fun { Payload = editor } -> editor), // afterChange: HTMLLifecyclePayload<'S, 'EL, 'P> -> 'P,
                  ignore, // beforeDestroy: HTMLLifecyclePayload<'S, 'EL, 'P> -> unit,
                  (fun q { Payload = editor } -> editor) // respond: 'Q -> HTMLLifecyclePayload<'S, 'EL, 'P> -> 'P
              ) ]
        )
