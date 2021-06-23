import { Record } from "./.fable/fable-library.3.1.10/Types.js";
import { class_type, record_type, lambda_type, unit_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { cons, iterate, ofArray } from "./.fable/fable-library.3.1.10/List.js";
import { toArray } from "./.fable/fable-library.3.1.10/Option.js";
import { makeTrigger, DOMAttribute$2, DOMAttributeValue$2, DOMElement$3, DOMNode$3, DOMImpl_$ctor_Z97BEC54, DOMImpl_$ctor_4C3D2741, MakeDOMRender$3_$ctor_Z6156FC82 } from "./Dom.fs.js";
import { OneOf2$9_$ctor_Z6CB02F2D, packOneOf2, MapState$7_$ctor_3D37D6BB, packMapState, Value$2, Template$5, ComponentView$4, MakeRender$5__Make_Z982EEB6 } from "./Core.fs.js";
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
    let doc_1;
    let invokes = ofArray(toArray(audit));
    const dispatch = (action) => {
        iterate((f) => {
            f(action);
        }, invokes);
    };
    const renderInstance = MakeDOMRender$3_$ctor_Z6156FC82(dispatch);
    const f_1 = MakeRender$5__Make_Z982EEB6(renderInstance, template);
    const render = f_1((doc == null) ? DOMImpl_$ctor_4C3D2741(el) : (doc_1 = doc, DOMImpl_$ctor_Z97BEC54(el, doc_1)));
    return (update) => ((middleware) => ((state) => {
        let localState = state;
        const view = render(localState);
        const updateDispatch = (action_1) => {
            const newState = update(localState, action_1);
            view.Change(newState);
            middleware(new MiddlewarePayload$2(newState, localState, action_1, dispatch));
            localState = newState;
        };
        invokes = cons(updateDispatch, invokes);
        return new ComponentView$4(view.Impl, dispatch, view.Change, view.Destroy, view.Query);
    }));
}

export function DOM_El_BD8EB2A(name, attributes, children) {
    return new Template$5(0, new DOMNode$3(0, new DOMElement$3(name, attributes, children)));
}

export function DOM_Text() {
    return new Template$5(0, new DOMNode$3(1, new Value$2(1, (x) => x)));
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

export function DOM_on_4A53169E(name, action) {
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

export function DOM_MapState_4A1126C5(f, template) {
    return new Template$5(2, packMapState(MapState$7_$ctor_3D37D6BB(f, template)));
}

export function DOM_OneOf_61755184(f, template1, template2) {
    return new Template$5(3, packOneOf2(OneOf2$9_$ctor_Z6CB02F2D(f, template1, template2)));
}

