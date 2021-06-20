import { Union, Record } from "./.fable/fable-library.3.1.10/Types.js";
import { class_type, union_type, option_type, record_type, list_type, string_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { View$2, render, resolve, Value$2$reflection, Template$2$reflection } from "./Core.fs.js";
import { iterate, map, partition } from "./.fable/fable-library.3.1.10/List.js";
import { remove } from "./DomHelper.fs.js";

export class DOMElement$1 extends Record {
    constructor(Name, Attributes, Triggers, Children) {
        super();
        this.Name = Name;
        this.Attributes = Attributes;
        this.Triggers = Triggers;
        this.Children = Children;
    }
}

export function DOMElement$1$reflection(gen0) {
    return record_type("Tempo.Dom.DOMElement`1", [gen0], DOMElement$1, () => [["Name", string_type], ["Attributes", list_type(DOMAttribute$1$reflection(gen0))], ["Triggers", list_type(DOMTrigger$1$reflection(gen0))], ["Children", list_type(Template$2$reflection(DOMNode$1$reflection(gen0), gen0))]]);
}

export class DOMAttributeValue$1 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["StringAttribute"];
    }
}

export function DOMAttributeValue$1$reflection(gen0) {
    return union_type("Tempo.Dom.DOMAttributeValue`1", [gen0], DOMAttributeValue$1, () => [[["Item", Value$2$reflection(option_type(string_type), gen0)]]]);
}

export class DOMAttribute$1 extends Record {
    constructor(Name, Value) {
        super();
        this.Name = Name;
        this.Value = Value;
    }
}

export function DOMAttribute$1$reflection(gen0) {
    return record_type("Tempo.Dom.DOMAttribute`1", [gen0], DOMAttribute$1, () => [["Name", string_type], ["Value", DOMAttributeValue$1$reflection(gen0)]]);
}

export class DOMTrigger$1 extends Record {
    constructor(Name, Action) {
        super();
        this.Name = Name;
        this.Action = Action;
    }
}

export function DOMTrigger$1$reflection(gen0) {
    return record_type("Tempo.Dom.DOMTrigger`1", [gen0], DOMTrigger$1, () => [["Name", string_type], ["Action", class_type("Tempo.Dom.DOMTriggerEvaluator`1", [gen0])]]);
}

export class DOMNode$1 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["DOMElement", "DOMText"];
    }
}

export function DOMNode$1$reflection(gen0) {
    return union_type("Tempo.Dom.DOMNode`1", [gen0], DOMNode$1, () => [[["Item", DOMElement$1$reflection(gen0)]], [["Item", Value$2$reflection(string_type, gen0)]]]);
}

export function isAttributeDerived(_arg1) {
    const value = _arg1.Value;
    if (value.fields[0].tag === 0) {
        return false;
    }
    else {
        return true;
    }
}

export function applyAttribute(el, state, _arg1) {
    const value = _arg1.Value;
    const name = _arg1.Name;
    const v = value.fields[0];
    const matchValue = resolve(v, state);
    if (matchValue == null) {
        el.removeAttribute(name);
    }
    else {
        const v_1 = matchValue;
        el.setAttribute(name, v_1);
    }
}

export function applyAttributeF(el, _arg1) {
    const value = _arg1.Value;
    const name = _arg1.Name;
    const v = value.fields[0];
    return (state) => {
        const matchValue = resolve(v, state);
        if (matchValue == null) {
            el.removeAttribute(name);
        }
        else {
            const v_1 = matchValue;
            el.setAttribute(name, v_1);
        }
    };
}

export function applyTrigger(el, state, dispatch, _arg1) {
    const name = _arg1.Name;
    const action = _arg1.Action;
    const f = action.Eval();
    el.addEventListener(name, (e) => {
        f(state())(e);
    });
}

export function renderDOMElement(dispatch, node, impl, state) {
    let localState = state;
    const el = document.createElement(node.Name);
    const derived = partition((arg00$0040) => isAttributeDerived(arg00$0040), node.Attributes)[0];
    const derived_1 = map((arg10$0040) => applyAttributeF(el, arg10$0040), derived);
    iterate((a) => {
        applyAttributeF(el, a)(localState);
    }, node.Attributes);
    iterate((arg30$0040) => {
        applyTrigger(el, () => localState, dispatch, arg30$0040);
    }, node.Triggers);
    const children = map((c) => render((node_1, impl_1, state_2) => renderDOMNode(dispatch, node_1, impl_1, state_2), c, el, localState), node.Children);
    void impl.appendChild(el);
    return new View$2(el, (state_3) => {
        localState = state_3;
        iterate((f) => {
            f(localState);
        }, derived_1);
        iterate((v) => {
            v.Change(localState);
        }, children);
    }, () => {
        remove(el);
        iterate((v_1) => {
            v_1.Destroy();
        }, children);
    });
}

export function renderDOMText(node, impl, state) {
    const s = resolve(node, state);
    const n = document.createTextNode(s);
    return new View$2(impl, (state_1) => {
        n.nodeValue = resolve(node, state_1);
    }, () => {
        remove(n);
    });
}

export function renderDOMNode(dispatch, node, impl, state) {
    if (node.tag === 1) {
        const v = node.fields[0];
        return renderDOMText(v, impl, state);
    }
    else {
        const el = node.fields[0];
        return renderDOMElement(dispatch, el, impl, state);
    }
}

export function renderDOM(dispatch, template, parent, state) {
    return render((node, impl, state_1) => renderDOMNode(dispatch, node, impl, state_1), template, parent, state);
}

