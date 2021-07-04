module App

open Browser
open Tempo.Core
open Tempo.Html

open type Tempo.Html.DSL
open Tempo.Demo.Html2Tempo.View

let template : HTMLTemplate<Html2TempoState, Html2TempoAction, Html2TempoQuery> = comp

let middleware
    ({ Current = current
       Previous = prev
       Action = action }: MiddlewarePayload<_, _, _>)
    =
    ()

let render =
    MakeProgramOnContentLoaded(template, "#tempofs-demo-app", ignore)

render update middleware { FilterComments = true }
