namespace Tempo.Demo.Utils

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Dom
open Browser.Types
open Tempo.Html

open Tempo.Html.Impl

open type Tempo.Html.DSL.HTML

module Monaco =
    type MonacoAction = | Unknwon
    type MonacoState = { Value: string }
    type MonacoQuery = unit

    type MonacoEditorOptions = {| value: string; language: string |}

    [<AbstractClass>]
    type MonacoEditorInstance =
        class
        end


    [<AbstractClass>]
    type MonacoEditorClass =
        // [<CompiledName("create")>]
        // abstract create : HTMLElement * MonacoEditorOptions -> MonacoEditorInstance
        [<Emit("$0.create($1, $2)")>]
        member this.create(element: HTMLElement, options: MonacoEditorOptions) : MonacoEditorInstance = jsNative

    type MonacoModule = { editor: MonacoEditorClass }

    let delay<'T> (f: unit -> 'T) : Promise<'T> =
        Constructors.Promise.Create
            (fun resolve reject ->
                (window.setTimeout ((fun _ -> resolve <| f ()), 0, [])
                 |> ignore))

    [<Import("*", from = "monaco-editor")>]
    let monaco : MonacoModule = jsNative

    let MonacoEditorAttribute<'S, 'A, 'Q>
        (mapToOptions: 'S -> MonacoEditorOptions)
        : HTMLTemplateAttribute<'S, 'A, 'Q> =
        lifecycleAttribute<'S, 'A, 'Q, _, Promise<MonacoEditorInstance>>
            (fun { Element = element; State = state } ->
                // TODO remove delay
                delay
                    (fun () ->

                        let editor =
                            monaco.editor.create (element, mapToOptions state)

                        editor))

            (fun { Payload = promiseEditor } -> (true, promiseEditor))

            (fun { Payload = promiseEditor } -> promiseEditor)
            (fun { Payload = promiseEditor } -> ())
            (fun q { Payload = promiseEditor } -> promiseEditor)
