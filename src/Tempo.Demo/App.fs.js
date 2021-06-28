import { Html2TempoState, update, template as template_1 } from "./Html2TempoView.fs.js";
import { interpolate, toText } from "./.fable/fable-library.3.1.10/String.js";
import { some } from "./.fable/fable-library.3.1.10/Option.js";
import { HTML_MakeProgram_Z9447D8C } from "../Tempo.Html/HtmlDSL.fs.js";

export const template = template_1;

export function middleware(_arg1) {
    const prev = _arg1.Previous;
    const current = _arg1.Current;
    const action = _arg1.Action;
    console.log(some(toText(interpolate("Action: %P(), State: %P(), Previous %P()", [action, current, prev]))));
}

export const render = HTML_MakeProgram_Z9447D8C(template, document.body);

export const view = render((state, action) => update(state, action))((arg00$0040) => {
    middleware(arg00$0040);
})(new Html2TempoState(void 0));

