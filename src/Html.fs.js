import { option_type, record_type, list_type, union_type, string_type, class_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { append, map, empty, filter, cons, reverse, collect, iterate, singleton } from "./.fable/fable-library.3.1.10/List.js";
import { interpolate, toText } from "./.fable/fable-library.3.1.10/String.js";
import { remove } from "./HtmlTools.fs.js";
import { partialApply, mapCurriedArgs, equals } from "./.fable/fable-library.3.1.10/Util.js";
import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { View$2, MakeRender$4__Make_1DCD9633, MakeRender$4$reflection, MakeRender$4, Value$2_Resolve, Template$4$reflection, Value$2$reflection } from "./Core.fs.js";
import { filterMap } from "./ListExtra.fs.js";

export class HTMLImpl {
    constructor() {
    }
}

export function HTMLImpl$reflection() {
    return class_type("Tempo.Html.HTMLImpl", void 0, HTMLImpl);
}

export function HTMLImpl_$ctor() {
    return new HTMLImpl();
}

export class HTMLElementImpl {
    constructor(element) {
        this.element = element;
    }
    ["Tempo.Html.HTMLImpl.GetNodes"]() {
        const this$ = this;
        return singleton(this$.element);
    }
    Append(child) {
        const this$ = this;
        if (child["GetNodes"] !== null) {
            const child_1 = child;
            const nodes = child_1["Tempo.Html.HTMLImpl.GetNodes"]();
            iterate((arg) => {
                void this$.element.appendChild(arg);
            }, nodes);
        }
        else {
            throw (new Error(toText(interpolate("HTMLElementImpl doesn\u0027t know how to append a child of type %P()", [child]))));
        }
    }
    Remove(child) {
        if (child["GetNodes"] !== null) {
            const child_1 = child;
            const ls = child_1["Tempo.Html.HTMLImpl.GetNodes"]();
            iterate((n) => {
                remove(n);
            }, ls);
        }
        else {
            throw (new Error(toText(interpolate("HTMLElementImpl doesn\u0027t know how to remove a child of type %P()", [child]))));
        }
    }
}

export function HTMLElementImpl$reflection() {
    return class_type("Tempo.Html.HTMLElementImpl", void 0, HTMLElementImpl, HTMLImpl$reflection());
}

export function HTMLElementImpl_$ctor_4C3D2741(el) {
    HTMLImpl_$ctor();
    return new HTMLElementImpl(el);
}

export function HTMLElementImpl_$ctor_Z721C83C5(name) {
    return HTMLElementImpl_$ctor_4C3D2741(document.createElement(name));
}

export class HTMLTextImpl {
    constructor(text) {
        this.text = text;
    }
    ["Tempo.Html.HTMLImpl.GetNodes"]() {
        const this$ = this;
        return singleton(this$.text);
    }
    Append(child) {
        throw (new Error("HTMLTextImpl does not support adding children"));
    }
    Remove(child) {
        throw (new Error("HTMLTextImpl does not support removing children"));
    }
}

export function HTMLTextImpl$reflection() {
    return class_type("Tempo.Html.HTMLTextImpl", void 0, HTMLTextImpl, HTMLImpl$reflection());
}

export function HTMLTextImpl_$ctor_171E4B9F(text) {
    HTMLImpl_$ctor();
    return new HTMLTextImpl(text);
}

export function HTMLTextImpl_$ctor_Z721C83C5(value) {
    return HTMLTextImpl_$ctor_171E4B9F(document.createTextNode(value));
}

let counter = 0;

export class HTMLGroupImpl {
    constructor(ref, children) {
        this.ref = ref;
        this.children = children;
    }
    ["Tempo.Html.HTMLImpl.GetNodes"]() {
        const this$ = this;
        return cons(this$.ref, collect((child) => {
            if (child["GetNodes"] !== null) {
                const child_1 = child;
                return child_1["Tempo.Html.HTMLImpl.GetNodes"]();
            }
            else {
                throw (new Error(toText(interpolate("Group contains a foreign element %P()", [child]))));
            }
        }, reverse(this$.children)));
    }
    Append(child) {
        const this$ = this;
        this$.children = cons(child, this$.children);
        if (child["GetNodes"] !== null) {
            const child_1 = child;
            const nodes = child_1["Tempo.Html.HTMLImpl.GetNodes"]();
            const parent = this$.ref.parentNode;
            iterate((node) => {
                void parent.insertBefore(node, this$.ref);
            }, nodes);
        }
        else {
            throw (new Error(toText(interpolate("HTMLGroupImpl doesn\u0027t know how to append a child of type %P()", [child]))));
        }
    }
    Remove(child) {
        const this$ = this;
        if (child["GetNodes"] !== null) {
            const htmlChild = child;
            this$.children = filter((c) => (!equals(c, child)), this$.children);
            const ls = htmlChild["Tempo.Html.HTMLImpl.GetNodes"]();
            iterate((n) => {
                remove(n);
            }, ls);
        }
        else {
            throw (new Error(toText(interpolate("HTMLGroupImpl doesn\u0027t know how to append a child of type %P()", [child]))));
        }
    }
}

export function HTMLGroupImpl$reflection() {
    return class_type("Tempo.Html.HTMLGroupImpl", void 0, HTMLGroupImpl, HTMLImpl$reflection());
}

export function HTMLGroupImpl_$ctor_Z721C83C5(label) {
    counter = (counter + 1);
    HTMLImpl_$ctor();
    return new HTMLGroupImpl(document.createComment(toText(interpolate("%P(): %P()", [label, counter]))), empty());
}

export class HTMLTemplateNode$3 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["HTMLTemplateElement", "HTMLTemplateText"];
    }
}

