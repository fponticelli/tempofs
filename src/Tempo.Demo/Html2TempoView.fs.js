import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { lambda_type, unit_type, string_type, record_type, union_type, bool_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { createAtom } from "./.fable/fable-library.3.1.10/Util.js";
import { transformHtml } from "./HtmlEncoder.fs.js";
import { DSL_Component_Z228F47D0, DSL_NSEl_7639458A, DSL_Text_Z721C83C5, DSL_Attr_30230F9B, DSL_On_47AABEE2, DSL_Attr_Z384F8060, DSL_El_Z7374416F } from "../Tempo.Html/HtmlDSL.fs.js";
import { empty, ofArray, singleton } from "./.fable/fable-library.3.1.10/List.js";
import { Template$4, map } from "../Tempo.Core/Core.fs.js";
import { interpolate, toText } from "./.fable/fable-library.3.1.10/String.js";
import { MonacoEditorAttribute } from "./MonacoEditor.fs.js";

export class Html2TempoAction extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["HtmlPasted", "HtmlChanged", "SetFilterComments"];
    }
}

export function Html2TempoAction$reflection() {
    return union_type("Tempo.Demo.Html2Tempo.View.Html2TempoAction", [], Html2TempoAction, () => [[], [], [["Item", bool_type]]]);
}

export class Html2TempoState extends Record {
    constructor(FilterComments) {
        super();
        this.FilterComments = FilterComments;
    }
}

export function Html2TempoState$reflection() {
    return record_type("Tempo.Demo.Html2Tempo.View.Html2TempoState", [], Html2TempoState, () => [["FilterComments", bool_type]]);
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
    switch (action.tag) {
        case 0:
        case 1: {
            return state;
        }
        default: {
            return new Html2TempoState(action.fields[0]);
        }
    }
}

export const timerId = createAtom(void 0);

export function middleware(_arg1) {
    const query = _arg1.Query;
    const filterComments = _arg1.Current.FilterComments;
    const action = _arg1.Action;
    const transformRoundtrip = (filterComments_1) => {
        query(new Html2TempoQuery(0, (arg_1) => {
            query(new Html2TempoQuery(1, transformHtml(filterComments_1, arg_1)));
        }));
    };
    switch (action.tag) {
        case 1: {
            if (timerId() == null) {
            }
            else {
                window.clearTimeout(timerId());
            }
            timerId(window.setTimeout((_arg1_1) => {
                timerId(void 0, true);
                transformRoundtrip(filterComments);
            }, 200), true);
            break;
        }
        case 2: {
            transformRoundtrip(filterComments);
            break;
        }
        default: {
            transformRoundtrip(filterComments);
        }
    }
}

export const sample = "\u003cdiv class=\"relative w-screen max-w-md\"\u003e\n    \u003cdiv class=\"h-full flex flex-col py-6 bg-white shadow-xl overflow-y-scroll\"\u003e\n        \u003cdiv class=\"px-4 sm:px-6\"\u003e\n        \u003ch2 class=\"text-lg font-medium text-gray-900\" id=\"slide-over-title\"\u003e\n            Panel title\n        \u003c/h2\u003e\n        \u003c/div\u003e\n        \u003cdiv class=\"mt-6 relative flex-1 px-4 sm:px-6\"\u003e\n            \u003c!-- Replace with your content --\u003e\n            \u003cdiv class=\"absolute inset-0 px-4 sm:px-6\"\u003e\n                \u003cdiv class=\"h-full border-2 border-dashed border-gray-200\" aria-hidden=\"true\"\u003e\u003c/div\u003e\n            \u003c/div\u003e\n            \u003c!-- /End replace --\u003e\n        \u003c/div\u003e\n    \u003c/div\u003e\n\u003c/div\u003e\n    ";

