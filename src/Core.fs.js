import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { record_type, unit_type, option_type, class_type, list_type, union_type, lambda_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { uncurry, partialApply, mapCurriedArgs, curry } from "./.fable/fable-library.3.1.10/Util.js";
import { iterate, map } from "./.fable/fable-library.3.1.10/List.js";

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
        return ["Node", "Fragment", "MapState"];
    }
}

export function Template$5$reflection(gen0, gen1, gen2, gen3, gen4) {
    return union_type("Tempo.Core.Template`5", [gen0, gen1, gen2, gen3, gen4], Template$5, () => [[["Item", gen0]], [["Item", list_type(Template$5$reflection(gen0, gen1, gen2, gen3, gen4))]], [["Item", class_type("Tempo.Core.IMapState`5", [gen0, gen1, gen2, gen3, gen4])]]]);
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
    return record_type("Tempo.Core.ComponentView`4", [gen0, gen1, gen2, gen3], ComponentView$4, () => [["Impl", option_type(gen0)], ["Dispatch", lambda_type(gen2, unit_type)], ["Change", lambda_type(gen1, unit_type)], ["Destroy", lambda_type(unit_type, unit_type)], ["Query", lambda_type(gen3, unit_type)]]);
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
    return record_type("Tempo.Core.View`3", [gen0, gen1, gen2], View$3, () => [["Impl", option_type(gen0)], ["Change", lambda_type(gen1, unit_type)], ["Destroy", lambda_type(unit_type, unit_type)], ["Query", lambda_type(gen2, unit_type)]]);
}

export class MapState$6 {
    constructor(m, t, mrn) {
        this.m = m;
        this.t = t;
        this.mrn = mrn;
    }
    Accept(f) {
        const this$ = this;
        return f.Invoke(this$);
    }
}

export function MapState$6$reflection(gen0, gen1, gen2, gen3, gen4, gen5) {
    return class_type("Tempo.Core.MapState`6", [gen0, gen1, gen2, gen3, gen4, gen5], MapState$6);
}

export function MapState$6_$ctor_10B296A9(m, t, mrn) {
    return new MapState$6(m, t, mrn);
}

export function MapState$6__get_MapF(this$) {
    return this$.m;
}

export function MapState$6__get_Template(this$) {
    return this$.t;
}

export function MapState$6__get_MakeRenderNode(this$) {
    return curry(3, this$.mrn);
}

export function packMapState(mapState) {
    return mapState;
}

export function unpackMapState(mapState, f) {
    return mapState.Accept(f);
}

export function makeMapState(makeNodeRender, f, t) {
    return packMapState(MapState$6_$ctor_10B296A9(f, t, makeNodeRender));
}

export function makeRender(makeNodeRender, template) {
    switch (template.tag) {
        case 1: {
            const ls = template.fields[0];
            const fs = map((template_1) => makeRender(makeNodeRender, template_1), ls);
            return (i) => ((s) => {
                const views = map(mapCurriedArgs((f) => f(i, s), [[0, 2]]), fs);
                return new View$3(void 0, (s_1) => {
                    iterate((i_1) => {
                        i_1.Change(s_1);
                    }, views);
                }, () => {
                    iterate((i_2) => {
                        i_2.Destroy();
                    }, views);
                }, (q) => {
                    iterate((i_3) => {
                        i_3.Query(q);
                    }, views);
                });
            });
        }
        case 2: {
            const mapState = template.fields[0];
            return makeRenderMapState(mapState);
        }
        default: {
            const n = template.fields[0];
            return partialApply(2, makeNodeRender, [n]);
        }
    }
}

export function makeRenderMapState(mapState) {
    return unpackMapState(mapState, {
        Invoke(mapState_1) {
            const t = MapState$6__get_Template(mapState_1);
            const render = makeRender(uncurry(3, MapState$6__get_MakeRenderNode(mapState_1)), t);
            return (i) => ((s) => {
                const view = render(i)(MapState$6__get_MapF(mapState_1)(s));
                return new View$3(view.Impl, (s1) => {
                    view.Change(MapState$6__get_MapF(mapState_1)(s1));
                }, view.Destroy, view.Query);
            });
        },
    });
}

