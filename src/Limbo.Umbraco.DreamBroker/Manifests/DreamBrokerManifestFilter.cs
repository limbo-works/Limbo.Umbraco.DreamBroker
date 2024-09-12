using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.DreamBroker.Manifests;

/// <inheritdoc />
public class DreamBrokerManifestFilter : IManifestFilter {

    /// <inheritdoc />
    public void Filter(List<PackageManifest> manifests) {

        // Initialize a new manifest filter for this package
        PackageManifest manifest = new() {
            AllowPackageTelemetry = true,
            PackageId = DreamBrokerPackage.Alias,
            PackageName = DreamBrokerPackage.Name,
            Version = DreamBrokerPackage.InformationalVersion,
            BundleOptions = BundleOptions.Independent,
            Scripts = [
                $"/App_Plugins/{DreamBrokerPackage.Alias}/Scripts/Services/DreamBrokerService.js",
                $"/App_Plugins/{DreamBrokerPackage.Alias}/Scripts/Controllers/Video.js",
                $"/App_Plugins/{DreamBrokerPackage.Alias}/Scripts/Controllers/VideoOverlay.js"
            ],
            Stylesheets = [
                $"/App_Plugins/{DreamBrokerPackage.Alias}/Styles/Default.css"
            ]
        };

        // Append the manifest
        manifests.Add(manifest);

    }

}