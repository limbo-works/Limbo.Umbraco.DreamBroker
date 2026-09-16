using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable 1591

namespace Limbo.Umbraco.DreamBroker.PropertyEditors;

/// <summary>
/// Represents the DreamBroker video property editor.
/// </summary>
[DataEditor(EditorAlias, ValueType = EditorValueType)]
public class DreamBrokerVideoPropertyEditor : DataEditor {

    private readonly IIOHelper _ioHelper;

    #region Constants

    public const string EditorName = "Limbo DreamBroker Video";

    public const string EditorAlias = "Limbo.Umbraco.DreamBroker.Video";

    public const string EditorUiAlias = "Limbo.Umbraco.DreamBroker.Video.PropertyEditorUi";

    public const string EditorIcon = "limbo-dreambroker-alt";

    public const string EditorGroup = "Limbo";

    public const string EditorValueType = ValueTypes.Json;

    #endregion

    #region Constructors

    public DreamBrokerVideoPropertyEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper) : base(dataValueEditorFactory) {
        _ioHelper = ioHelper;
    }

    #endregion

    #region Member methods

    protected override IConfigurationEditor CreateConfigurationEditor() {
        return new DreamBrokerVideoConfigurationEditor(_ioHelper);
    }

    #endregion

}
