import { Record, Union } from "../Tempo.Demo/.fable/fable-library.3.1.10/Types.js";
import { record_type, unit_type, class_type, list_type, union_type, lambda_type } from "../Tempo.Demo/.fable/fable-library.3.1.10/Reflection.js";
import { comparePrimitives, min as min_1, mapCurriedArgs, uncurry, partialApply, curry } from "../Tempo.Demo/.fable/fable-library.3.1.10/Util.js";
import { append, take, skip, zip, length, iterate, map as map_3 } from "../Tempo.Demo/.fable/fable-library.3.1.10/List.js";
import { value } from "../Tempo.Demo/.fable/fable-library.3.1.10/Option.js";

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

export function Value$2_Of_2B594(v) {
    return new Value$2(0, v);
}

export function Value$2_Of_Z1FC644C9(f) {
    return new Value$2(1, f);
}

export function Value$2_Of() {
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

export class Template$4 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Node", "Fragment", "Transform", "OneOf2", "Iterator"];
    }
}

export function Template$4$reflection(gen0, gen1, gen2, gen3) {
    return union_type("Tempo.Core.Template`4", [gen0, gen1, gen2, gen3], Template$4, () => [[["Item", gen0]], [["Item", list_type(Template$4$reflection(gen0, gen1, gen2, gen3))]], [["Item", class_type("Tempo.Core.ITransform`4", [gen0, gen1, gen2, gen3])]], [["Item", class_type("Tempo.Core.IOneOf2`4", [gen0, gen1, gen2, gen3])]], [["Item", class_type("Tempo.Core.IIterator`4", [gen0, gen1, gen2, gen3])]]]);
}

export class ComponentView$3 extends Record {
    constructor(Impl, Dispatch, Change, Destroy, Query) {
        super();
        this.Impl = Impl;
        this.Dispatch = Dispatch;
        this.Change = Change;
        this.Destroy = Destroy;
        this.Query = Query;
    }
}

export function ComponentView$3$reflection(gen0, gen1, gen2) {
    return record_type("Tempo.Core.ComponentView`3", [gen0, gen1, gen2], ComponentView$3, () => [["Impl", class_type("Tempo.Core.Impl")], ["Dispatch", lambda_type(gen1, unit_type)], ["Change", lambda_type(gen0, unit_type)], ["Destroy", lambda_type(unit_type, unit_type)], ["Query", lambda_type(gen2, unit_type)]]);
}

export class View$2 extends Record {
    constructor(Impl, Change, Destroy, Query) {
        super();
        this.Impl = Impl;
        this.Change = Change;
        this.Destroy = Destroy;
        this.Query = Query;
    }
}

export function View$2$reflection(gen0, gen1) {
    return record_type("Tempo.Core.View`2", [gen0, gen1], View$2, () => [["Impl", class_type("Tempo.Core.Impl")], ["Change", lambda_type(gen0, unit_type)], ["Destroy", lambda_type(unit_type, unit_type)], ["Query", lambda_type(gen1, unit_type)]]);
}

export class Transform$8 {
    constructor(transform_1, template) {
        this.transform = transform_1;
        this.template = template;
    }
    Accept(f) {
        const this$ = this;
        return f.Invoke(this$);
    }
}

export function Transform$8$reflection(gen0, gen1, gen2, gen3, gen4, gen5, gen6, gen7) {
    return class_type("Tempo.Core.Transform`8", [gen0, gen1, gen2, gen3, gen4, gen5, gen6, gen7], Transform$8);
}

export function Transform$8_$ctor_Z75B4529B(transform_1, template) {
    return new Transform$8(transform_1, template);
}

