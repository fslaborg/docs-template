(**
---
title: F# code formatting example
category: sample content
categoryindex: 1
index: 2
---
*)

(**

[![Binder]({{root}}img/badge-binder.svg)](https://mybinder.org/v2/gh/plotly/Plotly.NET/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script]({{root}}img/badge-script.svg)]({{root}}{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook]({{root}}img/badge-notebook.svg)]({{root}}{{fsdocs-source-basename}}.ipynb)

[How to add these badges?]({{root}}4_download-badges.html)

# F# code formatting example

This page is rendered from a F# script as input, containing real F# code besides this markdown section.

hover above some bindings to get tooltips like in a real editor environment.

*)

///this is comment
let a = 42

(***include-value:a***)

// see some operators/keywords:

if a > 0 then printfn "see, this is included: %i" a

// an interface:
type IA =
    abstract member B : string -> string

// an interface implementation:

type C() =
    interface IA with
        member _.B(a) = id a

let d = C() :> IA

let e = d.B("soos")

(***include-value:e***)

module ThisIsAModule =

    type Union =
        | First
        | Second of IA

    type Enum =
        | First = 1
        | Second = 2