import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { record_type, unit_type, union_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { HTML_Text_Z721C83C5, HTML_Attr_Z384F8060, HTML_El_Z7374416F } from "../Tempo.Html/HtmlDSL.fs.js";
import { ofArray, singleton } from "./.fable/fable-library.3.1.10/List.js";
import { map } from "../Tempo.Core/Core.fs.js";
import { MonacoEditor, MonacoState } from "./MonacoEditor.fs.js";

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

export const template = HTML_El_Z7374416F("div", singleton(HTML_Attr_Z384F8060("class", "flex h-screen")), ofArray([HTML_El_Z7374416F("div", singleton(HTML_Attr_Z384F8060("class", "flex-auto")), singleton(HTML_Text_Z721C83C5("Panel 1"))), HTML_El_Z7374416F("div", singleton(HTML_Attr_Z384F8060("class", "flex-auto")), singleton(map((x_3) => x_3, (s) => (new MonacoState("hello")), (arg0) => arg0, () => {
}, map((x) => x, (x_1) => x_1, (_arg1) => (void 0), () => {
}, MonacoEditor()))))]));

