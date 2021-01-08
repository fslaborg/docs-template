# The fslab documentation template

`dotnet new` template setting up the necessary folder structure and files for [FSharp.Formatting](https://github.com/fsprojects/FSharp.Formatting) documentation. It uses a custom fslab color scheme and html template.

## How does it look like?

The current version is live on gh-pages:

https://fslab.org/docs-template/


## Installation

```powershell
dotnet new -i FsLab.DocumentationTemplate
```

## Usage

If not already present, create a local tool manifest in the root of your project that you want to write documentation for:

```powershell
dotnet new tool-manifest
```

Then, still in the root of your project, run:

```powershell
dotnet new fslab-docs
```

for more more indepth information head over [here](https://fslab.org/docs-template/#Usage)

## Develop

To (re)compile with sass:

`build.cmd -t compileSass` (Win)

`build.sh -t compileSass` (Mac/Linux)

Build the template nuget package: 

`build.cmd` (Win)

`build.sh` (Mac/Linux)

Start fsdocs in watcher mode for the test project:

`build.cmd -t watchExampleDocs` (Win)

`build.sh -t watchExampleDocs` (Mac/Linux)

Test the template package intallation and check correct contents of an initialized template:

`build.cmd -t test` (Win)

`build.cmd -t test` (Mac/Linux)

will create the template output in the `tests` folder.

