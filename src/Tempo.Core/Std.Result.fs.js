import { interpolate, toText } from "../../../src/.fable/fable-library.3.1.10/String.js";

export function iter(audit, res) {
    if (res.tag === 1) {
    }
    else {
        const v = res.fields[0];
        audit(v);
    }
}

export function get$(res) {
    if (res.tag === 1) {
        const e = res.fields[0];
        throw (new Error(toText(interpolate("%P()", [e]))));
    }
    else {
        const v = res.fields[0];
        return v;
    }
}

