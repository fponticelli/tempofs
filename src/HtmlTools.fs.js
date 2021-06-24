
export function remove(n) {
    if (n instanceof HTMLElement) {
        const el = n;
        if ((el != null) ? ((() => {
            el.blur();
        }) != null) : false) {
            void (el["onblur"] = null);
        }
    }
    if (((n != null) ? (n.ownerDocument != null) : false) ? (n.parentNode != null) : false) {
        void n.parentNode.removeChild(n);
    }
}

