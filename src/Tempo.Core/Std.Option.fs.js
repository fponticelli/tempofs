import { value as value_2, defaultArg } from "../../../src/.fable/fable-library.3.1.10/Option.js";
import { empty, cons, fold } from "../../../src/.fable/fable-library.3.1.10/List.js";

export function ofString(value) {
    if (value === "") {
        return void 0;
    }
    else {
        return value;
    }
}

export function ofTrimmedString(value) {
    return ofString(value.trim());
}

export function asString(value) {
    return defaultArg(value, "");
}

export function ofOptionList(ls) {
    return fold((acc, opt) => {
        const matchValue = [acc, opt];
        let pattern_matching_result, ls_1, v;
        if (matchValue[0] != null) {
            if (matchValue[1] != null) {
                pattern_matching_result = 0;
                ls_1 = matchValue[0];
                v = value_2(matchValue[1]);
            }
            else {
                pattern_matching_result = 1;
            }
        }
        else {
            pattern_matching_result = 1;
        }
        switch (pattern_matching_result) {
            case 0: {
                return cons(v, ls_1);
            }
            case 1: {
                return void 0;
            }
        }
    }, empty(), ls);
}

export function toBool(value) {
    if (value == null) {
        return false;
    }
    else {
        return true;
    }
}

