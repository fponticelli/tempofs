import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { class_type, record_type, string_type, union_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { empty } from "./.fable/fable-library.3.1.10/List.js";
import { lifecycleAttribute } from "../Tempo.Html/HtmlImpl.fs.js";
import * as monaco$002Deditor from "monaco-editor";

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

export class MonacoEditorInstance {
    constructor() {
    }
}

export function MonacoEditorInstance$reflection() {
    return class_type("Tempo.Demo.Utils.Monaco.MonacoEditorInstance", void 0, MonacoEditorInstance);
}

export class MonacoEditorClass {
    constructor() {
    }
}

export function MonacoEditorClass$reflection() {
    return class_type("Tempo.Demo.Utils.Monaco.MonacoEditorClass", void 0, MonacoEditorClass);
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

export function delay(f) {
    return new Promise(((resolve, reject) => {
        void window.setTimeout((_arg1) => {
            resolve(f());
        }, 0, empty());
    }));
}

export function MonacoEditorAttribute(mapToOptions) {
    return lifecycleAttribute((_arg1) => {
        const state = _arg1.State;
        const element = _arg1.Element;
        return delay(() => {
            const editor = monaco$002Deditor.editor.create(element, mapToOptions(state));
            return editor;
        });
    }, (_arg2) => {
        const promiseEditor = _arg2.Payload;
        return [true, promiseEditor];
    }, (_arg3) => {
        const promiseEditor_1 = _arg3.Payload;
        return promiseEditor_1;
    }, (_arg4) => {
        const promiseEditor_2 = _arg4.Payload;
    }, (q, _arg5) => {
        const promiseEditor_3 = _arg5.Payload;
        return promiseEditor_3;
    });
}