export class OneOf2$8 {
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

export function OneOf2$8$reflection(gen0, gen1, gen2, gen3, gen4, gen5, gen6, gen7) {
    return class_type("Tempo.Core.OneOf2`8", [gen0, gen1, gen2, gen3, gen4, gen5, gen6, gen7], OneOf2$8);
}

export function OneOf2$8_$ctor_Z4F5F76C(m, c1, c2) {
    return new OneOf2$8(m, c1, c2);
}

export class Iterator$6 {
    constructor(f, template) {
        this.f = f;
        this.template = template;
    }
    Accept(f) {
        const this$ = this;
        return f.Invoke(this$);
    }
}

export function Iterator$6$reflection(gen0, gen1, gen2, gen3, gen4, gen5) {
    return class_type("Tempo.Core.Iterator`6", [gen0, gen1, gen2, gen3, gen4, gen5], Iterator$6);
}

export function Iterator$6_$ctor_4854B10D(f, template) {
    return new Iterator$6(f, template);
}

export function Transform$8__get_Transform(this$) {
    return curry(4, this$.transform);
}

export function Transform$8__get_Template(this$) {
    return this$.template;
}

export function OneOf2$8__get_MapF(this$) {
    return this$.m;
}

export function OneOf2$8__get_Template1(this$) {
    return this$.c1;
}

export function OneOf2$8__get_Template2(this$) {
    return this$.c2;
}

export function Iterator$6__get_MapF(this$) {
    return this$.f;
}

export function Iterator$6__get_Template(this$) {
    return this$.template;
}

export function packTransform(transform_1) {
    return transform_1;
}

export function unpackTransform(transform_1, f) {
    return transform_1.Accept(f);
}

export function packOneOf2(oneOf2) {
    return oneOf2;
}

export function unpackOneOf2(oneOf2, f) {
    return oneOf2.Accept(f);
}

export function packIterator(iterator) {
    return iterator;
}

export function unpackIterator(iterator, f) {
    return iterator.Accept(f);
}

export class ChoiceAssignament$2 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["FirstOnly", "SecondOnly", "FirstAndSecond", "SecondAndFirst"];
    }
}

export function ChoiceAssignament$2$reflection(gen0, gen1) {
    return union_type("Tempo.Core.ChoiceAssignament`2", [gen0, gen1], ChoiceAssignament$2, () => [[["Item", gen0]], [["Item", gen1]], [["Item1", gen0], ["Item2", gen1]], [["Item1", gen0], ["Item2", gen1]]]);
}

export class MakeRender$4 {
    constructor(makeNodeRender, createGroupNode) {
        this.makeNodeRender = makeNodeRender;
        this.createGroupNode = createGroupNode;
    }
}

export function MakeRender$4$reflection(gen0, gen1, gen2, gen3) {
    return class_type("Tempo.Core.MakeRender`4", [gen0, gen1, gen2, gen3], MakeRender$4);
}

export function MakeRender$4_$ctor_Z4E96F168(makeNodeRender, createGroupNode) {
    return new MakeRender$4(makeNodeRender, createGroupNode);
}

export function MakeRender$4__Make_1DCD9633(this$, template) {
    switch (template.tag) {
        case 1: {
            const ls = template.fields[0];
            return MakeRender$4__MakeFragmentRender_7D0C1B99(this$, ls);
        }
        case 2: {
            const map_1 = template.fields[0];
            return MakeRender$4__MakeTransformRender_Z673AFE5A(this$, map_1);
        }
        case 3: {
            const oneOf2 = template.fields[0];
            return MakeRender$4__MakeOneOf2Render_134AD555(this$, oneOf2);
        }
        case 4: {
            const iterator = template.fields[0];
            return MakeRender$4__MakeIteratorRender_40023148(this$, iterator);
        }
        default: {
            const n = template.fields[0];
            return partialApply(3, this$.makeNodeRender, [uncurry(4, (arg00) => MakeRender$4__Make_1DCD9633(this$, arg00)), n]);
        }
    }
}

export function MakeRender$4__MakeRenderS(this$) {
    return this$;
}

export function MakeRender$4__MakeFragmentRender_7D0C1B99(this$, templates) {
    const fs = map_3((arg00) => MakeRender$4__Make_1DCD9633(this$, arg00), templates);
    return (parent) => ((s) => ((dispatch) => {
        const group = this$.createGroupNode("Fragment");
        parent.Append(group);
        const views = map_3(mapCurriedArgs((render) => render(group, s, dispatch), [[0, 3]]), fs);
        return new View$2(group, (s_1) => {
            iterate((i) => {
                i.Change(s_1);
            }, views);
        }, () => {
            parent.Remove(group);
            iterate((i_1) => {
                i_1.Destroy();
            }, views);
        }, (q) => {
            iterate((i_2) => {
                i_2.Query(q);
            }, views);
        });
    }));
}

