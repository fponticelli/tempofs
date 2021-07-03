import { class_type } from "../../../src/.fable/fable-library.3.1.10/Reflection.js";
import { transform, comp, makeCaptureAction, makeCaptureState, makeCaptureSA, lifecycle, iterator, OneOf2$8_$ctor_Z4F5F76C, packOneOf2, Value$2, Template$4, ComponentView$3, MiddlewarePayload$3, MakeRender$4__Make_1DCD9633, MakeRender$4_$ctor_Z4E96F168 } from "../Tempo.Core/Core.fs.js";
import { uncurry } from "../../../src/.fable/fable-library.3.1.10/Util.js";
import { makeTrigger, packProperty, HTMLElementImpl_$ctor_Z5966C024, createGroupNode, makeHTMLNodeRender } from "./Html.Impl.fs.js";
import { Property$2_$ctor_1D5210CF, HTMLNamedAttribute$3, HTMLTemplateAttributeValue$3, HTMLTemplateAttribute$3, HTMLTemplateElement$3, HTMLTemplateNode$3 } from "./Html.fs.js";
import { some } from "../../../src/.fable/fable-library.3.1.10/Option.js";
import { FSharpChoice$2 } from "../../../src/.fable/fable-library.3.1.10/Choice.js";
import { empty } from "../../../src/.fable/fable-library.3.1.10/List.js";

export class DSL {
    constructor() {
    }
}

export function DSL$reflection() {
    return class_type("Tempo.Html.DSL", void 0, DSL);
}

function DSL_MakeRender() {
    return MakeRender$4_$ctor_Z4E96F168(uncurry(5, (make) => ((node) => makeHTMLNodeRender(make, node))), (label) => createGroupNode(label));
}

export function DSL_MakeProgram_Z9447D8C(template, el) {
    const renderInstance = DSL_MakeRender();
    const f = MakeRender$4__Make_1DCD9633(renderInstance, template);
    const parent = HTMLElementImpl_$ctor_Z5966C024(el);
    let render;
    const clo1 = f(parent);
    render = ((arg10) => {
        const clo2 = clo1(arg10);
        return (arg20) => clo2(arg20);
    });
    return (update) => ((middleware) => ((state) => {
        let localState = state;
        const dispatch = (action) => {
            const newState = update(localState, action);
            view.Change(newState);
            middleware(new MiddlewarePayload$3(newState, localState, action, dispatch, view.Query));
            localState = newState;
        };
        const view = render(localState)(dispatch);
        return new ComponentView$3(view.Impl, dispatch, view.Change, view.Destroy, view.Query);
    }));
}

export function DSL_MakeProgramOnContentLoaded_56DDD9AE(template, selector, f) {
    return (update) => ((middleware) => ((state) => {
        const value = window.addEventListener("DOMContentLoaded", (_arg1) => {
            const el = document.querySelector(selector);
            const render = DSL_MakeProgram_Z9447D8C(template, el);
            f(render(update)(middleware)(state));
        });
    }));
}

export function DSL_NSEl_7639458A(ns, name, attributes, children) {
    return new Template$4(0, new HTMLTemplateNode$3(0, ns, new HTMLTemplateElement$3(name, attributes, children)));
}

export function DSL_El_Z7374416F(name, attributes, children) {
    return new Template$4(0, new HTMLTemplateNode$3(1, new HTMLTemplateElement$3(name, attributes, children)));
}

export function DSL_Text() {
    return new Template$4(0, new HTMLTemplateNode$3(2, new Value$2(1, (x) => x)));
}

export function DSL_Text_Z721C83C5(value) {
    return new Template$4(0, new HTMLTemplateNode$3(2, new Value$2(0, value)));
}

export function DSL_Text_77A7E8C8(f) {
    return new Template$4(0, new HTMLTemplateNode$3(2, new Value$2(1, f)));
}

export function DSL_Attr_68C4AEB5(name, value) {
    return new HTMLTemplateAttribute$3(0, new HTMLNamedAttribute$3(name, new HTMLTemplateAttributeValue$3(0, new Value$2(0, value))));
}

export function DSL_Attr_Z384F8060(name, value) {
    return new HTMLTemplateAttribute$3(0, new HTMLNamedAttribute$3(name, new HTMLTemplateAttributeValue$3(0, new Value$2(0, value))));
}

export function DSL_Attr_Z3A5D29FA(name, f) {
    return new HTMLTemplateAttribute$3(0, new HTMLNamedAttribute$3(name, new HTMLTemplateAttributeValue$3(0, new Value$2(1, f))));
}

