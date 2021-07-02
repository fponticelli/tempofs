import { record_type, unit_type, lambda_type, bool_type, class_type } from "../../../src/.fable/fable-library.3.1.10/Reflection.js";
import { append, map, tail as tail_1, head as head_1, ofArray, fold, empty, filter, cons, reverse, collect, iterate, singleton } from "../../../src/.fable/fable-library.3.1.10/List.js";
import { interpolate, toText } from "../../../src/.fable/fable-library.3.1.10/String.js";
import { remove } from "./Html.Tools.fs.js";
import { HTMLLifecycle$5__get_Respond, HTMLLifecycle$5__get_BeforeDestroy, HTMLLifecycle$5__get_AfterChange, HTMLLifecyclePayload$4, HTMLLifecycle$5__get_BeforeChange, HTMLLifecycleInitialPayload$3, HTMLLifecycle$5__get_AfterRender, Property$2__get_Value, HTMLTemplateAttribute$3, HTMLLifecycle$5_$ctor_17DF349, HTMLTrigger$4_$ctor_75095B8B, TriggerPayload$3, HTMLTrigger$4__get_Handler } from "./Html.fs.js";
import { partialApply, mapCurriedArgs, stringHash, uncurry, equals } from "../../../src/.fable/fable-library.3.1.10/Util.js";
import { Record } from "../../../src/.fable/fable-library.3.1.10/Types.js";
import { View$2, Value$2_Combine_Z4D48493B, Value$2_Resolve } from "../Tempo.Core/Core.fs.js";
import { tryFind, ofList } from "../../../src/.fable/fable-library.3.1.10/Map.js";
import { map2 } from "../../../src/.fable/fable-library.3.1.10/Option.js";
import { filterMap } from "../Tempo.Core/Std.List.fs.js";
import { List_groupBy } from "../../../src/.fable/fable-library.3.1.10/Seq2.js";

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

export function HTMLElementImpl__SetAttribute_68C4AEB5(this$, name, value) {
    if (value == null) {
        this$.element.removeAttribute();
    }
    else {
        const s = value;
        this$.element.setAttribute(name, s);
    }
}

export function HTMLElementImpl__SetProperty_4A53169E(this$, name, value) {
    this$.element[name] = value;
}

export function HTMLElementImpl__SetHandler(this$, name, getState, dispatch, trigger) {
    this$.element.addEventListener(name, (e) => {
        dispatch(HTMLTrigger$4__get_Handler(trigger)(new TriggerPayload$3(getState(), e, this$.element)));
    });
}

export function HTMLElementImpl_$ctor_Z5966C024(el) {
    HTMLImpl_$ctor();
    return new HTMLElementImpl(el);
}

export function HTMLElementImpl_$ctor_Z721C83C5(name) {
    return HTMLElementImpl_$ctor_Z5966C024(document.createElement(name));
}

