// [CHANGE: Umbraco 17 upgrade - the [DataEditor] attribute no longer carries a name, view, icon or group. Those are
// now declared client side by the "propertyEditorSchema"/"propertyEditorUi" manifests, and IEditorConfigurationParser
// was removed] Related: wwwroot/EntryPoint.js, PropertyEditors/DreamBrokerVideoConfigurationEditor.cs

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable 1591

namespace Limbo.Umbraco.DreamBroker.PropertyEditors;

/// <summary>
/// Represents the DreamBroker video property editor.
/// </summary>
[DataEditor(EditorAlias, ValueType = ValueTypes.Json)]
public class DreamBrokerVideoEditor : DataEditor {

    private readonly IIOHelper _ioHelper;

    #region Constants

    public const string EditorAlias = "Limbo.Umbraco.DreamBroker";

    public const string EditorName = "Limbo DreamBroker Video";

    /// <summary>
    /// Gets the alias of the property editor UI registered for this editor.
    /// </summary>
    public const string EditorUiAlias = "Limbo.Umbraco.DreamBroker.Video.Ui";

    public const string EditorIcon = "limbo-dreambroker-alt";

    #endregion

    #region Constructors

    public DreamBrokerVideoEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper) : base(dataValueEditorFactory) {
        _ioHelper = ioHelper;
    }

    #endregion

    #region Member methods

    protected override IConfigurationEditor CreateConfigurationEditor() {
        return new DreamBrokerVideoConfigurationEditor(_ioHelper);
    }

    #endregion

}
