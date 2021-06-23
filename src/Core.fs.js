import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { record_type, unit_type, class_type, list_type, union_type, lambda_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { iterate, empty, map } from "./.fable/fable-library.3.1.10/List.js";
import { mapCurriedArgs } from "./.fable/fable-library.3.1.10/Util.js";

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
    return union_type("Tempo.Core.Value`2", [gen0, gen1], Value$2, () => [[["Item", gen1]], [["Item", lambda_type(gen0, gen1)]]]);
}

export function Value$2_Val_2B594(v) {
    return new Value$2(0, v);
}

export function Value$2_Val_Z1FC644C9(f) {
    return new Value$2(1, f);
}

export function Value$2_Val() {
    return new Value$2(1, (x) => x);
}

export function Value$2_Resolve(v, s) {
    if (v.tag === 1) {
        const f = v.fields[0];
        return f(s);
    }
    else {
        const v_1 = v.fields[0];
        return v_1;
    }
}

export function Value$2_Map(m, v) {
    if (v.tag === 1) {
        const f = v.fields[0];
        return new Value$2(1, (arg) => m(f(arg)));
    }
    else {
        const v_1 = v.fields[0];
        return new Value$2(0, m(v_1));
    }
}

export class Template$5 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Node", "Fragment", "MapState", "OneOf2"];
    }
}

export function Template$5$reflection(gen0, gen1, gen2, gen3, gen4) {
    return union_type("Tempo.Core.Template`5", [gen0, gen1, gen2, gen3, gen4], Template$5, () => [[["Item", gen0]], [["Item", list_type(Template$5$reflection(gen0, gen1, gen2, gen3, gen4))]], [["Item", class_type("Tempo.Core.IMapState`5", [gen0, gen1, gen2, gen3, gen4])]], [["Item", class_type("Tempo.Core.IOneOf2`5", [gen0, gen1, gen2, gen3, gen4])]]]);
}

export class ComponentView$4 extends Record {
    constructor(Impl, Dispatch, Change, Destroy, Query) {
        super();
        this.Impl = Impl;
        this.Dispatch = Dispatch;
        this.Change = Change;
        this.Destroy = Destroy;
        this.Query = Query;
    }
}

export function ComponentView$4$reflection(gen0, gen1, gen2, gen3) {
    return record_type("Tempo.Core.ComponentView`4", [gen0, gen1, gen2, gen3], ComponentView$4, () => [["Impl", gen0], ["Dispatch", lambda_type(gen2, unit_type)], ["Change", lambda_type(gen1, unit_type)], ["Destroy", lambda_type(unit_type, unit_type)], ["Query", lambda_type(gen3, unit_type)]]);
}

export class View$3 extends Record {
    constructor(Impl, Change, Destroy, Query) {
        super();
        this.Impl = Impl;
        this.Change = Change;
        this.Destroy = Destroy;
        this.Query = Query;
    }
}

export function View$3$reflection(gen0, gen1, gen2) {
    return record_type("Tempo.Core.View`3", [gen0, gen1, gen2], View$3, () => [["Impl", gen0], ["Change", lambda_type(gen1, unit_type)], ["Destroy", lambda_type(unit_type, unit_type)], ["Query", lambda_type(gen2, unit_type)]]);
}

export class MapState$7 {
    constructor(m, t) {
        this.m = m;
        this.t = t;
    }
    Accept(f) {
        const this$ = this;
        return f.Invoke(this$);
    }
}

export function MapState$7$reflection(gen0, gen1, gen2, gen3, gen4, gen5, gen6) {
    return class_type("Tempo.Core.MapState`7", [gen0, gen1, gen2, gen3, gen4, gen5, gen6], MapState$7);
}

export function MapState$7_$ctor_3D37D6BB(m, t) {
    return new MapState$7(m, t);
}

export class OneOf2$9 {
    constructor(m, c1, c2) {
        this.m = m;
        this.c1 = c1;
        this.c2 = c2;
    }
    Accept(f) {
        const this$ = this;
        return f.Invoke(this$);
    }
}

export function OneOf2$9$reflection(gen0, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8) {
    return class_type("Tempo.Core.OneOf2`9", [gen0, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8], OneOf2$9);
}

export function OneOf2$9_$ctor_Z6CB02F2D(m, c1, c2) {
    return new OneOf2$9(m, c1, c2);
}

