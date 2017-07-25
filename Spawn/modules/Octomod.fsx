#r @"tools/FAKE.4.62.5/tools/FakeLib.dll"
open Fake.OctoTools
open System

let CreateOctoRelease (config : Map<string,string>) (tools : Map<string,(string*string)>) = (fun _ ->
    let releaseSpec = {
        releaseOptions with
            Project = config.["octoProjectName"];
            Version = (sprintf "%sT%s" config.["nugetPackageVersion"] (DateTime.Now.ToString "yyyyMMddHHmmss"));
            PackageVersion = config.["nugetPackageVersion"];
            IgnoreExisting = true;
    }
    let deploySpec = {
        deployOptions with
            DeployTo=config.["buildConfiguration"];
            Force=true;
            WaitForDeployment=true;
    }
    let server = {
        ApiKey = config.["octoApiKey"];
        Server = config.["octoServer"]
    }
    let buildTool = tools.["octopus"]
    Octo (fun p ->
    {
        p with
            Command = CreateRelease (releaseSpec, Some deploySpec)
            Server = server
            ToolPath = (sprintf "./tools/%s.%s/tools/" (fst buildTool) (snd buildTool))
    }))
