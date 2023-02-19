using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Time;

namespace Limbo.Umbraco.DreamBroker.Models.Channels {

    /// <summary>
    /// Class representing a DreamBroker channel as saved in Umbraco.
    /// </summary>
    public class DreamBrokerChannel {

        #region Properties

        /// <summary>
        /// Gets the GUID of the channel.
        /// </summary>
        [JsonProperty("key")]
        public Guid Key { get; internal set; }

        /// <summary>
        /// Gets the friendly name of the channel.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the DreamBroker ID of the channel.
        /// </summary>
        [JsonProperty("channelId")]
        public string ChannelId { get; internal set; }

        /// <summary>
        /// Gets a timestamp for when the channel was added to Umbraco.
        /// </summary>
        [JsonProperty("createDate")]
        public EssentialsTime CreateDate { get; internal set; }

        /// <summary>
        /// Gets a timestamp for when the channel was last updated in Umbraco.
        /// </summary>
        [JsonProperty("updateDate")]
        public EssentialsTime UpdateDate { get; internal set; }

        #endregion

        #region Constructors

        internal DreamBrokerChannel() { }

        private DreamBrokerChannel(JObject json) {
            Key = json.GetGuid("key");
            Name = json.GetString("name");
            ChannelId = json.GetString("channelId");
            CreateDate = json.GetString("createDate", EssentialsTime.FromIso8601);
            UpdateDate = json.GetString("updateDate", EssentialsTime.FromIso8601);
        }

        #endregion

        #region Static methods

        internal static DreamBrokerChannel Parse(JObject json) {
            return json == null ? null : new DreamBrokerChannel(json);
        }

        #endregion

    }

}