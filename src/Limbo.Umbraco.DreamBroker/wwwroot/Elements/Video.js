// Replaces Views/Video.html + Scripts/Controllers/Video.js.
//
// The saved value is unchanged from previous versions of the package: { source, details } - where "details" is the
// video as returned by our API. Keeping that shape means content saved by v13 keeps working, and
// DreamBrokerVideoValue can still parse it server side.
import { html, css, when, nothing } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";
import { UmbFormControlMixin, UMB_VALIDATION_EMPTY_LOCALIZATION_KEY } from "@umbraco-cms/backoffice/validation";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";

import "@limbo/video/elements/duration";
import { DreamBrokerService } from "@limbo/dreambroker/service";
import {
    LIMBO_DREAMBROKER_SELECT_VIDEO_MODAL,
    LIMBO_DREAMBROKER_SUGGEST_CHANNEL_MODAL
} from "@limbo/dreambroker/modals/tokens";

export class LimboDreamBrokerVideoElement extends UmbFormControlMixin(UmbLitElement, undefined) {

    static properties = {
        readonly: { type: Boolean, reflect: true },
        mandatory: { type: Boolean },
        mandatoryMessage: { type: String },
        _loading: { state: true },
        _error: { state: true }
    };

    #value;
    #sourceInput;
    #debounceTimer = 0;
    #requestToken = 0;
    #modalManagerContext;

    get value() {
        return this.#value;
    }

    set value(value) {
        const oldValue = this.#value;
        this.#value = value ?? undefined;
        this.requestUpdate("value", oldValue);
    }

