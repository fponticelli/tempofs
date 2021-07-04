import { Record, Union } from "../../../src/.fable/fable-library.3.1.10/Types.js";
import { lambda_type, unit_type, option_type, class_type, record_type, list_type, union_type, string_type } from "../../../src/.fable/fable-library.3.1.10/Reflection.js";
import { Template$4$reflection, Value$2$reflection } from "../Tempo.Core/Core.fs.js";
import { curry } from "../../../src/.fable/fable-library.3.1.10/Util.js";

export class HTMLTemplateNode$3 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["HTMLTemplateElementNS", "HTMLTemplateElement", "HTMLTemplateText"];
    }
}

export function HTMLTemplateNode$3$reflection(gen0, gen1, gen2) {
    return union_type("Tempo.Html.HTMLTemplateNode`3", [gen0, gen1, gen2], HTMLTemplateNode$3, () => [[["Item1", string_type], ["Item2", HTMLTemplateElement$3$reflection(gen0, gen1, gen2)]], [["Item", HTMLTemplateElement$3$reflection(gen0, gen1, gen2)]], [["Item", Value$2$reflection(gen0, string_type)]]]);
}

export class HTMLTemplateElement$3 extends Record {
    constructor(Name, Attributes, Children) {
        super();
        this.Name = Name;
        this.Attributes = Attributes;
        this.Children = Children;
    }
}

export function HTMLTemplateElement$3$reflection(gen0, gen1, gen2) {
    return record_type("Tempo.Html.HTMLTemplateElement`3", [gen0, gen1, gen2], HTMLTemplateElement$3, () => [["Name", string_type], ["Attributes", list_type(HTMLTemplateAttribute$3$reflection(gen0, gen1, gen2))], ["Children", list_type(Template$4$reflection(HTMLTemplateNode$3$reflection(gen0, gen1, gen2), gen0, gen1, gen2))]]);
}

export class HTMLNamedAttribute$3 extends Record {
    constructor(Name, Value) {
        super();
        this.Name = Name;
        this.Value = Value;
    }
}

export function HTMLNamedAttribute$3$reflection(gen0, gen1, gen2) {
    return record_type("Tempo.Html.HTMLNamedAttribute`3", [gen0, gen1, gen2], HTMLNamedAttribute$3, () => [["Name", string_type], ["Value", HTMLTemplateAttributeValue$3$reflection(gen0, gen1, gen2)]]);
}

export class HTMLTemplateAttribute$3 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["HTMLNamedAttribute", "Lifecycle"];
    }
}

export function HTMLTemplateAttribute$3$reflection(gen0, gen1, gen2) {
    return union_type("Tempo.Html.HTMLTemplateAttribute`3", [gen0, gen1, gen2], HTMLTemplateAttribute$3, () => [[["Item", HTMLNamedAttribute$3$reflection(gen0, gen1, gen2)]], [["Item", class_type("Tempo.Html.IHTMLLifecycle`3", [gen0, gen1, gen2])]]]);
}

export class HTMLTemplateAttributeValue$3 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["StringAttr", "Property", "Trigger"];
    }
}

export function HTMLTemplateAttributeValue$3$reflection(gen0, gen1, gen2) {
    return union_type("Tempo.Html.HTMLTemplateAttributeValue`3", [gen0, gen1, gen2], HTMLTemplateAttributeValue$3, () => [[["Item", Value$2$reflection(gen0, option_type(string_type))]], [["Item", class_type("Tempo.Html.IProperty`1", [gen0])]], [["Item", class_type("Tempo.Html.IHTMLTrigger`2", [gen0, gen1])]]]);
}

export class Property$2 {
    constructor(value) {
        this.value = value;
    }
    Accept(f) {
        const this$ = this;
        return f.Invoke(this$);
    }
}

export function Property$2$reflection(gen0, gen1) {
    return class_type("Tempo.Html.Property`2", [gen0, gen1], Property$2);
}

