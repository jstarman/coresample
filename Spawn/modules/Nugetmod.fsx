#r @"tools/FAKE.4.62.5/tools/FakeLib.dll"
open Fake

let useLocalNuget (config : Map<string,string>) = ((config.["nugetUsername"] + config.["nugetPassword"]) = "")

let RestoreBuildTooling (config : Map<string,string>) (tools : Map<string,(string*string)>) = (fun _ ->
    let nugetInstallCommand packageId version useLocalNuget =
        if useLocalNuget
        then
            sprintf "install %s -version %s -outputdirectory ./tools -noninteractive -verbosity detailed" packageId version
        else
            sprintf "install %s -version %s -outputdirectory ./tools -noninteractive -verbosity detailed  -ConfigFile ./tools/nuget/NuGet.Config" packageId version
    tools
    |> Seq.iter (fun tool ->
        Shell.Exec(config.["nugetExe"], nugetInstallCommand (fst tool.Value) (snd tool.Value) (useLocalNuget config))
        |> ignore
    )
    ())

let CreateNugetConfig () =
    FileUtils.cp "Tools/nuget/NuGet.Config.TT" "Tools/nuget/NuGet.Config"

let AddSourceToNugetConfig (config : Map<string,string>) = (fun _ ->
    let nugetAddSourceCommand (config : Map<string,string>) = sprintf "sources add -Name Artifactory -Source %s -Username %s -Password %s  -ConfigFile ./tools/nuget/NuGet.Config" config.["artifactoryUrl"] config.["nugetUsername"] config.["nugetPassword"]
    let addNugetSourceResult = Shell.Exec(config.["nugetExe"], nugetAddSourceCommand config)
    ())

let DeleteNugetConfig () =
    DeleteFile "./tools/nuget/NuGet.Config"

let RestoreNugetPackages (config : Map<string,string>) = (fun _ ->
    let nugetRestoreCommand (config : Map<string,string>) packagesFile = 
        if useLocalNuget config
        then
            "restore " + packagesFile + " -PackagesDirectory ../packages -verbosity detailed"
        else
            "restore " + packagesFile + " -ConfigFile ./tools/nuget/NuGet.Config -PackagesDirectory ../packages"
    !! "../*/packages.config"
    |> Seq.iter (fun f -> 
        Shell.Exec(config.["nugetExe"], nugetRestoreCommand config f)
        |> ignore
    )
    ())

let publishNugetPackage (config : Map<string,string>) = (fun _ ->
    !! (sprintf "../**/bin/*.*%s.nupkg" config.["nugetPackageVersion"])
    |> Seq.iter(fun nugetPackage ->
        let pushCommand = sprintf "push %s -Source %s -ConfigFile ./tools/nuget/NuGet.Config" nugetPackage config.["publishFeed"]
        trace ("pushCommand" + pushCommand)
        let exitCode = Shell.Exec(config.["nugetExe"], pushCommand)
        ()
    ))