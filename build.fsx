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

let withProjectRoot dir = __SOURCE_DIRECTORY__ @@ dir

let clean = BuildTask.create "clean" [] {
    [
        "bin"
        "obj"
        "pkg"
        "Content/bin"
        "Content/obj"
        "Content/.fsdocs"
        "Content/tmp"
    ]
    |> List.map withProjectRoot
    |> Shell.cleanDirs
}

let compileSass = BuildTask.create "compileSass" [] {
    !! "Content/docs/**/*.scss"
    ++ "Content/docs/*.scss"
    |> Seq.map withProjectRoot
    |> Seq.iter (fun sassFile -> 
        let targetFile = sassFile.Replace(".scss", ".css")
        let cmd = sprintf "webcompiler %s %s" sassFile targetFile
        printfn "[Sass]: Compiling %s --> %s" sassFile targetFile
        runDotNet cmd (Path.getDirectory sassFile)
    )
}

let buildTestProject = BuildTask.create "buildTestProject" [clean] { 
    !! "Content/*.fsproj"
    |> Seq.iter (DotNet.build id)
}

let watchTestDocs = BuildTask.create "watchTestDocs" [clean; compileSass; buildTestProject] { 
    runDotNet "fsdocs watch --eval --strict --clean --property Configuration=Release" "Content"
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

let uninstallTemplate =  BuildTask.create "uninstallTemplate" [] {
    runDotNet "new -u FsLab.DocumentationTemplate" __SOURCE_DIRECTORY__
}

let installTemplate =  BuildTask.create "installTemplate" [pack] {
    runDotNet 
        (sprintf "new -i %s" (__SOURCE_DIRECTORY__ @@ (sprintf "pkg/FsLab.DocumentationTemplate.%i.%i.%i.nupkg" version.Major version.Minor version.Patch)))
        __SOURCE_DIRECTORY__
}

let testTemplate =  BuildTask.create "testTemplate" [installTemplate] {
    runDotNet "new fslab-docs" (__SOURCE_DIRECTORY__ @@ "tests")
}

BuildTask.runOrDefaultWithArguments watchTestDocs