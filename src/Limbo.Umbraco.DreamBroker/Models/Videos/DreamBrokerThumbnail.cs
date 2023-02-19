using System;
using Limbo.Umbraco.Video.Models.Videos;

namespace Limbo.Umbraco.DreamBroker.Models.Videos {

    /// <summary>
    /// Class representing a DreamBroker thumbnail.
    /// </summary>
    public class DreamBrokerThumbnail : IVideoThumbnail {

        #region Properties

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
            string url = $"https://dreambroker.com/channel/{channelId}/{videoId}/get/poster/{width}x{height}.jpg{(crop ? "?crop=true" : "")}";
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
        /// <param name="value">The instance of <see cref="DreamBrokerValue"/> the thumbnail should be based on.</param>
        /// <param name="width">The width of the thumbnail.</param>
        /// <param name="height">The height of the thumbnail.</param>
        /// <param name="crop">Whether the thumbnail should be cropped if it doesn't match the aspect ratio of the video.</param>
        /// <returns>An instance of <see cref="DreamBrokerThumbnail"/>.</returns>
        public static DreamBrokerThumbnail Create(DreamBrokerValue value, int width, int height, bool crop) {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return Create(value.Video, width, height, crop);
        }

        #endregion

    }

}