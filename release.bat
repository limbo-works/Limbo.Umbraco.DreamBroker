@echo off
dotnet build src/Limbo.Umbraco.DreamBroker --configuration Release /t:rebuild /t:pack -p:PackageOutputPath=../../releases/nuget