using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Limbo.Umbraco.DreamBroker.Models.Channels;
using Limbo.Umbraco.DreamBroker.Models.Videos;
using Limbo.Umbraco.DreamBroker.Services;
using Newtonsoft.Json;
using Skybrud.Essentials.Http;
using Skybrud.Essentials.Strings;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;

#pragma warning disable 1591

namespace Limbo.Umbraco.DreamBroker.Controllers {

    [PluginController("Limbo")]
    public class DreamBrokerController : UmbracoAuthorizedApiController {

        private readonly DreamBrokerService _dreamBrokerService;

        #region Constructors

        public DreamBrokerController(DreamBrokerService dreamBrokerService) {
            _dreamBrokerService = dreamBrokerService;
        }

        #endregion

        #region Public API methods

        /// <summary>
        /// Adds the channel with the specified <paramref name="channelId"/> and <paramref name="name"/> to Umbraco.
        /// </summary>
        /// <param name="channelId">The DreamBroker ID of the channel.</param>
        /// <param name="name">The name of the channel.</param>
        public object AddChannel(string channelId, string name) {
            if (string.IsNullOrWhiteSpace(channelId)) return BadRequest("No channel ID specified.");
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("No channel name specified.");
            return _dreamBrokerService.AddChannel(channelId, name);
        }

        /// <summary>
        /// Deletes the channel with the specified <paramref name="channelId"/> from Umbraco.
        /// </summary>
        /// <param name="channelId">The DreamBroker ID of the channel.</param>
        public object DeleteChannel(string channelId) {
            if (string.IsNullOrWhiteSpace(channelId)) return BadRequest("No channel ID specified.");
            DreamBrokerChannel channel = _dreamBrokerService.GetChannel(channelId);
            if (channel == null) return NotFound("Channel not found.");
            _dreamBrokerService.DeleteChannel(channel);
            return Ok();
        }

        /// <summary>
        /// Returns a list of the channels added in Umbraco.
        /// </summary>
        public object GetChannels() {
            return _dreamBrokerService.GetChannels();
        }

        /// <summary>
        /// Returns a list of videos of the channels added in Umbraco.
        /// </summary>
        /// <param name="text">If specified, only videos matching this parameter will be returned.</param>
        public object GetVideos(string text = null) {

            List<object> channels = new();

            // Iterate through the channels added to Umbraco
            foreach (DreamBrokerChannel channnel in _dreamBrokerService.GetChannels()) {

                // get the videos of the channel
                IEnumerable<VideoItem> cv = _dreamBrokerService
                    .GetChannelVideos(channnel)
                    .Where(x => string.IsNullOrWhiteSpace(text) || x.Title.InvariantIndexOf(text) >= 0 || x.VideoId == text);

                // Append the channel to the overall list
                channels.Add(new {
                    channelId = channnel.ChannelId,
                    name = channnel.Name,
                    videos = cv
                });

            }

            return new {
                channels
            };

        }

        /// <summary>
        /// Returns information about the video with the specified <paramref name="channelId"/> and <paramref name="videoId"/>.
        /// </summary>
        /// <param name="channelId">The ID of the channel.</param>
        /// <param name="videoId">The ID of the video.</param>
        public object GetVideo(string channelId, string videoId) {

            if (string.IsNullOrWhiteSpace(channelId)) return BadRequest("No channel ID specified.");
            if (string.IsNullOrWhiteSpace(videoId)) return BadRequest("No video ID specified.");

            // Get a reference to the channel (if stored in Umbraco)
            DreamBrokerChannel channel = _dreamBrokerService.GetChannel(channelId);


            DreamBrokerChannelDetails channelDetails;

            if (channel == null) {

                channelDetails = new DreamBrokerChannelDetails(channelId);

                try {
                    IHttpResponse response = HttpUtils.Requests.Get("https://dreambroker.com/channel/" + channelId);
                    if (response.StatusCode == HttpStatusCode.OK) {
                        if (RegexUtils.IsMatch(response.Body, "channelTitle: '(.+?)',", out Match match)) {
                            channelDetails = new DreamBrokerChannelDetails(channelId, match.Groups[1].Value, false);
                        }
                    }
                } catch {
                    // Ignore
                }

            } else {
                channelDetails = new DreamBrokerChannelDetails(channel);
            }





            // As DreamBroker doesn't really have an API, we get all the videos of the channel via their internal API,
            // and then pick the video with the matching ID
            VideoItem video = _dreamBrokerService.GetChannelVideos(channelId).FirstOrDefault(x => x.VideoId == videoId);
            if (video == null) return NotFound("Video not found.");

            return new {
                channel = channelDetails,
                video
            };

        }

        #endregion

    }

    public class DreamBrokerChannelDetails {

        [JsonProperty("channelId")]
        public string ChannelId { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("exists")]
        public bool Exists { get; }

        public DreamBrokerChannelDetails(string channelId) {
            ChannelId = channelId;
            Exists = false;
        }

        public DreamBrokerChannelDetails(string channelId, string name, bool exists) {
            ChannelId = channelId;
            Name = name;
            Exists = exists;
        }

        public DreamBrokerChannelDetails(DreamBrokerChannel channel) {
            ChannelId = channel.ChannelId;
            Name = channel.Name;
            Exists = true;
        }

    }

}