export function MakeRender$4__MakeTransformRender_Z673AFE5A(this$, map_1) {
    return unpackTransform(map_1, {
        Invoke(map_2) {
            const render2 = MakeRender$4__Make_1DCD9633(MakeRender$4__MakeRenderS(this$), Transform$8__get_Template(map_2));
            return (impl) => ((state) => ((dispatch) => Transform$8__get_Transform(map_2)(uncurry(3, render2))(impl)(state)(dispatch)));
        },
    });
}

export function MakeRender$4__MakeOneOf2Render_134AD555(this$, oneOf2) {
    return unpackOneOf2(oneOf2, {
        Invoke(oneOf2_1) {
            const render1 = MakeRender$4__Make_1DCD9633(MakeRender$4__MakeRenderS(this$), OneOf2$8__get_Template1(oneOf2_1));
            const render2 = MakeRender$4__Make_1DCD9633(MakeRender$4__MakeRenderS(this$), OneOf2$8__get_Template2(oneOf2_1));
            return (parent) => ((s) => ((dispatch) => {
                const group = this$.createGroupNode("OneOf2");
                parent.Append(group);
                let assignament;
                const matchValue = OneOf2$8__get_MapF(oneOf2_1)(s);
                if (matchValue.tag === 1) {
                    const s2 = matchValue.fields[0];
                    const view2 = render2(group)(s2)(dispatch);
                    assignament = (new ChoiceAssignament$2(1, view2));
                }
                else {
                    const s1 = matchValue.fields[0];
                    const view1 = render1(group)(s1)(dispatch);
                    assignament = (new ChoiceAssignament$2(0, view1));
                }
                const change = (state) => {
                    const matchValue_1 = [assignament, OneOf2$8__get_MapF(oneOf2_1)(state)];
                    if (matchValue_1[0].tag === 2) {
                        if (matchValue_1[1].tag === 1) {
                            const s2_4 = matchValue_1[1].fields[0];
                            const view1_4 = matchValue_1[0].fields[0];
                            const view2_4 = matchValue_1[0].fields[1];
                            group.Append(view2_4.Impl);
                            view2_4.Change(s2_4);
                            group.Remove(view1_4.Impl);
                            assignament = (new ChoiceAssignament$2(3, view1_4, view2_4));
                        }
                        else {
                            const s1_2 = matchValue_1[1].fields[0];
                            const view1_2 = matchValue_1[0].fields[0];
                            view1_2.Change(s1_2);
                        }
                    }
                    else if (matchValue_1[0].tag === 1) {
                        if (matchValue_1[1].tag === 0) {
                            const s1_3 = matchValue_1[1].fields[0];
                            const view2_5 = matchValue_1[0].fields[0];
                            const view1_5 = render1(group)(s1_3)(dispatch);
                            group.Remove(view2_5.Impl);
                            assignament = (new ChoiceAssignament$2(2, view1_5, view2_5));
                        }
                        else {
                            const s2_1 = matchValue_1[1].fields[0];
                            const view2_1 = matchValue_1[0].fields[0];
                            view2_1.Change(s2_1);
                        }
                    }
                    else if (matchValue_1[0].tag === 3) {
                        if (matchValue_1[1].tag === 0) {
                            const s1_4 = matchValue_1[1].fields[0];
                            const view1_6 = matchValue_1[0].fields[0];
                            const view2_6 = matchValue_1[0].fields[1];
                            group.Append(view1_6.Impl);
                            view1_6.Change(s1_4);
                            group.Remove(view2_6.Impl);
                            assignament = (new ChoiceAssignament$2(2, view1_6, view2_6));
                        }
                        else {
                            const s2_2 = matchValue_1[1].fields[0];
                            const view2_2 = matchValue_1[0].fields[1];
                            view2_2.Change(s2_2);
                        }
                    }
                    else if (matchValue_1[1].tag === 1) {
                        const s2_3 = matchValue_1[1].fields[0];
                        const view1_3 = matchValue_1[0].fields[0];
                        const view2_3 = render2(group)(s2_3)(dispatch);
                        group.Remove(view1_3.Impl);
                        assignament = (new ChoiceAssignament$2(3, view1_3, view2_3));
                    }
                    else {
                        const s1_1 = matchValue_1[1].fields[0];
                        const view1_1 = matchValue_1[0].fields[0];
                        view1_1.Change(s1_1);
                    }
                };
                const query = (q) => {
                    let pattern_matching_result, view1_7, view2_7;
                    switch (assignament.tag) {
                        case 0: {
                            pattern_matching_result = 0;
                            view1_7 = assignament.fields[0];
                            break;
                        }
                        case 3: {
                            pattern_matching_result = 1;
                            view2_7 = assignament.fields[1];
                            break;
                        }
                        case 1: {
                            pattern_matching_result = 1;
                            view2_7 = assignament.fields[0];
                            break;
                        }
                        default: {
                            pattern_matching_result = 0;
                            view1_7 = assignament.fields[0];
                        }
                    }
                    switch (pattern_matching_result) {
                        case 0: {
                            view1_7.Query(q);
                            break;
                        }
                        case 1: {
                            view2_7.Query(q);
                            break;
                        }
                    }
                };
                const destroy = (q_1) => {
                    parent.Remove(group);
                    switch (assignament.tag) {
                        case 0: {
                            const view1_9 = assignament.fields[0];
                            view1_9.Destroy();
                            break;
                        }
                        case 3: {
                            const view2_9 = assignament.fields[1];
                            const view1_10 = assignament.fields[0];
                            view2_9.Destroy();
                            view1_10.Destroy();
                            break;
                        }
                        case 1: {
                            const view2_10 = assignament.fields[0];
                            view2_10.Destroy();
                            break;
                        }
                        default: {
                            const view2_8 = assignament.fields[1];
                            const view1_8 = assignament.fields[0];
                            view1_8.Destroy();
                            view2_8.Destroy();
                        }
                    }
                };
                return new View$2(group, change, destroy, query);
            }));
        },
    });
}

