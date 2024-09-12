# Limbo DreamBroker

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/limbo-works/Limbo.Umbraco.DreamBroker/blob/v13/main/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/Limbo.Umbraco.DreamBroker.svg)](https://www.nuget.org/packages/Limbo.Umbraco.DreamBroker)
[![NuGet](https://img.shields.io/nuget/dt/Limbo.Umbraco.DreamBroker.svg)](https://www.nuget.org/packages/Limbo.Umbraco.DreamBroker)
[![Our Umbraco](https://img.shields.io/badge/our-umbraco-%233544B1)](https://our.umbraco.com/packages/backoffice-extensions/limbo-dreambroker/)
[![Umbraco Marketplace](https://img.shields.io/badge/umbraco-marketplace-%233544B1)](https://marketplace.umbraco.com/package/limbo.umbraco.dreambroker)

**Limbo.Umbraco.DreamBroker** is a package for Umbraco that features a property editor for inserting (via URL) or selecting a DreamBroker video. The property editor saves a bit of information about the video, which then will be availble in C#.

The latest version (`v13.x`) supports Umbraco 13, whereas older releases support support Umbraco 10-12 (`v2.x`) and Umbraco 9 (`v1.x`).

<table>
  <tr>
    <td><strong>License:</strong></td>
    <td><a href="https://github.com/limbo-works/Limbo.Umbraco.DreamBroker/blob/v2/main/LICENSE.md"><strong>MIT License</strong></a></td>
  </tr>
  <tr>
    <td><strong>Umbraco:</strong></td>
    <td>Umbraco 13</td>
  </tr>
  <tr>
    <td><strong>Target Framework:</strong></td>
    <td>.NET 8</td>
  </tr>
</table>




<br /><br />

## Installation

### Umbraco 13

The package targets Umbraco 13 and is available via [**NuGet**](https://www.nuget.org/packages/Limbo.Umbraco.DreamBroker/13.0.0). To install the package, you can use either .NET CLI:

```
dotnet add package Limbo.Umbraco.DreamBroker --version 13.0.0
```

or the NuGet Package Manager:

```
Install-Package Limbo.Umbraco.DreamBroker -Version 13.0.0
```

### Other versions of Umbraco

- [**`v2/main`**](https://github.com/limbo-works/Limbo.Umbraco.DreamBroker/tree/v2/main) (Umbraco 10-12)
- [**`v1/main`**](https://github.com/limbo-works/Limbo.Umbraco.DreamBroker/tree/v1/main) (Umbraco 9)




<br /><br />

## Configuration

### Authentication

DreamBroker doesn't have a public API, but relies on more or less undocumented endpoints that exposes already public data, and as such doesn't require any authentication.

### Channels

Future versions of this package will allow users to add channels to Umbraco, and search among the videos of those channels. With the current version, there is no interface for managing channels - but when adding a new video, the user is prompted to add the channel of the video to Umbraco if the channel hasn't already been added.
