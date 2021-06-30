import { record_type, unit_type, lambda_type, bool_type, class_type } from "../Tempo.Demo/.fable/fable-library.3.1.10/Reflection.js";
import { append, map, fold, empty, filter, cons, reverse, collect, iterate, singleton } from "../Tempo.Demo/.fable/fable-library.3.1.10/List.js";
import { interpolate, toText } from "../Tempo.Demo/.fable/fable-library.3.1.10/String.js";
import { remove } from "./HtmlTools.fs.js";
import { partialApply, mapCurriedArgs, equals } from "../Tempo.Demo/.fable/fable-library.3.1.10/Util.js";
import { Record } from "../Tempo.Demo/.fable/fable-library.3.1.10/Types.js";
import { HTMLLifecycle$5__get_Respond, HTMLLifecycle$5__get_BeforeDestroy, HTMLLifecycle$5__get_AfterChange, HTMLLifecyclePayload$4, HTMLLifecycle$5__get_BeforeChange, HTMLLifecycleInitialPayload$3, HTMLLifecycle$5__get_AfterRender, Property$2__get_Name, Property$2__get_Value, TriggerPayload$3, HTMLTrigger$4__get_Handler, HTMLTemplateAttribute$3, HTMLLifecycle$5_$ctor_17DF349, HTMLTrigger$4_$ctor_75095B8B } from "./Html.fs.js";
import { View$2, Value$2_Resolve } from "../Tempo.Core/Core.fs.js";
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
            iterate((arg) => {
                void this$.element.appendChild(arg);
            }, (child)["Tempo.Html.Impl.HTMLImpl.GetNodes"]());
        }
        else {
            throw (new Error(toText(interpolate("HTMLElementImpl doesn\u0027t know how to append a child of type %P()", [child]))));
        }
    }
    Remove(child) {
        if (child["GetNodes"] !== null) {
            iterate((n) => {
                remove(n);
            }, (child)["Tempo.Html.Impl.HTMLImpl.GetNodes"]());
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

export function HTMLElementImpl_$ctor_Z384F8060(ns, name) {
    return HTMLElementImpl_$ctor_4C3D2741(document.createElementNS(ns, name));
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
                return (child)["Tempo.Html.Impl.HTMLImpl.GetNodes"]();
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
            const parent = this$.ref.parentNode;
            iterate((node) => {
                void parent.insertBefore(node, this$.ref);
            }, (child)["Tempo.Html.Impl.HTMLImpl.GetNodes"]());
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
            iterate((n) => {
                remove(n);
            }, htmlChild["Tempo.Html.Impl.HTMLImpl.GetNodes"]());
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
    HTMLImpl_$ctor();
    return new HTMLGroupImpl(document.createTextNode(""), empty());
}

export class LifecycleImpl$3 extends Record {
    constructor(BeforeChange, AfterChange, BeforeDestroy, Dispatch, Respond) {
        super();
        this.BeforeChange = BeforeChange;
        this.AfterChange = AfterChange;
        this.BeforeDestroy = BeforeDestroy;
        this.Dispatch = Dispatch;
        this.Respond = Respond;
    }
}

export function LifecycleImpl$3$reflection(gen0, gen1, gen2) {
    return record_type("Tempo.Html.Impl.LifecycleImpl`3", [gen0, gen1, gen2], LifecycleImpl$3, () => [["BeforeChange", lambda_type(gen0, bool_type)], ["AfterChange", lambda_type(gen0, unit_type)], ["BeforeDestroy", lambda_type(unit_type, unit_type)], ["Dispatch", lambda_type(gen1, unit_type)], ["Respond", lambda_type(gen2, unit_type)]]);
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

export function packProperty(trigger) {
    return trigger;
}

export function unpackProperty(trigger, f) {
    return trigger.Accept(f);
}

export function packHTMLLifecycle(lifecycle) {
    return lifecycle;
}

export function unpackHTMLLifecycle(lifecycle, f) {
    return lifecycle.Accept(f);
}

export function makeLifecycle(afterRender, beforeChange, afterChange, beforeDestroy, respond) {
    return packHTMLLifecycle(HTMLLifecycle$5_$ctor_17DF349(afterRender, beforeChange, afterChange, beforeDestroy, respond));
}

export function lifecycleAttribute(afterRender, beforeChange, afterChange, beforeDestroy, respond) {
    return new HTMLTemplateAttribute$3(1, makeLifecycle(afterRender, beforeChange, afterChange, beforeDestroy, respond));
}

export function applyStringAttribute(name, el, s) {
    if (s == null) {
        el.removeAttribute(name);
    }
    else {
        el.setAttribute(name, s);
    }
}

export function applyTrigger(domTrigger, name, el, dispatch, state) {
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

export function applyProperty(prop, el, state) {
    void unpackProperty(prop, {
        Invoke(prop_1) {
            const v = Value$2_Resolve(Property$2__get_Value(prop_1), state);
            const prop_2 = Property$2__get_Name(prop_1);
            el[prop_2] = v;
            return 0;
        },
    });
}

export function extractDerivedProperty(prop) {
    return unpackProperty(prop, {
        Invoke(prop_1) {
            const matchValue = Property$2__get_Value(prop_1);
            return (matchValue.tag === 0) ? (void 0) : ((el) => ((state) => {
                el[Property$2__get_Name(prop_1)] = matchValue.fields[0](state);
            }));
        },
    });
}

export function derivedApplication(_arg1) {
    const value = _arg1.Value;
    if (value.tag === 1) {
        return extractDerivedProperty(value.fields[0]);
    }
    else if (value.tag === 2) {
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
    switch (value.tag) {
        case 1: {
            applyProperty(value.fields[0], el, state());
            break;
        }
        case 2: {
            applyTrigger(value.fields[0], name, el, dispatch, state);
            break;
        }
        default: {
            applyStringAttribute(name, el, Value$2_Resolve(value.fields[0], state()));
        }
    }
}

export function extractLifecycle(lc, dispatch) {
    return unpackHTMLLifecycle(lc, {
        Invoke(t) {
            return (el) => ((state) => {
                let payload = HTMLLifecycle$5__get_AfterRender(t)(new HTMLLifecycleInitialPayload$3(state, el, dispatch));
                return new LifecycleImpl$3((state_1) => {
                    const patternInput = HTMLLifecycle$5__get_BeforeChange(t)(new HTMLLifecyclePayload$4(state_1, el, payload, dispatch));
                    payload = patternInput[1];
                    return patternInput[0];
                }, (state_2) => {
                    payload = HTMLLifecycle$5__get_AfterChange(t)(new HTMLLifecyclePayload$4(state_2, el, payload, dispatch));
                }, () => {
                    HTMLLifecycle$5__get_BeforeDestroy(t)(new HTMLLifecyclePayload$4(state, el, payload, dispatch));
                }, dispatch, (query) => {
                    payload = HTMLLifecycle$5__get_Respond(t)(query)(new HTMLLifecyclePayload$4(state, el, payload, dispatch));
                });
            });
        },
    });
}

export function mergeLifecycles(ls) {
    return fold((a, b) => (new LifecycleImpl$3((s) => {
        if (a.BeforeChange(s)) {
            return true;
        }
        else {
            return b.BeforeChange(s);
        }
    }, (s_1) => {
        a.AfterChange(s_1);
        b.AfterChange(s_1);
    }, () => {
        a.BeforeDestroy();
        b.BeforeDestroy();
    }, (v) => {
        a.Dispatch(v);
        b.Dispatch(v);
    }, (q) => {
        a.Respond(q);
        b.Respond(q);
    })), new LifecycleImpl$3((_arg1) => true, (value) => {
    }, () => {
    }, (value_3) => {
    }, (value_2) => {
    }), ls);
}

export function createGroupNode(label) {
    return HTMLGroupImpl_$ctor_Z721C83C5(label);
}

export function makeHTMLNodeRender(make, node) {
    switch (node.tag) {
        case 1: {
            const make_2 = make;
            return (parent_1) => ((state_1) => ((dispatch_1) => makeRenderDOMElement(void 0, node.fields[0], make_2, parent_1, state_1, dispatch_1)));
        }
        case 2: {
            return (parent_2) => ((state_2) => ((dispatch_2) => makeRenderDOMText(node.fields[0], parent_2, state_2, dispatch_2)));
        }
        default: {
            const make_1 = make;
            return (parent) => ((state) => ((dispatch) => makeRenderDOMElement(node.fields[0], node.fields[1], make_1, parent, state, dispatch)));
        }
    }
}

export function makeRenderDOMElement(ns, node, make, parent, state, dispatch) {
    let localState = state;
    const htmlImpl = (ns == null) ? HTMLElementImpl_$ctor_Z721C83C5(node.Name) : HTMLElementImpl_$ctor_Z384F8060(ns, node.Name);
    const impl = htmlImpl;
    const namedAttributes = filterMap((_arg1) => ((_arg1.tag === 0) ? _arg1.fields[0] : (void 0)), node.Attributes);
    iterate((arg30$0040) => {
        applyAttribute(dispatch, htmlImpl.element, () => localState, arg30$0040);
    }, namedAttributes);
    parent.Append(impl);
    const childViews = map((child) => make(child, impl, localState, dispatch), node.Children);
    const patternInput = mergeLifecycles(filterMap((_arg2) => ((_arg2.tag === 1) ? extractLifecycle(_arg2.fields[0], dispatch)(htmlImpl.element)(state) : (void 0)), node.Attributes));
    const childUpdates = map((_arg1_1) => _arg1_1.Change, childViews);
    const childDestroys = map((_arg2_1) => _arg2_1.Destroy, childViews);
    const childQueries = map((_arg3) => _arg3.Query, childViews);
    const updates = append(map(mapCurriedArgs((f_1) => partialApply(1, f_1, [htmlImpl.element]), [[0, 2]]), filterMap((arg00$0040) => derivedApplication(arg00$0040), namedAttributes)), childUpdates);
    return new View$2(impl, (state_1) => {
        if (patternInput.BeforeChange(state_1)) {
            localState = state_1;
            iterate((change_1) => {
                change_1(localState);
            }, updates);
            patternInput.AfterChange(localState);
        }
    }, () => {
        patternInput.BeforeDestroy();
        parent.Remove(impl);
        iterate((destroy_1) => {
            destroy_1();
        }, childDestroys);
    }, (q) => {
        iterate((query_1) => {
            query_1(q);
        }, childQueries);
        patternInput.Respond(q);
    });
}

export function makeRenderDOMText(value, parent, state, dispatch) {
    if (value.tag === 0) {
        const impl_1 = HTMLTextImpl_$ctor_Z721C83C5(value.fields[0]);
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
}

