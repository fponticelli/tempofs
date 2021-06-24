import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { record_type, union_type, int32_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { Template$4 } from "./Core.fs.js";
import { HTML_MakeProgram_Z7A39E9B2, HTML_Text, HTML_OneOf_Z2AFE4804, HTML_Text_77A7E8C8, HTML_Attr_Z384F8060, HTML_MapState_Z5D9F8A77, HTML_Text_Z721C83C5, HTML_on_4A53169E, HTML_Attr_3DF4EB53, HTML_El_Z67FF9A04 } from "./HtmlDSL.fs.js";
import { interpolate, toText } from "./.fable/fable-library.3.1.10/String.js";
import { int32ToString, comparePrimitives, max } from "./.fable/fable-library.3.1.10/Util.js";
import { empty, ofArray, singleton } from "./.fable/fable-library.3.1.10/List.js";
import { FSharpChoice$2 } from "./.fable/fable-library.3.1.10/Choice.js";
import { some } from "./.fable/fable-library.3.1.10/Option.js";

export class Action extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Reset", "Increment"];
    }
}

export function Action$reflection() {
    return union_type("App.Action", [], Action, () => [[], [["Item", int32_type]]]);
}

export class State extends Record {
    constructor(Counter) {
        super();
        this.Counter = (Counter | 0);
    }
}

export function State$reflection() {
    return record_type("App.State", [], State, () => [["Counter", int32_type]]);
}

export function makeState(v) {
    return new State(v);
}

export const state = new State(0);

export const template = new Template$4(1, ofArray([HTML_El_Z67FF9A04("div", singleton(HTML_Attr_3DF4EB53("class", (_arg1) => {
    const counter = _arg1.Counter | 0;
    return toText(interpolate("size-%P()", [max((x, y) => comparePrimitives(x, y), 1, counter)]));
})), ofArray([HTML_El_Z67FF9A04("button", singleton(HTML_on_4A53169E("click", new Action(1, -10))), singleton(HTML_Text_Z721C83C5("-10"))), HTML_El_Z67FF9A04("button", singleton(HTML_on_4A53169E("click", new Action(1, -1))), singleton(HTML_Text_Z721C83C5("-"))), HTML_El_Z67FF9A04("button", singleton(HTML_on_4A53169E("click", new Action(1, 1))), singleton(HTML_Text_Z721C83C5("+"))), HTML_El_Z67FF9A04("button", singleton(HTML_on_4A53169E("click", new Action(1, 10))), singleton(HTML_Text_Z721C83C5("+10"))), HTML_El_Z67FF9A04("button", singleton(HTML_on_4A53169E("click", new Action(0))), singleton(HTML_Text_Z721C83C5("reset")))])), HTML_El_Z67FF9A04("div", empty(), ofArray([HTML_Text_Z721C83C5("count: "), HTML_MapState_Z5D9F8A77((_arg2) => {
    const counter_1 = _arg2.Counter | 0;
    return counter_1 | 0;
}, ofArray([HTML_El_Z67FF9A04("b", singleton(HTML_Attr_Z384F8060("style", "font-size: 32px;")), ofArray([HTML_Text_Z721C83C5("("), HTML_Text_77A7E8C8((s) => {
    let copyOfStruct = s | 0;
    return int32ToString(copyOfStruct);
}), HTML_Text_Z721C83C5(") ")])), HTML_OneOf_Z2AFE4804((v) => ((v > 9) ? (new FSharpChoice$2(0, "Great!")) : ((v > 4) ? (new FSharpChoice$2(1, v)) : (new FSharpChoice$2(2, void 0)))), HTML_Text(), HTML_Text_77A7E8C8((s_1) => toText(interpolate("%P() is a good number", [s_1]))), HTML_Text_Z721C83C5("meh"))]))]))]));

export function update(state_1, action) {
    if (action.tag === 0) {
        return new State(0);
    }
    else {
        const by = action.fields[0] | 0;
        return new State(state_1.Counter + by);
    }
}

export function middleware(_arg1) {
    const old = _arg1.Old;
    const dispatch = _arg1.Dispatch;
    const current = _arg1.Current;
    const action = _arg1.Action;
    console.log(some(toText(interpolate("Action: %P(), State: %P()", [action, current]))));
}

export const render = HTML_MakeProgram_Z7A39E9B2(template, document.body);

export const view = render((state_1, action) => update(state_1, action))((arg00$0040) => {
    middleware(arg00$0040);
})(state);

