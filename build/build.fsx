#r "FakeLib.dll"
open Fake
open Fake.NuGet.Install
open Fake.Testing.XUnit

Target "Clean" (fun _ ->
    CleanDir "./build/release"
    CleanDir "./build/tests"
    CleanDir "./build/package"
)

Target "Restore" (fun _ ->
    RestorePackages()
    "xunit.runner.console"
        |> NugetInstall (fun p ->
            { p with
                OutputDirectory = "./build"})
)

Target "BuildApp" (fun _ ->
    !! "src/**/omlet.csproj"
      |> MSBuildRelease "./build/release" "Build"
      |> Log "Application-Build-Output: "
)

Target "BuildTests" (fun _ ->
    !! "src/**/omlet.tests.csproj"
      |> MSBuildDebug "./build/tests" "Build"
      |> Log "Tests-Build-Output: "
)

Target "ExecuteTests" (fun _ ->
    !! ("./build/tests/*.Tests.dll")
        |> xUnit (fun p -> 
            { p with
                ToolPath = findToolInSubPath "xunit.console.x86.exe" "./build" })
)

Target "CreatePackage" (fun _ ->
     NuGet (fun p -> 
        { p with
            Version = "1.0"
            OutputPath = "./build/package"
            WorkingDir = "./build/release"
            Dependencies = []
            Files = [( "Omlet.dll", Some "lib\\net45", None )]
            Publish = false }) "./build/build.nuspec"
)

Target "Default" (fun _ ->
    trace "Build completed."
)

"Clean" 
    ==> "Restore"
    ==> "BuildApp"
    ==> "BuildTests"
    ==> "ExecuteTests"
    ==> "CreatePackage"
    ==> "Default"    

RunTargetOrDefault "Default"