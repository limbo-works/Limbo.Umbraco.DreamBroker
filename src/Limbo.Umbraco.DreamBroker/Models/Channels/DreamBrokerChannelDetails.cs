// [CHANGE: Umbraco 17 upgrade - moved out of Controllers/DreamBrokerController.cs (and into the models namespace,
// where it belongs) while that file was rewritten for the Management API]
// Related: Controllers/DreamBrokerController.cs, Services/DreamBrokerService.cs

using Newtonsoft.Json;

namespace Limbo.Umbraco.DreamBroker.Models.Channels;

/// <summary>
/// Class describing a DreamBroker channel as returned to the backoffice - including whether the channel has already
/// been added to Umbraco.
/// </summary>
public class DreamBrokerChannelDetails {

    /// <summary>
    /// Gets the DreamBroker ID of the channel.
    /// </summary>
    [JsonProperty("channelId")]
    public string ChannelId { get; }

    /// <summary>
    /// Gets the name of the channel, if known.
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; }

    /// <summary>
    /// Gets whether the channel has been added to Umbraco.
    /// </summary>
    [JsonProperty("exists")]
    public bool Exists { get; }

    /// <summary>
    /// Initializes a new instance for a channel that is not known to Umbraco, and whose name could not be determined.
    /// </summary>
    /// <param name="channelId">The DreamBroker ID of the channel.</param>
    public DreamBrokerChannelDetails(string channelId) {
        ChannelId = channelId;
        Exists = false;
    }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="channelId"/>, <paramref name="name"/> and <paramref name="exists"/>.
    /// </summary>
    /// <param name="channelId">The DreamBroker ID of the channel.</param>
    /// <param name="name">The name of the channel.</param>
    /// <param name="exists">Whether the channel has been added to Umbraco.</param>
    public DreamBrokerChannelDetails(string channelId, string name, bool exists) {
        ChannelId = channelId;
        Name = name;
        Exists = exists;
    }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="channel"/> as saved in Umbraco.
    /// </summary>
    /// <param name="channel">The channel.</param>
    public DreamBrokerChannelDetails(DreamBrokerChannel channel) {
        ChannelId = channel.ChannelId;
        Name = channel.Name;
        Exists = true;
    }

}
