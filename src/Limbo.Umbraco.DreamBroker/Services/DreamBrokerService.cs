using System;
using System.Linq;
using System.Net;
using Limbo.Umbraco.DreamBroker.Models;
using Newtonsoft.Json;
using Skybrud.Essentials.Http;
using Skybrud.Essentials.Http.Collections;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Time;
using Umbraco.Cms.Core.Services;

namespace Limbo.Umbraco.DreamBroker.Services {
    
    /// <summary>
    /// Service for working with the DreamBroker integration.
    /// </summary>
    public class DreamBrokerService {
        
        private readonly IKeyValueService _keyValueService;

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified dependencies.
        /// </summary>
        /// <param name="keyValueService">A reference to the current <see cref="IKeyValueService"/>.</param>
        public DreamBrokerService(IKeyValueService keyValueService) {
            _keyValueService = keyValueService;
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Adds a new channel to Umbraco.
        /// </summary>
        /// <param name="channelId">The DreamBroker ID of the channel.</param>
        /// <param name="name">A friendly name of the channel. The name doesn't have to match the channel's actual name.</param>
        /// <returns>An instance of <see cref="DreamBrokerChannel"/> representing the created channel.</returns>
        public DreamBrokerChannel AddChannel(string channelId, string name) {

            // Initialize a new channel
            DreamBrokerChannel channel = new() {
                Key = Guid.NewGuid(),
                Name = name,
                ChannelId = channelId,
                CreateDate = EssentialsTime.UtcNow,
                UpdateDate = EssentialsTime.UtcNow
            };

            // Save the channel in Umbraco
            _keyValueService.SetValue($"Limbo.Umbraco.DreamBroker.Channels.{channel.Key}", JsonConvert.SerializeObject(channel));

            return channel;

        }

        /// <summary>
        /// Returns information about the channel stored in Umbraco.
        /// </summary>
        /// <param name="channelId">The ID or key of the channel.</param>
        /// <returns>An instance of <see cref="DreamBrokerChannel"/>, or <c>null</c> if not found.</returns>
        public DreamBrokerChannel GetChannel(string channelId) {
            if (string.IsNullOrWhiteSpace(channelId)) throw new ArgumentNullException(nameof(channelId));
            return GetChannels().FirstOrDefault(x => x.ChannelId == channelId || x.Key.ToString() == channelId);
        }

        /// <summary>
        /// Deletes the specified channel from Umbraco.
        /// </summary>
        /// <param name="channel">The channel to be deleted.</param>
        public void DeleteChannel(DreamBrokerChannel channel) {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            _keyValueService.SetValue($"Limbo.Umbraco.DreamBroker.Channels.{channel.Key}", null);
        }

        /// <summary>
        /// Returns an array of the DreamBroker channels added in Umbraco.
        /// </summary>
        /// <returns>An array of <see cref="DreamBrokerChannel"/>.</returns>
        public DreamBrokerChannel[] GetChannels() {
            return _keyValueService
                .FindByKeyPrefix("Limbo.Umbraco.DreamBroker.Channels.").Values
                .Select(x => JsonUtils.ParseJsonObject(x, DreamBrokerChannel.Parse))
                .ToArray();
        }

        /// <summary>
        /// Returns a list of videos of the channel matching the specified <paramref name="channelId"/>.
        /// </summary>
        /// <param name="channelId">The ID of the channel.</param>
        /// <returns>An array of <see cref="VideoItem"/>.</returns>
        public VideoItem[] GetChannelVideos(string channelId) {

            // Validate the input
            if (string.IsNullOrWhiteSpace(channelId)) throw new ArgumentNullException(nameof(channelId));

            // Make the request to the "API"
            IHttpResponse response = HttpUtils.Requests
                .Get($"https://www.dreambroker.com/channel/v2/{channelId}/searchresources?limit=1000&offset=0");

            // Parse the response and return the videos
            return JsonUtils
                .ParseJsonArray(response.Body, x => VideoItem.Parse(channelId, x))
                .Where(x => x.JObject.GetString("type") == "VIDEO")
                .ToArray();

        }

        /// <summary>
        /// Returns a list of videos of the specified <paramref name="channel"/>.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>An array of <see cref="VideoItem"/>.</returns>
        public VideoItem[] GetChannelVideos(DreamBrokerChannel channel) {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return GetChannelVideos(channel.ChannelId);
        }

        /// <summary>
        /// Returns OEmbed details for the video matching the specified <paramref name="channelId"/> and <paramref name="videoId"/>.
        /// </summary>
        /// <param name="channelId">The ID of the channel.</param>
        /// <param name="videoId">The ID of the video.</param>
        /// <returns>An instance of <see cref="DreamBrokerOEmbed"/>.</returns>
        public DreamBrokerOEmbed GetOEmbed(string channelId, string videoId) {
            
            // Input validation
            if (string.IsNullOrWhiteSpace(channelId)) throw new ArgumentNullException(nameof(channelId));
            if (string.IsNullOrWhiteSpace(videoId)) throw new ArgumentNullException(nameof(videoId));

            // Initialize the query string
            IHttpQueryString query = new HttpQueryString {
                { "url", $"https://dreambroker.com/channel/{channelId}/{videoId}" },
                { "format", "json" }
            };

            // Get video information from the OEmbed endpoint
            IHttpResponse response = HttpUtils.Requests
                    .Get("https://dreambroker.com/channel/oembed", query);

            // Validate the response
            if (response.StatusCode != HttpStatusCode.OK) throw new Exception($"Failed getting OEmbed details for video.\r\n\r\nChannel ID: {channelId}\r\nVideo ID: {videoId}");

            // Parse the response body
            return JsonUtils.ParseJsonObject(response.Body, DreamBrokerOEmbed.Parse);

        }

        #endregion

    }

}