export const bar = DSL_El_Z7374416F("header", singleton(DSL_Attr_Z384F8060("class", "bg-white shadow-sm lg:static lg:overflow-y-visible")), singleton(DSL_El_Z7374416F("div", singleton(DSL_Attr_Z384F8060("class", "max-w-7xl mx-auto px-4 sm:px-6 lg:px-8")), singleton(DSL_El_Z7374416F("div", singleton(DSL_Attr_Z384F8060("class", "relative flex justify-between xl:grid xl:grid-cols-12 lg:gap-8")), singleton(DSL_El_Z7374416F("div", singleton(DSL_Attr_Z384F8060("class", "min-w-0 flex-1 md:px-8 lg:px-0 xl:col-span-6")), singleton(DSL_El_Z7374416F("div", singleton(DSL_Attr_Z384F8060("class", "flex items-center px-6 py-4 md:max-w-3xl md:mx-auto lg:max-w-none lg:mx-0 xl:px-0")), singleton(DSL_El_Z7374416F("div", singleton(DSL_Attr_Z384F8060("class", "w-full")), singleton(DSL_El_Z7374416F("div", singleton(DSL_Attr_Z384F8060("class", "relative")), singleton(DSL_El_Z7374416F("div", singleton(DSL_Attr_Z384F8060("class", "flex items-center")), singleton(map((x) => x, (_arg1) => _arg1.FilterComments, (arg0_1) => arg0_1, (x_1) => x_1, new Template$4(1, ofArray([DSL_El_Z7374416F("button", ofArray([DSL_Attr_Z384F8060("id", "filter-comments"), DSL_Attr_Z384F8060("type", "button"), DSL_On_47AABEE2("click", (_arg4) => (new Html2TempoAction(2, !_arg4.State))), DSL_Attr_Z384F8060("class", "relative inline-flex flex-shrink-0 h-6 w-11 border-2 border-transparent rounded-full cursor-pointer transition-colors ease-in-out duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"), DSL_Attr_30230F9B("class", "bg-indigo-600", "bg-gray-200 focus:bg-gray-200"), DSL_Attr_Z384F8060("role", "switch"), DSL_Attr_30230F9B(toText(interpolate("aria-%P()", ["checked"])), "true", "false")]), ofArray([DSL_El_Z7374416F("span", singleton(DSL_Attr_Z384F8060("class", "sr-only")), singleton(DSL_Text_Z721C83C5("Filter-out comments"))), DSL_El_Z7374416F("span", ofArray([DSL_Attr_Z384F8060("class", "pointer-events-none relative inline-block h-5 w-5 rounded-full bg-white shadow transform ring-0 transition ease-in-out duration-200"), DSL_Attr_30230F9B("class", "translate-x-5", "translate-x-0")]), ofArray([DSL_El_Z7374416F("span", ofArray([DSL_Attr_Z384F8060("class", "absolute inset-0 h-full w-full flex items-center justify-center transition-opacity"), DSL_Attr_30230F9B("class", "opacity-0 ease-out duration-100", "opacity-100 ease-in duration-200"), DSL_Attr_Z384F8060(toText(interpolate("aria-%P()", ["hidden"])), "true")]), singleton(DSL_NSEl_7639458A("http://www.w3.org/2000/svg", "svg", ofArray([DSL_Attr_Z384F8060("class", "h-3 w-3 text-gray-400"), DSL_Attr_Z384F8060("fill", "none"), DSL_Attr_Z384F8060("viewBox", "0 0 12 12")]), singleton(DSL_NSEl_7639458A("http://www.w3.org/2000/svg", "path", ofArray([DSL_Attr_Z384F8060("d", "M4 8l2-2m0 0l2-2M6 6L4 4m2 2l2 2"), DSL_Attr_Z384F8060("stroke", "currentColor"), DSL_Attr_Z384F8060("stroke-width", "2"), DSL_Attr_Z384F8060("stroke-linecap", "round"), DSL_Attr_Z384F8060("stroke-linejoin", "round")]), empty()))))), DSL_El_Z7374416F("span", ofArray([DSL_Attr_Z384F8060("class", "absolute inset-0 h-full w-full flex items-center justify-center transition-opacity"), DSL_Attr_30230F9B("class", "opacity-100 ease-in duration-200", "opacity-0 ease-out duration-100"), DSL_Attr_Z384F8060(toText(interpolate("aria-%P()", ["hidden"])), "true")]), singleton(DSL_NSEl_7639458A("http://www.w3.org/2000/svg", "svg", ofArray([DSL_Attr_Z384F8060("class", "h-3 w-3 text-indigo-600"), DSL_Attr_Z384F8060("fill", "currentColor"), DSL_Attr_Z384F8060("viewBox", "0 0 12 12")]), singleton(DSL_NSEl_7639458A("http://www.w3.org/2000/svg", "path", singleton(DSL_Attr_Z384F8060("d", "M3.707 5.293a1 1 0 00-1.414 1.414l1.414-1.414zM5 8l-.707.707a1 1 0 001.414 0L5 8zm4.707-3.293a1 1 0 00-1.414-1.414l1.414 1.414zm-7.414 2l2 2 1.414-1.414-2-2-1.414 1.414zm3.414 2l4-4-1.414-1.414-4 4 1.414 1.414z")), empty())))))]))])), DSL_El_Z7374416F("label", singleton(DSL_Attr_Z384F8060("for", "filter-comments")), singleton(DSL_El_Z7374416F("span", singleton(DSL_Attr_Z384F8060("class", "ml-3")), singleton(DSL_El_Z7374416F("span", singleton(DSL_Attr_Z384F8060("class", "text-sm font-medium text-gray-900")), singleton(DSL_Text_Z721C83C5("Filter-out Comments")))))))])))))))))))))))))));

export const template = DSL_El_Z7374416F("div", singleton(DSL_Attr_Z384F8060("class", "flex flex-col h-screen")), ofArray([bar, DSL_El_Z7374416F("div", singleton(DSL_Attr_Z384F8060("class", "flex h-full")), ofArray([DSL_El_Z7374416F("div", ofArray([DSL_Attr_Z384F8060("class", "flex-auto h-full w-6/12"), MonacoEditorAttribute((_arg1) => ({
    language: "html",
    value: sample,
    wordWrap: "on",
}), (_arg1_1) => ((_arg1_1.tag === 1) ? (new Html2TempoAction(1)) : (new Html2TempoAction(0))), (request, editor) => {
    if (request.tag === 1) {
    }
    else {
        request.fields[0](editor.getValue());
    }
})]), empty()), DSL_El_Z7374416F("div", ofArray([DSL_Attr_Z384F8060("class", "flex-auto h-full w-6/12"), MonacoEditorAttribute((_arg2) => ({
    language: "fsharp",
    value: transformHtml(_arg2.FilterComments, sample),
    wordWrap: "on",
}), (e) => (void 0), (request_1, editor_1) => {
    if (request_1.tag === 1) {
        editor_1.setValue(request_1.fields[0]);
    }
})]), empty())]))]));

export const comp = DSL_Component_Z228F47D0((state, action) => update(state, action), (arg00$0040) => {
    middleware(arg00$0040);
}, template);

