import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { option_type, record_type, string_type, union_type, list_type, class_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { append, iterate, cons, map, concat, singleton } from "./.fable/fable-library.3.1.10/List.js";
import { remove } from "./DomHelper.fs.js";
import { View$3, MakeRender$5__Make_Z982EEB6, MakeRender$5$reflection, MakeRender$5, Value$2_Resolve, Template$5$reflection, Value$2$reflection } from "./Core.fs.js";
import { partialApply, mapCurriedArgs, curry } from "./.fable/fable-library.3.1.10/Util.js";
import { filterMap } from "./ListExtra.fs.js";

export class DOMImplNode extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["ElementI", "TextI", "RefI"];
    }
}

export function DOMImplNode$reflection() {
    return union_type("Tempo.Dom.DOMImplNode", [], DOMImplNode, () => [[["Item", class_type("Browser.Types.HTMLElement")]], [["Item", class_type("Browser.Types.Text")]], [["Item1", class_type("Browser.Types.Node")], ["Item2", list_type(DOMImplNode$reflection())]]]);
}

export function DOMImplNode__GetNodes(this$) {
    switch (this$.tag) {
        case 1: {
            const t = this$.fields[0];
            return singleton(t);
        }
        case 2: {
            const r = this$.fields[0];
            const ls = this$.fields[1];
            return concat(cons(singleton(r), map((i) => DOMImplNode__GetNodes(i), ls)));
        }
        default: {
            const el = this$.fields[0];
            return singleton(el);
        }
    }
}

export function DOMImplNode__HeadNode(this$) {
    switch (this$.tag) {
        case 1: {
            const t = this$.fields[0];
            return t;
        }
        case 2: {
            const r = this$.fields[0];
            return r;
        }
        default: {
            const el = this$.fields[0];
            return el;
        }
    }
}

export function DOMImplNode__InsertBefore_Z7415BE42(this$, ref) {
    const toInsert = DOMImplNode__GetNodes(this$);
    const refNode = DOMImplNode__HeadNode(ref);
    const parent = refNode.parentElement;
    iterate((item) => {
        void parent.insertBefore(item, refNode);
    }, toInsert);
}

export function DOMImplNode__Remove(this$) {
    const toRemove = DOMImplNode__GetNodes(this$);
    iterate((n) => {
        remove(n);
    }, toRemove);
}

export function DOMImplNode__Append_Z7415BE42(this$, child) {
    switch (this$.tag) {
        case 1:
        case 2: {
            DOMImplNode__InsertBefore_Z7415BE42(child, this$);
            break;
        }
        default: {
            const parent = this$.fields[0];
            const toAppend = DOMImplNode__GetNodes(child);
            iterate((item) => {
                void parent.appendChild(item);
            }, toAppend);
        }
    }
}

export class DOMImpl {
    constructor(node, doc) {
        this.node = node;
        this.doc = doc;
    }
}

export function DOMImpl$reflection() {
    return class_type("Tempo.Dom.DOMImpl", void 0, DOMImpl);
}

export function DOMImpl_$ctor_2C5465B3(node, doc) {
    return new DOMImpl(node, doc);
}

export function DOMImpl__get_Node(this$) {
    return this$.node;
}

export function DOMImpl__get_Doc(this$) {
    return this$.doc;
}

export function DOMImpl_$ctor_4C3D2741(el) {
    return DOMImpl_$ctor_2C5465B3(new DOMImplNode(0, el), el.ownerDocument || document);
}

export function DOMImpl_$ctor_Z97BEC54(el, doc) {
    return DOMImpl_$ctor_2C5465B3(new DOMImplNode(0, el), doc);
}

export function DOMImpl_$ctor_171E4B9F(text) {
    return DOMImpl_$ctor_2C5465B3(new DOMImplNode(1, text), text.ownerDocument || document);
}

export function DOMImpl_$ctor_Z207E5C4E(text, doc) {
    return DOMImpl_$ctor_2C5465B3(new DOMImplNode(1, text), doc);
}

export function DOMImpl_$ctor_2130CD6(ref, ls) {
    return DOMImpl_$ctor_2C5465B3(new DOMImplNode(2, ref, ls), ref.ownerDocument || document);
}

export function DOMImpl_$ctor_6112BB5B(ref, ls, doc) {
    return DOMImpl_$ctor_2C5465B3(new DOMImplNode(2, ref, ls), doc);
}

