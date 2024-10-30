using Limbo.Umbraco.DreamBroker.Models.Videos.Intermediary;
using Limbo.Umbraco.DreamBroker.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.DreamBroker;

/// <summary>
/// Static class with various utility methods for Dream Broker implementation.
/// </summary>
public static class DreamBrokerUtils {

    /// <summary>
    /// Attempts to look up the video identified by the specified <paramref name="source"/>, and return an instance of <see cref="DreamBrokerIntermediaryVideoValue"/> if successful. When serialize to JSON, the value equals the property value saved in the database for properties using the Dream Broker video data type.
    /// </summary>
    /// <param name="source">The source (URL) as entered by the user.</param>
    /// <returns>An instance of <see cref="DreamBrokerIntermediaryVideoValue"/> if successful; otherwise, <see langword="null"/>.</returns>
    public static DreamBrokerIntermediaryVideoValue? GetDreamBrokerVideoValueFromSource(string? source) {
        return StaticServiceProvider.Instance
            .GetRequiredService<DreamBrokerService>()
            .GetIntermediaryVideoValueFromSource(source);
    }

}