using System.Diagnostics.CodeAnalysis;
using Limbo.Umbraco.DreamBroker.PropertyEditors;
using Limbo.Umbraco.Video.Models.Providers;
using Limbo.Umbraco.Video.Models.Videos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft.Extensions;

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

    private DreamBrokerVideoValue(JObject json, DreamBrokerVideoDetails details) {
        Source = json.GetString("source") ?? json.GetString("url") ?? details.Url;
        Provider = DreamBrokerVideoProvider.Default;
        Details = details;
        Embed = new DreamBrokerEmbed(details);
    }

    #endregion

    #region Static methods

    internal static DreamBrokerVideoValue? Parse(JObject? json) {

        if (json is null) return null;

        // The saved value may contain just a "source" - e.g. if an editor typed a URL that never resolved to a video,
        // and then saved. There is no video to describe in that case, so there is no value either. Without this the
        // details would be null and DreamBrokerEmbed would throw while rendering.
        DreamBrokerVideoDetails? details = json.GetObject("details", DreamBrokerVideoDetails.Parse)
                                           ?? json.GetObject("video", DreamBrokerVideoDetails.Parse);

        return details is null ? null : new DreamBrokerVideoValue(json, details);

    }

    #endregion

}