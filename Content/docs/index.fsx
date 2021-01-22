(**
# The fslab documentation template

This template scaffolds the necessary folder structure for FSharp.Formatting 
and adds custom styles in the **fslab** theme. 

The provided stylesheet was compiled from sass (before uploading the nuget package) and
uses the [Bulma](https://bulma.io/) CSS framework instead of bootstrap which is used by FSharp.Formatting per default.

#### Table of contents 

- [Installation](#Installation)
- [Usage](#Usage)
- [Quick content rundown](#Quick-content-rundown)
- [Creating new content](#Creating-new-content)
- [Customization options](#Customization-options)
    - [Style sheet options](#Style-sheet-options)
    - [Inclusion of sample content](#Inclusion-of-sample-content)


## Installation

This template is available as a _dotnet new_ template (from [nuget](https://www.nuget.org/packages/FsLab.DocumentationTemplate/)):

```no-highlight
dotnet new -i FsLab.DocumentationTemplate
```

## Usage

If not already present, create a _local tool manifest_ in the root of your project that you want to write documentation for:

```no-highlight
dotnet new tool-manifest
```

Then, still in the root of your project, run:

```no-highlight
dotnet new fslab-docs
```

See [further below](#Customization-options) for command line customization options of the template:


## Quick content rundown:

The template initializes the following folder structure when you initialize it in the root of your project:

<pre>
docs
│   index.fsx
│   _template.html
|   
|   
│   0_Markdown-Cheatsheet.md
│   1_fsharp-code-example.fsx
│   fsdocs-custom.css
│   fsdocs-custom.min.css
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

- `_template.html` is the root html scaffold (sidebar to the left, script and style loading) where all of the individual docs will be injected into

- `0_Markdown-Cheatsheet.md` is a adaption of [this markdown cheat sheet]() that shows how to write markdown and showcases the rendered equivalents. It can also be viewed in all its glory [here]().

- `1_fsharp-code-example.fsx` is a script file that showcases the syntax highlighting style for F# snippets. It can also be viewed in all its glory [here]().

- `fsdocs-custom.css` and `fsdocs-custom.min.css` contain the custom styling that applies the fslab styles.

 - the `img` folder contains the fslab logo and favicon. replace these files (with the same names) to youse sours

 - `reference/_template.html` is a slightly adapted version of the template above for the API documentation

## Creating new content

- run `dotnet fsdocs watch --eval` to spawn a watcher and dev server that hosts your docs on http://localhost:8901/ (You currently will still have to refresh the page when you make changes to files)

- add a new .md or .fsx file to the `content` directory (or into a new subdirectory there)

- the sidebar title for the document will be either the file name or, if existent, the first level 1 header in the file

- when writing a .fsx file, code will automatically become syntax-highlighted code snippets. 

- use `(** <markdown here> *)` to guard markdown sections in .fsx files

- use `(*** include-value:<val name> ***)` to include the value of a binding

- use `(*** include-it ***)` to include the evaluation of the previous snippet block 

For more info please refer to the [FSharp.Formatting documentation](http://fsprojects.github.io/FSharp.Formatting/).


## Customization options

### Style sheet options

```no-highlight
-s|--styles             Set the type of style content the template will initialize. For the sass file to work, you will have to download bulma

        all             - sass file, compiled csss, and minified css

        sass            - only include the sass file

        minified        - only include the minified css file

        css             - only include the compiled css file

        Default:        css
```

### Inclusion of sample content

```no-highlight
-is|--include-samples   wether to include sample files in the generated content

        bool            - Optional

        Default:        true
```
*)
