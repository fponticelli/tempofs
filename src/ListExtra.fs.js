import { empty, cons, foldBack } from "./.fable/fable-library.3.1.10/List.js";
import { value } from "./.fable/fable-library.3.1.10/Option.js";

export function filterMap(f, ls) {
    return foldBack((curr, acc) => {
        const matchValue = f(curr);
        return (matchValue == null) ? acc : cons(value(matchValue), acc);
    }, ls, empty());
}

