ECHO OFF

.\tools\nuget\nuget install Fake -version 4.62.5 -outputdirectory .\tools -noninteractive -source https://www.nuget.org/api/v2/

.\tools\FAKE.4.62.5\tools\FAKE.exe Build.fsx %*

ECHO ON