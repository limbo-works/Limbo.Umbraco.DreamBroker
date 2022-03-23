using Limbo.Umbraco.Video.Models.Videos;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;

namespace Limbo.Umbraco.DreamBroker.Models.Videos {
    
    /// <summary>
    /// Class representing the embed options of the video.
    /// </summary>
    public class DreamBrokerEmbed : IVideoEmbed {

        #region Properties

        /// <summary>
        /// Gets the embed URL.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; }

        /// <summary>
        /// Gets the HTML embed code.
        /// </summary>
        public HtmlString Html { get; }

        #endregion

        #region Constructors

        internal DreamBrokerEmbed(DreamBrokerVideoDetails video) {
            Url = $"https://dreambroker.com/channel/{video.ChannelId}/iframe/{video.VideoId}";
            Html = new HtmlString($"<iframe frameborder=\"0\" width=\"854\" height=\"480\" allowfullscreen webkitallowfullscreen mozallowfullscreen src=\"{Url}\"></iframe>");
        }

        #endregion

    }

}