import { class_type } from "../Tempo.Demo/.fable/fable-library.3.1.10/Reflection.js";
import { append, map, empty, filter, cons, reverse, collect, iterate, singleton } from "../Tempo.Demo/.fable/fable-library.3.1.10/List.js";
import { interpolate, toText } from "../Tempo.Demo/.fable/fable-library.3.1.10/String.js";
import { remove } from "./HtmlTools.fs.js";
import { partialApply, mapCurriedArgs, equals } from "../Tempo.Demo/.fable/fable-library.3.1.10/Util.js";
import { HTMLTemplateNode$3$reflection, TriggerPayload$3, HTMLTrigger$4__get_Handler, HTMLTrigger$4_$ctor_75095B8B } from "./Html.fs.js";
import { View$2, MakeRender$4__Make_1DCD9633, MakeRender$4$reflection, MakeRender$4, Value$2_Resolve } from "../Tempo.Core/Core.fs.js";
import { filterMap } from "../Tempo.Core/ListExtra.fs.js";

export class HTMLImpl {
    constructor() {
    }
}

export function HTMLImpl$reflection() {
    return class_type("Tempo.Html.Impl.HTMLImpl", void 0, HTMLImpl);
}

export function HTMLImpl_$ctor() {
    return new HTMLImpl();
}

export class HTMLElementImpl {
    constructor(element) {
        this.element = element;
    }
    ["Tempo.Html.Impl.HTMLImpl.GetNodes"]() {
        const this$ = this;
        return singleton(this$.element);
    }
    Append(child) {
        const this$ = this;
        if (child["GetNodes"] !== null) {
            const child_1 = child;
            const nodes = child_1["Tempo.Html.Impl.HTMLImpl.GetNodes"]();
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
            const ls = child_1["Tempo.Html.Impl.HTMLImpl.GetNodes"]();
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
    return class_type("Tempo.Html.Impl.HTMLElementImpl", void 0, HTMLElementImpl, HTMLImpl$reflection());
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
    ["Tempo.Html.Impl.HTMLImpl.GetNodes"]() {
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
    return class_type("Tempo.Html.Impl.HTMLTextImpl", void 0, HTMLTextImpl, HTMLImpl$reflection());
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
    ["Tempo.Html.Impl.HTMLImpl.GetNodes"]() {
        const this$ = this;
        return cons(this$.ref, collect((child) => {
            if (child["GetNodes"] !== null) {
                const child_1 = child;
                return child_1["Tempo.Html.Impl.HTMLImpl.GetNodes"]();
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
            const nodes = child_1["Tempo.Html.Impl.HTMLImpl.GetNodes"]();
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
            const ls = htmlChild["Tempo.Html.Impl.HTMLImpl.GetNodes"]();
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
    return class_type("Tempo.Html.Impl.HTMLGroupImpl", void 0, HTMLGroupImpl, HTMLImpl$reflection());
}

export function HTMLGroupImpl_$ctor_Z721C83C5(label) {
    counter = (counter + 1);
    HTMLImpl_$ctor();
    return new HTMLGroupImpl(document.createComment(toText(interpolate("%P(): %P()", [label, counter]))), empty());
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
    else if (value.tag === 2) {
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
    switch (value.tag) {
        case 1: {
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
            break;
        }
        case 2: {
            const lc = value.fields[0];
            throw (new Error("ainono"));
            break;
        }
        default: {
            const v = value.fields[0];
            applyStringAttribute(name, el, Value$2_Resolve(v, state()));
        }
    }
}

export class MakeHTMLRender$3 extends MakeRender$4 {
    constructor() {
        super();
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
    return class_type("Tempo.Html.Impl.MakeHTMLRender`3", [gen0, gen1, gen2], MakeHTMLRender$3, MakeRender$4$reflection(HTMLTemplateNode$3$reflection(gen0, gen1, gen2), gen0, gen1, gen2));
}

export function MakeHTMLRender$3_$ctor() {
    return new MakeHTMLRender$3();
}

export function MakeHTMLRender$3__MakeRenderDOMElement_3B41954E(this$, node) {
    return (parent) => ((state) => ((dispatch) => {
        let localState = state;
        const htmlImpl = HTMLElementImpl_$ctor_Z721C83C5(node.Name);
        const impl = htmlImpl;
        const getState = () => localState;
        iterate((arg30$0040) => {
            applyAttribute(dispatch, htmlImpl.element, getState, arg30$0040);
        }, node.Attributes);
        parent.Append(impl);
        const childViews = map((child) => MakeRender$4__Make_1DCD9633(this$, child)(impl)(localState)(dispatch), node.Children);
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
    }));
}

export function MakeHTMLRender$3__MakeRenderDOMText_Z320284C0(this$, value) {
    return (parent) => ((state) => ((dispatch) => {
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
    }));
}

