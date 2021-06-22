import { Record } from "./.fable/fable-library.3.1.10/Types.js";
import { class_type, record_type, lambda_type, unit_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { cons, iterate, ofArray } from "./.fable/fable-library.3.1.10/List.js";
import { toArray } from "./.fable/fable-library.3.1.10/Option.js";
import { Value$2, Template$5, ComponentView$4, makeRender } from "./Core.fs.js";
import { uncurry } from "./.fable/fable-library.3.1.10/Util.js";
import { makeTrigger, DOMAttribute$2, DOMAttributeValue$2, DOMElement$3, DOMNode$3, DOMImpl_$ctor_3A48CFF9, makeRenderDOMNode } from "./Dom.fs.js";
import { ToOption } from "./StringExtra.fs.js";

export class MiddlewarePayload$2 extends Record {
    constructor(Current, Old, Action, Dispatch) {
        super();
        this.Current = Current;
        this.Old = Old;
        this.Action = Action;
        this.Dispatch = Dispatch;
    }
}

export function MiddlewarePayload$2$reflection(gen0, gen1) {
    return record_type("Tempo.Dom.DSL.MiddlewarePayload`2", [gen0, gen1], MiddlewarePayload$2, () => [["Current", gen0], ["Old", gen0], ["Action", gen1], ["Dispatch", lambda_type(gen1, unit_type)]]);
}

export class DOM {
    constructor() {
    }
}

export function DOM$reflection() {
    return class_type("Tempo.Dom.DSL.DOM", void 0, DOM);
}

export function DOM_Make_Z28B756F0(template, el, audit, doc) {
    let invokes = ofArray(toArray(audit));
    const dispatch = (action) => {
        iterate((f) => {
            f(action);
        }, invokes);
    };
    const render = makeRender(uncurry(3, (node) => makeRenderDOMNode(dispatch, node)), template)(DOMImpl_$ctor_3A48CFF9(el, doc));
    return (update) => ((middleware) => ((state) => {
        let localState = state;
        const view = render(localState);
        invokes = cons((action_1) => {
            const newState = update(localState, action_1);
            view.Change(newState);
            middleware(new MiddlewarePayload$2(newState, localState, action_1, dispatch));
            localState = newState;
        }, invokes);
        return new ComponentView$4(void 0, dispatch, view.Change, view.Destroy, view.Query);
    }));
}

export function DOM_El_BD8EB2A(name, attributes, children) {
    return new Template$5(0, new DOMNode$3(0, new DOMElement$3(name, attributes, children)));
}

export function DOM_Text_Z721C83C5(value) {
    return new Template$5(0, new DOMNode$3(1, new Value$2(0, value)));
}

export function DOM_Text_77A7E8C8(f) {
    return new Template$5(0, new DOMNode$3(1, new Value$2(1, f)));
}

export function DOM_Attr_68C4AEB5(name, value) {
    return new DOMAttribute$2(name, new DOMAttributeValue$2(0, new Value$2(0, value)));
}

export function DOM_Attr_Z384F8060(name, value) {
    return new DOMAttribute$2(name, new DOMAttributeValue$2(0, new Value$2(0, ToOption(value))));
}

export function DOM_Attr_Z3A5D29FA(name, f) {
    return new DOMAttribute$2(name, new DOMAttributeValue$2(0, new Value$2(1, f)));
}

export function DOM_Attr_3DF4EB53(name, f) {
    return new DOMAttribute$2(name, new DOMAttributeValue$2(0, new Value$2(1, (arg) => ToOption(f(arg)))));
}

export function DOM_On_4A53169E(name, action) {
    return new DOMAttribute$2(name, new DOMAttributeValue$2(1, makeTrigger((_arg2, _arg1) => action)));
}

export function DOM_On_459CDA74(name, handler) {
    return new DOMAttribute$2(name, new DOMAttributeValue$2(1, makeTrigger((_arg4, _arg3) => handler())));
}

export function DOM_On_16D4E2A2(name, handler) {
    return new DOMAttribute$2(name, new DOMAttributeValue$2(1, makeTrigger((_arg5, e) => handler(e))));
}

export function DOM_On_Z5276E2EF(name, handler) {
    return new DOMAttribute$2(name, new DOMAttributeValue$2(1, makeTrigger(handler)));
}

export function DOM_On_36180E4D(name, handler) {
    return new DOMAttribute$2(name, new DOMAttributeValue$2(1, makeTrigger((s, _arg6) => handler(s))));
}

