# Limbo.Umbraco.DreamBroker

DreamBroker video picker for Umbraco.




## Configuration

### Authentication

DreamBroker doesn't have a public API, but relies on more or less undocumented endpoints that exposes already public data, and as such doesn't require any authentication.

### Channels

Future versions of this package will allow users to add channels to Umbraco, and search among the videos of those channels. With the current version, there is no interface for managing channels - but when adding a new video, the user is prompted to add the channel of the video to Umbraco if the channel hasn't already been added.
