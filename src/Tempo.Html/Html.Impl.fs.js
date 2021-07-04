import { record_type, unit_type, lambda_type, bool_type, class_type } from "../Tempo.Demo/.fable/fable-library.3.1.10/Reflection.js";
import { append, map, tail as tail_1, head as head_1, ofArray, fold, empty, filter, cons, reverse, collect, iterate, singleton } from "../Tempo.Demo/.fable/fable-library.3.1.10/List.js";
import { interpolate, toText } from "../Tempo.Demo/.fable/fable-library.3.1.10/String.js";
import { remove } from "./Html.Tools.fs.js";
import { HTMLLifecycle$5__get_Respond, HTMLLifecycle$5__get_BeforeDestroy, HTMLLifecycle$5__get_AfterChange, HTMLLifecyclePayload$4, HTMLLifecycle$5__get_BeforeChange, HTMLLifecycleInitialPayload$3, HTMLLifecycle$5__get_AfterRender, Property$2__get_Value, HTMLTemplateAttribute$3, HTMLLifecycle$5_$ctor_17DF349, HTMLTrigger$4_$ctor_75095B8B, TriggerPayload$3, HTMLTrigger$4__get_Handler } from "./Html.fs.js";
import { partialApply, mapCurriedArgs, stringHash, uncurry, equals } from "../Tempo.Demo/.fable/fable-library.3.1.10/Util.js";
import { Record } from "../Tempo.Demo/.fable/fable-library.3.1.10/Types.js";
import { View$2, Value$2_Combine_Z4D48493B, Value$2_Resolve } from "../Tempo.Core/Core.fs.js";
import { tryFind, ofList } from "../Tempo.Demo/.fable/fable-library.3.1.10/Map.js";
import { map2 } from "../Tempo.Demo/.fable/fable-library.3.1.10/Option.js";
import { filterMap } from "../Tempo.Core/Std.List.fs.js";
import { List_groupBy } from "../Tempo.Demo/.fable/fable-library.3.1.10/Seq2.js";

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

export function HTMLElementImpl__SetAttribute_68C4AEB5(this$, name, value) {
    if (value == null) {
        this$.element.removeAttribute();
    }
    else {
        this$.element.setAttribute(name, value);
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
            throw (new Error(toText(interpolate("HTMLGroupImpl doesn\u0027t know how to remove a child of type %P()", [child]))));
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
            return (matchValue.tag === 0) ? (void 0) : ((impl) => ((state) => {
                HTMLElementImpl__SetProperty_4A53169E(impl, name, matchValue.fields[0](state));
            }));
        },
    });
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

export const aggregatedAttributes = ofList(ofArray([["class", " "], ["style", "; "]]));

export function foldSelf(f, ls) {
    return fold(f, head_1(ls), tail_1(ls));
}

export function combineAttributes(name, va, vb) {
    const matchValue = tryFind(name, aggregatedAttributes);
    if (matchValue == null) {
        return va;
    }
    else {
        const sep = matchValue;
        return Value$2_Combine_Z4D48493B((a, b) => map2((a_1, b_1) => toText(interpolate("%P()%P()%P()", [a_1, sep, b_1])), a, b), va, vb);
    }
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
    const patternInput = fold(uncurry(2, (tupledArg) => {
        const attributes = tupledArg[0];
        const properties = tupledArg[1];
        const triggers = tupledArg[2];
        return (_arg1_1) => {
            const value = _arg1_1.Value;
            const name = _arg1_1.Name;
            switch (value.tag) {
                case 1: {
                    return [attributes, cons([name, value.fields[0]], properties), triggers];
                }
                case 2: {
                    return [attributes, properties, cons([name, value.fields[0]], triggers)];
                }
                default: {
                    return [cons([name, value.fields[0]], attributes), properties, triggers];
                }
            }
        };
    }), [empty(), empty(), empty()], filterMap((_arg1) => ((_arg1.tag === 0) ? _arg1.fields[0] : (void 0)), node.Attributes));
    const properties_1 = patternInput[1];
    const attributes_2 = map((tupledArg_4) => {
        const name_3 = tupledArg_4[0];
        return [name_3, foldSelf((va, vb) => combineAttributes(name_3, va, vb), tupledArg_4[1])];
    }, map((tupledArg_2) => [tupledArg_2[0], map((tupledArg_3) => tupledArg_3[1], tupledArg_2[1])], List_groupBy((tupledArg_1) => tupledArg_1[0], patternInput[0], {
        Equals: (x, y) => (x === y),
        GetHashCode: (x) => stringHash(x),
    })));
    iterate((tupledArg_5) => {
        HTMLElementImpl__SetAttribute_68C4AEB5(htmlImpl, tupledArg_5[0], Value$2_Resolve(tupledArg_5[1], state));
    }, attributes_2);
    const attributeUpdates = map((tupledArg_7) => ((s) => {
        HTMLElementImpl__SetAttribute_68C4AEB5(htmlImpl, tupledArg_7[0], tupledArg_7[1](s));
    }), filterMap((tupledArg_6) => {
        const value_2 = tupledArg_6[1];
        return (value_2.tag === 0) ? (void 0) : [tupledArg_6[0], value_2.fields[0]];
    }, attributes_2));
    iterate((tupledArg_8) => {
        applyProperty(tupledArg_8[0], tupledArg_8[1], htmlImpl, state);
    }, properties_1);
    const propertyUpdates = map(mapCurriedArgs((f_2) => partialApply(1, f_2, [htmlImpl]), [[0, 2]]), filterMap((tupledArg_9) => extractDerivedProperty(tupledArg_9[0], tupledArg_9[1]), properties_1));
    iterate((tupledArg_10) => {
        applyTrigger(tupledArg_10[0], tupledArg_10[1], htmlImpl, dispatch, () => localState);
    }, patternInput[2]);
    parent.Append(impl);
    const childViews = map((child) => make(child, impl, localState, dispatch), node.Children);
    const patternInput_1 = mergeLifecycles(filterMap((_arg2_1) => ((_arg2_1.tag === 1) ? extractLifecycle(_arg2_1.fields[0], dispatch)(htmlImpl.element)(state) : (void 0)), node.Attributes));
    const childUpdates = map((_arg4) => _arg4.Change, childViews);
    const childDestroys = map((_arg5) => _arg5.Destroy, childViews);
    const childQueries = map((_arg6) => _arg6.Query, childViews);
    const updates = append(attributeUpdates, append(propertyUpdates, childUpdates));
    return new View$2(impl, (state_1) => {
        if (patternInput_1.BeforeChange(state_1)) {
            localState = state_1;
            iterate((change_1) => {
                change_1(localState);
            }, updates);
            patternInput_1.AfterChange(localState);
        }
    }, () => {
        patternInput_1.BeforeDestroy();
        parent.Remove(impl);
        iterate((destroy_1) => {
            destroy_1();
        }, childDestroys);
    }, (q) => {
        iterate((query_1) => {
            query_1(q);
        }, childQueries);
        patternInput_1.Respond(q);
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

