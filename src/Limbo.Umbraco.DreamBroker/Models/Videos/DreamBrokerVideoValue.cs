using System.Diagnostics.CodeAnalysis;
using Limbo.Umbraco.DreamBroker.PropertyEditors;
using Limbo.Umbraco.Video.Models.Providers;
using Limbo.Umbraco.Video.Models.Videos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Limbo.Umbraco.DreamBroker.Models.Videos;

/// <summary>
/// Class representing the value of a <see cref="DreamBrokerVideoEditor"/> property editor.
/// </summary>
public class DreamBrokerVideoValue : IVideoValue {

    #region Properties

    /// <summary>
    /// Gets the source (URL) as entered by the user.
    /// </summary>
    [JsonProperty("source")]
    public string Source { get; }

    /// <summary>
    /// Gets information about the video provider.
    /// </summary>
    [JsonProperty("provider")]
    public DreamBrokerVideoProvider Provider { get; }

    /// <summary>
    /// Gets the details about the picked video.
    /// </summary>
    [JsonProperty("details")]
    public DreamBrokerVideoDetails Details { get; }

    /// <summary>
    /// Gets embed information for the video.
    /// </summary>
    [JsonProperty("embed")]
    public DreamBrokerEmbed Embed { get; }

    IVideoProvider IVideoValue.Provider => Provider;

    IVideoDetails IVideoValue.Details => Details;

    IVideoEmbed IVideoValue.Embed => Embed;

    #endregion

    #region Constructors

    private DreamBrokerVideoValue(JObject json) {
        Source = json.GetString("source") ?? json.GetString("url")!;
        Provider = DreamBrokerVideoProvider.Default;
        Details = json.GetObject("details", DreamBrokerVideoDetails.Parse) ?? json.GetObject("video", DreamBrokerVideoDetails.Parse)!;
        Embed = new DreamBrokerEmbed(Details);
    }

    #endregion

    #region Static methods

    [return: NotNullIfNotNull(nameof(json))]
    internal static DreamBrokerVideoValue? Parse(JObject? json) {
        return json == null ? null : new DreamBrokerVideoValue(json);
    }

    #endregion

}