export function HTMLTemplateNode$3$reflection(gen0, gen1, gen2) {
    return union_type("Tempo.Html.HTMLTemplateNode`3", [gen0, gen1, gen2], HTMLTemplateNode$3, () => [[["Item", HTMLTemplateElement$3$reflection(gen0, gen1, gen2)]], [["Item", Value$2$reflection(gen0, string_type)]]]);
}

export class HTMLTemplateElement$3 extends Record {
    constructor(Name, Attributes, Children) {
        super();
        this.Name = Name;
        this.Attributes = Attributes;
        this.Children = Children;
    }
}

export function HTMLTemplateElement$3$reflection(gen0, gen1, gen2) {
    return record_type("Tempo.Html.HTMLTemplateElement`3", [gen0, gen1, gen2], HTMLTemplateElement$3, () => [["Name", string_type], ["Attributes", list_type(HTMLTemplateAttribute$2$reflection(gen0, gen1))], ["Children", list_type(Template$4$reflection(HTMLTemplateNode$3$reflection(gen0, gen1, gen2), gen0, gen1, gen2))]]);
}

export class HTMLTemplateAttribute$2 extends Record {
    constructor(Name, Value) {
        super();
        this.Name = Name;
        this.Value = Value;
    }
}

export function HTMLTemplateAttribute$2$reflection(gen0, gen1) {
    return record_type("Tempo.Html.HTMLTemplateAttribute`2", [gen0, gen1], HTMLTemplateAttribute$2, () => [["Name", string_type], ["Value", HTMLTemplateAttributeValue$2$reflection(gen0, gen1)]]);
}

export class HTMLTemplateAttributeValue$2 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["StringValue", "TriggerValue"];
    }
}

export function HTMLTemplateAttributeValue$2$reflection(gen0, gen1) {
    return union_type("Tempo.Html.HTMLTemplateAttributeValue`2", [gen0, gen1], HTMLTemplateAttributeValue$2, () => [[["Item", Value$2$reflection(gen0, option_type(string_type))]], [["Item", class_type("Tempo.Html.IHTMLTrigger`2", [gen0, gen1])]]]);
}

export class TriggerPayload$3 extends Record {
    constructor(State, Event$, Element$) {
        super();
        this.State = State;
        this.Event = Event$;
        this.Element = Element$;
    }
}

export function TriggerPayload$3$reflection(gen0, gen1, gen2) {
    return record_type("Tempo.Html.TriggerPayload`3", [gen0, gen1, gen2], TriggerPayload$3, () => [["State", gen0], ["Event", gen1], ["Element", gen2]]);
}

export class HTMLTrigger$4 {
    constructor(handler) {
        this.handler = handler;
    }
    Accept(f) {
        const this$ = this;
        return f.Invoke(this$);
    }
}

export function HTMLTrigger$4$reflection(gen0, gen1, gen2, gen3) {
    return class_type("Tempo.Html.HTMLTrigger`4", [gen0, gen1, gen2, gen3], HTMLTrigger$4);
}

