import { Record, Union } from "./.fable/fable-library.3.1.10/Types.js";
import { record_type, class_type, union_type } from "./.fable/fable-library.3.1.10/Reflection.js";
import { lifecycleAttribute } from "../Tempo.Html/Html.Impl.fs.js";
import * as monaco$002Deditor from "monaco-editor";
import { value } from "./.fable/fable-library.3.1.10/Option.js";

export class MonacoEvent extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["OnPaste", "OnChange"];
    }
}

export function MonacoEvent$reflection() {
    return union_type("Tempo.Demo.Utils.Monaco.MonacoEvent", [], MonacoEvent, () => [[], []]);
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

export function MonacoEditorAttribute(mapToOptions, mapAction, respond) {
    return lifecycleAttribute((_arg1) => {
        const dispatch = _arg1.Dispatch;
        const editor = monaco$002Deditor.editor.create(_arg1.Element, mapToOptions(_arg1.State));
        editor.onDidChangeModelContent((() => {
            const matchValue = mapAction(new MonacoEvent(1));
            if (matchValue == null) {
            }
            else {
                dispatch(value(matchValue));
            }
        }));
        editor.onDidPaste((() => {
            const matchValue_1 = mapAction(new MonacoEvent(0));
            if (matchValue_1 == null) {
            }
            else {
                dispatch(value(matchValue_1));
            }
        }));
        return editor;
    }, (_arg2) => [true, _arg2.Payload], (_arg3) => _arg3.Payload, (_arg4) => {
        _arg4.Payload.dispose();
    }, (q, _arg5) => {
        const editor_4 = _arg5.Payload;
        respond(q, editor_4);
        return editor_4;
    });
}

