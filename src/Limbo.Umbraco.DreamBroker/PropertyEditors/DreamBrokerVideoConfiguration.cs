// [CHANGE: Umbraco 17 upgrade - [ConfigurationField] only takes the alias now; the label, description and editor for
// each field live in the "settings" of the client side propertyEditorSchema manifest]
// Related: wwwroot/EntryPoint.js, PropertyEditors/DreamBrokerVideoConfigurationEditor.cs

using System;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.DreamBroker.PropertyEditors;

public class DreamBrokerVideoConfiguration {

    /// <summary>
    /// Gets or sets whether the label and description of properties using this data type should be hidden.
    /// </summary>
    /// <remarks>
    /// The new backoffice has no equivalent of the old <c>hideLabel</c> flag - a property editor UI can no longer
    /// hide the label of the property it is rendered in. The property is kept so configuration saved by earlier
    /// versions of the package still deserializes, but it is no longer editable and no longer has any effect.
    /// </remarks>
    [Obsolete("Umbraco 17 does not allow a property editor UI to hide its label, so this setting no longer has any effect.")]
    [ConfigurationField("hideLabel")]
    public bool HideLabel { get; set; }

}
