using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.DreamBroker.Manifests {

    /// <inheritdoc />
    public class DreamBrokerManifestFilter : IManifestFilter {

        /// <inheritdoc />
        public void Filter(List<PackageManifest> manifests) {
            manifests.Add(new PackageManifest {
                AllowPackageTelemetry = true,
                PackageName = DreamBrokerPackage.Name,
                Version = DreamBrokerPackage.InformationalVersion,
                BundleOptions = BundleOptions.Independent,
                Scripts = new[] {
                    $"/App_Plugins/{DreamBrokerPackage.Alias}/Scripts/Services/DreamBrokerService.js",
                    $"/App_Plugins/{DreamBrokerPackage.Alias}/Scripts/Controllers/Video.js",
                    $"/App_Plugins/{DreamBrokerPackage.Alias}/Scripts/Controllers/VideoOverlay.js"
                },
                Stylesheets = new [] {
                    $"/App_Plugins/{DreamBrokerPackage.Alias}/Styles/Default.css"
                }
            });
        }

    }

}