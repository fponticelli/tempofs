import { update, comp } from "./Html2TempoView.fs.js";
import { interpolate, toText } from "./.fable/fable-library.3.1.10/String.js";
import { some } from "./.fable/fable-library.3.1.10/Option.js";
import { DSL_MakeProgramOnContentLoaded_56DDD9AE } from "../Tempo.Html/HtmlDSL.fs.js";

export const template = comp;

export function middleware(_arg1) {
    const prev = _arg1.Previous;
    const current = _arg1.Current;
    const action = _arg1.Action;
    console.log(some(toText(interpolate("Action: %P(), State: %P(), Previous %P()", [action, current, prev]))));
}

export const render = DSL_MakeProgramOnContentLoaded_56DDD9AE(template, "#tempofs-demo-app", (value) => {
});

render((state, action) => {
    update(void 0, action);
})((arg00$0040) => {
    middleware(arg00$0040);
})(void 0);

