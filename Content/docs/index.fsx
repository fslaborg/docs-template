(**
# The fslab documentation template

This template scaffolds the necessary folder structure for FSharp.Formatting 
and adds custom styles in the fslab theme. 

## Installation

This template is available as a dotnet new template:

```no-highlight
dotnet new -i FsLab.DocumentationTemplate - 
```

## Usage

in the root of your project that you want to write documentation for, run:

```no-highlight
dotnet new fslab-docs
```

## Quick content rundown:

The template initializes the following folder structure when you initialize it in the root of your project:

<pre>
docs
│   index.fsx
│   _template.html
│
├───content
│       0_Markdown-Cheatsheet.md
│       1_fsharp-code-example.fsx
│       fsdocs-custom.css
│       fsdocs-custom.min.css
│
├───img
│       favicon.ico
│       logo.png
│
└───reference
        _template.html
</pre>

- `index.fsx` is the file you are reading just now. It contains the very content you are reading at the moment 
in a markdown block indicated by `(** *)` guards. It will be rendered as the root `index.html` file of your documentation.

- `_template.html` is the root html scaffold (sidebar to the left, script and style loading)

- the `content` folder contains the following files:

    - `0_Markdown-Cheatsheet.md` is a adaption of [this markdown cheat sheet]() that shows how to write markdown and
    showcases the rendered equivalents. It can also be viewed in all its glory [here]().

    - `1_fsharp-code-example.fsx` is a script file that showcases the syntax highlighting style for F# snippets. 
    It can also be viewed in all its glory [here]().

    - `fsdocs-custom.css` and `fsdocs-custom.min.css` contain the custom styling that applies the fslab styles.

 - the `img` folder contains the fslab logo and favicon. replace these files (with the same names) to youse sours

 - `reference/_template.html` is a slightly adapted version of the template above for the API documentation

## Creating new content

Please refer to the [FSharp.Formatting documentation]().

*)
