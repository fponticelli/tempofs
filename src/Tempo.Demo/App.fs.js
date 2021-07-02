import { Html2TempoState, update, comp } from "./Html2TempoView.fs.js";
import { DSL_MakeProgramOnContentLoaded_56DDD9AE } from "../Tempo.Html/Html.DSL.fs.js";

export const template = comp;

export function middleware(_arg1) {
}

export const render = DSL_MakeProgramOnContentLoaded_56DDD9AE(template, "#tempofs-demo-app", (value) => {
});

render((state, action) => update(state, action))((arg00$0040) => {
    middleware(arg00$0040);
})(new Html2TempoState(true));

