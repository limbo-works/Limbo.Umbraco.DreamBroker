﻿using Microsoft.AspNetCore.Html;

namespace Limbo.Umbraco.DreamBroker.Models {
    
    /// <summary>
    /// Class representing the embed options of the video.
    /// </summary>
    public class DreamBrokerEmbed {

        #region Properties

        /// <summary>
        /// Gets the HTML embed code.
        /// </summary>
        public HtmlString Html { get; }

        #endregion

        #region Constructors

        internal DreamBrokerEmbed(DreamBrokerVideoDetails video) {
            Html = new HtmlString($"<iframe frameborder=\"0\" width=\"854\" height=\"480\" allowfullscreen webkitallowfullscreen mozallowfullscreen  src=\"https://dreambroker.com/channel/{video.ChannelId}/iframe/{video.ChannelId}\"></iframe>");
        }

        #endregion

    }

}