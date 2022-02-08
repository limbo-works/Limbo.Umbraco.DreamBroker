# Limbo DreamBroker

This package features a property editor for inserting (via URL) or selecting a Dream Broker video. The property editor saves a bit of information about the video, which then will be availble in C#.

## Installation

Install for Umbraco 9 via [NuGet](https://www.nuget.org/packages/Limbo.Umbraco.DreamBroker/1.0.0-alpha002):

```
dotnet add package Limbo.Umbraco.DreamBroker --version 1.0.0-alpha002
```

## Configuration

### Authentication

DreamBroker doesn't have a public API, but relies on more or less undocumented endpoints, and as such doesn't require any authentication.

### Channels

Future versions of this package will allow users to add channels to Umbraco, and search among the videos of those channels. With the current version, there is no interface for managing channels - but when adding a new video, the user is prompted to add the channel of the video to Umbraco if the channel hasn't already been added.
