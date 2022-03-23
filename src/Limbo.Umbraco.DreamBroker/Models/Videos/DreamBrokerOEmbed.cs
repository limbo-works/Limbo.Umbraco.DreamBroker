using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Limbo.Umbraco.DreamBroker.Models.Videos {
    
    /// <summary>
    /// Class representing the OEmbed value returned by the DreamBroker API.
    /// </summary>
    public class DreamBrokerOEmbed {

        #region Properties

        /// <summary>
        /// Gets the title of the video.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the HTML embed code of the video.
        /// </summary>
        public string Html { get; }

        /// <summary>
        /// Gets the thumbnail URL of the video.
        /// </summary>
        public string ThumbnailUrl { get; }

        /// <summary>
        /// Gets the width of the video's thumbnail.
        /// </summary>
        public int ThumbnailWidth { get; }
        
        /// <summary>
        /// Gets the height of the video's thumbnail.
        /// </summary>
        public int ThumbnailHeight { get; }

        #endregion

        #region Constructors

        private DreamBrokerOEmbed(JObject obj) {
            Title = obj.GetString("title");
            Html = obj.GetString("html");
            ThumbnailUrl = obj.GetString("thumbnail_url");
            ThumbnailWidth = obj.GetInt32("thumbnail_width");
            ThumbnailHeight = obj.GetInt32("thumbnail_height");
        }

        #endregion

        #region Static methods

        internal static DreamBrokerOEmbed Parse(JObject json) {
            return json == null ? null : new DreamBrokerOEmbed(json);
        }

        #endregion

    }

}