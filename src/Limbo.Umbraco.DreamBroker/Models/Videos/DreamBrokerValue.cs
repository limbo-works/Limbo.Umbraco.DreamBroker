using Limbo.Umbraco.DreamBroker.PropertyEditors;
using Limbo.Umbraco.Video.Models.Videos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Limbo.Umbraco.DreamBroker.Models.Videos {

    /// <summary>
    /// Class representing the value of a <see cref="DreamBrokerEditor"/> property editor.
    /// </summary>
    public class DreamBrokerValue : IVideoValue {

        #region Properties

        /// <summary>
        /// Gets the source (URL) as entered by the user.
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; }

        /// <summary>
        /// Gets the details about the picked video.
        /// </summary>
        [JsonProperty("video")]
        public DreamBrokerVideoDetails Video { get; }

        /// <summary>
        /// Gets embed information for the video.
        /// </summary>
        [JsonProperty("embed")]
        public DreamBrokerEmbed Embed { get; }

        IVideoDetails IVideoValue.Video => Video;

        IVideoEmbed IVideoValue.Embed => Embed;

        #endregion

        #region Constructors

        private DreamBrokerValue(JObject json) {
            Source = json.GetString("source") ?? json.GetString("url");
            Video = json.GetObject("video", DreamBrokerVideoDetails.Parse);
            Embed = new DreamBrokerEmbed(Video);
        }

        #endregion

        #region Static methods

        internal static DreamBrokerValue Parse(JObject json) {
            return json == null ? null : new DreamBrokerValue(json);
        }

        #endregion

    }

}