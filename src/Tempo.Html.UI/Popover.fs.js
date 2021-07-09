import { Record, Union } from "../../../src/.fable/fable-library.3.1.10/Types.js";
import { float64_type, record_type, lambda_type, unit_type, class_type, bool_type, union_type } from "../../../src/.fable/fable-library.3.1.10/Reflection.js";
import { defaultArg } from "../../../src/.fable/fable-library.3.1.10/Option.js";
import { ofArray, iterate, singleton } from "../../../src/.fable/fable-library.3.1.10/List.js";
import { collectElementAndAncestors } from "../Tempo.Html/Html.Tools.fs.js";
import { ComponentView$3, map } from "../Tempo.Core/Core.fs.js";
import { interpolate, toText } from "../../../src/.fable/fable-library.3.1.10/String.js";
import { DSL_MakeProgram_1C1F9AE9, DSL_Text_Z721C83C5, DSL_Attr_Z384F8060, DSL_El_Z7374416F, DSL_OneOf_Z491B0F3C } from "../Tempo.Html/Html.DSL.fs.js";
import { FSharpChoice$2 } from "../../../src/.fable/fable-library.3.1.10/Choice.js";
import { lifecycleAttribute } from "../Tempo.Html/Html.Impl.fs.js";

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

class PopoverImpl_State$2 extends Record {
    constructor(Open, TriggerElement, OuterState, OuterDispatch) {
        super();
        this.Open = Open;
        this.TriggerElement = TriggerElement;
        this.OuterState = OuterState;
        this.OuterDispatch = OuterDispatch;
    }
}

function PopoverImpl_State$2$reflection(gen0, gen1) {
    return record_type("Tempo.Html.UI.PopoverImpl.State`2", [gen0, gen1], PopoverImpl_State$2, () => [["Open", bool_type], ["TriggerElement", class_type("Browser.Types.Element")], ["OuterState", gen0], ["OuterDispatch", lambda_type(gen1, unit_type)]]);
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

class PopoverImpl_Action$2 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Open", "Close", "Toggle", "Reposition", "SetOuterState", "OuterActionTriggered"];
    }
}

