import { Record, Union } from "../Tempo.Demo/.fable/fable-library.3.1.10/Types.js";
import { class_type, option_type, record_type, list_type, union_type, string_type } from "../Tempo.Demo/.fable/fable-library.3.1.10/Reflection.js";
import { Template$4$reflection, Value$2$reflection } from "../Tempo.Core/Core.fs.js";

export class HTMLTemplateNode$3 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["HTMLTemplateElement", "HTMLTemplateText"];
    }
}

export function HTMLTemplateNode$3$reflection(gen0, gen1, gen2) {
    return union_type("Tempo.Html.HTMLTemplateNode`3", [gen0, gen1, gen2], HTMLTemplateNode$3, () => [[["Item", HTMLTemplateElement$3$reflection(gen0, gen1, gen2)]], [["Item", Value$2$reflection(gen0, string_type)]]]);
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

export class HTMLTemplateAttribute$3 extends Record {
    constructor(Name, Value) {
        super();
        this.Name = Name;
        this.Value = Value;
    }
}

export function HTMLTemplateAttribute$3$reflection(gen0, gen1, gen2) {
    return record_type("Tempo.Html.HTMLTemplateAttribute`3", [gen0, gen1, gen2], HTMLTemplateAttribute$3, () => [["Name", string_type], ["Value", HTMLTemplateAttributeValue$3$reflection(gen0, gen1, gen2)]]);
}

export class HTMLTemplateAttributeValue$3 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["StringValue", "TriggerValue", "LifecycleValue"];
    }
}

export function HTMLTemplateAttributeValue$3$reflection(gen0, gen1, gen2) {
    return union_type("Tempo.Html.HTMLTemplateAttributeValue`3", [gen0, gen1, gen2], HTMLTemplateAttributeValue$3, () => [[["Item", Value$2$reflection(gen0, option_type(string_type))]], [["Item", class_type("Tempo.Html.IHTMLTrigger`2", [gen0, gen1])]], [["Item", class_type("Tempo.Html.ILifecycleValue`2", [gen0, gen2])]]]);
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

export class LifecycleValueInitialPayload$3 extends Record {
    constructor(State, Element$) {
        super();
        this.State = State;
        this.Element = Element$;
    }
}

export function LifecycleValueInitialPayload$3$reflection(gen0, gen1, gen2) {
    return record_type("Tempo.Html.LifecycleValueInitialPayload`3", [gen0, gen1, gen2], LifecycleValueInitialPayload$3, () => [["State", gen0], ["Element", gen2]]);
}

export class LifecycleValuePayload$4 extends Record {
    constructor(State, Element$, Payload) {
        super();
        this.State = State;
        this.Element = Element$;
        this.Payload = Payload;
    }
}

export function LifecycleValuePayload$4$reflection(gen0, gen1, gen2, gen3) {
    return record_type("Tempo.Html.LifecycleValuePayload`4", [gen0, gen1, gen2, gen3], LifecycleValuePayload$4, () => [["State", gen0], ["Element", gen2], ["Payload", gen3]]);
}

export class LifecycleValue$4 {
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

export function LifecycleValue$4$reflection(gen0, gen1, gen2, gen3) {
    return class_type("Tempo.Html.LifecycleValue`4", [gen0, gen1, gen2, gen3], LifecycleValue$4);
}

export function LifecycleValue$4_$ctor_51FBBB95(afterRender, beforeChange, afterChange, beforeDestroy, respond) {
    return new LifecycleValue$4(afterRender, beforeChange, afterChange, beforeDestroy, respond);
}

export function HTMLTrigger$4__get_Handler(this$) {
    return this$.handler;
}

export function LifecycleValue$4__get_AfterRender(this$) {
    return this$.afterRender;
}

export function LifecycleValue$4__get_BeforeChange(this$) {
    return this$.beforeChange;
}

export function LifecycleValue$4__get_AfterChange(this$) {
    return this$.afterChange;
}

export function LifecycleValue$4__get_BeforeDestroy(this$) {
    return this$.beforeDestroy;
}

export function LifecycleValue$4__get_Respond(this$) {
    return this$.respond;
}