export function MakeRender$4__MakeIteratorRender_40023148(this$, iterator) {
    return unpackIterator(iterator, {
        Invoke(iterator_1) {
            const render = MakeRender$4__Make_1DCD9633(MakeRender$4__MakeRenderS(this$), Iterator$6__get_Template(iterator_1));
            return (parent) => ((s) => ((dispatch) => {
                const group = this$.createGroupNode("Iterator");
                parent.Append(group);
                const ls = Iterator$6__get_MapF(iterator_1)(s);
                let views = map_3((state) => render(group)(state)(dispatch), ls);
                const query = (q) => {
                    iterate((view) => {
                        view.Query(q);
                    }, views);
                };
                const change = (s_1) => {
                    const states = Iterator$6__get_MapF(iterator_1)(s_1);
                    const min = min_1((x, y) => comparePrimitives(x, y), length(views), length(states)) | 0;
                    iterate((tupledArg) => {
                        const view_1 = tupledArg[0];
                        const state_1 = tupledArg[1];
                        view_1.Change(state_1);
                    }, zip(views, states));
                    iterate((view_2) => {
                        view_2.Destroy();
                    }, skip(min, views));
                    views = take(min, views);
                    const newViews = map_3((state_2) => render(group)(state_2)(dispatch), skip(min, states));
                    views = append(views, newViews);
                };
                const destroy = () => {
                    iterate((view_3) => {
                        view_3.Destroy();
                    }, views);
                };
                return new View$2(group, change, destroy, query);
            }));
        },
    });
}

export function transform(transform_1, template) {
    return new Template$4(2, packTransform(Transform$8_$ctor_Z75B4529B(transform_1, template)));
}

export function map(mapImpl, stateMap, actionMap, queryMap, template) {
    return transform((render2, impl, state, dispatch) => {
        const state2 = stateMap(state);
        const dispatch2 = (action2) => {
            const matchValue = actionMap(action2);
            if (matchValue == null) {
            }
            else {
                const a = value(matchValue);
                dispatch(a);
            }
        };
        const impl2 = mapImpl(impl);
        const view = render2(impl2, state2, dispatch2);
        return new View$2(impl2, (s1) => {
            view.Change(stateMap(s1));
        }, view.Destroy, (q1) => {
            view.Query(queryMap(q1));
        });
    }, template);
}

export function lifecycle(afterRender, beforeChange, afterChange, beforeDestroy, respond, template) {
    return transform((render, impl, state, dispatch) => {
        const view = render(impl, state, dispatch);
        let payload = afterRender(state);
        return new View$2(impl, (s) => {
            if (beforeChange(s, payload)) {
                view.Change(s);
                payload = afterChange(s, payload);
            }
        }, () => {
            beforeDestroy(payload);
            view.Destroy();
        }, (q) => {
            view.Query(q);
            payload = respond(q, payload);
        });
    }, template);
}