export function DSL_Attr_3DF4EB53(name, f) {
    return new HTMLTemplateAttribute$3(0, new HTMLNamedAttribute$3(name, new HTMLTemplateAttributeValue$3(0, new Value$2(1, (arg) => f(arg)))));
}

export function DSL_Attr_Z55EFCE8F(name, value) {
    return new HTMLTemplateAttribute$3(0, new HTMLNamedAttribute$3(name, new HTMLTemplateAttributeValue$3(0, new Value$2(0, value ? name : (void 0)))));
}

export function DSL_Attr_Z6A312DE(name, f) {
    return new HTMLTemplateAttribute$3(0, new HTMLNamedAttribute$3(name, new HTMLTemplateAttributeValue$3(0, new Value$2(1, (s) => (f(s) ? name : (void 0))))));
}

export function DSL_Attr_30230F9B(name, whenTrue, whenFalse) {
    return DSL_Attr_3DF4EB53(name, (b) => (b ? whenTrue : whenFalse));
}

export function DSL_Prop_4A53169E(name, value) {
    return new HTMLTemplateAttribute$3(0, new HTMLNamedAttribute$3(name, new HTMLTemplateAttributeValue$3(1, packProperty(Property$2_$ctor_1D5210CF(new Value$2(0, value))))));
}

export function DSL_Prop_36180E4D(name, f) {
    return new HTMLTemplateAttribute$3(0, new HTMLNamedAttribute$3(name, new HTMLTemplateAttributeValue$3(1, packProperty(Property$2_$ctor_1D5210CF(new Value$2(1, (arg) => some(f(arg))))))));
}

export function DSL_On_47AABEE2(name, handler) {
    return new HTMLTemplateAttribute$3(0, new HTMLNamedAttribute$3(name, new HTMLTemplateAttributeValue$3(2, makeTrigger(handler))));
}

export function DSL_OneOf_Z491B0F3C(f, template1, template2) {
    return new Template$4(3, packOneOf2(OneOf2$8_$ctor_Z4F5F76C(f, template1, template2)));
}

export function DSL_OneOf_Z2AFE4804(f, template1, template2, template3) {
    return DSL_OneOf_Z491B0F3C((s) => {
        const matchValue = f(s);
        switch (matchValue.tag) {
            case 1: {
                const c_1 = matchValue.fields[0];
                return new FSharpChoice$2(1, new FSharpChoice$2(0, c_1));
            }
            case 2: {
                const c_2 = matchValue.fields[0];
                return new FSharpChoice$2(1, new FSharpChoice$2(1, c_2));
            }
            default: {
                const c = matchValue.fields[0];
                return new FSharpChoice$2(0, c);
            }
        }
    }, template1, DSL_OneOf_Z491B0F3C((x) => x, template2, template3));
}

export function DSL_OneOf_Z1F5D2DDE(f, template1, template2, template3, template4) {
    return DSL_OneOf_Z491B0F3C((s) => {
        const matchValue = f(s);
        switch (matchValue.tag) {
            case 1: {
                const c_1 = matchValue.fields[0];
                return new FSharpChoice$2(0, new FSharpChoice$2(1, c_1));
            }
            case 2: {
                const c_2 = matchValue.fields[0];
                return new FSharpChoice$2(1, new FSharpChoice$2(0, c_2));
            }
            case 3: {
                const c_3 = matchValue.fields[0];
                return new FSharpChoice$2(1, new FSharpChoice$2(1, c_3));
            }
            default: {
                const c = matchValue.fields[0];
                return new FSharpChoice$2(0, new FSharpChoice$2(0, c));
            }
        }
    }, DSL_OneOf_Z491B0F3C((x) => x, template1, template2), DSL_OneOf_Z491B0F3C((x_1) => x_1, template3, template4));
}

export function DSL_OneOf_7B4F0B5A(f, template1, template2, template3, template4, template5) {
    return DSL_OneOf_Z491B0F3C((s) => {
        const matchValue = f(s);
        switch (matchValue.tag) {
            case 1: {
                const c_1 = matchValue.fields[0];
                return new FSharpChoice$2(0, new FSharpChoice$2(1, c_1));
            }
            case 2: {
                const c_2 = matchValue.fields[0];
                return new FSharpChoice$2(1, new FSharpChoice$2(0, c_2));
            }
            case 3: {
                const c_3 = matchValue.fields[0];
                return new FSharpChoice$2(1, new FSharpChoice$2(1, c_3));
            }
            case 4: {
                const c_4 = matchValue.fields[0];
                return new FSharpChoice$2(1, new FSharpChoice$2(2, c_4));
            }
            default: {
                const c = matchValue.fields[0];
                return new FSharpChoice$2(0, new FSharpChoice$2(0, c));
            }
        }
    }, DSL_OneOf_Z491B0F3C((x) => x, template1, template2), DSL_OneOf_Z2AFE4804((x_1) => x_1, template3, template4, template5));
}

