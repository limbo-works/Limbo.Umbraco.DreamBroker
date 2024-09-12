using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.DreamBroker.PropertyEditors;

public class DreamBrokerVideoConfiguration {

    [ConfigurationField("hideLabel", "Hide label", "boolean", Description = "Select whether the label and description of properties using this data type should be hidden.<br /><br />Hiding the label and description can be useful in some cases - e.g. to give the video picker a bit more horizontal space.")]
    public bool HideLabel { get; set; }

}