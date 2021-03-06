namespace Tempo.Demo.Utils

open Tempo.Std
open Fable.Core
open Browser.Dom
open Browser.Types

module HtmlParser =
    let makeDOM content =
        let doc =
            document.implementation.createHTMLDocument ("")

        doc.body.innerHTML <- content
        doc

    let escape (value: string) = value.Replace("\"", "\\\"")

    let quote (value: string) = $"\"{value}\""

    let knownElements =
        [ "a"
          "article"
          "aside"
          "button"
          "dd"
          "div"
          "dl"
          "dt"
          "h1"
          "h2"
          "h3"
          "h4"
          "h5"
          "h6"
          "img"
          "input"
          "li"
          "main"
          "nav"
          "ol"
          "p"
          "path"
          "select"
          "span"
          "svg"
          "table"
          "tbody"
          "td"
          "textarea"
          "tfoot"
          "th"
          "thead"
          "tr"
          "ul" ]
        |> Set.ofList

    let knownAttributes =
        [ "class", (fun value -> $"cls {quote value}")
          "href", (fun value -> $"href {quote value}")
          "src", (fun value -> $"src {quote value}")
          "id", (fun value -> $"elId {quote value}") ]
        |> Map.ofList

    let makeAttribute (name: string) value =
        if name.StartsWith "aria-" then
            $"aria ({quote <| name.[5..]}, {quote value})"
        else
            let func = Map.tryFind name knownAttributes

            match func with
            | Some f -> f value
            | None -> $"Attr({quote name}, {quote value})"


    let rec renderDOMBody (filterComments: bool) (body: Element) : string option =
        let children =
            renderDOMElementChildren filterComments body

        if children.Length = 0 then
            None
        else if children.Length = 1 then
            Some <| children.Head
        else
            Some
            <| "Fragment [" + (String.concat "; " children) + "]"

    and renderDOMElementChildren filterComments (el: Element) : string list =
        let children =
            (JS.Constructors.Array.from (el.childNodes))
            |> Array.map (renderDOMNode filterComments)

        Array.toList children |> List.filterMap id

    and renderDOMNode filterComments (node: Node) : string option =
        if node.nodeType = 1.0 then
            renderDOMElement filterComments (node :?> Element)
        else if node.nodeType = 3.0 then
            renderDOMText (node :?> Text)
        else if node.nodeType = 8.0 then
            if filterComments then
                None
            else
                renderDOMComment (node :?> Comment)
        else
            console.error $"unknown node type: {node}"
            None

    and renderDOMElement filterComments (element: Element) : string option =
        let mutable args = []

        let name = element.tagName.ToLower()

        let func =
            if Set.contains name knownElements then
                element.tagName.ToUpper()
            else
                args <- [ quote name ]
                "El"

        let count = element.attributes.length

        let attributes =
            [ for index in 0 .. (count - 1) ->
                  let att = element.attributes.item (float index)
                  let name = att.name
                  let value = att.value
                  makeAttribute name value ]

        let children =
            renderDOMElementChildren filterComments element

        let items =
            String.concat "; " (attributes @ children)

        if args.Length > 0 then
            let args = String.concat ", " args
            Some $"{func}({args}, [{items}])"
        else
            Some $"{func} [{items}]"

    and renderDOMText (text: Text) : string option =
        let text = text.textContent.Trim()

        if text.Length = 0 then
            None
        else
            Some <| "Text " + (quote text)

    and renderDOMComment (comment: Comment) : string option =
        Some <| "(* " + (comment.textContent) + " *)"

    let transformHtml filterComments content =
        let dom = makeDOM content
        let maybe = renderDOMBody filterComments dom.body
        // dirty cleanup
        Option.map (fun (s: string) -> s.Replace(" *);", " *)")) maybe
        |> Option.defaultValue ""
