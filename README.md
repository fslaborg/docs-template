# fslab documentation template

`dotnet new` template setting up the necessary folder structure and files for [FSharp.Formatting]() documentation. It uses a fslab color scheme.

## How does it look like?

The current version is live on gh-pages:

https://fslab.org/docs-template/


## Use

WIP

## Develop

Build the template nuget package: 

`build.cmd` (Win)

`build.sh` (Mac/Linux)

Start fsdocs in watcher mode for the test project:

`build.cmd -t watch` (Win)

`build.sh -t watch` (Mac/Linux)

Test the template package intallation and check correct contents of an initialized template:

`build.cmd -t test` (Win)

`build.cmd -t test` (Mac/Linux)

will create the template output in the `tests` folder.

