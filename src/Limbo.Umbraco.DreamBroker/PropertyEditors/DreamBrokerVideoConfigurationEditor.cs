﻿using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

#pragma warning disable CS1591

namespace Limbo.Umbraco.DreamBroker.PropertyEditors;

public class DreamBrokerVideoConfigurationEditor : ConfigurationEditor<DreamBrokerVideoConfiguration> {

    public DreamBrokerVideoConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) : base(ioHelper, editorConfigurationParser) {

        foreach (ConfigurationField field in Fields) {

            if (field.View is not null) {
                field.View = field.View
                    .Replace("{version}", DreamBrokerPackage.InformationalVersion)
                    .Replace("{alias}", field.Key);
            }

        }

    }

}