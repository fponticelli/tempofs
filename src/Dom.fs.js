import { lambda_type, unit_type, option_type, record_type, list_type, union_type, string_type, class_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { toArray, defaultArg } from "./.fable/fable-library.3.1.10/Option.js";
import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { Value$2, Template$5, ComponentView$4, View$3, makeRender, Value$2_Resolve, Template$5$reflection, Value$2$reflection } from "./Core.fs.js";
import { partialApply, mapCurriedArgs, uncurry, curry } from "./.fable/fable-library.3.1.10/Util.js";
import { empty, cons, ofArray, append, map, iterate } from "./.fable/fable-library.3.1.10/List.js";
import { filterMap } from "./ListExtra.fs.js";
import { remove } from "./DomHelper.fs.js";
import { ToOption } from "./StringExtra.fs.js";

export class DOMImpl {
    constructor(el, doc) {
        this.el = el;
        this.doc = doc;
    }
}

export function DOMImpl$reflection() {
    return class_type("Tempo.Dom.DOMImpl", void 0, DOMImpl);
}

export function DOMImpl_$ctor_3A48CFF9(el, doc) {
    return new DOMImpl(el, doc);
}

export function DOMImpl__get_Element(this$) {
    return this$.el;
}

export function DOMImpl__get_Doc(this$) {
    return defaultArg(this$.doc, DOMImpl__get_Element(this$).ownerDocument || document);
}

export class DOMNode$3 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["DOMElement", "DOMText"];
    }
}

export function DOMNode$3$reflection(gen0, gen1, gen2) {
    return union_type("Tempo.Dom.DOMNode`3", [gen0, gen1, gen2], DOMNode$3, () => [[["Item", DOMElement$3$reflection(gen0, gen1, gen2)]], [["Item", Value$2$reflection(gen0, string_type)]]]);
}

export class DOMElement$3 extends Record {
    constructor(Name, Attributes, Children) {
        super();
        this.Name = Name;
        this.Attributes = Attributes;
        this.Children = Children;
    }
}

export function DOMElement$3$reflection(gen0, gen1, gen2) {
    return record_type("Tempo.Dom.DOMElement`3", [gen0, gen1, gen2], DOMElement$3, () => [["Name", string_type], ["Attributes", list_type(DOMAttribute$2$reflection(gen0, gen1))], ["Children", list_type(Template$5$reflection(DOMNode$3$reflection(gen0, gen1, gen2), DOMImpl$reflection(), gen0, gen1, gen2))]]);
}

export class DOMAttribute$2 extends Record {
    constructor(Name, Value) {
        super();
        this.Name = Name;
        this.Value = Value;
    }
}

export function DOMAttribute$2$reflection(gen0, gen1) {
    return record_type("Tempo.Dom.DOMAttribute`2", [gen0, gen1], DOMAttribute$2, () => [["Name", string_type], ["Value", DOMAttributeValue$2$reflection(gen0, gen1)]]);
}

export class DOMAttributeValue$2 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["StringValue", "TriggerValue"];
    }
}

export function DOMAttributeValue$2$reflection(gen0, gen1) {
    return union_type("Tempo.Dom.DOMAttributeValue`2", [gen0, gen1], DOMAttributeValue$2, () => [[["Item", Value$2$reflection(gen0, option_type(string_type))]], [["Item", class_type("Tempo.Dom.IDOMTrigger`2", [gen0, gen1])]]]);
}

export class DOMTrigger$3 {
    constructor(handler) {
        this.handler = handler;
    }
    Accept(f) {
        const this$ = this;
        return f.Invoke(this$);
    }
}

export function DOMTrigger$3$reflection(gen0, gen1, gen2) {
    return class_type("Tempo.Dom.DOMTrigger`3", [gen0, gen1, gen2], DOMTrigger$3);
}

export function DOMTrigger$3_$ctor_Z685BAA8A(handler) {
    return new DOMTrigger$3(handler);
}

export function DOMTrigger$3__get_Handler(this$) {
    return curry(2, this$.handler);
}

export function packDOMTrigger(trigger) {
    return trigger;
}

export function unpackDOMTrigger(trigger, f) {
    return trigger.Accept(f);
}

export function makeTrigger(f) {
    return packDOMTrigger(DOMTrigger$3_$ctor_Z685BAA8A(f));
}

export function applyStringAttribute(name, el, s) {
    if (s == null) {
        el.removeAttribute(name);
    }
    else {
        const s_1 = s;
        el.setAttribute(name, s_1);
    }
}

export function derivedApplication(_arg1) {
    const value = _arg1.Value;
    const name = _arg1.Name;
    if (value.tag === 1) {
        return void 0;
    }
    else if (value.fields[0].tag === 0) {
        return void 0;
    }
    else {
        const f = value.fields[0].fields[0];
        return (el) => ((state) => {
            applyStringAttribute(name, el, f(state));
        });
    }
}

