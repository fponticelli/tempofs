import { option_type, record_type, list_type, union_type, string_type, class_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { defaultArg } from "./.fable/fable-library.3.1.10/Option.js";
import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { View$3, makeRender, Value$2_Resolve, Template$5$reflection, Value$2$reflection } from "./Core.fs.js";
import { partialApply, mapCurriedArgs, uncurry, curry } from "./.fable/fable-library.3.1.10/Util.js";
import { append, map, iterate } from "./.fable/fable-library.3.1.10/List.js";
import { filterMap } from "./ListExtra.fs.js";
import { remove } from "./DomHelper.fs.js";

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
        el.setAttribute(name, s);
    }
}

export function derivedApplication(_arg1) {
    const value = _arg1.Value;
    if (value.tag === 1) {
        return void 0;
    }
    else if (value.fields[0].tag === 0) {
        return void 0;
    }
    else {
        return (el) => ((state) => {
            applyStringAttribute(_arg1.Name, el, value.fields[0].fields[0](state));
        });
    }
}

export function applyAttribute(dispatch, el, state, _arg1) {
    const value = _arg1.Value;
    const name = _arg1.Name;
    if (value.tag === 1) {
        void unpackDOMTrigger(value.fields[0], {
            Invoke(t) {
                el.addEventListener(name, (e) => {
                    dispatch(DOMTrigger$3__get_Handler(t)(state())(e));
                });
                return 0;
            },
        });
    }
    else {
        applyStringAttribute(name, el, Value$2_Resolve(value.fields[0], state()));
    }
}

export function makeRenderDOMNode(dispatch, node) {
    if (node.tag === 1) {
        return (impl) => ((state_1) => makeRenderDOMText(node.fields[0], impl, state_1));
    }
    else {
        return (parent) => ((state) => makeRenderDOMElement(node.fields[0], dispatch, parent, state));
    }
}

export function makeRenderDOMElement(node, dispatch, parent, state) {
    let localState = state;
    const el = document.createElement(node.Name);
    const impl = DOMImpl_$ctor_3A48CFF9(el, DOMImpl__get_Doc(parent));
    iterate((arg30$0040) => {
        applyAttribute(dispatch, el, () => localState, arg30$0040);
    }, node.Attributes);
    void DOMImpl__get_Element(parent).appendChild(el);
    const childViews = map((child) => makeRender(uncurry(3, (node_1) => makeRenderDOMNode(dispatch, node_1)), child)(impl)(localState), node.Children);
    const childUpdates = map((_arg1) => _arg1.Change, childViews);
    const childDestroys = map((_arg2) => _arg2.Destroy, childViews);
    const childQueries = map((_arg3) => _arg3.Query, childViews);
    const updates = append(map(mapCurriedArgs((f) => partialApply(1, f, [el]), [[0, 2]]), filterMap((arg00$0040) => derivedApplication(arg00$0040), node.Attributes)), childUpdates);
    return new View$3(impl, (state_1) => {
        localState = state_1;
        iterate((change_1) => {
            change_1(localState);
        }, updates);
    }, () => {
        remove(el);
        iterate((destroy_1) => {
            destroy_1();
        }, childDestroys);
    }, (q) => {
        iterate((query_1) => {
            query_1(q);
        }, childQueries);
    });
}

export function makeRenderDOMText(value, impl, state) {
    let patternInput;
    if (value.tag === 0) {
        const n_1 = DOMImpl__get_Doc(impl).createTextNode(value.fields[0]);
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
    return new View$3(void 0, patternInput[0], patternInput[1], (value_4) => {
    });
}