function PopoverImpl_Action$2$reflection(gen0, gen1) {
    return union_type("Tempo.Html.UI.PopoverImpl.Action`2", [gen0, gen1], PopoverImpl_Action$2, () => [[], [], [], [], [["Item", gen0]], [["Item", gen1]]]);
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

export function Popover_MakeAttr_Z3D21D21A(panel, position, triggeringEvents, distance, container, startOpen, closeOnAction) {
    const position_1 = defaultArg(position, new PopoverModule_Position(7));
    const distance_1 = defaultArg(distance, 2);
    const triggeringEvents_1 = defaultArg(triggeringEvents, singleton("click"));
    const container_1 = defaultArg(container, document.body);
    const startOpen_1 = defaultArg(startOpen, (_arg1) => false);
    const closeOnAction_1 = defaultArg(closeOnAction, (_arg2) => true);
    const calcPosition = PopoverImpl_makeCalculatePosition(position_1, distance_1);
    const calcPosition_1 = (ref, target) => calcPosition(ref.getBoundingClientRect())(target.getBoundingClientRect());
    const makeCloseOnClickOutsideImpl = (dispatch) => {
        const f = (ev) => {
            let k;
            remove();
            const matchValue = ev["key"];
            if (matchValue != null) {
                if (k = matchValue, (k === "Escape") ? true : (k === "Esc")) {
                    const k_1 = matchValue;
                    dispatch(new PopoverImpl_Action$2(1));
                }
                else if (matchValue != null) {
                }
                else {
                    throw (new Error("Match failure"));
                }
            }
            else {
                dispatch(new PopoverImpl_Action$2(1));
            }
        };
        const remove = () => {
            document.removeEventListener("keyup", f);
            document.removeEventListener("click", f);
        };
        void window.setTimeout(() => {
            document.addEventListener("keyup", f);
            document.addEventListener("click", f);
        }, 0);
        return remove;
    };
    const makeCloseOnClickOutside = (isOpen) => ((dispatch_1) => (isOpen ? makeCloseOnClickOutsideImpl(dispatch_1) : (() => {
    })));
    const makeReposition = (el) => ((dispatch_2) => {
        const dispatchReposition = (_arg1_1) => {
            dispatch_2(new PopoverImpl_Action$2(3));
        };
        const rObserver = new ResizeObserver(((_arg4, _arg3) => {
            dispatchReposition();
        }), {root: undefined, rootMargin: undefined, threshold: undefined});
        const ancestors = collectElementAndAncestors(el);
        iterate((el_1) => {
            el_1.addEventListener("scroll", dispatchReposition);
            rObserver.observe(el_1);
        }, ancestors);
        const remove_1 = () => {
            rObserver.disconnect();
            iterate((i) => {
                i.removeEventListener("scroll", dispatchReposition);
            }, ancestors);
        };
        return remove_1;
    });
    const mapped = map((x) => x, (_arg5) => {
        const state = _arg5.OuterState;
        return state;
    }, (arg) => (new PopoverImpl_Action$2(5, arg)), () => (void 0), panel);
    const applyPositioning = (isOpen_1, trigger, panel_1) => {
        if (isOpen_1) {
            const patternInput = calcPosition_1(trigger, panel_1);
            const y = patternInput.Y;
            const x_1 = patternInput.X;
            panel_1.style["left"] = toText(interpolate("%P()px", [x_1]));
            panel_1.style["top"] = toText(interpolate("%P()px", [y]));
        }
    };
    const template_2 = DSL_OneOf_Z491B0F3C((s) => (s.Open ? (new FSharpChoice$2(0, s)) : (new FSharpChoice$2(1, void 0))), DSL_El_Z7374416F("div", ofArray([DSL_Attr_Z384F8060("style", "position: absolute"), lifecycleAttribute((_arg6) => {
        const trigger_1 = _arg6.State.TriggerElement;
        const panel_2 = _arg6.Element;
        const isOpen_2 = _arg6.State.Open;
        const dispatch_3 = _arg6.Dispatch;
        let copyOfStruct = panel_2;
        copyOfStruct.addEventListener("click", (event) => {
            event.cancelBubble = true;
            event.preventDefault();
        });
        applyPositioning(isOpen_2, trigger_1, panel_2);
        return makeCloseOnClickOutside(isOpen_2)(dispatch_3);
    }, (_arg12) => {
        const payload = _arg12.Payload;
        return [true, payload];
    }, (_arg7) => {
        const trigger_2 = _arg7.State.TriggerElement;
        const panel_3 = _arg7.Element;
        const manageClickDoc = _arg7.Payload;
        const isOpen_3 = _arg7.State.Open;
        const dispatch_4 = _arg7.Dispatch;
        applyPositioning(isOpen_3, trigger_2, panel_3);
        manageClickDoc();
        return makeCloseOnClickOutside(isOpen_3)(dispatch_4);
    }, (_arg8) => {
        const manageClickDoc_1 = _arg8.Payload;
        manageClickDoc_1();
    }, (_arg11, _arg10) => {
        const payload_1 = _arg10.Payload;
        return payload_1;
    })]), singleton(mapped)), DSL_Text_Z721C83C5(""));
    return lifecycleAttribute((_arg9) => {
        const state_1 = _arg9.State;
        const el_2 = _arg9.Element;
        const dispatch_5 = _arg9.Dispatch;
        const render = DSL_MakeProgram_1C1F9AE9(template_2, container_1);
        const update = (state_2, action) => {
            switch (action.tag) {
                case 0: {
                    return new PopoverImpl_State$2(true, state_2.TriggerElement, state_2.OuterState, state_2.OuterDispatch);
                }
                case 1: {
                    return new PopoverImpl_State$2(false, state_2.TriggerElement, state_2.OuterState, state_2.OuterDispatch);
                }
                case 4: {
                    const outs = action.fields[0];
                    return new PopoverImpl_State$2(state_2.Open, state_2.TriggerElement, outs, state_2.OuterDispatch);
                }
                case 5: {
                    const act = action.fields[0];
                    state_2.OuterDispatch(act);
                    if (closeOnAction_1(act)) {
                        return new PopoverImpl_State$2(false, state_2.TriggerElement, state_2.OuterState, state_2.OuterDispatch);
                    }
                    else {
                        return state_2;
                    }
                }
                case 3: {
                    return state_2;
                }
                default: {
                    return new PopoverImpl_State$2(!state_2.Open, state_2.TriggerElement, state_2.OuterState, state_2.OuterDispatch);
                }
            }
        };
        const view = render(update)((value_2) => {
        })(new PopoverImpl_State$2(startOpen_1(state_1), el_2, state_1, dispatch_5));
        const dispatchOpen = (_arg2_1) => {
            view.Dispatch(new PopoverImpl_Action$2(2));
        };
        iterate((te) => {
            el_2.addEventListener(te, dispatchOpen);
        }, triggeringEvents_1);
        const removeReposition = makeReposition(el_2)(view.Dispatch);
        const destroy = () => {
            removeReposition();
            iterate((te_1) => {
                el_2.removeEventListener(te_1, dispatchOpen);
            }, triggeringEvents_1);
            view.Destroy();
        };
        return new ComponentView$3(view.Impl, view.Dispatch, view.Change, destroy, view.Query);
    }, (_arg12_1) => {
        const payload_2 = _arg12_1.Payload;
        return [true, payload_2];
    }, (_arg10_1) => {
        const view_1 = _arg10_1.Payload;
        const state_3 = _arg10_1.State;
        view_1.Dispatch(new PopoverImpl_Action$2(4, state_3));
        return view_1;
    }, (_arg11_1) => {
        const view_2 = _arg11_1.Payload;
        view_2.Destroy();
    }, (_arg11_2, _arg10_2) => {
        const payload_3 = _arg10_2.Payload;
        return payload_3;
    });
}

