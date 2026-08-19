using System;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Newtonsoft.Converters.Time;

namespace Limbo.Umbraco.DreamBroker.Models.Videos.Intermediary;

/// <summary>
/// Class representing the intermediary details about a Dream Broker video.
/// </summary>
public class DreamBrokerIntermediaryVideoDetails {

    /// <summary>
    /// Gets the DreamBroker ID of the video.
    /// </summary>
    [JsonProperty("videoId")]
    public string VideoId { get; }

    /// <summary>
    /// Gets the DreamBroker ID of the channel.
    /// </summary>
    [JsonProperty("channelId")]
    public string ChannelId { get; }

    /// <summary>
    /// Gets the title of the video.
    /// </summary>
    [JsonProperty("title")]
    public string Title { get; }

    /// <summary>
    /// Gets the duration of the video.
    /// </summary>
    [JsonProperty("duration")]
    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan Duration { get; }

    /// <summary>
    /// Initializes a new instance based on the specified <see cref="VideoItem"/>.
    /// </summary>
    /// <param name="video"></param>
    public DreamBrokerIntermediaryVideoDetails(VideoItem video) {
        VideoId = video.VideoId;
        ChannelId = video.ChannelId;
        Title = video.Title;
        Duration = video.Duration;
    }

}