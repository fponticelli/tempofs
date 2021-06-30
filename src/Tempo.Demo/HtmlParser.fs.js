import { join, interpolate, toText, replace } from "./.fable/fable-library.3.1.10/String.js";
import { tryFind, ofList } from "./.fable/fable-library.3.1.10/Map.js";
import { append, empty, ofArray, head, length, singleton } from "./.fable/fable-library.3.1.10/List.js";
import { map } from "./.fable/fable-library.3.1.10/Array.js";
import { filterMap } from "../Tempo.Core/ListExtra.fs.js";
import { map as map_2, defaultArg, some } from "./.fable/fable-library.3.1.10/Option.js";
import { map as map_1, delay, toList } from "./.fable/fable-library.3.1.10/Seq.js";
import { rangeDouble } from "./.fable/fable-library.3.1.10/Range.js";

export function makeDOM(content) {
    const doc = document.implementation.createHTMLDocument("");
    doc.body.innerHTML = content;
    return doc;
}

export function escape(value) {
    return replace(value, "\"", "\\\"");
}

export function quote(value) {
    return toText(interpolate("\"%P()\"", [value]));
}

export const knownElements = ofList(singleton(["div", "DIV"]));

export const knownAttributes = ofList(singleton(["class", (value) => toText(interpolate("cls %P()", [quote(value)]))]));

export function renderDOMBody(body) {
    const children = renderDOMElementChildren(body);
    if (length(children) === 0) {
        return void 0;
    }
    else if (length(children) === 1) {
        return head(children);
    }
    else {
        return ("Fragment [" + join("; ", children)) + "]";
    }
}

export function renderDOMElementChildren(el) {
    const children = map((node) => renderDOMNode(node), Array.from(el.childNodes));
    return filterMap((x) => x, ofArray(children));
}

export function renderDOMNode(node) {
    if (node.nodeType === 1) {
        return renderDOMElement(node);
    }
    else if (node.nodeType === 3) {
        return renderDOMText(node);
    }
    else if (node.nodeType === 8) {
        return renderDOMComment(node);
    }
    else {
        console.error(some(toText(interpolate("unknown node type: %P()", [node]))));
        return void 0;
    }
}

export function renderDOMElement(element) {
    let args = empty();
    const name = element.tagName.toLocaleLowerCase();
    const func = tryFind(name, knownElements);
    let func_1;
    if (func == null) {
        args = singleton(quote(name));
        func_1 = "El";
    }
    else {
        const v = func;
        func_1 = v;
    }
    const count = element.attributes.length | 0;
    const attributes = toList(delay(() => map_1((index) => {
        const att = element.attributes.item(index);
        const name_1 = att.name;
        const value = att.value;
        const func_2 = tryFind(name_1, knownAttributes);
        if (func_2 == null) {
            return toText(interpolate("Attr(%P(), %P())", [quote(name_1), quote(value)]));
        }
        else {
            const f = func_2;
            return f(value);
        }
    }, rangeDouble(0, 1, count - 1))));
    if (length(attributes) > 0) {
        args = append(args, singleton(("[" + join("; ", attributes)) + "]"));
    }
    const children = renderDOMElementChildren(element);
    if (length(children) > 0) {
        args = append(args, singleton(("[" + join("; ", children)) + "]"));
    }
    if (length(args) === 1) {
        return toText(interpolate("%P()(%P())", [func_1, head(args)]));
    }
    else {
        const args_1 = join(", ", args);
        return toText(interpolate("%P()(%P())", [func_1, args_1]));
    }
}

export function renderDOMText(text) {
    const text_1 = text.textContent.trim();
    if (text_1.length === 0) {
        return void 0;
    }
    else {
        return ("Text(" + quote(text_1)) + ")";
    }
}

export function renderDOMComment(comment) {
    return ("(* " + comment.textContent) + " *)";
}

export function transformHtml(content) {
    const dom = makeDOM(content);
    const maybe = renderDOMBody(dom.body);
    return defaultArg(map_2((s) => replace(s, " *);", " *)"), maybe), "");
}

