import { Record, Union } from "../../../src/.fable/fable-library.3.1.10/Types.js";
import { lambda_type, unit_type, float64_type, record_type, class_type, bool_type, union_type } from "../../../src/.fable/fable-library.3.1.10/Reflection.js";
import { DSL_Attr_3DF4EB53, DSL_DispatchOn_322CD462, DSL_Portal_10AF510E, DSL_Attr_Z384F8060, DSL_On_47AABEE2, DSL_El_Z7374416F, DSL_Component_71622930, DSL_MakeCaptureSA } from "../Tempo.Html/Html.DSL.fs.js";
import { collectElementAndAncestors } from "../Tempo.Html/Html.Tools.fs.js";
import { singleton, ofArray, iterate } from "../../../src/.fable/fable-library.3.1.10/List.js";
import { Template$4, map } from "../Tempo.Core/Core.fs.js";
import { some } from "../../../src/.fable/fable-library.3.1.10/Option.js";
import { lifecycleAttribute } from "../Tempo.Html/Html.Impl.fs.js";
import { interpolate, toText } from "../../../src/.fable/fable-library.3.1.10/String.js";
import { FSharpChoice$2 } from "../../../src/.fable/fable-library.3.1.10/Choice.js";

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

class PopoverImpl_PopoverState extends Record {
    constructor(Open, Rect) {
        super();
        this.Open = Open;
        this.Rect = Rect;
    }
}

function PopoverImpl_PopoverState$reflection() {
    return record_type("Tempo.Html.UI.PopoverImpl.PopoverState", [], PopoverImpl_PopoverState, () => [["Open", bool_type], ["Rect", class_type("Browser.Types.ClientRect")]]);
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

class PopoverImpl_PopoverAction extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Open", "Close", "Reposition"];
    }
}

function PopoverImpl_PopoverAction$reflection() {
    return union_type("Tempo.Html.UI.PopoverImpl.PopoverAction", [], PopoverImpl_PopoverAction, () => [[], [], [["Item", class_type("Browser.Types.ClientRect")]]]);
}

class PopoverImpl_PopoverPayload extends Record {
    constructor(ManageClickDoc, RemoveRepositionHandlers) {
        super();
        this.ManageClickDoc = ManageClickDoc;
        this.RemoveRepositionHandlers = RemoveRepositionHandlers;
    }
}

function PopoverImpl_PopoverPayload$reflection() {
    return record_type("Tempo.Html.UI.PopoverImpl.PopoverPayload", [], PopoverImpl_PopoverPayload, () => [["ManageClickDoc", lambda_type(unit_type, unit_type)], ["RemoveRepositionHandlers", lambda_type(unit_type, unit_type)]]);
}