    constructor() {

        super();

        this.readonly = false;
        this.mandatory = false;
        this.mandatoryMessage = UMB_VALIDATION_EMPTY_LOCALIZATION_KEY;
        this._loading = false;
        this._error = "";

        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this.#modalManagerContext = instance;
        });

        // A source on its own isn't a value - the property is only satisfied once a video has been resolved
        this.addValidator(
            "valueMissing",
            () => this.mandatoryMessage,
            () => !!this.mandatory && !this.#details()
        );

    }

    firstUpdated(changedProperties) {
        super.firstUpdated(changedProperties);
        this.#sourceInput = this.renderRoot.querySelector(".source");
        if (this.#sourceInput) this.addFormControlElement(this.#sourceInput);
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        window.clearTimeout(this.#debounceTimer);
    }

    #currentValue() {
        return this.#value && typeof this.#value === "object" ? this.#value : null;
    }

    #sourceValue() {
        return this.#currentValue()?.source ?? "";
    }

    #details() {
        return this.#currentValue()?.details ?? null;
    }

    #commit(value) {
        this.value = value;
        this.dispatchEvent(new UmbChangeEvent());
    }

    #clear() {
        window.clearTimeout(this.#debounceTimer);
        this.#requestToken++;
        this._error = "";
        this._loading = false;
        this.#commit(undefined);
    }

    async #lookup(source) {

        const parsed = DreamBrokerService.parseSource(source);

        if (!parsed) {
            // Keep whatever the user typed, but drop the stale video details
            this._error = this.localize.term("limboDreamBroker_invalidUrl");
            this._loading = false;
            this.#commit({ source });
            return;
        }

        const requestId = ++this.#requestToken;

        this._loading = true;
        this._error = "";

        try {

            const data = await DreamBrokerService.getVideo(parsed.channelId, parsed.videoId);

            // A newer lookup (or a clear) has happened in the meantime
            if (requestId !== this.#requestToken) return;

            this.#commit({ source, details: data.video });

            // The channel isn't in Umbraco yet - offer to add it, like the old SuggestChannelOverlay did
            if (data.channel && !data.channel.exists && data.channel.name) {
                this.#openSuggestChannel(data.channel, data.video);
            }

        } catch (res) {

            if (requestId !== this.#requestToken) return;

            this._error = res?.textContent || res?.data?.detail || this.localize.term("limboDreamBroker_errorGeneric");

        } finally {

            if (requestId === this.#requestToken) {
                this._loading = false;
            }

        }

    }

    #onSourceInput(event) {

        const source = event.target.value ?? "";

        window.clearTimeout(this.#debounceTimer);

        if (!source.trim()) {
            this.#clear();
            return;
        }

        this.#commit({ ...(this.#currentValue() ?? {}), source });

        this._loading = true;
        this._error = "";

        this.#debounceTimer = window.setTimeout(() => this.#lookup(source.trim()), 300);

    }

    #onRefresh() {
        const source = this.#sourceValue().trim();
        if (!source) {
            this.#clear();
            return;
        }
        this.#lookup(source);
    }

    #openSelectVideo() {

        const modal = this.#modalManagerContext?.open(this, LIMBO_DREAMBROKER_SELECT_VIDEO_MODAL);
        if (!modal) return;

        modal.onSubmit().then((video) => {
            this.#requestToken++;
            this._error = "";
            this.#commit({
                source: DreamBrokerService.getVideoUrl(video.channelId, video.videoId),
                details: video
            });
        }, () => {
            // Modal closed by the user
        });

    }

    #openSuggestChannel(channel, video) {
        this.#modalManagerContext?.open(this, LIMBO_DREAMBROKER_SUGGEST_CHANNEL_MODAL, {
            data: { channel, video }
        })?.onSubmit().then(() => {
            // Channel added
        }, () => {
            // Declined by the user
        });
    }

    #renderDetails() {

        const details = this.#details();

        if (!details) return nothing;

        const thumbnail = DreamBrokerService.getThumbnail(details.channelId, details.videoId);
        const url = DreamBrokerService.getVideoUrl(details.channelId, details.videoId);

        return html`
            <div class="block">
                <h5>${this.localize.term("limboDreamBroker_video")}</h5>
                <div class="box card-row">
                    ${when(thumbnail, () => html`
                        <div class="thumbnail">
                            <img src=${thumbnail.url} alt=${details.title ?? ""} />
                        </div>
                    `)}
                    <table>
                        <tr>
                            <th>${this.localize.term("limboDreamBroker_id")}</th>
                            <td><code>${details.videoId}</code></td>
                        </tr>
                        <tr>
                            <th>${this.localize.term("limboDreamBroker_title")}</th>
                            <td>${details.title}</td>
                        </tr>
                        <tr>
                            <th>${this.localize.term("limboDreamBroker_duration")}</th>
                            <td><limbo-video-duration .value=${details.duration}></limbo-video-duration></td>
                        </tr>
                    </table>
                    ${when(url, () => html`
                        <a class="app-url" href=${url} target="_blank" rel="noopener noreferrer">
                            <uui-icon name="icon-out"></uui-icon>
                        </a>
                    `)}
                </div>
            </div>
        `;

    }

    render() {

        const source = this.#sourceValue();

        return html`
            <div class="shell ${this._loading ? "loading" : ""}">
                <div>
                    <div class="editor">
                        <div class="actions">
                            <uui-input
                                class="source"
                                .value=${source}
                                ?readonly=${this.readonly}
                                placeholder=${this.localize.term("limboDreamBroker_urlPlaceholder")}
                                @input=${this.#onSourceInput}></uui-input>
                            <uui-button
                                look="outline"
                                label=${this.localize.term("limboDreamBroker_select")}
                                ?disabled=${this.readonly}
                                @click=${this.#openSelectVideo}>
                                <uui-icon name="icon-search"></uui-icon>
                            </uui-button>
                            <uui-button
                                look="outline"
                                label=${this.localize.term("limboDreamBroker_refresh")}
                                ?disabled=${this.readonly || !source.trim()}
                                @click=${this.#onRefresh}>
                                <uui-icon name="icon-refresh"></uui-icon>
                            </uui-button>
                            <uui-button
                                look="outline"
                                label=${this.localize.term("limboDreamBroker_clear")}
                                ?disabled=${this.readonly || !source.trim()}
                                @click=${this.#clear}>
                                <uui-icon name="icon-trash"></uui-icon>
                            </uui-button>
                        </div>
                    </div>
                    ${when(this._error, () => html`<p class="notice --error">${this._error}</p>`)}
                    ${this.#renderDetails()}
                </div>
                ${this._loading ? html`<uui-loader></uui-loader>` : nothing}
            </div>
        `;

    }

    static styles = css`

        :host {
            display: block;
            position: relative;
        }

        .loading > div {
            opacity: 0.6;
            pointer-events: none;
        }

        .loading uui-loader {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
        }

        .actions {
            display: flex;
            flex-wrap: wrap;
            gap: var(--uui-size-space-3);
            align-items: center;
        }

        .source {
            flex: 1;
            min-width: 260px;
        }

        .notice {
            color: var(--uui-color-text-alt);
            font-size: var(--uui-font-size-1);
        }

        .notice.--error {
            color: var(--uui-color-danger);
        }

        .block {
            margin-top: var(--uui-size-layout-1);
        }

        .block > h5 {
            margin: 0 0 var(--uui-size-space-2);
        }

        .box {
            position: relative;
            padding: var(--uui-size-space-4);
            border: 1px solid var(--uui-color-border);
            border-radius: var(--uui-border-radius);
            background: var(--uui-color-surface-alt);
            max-width: 800px;
        }

        .card-row {
            display: flex;
            gap: var(--uui-size-space-4);
            align-items: flex-start;
        }

        .thumbnail {
            flex: 0 0 203px;
            max-width: 203px;
            aspect-ratio: 16 / 9;
            border-radius: var(--uui-border-radius);
            overflow: hidden;
            background: var(--uui-color-surface);
        }

        .thumbnail img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            display: block;
        }

        table {
            border-collapse: collapse;
        }

        th {
            text-align: left;
            padding: 2px var(--uui-size-space-4) 2px 0;
            color: var(--uui-color-text-alt);
            font-weight: 600;
            vertical-align: top;
            white-space: nowrap;
        }

        td {
            padding: 2px 0;
        }

        .app-url {
            position: absolute;
            top: var(--uui-size-space-3);
            right: var(--uui-size-space-3);
            color: var(--uui-color-text-alt);
        }

    `;

}

customElements.define("limbo-dreambroker-video", LimboDreamBrokerVideoElement);

export { LimboDreamBrokerVideoElement as element };
export default LimboDreamBrokerVideoElement;
