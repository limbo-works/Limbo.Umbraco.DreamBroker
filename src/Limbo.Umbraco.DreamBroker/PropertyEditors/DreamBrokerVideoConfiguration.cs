using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.DreamBroker.PropertyEditors;

public class DreamBrokerVideoConfiguration {

    /// <summary>
    /// Gets or sets whether the label and description of properties using this data type should be hidden.
    /// </summary>
    [ConfigurationField("hideLabel")]
    public bool HideLabel { get; set; }

}