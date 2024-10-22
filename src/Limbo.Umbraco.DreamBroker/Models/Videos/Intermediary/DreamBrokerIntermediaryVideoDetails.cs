using Newtonsoft.Json;
using Skybrud.Essentials.Json.Converters.Time;
using System;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.DreamBroker.Models.Videos.Intermediary;

public class DreamBrokerIntermediaryVideoDetails {

    [JsonProperty("videoId")]
    public string VideoId { get; }

    [JsonProperty("channelId")]
    public string ChannelId { get; }

    [JsonProperty("title")]
    public string Title { get; }

    [JsonProperty("duration")]
    [JsonConverter(typeof(TimeSpanSecondsConverter))]
    public TimeSpan Duration { get; }

    public DreamBrokerIntermediaryVideoDetails(VideoItem video) {
        VideoId = video.VideoId;
        ChannelId = video.ChannelId;
        Title = video.Title;
        Duration = video.Duration;
    }

}