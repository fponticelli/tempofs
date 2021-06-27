namespace Tempo.Html

open Browser
open Tempo.Core
open Tempo.Std
open Tempo.Html.Tools
open Browser.Types

type HTMLTemplate<'S, 'A, 'Q> = Template<HTMLTemplateNode<'S, 'A, 'Q>, 'S, 'A, 'Q>

and HTMLTemplateNode<'S, 'A, 'Q> =
    | HTMLTemplateElement of HTMLTemplateElement<'S, 'A, 'Q>
    | HTMLTemplateText of Value<'S, string>

and HTMLTemplateElement<'S, 'A, 'Q> =
    { Name: string
      Attributes: HTMLTemplateAttribute<'S, 'A, 'Q> list
      Children: HTMLTemplate<'S, 'A, 'Q> list }

and HTMLTemplateAttribute<'S, 'A, 'Q> =
    { Name: string
      Value: HTMLTemplateAttributeValue<'S, 'A, 'Q> }

and HTMLTemplateAttributeValue<'S, 'A, 'Q> =
    | StringAttr of Value<'S, string option>
    | Property of IProperty<'S>
    | Trigger of IHTMLTrigger<'S, 'A>
    | Lifecycle of IHTMLLifecycle<'S, 'Q>

and IProperty<'S> =
    abstract Accept : IPropertyInvoker<'S, 'R> -> 'R

and Property<'S, 'V>(name, value) =
    member this.Name : string = name
    member this.Value : Value<'S, 'V> = value
    with
        interface IProperty<'S> with
            member this.Accept f = f.Invoke<'V> this

and IPropertyInvoker<'S, 'R> =
    abstract Invoke<'V> : Property<'S, 'V> -> 'R

and IHTMLTrigger<'S, 'A> =
    abstract Accept : IHTMLTriggerInvoker<'S, 'A, 'R> -> 'R

and TriggerPayload<'S, 'E, 'EL when 'E :> Event and 'EL :> Element> = { State: 'S; Event: 'E; Element: 'EL }

and HTMLTrigger<'S, 'A, 'E, 'EL when 'E :> Event and 'EL :> Element>(handler) =
    member this.Handler : TriggerPayload<'S, 'E, 'EL> -> 'A = handler
    with
        interface IHTMLTrigger<'S, 'A> with
            member this.Accept f = f.Invoke<'E, 'EL> this

and IHTMLTriggerInvoker<'S, 'A, 'R> =
    abstract Invoke<'E, 'EL when 'E :> Event and 'EL :> Element> : HTMLTrigger<'S, 'A, 'E, 'EL> -> 'R


and IHTMLLifecycle<'S, 'Q> =
    abstract Accept : IHTMLLifecycleInvoker<'S, 'Q, 'R> -> 'R

and HTMLLifecycleInitialPayload<'S, 'Q, 'EL when 'EL :> Element> = { State: 'S; Element: 'EL }

and HTMLLifecyclePayload<'S, 'Q, 'EL, 'P when 'EL :> Element> =
    { State: 'S
      Element: 'EL
      Payload: 'P }

and HTMLLifecycle<'S, 'Q, 'EL, 'P when 'EL :> Element>(afterRender, beforeChange, afterChange, beforeDestroy, respond) =
    member this.AfterRender : HTMLLifecycleInitialPayload<'S, 'Q, 'EL> -> 'P = afterRender
    member this.BeforeChange : HTMLLifecyclePayload<'S, 'Q, 'EL, 'P> -> (bool * 'P) = beforeChange
    member this.AfterChange : HTMLLifecyclePayload<'S, 'Q, 'EL, 'P> -> 'P = afterChange
    member this.BeforeDestroy : HTMLLifecyclePayload<'S, 'Q, 'EL, 'P> -> 'P = beforeDestroy
    member this.Respond : HTMLLifecyclePayload<'S, 'Q, 'EL, 'P> -> 'P = respond
    with
        interface IHTMLLifecycle<'S, 'Q> with
            member this.Accept f = f.Invoke<'EL, 'P> this

and IHTMLLifecycleInvoker<'S, 'Q, 'R> =
    abstract Invoke<'EL, 'P when 'EL :> Element> : HTMLLifecycle<'S, 'Q, 'EL, 'P> -> 'R
