import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { record_type, unit_type, union_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { HTML_Attr_Z384F8060, HTML_El_Z7374416F } from "../Tempo.Html/HtmlDSL.fs.js";
import { empty, ofArray, singleton } from "./.fable/fable-library.3.1.10/List.js";
import { MonacoEditorAttribute } from "./MonacoEditor.fs.js";

export class Html2TempoAction extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Unknwon"];
    }
}

export function Html2TempoAction$reflection() {
    return union_type("Tempo.Demo.Html2Tempo.View.Html2TempoAction", [], Html2TempoAction, () => [[]]);
}

export class Html2TempoState extends Record {
    constructor(Value) {
        super();
        this.Value = Value;
    }
}

export function Html2TempoState$reflection() {
    return record_type("Tempo.Demo.Html2Tempo.View.Html2TempoState", [], Html2TempoState, () => [["Value", unit_type]]);
}

export function update(state, action) {
    return state;
}

export const template = HTML_El_Z7374416F("div", singleton(HTML_Attr_Z384F8060("class", "flex h-full")), ofArray([HTML_El_Z7374416F("div", ofArray([HTML_Attr_Z384F8060("class", "flex-auto h-full w-6/12"), MonacoEditorAttribute((s) => ({
    language: "html",
    value: "\u003ca\u003eLink\u003c/a\u003e",
}))]), empty()), HTML_El_Z7374416F("div", ofArray([HTML_Attr_Z384F8060("class", "flex-auto h-full w-6/12"), MonacoEditorAttribute((s_1) => ({
    language: "fsharp",
    value: "let a = 2",
}))]), empty())]));

