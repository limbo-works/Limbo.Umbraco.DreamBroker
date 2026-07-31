// Replaces Views/VideoOverlay.html + Scripts/Controllers/VideoOverlay.js.
//
// Unlike the old overlay - which fetched every video of every channel once and filtered client side - the search is
// handed to the "videos" endpoint, which is what the old (unused) "text" parameter was always meant for.
import { html, css, when, repeat } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";

import "@limbo/video/elements/duration";
import { DreamBrokerService } from "@limbo/dreambroker/service";

export class LimboDreamBrokerSelectVideoModalElement extends UmbModalBaseElement {

    static properties = {
        ...UmbModalBaseElement.properties,
        _loading: { state: true },
        _search: { state: true },
        _videos: { state: true },
        _channelCount: { state: true },
        _selected: { state: true }
    };

    #searchTimer = 0;
    #requestToken = 0;

    constructor() {
        super();
        this._loading = true;
        this._search = "";
        this._videos = [];
        this._channelCount = 0;
        this._selected = null;
    }

    connectedCallback() {
        super.connectedCallback();
        this.#loadVideos();
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        window.clearTimeout(this.#searchTimer);
    }

    async #loadVideos() {

        const requestId = ++this.#requestToken;

        this._loading = true;

        try {

            const result = await DreamBrokerService.getVideos(this._search);

            // Ignore responses that have been superseded by a newer search
            if (requestId !== this.#requestToken) return;

            const channels = result?.channels ?? [];

            this._channelCount = channels.length;

            // Flatten the per-channel lists into one list, remembering which channel each video came from
            this._videos = channels
                .flatMap((channel) => (channel.videos ?? []).map((video) => ({ ...video, channelName: channel.name })))
                .sort((a, b) => (a.title ?? "").localeCompare(b.title ?? ""));

        } catch {

            if (requestId !== this.#requestToken) return;

            this._videos = [];

        } finally {

            if (requestId === this.#requestToken) {
                this._loading = false;
            }

        }

    }

    #onSearch(event) {
        this._search = event.target.value ?? "";
        window.clearTimeout(this.#searchTimer);
        this.#searchTimer = window.setTimeout(() => this.#loadVideos(), 300);
    }

    #select(video, submit = false) {
        this._selected = video;
        if (submit) this.#submit();
    }

    #submit() {
        if (!this._selected) return;
        this.value = this._selected;
        this.modalContext?.submit();
    }

    #renderVideos() {

        if (this._loading) return html``;

        if (this._channelCount === 0) {
            return html`<uui-box><p>${this.localize.term("limboDreamBroker_noChannels")}</p></uui-box>`;
        }

        if (this._videos.length === 0) {
            return html`<uui-box><p>${this.localize.term("limboDreamBroker_noVideos")}</p></uui-box>`;
        }

        return html`
            <div class="grid">
                ${repeat(this._videos, (video) => `${video.channelId}/${video.videoId}`, (video) => html`
                    <uui-card
                        class="video-card"
                        selectable
                        .selected=${this._selected === video}
                        @click=${() => this.#select(video)}
                        @dblclick=${() => this.#select(video, true)}>
                        <div>
                            <img
                                loading="lazy"
                                src=${DreamBrokerService.getThumbnail(video.channelId, video.videoId, 406, 228).url}
                                alt=${video.title ?? ""} />
                            <div class="video-card-text">
                                <div class="video-card-title">${video.title}</div>
                                <div class="video-card-details">
                                    <limbo-video-duration .value=${video.duration}></limbo-video-duration>
                                </div>
                                <div class="video-card-channel">${video.channelName}</div>
                            </div>
                        </div>
                    </uui-card>
                `)}
            </div>
        `;

    }

    render() {
        return html`
            <umb-body-layout headline=${this.localize.term("limboDreamBroker_select")}>
                ${when(this._loading, () => html`<uui-loader-bar></uui-loader-bar>`)}
                <div slot="action-menu">
                    <uui-input
                        class="search"
                        placeholder=${this.localize.term("limboDreamBroker_searchPlaceholder")}
                        .value=${this._search}
                        @input=${this.#onSearch}></uui-input>
                </div>
                <div class="content">
                    ${this.#renderVideos()}
                </div>
                <div slot="actions">
                    <uui-button
                        label=${this.localize.term("general_close")}
                        @click=${() => this.modalContext?.reject()}></uui-button>
                    ${when(this._selected, () => html`
                        <uui-button
                            color="positive"
                            look="primary"
                            label=${this.localize.term("general_choose")}
                            @click=${() => this.#submit()}></uui-button>
                    `)}
                </div>
            </umb-body-layout>
        `;
    }

    static styles = css`

        uui-loader-bar {
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            z-index: 9001;
        }

        div[slot="action-menu"] {
            padding-right: 20px;
            display: flex;
            gap: 10px;
        }

        .search {
            flex: 1;
            width: 300px;
        }

        .grid {
            height: 100%;
            padding: 5px;
            margin: 0 -5px;
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(230px, 1fr));
            gap: var(--uui-size-space-5);
            align-content: start;
        }

        .video-card {
            cursor: pointer;
        }

        .video-card img {
            width: 100%;
            display: block;
            aspect-ratio: 16 / 9;
            object-fit: cover;
            background: var(--uui-color-surface-alt);
        }

        .video-card-text {
            padding: 2px 7px 7px 7px;
        }

        .video-card-title {
            font-weight: bold;
        }

        .video-card-details {
            margin-top: 2px;
            font-size: 12px;
        }

        .video-card-channel {
            margin-top: 5px;
            font-size: 12px;
            line-height: 16px;
            color: var(--uui-color-text-alt);
        }

    `;

}

customElements.define("limbo-dreambroker-select-video", LimboDreamBrokerSelectVideoModalElement);

export default LimboDreamBrokerSelectVideoModalElement;