export function DOMImpl__Remove(this$) {
    DOMImplNode__Remove(DOMImpl__get_Node(this$));
}

export function DOMImpl__Append_7B40145E(this$, child) {
    DOMImplNode__Append_Z7415BE42(DOMImpl__get_Node(this$), DOMImpl__get_Node(child));
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

export class MakeDOMRender$3 extends MakeRender$5 {
    constructor(dispatch) {
        super();
        this.dispatch = dispatch;
    }
    ["Tempo.Core.MakeRender`5.MakeNode2B595"](node) {
        const this$ = this;
        if (node.tag === 1) {
            const v = node.fields[0];
            return MakeDOMRender$3__MakeRenderDOMText_Z320284C0(this$, v);
        }
        else {
            const el = node.fields[0];
            return MakeDOMRender$3__MakeRenderDOMElement_630FAB1E(this$, el);
        }
    }
    ["Tempo.Core.MakeRender`5.MakeRef"](parent, children) {
        const nodes = map((child) => DOMImpl__get_Node(child), children);
        const ref = document.createTextNode("");
        return DOMImpl_$ctor_6112BB5B(ref, nodes, DOMImpl__get_Doc(parent));
    }
    ["Tempo.Core.MakeRender`5.RemoveNode2B594"](node) {
        DOMImpl__Remove(node);
    }
    ["Tempo.Core.MakeRender`5.InsertBeforeNode"](ref, toInsert) {
        DOMImplNode__InsertBefore_Z7415BE42(DOMImpl__get_Node(ref), DOMImpl__get_Node(toInsert));
    }
    ["Tempo.Core.MakeRender`5.AppendNode"](parent, child) {
        DOMImpl__Append_7B40145E(parent, child);
    }
}

export function MakeDOMRender$3$reflection(gen0, gen1, gen2) {
    return class_type("Tempo.Dom.MakeDOMRender`3", [gen0, gen1, gen2], MakeDOMRender$3, MakeRender$5$reflection(DOMNode$3$reflection(gen0, gen1, gen2), DOMImpl$reflection(), gen0, gen1, gen2));
}

export function MakeDOMRender$3_$ctor_Z6156FC82(dispatch) {
    return new MakeDOMRender$3(dispatch);
}

export function MakeDOMRender$3__MakeRenderDOMElement_630FAB1E(this$, node) {
    return (parent) => ((state) => {
        let localState = state;
        const el = document.createElement(node.Name);
        const impl = DOMImpl_$ctor_Z97BEC54(el, DOMImpl__get_Doc(parent));
        const getState = () => localState;
        iterate((arg30$0040) => {
            applyAttribute(this$.dispatch, el, getState, arg30$0040);
        }, node.Attributes);
        DOMImpl__Append_7B40145E(parent, impl);
        const childViews = map((child) => MakeRender$5__Make_Z982EEB6(this$, child)(impl)(localState), node.Children);
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
        return new View$3(DOMImpl_$ctor_Z97BEC54(el, DOMImpl__get_Doc(parent)), change_2, destroy_2, query_2);
    });
}

export function MakeDOMRender$3__MakeRenderDOMText_Z320284C0(this$, value) {
    return (parent) => ((state) => {
        if (value.tag === 0) {
            const s = value.fields[0];
            const n_1 = DOMImpl__get_Doc(parent).createTextNode(s);
            const impl_1 = DOMImpl_$ctor_Z207E5C4E(n_1, DOMImpl__get_Doc(parent));
            DOMImpl__Append_7B40145E(parent, impl_1);
            return new View$3(impl_1, (value_2) => {
            }, () => {
                DOMImpl__Remove(impl_1);
            }, (value_3) => {
            });
        }
        else {
            const f = value.fields[0];
            let n;
            const arg00 = f(state);
            n = DOMImpl__get_Doc(parent).createTextNode(arg00);
            const impl = DOMImpl_$ctor_Z207E5C4E(n, DOMImpl__get_Doc(parent));
            DOMImpl__Append_7B40145E(parent, impl);
            return new View$3(impl, (state_1) => {
                n.nodeValue = f(state_1);
            }, () => {
                DOMImpl__Remove(impl);
            }, (value_1) => {
            });
        }
    });
}

