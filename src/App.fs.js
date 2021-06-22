import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { record_type, union_type, int32_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { Template$5 } from "./Core.fs.js";
import { DOM_Make_Z28B756F0, DOM_Text_77A7E8C8, DOM_El_Z46453AFE, DOM_Text_Z721C83C5, DOM_On_4A53169E, DOM_Attr_3DF4EB53, DOM_El_BD8EB2A } from "./Dom.fs.js";
import { interpolate, toText } from "./.fable/fable-library.3.1.10/String.js";
import { int32ToString, comparePrimitives, max } from "./.fable/fable-library.3.1.10/Util.js";
import { ofArray, singleton } from "./.fable/fable-library.3.1.10/List.js";
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

export const template = new Template$5(1, ofArray([DOM_El_BD8EB2A("div", singleton(DOM_Attr_3DF4EB53("class", (_arg1) => {
    const counter = _arg1.Counter | 0;
    return toText(interpolate("size-%P()", [max((x, y) => comparePrimitives(x, y), 1, counter)]));
})), ofArray([DOM_El_BD8EB2A("button", singleton(DOM_On_4A53169E("click", new Action(1, -10))), singleton(DOM_Text_Z721C83C5("-10"))), DOM_El_BD8EB2A("button", singleton(DOM_On_4A53169E("click", new Action(1, -1))), singleton(DOM_Text_Z721C83C5("-"))), DOM_El_BD8EB2A("button", singleton(DOM_On_4A53169E("click", new Action(1, 1))), singleton(DOM_Text_Z721C83C5("+"))), DOM_El_BD8EB2A("button", singleton(DOM_On_4A53169E("click", new Action(1, 10))), singleton(DOM_Text_Z721C83C5("+10"))), DOM_El_BD8EB2A("button", singleton(DOM_On_4A53169E("click", new Action(0))), singleton(DOM_Text_Z721C83C5("reset")))])), DOM_El_Z46453AFE("div", ofArray([DOM_Text_Z721C83C5("count: "), DOM_Text_77A7E8C8((_arg2) => {
    const counter_1 = _arg2.Counter | 0;
    return int32ToString(counter_1);
})]))]));

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

export const render = DOM_Make_Z28B756F0(template, document.body);

export const view = render((state_1, action) => update(state_1, action))((arg00$0040) => {
    middleware(arg00$0040);
})(state);

