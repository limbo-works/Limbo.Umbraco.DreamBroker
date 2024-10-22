using Limbo.Umbraco.DreamBroker.Models.Videos.Intermediary;
using Limbo.Umbraco.DreamBroker.PropertyEditors;
using Limbo.Umbraco.DreamBroker.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.DreamBroker;

/// <summary>
/// Static class with various utility methods used through the package.
/// </summary>
public static class DreamBrokerUtils {

    /// <summary>
    /// Returns an instance of <see cref="DreamBrokerIntermediaryVideoValue"/> representing the property value as saved by <see cref="DreamBrokerVideoEditor"/>.
    /// </summary>
    /// <param name="source">The Dream Broker video URL.</param>
    /// <returns>An instance of <see cref="DreamBrokerIntermediaryVideoValue"/> representing the video.</returns>
    public static DreamBrokerIntermediaryVideoValue? GetDreamBrokerVideoValueFromSource(string? source) {
        return StaticServiceProvider.Instance
            .GetRequiredService<DreamBrokerService>()
            .GetIntermediaryVideoValueFromSource(source);
    }

}