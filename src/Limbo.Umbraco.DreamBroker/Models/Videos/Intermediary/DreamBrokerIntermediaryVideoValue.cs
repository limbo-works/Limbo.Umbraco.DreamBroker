using Newtonsoft.Json;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.DreamBroker.Models.Videos.Intermediary;

public class DreamBrokerIntermediaryVideoValue {

    [JsonProperty("source")]
    public string Source { get; }

    [JsonProperty("details")]
    public DreamBrokerIntermediaryVideoDetails Details { get; }

    public DreamBrokerIntermediaryVideoValue(string source, VideoItem video) {
        Source = source;
        Details = new DreamBrokerIntermediaryVideoDetails(video);
    }

}