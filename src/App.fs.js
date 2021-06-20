import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { record_type, int32_type, union_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { Value$2, Template$2 } from "./Core.fs.js";
import { renderDOM, DOMElement$1, DOMNode$1 } from "./Dom.fs.js";
import { singleton, empty } from "./.fable/fable-library.3.1.10/List.js";
import { interpolate, toText } from "./.fable/fable-library.3.1.10/String.js";
import { some } from "./.fable/fable-library.3.1.10/Option.js";
import { equals } from "./.fable/fable-library.3.1.10/Util.js";

export class Action extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Increment", "Decrement"];
    }
}

export function Action$reflection() {
    return union_type("App.Action", [], Action, () => [[], []]);
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

export const template = new Template$2(0, new DOMNode$1(0, new DOMElement$1("div", empty(), empty(), singleton(new Template$2(0, new DOMNode$1(1, new Value$2(1, (_arg1) => {
    const counter = _arg1.Counter | 0;
    return toText(interpolate("count: %P()", [counter]));
})))))));

export const view = renderDOM((v) => {
    console.log(some(v));
}, template, document.body, state);

export const eq = equals(makeState(1), makeState(1));

console.log(some(eq));

