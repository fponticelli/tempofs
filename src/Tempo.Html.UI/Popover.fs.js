import { Record, Union } from "../../../src/.fable/fable-library.3.1.10/Types.js";
import { class_type, float64_type, record_type, option_type, union_type } from "../../../src/.fable/fable-library.3.1.10/Reflection.js";
import { ComponentView$3, ComponentView$3$reflection } from "../Tempo.Core/Core.fs.js";
import { toArray, defaultArg } from "../../../src/.fable/fable-library.3.1.10/Option.js";
import { iterate, ofArray, singleton } from "../../../src/.fable/fable-library.3.1.10/List.js";
import { interpolate, toText } from "../../../src/.fable/fable-library.3.1.10/String.js";
import { getFocusable, targetHasSpecifiedAncestor, remove as remove_1, collectElementAndAncestors } from "../Tempo.Html/Html.Tools.fs.js";
import { DSL_MakeProgram_1C1F9AE9 } from "../Tempo.Html/Html.DSL.fs.js";
import { lifecycleAttribute } from "../Tempo.Html/Html.Impl.fs.js";
import { iterate as iterate_1 } from "../../../src/.fable/fable-library.3.1.10/Seq.js";
import { tryItem } from "../../../src/.fable/fable-library.3.1.10/Array.js";

export class PopoverModule_Position extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Centered", "Top", "Bottom", "Left", "Right", "TopLeft", "TopRight", "BottomLeft", "BottomRight", "LeftTop", "LeftBottom", "RightTop", "RightBottom"];
    }
}

export function PopoverModule_Position$reflection() {
    return union_type("Tempo.Html.UI.PopoverModule.Position", [], PopoverModule_Position, () => [[], [], [], [], [], [], [], [], [], [], [], [], []]);
}

class PopoverImpl_Payload$3 extends Record {
    constructor(State, MaybeView) {
        super();
        this.State = State;
        this.MaybeView = MaybeView;
    }
}

function PopoverImpl_Payload$3$reflection(gen0, gen1, gen2) {
    return record_type("Tempo.Html.UI.PopoverImpl.Payload`3", [gen0, gen1, gen2], PopoverImpl_Payload$3, () => [["State", gen0], ["MaybeView", option_type(ComponentView$3$reflection(gen0, gen1, gen2))]]);
}

class PopoverImpl_Coords extends Record {
    constructor(X, Y) {
        super();
        this.X = X;
        this.Y = Y;
    }
}

function PopoverImpl_Coords$reflection() {
    return record_type("Tempo.Html.UI.PopoverImpl.Coords", [], PopoverImpl_Coords, () => [["X", float64_type], ["Y", float64_type]]);
}

function PopoverImpl_makeCalculatePosition(position, distance) {
    switch (position.tag) {
        case 1: {
            return (ref_1) => ((target_1) => {
                const x_1 = ref_1.left + ((ref_1.width - target_1.width) / 2);
                const y_1 = (ref_1.top - target_1.height) - distance;
                return new PopoverImpl_Coords(x_1, y_1);
            });
        }
        case 2: {
            return (ref_2) => ((target_2) => {
                const x_2 = ref_2.left + ((ref_2.width - target_2.width) / 2);
                const y_2 = ref_2.bottom + distance;
                return new PopoverImpl_Coords(x_2, y_2);
            });
        }
        case 3: {
            return (ref_3) => ((target_3) => {
                const x_3 = (ref_3.left - target_3.width) - distance;
                const y_3 = ref_3.top + ((ref_3.height - target_3.height) / 2);
                return new PopoverImpl_Coords(x_3, y_3);
            });
        }
        case 4: {
            return (ref_4) => ((target_4) => {
                const x_4 = ref_4.right + distance;
                const y_4 = ref_4.top + ((ref_4.height - target_4.height) / 2);
                return new PopoverImpl_Coords(x_4, y_4);
            });
        }
        case 5: {
            return (ref_5) => ((target_5) => {
                const x_5 = ref_5.left;
                const y_5 = (ref_5.top - target_5.height) - distance;
                return new PopoverImpl_Coords(x_5, y_5);
            });
        }
        case 6: {
            return (ref_6) => ((target_6) => {
                const x_6 = ref_6.right - target_6.width;
                const y_6 = (ref_6.top - target_6.height) - distance;
                return new PopoverImpl_Coords(x_6, y_6);
            });
        }
        case 7: {
            return (ref_7) => ((target_7) => {
                const x_7 = ref_7.left;
                const y_7 = ref_7.bottom + distance;
                return new PopoverImpl_Coords(x_7, y_7);
            });
        }
        case 8: {
            return (ref_8) => ((target_8) => {
                const x_8 = ref_8.right - target_8.width;
                const y_8 = ref_8.bottom + distance;
                return new PopoverImpl_Coords(x_8, y_8);
            });
        }
        case 9: {
            return (ref_9) => ((target_9) => {
                const x_9 = (ref_9.left - target_9.width) - distance;
                const y_9 = ref_9.top;
                return new PopoverImpl_Coords(x_9, y_9);
            });
        }
        case 10: {
            return (ref_10) => ((target_10) => {
                const x_10 = (ref_10.left - target_10.width) - distance;
                const y_10 = ref_10.bottom - target_10.height;
                return new PopoverImpl_Coords(x_10, y_10);
            });
        }
        case 11: {
            return (ref_11) => ((target_11) => {
                const x_11 = ref_11.right + distance;
                const y_11 = ref_11.top;
                return new PopoverImpl_Coords(x_11, y_11);
            });
        }
        case 12: {
            return (ref_12) => ((target_12) => {
                const x_12 = ref_12.right + distance;
                const y_12 = ref_12.bottom - target_12.height;
                return new PopoverImpl_Coords(x_12, y_12);
            });
        }
        default: {
            return (ref) => ((target) => {
                const x = ref.left + ((ref.width - target.width) / 2);
                const y = ref.top + ((ref.height - target.height) / 2);
                return new PopoverImpl_Coords(x, y);
            });
        }
    }
}

