import { UmbModalToken } from "@umbraco-cms/backoffice/modal";

// Replaces the AngularJS "editorService"/"overlayService" views.
export const LIMBO_DREAMBROKER_SELECT_VIDEO_MODAL = new UmbModalToken("Limbo.Umbraco.DreamBroker.SelectVideoModal", {
    modal: {
        type: "sidebar",
        size: "medium"
    }
});

export const LIMBO_DREAMBROKER_SUGGEST_CHANNEL_MODAL = new UmbModalToken("Limbo.Umbraco.DreamBroker.SuggestChannelModal", {
    modal: {
        type: "dialog",
        size: "small"
    }
});
