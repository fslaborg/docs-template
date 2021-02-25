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

[<AutoOpen>]
/// user interaction prompts for critical build tasks where you may want to interrupt when you see wrong inputs.
module MessagePrompts =

    let prompt (msg:string) =
        System.Console.Write(msg)
        System.Console.ReadLine().Trim()
        |> function | "" -> None | s -> Some s
        |> Option.map (fun s -> s.Replace ("\"","\\\""))

    let rec promptYesNo msg =
        match prompt (sprintf "%s [Yn]: " msg) with
        | Some "Y" | Some "y" -> true
        | Some "N" | Some "n" -> false
        | _ -> System.Console.WriteLine("Sorry, invalid answer"); promptYesNo msg

    let releaseMsg = """This will stage all uncommitted changes, push them to the origin and bump the release version to the latest number in the RELEASE_NOTES.md file. 
        Do you want to continue?"""

    let releaseDocsMsg = """This will push the docs to gh-pages. Remember building the docs prior to this. Do you want to continue?"""

/// Executes a dotnet command in the given working directory
let runDotNet cmd workingDir =
    let result =
        DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

/// Metadata about the project
module ProjectInfo = 

    let project = "docs-template"

    let summary = "fslab documentation theme for FSharp.Formatting"

    let configuration = "Release"

    // Git configuration (used for publishing documentation in gh-pages branch)
    // The profile where the project is posted
    let gitOwner = "fslaborg"
    let gitName = "docs-template"

    let gitHome = sprintf "%s/%s" "https://github.com" gitOwner

    let projectRepo = sprintf "%s/%s/%s" "https://github.com" gitOwner gitName

    let website = "/docs-template"

    let pkgDir = "pkg"

    let release = ReleaseNotes.load "RELEASE_NOTES.md"

    let stableVersion = SemVer.parse release.NugetVersion

    let stableVersionTag = (sprintf "%i.%i.%i" stableVersion.Major stableVersion.Minor stableVersion.Patch )

    let mutable prereleaseSuffix = ""

    let mutable prereleaseTag = ""

    let mutable isPrerelease = false

/// Barebones, minimal build tasks
module BasicTasks = 

    open ProjectInfo

    let setPrereleaseTag = BuildTask.create "SetPrereleaseTag" [] {
        printfn "Please enter pre-release package suffix"
        let suffix = System.Console.ReadLine()
        prereleaseSuffix <- suffix
        prereleaseTag <- (sprintf "%s-%s" release.NugetVersion suffix)
        isPrerelease <- true
    }

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