export function HTMLTrigger$4_$ctor_75095B8B(handler) {
    return new HTMLTrigger$4(handler);
}

export function HTMLTrigger$4__get_Handler(this$) {
    return this$.handler;
}

export function packHTMLTrigger(trigger) {
    return trigger;
}

export function unpackHTMLTrigger(trigger, f) {
    return trigger.Accept(f);
}

export function makeTrigger(f) {
    return packHTMLTrigger(HTMLTrigger$4_$ctor_75095B8B(f));
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
        void unpackHTMLTrigger(domTrigger, {
            Invoke(t) {
                const el_1 = el;
                let copyOfStruct = el_1;
                copyOfStruct.addEventListener(name, (e) => {
                    dispatch(HTMLTrigger$4__get_Handler(t)(new TriggerPayload$3(state(), e, el_1)));
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

export class MakeHTMLRender$3 extends MakeRender$4 {
    constructor(dispatch) {
        super();
        this.dispatch = dispatch;
    }
    ["Tempo.Core.MakeRender`4.MakeNodeRender2B595"](node) {
        const this$ = this;
        if (node.tag === 1) {
            const v = node.fields[0];
            return MakeHTMLRender$3__MakeRenderDOMText_Z320284C0(this$, v);
        }
        else {
            const el = node.fields[0];
            return MakeHTMLRender$3__MakeRenderDOMElement_3B41954E(this$, el);
        }
    }
    ["Tempo.Core.MakeRender`4.CreateGroupNodeZ721C83C5"](label) {
        return HTMLGroupImpl_$ctor_Z721C83C5(label);
    }
}

export function MakeHTMLRender$3$reflection(gen0, gen1, gen2) {
    return class_type("Tempo.Html.MakeHTMLRender`3", [gen0, gen1, gen2], MakeHTMLRender$3, MakeRender$4$reflection(HTMLTemplateNode$3$reflection(gen0, gen1, gen2), gen0, gen1, gen2));
}

export function MakeHTMLRender$3_$ctor_Z6156FC82(dispatch) {
    return new MakeHTMLRender$3(dispatch);
}

export function MakeHTMLRender$3__MakeRenderDOMElement_3B41954E(this$, node) {
    return (parent) => ((state) => {
        let localState = state;
        const htmlImpl = HTMLElementImpl_$ctor_Z721C83C5(node.Name);
        const impl = htmlImpl;
        const getState = () => localState;
        iterate((arg30$0040) => {
            applyAttribute(this$.dispatch, htmlImpl.element, getState, arg30$0040);
        }, node.Attributes);
        parent.Append(impl);
        const childViews = map((child) => MakeRender$4__Make_1DCD9633(this$, child)(impl)(localState), node.Children);
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
        const attributeUpdates = map(mapCurriedArgs((f) => partialApply(1, f, [htmlImpl.element]), [[0, 2]]), filterMap((arg00$0040) => derivedApplication(arg00$0040), node.Attributes));
        const updates = append(attributeUpdates, childUpdates);
        const change_2 = (state_1) => {
            localState = state_1;
            iterate((change_1) => {
                change_1(localState);
            }, updates);
        };
        const destroy_2 = () => {
            parent.Remove(impl);
            iterate((destroy_1) => {
                destroy_1();
            }, childDestroys);
        };
        const query_2 = (q) => {
            iterate((query_1) => {
                query_1(q);
            }, childQueries);
        };
        return new View$2(impl, change_2, destroy_2, query_2);
    });
}

export function MakeHTMLRender$3__MakeRenderDOMText_Z320284C0(this$, value) {
    return (parent) => ((state) => {
        if (value.tag === 0) {
            const s = value.fields[0];
            const htmlImpl_1 = HTMLTextImpl_$ctor_Z721C83C5(s);
            const impl_1 = htmlImpl_1;
            parent.Append(impl_1);
            return new View$2(impl_1, (value_2) => {
            }, () => {
                parent.Remove(impl_1);
            }, (value_3) => {
            });
        }
        else {
            const f = value.fields[0];
            const htmlImpl = HTMLTextImpl_$ctor_Z721C83C5(f(state));
            const impl = htmlImpl;
            parent.Append(impl);
            return new View$2(impl, (state_1) => {
                htmlImpl.text.nodeValue = f(state_1);
            }, () => {
                parent.Remove(impl);
            }, (value_1) => {
            });
        }
    });
}