export class Popover {
    constructor() {
    }
}

export function Popover$reflection() {
    return class_type("Tempo.Html.UI.Popover", void 0, Popover);
}

export function Popover_MakeAttr_2F44AA22(panel, position, triggeringEvents, closingEvents, distance, container, startOpen, closeOnAction) {
    const position_1 = defaultArg(position, new PopoverModule_Position(7));
    const distance_1 = defaultArg(distance, 2);
    const triggeringEvents_1 = defaultArg(triggeringEvents, singleton("click"));
    const closingEvents_1 = defaultArg(closingEvents, ofArray(["mousedown", "keyup"]));
    const container_1 = defaultArg(container, document.body);
    const startOpen_1 = defaultArg(startOpen, (_arg1) => false);
    const closeOnAction_1 = defaultArg(closeOnAction, (_arg2) => true);
    const calcPosition = PopoverImpl_makeCalculatePosition(position_1, distance_1);
    const calcPosition_1 = (ref, target) => calcPosition(ref.getBoundingClientRect())(target.getBoundingClientRect());
    const applyPositioning = (trigger, panel_1) => {
        const patternInput = calcPosition_1(trigger, panel_1);
        const y = patternInput.Y;
        const x = patternInput.X;
        panel_1.style["left"] = toText(interpolate("%P()px", [x]));
        panel_1.style["top"] = toText(interpolate("%P()px", [y]));
    };
    const wireReposition = (button) => ((container_2) => {
        const apply = () => {
            applyPositioning(button, container_2);
        };
        const rObserver = new ResizeObserver(((_arg4, _arg3) => {
            apply();
        }), {root: undefined, rootMargin: undefined, threshold: undefined});
        const ancestors = collectElementAndAncestors(button);
        iterate((el) => {
            el.addEventListener("scroll", (_arg5) => {
                apply();
            });
            rObserver.observe(el);
        }, ancestors);
        const remove = () => {
            rObserver.disconnect();
            iterate((i) => {
                i.removeEventListener("scroll", (_arg6) => {
                    apply();
                });
            }, ancestors);
        };
        return remove;
    });
    const makePanelView = (payload, dispatch, parent) => {
        const update = (s, _arg1_1) => s;
        const middleware = (_arg2_1) => {
            const action = _arg2_1.Action;
            dispatch(action);
        };
        const container_3 = document.createElement("div");
        container_3.style["position"] = "absolute";
        void parent.appendChild(container_3);
        const view = DSL_MakeProgram_1C1F9AE9(panel, container_3)(update)(middleware)(payload.State);
        return [container_3, new ComponentView$3(view.Impl, view.Dispatch, view.Change, () => {
            remove_1(container_3);
            view.Destroy();
        }, view.Query)];
    };
    return lifecycleAttribute((_arg7) => {
        const state = _arg7.State;
        const dispatch_1 = _arg7.Dispatch;
        const button_1 = _arg7.Element;
        const payload_1 = new PopoverImpl_Payload$3(state, void 0);
        const openPopover = (ev) => {
            document.activeElement.blur();
            ev.cancelBubble = true;
            const patternInput_1 = makePanelView(payload_1, dispatch_1, container_1);
            const view_1 = patternInput_1[1];
            const containerEl = patternInput_1[0];
            applyPositioning(button_1, containerEl);
            let removeWiring;
            const clo2 = wireReposition(button_1)(containerEl);
            removeWiring = (() => {
                clo2();
            });
            const closePopover = (ev_1) => {
                if (!targetHasSpecifiedAncestor(ev_1.target, containerEl)) {
                    removeWiring();
                    iterate((te) => {
                        button_1.addEventListener(te, openPopover);
                    }, triggeringEvents_1);
                    iterate((ce) => {
                        document.removeEventListener(ce, closePopover);
                    }, closingEvents_1);
                    payload_1.MaybeView = (void 0);
                    button_1.focus();
                    view_1.Destroy();
                }
            };
            iterate((te_1) => {
                button_1.removeEventListener(te_1, openPopover);
            }, triggeringEvents_1);
            iterate((ce_1) => {
                document.addEventListener(ce_1, closePopover);
            }, closingEvents_1);
            button_1.addEventListener("click", closePopover);
            payload_1.MaybeView = view_1;
            iterate_1((el_1) => {
                el_1.focus();
            }, toArray(tryItem(0, getFocusable(containerEl))));
        };
        iterate((te_2) => {
            button_1.addEventListener(te_2, openPopover);
        }, triggeringEvents_1);
        return payload_1;
    }, (_arg12) => {
        const payload_2 = _arg12.Payload;
        return [true, payload_2];
    }, (_arg8) => {
        const state_1 = _arg8.State;
        const p = _arg8.Payload;
        p.State = state_1;
        iterate_1((v) => {
            v.Change(state_1);
        }, toArray(p.MaybeView));
        return p;
    }, (_arg9) => {
        const p_1 = _arg9.Payload;
        iterate_1((v_1) => {
            v_1.Destroy();
        }, toArray(p_1.MaybeView));
    }, (_arg11, _arg10) => {
        const payload_3 = _arg10.Payload;
        return payload_3;
    });
}