export function applyAttribute(dispatch, el, state, _arg1) {
    const value = _arg1.Value;
    const name = _arg1.Name;
    if (value.tag === 1) {
        const domTrigger = value.fields[0];
        void unpackDOMTrigger(domTrigger, {
            Invoke(t) {
                el.addEventListener(name, (e) => {
                    dispatch(DOMTrigger$3__get_Handler(t)(state())(e));
                });
                return 0;
            },
        });
    }
    else {
        const v = value.fields[0];
        applyStringAttribute(name, el, Value$2_Resolve(v, state()));
    }
}

export function makeRenderDOMNode(dispatch, node) {
    if (node.tag === 1) {
        const v = node.fields[0];
        return (impl) => ((state_1) => makeRenderDOMText(v, impl, state_1));
    }
    else {
        const el = node.fields[0];
        return (parent) => ((state) => makeRenderDOMElement(el, dispatch, parent, state));
    }
}

export function makeRenderDOMElement(node, dispatch, parent, state) {
    let localState = state;
    const el = document.createElement(node.Name);
    const impl = DOMImpl_$ctor_3A48CFF9(el, DOMImpl__get_Doc(parent));
    const getState = () => localState;
    iterate((arg30$0040) => {
        applyAttribute(dispatch, el, getState, arg30$0040);
    }, node.Attributes);
    const childRender = (template) => makeRender(uncurry(3, (node_1) => makeRenderDOMNode(dispatch, node_1)), template);
    void DOMImpl__get_Element(parent).appendChild(el);
    const childViews = map((child) => childRender(child)(impl)(localState), node.Children);
    const childUpdates = map((_arg1) => {
        const change = _arg1.Change;
        return change;
    }, childViews);
    const childDestroys = map((_arg2) => {
        const destroy = _arg2.Destroy;
        return destroy;
    }, childViews);
    const childQueries = map((_arg3) => {
        const query = _arg3.Query;
        return query;
    }, childViews);
    const attributeUpdates = map(mapCurriedArgs((f) => partialApply(1, f, [el]), [[0, 2]]), filterMap((arg00$0040) => derivedApplication(arg00$0040), node.Attributes));
    const updates = append(attributeUpdates, childUpdates);
    const change_2 = (state_1) => {
        localState = state_1;
        iterate((change_1) => {
            change_1(localState);
        }, updates);
    };
    const destroy_2 = () => {
        remove(el);
        iterate((destroy_1) => {
            destroy_1();
        }, childDestroys);
    };
    const query_2 = (q) => {
        iterate((query_1) => {
            query_1(q);
        }, childQueries);
    };
    return new View$3(impl, change_2, destroy_2, query_2);
}

export function makeRenderDOMText(value, impl, state) {
    let patternInput;
    if (value.tag === 0) {
        const s = value.fields[0];
        const n_1 = DOMImpl__get_Doc(impl).createTextNode(s);
        void DOMImpl__get_Element(impl).appendChild(n_1);
        patternInput = [(value_3) => {
        }, () => {
            remove(n_1);
        }];
    }
    else {
        const f = value.fields[0];
        let n;
        const arg00 = f(state);
        n = DOMImpl__get_Doc(impl).createTextNode(arg00);
        void DOMImpl__get_Element(impl).appendChild(n);
        patternInput = [(state_1) => {
            n.nodeValue = f(state_1);
        }, () => {
            remove(n);
        }];
    }
    const destroy = patternInput[1];
    const change = patternInput[0];
    return new View$3(void 0, change, destroy, (value_4) => {
    });
}

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
    return record_type("Tempo.Dom.MiddlewarePayload`2", [gen0, gen1], MiddlewarePayload$2, () => [["Current", gen0], ["Old", gen0], ["Action", gen1], ["Dispatch", lambda_type(gen1, unit_type)]]);
}

export class DOM {
    constructor() {
    }
}

export function DOM$reflection() {
    return class_type("Tempo.Dom.DOM", void 0, DOM);
}

export function DOM_Make_Z28B756F0(template, el, audit, doc) {
    let invokes = ofArray(toArray(audit));
    const dispatch = (action) => {
        iterate((f) => {
            f(action);
        }, invokes);
    };
    const f_1 = makeRender(uncurry(3, (node) => makeRenderDOMNode(dispatch, node)), template);
    const render = f_1(DOMImpl_$ctor_3A48CFF9(el, doc));
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
        return new ComponentView$4(void 0, dispatch, view.Change, view.Destroy, view.Query);
    }));
}

export function DOM_El_7E2C5EAA(name, attributes, children) {
    return new Template$5(0, new DOMNode$3(0, new DOMElement$3(name, defaultArg(attributes, empty()), defaultArg(children, empty()))));
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

