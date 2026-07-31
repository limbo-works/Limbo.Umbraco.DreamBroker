// [CHANGE: Umbraco 17 upgrade - "builder.ManifestFilters()" no longer exists; the manifest is registered as an
// IPackageManifestReader, and the Management API needs its Swagger document configured]
// Related: Manifests/DreamBrokerPackageManifestReader.cs, Api/DreamBrokerSwaggerGenOptions.cs, Controllers/DreamBrokerController.cs

using Limbo.Umbraco.DreamBroker.Api;
using Limbo.Umbraco.DreamBroker.Manifests;
using Limbo.Umbraco.DreamBroker.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable 1591

namespace Limbo.Umbraco.DreamBroker.Composers;

public class DreamBrokerComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {
        builder.Services.AddTransient<DreamBrokerService>();
        builder.Services.AddSingleton<IPackageManifestReader, DreamBrokerPackageManifestReader>();
        builder.Services.ConfigureOptions<DreamBrokerSwaggerGenOptions>();
    }

}
