import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { record_type, unit_type, class_type, list_type, union_type, lambda_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { iterate, map as map_1 } from "./.fable/fable-library.3.1.10/List.js";
import { uncurry } from "./.fable/fable-library.3.1.10/Util.js";

export class Value$2 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Literal", "Derived"];
    }
}

export function Value$2$reflection(gen0, gen1) {
    return union_type("Tempo.Core.Value`2", [gen0, gen1], Value$2, () => [[["Item", gen0]], [["Item", lambda_type(gen1, gen0)]]]);
}

export class Template$2 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Node", "Fragment", "MapState", "Embed"];
    }
}

export function Template$2$reflection(gen0, gen1) {
    return union_type("Tempo.Core.Template`2", [gen0, gen1], Template$2, () => [[["Item", gen0]], [["Item", list_type(Template$2$reflection(gen0, gen1))]], [["Item", class_type("Tempo.Core.MapStateEvaluator`1", [gen1])]], [["Item", class_type("Tempo.Core.EmbedEvaluator`2", [gen0, gen1])]]]);
}

export class View$2 extends Record {
    constructor(Impl, Change, Destroy) {
        super();
        this.Impl = Impl;
        this.Change = Change;
        this.Destroy = Destroy;
    }
}

export function View$2$reflection(gen0, gen1) {
    return record_type("Tempo.Core.View`2", [gen0, gen1], View$2, () => [["Impl", gen1], ["Change", lambda_type(gen0, unit_type)], ["Destroy", lambda_type(unit_type, unit_type)]]);
}

export function resolve(value, state) {
    if (value.tag === 1) {
        const f = value.fields[0];
        return f(state);
    }
    else {
        const v = value.fields[0];
        return v;
    }
}

export function render(renderNode_mut, template_mut, impl_mut, state_mut) {
    render:
    while (true) {
        const renderNode = renderNode_mut, template = template_mut, impl = impl_mut, state = state_mut;
        switch (template.tag) {
            case 1: {
                const ls = template.fields[0];
                const ls_1 = map_1((i) => render(renderNode, i, impl, state), ls);
                return new View$2(impl, (state_1) => {
                    iterate((i_1) => {
                        i_1.Change(state_1);
                    }, ls_1);
                }, () => {
                    iterate((i_2) => {
                        i_2.Destroy();
                    }, ls_1);
                });
            }
            case 2: {
                const ev = template.fields[0];
                const patternInput = ev.Eval();
                const template_1 = patternInput[1];
                const map = patternInput[0];
                renderNode_mut = renderNode;
                template_mut = template_1;
                impl_mut = impl;
                state_mut = map(state);
                continue render;
            }
            case 3: {
                const ev_1 = template.fields[0];
                const patternInput_1 = ev_1.Eval();
                const template_2 = patternInput_1[2];
                const renderEmbedded = patternInput_1[0];
                const attach = patternInput_1[1];
                const implEmbedded = attach(impl);
                renderNode_mut = uncurry(3, renderEmbedded);
                template_mut = template_2;
                impl_mut = implEmbedded;
                state_mut = state;
                continue render;
            }
            default: {
                const n = template.fields[0];
                return renderNode(n, impl, state);
            }
        }
        break;
    }
}