export function MapState$7__get_MapF(this$) {
    return this$.m;
}

export function MapState$7__get_Template(this$) {
    return this$.t;
}

export function OneOf2$9__get_MapF(this$) {
    return this$.m;
}

export function OneOf2$9__get_Template1(this$) {
    return this$.c1;
}

export function OneOf2$9__get_Template2(this$) {
    return this$.c2;
}

export function packMapState(mapState) {
    return mapState;
}

export function unpackMapState(mapState, f) {
    return mapState.Accept(f);
}

export function packOneOf2(oneOf2) {
    return oneOf2;
}

export function unpackOneOf2(oneOf2, f) {
    return oneOf2.Accept(f);
}

export class MakeRender$5 {
    constructor() {
    }
}

export function MakeRender$5$reflection(gen0, gen1, gen2, gen3, gen4) {
    return class_type("Tempo.Core.MakeRender`5", [gen0, gen1, gen2, gen3, gen4], MakeRender$5);
}

export function MakeRender$5_$ctor() {
    return new MakeRender$5();
}

export function MakeRender$5__Make_Z982EEB6(this$, template) {
    switch (template.tag) {
        case 1: {
            const ls = template.fields[0];
            return MakeRender$5__MakeFragment_Z1C923E80(this$, ls);
        }
        case 2: {
            const mapState = template.fields[0];
            return MakeRender$5__MakeMapState_5E25218(this$, mapState);
        }
        case 3: {
            const oneOf2 = template.fields[0];
            return MakeRender$5__MakeOneOf2_369027EC(this$, oneOf2);
        }
        default: {
            const n = template.fields[0];
            return this$["Tempo.Core.MakeRender`5.MakeNode2B595"](n);
        }
    }
}

export function MakeRender$5__MakeRenderS(this$) {
    return this$;
}

export function MakeRender$5__MakeFragment_Z1C923E80(this$, templates) {
    const fs = map((arg00) => MakeRender$5__Make_Z982EEB6(this$, arg00), templates);
    return (parent) => ((s) => {
        const impl = this$["Tempo.Core.MakeRender`5.MakeRef"](parent, empty());
        this$["Tempo.Core.MakeRender`5.AppendNode"](parent, impl);
        const views = map(mapCurriedArgs((f) => f(impl, s), [[0, 2]]), fs);
        return new View$3(impl, (s_1) => {
            iterate((i) => {
                i.Change(s_1);
            }, views);
        }, () => {
            iterate((i_1) => {
                i_1.Destroy();
            }, views);
        }, (q) => {
            iterate((i_2) => {
                i_2.Query(q);
            }, views);
        });
    });
}

export function MakeRender$5__MakeMapState_5E25218(this$, mapState) {
    return unpackMapState(mapState, {
        Invoke(mapState_1) {
            const render = MakeRender$5__Make_Z982EEB6(MakeRender$5__MakeRenderS(this$), MapState$7__get_Template(mapState_1));
            return (i) => ((s) => {
                const view = render(i)(MapState$7__get_MapF(mapState_1)(s));
                return new View$3(view.Impl, (s1) => {
                    view.Change(MapState$7__get_MapF(mapState_1)(s1));
                }, view.Destroy, view.Query);
            });
        },
    });
}

export function MakeRender$5__MakeOneOf2_369027EC(this$, oneOf2) {
    return unpackOneOf2(oneOf2, {
        Invoke(oneOf2_1) {
            const render1 = MakeRender$5__Make_Z982EEB6(MakeRender$5__MakeRenderS(this$), OneOf2$9__get_Template1(oneOf2_1));
            const render2 = MakeRender$5__Make_Z982EEB6(MakeRender$5__MakeRenderS(this$), OneOf2$9__get_Template2(oneOf2_1));
            return (parent) => ((s) => {
                let views = void 0;
                const matchValue = OneOf2$9__get_MapF(oneOf2_1)(s);
                if (matchValue.tag === 1) {
                    const s2 = matchValue.fields[0];
                }
                else {
                    const s1 = matchValue.fields[0];
                    const view1 = render1(parent)(s1);
                }
                return new View$3(parent, (value) => {
                }, () => {
                }, (value_1) => {
                });
            });
        },
    });
}

