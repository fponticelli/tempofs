module App

open Browser
open Tempo.Html
open Tempo.Html.Template
open Tempo.Demo.Html2Tempo.View

let template : Template<Html2TempoState, Html2TempoAction, Html2TempoQuery> = comp

let container =
    document.querySelector ("#tempofs-demo-app")

Program.MakeOnContentLoaded(
    { Template = template
      Update = update
      Container = container
      Middleware = None
      State = { FilterComments = true } },
    ignore
)
