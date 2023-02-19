@echo off
dotnet build src/Limbo.Umbraco.DreamBroker --configuration Debug /t:rebuild /t:pack -p:PackageOutputPath=c:\nuget\Umbraco10