/// Package creation
module PackageTasks = 

    open ProjectInfo
    open BasicTasks

    let pack = BuildTask.create "Pack" [clean; compileSass;] {
        !! "./FsLab.DocumentationTemplate.fsproj"
        |> Seq.iter (Fake.DotNet.DotNet.pack (fun p ->
            let msBuildParams =
                {p.MSBuildParams with 
                    Properties = ([
                        "Version",stableVersionTag
                        "PackageReleaseNotes",  (release.Notes |> String.concat "\r\n")
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
    
    let packPrerelease = BuildTask.create "PackPrerelease" [setPrereleaseTag; clean; compileSass;] {
        !! "./FsLab.DocumentationTemplate.fsproj"
        |> Seq.iter (Fake.DotNet.DotNet.pack (fun p ->
            let msBuildParams =
                {p.MSBuildParams with 
                    Properties = ([
                        "Version",prereleaseTag
                        "PackageReleaseNotes",  (release.Notes |> String.concat "\r\n")
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

module TestTasks = 
    
    open ProjectInfo
    open BasicTasks
    open PackageTasks

    let uninstallTestTemplate =  BuildTask.create "uninstallTestTemplate" [] {
        runDotNet "new -u FsLab.DocumentationTemplate" __SOURCE_DIRECTORY__
    }

    let installTestTemplate =  BuildTask.create "installTestTemplate" [pack] {
        runDotNet 
            (sprintf "new -i FsLab.DocumentationTemplate.%s.nupkg" (if isPrerelease then prereleaseTag else stableVersionTag))
            (__SOURCE_DIRECTORY__ @@ "pkg")
    }

    let runTestTemplate =  BuildTask.create "runTestTemplate" [installTestTemplate] {
        Directory.ensure (__SOURCE_DIRECTORY__ @@ "tests")
        runDotNet "new fslab-docs" (__SOURCE_DIRECTORY__ @@ "tests")
    }

    let test = BuildTask.createEmpty "test" [installTestTemplate; runTestTemplate; uninstallTestTemplate]

module ReleaseTasks =

    open ProjectInfo

    open BasicTasks
    open PackageTasks

    let createTag = BuildTask.create "CreateTag" [clean; compileSass; pack] {
        if promptYesNo (sprintf "tagging branch with %s OK?" stableVersionTag ) then
            Git.Branches.tag "" stableVersionTag
            Git.Branches.pushTag "" projectRepo stableVersionTag
        else
            failwith "aborted"
    }

    let createPrereleaseTag = BuildTask.create "CreatePrereleaseTag" [setPrereleaseTag; clean; compileSass; pack] {
        if promptYesNo (sprintf "tagging branch with %s OK?" prereleaseTag ) then 
            Git.Branches.tag "" prereleaseTag
            Git.Branches.pushTag "" projectRepo prereleaseTag
        else
            failwith "aborted"
    }

    
    let publishNuget = BuildTask.create "PublishNuget" [clean; compileSass; pack] {
        let targets = (!! (sprintf "%s/*.*pkg" pkgDir ))
        for target in targets do printfn "%A" target
        let msg = sprintf "release package with version %s?" stableVersionTag
        if promptYesNo msg then
            let source = "https://api.nuget.org/v3/index.json"
            let apikey =  Environment.environVar "NUGET_KEY"
            for artifact in targets do
                let result = DotNet.exec id "nuget" (sprintf "push -s %s -k %s %s --skip-duplicate" source apikey artifact)
                if not result.OK then failwith "failed to push packages"
        else failwith "aborted"
    }

    let publishNugetPrerelease = BuildTask.create "PublishNugetPrerelease" [setPrereleaseTag; clean; compileSass; pack] {
        let targets = (!! (sprintf "%s/*.*pkg" pkgDir ))
        for target in targets do printfn "%A" target
        let msg = sprintf "release package with version %s?" prereleaseTag 
        if promptYesNo msg then
            let source = "https://api.nuget.org/v3/index.json"
            let apikey =  Environment.environVar "NUGET_KEY"
            for artifact in targets do
                let result = DotNet.exec id "nuget" (sprintf "push -s %s -k %s %s --skip-duplicate" source apikey artifact)
                if not result.OK then failwith "failed to push packages"
        else failwith "aborted"
    }

/// Build tasks for documentation setup and development
module DocumentationTasks =

    open ProjectInfo

    open BasicTasks

    let buildDocs = BuildTask.create "BuildDocs" [clean; compileSass; buildTestProject] {
        printfn "building docs with stable version %s" stableVersionTag
        runDotNet "fsdocs build --eval --strict --clean --properties Configuration=Release --output ../gh-pages" "Content"
    }

    let buildDocsPrerelease = BuildTask.create "BuildDocsPrerelease" [setPrereleaseTag; clean; compileSass; buildTestProject] {
        printfn "building docs with prerelease version %s" prereleaseTag
        runDotNet 
            (sprintf "fsdocs build --eval --strict --clean --properties Configuration=Release --output ../gh-pages --parameters fsdocs-package-version %s" prereleaseTag)
            "./"
    }

    let watchDocs = BuildTask.create "WatchDocs" [clean; compileSass; buildTestProject] {
        printfn "watching docs with stable version %s" stableVersionTag
        runDotNet "fsdocs watch --eval --strict --clean --properties Configuration=Release" "Content"
    }

    let watchDocsPrerelease = BuildTask.create "WatchDocsPrerelease" [setPrereleaseTag; clean; compileSass; buildTestProject] {
        printfn "watching docs with prerelease version %s" prereleaseTag
        runDotNet (sprintf "fsdocs watch --eval --strict --clean --properties Configuration=Release --parameters %s" prereleaseTag) "Content"
    }

open ProjectInfo
open BasicTasks
open PackageTasks
open TestTasks
open ReleaseTasks

BuildTask.runOrDefaultWithArguments pack