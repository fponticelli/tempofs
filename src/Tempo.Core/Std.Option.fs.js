import { defaultArg } from "../Tempo.Demo/.fable/fable-library.3.1.10/Option.js";

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

export function toBool(value) {
    if (value == null) {
        return false;
    }
    else {
        return true;
    }
}

