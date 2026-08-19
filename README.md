# Limbo DreamBroker

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/limbo-works/Limbo.Umbraco.DreamBroker/blob/v17/main/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/Limbo.Umbraco.DreamBroker.svg)](https://www.nuget.org/packages/Limbo.Umbraco.DreamBroker)
[![NuGet](https://img.shields.io/nuget/dt/Limbo.Umbraco.DreamBroker.svg)](https://www.nuget.org/packages/Limbo.Umbraco.DreamBroker)
[![Our Umbraco](https://img.shields.io/badge/our-umbraco-%233544B1)](https://our.umbraco.com/packages/backoffice-extensions/limbo-dreambroker/)
[![Umbraco Marketplace](https://img.shields.io/badge/umbraco-marketplace-%233544B1)](https://marketplace.umbraco.com/package/limbo.umbraco.dreambroker)

**Limbo.Umbraco.DreamBroker** is a package for Umbraco that features a property editor for inserting (via URL) or selecting a DreamBroker video. The property editor saves a bit of information about the video, which then will be availble in C#.

The latest version (`v17.x`) supports Umbraco 17, whereas older releases support Umbraco 13 (`v13.x`), Umbraco 10-12 (`v2.x`) and Umbraco 9 (`v1.x`).

<table>
  <tr>
    <td><strong>License:</strong></td>
    <td><a href="https://github.com/limbo-works/Limbo.Umbraco.DreamBroker/blob/v17/main/LICENSE.md"><strong>MIT License</strong></a></td>
  </tr>
  <tr>
    <td><strong>Umbraco:</strong></td>
    <td>Umbraco 17</td>
  </tr>
  <tr>
    <td><strong>Target Framework:</strong></td>
    <td>.NET 10</td>
  </tr>
</table>




<br /><br />

## Installation

### Umbraco 17

The package targets Umbraco 17 and is available via [**NuGet**](https://www.nuget.org/packages/Limbo.Umbraco.DreamBroker/17.0.0-alpha000). To install the package, you can use either .NET CLI:

```
dotnet add package Limbo.Umbraco.DreamBroker --version 17.0.0-alpha000
```

or the NuGet Package Manager:

```
Install-Package Limbo.Umbraco.DreamBroker -Version 17.0.0-alpha000
```

> **Note:** this is a prerelease, so remember to allow prerelease packages when installing.

### Other versions of Umbraco

- [**`v13/main`**](https://github.com/limbo-works/Limbo.Umbraco.DreamBroker/tree/v13/main) Umbraco 13
- ~~[**`v2/main`**](https://github.com/limbo-works/Limbo.Umbraco.DreamBroker/tree/v2/main) Umbraco 10-12 <sub title="Umbraco 10, 11 and 12 have reached end-of-life"><sup>(EOL)</sup>
- ~~[**`v1/main`**](https://github.com/limbo-works/Limbo.Umbraco.DreamBroker/tree/v1/main) Umbraco 9 <sub title="Umbraco 9 has reached end-of-life"><sup>(EOL)</sup></sub>




<br /><br />

## Configuration

### Authentication

DreamBroker doesn't have a public API, but relies on more or less undocumented endpoints that exposes already public data, and as such doesn't require any authentication.

### Channels

Future versions of this package will allow users to add channels to Umbraco, and search among the videos of those channels. With the current version, there is no interface for managing channels - but when adding a new video, the user is prompted to add the channel of the video to Umbraco if the channel hasn't already been added.




<br /><br />

## Upgrading from v13

The saved property value is unchanged, so existing content keeps working. The backoffice part of the package was rewritten for the new backoffice, and a few server side APIs changed along with it - see [**documentation/umbraco-17-upgrade.md**](./documentation/umbraco-17-upgrade.md) for the full list of breaking changes.
