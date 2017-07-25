ECHO OFF

.\tools\nuget\nuget install Fake -version 4.50.0 -outputdirectory .\tools -noninteractive -source https://www.nuget.org/api/v2/

.\tools\FAKE.4.55.0\tools\FAKE.exe Build.fsx PublishToArtifactory %*

ECHO ON