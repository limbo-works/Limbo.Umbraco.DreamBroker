using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable 1591

namespace Limbo.Umbraco.DreamBroker.PropertyEditors {

    /// <summary>
    /// Represents a block list property editor.
    /// </summary>
    [DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.Json, Group = "Limbo", Icon = EditorIcon)]
    public class DreamBrokerEditor : DataEditor {

        #region Constants

        internal const string EditorAlias = "Limbo.Umbraco.DreamBroker";

        internal const string EditorName = "Limbo DreamBroker Video";

        internal const string EditorView = "/App_Plugins/Limbo.Umbraco.DreamBroker/Views/Video.html";

        internal const string EditorIcon = "icon-video color-limbo";

        #endregion

        #region Constructors

        public DreamBrokerEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

        #endregion

    }

}