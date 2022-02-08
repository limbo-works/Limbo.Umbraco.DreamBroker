using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Converters.Time;
using Skybrud.Essentials.Json.Extensions;

namespace Limbo.Umbraco.DreamBroker.Models {
    
    /// <summary>
    /// Class representing the details if a DreamBroker video.
    /// </summary>
    public class DreamBrokerVideoDetails {

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

        /// <summary>
        /// Gets an array with the thumbnails of the video.
        /// </summary>
        [JsonProperty("thumbnails")]
        public DreamBrokerThumbnail[] Thumbnails { get; }

        #endregion

        #region Constructors

        private DreamBrokerVideoDetails(JObject json) {
            ChannelId = json.GetString("channelId");
            VideoId = json.GetString("videoId");
            Title = json.GetString("title");
            Duration = json.GetDouble("duration", TimeSpan.FromSeconds);
            Thumbnails = new[] {
                DreamBrokerThumbnail.Create(this, 470, 264, true)
            };
        }

        #endregion

        #region Static methods

        internal static DreamBrokerVideoDetails Parse(JObject json) {
            return json == null ? null : new DreamBrokerVideoDetails(json);
        }

        #endregion

    }

}