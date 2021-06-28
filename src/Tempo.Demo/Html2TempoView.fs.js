import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { record_type, unit_type, union_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { a } from "./MonacoEditor.fs.js";
import { HTML_Text_Z721C83C5, HTML_El_Z7374416F } from "../Tempo.Html/HtmlDSL.fs.js";
import { ofArray, singleton, empty } from "./.fable/fable-library.3.1.10/List.js";

export class Action extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Unknwon"];
    }
}

export function Action$reflection() {
    return union_type("Tempo.Demo.Html2Tempo.View.Action", [], Action, () => [[]]);
}

export class State extends Record {
    constructor(Value) {
        super();
        this.Value = Value;
    }
}

export function State$reflection() {
    return record_type("Tempo.Demo.Html2Tempo.View.State", [], State, () => [["Value", unit_type]]);
}

export function update(state, action) {
    return state;
}

export function init() {
    void a();
    return new State(void 0);
}

export const template = HTML_El_Z7374416F("div", empty(), ofArray([HTML_El_Z7374416F("div", empty(), singleton(HTML_Text_Z721C83C5("Panel 1"))), HTML_El_Z7374416F("div", empty(), singleton(HTML_Text_Z721C83C5("Panel 2")))]));

