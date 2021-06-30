import { Union } from "./.fable/fable-library.3.1.10/Types.js";
import { lambda_type, unit_type, string_type, union_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { createAtom } from "./.fable/fable-library.3.1.10/Util.js";
import { transformHtml } from "./HtmlEncoder.fs.js";
import { DSL_Component_Z228F47D0, DSL_Attr_Z384F8060, DSL_El_Z7374416F } from "../Tempo.Html/HtmlDSL.fs.js";
import { empty, ofArray, singleton } from "./.fable/fable-library.3.1.10/List.js";
import { MonacoEditorAttribute } from "./MonacoEditor.fs.js";

export class Html2TempoAction extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["HtmlPasted", "HtmlChanged"];
    }
}

export function Html2TempoAction$reflection() {
    return union_type("Tempo.Demo.Html2Tempo.View.Html2TempoAction", [], Html2TempoAction, () => [[], []]);
}

export class Html2TempoQuery extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["GetHtml", "SetGenerated"];
    }
}

export function Html2TempoQuery$reflection() {
    return union_type("Tempo.Demo.Html2Tempo.View.Html2TempoQuery", [], Html2TempoQuery, () => [[["Item", lambda_type(string_type, unit_type)]], [["Item", string_type]]]);
}

export function update(state, action) {
}

export const timerId = createAtom(void 0);

export function middleware(_arg1) {
    const query = _arg1.Query;
    const prev = _arg1.Previous;
    const current = _arg1.Current;
    const action = _arg1.Action;
    const transformRoundtrip = () => {
        query(new Html2TempoQuery(0, (arg_1) => {
            query(new Html2TempoQuery(1, transformHtml(arg_1)));
        }));
    };
    if (action.tag === 1) {
        if (timerId() == null) {
        }
        else {
            const id = timerId();
            window.clearTimeout(id);
        }
        timerId(window.setTimeout((_arg1_1) => {
            timerId(void 0, true);
            transformRoundtrip();
        }, 200), true);
    }
    else {
        transformRoundtrip();
    }
}

export const sample = "\u003cdiv class=\"relative w-screen max-w-md\"\u003e\n    \u003cdiv class=\"h-full flex flex-col py-6 bg-white shadow-xl overflow-y-scroll\"\u003e\n        \u003cdiv class=\"px-4 sm:px-6\"\u003e\n        \u003ch2 class=\"text-lg font-medium text-gray-900\" id=\"slide-over-title\"\u003e\n            Panel title\n        \u003c/h2\u003e\n        \u003c/div\u003e\n        \u003cdiv class=\"mt-6 relative flex-1 px-4 sm:px-6\"\u003e\n            \u003c!-- Replace with your content --\u003e\n            \u003cdiv class=\"absolute inset-0 px-4 sm:px-6\"\u003e\n                \u003cdiv class=\"h-full border-2 border-dashed border-gray-200\" aria-hidden=\"true\"\u003e\u003c/div\u003e\n            \u003c/div\u003e\n            \u003c!-- /End replace --\u003e\n        \u003c/div\u003e\n    \u003c/div\u003e\n\u003c/div\u003e\n    ";

export const template = DSL_El_Z7374416F("div", singleton(DSL_Attr_Z384F8060("class", "flex h-full")), ofArray([DSL_El_Z7374416F("div", ofArray([DSL_Attr_Z384F8060("class", "flex-auto h-full w-6/12"), MonacoEditorAttribute(() => ({
    language: "html",
    value: sample,
    wordWrap: "on",
}), (_arg1) => ((_arg1.tag === 1) ? (new Html2TempoAction(1)) : (new Html2TempoAction(0))), (request, editor) => {
    if (request.tag === 1) {
    }
    else {
        const f = request.fields[0];
        f(editor.getValue());
    }
})]), empty()), DSL_El_Z7374416F("div", ofArray([DSL_Attr_Z384F8060("class", "flex-auto h-full w-6/12"), MonacoEditorAttribute(() => ({
    language: "fsharp",
    value: transformHtml(sample),
    wordWrap: "on",
}), (e) => (void 0), (request_1, editor_1) => {
    if (request_1.tag === 1) {
        const content = request_1.fields[0];
        editor_1.setValue(content);
    }
})]), empty())]));

export const comp = DSL_Component_Z228F47D0((state, action) => {
    update(void 0, action);
}, (arg00$0040) => {
    middleware(arg00$0040);
}, template);

