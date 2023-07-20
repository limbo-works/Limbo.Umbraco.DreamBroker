using System.Collections.Generic;
using System.Reflection;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.DreamBroker.Manifests {

    /// <inheritdoc />
    public class DreamBrokerManifestFilter : IManifestFilter {

        /// <inheritdoc />
        public void Filter(List<PackageManifest> manifests) {

            // Initialize a new manifest filter for this package
            PackageManifest manifest = new() {
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
            };

            // The "PackageId" property isn't available prior to Umbraco 12, and since the package is build against
            // Umbraco 10, we need to use reflection for setting the property value for Umbraco 12+. Ideally this
            // shouldn't fail, but we might at least add a try/catch to be sure
            try {
                PropertyInfo? property = manifest.GetType().GetProperty("PackageId");
                property?.SetValue(manifest, DreamBrokerPackage.Alias);
            } catch {
                // We don't really care about the exception
            }

            // Append the manifest
            manifests.Add(manifest);

        }

    }

}