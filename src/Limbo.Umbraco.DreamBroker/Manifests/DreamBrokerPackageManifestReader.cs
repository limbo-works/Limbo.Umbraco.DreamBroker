// [CHANGE: Umbraco 17 upgrade - IManifestFilter was removed in Umbraco 14+, so the package manifest is now
// served through IPackageManifestReader. Scripts/Stylesheets/BundleOptions no longer exist; the client side
// assets are exposed as an importmap and a backoffice entry point instead]
// Related: Composers/DreamBrokerComposer.cs, wwwroot/EntryPoint.js, Limbo.Umbraco.DreamBroker.csproj

using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.DreamBroker.Manifests;

public class DreamBrokerPackageManifestReader : IPackageManifestReader {

    public async Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        const string alias = DreamBrokerPackage.Alias;
        string cacheBuster = DreamBrokerPackage.CacheBuster;

        List<PackageManifest> manifests = [
            new() {
                Id = alias,
                Name = DreamBrokerPackage.Name,
                AllowTelemetry = true,
                Version = DreamBrokerPackage.InformationalVersion,
                Extensions = [
                    new {
                        type = "backofficeEntryPoint",
                        alias = $"{alias}.EntryPoint",
                        name = $"{DreamBrokerPackage.Name}: Entry Point",
                        js = $"/App_Plugins/{alias}/EntryPoint.js?v={cacheBuster}"
                    }
                ],
                Importmap = new PackageManifestImportmap {
                    Imports = new Dictionary<string, string> {
                        { "@limbo/dreambroker/auth", $"/App_Plugins/{alias}/Auth.js?v={cacheBuster}" },
                        { "@limbo/dreambroker/package", $"/App_Plugins/{alias}/Package.js?v={cacheBuster}" },
                        { "@limbo/dreambroker/service", $"/App_Plugins/{alias}/Service.js?v={cacheBuster}" },
                        { "@limbo/dreambroker/modals/tokens", $"/App_Plugins/{alias}/Modals/Tokens.js?v={cacheBuster}" },
                        { "@limbo/dreambroker/modals/select-video", $"/App_Plugins/{alias}/Modals/SelectVideo.js?v={cacheBuster}" },
                        { "@limbo/dreambroker/modals/suggest-channel", $"/App_Plugins/{alias}/Modals/SuggestChannel.js?v={cacheBuster}" }
                    }
                }
            }
        ];

        return await Task.FromResult(manifests);

    }

}
