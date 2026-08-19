// Registers everything this package contributes to the backoffice. In Umbraco 13 this was an IManifestFilter that
// pushed AngularJS scripts and a stylesheet; from Umbraco 14 onwards extensions are registered from the client.
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";

import { DreamBrokerAuth } from "@limbo/dreambroker/auth";
import { DreamBrokerPackage } from "@limbo/dreambroker/package";
import { DreamBrokerService } from "@limbo/dreambroker/service";

const ALIAS = "Limbo.Umbraco.DreamBroker";

// Matches DreamBrokerVideoEditor.EditorAlias
const SCHEMA_ALIAS = "Limbo.Umbraco.DreamBroker.Video";

// Matches DreamBrokerVideoEditor.EditorUiAlias
const UI_ALIAS = "Limbo.Umbraco.DreamBroker.Video.Ui";

// The manifest stamps "?v=<cacheBuster>" onto this module's own URL, so we can read the cache buster straight off it
// instead of waiting for a round-trip to the server before registering anything.
const cacheBuster = new URL(import.meta.url).searchParams.get("v") ?? "";

function registerExtensions(extensionRegistry) {

    extensionRegistry.register({
        type: "localization",
        alias: `${ALIAS}.Localization.EnUs`,
        name: "English (US)",
        js: () => import(`./Localization/en-US.js?v=${cacheBuster}`),
        meta: { culture: "en" }
    });

    extensionRegistry.register({
        type: "localization",
        alias: `${ALIAS}.Localization.DaDk`,
        name: "Danish",
        js: () => import(`./Localization/da-DK.js?v=${cacheBuster}`),
        meta: { culture: "da" }
    });

    extensionRegistry.register({
        type: "icons",
        alias: `${ALIAS}.Icons`,
        name: "Limbo DreamBroker Icons",
        js: `/App_Plugins/${ALIAS}/Icons.js?v=${cacheBuster}`
    });

    // The schema replaces the metadata that used to sit on the [DataEditor] attribute server side. There are no
    // configurable settings: the old "hideLabel" option has no equivalent in the new backoffice.
    extensionRegistry.register({
        type: "propertyEditorSchema",
        alias: SCHEMA_ALIAS,
        name: "Limbo DreamBroker Video",
        meta: {
            defaultPropertyEditorUiAlias: UI_ALIAS,
            settings: {
                properties: [
                    {
                        alias: "hideLabel",
                        label: "Hide label?",
                        description: "Should the label in the property editor be hidden?",
                        propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
                    }
                ]
            }
        }
    });

    extensionRegistry.register({
        type: "propertyEditorUi",
        alias: UI_ALIAS,
        name: "Limbo DreamBroker Video Property Editor UI",
        js: () => import(`./Elements/Video.js?v=${cacheBuster}`),
        elementName: "limbo-dreambroker-video",
        meta: {
            label: "Limbo DreamBroker Video",
            propertyEditorSchemaAlias: SCHEMA_ALIAS,
            icon: "limbo-dreambroker-alt",
            group: "Limbo",
            supportsReadOnly: true
        }
    });

    extensionRegistry.register({
        type: "modal",
        alias: `${ALIAS}.SelectVideoModal`,
        name: "Limbo DreamBroker Select Video Modal",
        element: `/App_Plugins/${ALIAS}/Modals/SelectVideo.js?v=${cacheBuster}`
    });

    extensionRegistry.register({
        type: "modal",
        alias: `${ALIAS}.SuggestChannelModal`,
        name: "Limbo DreamBroker Suggest Channel Modal",
        element: `/App_Plugins/${ALIAS}/Modals/SuggestChannel.js?v=${cacheBuster}`
    });

}

export const onInit = (host, extensionRegistry) => {

    // Register up front. If we waited for a network round-trip here, a content node opened on a hard refresh could
    // resolve its property editor UI before we got there, and umb-property permanently swaps to the
    // "missing property editor" UI once it fails to find the alias.
    registerExtensions(extensionRegistry);

    let configured = false;

    host.consumeContext(UMB_AUTH_CONTEXT, (authContext) => {

        // consumeContext fires again if the context is re-provided
        if (!authContext || configured) return;
        configured = true;

        DreamBrokerAuth.setConfiguration(authContext.getOpenApiConfiguration());

        // Purely informational - nothing above depends on it
        DreamBrokerService.getServerVariables().then((serverVariables) => {
            DreamBrokerPackage.serverVariables = serverVariables;
        }, () => {
            // Ignore - the package still works without it
        });

    });

};
