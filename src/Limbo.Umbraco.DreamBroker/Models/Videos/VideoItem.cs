using System;
using Limbo.Umbraco.DreamBroker.Models.Channels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Converters.Time;
using Skybrud.Essentials.Json.Extensions;

namespace Limbo.Umbraco.DreamBroker.Models.Videos {
    
    /// <summary>
    /// Class representing a video as received from the DreamBroker API.
    /// </summary>
    public class VideoItem : JsonObjectBase {

        #region Properties

        /// <summary>
        /// Gets the ID of the video.
        /// </summary>
        [JsonProperty("videoId")]
        public string VideoId { get; }
        
        /// <summary>
        /// Gets the ID of the parent channel.
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
        [JsonConverter(typeof(TimeSpanSecondsConverter))]
        public TimeSpan Duration { get; }

        #endregion

        #region Constructors

        private VideoItem(string channelId, JObject json) : base(json) {
            Title = json.GetString("title");
            VideoId = json.GetString("relativePath");
            ChannelId = channelId;
            Duration = json.GetDouble("duration", TimeSpan.FromMilliseconds);
        }

        #endregion

        #region Static methods

        internal static VideoItem Parse(string channelId, JObject json) {
            return json == null ? null : new VideoItem(channelId, json);
        }

        internal static VideoItem Parse(DreamBrokerChannel channel, JObject json) {
            return json == null ? null : new VideoItem(channel.ChannelId, json);
        }

        #endregion

    }

}