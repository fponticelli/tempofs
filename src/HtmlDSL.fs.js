import { Record } from "./.fable/fable-library.3.1.10/Types.js";
import { class_type, record_type, lambda_type, unit_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { cons, iterate, ofArray } from "./.fable/fable-library.3.1.10/List.js";
import { toArray } from "./.fable/fable-library.3.1.10/Option.js";
import { makeTrigger, HTMLTemplateAttribute$2, HTMLTemplateAttributeValue$2, HTMLTemplateElement$3, HTMLTemplateNode$3, HTMLElementImpl_$ctor_4C3D2741, MakeHTMLRender$3_$ctor_Z6156FC82 } from "./Html.fs.js";
import { Iterator$6_$ctor_4854B10D, packIterator, OneOf2$8_$ctor_Z4F5F76C, packOneOf2, MapState$6_$ctor_Z445964B9, packMapState, Value$2, Template$4, ComponentView$3, MakeRender$4__Make_1DCD9633 } from "./Core.fs.js";
import { FSharpChoice$2 } from "./.fable/fable-library.3.1.10/Choice.js";

export class MiddlewarePayload$2 extends Record {
    constructor(Current, Old, Action, Dispatch) {
        super();
        this.Current = Current;
        this.Old = Old;
        this.Action = Action;
        this.Dispatch = Dispatch;
    }
}

export function MiddlewarePayload$2$reflection(gen0, gen1) {
    return record_type("Tempo.Html.DSL.MiddlewarePayload`2", [gen0, gen1], MiddlewarePayload$2, () => [["Current", gen0], ["Old", gen0], ["Action", gen1], ["Dispatch", lambda_type(gen1, unit_type)]]);
}

export class HTML {
    constructor() {
    }
}

export function HTML$reflection() {
    return class_type("Tempo.Html.DSL.HTML", void 0, HTML);
}

export function HTML_MakeProgram_Z7A39E9B2(template, el, audit) {
    let invokes = ofArray(toArray(audit));
    const dispatch = (action) => {
        iterate((f) => {
            f(action);
        }, invokes);
    };
    const renderInstance = MakeHTMLRender$3_$ctor_Z6156FC82(dispatch);
    const f_1 = MakeRender$4__Make_1DCD9633(renderInstance, template);
    const parent = HTMLElementImpl_$ctor_4C3D2741(el);
    let render;
    const clo1 = f_1(parent);
    render = ((arg10) => clo1(arg10));
    return (update) => ((middleware) => ((state) => {
        let localState = state;
        const view = render(localState);
        const updateDispatch = (action_1) => {
            const newState = update(localState, action_1);
            view.Change(newState);
            middleware(new MiddlewarePayload$2(newState, localState, action_1, dispatch));
            localState = newState;
        };
        invokes = cons(updateDispatch, invokes);
        return new ComponentView$3(view.Impl, dispatch, view.Change, view.Destroy, view.Query);
    }));
}

export function HTML_El_Z67FF9A04(name, attributes, children) {
    return new Template$4(0, new HTMLTemplateNode$3(0, new HTMLTemplateElement$3(name, attributes, children)));
}

export function HTML_Text() {
    return new Template$4(0, new HTMLTemplateNode$3(1, new Value$2(1, (x) => x)));
}

export function HTML_Text_Z721C83C5(value) {
    return new Template$4(0, new HTMLTemplateNode$3(1, new Value$2(0, value)));
}

export function HTML_Text_77A7E8C8(f) {
    return new Template$4(0, new HTMLTemplateNode$3(1, new Value$2(1, f)));
}

export function HTML_Attr_68C4AEB5(name, value) {
    return new HTMLTemplateAttribute$2(name, new HTMLTemplateAttributeValue$2(0, new Value$2(0, value)));
}

export function HTML_Attr_Z384F8060(name, value) {
    return new HTMLTemplateAttribute$2(name, new HTMLTemplateAttributeValue$2(0, new Value$2(0, value)));
}

export function HTML_Attr_Z3A5D29FA(name, f) {
    return new HTMLTemplateAttribute$2(name, new HTMLTemplateAttributeValue$2(0, new Value$2(1, f)));
}

export function HTML_Attr_3DF4EB53(name, f) {
    return new HTMLTemplateAttribute$2(name, new HTMLTemplateAttributeValue$2(0, new Value$2(1, (arg) => f(arg))));
}

export function HTML_On_4A53169E(name, action) {
    return new HTMLTemplateAttribute$2(name, new HTMLTemplateAttributeValue$2(1, makeTrigger((_arg1) => action)));
}

export function HTML_On_459CDA74(name, handler) {
    return new HTMLTemplateAttribute$2(name, new HTMLTemplateAttributeValue$2(1, makeTrigger((_arg2) => handler())));
}

export function HTML_On_47AABEE2(name, handler) {
    return new HTMLTemplateAttribute$2(name, new HTMLTemplateAttributeValue$2(1, makeTrigger(handler)));
}

export function HTML_MapState_5F509A03(f, template) {
    return new Template$4(2, packMapState(MapState$6_$ctor_Z445964B9(f, template)));
}

export function HTML_MapState_Z5D9F8A77(f, templates) {
    return HTML_MapState_5F509A03(f, new Template$4(1, templates));
}

export function HTML_OneOf_Z491B0F3C(f, template1, template2) {
    return new Template$4(3, packOneOf2(OneOf2$8_$ctor_Z4F5F76C(f, template1, template2)));
}

export function HTML_OneOf_Z2AFE4804(f, template1, template2, template3) {
    return HTML_OneOf_Z491B0F3C((s) => {
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
    }, template1, HTML_OneOf_Z491B0F3C((x) => x, template2, template3));
}

export function HTML_OneOf_Z1F5D2DDE(f, template1, template2, template3, template4) {
    return HTML_OneOf_Z491B0F3C((s) => {
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
    }, HTML_OneOf_Z491B0F3C((x) => x, template1, template2), HTML_OneOf_Z491B0F3C((x_1) => x_1, template3, template4));
}

export function HTML_OneOf_7B4F0B5A(f, template1, template2, template3, template4, template5) {
    return HTML_OneOf_Z491B0F3C((s) => {
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
    }, HTML_OneOf_Z491B0F3C((x) => x, template1, template2), HTML_OneOf_Z2AFE4804((x_1) => x_1, template3, template4, template5));
}

export function HTML_OneOf_Z35273380(f, template1, template2, template3, template4, template5, template6) {
    return HTML_OneOf_Z491B0F3C((s) => {
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
    }, HTML_OneOf_Z2AFE4804((x) => x, template1, template2, template3), HTML_OneOf_Z2AFE4804((x_1) => x_1, template4, template5, template6));
}

export function HTML_OneOf_390DE0B8(f, template1, template2, template3, template4, template5, template6, template7) {
    return HTML_OneOf_Z491B0F3C((s) => {
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
    }, HTML_OneOf_Z2AFE4804((x) => x, template1, template2, template3), HTML_OneOf_Z1F5D2DDE((x_1) => x_1, template4, template5, template6, template7));
}

export function HTML_Seq_Z7461BB91(f, template) {
    return new Template$4(4, packIterator(Iterator$6_$ctor_4854B10D(f, template)));
}

