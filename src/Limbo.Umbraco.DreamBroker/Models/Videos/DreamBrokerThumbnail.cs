using System;
using System.Threading.Channels;
using Limbo.Umbraco.Video.Models.Videos;
using Newtonsoft.Json;

namespace Limbo.Umbraco.DreamBroker.Models.Videos;

/// <summary>
/// Class representing a DreamBroker thumbnail.
/// </summary>
public class DreamBrokerThumbnail : IVideoThumbnail {

    #region Properties

    /// <summary>
    /// Gets the alias of the thumbnail, if any.
    /// </summary>
    [JsonProperty("alias", NullValueHandling = NullValueHandling.Ignore, Order = -999)]
    public string? Alias { get; }

    /// <summary>
    /// Gets the width of the thumbnail.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the height of the thumbnail.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets the URL of the thumbnail.
    /// </summary>
    public string Url { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new thumbnail based on the specified <paramref name="alias"/>, <paramref name="width"/>, <paramref name="height"/> and
    /// <paramref name="url"/>.
    /// </summary>
    /// <param name="alias">The alias of the thumbnail.</param>
    /// <param name="width">The width of the thumbnail.</param>
    /// <param name="height">The height of the thumbnail.</param>
    /// <param name="url">The URL of the thumbnail.</param>
    public DreamBrokerThumbnail(string? alias, int width, int height, string url) {
        Alias = alias;
        Width = width;
        Height = height;
        Url = url;
    }

    /// <summary>
    /// Initializes a new thumbnail based on the specified <paramref name="width"/>, <paramref name="height"/> and
    /// <paramref name="url"/>.
    /// </summary>
    /// <param name="width">The width of the thumbnail.</param>
    /// <param name="height">The height of the thumbnail.</param>
    /// <param name="url">The URL of the thumbnail.</param>
    public DreamBrokerThumbnail(int width, int height, string url) {
        Width = width;
        Height = height;
        Url = url;
    }

    #endregion

    #region Static methods

    internal static string GetPosterUrl(string channelId, string videoId) {
        return $"https://dreambroker.com/channel/{channelId}/{videoId}/get/poster";
    }

    internal static string GetThumnailUrl(string channelId, string videoId, int width, int height, bool crop) {
        if (string.IsNullOrWhiteSpace(channelId)) throw new ArgumentNullException(nameof(channelId));
        if (string.IsNullOrWhiteSpace(videoId)) throw new ArgumentNullException(nameof(videoId));
        return $"https://dreambroker.com/channel/{channelId}/{videoId}/get/poster/{width}x{height}.jpg{(crop ? "?crop=true" : "")}";
    }

    /// <summary>
    /// Creates a new thumbnail based on the specified <paramref name="channelId"/> and <paramref name="videoId"/>.
    /// The thumbnail will have a size of 470x264 pixels.
    /// </summary>
    /// <param name="channelId">The ID of the channel.</param>
    /// <param name="videoId">The ID of the video.</param>
    /// <returns>An instance of <see cref="DreamBrokerThumbnail"/>.</returns>
    public static DreamBrokerThumbnail Create(string channelId, string videoId) {
        return Create(channelId, videoId, 470, 264, true);
    }

    /// <summary>
    /// Creates a new thumbnail based on the specified <paramref name="channelId"/> and <paramref name="videoId"/>.
    /// </summary>
    /// <param name="channelId">The ID of the channel.</param>
    /// <param name="videoId">The ID of the video.</param>
    /// <param name="width">The width of the thumbnail.</param>
    /// <param name="height">The height of the thumbnail.</param>
    /// <param name="crop">Whether the thumbnail should be cropped if it doesn't match the aspect ratio of the video.</param>
    /// <returns>An instance of <see cref="DreamBrokerThumbnail"/>.</returns>
    public static DreamBrokerThumbnail Create(string channelId, string videoId, int width, int height, bool crop) {
        if (string.IsNullOrWhiteSpace(channelId)) throw new ArgumentNullException(nameof(channelId));
        if (string.IsNullOrWhiteSpace(videoId)) throw new ArgumentNullException(nameof(videoId));
        string url = width is 0 && height is 0 ? GetPosterUrl(channelId, videoId) : GetThumnailUrl(channelId, videoId, width, height, crop);
        return new DreamBrokerThumbnail(width, height, url);
    }

    /// <summary>
    /// Creates a new thumbnail based on the specified <paramref name="video"/>.
    /// </summary>
    /// <param name="video">The instance of <see cref="DreamBrokerVideoDetails"/> the thumbnail should be based on.</param>
    /// <param name="width">The width of the thumbnail.</param>
    /// <param name="height">The height of the thumbnail.</param>
    /// <param name="crop">Whether the thumbnail should be cropped if it doesn't match the aspect ratio of the video.</param>
    /// <returns>An instance of <see cref="DreamBrokerThumbnail"/>.</returns>
    public static DreamBrokerThumbnail Create(DreamBrokerVideoDetails video, int width, int height, bool crop) {
        if (video == null) throw new ArgumentNullException(nameof(video));
        return Create(video.ChannelId, video.VideoId, width, height, crop);
    }

    /// <summary>
    /// Creates a new thumbnail based on the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The instance of <see cref="DreamBrokerVideoValue"/> the thumbnail should be based on.</param>
    /// <param name="width">The width of the thumbnail.</param>
    /// <param name="height">The height of the thumbnail.</param>
    /// <param name="crop">Whether the thumbnail should be cropped if it doesn't match the aspect ratio of the video.</param>
    /// <returns>An instance of <see cref="DreamBrokerThumbnail"/>.</returns>
    public static DreamBrokerThumbnail Create(DreamBrokerVideoValue value, int width, int height, bool crop) {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return Create(value.Details, width, height, crop);
    }

    /// <summary>
    /// Creates and returns a new <c>poster</c> thumbnail based on the specified vieo <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value representing the video.</param>
    /// <returns>An instance of <see cref="DreamBrokerThumbnail"/> representing the poster.</returns>
    public static DreamBrokerThumbnail CreatePoster(DreamBrokerVideoDetails value) {
        return new DreamBrokerThumbnail("poster", 0, 0, GetPosterUrl(value.ChannelId, value.VideoId));
    }

    #endregion

}