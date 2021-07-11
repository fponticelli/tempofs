import { empty, cons } from "../../../src/.fable/fable-library.3.1.10/List.js";
import { equals } from "../../../src/.fable/fable-library.3.1.10/Util.js";

export function remove(n) {
    if (n instanceof HTMLElement) {
        const el = n;
        if ((el != null) ? ((() => {
            el.blur();
        }) != null) : false) {
            el["onblur"] = null;
        }
    }
    if (((n != null) ? (n.ownerDocument != null) : false) ? (n.parentNode != null) : false) {
        void n.parentNode.removeChild(n);
    }
}

export function collectElementAndAncestors(el) {
    const go = (el_1_mut, acc_mut) => {
        go:
        while (true) {
            const el_1 = el_1_mut, acc = acc_mut;
            if (el_1 == null) {
                return acc;
            }
            else {
                el_1_mut = el_1.parentElement;
                acc_mut = cons(el_1, acc);
                continue go;
            }
            break;
        }
    };
    return go(el, empty());
}

export function hasSpecifiedAncestor(element_mut, ancestor_mut) {
    hasSpecifiedAncestor:
    while (true) {
        const element = element_mut, ancestor = ancestor_mut;
        if (element == null) {
            return false;
        }
        else if (equals(element, ancestor)) {
            return true;
        }
        else {
            element_mut = element.parentElement;
            ancestor_mut = ancestor;
            continue hasSpecifiedAncestor;
        }
        break;
    }
}

export function targetHasSpecifiedAncestor(target, ancestor) {
    if (target["tagName"] !== null) {
        return hasSpecifiedAncestor(target, ancestor);
    }
    else {
        return false;
    }
}

