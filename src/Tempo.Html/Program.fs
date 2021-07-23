namespace Tempo.Html

type Program =
    static member make() = 1
// static member private MakeRender<'S, 'A, 'Q>() =
//     MakeRender<HTMLTemplateNode<'S, 'A, 'Q>, 'S, 'A, 'Q>(makeHTMLNodeRender, createGroupNode)

// static member MakeProgram<'S, 'A, 'Q>(template: HTMLTemplate<'S, 'A, 'Q>, el: Element) =
//     let renderInstance = DSL.MakeRender()

//     let f = renderInstance.Make template
//     let parent = HTMLElementImpl(el)

//     let render = f parent

//     fun update middleware state ->
//         let mutable localState = state

//         let rec dispatch action =
//             let newState = update localState action
//             view.Change newState

//             middleware
//                 { Previous = localState
//                   Current = newState
//                   Action = action
//                   Dispatch = dispatch
//                   Query = view.Query }

//             localState <- newState

//         and view = render localState dispatch

//         { Impl = view.Impl
//           Change = view.Change
//           Dispatch = dispatch
//           Destroy = view.Destroy
//           Query = view.Query }: ComponentView<'S, 'A, 'Q>

// static member MakeProgramOnContentLoaded<'S, 'A, 'Q>
//     (
//         template: HTMLTemplate<'S, 'A, 'Q>,
//         parent: Element,
//         f: (ComponentView<'S, 'A, 'Q> -> unit)
//     ) =
//     fun update middleware state ->
//         window.addEventListener (
//             "DOMContentLoaded",
//             fun _ ->
//                 let render = DSL.MakeProgram(template, parent)
//                 render update middleware state |> f
//         )
//         |> ignore

// static member MakeProgramOnContentLoaded<'S, 'A, 'Q>
//     (
//         template: HTMLTemplate<'S, 'A, 'Q>,
//         selector: string,
//         f: (ComponentView<'S, 'A, 'Q> -> unit)
//     ) =
//     DSL.MakeProgramOnContentLoaded<'S, 'A, 'Q>(template, (document.querySelector selector), f)
