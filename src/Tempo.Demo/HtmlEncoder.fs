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
        [ "div", "DIV"
          "button", "BUTTON"
          "img", "IMG"
          "span", "SPAN"
          "svg", "SVG"
          "path", "PATH" ]
        |> Map.ofList

    let knownAttributes =
        [ "class", (fun value -> $"cls {quote value}") ]
        |> Map.ofList

    let makeAttribute (name: string) value =
        if name.StartsWith "aria-" then
            $"aria ({quote <| name.Substring 5}, {quote value})"
        else
            let func = Map.tryFind name knownAttributes

            match func with
            | Some f -> f value
            | None -> $"Attr({quote name}, {quote value})"


    let rec renderDOMBody (body: Element) : string option =
        let children = renderDOMElementChildren body

        if children.Length = 0 then
            None
        else if children.Length = 1 then
            Some <| children.Head
        else
            Some
            <| "Fragment [" + (String.concat "; " children) + "]"

    and renderDOMElementChildren (el: Element) : string list =
        let children =
            (JS.Constructors.Array.from (el.childNodes))
            |> Array.map renderDOMNode

        Array.toList children |> List.filterMap id

    and renderDOMNode (node: Node) : string option =
        if node.nodeType = 1.0 then
            renderDOMElement (node :?> Element)
        else if node.nodeType = 3.0 then
            renderDOMText (node :?> Text)
        else if node.nodeType = 8.0 then
            renderDOMComment (node :?> Comment)
        else
            console.error $"unknown node type: {node}"
            None

    and renderDOMElement (element: Element) : string option =
        let mutable args = []

        let name = element.tagName.ToLower()

        let func = Map.tryFind name knownElements

        let func =
            match func with
            | Some v -> v
            | None ->
                args <- [ quote name ]
                "El"

        let count = element.attributes.length

        let attributes =
            [ for index in 0 .. (count - 1) ->
                  let att = element.attributes.item (float index)
                  let name = att.name
                  let value = att.value
                  makeAttribute name value ]

        if attributes.Length > 0 then
            args <-
                args
                @ [ "[" + (String.concat "; " attributes) + "]" ]

        let children = renderDOMElementChildren element

        if children.Length > 0 then
            args <-
                args
                @ [ "[" + (String.concat "; " children) + "]" ]

        if args.Length = 1 then
            Some $"{func}({args.Head})"
        else
            let args = String.concat ", " args
            Some $"{func}({args})"

    and renderDOMText (text: Text) : string option =
        let text = text.textContent.Trim()

        if text.Length = 0 then
            None
        else
            Some <| "Text(" + (quote text) + ")"

    and renderDOMComment (comment: Comment) : string option =
        Some <| "(* " + (comment.textContent) + " *)"

    let transformHtml content =
        let dom = makeDOM content
        let maybe = renderDOMBody dom.body
        // dirty cleanup
        Option.map (fun (s: string) -> s.Replace(" *);", " *)")) maybe
        |> Option.defaultValue ""
