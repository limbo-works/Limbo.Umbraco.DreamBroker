# Limbo DreamBroker

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md) [![NuGet](https://img.shields.io/nuget/v/Limbo.Umbraco.DreamBroker.svg)](https://www.nuget.org/packages/Limbo.Umbraco.DreamBroker) [![NuGet](https://img.shields.io/nuget/dt/Limbo.Umbraco.DreamBroker.svg)](https://www.nuget.org/packages/Limbo.Umbraco.DreamBroker) [![Our Umbraco](https://img.shields.io/badge/our-umbraco-%233544B1)](https://our.umbraco.com/packages/backoffice-extensions/limbo-dreambroker/) [![Umbraco Marketplace](https://img.shields.io/badge/umbraco-marketplace-%233544B1)](https://marketplace.umbraco.com/package/limbo.umbraco.dreambroker)

**Limbo.Umbraco.DreamBroker** is a package for Umbraco 10+ that features a property editor for inserting (via URL) or selecting a Dream Broker video. The property editor saves a bit of information about the video, which then will be availble in C#.

The latest version (`v2.x`) supports Umbraco 10, 11 and 12, whereas older releases (`v1.x`) supports Umbraco 9.

<table>
  <tr>
    <td><strong>License:</strong></td>
    <td><a href="./LICENSE.md"><strong>MIT License</strong></a></td>
  </tr>
  <tr>
    <td><strong>Umbraco:</strong></td>
    <td>Umbraco 10, 11 and 12</td>
  </tr>
  <tr>
    <td><strong>Target Framework:</strong></td>
    <td>.NET 6</td>
  </tr>
</table>




<br /><br />

## Installation

The package targets Umbraco 10+ and is available via [**NuGet**](https://www.nuget.org/packages/Limbo.Umbraco.DreamBroker/2.0.3). To install the package, you can use either .NET CLI:

```
dotnet add package Limbo.Umbraco.DreamBroker --version 2.0.3
```

or the NuGet Package Manager:

```
Install-Package Limbo.Umbraco.DreamBroker -Version 2.0.3
```

For Umbraco 9, see the [`v1/main`](https://github.com/limbo-works/Limbo.Umbraco.DreamBroker/tree/v1/main) branch instead.





<br /><br />

## Configuration

### Authentication

DreamBroker doesn't have a public API, but relies on more or less undocumented endpoints that exposes already public data, and as such doesn't require any authentication.

### Channels

Future versions of this package will allow users to add channels to Umbraco, and search among the videos of those channels. With the current version, there is no interface for managing channels - but when adding a new video, the user is prompted to add the channel of the video to Umbraco if the channel hasn't already been added.
