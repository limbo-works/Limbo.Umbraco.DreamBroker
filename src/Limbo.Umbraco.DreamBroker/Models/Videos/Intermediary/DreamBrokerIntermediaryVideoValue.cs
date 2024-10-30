using Newtonsoft.Json;

namespace Limbo.Umbraco.DreamBroker.Models.Videos.Intermediary;

/// <summary>
/// Class representing the intermediary value for a Dream Broker video. When serialized to JSON, this value equals the property value saved to the database.
/// </summary>
public class DreamBrokerIntermediaryVideoValue {

    /// <summary>
    /// Gets the source (URL) as entered by the user.
    /// </summary>
    [JsonProperty("source")]
    public string Source { get; }

    /// <summary>
    /// Gets the details about the picked video.
    /// </summary>
    [JsonProperty("details")]
    public DreamBrokerIntermediaryVideoDetails Details { get; }

    /// <summary>
    /// Initializes  new instance based on the specified <paramref name="source"/> and <paramref name="video"/>.
    /// </summary>
    /// <param name="source">The source (URL) as entered by the user.</param>
    /// <param name="video">The <see cref="VideoItem"/> as received from the Dream Broker website.</param>
    public DreamBrokerIntermediaryVideoValue(string source, VideoItem video) {
        Source = source;
        Details = new DreamBrokerIntermediaryVideoDetails(video);
    }
}