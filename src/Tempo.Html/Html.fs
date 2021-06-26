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
    | StringValue of Value<'S, string option>
    | TriggerValue of IHTMLTrigger<'S, 'A>
    | LifecycleValue of ILifecycleValue<'S, 'Q>

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


and ILifecycleValue<'S, 'Q> =
    abstract Accept : ILifecycleValueInvoker<'S, 'Q, 'R> -> 'R

and LifecycleValueInitialPayload<'S, 'Q, 'EL when 'EL :> Element> = { State: 'S; Element: 'EL }

and LifecycleValuePayload<'S, 'Q, 'EL, 'P when 'EL :> Element> =
    { State: 'S
      Element: 'EL
      Payload: 'P }

and LifecycleValue<'S, 'Q, 'EL, 'P when 'EL :> Element>(afterRender, beforeChange, afterChange, beforeDestroy, respond) =
    member this.AfterRender : LifecycleValueInitialPayload<'S, 'Q, 'EL> -> 'P = afterRender
    member this.BeforeChange : LifecycleValuePayload<'S, 'Q, 'EL, 'P> -> (bool * 'P) = beforeChange
    member this.AfterChange : LifecycleValuePayload<'S, 'Q, 'EL, 'P> -> 'P = afterChange
    member this.BeforeDestroy : LifecycleValuePayload<'S, 'Q, 'EL, 'P> -> 'P = beforeDestroy
    member this.Respond : LifecycleValuePayload<'S, 'Q, 'EL, 'P> -> 'P = respond
    with
        interface ILifecycleValue<'S, 'Q> with
            member this.Accept f = f.Invoke<'EL, 'P> this

and ILifecycleValueInvoker<'S, 'Q, 'R> =
    abstract Invoke<'EL, 'P when 'EL :> Element> : LifecycleValue<'S, 'Q, 'EL, 'P> -> 'R
