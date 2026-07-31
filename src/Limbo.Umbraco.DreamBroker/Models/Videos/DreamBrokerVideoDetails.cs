using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Limbo.Umbraco.Video.Models.Videos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft.Converters.Time;
using Skybrud.Essentials.Json.Newtonsoft.Extensions;

namespace Limbo.Umbraco.DreamBroker.Models.Videos;

/// <summary>
/// Class representing the details if a DreamBroker video.
/// </summary>
public class DreamBrokerVideoDetails : IVideoDetails {

    #region Properties

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
    /// Gets the Vimeo URL of the video.
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; }

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
    public TimeSpan? Duration { get; }

    /// <summary>
    /// Gets an array with the thumbnails of the video.
    /// </summary>
    [JsonProperty("thumbnails")]
    public DreamBrokerThumbnail[] Thumbnails { get; }

    // [CHANGE: Umbraco 17 upgrade - IVideoDetails in Limbo.Umbraco.Video 17 exposes these as IReadOnlyList<T> rather
    // than IEnumerable<T>] Related: Models/Videos/DreamBrokerVideoValue.cs, Limbo.Umbraco.DreamBroker.csproj
    IReadOnlyList<IVideoThumbnail> IVideoDetails.Thumbnails => Thumbnails;

    /// <summary>
    /// Gets an array with the files of the video. This will currently always be empty.
    /// </summary>
    [JsonIgnore]
    public IReadOnlyList<IVideoFile> Files { get; }

    #endregion

    #region Constructors

    private DreamBrokerVideoDetails(JObject json) {
        ChannelId = json.GetString("channelId")!;
        VideoId = json.GetString("videoId")!;
        Url = $"https://www.dreambroker.com/channel/{ChannelId}/{VideoId}";
        Title = json.GetString("title")!;
        Duration = json.GetDouble("duration", TimeSpan.FromSeconds);
        Thumbnails = [
            DreamBrokerThumbnail.Create(this, 470, 264, true),
            DreamBrokerThumbnail.CreatePoster(this)
        ];
        Files = [];
    }

    #endregion

    #region Static methods

    internal static DreamBrokerVideoDetails? Parse(JObject? json) {

        if (json is null) return null;

        // Both IDs are required - the URL and the thumbnails are derived from them, and DreamBrokerThumbnail.Create
        // throws if either is missing
        if (string.IsNullOrWhiteSpace(json.GetString("channelId"))) return null;
        if (string.IsNullOrWhiteSpace(json.GetString("videoId"))) return null;

        return new DreamBrokerVideoDetails(json);

    }

    #endregion

}