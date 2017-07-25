#r @"tools/FAKE.4.62.5/tools/FakeLib.dll"
open Fake
open Fake.Testing
open Fake.FileSystemHelper
open Fake.OctoTools
open System.IO

#load "./modules/Nugetmod.fsx"
#load "./modules/Octomod.fsx"
#load "./modules/Parammod.fsx"

//Add required build tooling here. It will be fetched via NuGet. Upgrade tool by changing version.
let tools = Map.ofList [
                           ("nunit", ("NUnit.ConsoleRunner", "3.6.1"));
                           ("octopus", ("OctopusTools", "4.9.0"));
                       ]

let config = Map.ofList [
                "artifactoryUrl", "https://ids.jfrog.io/ids/api/nuget/central";                
                "buildConfiguration", getBuildParamOrDefault "Config" "Debug";
                "buildCounter", getBuildParamOrDefault "buildCounter" "";
                "gitHash", getBuildParamOrDefault "gitHash" "";
                "nugetExe", "./tools/nuget/NuGet.exe";
                "nugetUsername", getBuildParamOrDefault "nugetuser" "";
                "nugetPassword", getBuildParamOrDefault "nugetpassword" "";
                "nugetPackageVersion", Parammod.getNugetPackageVersion (getBuildParamOrDefault "gitHash" "");
                "publishFeed", getBuildParamOrDefault "publishto" "http://10.20.30.55:888";
                "octoApiKey", getBuildParamOrDefault "octoapikey" "";
                "octoServer", getBuildParamOrDefault "octoserver" "http://10.20.30.86";
                "octoProjectName", getBuildParamOrDefault "octoprojectname" "";
                "netCoreVersion", getBuildParamOrDefault "netcoreversion" "netcoreapp1.1";
             ]

let ValidateNugetParams() =
    [
        ("nugetuser", config.["nugetUsername"])
        ("nugetpassword", config.["nugetPassword"])
    ] |> Seq.iter Parammod.raiseIfMissingParam

let ValidateArtifactoryAndOctoParams() =
    ValidateNugetParams()
    [
        ("githash", config.["gitHash"])
        ("octoapikey", config.["octoApiKey"])
        ("buildNumber", config.["buildNumber"])
    ] |> Seq.iter Parammod.raiseIfMissingParam

Target "CreateOctoRelease" <| Octomod.CreateOctoRelease config tools

let Completed() = trace "Build completed"


Target "CreateNugetConfig" <| Nugetmod.CreateNugetConfig 
Target "AddArtifactorySource" <| Nugetmod.AddSourceToNugetConfig config
Target "DeleteNugetConfig" <| Nugetmod.DeleteNugetConfig
Target "Completed" <| Completed

Target "Clean" (fun _ ->
    !! "../*/bin/"
        ++ "../*/obj/"
    |> CleanDirs
)

let version = 
    let current = System.DateTime.Now
    [(sprintf "/p:Version=%i.%i.%i-git%s-%s" current.Year current.Month current.Day config.["gitHash"] config.["buildCounter"]);]

Target "InstallSdk"(fun _ ->
    if(DotNetCli.isInstalled() <> true) then
        DotNetCli.InstallDotNetSDK config.["netCoreVersion"] |> ignore
)

Target "Restore" (fun _ ->
    DotNetCli.Restore (fun p -> 
        { p with WorkingDir = "../";              
              AdditionalArgs = version; } )
)

Target "Build" (fun _ ->
    DotNetCli.Build (fun p -> 
        { p with WorkingDir = "../";
              Configuration = config.["buildConfiguration"];
              AdditionalArgs = version; } )
)

Target "Test" (fun _ ->    
    DotNetCli.Test (fun p -> 
        { p with WorkingDir = "../coresampletests/";
              Configuration = config.["buildConfiguration"];
              AdditionalArgs = (List.append version ["--no-build";]) } )
)

Target "Pack" (fun _ ->    
    DotNetCli.Pack (fun p -> 
        { p with WorkingDir = "../";
              OutputPath = "./bin/";
              Configuration = config.["buildConfiguration"];
              AdditionalArgs = (List.append version ["--no-build";]) } )
)

Target "Setup" (fun _ ->
    "InstallSdk"
    ==> "Clean"
    ==> "Restore"
    ==> "Build"
    ==> "Completed" |> ignore
    run "Completed"
) 

Target "Everything:Local" (fun _ ->
    "InstallSdk"
    ==> "Clean"
    ==> "Restore"
    ==> "Build"
    ==> "Test"
    ==> "Completed" |> ignore
    run "Completed"
)

Target "Everything:ToPublishFeed" (fun _ ->
    ValidateArtifactoryAndOctoParams()
    "InstallSdk"
    ==> "CreateNugetConfig" //Teamcity
    ==> "AddArtifactorySource"
    ==> "Clean"
    ==> "Restore"
    ==> "Build"
    ==> "Test"
    ==> "Pack"
    // ==> "Publish"
    // ==> "Deploy"
    ==> "DeleteNugetConfig"
    ==> "Completed" |> ignore

    run "Completed"
)

Target "Default" (fun _ ->
    trace "Please specify an explicit task"
)

RunTargetOrDefault "Default"
