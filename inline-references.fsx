(**
[![Binder](https://fslab.org/docs-template/img/badge-binder.svg)](https://mybinder.org/v2/gh/plotly/Plotly.NET/gh-pages?filepath=inline-references.ipynb)&emsp;
[![Script](https://fslab.org/docs-template/img/badge-script.svg)](https://fslab.org/docs-template/inline-references.fsx)&emsp;
[![Notebook](https://fslab.org/docs-template/img/badge-notebook.svg)](https://fslab.org/docs-template/inline-references.ipynb)

[How to add these badges?](https://fslab.org/docs-template/4_download-badges.html)

# Inline package references and charting

With fsdocs 8.0, the tool can roll forward to .net 5, meaning you can use inline package references in the docs scripts:

*)
#r "nuget: Plotly.NET, 2.0.0-preview.6"

open Plotly.NET

let myChart = 
    Chart.Line(
        [
            1.,1.
            5.,6.
            23.,9.
        ]
    )
    |> Chart.withTitle "Hello fsdocs!"
(**
You can now also include raw html in your docs scripts with the new `include-it-raw`.
To incude the chart html of a Plotly.NET chart and and render it on the docs page, use the `GenericChart.toChartHTML`
and include the raw output.

the actual codeblock looks like this:

<pre>
(***hide***)
myChart |> GenericChart.toChartHTML
(***include-it-raw***)
</pre>
</pre>

Here is the rendered chart:

<div id="12a45aea-2bc9-453d-b9ae-934a2da1fb30" style="width: 600px; height: 600px;"><!-- Plotly chart will be drawn inside this DIV --></div>
<script type="text/javascript">

            var renderPlotly_12a45aea2bc9453db9ae934a2da1fb30 = function() {
            var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-latest.min'}}) || require;
            fsharpPlotlyRequire(['plotly'], function(Plotly) {

            var data = [{"type":"scatter","x":[1.0,5.0,23.0],"y":[1.0,6.0,9.0],"mode":"lines","line":{},"marker":{}}];
            var layout = {"title":"Hello fsdocs!"};
            var config = {};
            Plotly.newPlot('12a45aea-2bc9-453d-b9ae-934a2da1fb30', data, layout, config);
});
            };
            if ((typeof(requirejs) !==  typeof(Function)) || (typeof(requirejs.config) !== typeof(Function))) {
                var script = document.createElement("script");
                script.setAttribute("src", "https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
                script.onload = function(){
                    renderPlotly_12a45aea2bc9453db9ae934a2da1fb30();
                };
                document.getElementsByTagName("head")[0].appendChild(script);
            }
            else {
                renderPlotly_12a45aea2bc9453db9ae934a2da1fb30();
            }
</script>

*)

