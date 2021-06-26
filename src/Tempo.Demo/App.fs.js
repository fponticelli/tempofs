import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { record_type, union_type, int32_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { HTML_MakeProgram_Z9447D8C, HTML_Seq_Z7461BB91, HTML_Text, HTML_OneOf_Z2AFE4804, HTML_Text_77A7E8C8, HTML_Attr_Z3A5D29FA, HTML_Attr_3DF4EB53, HTML_On_47AABEE2, HTML_Text_Z721C83C5, HTML_On_4A53169E, HTML_Attr_Z384F8060, HTML_El_Z7374416F } from "../Tempo.Html/HtmlDSL.fs.js";
import { empty, singleton, ofArray } from "./.fable/fable-library.3.1.10/List.js";
import { interpolate, toText } from "./.fable/fable-library.3.1.10/String.js";
import { Template$4, mapState } from "../Tempo.Core/Core.fs.js";
import { int32ToString } from "./.fable/fable-library.3.1.10/Util.js";
import { FSharpChoice$2 } from "./.fable/fable-library.3.1.10/Choice.js";
import { toList } from "./.fable/fable-library.3.1.10/Seq.js";
import { rangeDouble } from "./.fable/fable-library.3.1.10/Range.js";
import { some } from "./.fable/fable-library.3.1.10/Option.js";

export class Action extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Reset", "Increment", "Set"];
    }
}

export function Action$reflection() {
    return union_type("App.Action", [], Action, () => [[], [["Item", int32_type]], [["Item", int32_type]]]);
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

export const template = HTML_El_Z7374416F("sp-theme", ofArray([HTML_Attr_Z384F8060("scale", "smallest"), HTML_Attr_Z384F8060("color", "light"), HTML_Attr_Z384F8060("style", "background: var(--spectrum-global-color-gray-75); padding: var(--spectrum-global-dimension-size-400);")]), ofArray([HTML_El_Z7374416F("div", singleton(HTML_Attr_Z384F8060("style", "display: flex; justify-content: space-between;")), ofArray([HTML_El_Z7374416F("sp-action-group", singleton(HTML_Attr_Z384F8060("compact", "compact")), ofArray([HTML_El_Z7374416F("sp-action-button", singleton(HTML_On_4A53169E("click", new Action(1, -10))), singleton(HTML_Text_Z721C83C5("-10"))), HTML_El_Z7374416F("sp-action-button", singleton(HTML_On_4A53169E("click", new Action(1, -1))), singleton(HTML_Text_Z721C83C5("-"))), HTML_El_Z7374416F("sp-action-button", singleton(HTML_On_4A53169E("click", new Action(1, 1))), singleton(HTML_Text_Z721C83C5("+"))), HTML_El_Z7374416F("sp-action-button", singleton(HTML_On_4A53169E("click", new Action(1, 10))), singleton(HTML_Text_Z721C83C5("+10")))])), HTML_El_Z7374416F("sp-number-field", ofArray([HTML_On_47AABEE2("input", (_arg1) => {
    const el = _arg1.Element;
    return new Action(2, el.value);
}), HTML_Attr_3DF4EB53("value", (_arg2) => {
    const c = _arg2.Counter | 0;
    return toText(interpolate("%P()", [c]));
})]), empty()), HTML_El_Z7374416F("sp-textfield", singleton(HTML_On_4A53169E("change", new Action(1, -1))), empty()), HTML_El_Z7374416F("sp-action-button", singleton(HTML_On_4A53169E("click", new Action(0))), singleton(HTML_Text_Z721C83C5("reset")))])), HTML_El_Z7374416F("sp-slider", ofArray([HTML_On_47AABEE2("input", (_arg3) => {
    const el_1 = _arg3.Element;
    return new Action(2, el_1.value);
}), HTML_Attr_3DF4EB53("value", (_arg4) => {
    const c_1 = _arg4.Counter | 0;
    return toText(interpolate("%P()", [c_1]));
}), HTML_Attr_Z384F8060("label", "Slider"), HTML_Attr_Z384F8060("variant", "tick"), HTML_Attr_Z384F8060("tick-labels", "tick-labels"), HTML_Attr_Z384F8060("tick-step", "10")]), empty()), HTML_El_Z7374416F("div", singleton(HTML_Attr_Z384F8060("style", "padding: 20px 0")), singleton(HTML_El_Z7374416F("sp-card", ofArray([HTML_Attr_3DF4EB53("heading", (_arg5) => {
    const counter = _arg5.Counter | 0;
    return toText(interpolate("counting ... %P()", [counter]));
}), HTML_Attr_Z384F8060("subheading", "No Preview")]), singleton(HTML_El_Z7374416F("div", singleton(HTML_Attr_Z384F8060("slot", "footer")), singleton(HTML_El_Z7374416F("sp-progress-circle", ofArray([HTML_Attr_Z384F8060("label", "Done?"), HTML_Attr_Z3A5D29FA("progress", (_arg6) => {
    const c_2 = _arg6.Counter | 0;
    return ((c_2 >= 0) ? (c_2 <= 100) : false) ? toText(interpolate("%P()", [c_2])) : (void 0);
}), HTML_Attr_Z3A5D29FA("indeterminate", (_arg7) => {
    const c_3 = _arg7.Counter | 0;
    return ((c_3 < 0) ? true : (c_3 > 100)) ? "" : (void 0);
}), HTML_Attr_Z384F8060("size", "large")]), empty()))))))), HTML_El_Z7374416F("div", empty(), ofArray([HTML_Text_Z721C83C5("count: "), mapState((_arg8) => {
    const counter_1 = _arg8.Counter | 0;
    return counter_1 | 0;
}, new Template$4(1, ofArray([HTML_El_Z7374416F("b", singleton(HTML_Attr_Z384F8060("style", "font-size: 32px;")), singleton(HTML_Text_77A7E8C8((s) => {
    let copyOfStruct = s | 0;
    return int32ToString(copyOfStruct);
}))), HTML_OneOf_Z2AFE4804((v) => ((v > 9) ? (new FSharpChoice$2(0, " ... Great!")) : ((v > 4) ? (new FSharpChoice$2(1, v)) : (new FSharpChoice$2(2, void 0)))), HTML_Text(), HTML_Text_77A7E8C8((s_1) => toText(interpolate(" ... %P() is a good number", [s_1]))), HTML_Text_Z721C83C5(" ... meh"))])))])), HTML_El_Z7374416F("ul", empty(), singleton(HTML_Seq_Z7461BB91((_arg9) => {
    const counter_2 = _arg9.Counter | 0;
    return toList(rangeDouble(1, 1, counter_2));
}, HTML_El_Z7374416F("li", empty(), singleton(HTML_Text_77A7E8C8((i) => toText(interpolate("Item: %P()", [i]))))))))]));

export function update(state_1, action) {
    switch (action.tag) {
        case 0: {
            return new State(0);
        }
        case 2: {
            const v = action.fields[0] | 0;
            return new State(v);
        }
        default: {
            const by = action.fields[0] | 0;
            return new State(state_1.Counter + by);
        }
    }
}

export function middleware(_arg1) {
    const prev = _arg1.Previous;
    const current = _arg1.Current;
    const action = _arg1.Action;
    console.log(some(toText(interpolate("Action: %P(), State: %P(), Previous %P()", [action, current, prev]))));
}

export const render = HTML_MakeProgram_Z9447D8C(template, document.body);

export const view = render((state_1, action) => update(state_1, action))((arg00$0040) => {
    middleware(arg00$0040);
})(state);

