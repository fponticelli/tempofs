import { Record, Union } from "../Tempo.Demo/.fable/fable-library.3.1.10/Types.js";
import { option_type, class_type, record_type, list_type, union_type, string_type } from "../Tempo.Demo/.fable/fable-library.3.1.10/Reflection.js";
import { Template$4$reflection, Value$2$reflection } from "../Tempo.Core/Core.fs.js";
import { curry } from "../Tempo.Demo/.fable/fable-library.3.1.10/Util.js";

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
    return union_type("Tempo.Html.HTMLTemplateAttribute`3", [gen0, gen1, gen2], HTMLTemplateAttribute$3, () => [[["Item", HTMLNamedAttribute$3$reflection(gen0, gen1, gen2)]], [["Item", class_type("Tempo.Html.IHTMLLifecycle`2", [gen0, gen2])]]]);
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
    constructor(name, value) {
        this.name = name;
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

export function Property$2_$ctor_57011354(name, value) {
    return new Property$2(name, value);
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

export function HTMLTrigger$4_$ctor_75095B8B(handler) {
    return new HTMLTrigger$4(handler);
}

export class HTMLLifecycleInitialPayload$2 extends Record {
    constructor(State, Element$) {
        super();
        this.State = State;
        this.Element = Element$;
    }
}

export function HTMLLifecycleInitialPayload$2$reflection(gen0, gen1) {
    return record_type("Tempo.Html.HTMLLifecycleInitialPayload`2", [gen0, gen1], HTMLLifecycleInitialPayload$2, () => [["State", gen0], ["Element", gen1]]);
}

export class HTMLLifecyclePayload$3 extends Record {
    constructor(State, Element$, Payload) {
        super();
        this.State = State;
        this.Element = Element$;
        this.Payload = Payload;
    }
}

export function HTMLLifecyclePayload$3$reflection(gen0, gen1, gen2) {
    return record_type("Tempo.Html.HTMLLifecyclePayload`3", [gen0, gen1, gen2], HTMLLifecyclePayload$3, () => [["State", gen0], ["Element", gen1], ["Payload", gen2]]);
}

export class HTMLLifecycle$4 {
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

export function HTMLLifecycle$4$reflection(gen0, gen1, gen2, gen3) {
    return class_type("Tempo.Html.HTMLLifecycle`4", [gen0, gen1, gen2, gen3], HTMLLifecycle$4);
}

export function HTMLLifecycle$4_$ctor_Z3754A3A9(afterRender, beforeChange, afterChange, beforeDestroy, respond) {
    return new HTMLLifecycle$4(afterRender, beforeChange, afterChange, beforeDestroy, respond);
}

export function Property$2__get_Name(this$) {
    return this$.name;
}

export function Property$2__get_Value(this$) {
    return this$.value;
}

export function HTMLTrigger$4__get_Handler(this$) {
    return this$.handler;
}

export function HTMLLifecycle$4__get_AfterRender(this$) {
    return this$.afterRender;
}

export function HTMLLifecycle$4__get_BeforeChange(this$) {
    return this$.beforeChange;
}

export function HTMLLifecycle$4__get_AfterChange(this$) {
    return this$.afterChange;
}

export function HTMLLifecycle$4__get_BeforeDestroy(this$) {
    return this$.beforeDestroy;
}

export function HTMLLifecycle$4__get_Respond(this$) {
    return curry(2, this$.respond);
}

