using Limbo.Umbraco.Video.Models.Providers;

namespace Limbo.Umbraco.DreamBroker.Models.Videos {

    /// <summary>
    /// Class with limited information about a video provider.
    /// </summary>
    public class DreamBrokerVideoProvider : VideoProvider {

        /// <summary>
        /// Gets a reference to a <see cref="DreamBrokerVideoProvider"/> instance.
        /// </summary>
        public static readonly DreamBrokerVideoProvider Default = new();

        private DreamBrokerVideoProvider() : base("dreambroker", "DreamBroker") { }

    }

}