function PopoverImpl_makeCalculatePosition(position, distance) {
    switch (position.tag) {
        case 1: {
            return (reference_1) => ((target_1) => {
                const x_1 = reference_1.left + ((reference_1.width - target_1.width) / 2);
                const y_1 = (reference_1.top - target_1.height) - distance;
                return new PopoverImpl_Coords(x_1, y_1);
            });
        }
        case 2: {
            return (reference_2) => ((target_2) => {
                const x_2 = reference_2.left + ((reference_2.width - target_2.width) / 2);
                const y_2 = reference_2.bottom + distance;
                return new PopoverImpl_Coords(x_2, y_2);
            });
        }
        case 3: {
            return (reference_3) => ((target_3) => {
                const x_3 = (reference_3.left - target_3.width) - distance;
                const y_3 = reference_3.top + ((reference_3.height - target_3.height) / 2);
                return new PopoverImpl_Coords(x_3, y_3);
            });
        }
        case 4: {
            return (reference_4) => ((target_4) => {
                const x_4 = reference_4.right + distance;
                const y_4 = reference_4.top + ((reference_4.height - target_4.height) / 2);
                return new PopoverImpl_Coords(x_4, y_4);
            });
        }
        case 5: {
            return (reference_5) => ((target_5) => {
                const x_5 = reference_5.left;
                const y_5 = (reference_5.top - target_5.height) - distance;
                return new PopoverImpl_Coords(x_5, y_5);
            });
        }
        case 6: {
            return (reference_6) => ((target_6) => {
                const x_6 = reference_6.right - target_6.width;
                const y_6 = (reference_6.top - target_6.height) - distance;
                return new PopoverImpl_Coords(x_6, y_6);
            });
        }
        case 7: {
            return (reference_7) => ((target_7) => {
                const x_7 = reference_7.left;
                const y_7 = reference_7.bottom + distance;
                return new PopoverImpl_Coords(x_7, y_7);
            });
        }
        case 8: {
            return (reference_8) => ((target_8) => {
                const x_8 = reference_8.right - target_8.width;
                const y_8 = reference_8.bottom + distance;
                return new PopoverImpl_Coords(x_8, y_8);
            });
        }
        case 9: {
            return (reference_9) => ((target_9) => {
                const x_9 = (reference_9.left - target_9.width) - distance;
                const y_9 = reference_9.top;
                return new PopoverImpl_Coords(x_9, y_9);
            });
        }
        case 10: {
            return (reference_10) => ((target_10) => {
                const x_10 = (reference_10.left - target_10.width) - distance;
                const y_10 = reference_10.bottom - target_10.height;
                return new PopoverImpl_Coords(x_10, y_10);
            });
        }
        case 11: {
            return (reference_11) => ((target_11) => {
                const x_11 = reference_11.right + distance;
                const y_11 = reference_11.top;
                return new PopoverImpl_Coords(x_11, y_11);
            });
        }
        case 12: {
            return (reference_12) => ((target_12) => {
                const x_12 = reference_12.right + distance;
                const y_12 = reference_12.bottom - target_12.height;
                return new PopoverImpl_Coords(x_12, y_12);
            });
        }
        default: {
            return (reference) => ((target) => {
                const x = reference.left + ((reference.width - target.width) / 2);
                const y = reference.top + ((reference.height - target.height) / 2);
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

export function Popover_Make_Z295A2382(position, trigger, panel, buttonClass) {
    return Popover_Make_21A43445(position, 2, trigger, panel, buttonClass);
}

export function Popover_Make_21A43445(position, distance, trigger, panel, buttonClass) {
    const patternInput = DSL_MakeCaptureSA();
    const release = patternInput[1];
    const hold = patternInput[0];
    const calcPosition = PopoverImpl_makeCalculatePosition(position, distance);
    const calcPosition_1 = (ref, target) => calcPosition(ref)(target.getBoundingClientRect());
    const makeCloseOnClickOutsideImpl = (el) => ((dispatch) => {
        const f = (ev) => {
            let k;
            remove();
            const matchValue = ev["key"];
            if (matchValue != null) {
                if (k = matchValue, (k === "Escape") ? true : (k === "Esc")) {
                    const k_1 = matchValue;
                    dispatch(new PopoverImpl_PopoverAction(1));
                }
                else if (matchValue != null) {
                }
                else {
                    throw (new Error("Match failure"));
                }
            }
            else {
                dispatch(new PopoverImpl_PopoverAction(1));
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
    });
    const makeCloseOnClickOutside = (isOpen) => ((el_1) => ((dispatch_1) => (isOpen ? makeCloseOnClickOutsideImpl(el_1)(dispatch_1) : (() => {
    }))));
    const makeReposition = (el_2) => ((dispatch_2) => {
        const update = (_arg1) => {
            const rect = el_2.getBoundingClientRect();
            dispatch_2(new PopoverImpl_PopoverAction(2, rect));
        };
        const rObserver = new ResizeObserver(((_arg2, _arg1_1) => {
            update();
        }), {root: undefined, rootMargin: undefined, threshold: undefined});
        const ancestors = collectElementAndAncestors(el_2);
        iterate((el_3) => {
            el_3.addEventListener("scroll", update);
            rObserver.observe(el_3);
        }, ancestors);
        const remove_1 = () => {
            rObserver.disconnect();
            iterate((i) => {
                i.removeEventListener("scroll", update);
            }, ancestors);
        };
        return remove_1;
    });
    return hold(map((x_2) => x_2, (_arg3) => (new PopoverImpl_PopoverState(false, document.body.getBoundingClientRect())), (_arg4) => (void 0), (arg0_5) => some(arg0_5), DSL_Component_71622930((s, a) => {
        switch (a.tag) {
            case 1: {
                return new PopoverImpl_PopoverState(false, s.Rect);
            }
            case 2: {
                const rect_1 = a.fields[0];
                return new PopoverImpl_PopoverState(s.Open, rect_1);
            }
            default: {
                return new PopoverImpl_PopoverState(true, s.Rect);
            }
        }
    }, new Template$4(1, ofArray([DSL_El_Z7374416F("button", ofArray([lifecycleAttribute((_arg5) => {
        const isOpen_1 = _arg5.State.Open;
        const el_4 = _arg5.Element;
        const dispatch_3 = _arg5.Dispatch;
        let manageClickDoc;
        const clo3 = makeCloseOnClickOutside(isOpen_1)(el_4)(dispatch_3);
        manageClickDoc = (() => {
            clo3();
        });
        const removeRepositionHandlers = makeReposition(el_4)(dispatch_3);
        return new PopoverImpl_PopoverPayload(manageClickDoc, removeRepositionHandlers);
    }, (_arg11) => {
        const payload_2 = _arg11.Payload;
        return [true, payload_2];
    }, (_arg6) => {
        const payload = _arg6.Payload;
        const isOpen_2 = _arg6.State.Open;
        const el_5 = _arg6.Element;
        const dispatch_4 = _arg6.Dispatch;
        payload.ManageClickDoc();
        let manageClickDoc_1;
        const clo3_1 = makeCloseOnClickOutside(isOpen_2)(el_5)(dispatch_4);
        manageClickDoc_1 = (() => {
            clo3_1();
        });
        return new PopoverImpl_PopoverPayload(manageClickDoc_1, payload.RemoveRepositionHandlers);
    }, (_arg7) => {
        const payload_1 = _arg7.Payload;
        payload_1.ManageClickDoc();
        payload_1.RemoveRepositionHandlers();
    }, (_arg10, _arg9) => {
        const payload_3 = _arg9.Payload;
        return payload_3;
    }), DSL_On_47AABEE2("click", (_arg8) => {
        const isOpen_3 = _arg8.State.Open;
        return isOpen_3 ? (new PopoverImpl_PopoverAction(1)) : (new PopoverImpl_PopoverAction(0));
    }), DSL_Attr_Z384F8060("type", "button"), DSL_Attr_Z384F8060("class", buttonClass), DSL_Attr_Z384F8060(toText(interpolate("aria-%P()", ["expanded"])), "true"), DSL_Attr_Z384F8060(toText(interpolate("aria-%P()", ["haspopup"])), "true")]), singleton(release([(s_1) => ((_arg9_1) => s_1), (arg) => (new FSharpChoice$2(0, arg)), trigger]))), DSL_Portal_10AF510E("body", DSL_El_Z7374416F("div", ofArray([DSL_DispatchOn_322CD462("click", (_arg11_1, _arg10_1) => {
        const event = _arg11_1.Event;
        let copyOfStruct = event;
        copyOfStruct.cancelBubble = true;
        let copyOfStruct_1 = event;
        copyOfStruct_1.preventDefault();
    }), DSL_Attr_3DF4EB53("style", (_arg12) => {
        const isOpen_4 = _arg12.Open;
        return isOpen_4 ? "position: absolute" : "display: none";
    }), lifecycleAttribute((_arg13) => {
        const target_1 = _arg13.Element;
        const reference = _arg13.State.Rect;
        const patternInput_1 = calcPosition_1(reference, target_1);
        const y = patternInput_1.Y;
        const x = patternInput_1.X;
        target_1.style["top"] = toText(interpolate("%P()px", [y]));
        target_1.style["left"] = toText(interpolate("%P()px", [x]));
    }, (_arg11_2) => {
        const payload_4 = _arg11_2.Payload;
        return [true, void 0];
    }, (_arg14) => {
        const target_2 = _arg14.Element;
        const reference_1 = _arg14.State.Rect;
        const patternInput_2 = calcPosition_1(reference_1, target_2);
        const y_1 = patternInput_2.Y;
        const x_1 = patternInput_2.X;
        target_2.style["top"] = toText(interpolate("%P()px", [y_1]));
        target_2.style["left"] = toText(interpolate("%P()px", [x_1]));
    }, (value_2) => {
    }, (_arg10_2, _arg9_2) => {
        const payload_5 = _arg9_2.Payload;
    })]), singleton(release([(s_2) => ((_arg15) => s_2), (arg_1) => (new FSharpChoice$2(0, arg_1)), panel]))))])))));
}

