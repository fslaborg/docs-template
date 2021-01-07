#r "paket:
nuget BlackFox.Fake.BuildTask
nuget Fake.Core.Target
nuget Fake.Core.Process
nuget Fake.Core.ReleaseNotes
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.DotNet.MSBuild
nuget Fake.DotNet.AssemblyInfoFile
nuget Fake.DotNet.Paket
nuget Fake.DotNet.FSFormatting
nuget Fake.DotNet.Fsi
nuget Fake.DotNet.NuGet
nuget Fake.Api.Github
nuget Fake.DotNet.Testing.Expecto //"

#load ".fake/build.fsx/intellisense.fsx"

open BlackFox.Fake
open System.IO
open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing
open Fake.IO.Globbing.Operators
open Fake.DotNet.Testing
open Fake.Tools
open Fake.Api
open Fake.Tools.Git

Target.initEnvironment ()

let release = ReleaseNotes.load ("RELEASE_NOTES.md")

let version = SemVer.parse release.NugetVersion

let runDotNet cmd workingDir =
    let result =
        DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

//let withProjectRoot dir = __SOURCE_DIRECTORY__ @@ dir

let clean = BuildTask.create "clean" [] {
    [
        "bin"
        "obj"
        "pkg"
        "tests"
        "Content/bin"
        "Content/obj"
        "Content/.fsdocs"
        "Content/tmp"
    ]
    |> Shell.cleanDirs

    [
        "Content/docs/content/fsdocs-custom.min.css.gz"
        "Content/docs/content/fsdocs-custom.min.css"
        "Content/docs/content/fsdocs-custom.css"
    ]
    |> File.deleteAll
}

let compileSass = BuildTask.create "compileSass" [clean] {
    !! "Content/docs/**/*.scss"
    ++ "Content/docs/*.scss"
    |> Seq.iter (fun sassFile -> 
        let targetFile = sassFile.Replace(".scss", ".css")
        let configFile = (__SOURCE_DIRECTORY__ + "/webcompilerconfiguration.json")
        let cmd = sprintf "webcompiler %s %s -c %s" sassFile targetFile configFile
        printfn "[Sass]: Compiling %s --> %s" sassFile targetFile
        runDotNet cmd (Path.getDirectory sassFile)
    )
}

let buildTestProject = BuildTask.create "buildTestProject" [clean] { 
    !! "Content/*.fsproj"
    |> Seq.iter (DotNet.build id)
}

let watchExampleDocs = BuildTask.create "watchExampleDocs" [clean; compileSass; buildTestProject] { 
    runDotNet "fsdocs watch --eval --strict --clean --property Configuration=Release" "Content"
}

let buildExampleDocs = BuildTask.create "buildExampleDocs" [clean; compileSass; buildTestProject] { 
    runDotNet "fsdocs build --eval --strict --clean --property Configuration=Release --output ../gh-pages" "Content"
}

let pack = BuildTask.create "Pack" [clean; compileSass;] {
    !! "./FsLab.DocumentationTemplate.fsproj"
    |> Seq.iter (Fake.DotNet.DotNet.pack (fun p ->
        let msBuildParams =
            {p.MSBuildParams with 
                Properties = ([
                    "Version",(sprintf "%i.%i.%i" version.Major version.Minor version.Patch )
                ] @ p.MSBuildParams.Properties
                )
            }
        {
            p with 
                MSBuildParams = msBuildParams
                OutputPath = Some "pkg"
        }
    ))
}

let uninstallTestTemplate =  BuildTask.create "uninstallTestTemplate" [] {
    runDotNet "new -u FsLab.DocumentationTemplate" __SOURCE_DIRECTORY__
}

let installTestTemplate =  BuildTask.create "installTestTemplate" [pack] {
    runDotNet 
        (sprintf "new -i FsLab.DocumentationTemplate.%i.%i.%i.nupkg" version.Major version.Minor version.Patch)
        (__SOURCE_DIRECTORY__ @@ "pkg")
}

let runTestTemplate =  BuildTask.create "runTestTemplate" [installTestTemplate] {
    Directory.ensure (__SOURCE_DIRECTORY__ @@ "tests")
    runDotNet "new fslab-docs" (__SOURCE_DIRECTORY__ @@ "tests")
}

let test = BuildTask.createEmpty "test" [installTestTemplate; runTestTemplate; uninstallTestTemplate]

BuildTask.runOrDefaultWithArguments pack