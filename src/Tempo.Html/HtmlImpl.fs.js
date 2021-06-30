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
        const s_1 = s;
        el.setAttribute(name, s_1);
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
            if (matchValue.tag === 0) {
                return void 0;
            }
            else {
                const f = matchValue.fields[0];
                return (el) => ((state) => {
                    el[Property$2__get_Name(prop_1)] = f(state);
                });
            }
        },
    });
}

export function derivedApplication(_arg1) {
    const value = _arg1.Value;
    const name = _arg1.Name;
    if (value.tag === 1) {
        const prop = value.fields[0];
        return extractDerivedProperty(prop);
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
            const prop = value.fields[0];
            applyProperty(prop, el, state());
            break;
        }
        case 2: {
            const domTrigger = value.fields[0];
            applyTrigger(domTrigger, name, el, dispatch, state);
            break;
        }
        default: {
            const v = value.fields[0];
            applyStringAttribute(name, el, Value$2_Resolve(v, state()));
        }
    }
}

export function extractLifecycle(lc, dispatch) {
    return unpackHTMLLifecycle(lc, {
        Invoke(t) {
            return (el) => ((state) => {
                let payload = HTMLLifecycle$5__get_AfterRender(t)(new HTMLLifecycleInitialPayload$3(state, el, dispatch));
                const beforeChange = (state_1) => {
                    const patternInput = HTMLLifecycle$5__get_BeforeChange(t)(new HTMLLifecyclePayload$4(state_1, el, payload, dispatch));
                    const result = patternInput[0];
                    const newPayload = patternInput[1];
                    payload = newPayload;
                    return result;
                };
                const afterChange = (state_2) => {
                    payload = HTMLLifecycle$5__get_AfterChange(t)(new HTMLLifecyclePayload$4(state_2, el, payload, dispatch));
                };
                const beforeDestroy = () => {
                    HTMLLifecycle$5__get_BeforeDestroy(t)(new HTMLLifecyclePayload$4(state, el, payload, dispatch));
                };
                const respond = (query) => {
                    payload = HTMLLifecycle$5__get_Respond(t)(query)(new HTMLLifecyclePayload$4(state, el, payload, dispatch));
                };
                return new LifecycleImpl$3(beforeChange, afterChange, beforeDestroy, dispatch, respond);
            });
        },
    });
}

export function mergeLifecycles(ls) {
    const merge = (a, b) => (new LifecycleImpl$3((s) => {
        const ra = a.BeforeChange(s);
        const rb = b.BeforeChange(s);
        if (ra) {
            return true;
        }
        else {
            return rb;
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
    }));
    const start = new LifecycleImpl$3((_arg1) => true, (value) => {
    }, () => {
    }, (value_3) => {
    }, (value_2) => {
    });
    return fold(merge, start, ls);
}

export function createGroupNode(label) {
    return HTMLGroupImpl_$ctor_Z721C83C5(label);
}

export function makeHTMLNodeRender(make, node) {
    switch (node.tag) {
        case 1: {
            const el_1 = node.fields[0];
            const make_2 = make;
            return (parent_1) => ((state_1) => ((dispatch_1) => makeRenderDOMElement(void 0, el_1, make_2, parent_1, state_1, dispatch_1)));
        }
        case 2: {
            const v = node.fields[0];
            return (parent_2) => ((state_2) => ((dispatch_2) => makeRenderDOMText(v, parent_2, state_2, dispatch_2)));
        }
        default: {
            const ns = node.fields[0];
            const el = node.fields[1];
            const make_1 = make;
            return (parent) => ((state) => ((dispatch) => makeRenderDOMElement(ns, el, make_1, parent, state, dispatch)));
        }
    }
}

export function makeRenderDOMElement(ns, node, make, parent, state, dispatch) {
    let localState = state;
    let htmlImpl;
    if (ns == null) {
        htmlImpl = HTMLElementImpl_$ctor_Z721C83C5(node.Name);
    }
    else {
        const ns_1 = ns;
        htmlImpl = HTMLElementImpl_$ctor_Z384F8060(ns_1, node.Name);
    }
    const impl = htmlImpl;
    const getState = () => localState;
    const namedAttributes = filterMap((_arg1) => {
        if (_arg1.tag === 0) {
            const at = _arg1.fields[0];
            return at;
        }
        else {
            return void 0;
        }
    }, node.Attributes);
    iterate((arg30$0040) => {
        applyAttribute(dispatch, htmlImpl.element, getState, arg30$0040);
    }, namedAttributes);
    parent.Append(impl);
    const childViews = map((child) => make(child, impl, localState, dispatch), node.Children);
    const patternInput = mergeLifecycles(filterMap((_arg2) => {
        if (_arg2.tag === 1) {
            const lc = _arg2.fields[0];
            return extractLifecycle(lc, dispatch)(htmlImpl.element)(state);
        }
        else {
            return void 0;
        }
    }, node.Attributes));
    const respond = patternInput.Respond;
    const beforeDestroy = patternInput.BeforeDestroy;
    const beforeChange = patternInput.BeforeChange;
    const afterChange = patternInput.AfterChange;
    const childUpdates = map((_arg1_1) => {
        const change = _arg1_1.Change;
        return change;
    }, childViews);
    const childDestroys = map((_arg2_1) => {
        const destroy = _arg2_1.Destroy;
        return destroy;
    }, childViews);
    const childQueries = map((_arg3) => {
        const query = _arg3.Query;
        return query;
    }, childViews);
    const attributeUpdates = map(mapCurriedArgs((f_1) => partialApply(1, f_1, [htmlImpl.element]), [[0, 2]]), filterMap((arg00$0040) => derivedApplication(arg00$0040), namedAttributes));
    const updates = append(attributeUpdates, childUpdates);
    const change_2 = (state_1) => {
        if (beforeChange(state_1)) {
            localState = state_1;
            iterate((change_1) => {
                change_1(localState);
            }, updates);
            afterChange(localState);
        }
    };
    const destroy_2 = () => {
        beforeDestroy();
        parent.Remove(impl);
        iterate((destroy_1) => {
            destroy_1();
        }, childDestroys);
    };
    const query_2 = (q) => {
        iterate((query_1) => {
            query_1(q);
        }, childQueries);
        respond(q);
    };
    return new View$2(impl, change_2, destroy_2, query_2);
}

export function makeRenderDOMText(value, parent, state, dispatch) {
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
}