export function DSL_OneOf_Z35273380(f, template1, template2, template3, template4, template5, template6) {
    return DSL_OneOf_Z491B0F3C((s) => {
        const matchValue = f(s);
        switch (matchValue.tag) {
            case 1: {
                const c_1 = matchValue.fields[0];
                return new FSharpChoice$2(0, new FSharpChoice$2(1, c_1));
            }
            case 2: {
                const c_2 = matchValue.fields[0];
                return new FSharpChoice$2(0, new FSharpChoice$2(2, c_2));
            }
            case 3: {
                const c_3 = matchValue.fields[0];
                return new FSharpChoice$2(1, new FSharpChoice$2(0, c_3));
            }
            case 4: {
                const c_4 = matchValue.fields[0];
                return new FSharpChoice$2(1, new FSharpChoice$2(1, c_4));
            }
            case 5: {
                const c_5 = matchValue.fields[0];
                return new FSharpChoice$2(1, new FSharpChoice$2(2, c_5));
            }
            default: {
                const c = matchValue.fields[0];
                return new FSharpChoice$2(0, new FSharpChoice$2(0, c));
            }
        }
    }, DSL_OneOf_Z2AFE4804((x) => x, template1, template2, template3), DSL_OneOf_Z2AFE4804((x_1) => x_1, template4, template5, template6));
}

export function DSL_OneOf_390DE0B8(f, template1, template2, template3, template4, template5, template6, template7) {
    return DSL_OneOf_Z491B0F3C((s) => {
        const matchValue = f(s);
        switch (matchValue.tag) {
            case 1: {
                const c_1 = matchValue.fields[0];
                return new FSharpChoice$2(0, new FSharpChoice$2(1, c_1));
            }
            case 2: {
                const c_2 = matchValue.fields[0];
                return new FSharpChoice$2(0, new FSharpChoice$2(2, c_2));
            }
            case 3: {
                const c_3 = matchValue.fields[0];
                return new FSharpChoice$2(1, new FSharpChoice$2(0, c_3));
            }
            case 4: {
                const c_4 = matchValue.fields[0];
                return new FSharpChoice$2(1, new FSharpChoice$2(1, c_4));
            }
            case 5: {
                const c_5 = matchValue.fields[0];
                return new FSharpChoice$2(1, new FSharpChoice$2(2, c_5));
            }
            case 6: {
                const c_6 = matchValue.fields[0];
                return new FSharpChoice$2(1, new FSharpChoice$2(3, c_6));
            }
            default: {
                const c = matchValue.fields[0];
                return new FSharpChoice$2(0, new FSharpChoice$2(0, c));
            }
        }
    }, DSL_OneOf_Z2AFE4804((x) => x, template1, template2, template3), DSL_OneOf_Z1F5D2DDE((x_1) => x_1, template4, template5, template6, template7));
}

export function DSL_If_14D8D259(predicate, trueTemplate, falseTemplate) {
    return DSL_OneOf_Z491B0F3C((s) => (predicate(s) ? (new FSharpChoice$2(0, s)) : (new FSharpChoice$2(1, s))), trueTemplate, falseTemplate);
}

export function DSL_When_4FF1974C(predicate, template) {
    return DSL_If_14D8D259(predicate, template, new Template$4(1, empty()));
}

export function DSL_Unless_4FF1974C(predicate, template) {
    return DSL_When_4FF1974C((arg) => (!predicate(arg)), template);
}

export function DSL_Seq_Z7461BB91(f, template) {
    return iterator((label) => createGroupNode(label), f, template);
}

export function DSL_CompareStates_5C9B84BF(f, template) {
    return lifecycle((x) => x, f, (state, _arg6) => state, (value) => {
    }, (_arg7, p) => p, template);
}

export function DSL_MakeCaptureSA() {
    return makeCaptureSA();
}

export function DSL_MakeCaptureState() {
    return makeCaptureState();
}

export function DSL_MakeCaptureAction() {
    return makeCaptureAction();
}

export function DSL_Component_Z228F47D0(update, middleware, template) {
    return comp(update, middleware, template);
}

export function DSL_Component_71622930(update, template) {
    return comp(update, (value) => {
    }, template);
}

export function DSL_Portal_10AF510E(selector, template) {
    return transform((render2, impl, state, dispatch) => {
        const parent = document.querySelector(selector);
        const parent_1 = HTMLElementImpl_$ctor_Z5966C024(parent);
        const view = render2(impl, state, dispatch);
        parent_1.Append(view.Impl);
        return view;
    }, template);
}

