# fslab documentation template

`dotnet new` template setting up the necessary folder structure for [FSharp.Formatting]() and using the fslab color scheme.

## Use

WIP

## Develop

To start fsdocs in watcher mode for the test project:
`dotnet tool restore`
`dotnet fake build`

To test the template package intallation and check correct contents of an initialized template:
`dotnet tool restore`
`dotnet fake build -t testTemplate`

will create the template output in the `tests` folder.

Uninstall the template via
`dotnet fake build -t uninstallTemplate`

## How does it look like?

Current WIP version:

https://fslab.org/docs-template/