export function HTMLElementImpl_$ctor_Z384F8060(ns, name) {
    return HTMLElementImpl_$ctor_Z5966C024(document.createElementNS(ns, name));
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
            throw (new Error(toText(interpolate("HTMLGroupImpl doesn\u0027t know how to remove a child of type %P()", [child]))));
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

export function applyTrigger(name, domTrigger, impl, dispatch, getState) {
    void unpackHTMLTrigger(domTrigger, {
        Invoke(trigger) {
            HTMLElementImpl__SetHandler(impl, name, getState, dispatch, trigger);
            return 0;
        },
    });
}

export function applyProperty(name, prop, impl, state) {
    void unpackProperty(prop, {
        Invoke(prop_1) {
            HTMLElementImpl__SetProperty_4A53169E(impl, name, Value$2_Resolve(Property$2__get_Value(prop_1), state));
            return 0;
        },
    });
}

export function extractDerivedProperty(name, prop) {
    return unpackProperty(prop, {
        Invoke(prop_1) {
            const matchValue = Property$2__get_Value(prop_1);
            if (matchValue.tag === 0) {
                return void 0;
            }
            else {
                const f = matchValue.fields[0];
                return (impl) => ((state) => {
                    HTMLElementImpl__SetProperty_4A53169E(impl, name, f(state));
                });
            }
        },
    });
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

export const aggregatedAttributes = ofList(ofArray([["class", " "], ["style", "; "]]));

export function foldSelf(f, ls) {
    const head = head_1(ls);
    const tail = tail_1(ls);
    return fold(f, head, tail);
}

export function combineAttributes(name, va, vb) {
    const matchValue = tryFind(name, aggregatedAttributes);
    if (matchValue == null) {
        return va;
    }
    else {
        const sep = matchValue;
        const combiner = (a, b) => map2((a_1, b_1) => toText(interpolate("%P()%P()%P()", [a_1, sep, b_1])), a, b);
        return Value$2_Combine_Z4D48493B(combiner, va, vb);
    }
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
    const patternInput = fold(uncurry(2, (tupledArg) => {
        const attributes = tupledArg[0];
        const properties = tupledArg[1];
        const triggers = tupledArg[2];
        return (_arg1_1) => {
            const value = _arg1_1.Value;
            const name = _arg1_1.Name;
            switch (value.tag) {
                case 1: {
                    const v_1 = value.fields[0];
                    return [attributes, cons([name, v_1], properties), triggers];
                }
                case 2: {
                    const v_2 = value.fields[0];
                    return [attributes, properties, cons([name, v_2], triggers)];
                }
                default: {
                    const v = value.fields[0];
                    return [cons([name, v], attributes), properties, triggers];
                }
            }
        };
    }), [empty(), empty(), empty()], namedAttributes);
    const triggers_1 = patternInput[2];
    const properties_1 = patternInput[1];
    const attributes_1 = patternInput[0];
    const groupedAttributes = map((tupledArg_2) => {
        const name_2 = tupledArg_2[0];
        const ls = tupledArg_2[1];
        return [name_2, map((tupledArg_3) => {
            const v_3 = tupledArg_3[1];
            return v_3;
        }, ls)];
    }, List_groupBy((tupledArg_1) => {
        const name_1 = tupledArg_1[0];
        return name_1;
    }, attributes_1, {
        Equals: (x, y) => (x === y),
        GetHashCode: (x) => stringHash(x),
    }));
    const attributes_2 = map((tupledArg_4) => {
        const name_3 = tupledArg_4[0];
        const ls_1 = tupledArg_4[1];
        return [name_3, foldSelf((va, vb) => combineAttributes(name_3, va, vb), ls_1)];
    }, groupedAttributes);
    iterate((tupledArg_5) => {
        const name_4 = tupledArg_5[0];
        const value_1 = tupledArg_5[1];
        HTMLElementImpl__SetAttribute_68C4AEB5(htmlImpl, name_4, Value$2_Resolve(value_1, state));
    }, attributes_2);
    const attributeUpdates = map((tupledArg_7) => {
        const name_6 = tupledArg_7[0];
        const f_1 = tupledArg_7[1];
        return (s) => {
            HTMLElementImpl__SetAttribute_68C4AEB5(htmlImpl, name_6, f_1(s));
        };
    }, filterMap((tupledArg_6) => {
        const name_5 = tupledArg_6[0];
        const value_2 = tupledArg_6[1];
        if (value_2.tag === 0) {
            return void 0;
        }
        else {
            const f = value_2.fields[0];
            return [name_5, f];
        }
    }, attributes_2));
    iterate((tupledArg_8) => {
        const name_7 = tupledArg_8[0];
        const prop = tupledArg_8[1];
        applyProperty(name_7, prop, htmlImpl, state);
    }, properties_1);
    const propertyUpdates = map(mapCurriedArgs((f_2) => partialApply(1, f_2, [htmlImpl]), [[0, 2]]), filterMap((tupledArg_9) => {
        const name_8 = tupledArg_9[0];
        const prop_1 = tupledArg_9[1];
        return extractDerivedProperty(name_8, prop_1);
    }, properties_1));
    const callback = (tupledArg_10) => {
        const name_9 = tupledArg_10[0];
        const handler = tupledArg_10[1];
        applyTrigger(name_9, handler, htmlImpl, dispatch, getState);
    };
    iterate(callback, triggers_1);
    parent.Append(impl);
    const childViews = map((child) => make(child, impl, localState, dispatch), node.Children);
    const patternInput_1 = mergeLifecycles(filterMap((_arg2_1) => {
        if (_arg2_1.tag === 1) {
            const lc = _arg2_1.fields[0];
            return extractLifecycle(lc, dispatch)(htmlImpl.element)(state);
        }
        else {
            return void 0;
        }
    }, node.Attributes));
    const respond = patternInput_1.Respond;
    const beforeDestroy = patternInput_1.BeforeDestroy;
    const beforeChange = patternInput_1.BeforeChange;
    const afterChange = patternInput_1.AfterChange;
    const childUpdates = map((_arg4) => {
        const change = _arg4.Change;
        return change;
    }, childViews);
    const childDestroys = map((_arg5) => {
        const destroy = _arg5.Destroy;
        return destroy;
    }, childViews);
    const childQueries = map((_arg6) => {
        const query = _arg6.Query;
        return query;
    }, childViews);
    const updates = append(attributeUpdates, append(propertyUpdates, childUpdates));
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

