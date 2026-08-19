// [CHANGE: Umbraco 17 upgrade - ConfigurationEditor<T> now only takes IIOHelper, and configuration fields no longer
// have a server side "View" to rewrite (the editors are declared in the client side schema manifest)]
// Related: PropertyEditors/DreamBrokerVideoEditor.cs, wwwroot/EntryPoint.js

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.DreamBroker.PropertyEditors;

public class DreamBrokerVideoConfigurationEditor : ConfigurationEditor<DreamBrokerVideoConfiguration> {

    public DreamBrokerVideoConfigurationEditor(IIOHelper ioHelper) : base(ioHelper) { }

}
