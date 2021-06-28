import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { tuple_type, class_type, lambda_type, unit_type, record_type, string_type, union_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { HTML_El_Z7374416F } from "../Tempo.Html/HtmlDSL.fs.js";
import { lifecycleAttribute } from "../Tempo.Html/HtmlImpl.fs.js";
import * as monaco$002Deditor from "monaco-editor";
import { empty, singleton } from "./.fable/fable-library.3.1.10/List.js";

export class MonacoAction extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Unknwon"];
    }
}

export function MonacoAction$reflection() {
    return union_type("Tempo.Demo.Utils.Monaco.MonacoAction", [], MonacoAction, () => [[]]);
}

export class MonacoState extends Record {
    constructor(Value) {
        super();
        this.Value = Value;
    }
}

export function MonacoState$reflection() {
    return record_type("Tempo.Demo.Utils.Monaco.MonacoState", [], MonacoState, () => [["Value", string_type]]);
}

export class MonacoEditorOptions extends Record {
    constructor(value, language) {
        super();
        this.value = value;
        this.language = language;
    }
}

export function MonacoEditorOptions$reflection() {
    return record_type("Tempo.Demo.Utils.Monaco.MonacoEditorOptions", [], MonacoEditorOptions, () => [["value", string_type], ["language", string_type]]);
}

export class MonacoEditorInstance extends Record {
    constructor(update) {
        super();
        this.update = update;
    }
}

export function MonacoEditorInstance$reflection() {
    return record_type("Tempo.Demo.Utils.Monaco.MonacoEditorInstance", [], MonacoEditorInstance, () => [["update", lambda_type(string_type, unit_type)]]);
}

export class MonacoEditorClass extends Record {
    constructor(create) {
        super();
        this.create = create;
    }
}

export function MonacoEditorClass$reflection() {
    return record_type("Tempo.Demo.Utils.Monaco.MonacoEditorClass", [], MonacoEditorClass, () => [["create", lambda_type(tuple_type(class_type("Browser.Types.HTMLElement"), MonacoEditorOptions$reflection()), MonacoEditorInstance$reflection())]]);
}

export class MonacoModule extends Record {
    constructor(editor) {
        super();
        this.editor = editor;
    }
}

export function MonacoModule$reflection() {
    return record_type("Tempo.Demo.Utils.Monaco.MonacoModule", [], MonacoModule, () => [["editor", MonacoEditorClass$reflection()]]);
}

export function MonacoEditor() {
    return HTML_El_Z7374416F("div", singleton(lifecycleAttribute((_arg1) => {
        const state = _arg1.State;
        const element = _arg1.Element;
        const editor = monaco$002Deditor.editor.create([element, new MonacoEditorOptions(state.Value, "fsharp")]);
        return editor;
    }, (_arg2) => {
        const editor_1 = _arg2.Payload;
        return [true, editor_1];
    }, (_arg3) => {
        const editor_2 = _arg3.Payload;
        return editor_2;
    }, (value) => {
    }, (q, _arg4) => {
        const editor_3 = _arg4.Payload;
        return editor_3;
    })), empty());
}