export function Property$2_$ctor_1D5210CF(value) {
    return new Property$2(value);
}

export class TriggerPayload$3 extends Record {
    constructor(State, Event$, Element$) {
        super();
        this.State = State;
        this.Event = Event$;
        this.Element = Element$;
    }
}

export function TriggerPayload$3$reflection(gen0, gen1, gen2) {
    return record_type("Tempo.Html.TriggerPayload`3", [gen0, gen1, gen2], TriggerPayload$3, () => [["State", gen0], ["Event", gen1], ["Element", gen2]]);
}

export class HTMLTrigger$4 {
    constructor(handler) {
        this.handler = handler;
    }
    Accept(f) {
        const this$ = this;
        return f.Invoke(this$);
    }
}

export function HTMLTrigger$4$reflection(gen0, gen1, gen2, gen3) {
    return class_type("Tempo.Html.HTMLTrigger`4", [gen0, gen1, gen2, gen3], HTMLTrigger$4);
}

export function HTMLTrigger$4_$ctor_49480E0B(handler) {
    return new HTMLTrigger$4(handler);
}

export class HTMLLifecycleInitialPayload$3 extends Record {
    constructor(State, Element$, Dispatch) {
        super();
        this.State = State;
        this.Element = Element$;
        this.Dispatch = Dispatch;
    }
}

export function HTMLLifecycleInitialPayload$3$reflection(gen0, gen1, gen2) {
    return record_type("Tempo.Html.HTMLLifecycleInitialPayload`3", [gen0, gen1, gen2], HTMLLifecycleInitialPayload$3, () => [["State", gen0], ["Element", gen2], ["Dispatch", lambda_type(gen1, unit_type)]]);
}

export class HTMLLifecyclePayload$4 extends Record {
    constructor(State, Element$, Payload, Dispatch) {
        super();
        this.State = State;
        this.Element = Element$;
        this.Payload = Payload;
        this.Dispatch = Dispatch;
    }
}

export function HTMLLifecyclePayload$4$reflection(gen0, gen1, gen2, gen3) {
    return record_type("Tempo.Html.HTMLLifecyclePayload`4", [gen0, gen1, gen2, gen3], HTMLLifecyclePayload$4, () => [["State", gen0], ["Element", gen2], ["Payload", gen3], ["Dispatch", lambda_type(gen1, unit_type)]]);
}

export class HTMLLifecycle$5 {
    constructor(afterRender, beforeChange, afterChange, beforeDestroy, respond) {
        this.afterRender = afterRender;
        this.beforeChange = beforeChange;
        this.afterChange = afterChange;
        this.beforeDestroy = beforeDestroy;
        this.respond = respond;
    }
    Accept(f) {
        const this$ = this;
        return f.Invoke(this$);
    }
}

export function HTMLLifecycle$5$reflection(gen0, gen1, gen2, gen3, gen4) {
    return class_type("Tempo.Html.HTMLLifecycle`5", [gen0, gen1, gen2, gen3, gen4], HTMLLifecycle$5);
}

export function HTMLLifecycle$5_$ctor_17DF349(afterRender, beforeChange, afterChange, beforeDestroy, respond) {
    return new HTMLLifecycle$5(afterRender, beforeChange, afterChange, beforeDestroy, respond);
}

export function Property$2__get_Value(this$) {
    return this$.value;
}

export function HTMLTrigger$4__get_Handler(this$) {
    return curry(2, this$.handler);
}

export function HTMLLifecycle$5__get_AfterRender(this$) {
    return this$.afterRender;
}

export function HTMLLifecycle$5__get_BeforeChange(this$) {
    return this$.beforeChange;
}

export function HTMLLifecycle$5__get_AfterChange(this$) {
    return this$.afterChange;
}

export function HTMLLifecycle$5__get_BeforeDestroy(this$) {
    return this$.beforeDestroy;
}

export function HTMLLifecycle$5__get_Respond(this$) {
    return curry(2, this$.respond);
}

