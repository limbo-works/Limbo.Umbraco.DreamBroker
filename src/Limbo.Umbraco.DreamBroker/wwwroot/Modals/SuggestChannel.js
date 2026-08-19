// Replaces Views/SuggestChannelOverlay.html + Scripts/Controllers/SuggestChannelOverlay.js.
import { html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";

import { DreamBrokerService } from "@limbo/dreambroker/service";

export class LimboDreamBrokerSuggestChannelModalElement extends UmbModalBaseElement {

    #busy = false;

    get #channel() {
        return this.data?.channel ?? {};
    }

    get #video() {
        return this.data?.video ?? {};
    }

    async #onSubmit() {

        if (this.#busy) return;

        this.#busy = true;
        this.requestUpdate();

        try {
            await DreamBrokerService.addChannel(this.#channel.channelId, this.#channel.name);
            this.value = this.#channel;
            this.modalContext?.submit();
        } catch {
            this.#busy = false;
            this.requestUpdate();
        }

    }

    render() {

        // The title and channel name are scraped from DreamBroker's own pages, so they are never trusted as markup -
        // these are plain text bindings, which Lit escapes.
        return html`
            <umb-body-layout headline=${this.localize.term("limboDreamBroker_suggestChannelHeadline")}>
                <uui-box>
                    <p>${this.localize.term("limboDreamBroker_suggestChannelIntro")}</p>
                    <table>
                        <tr>
                            <th>${this.localize.term("limboDreamBroker_video")}</th>
                            <td>${this.#video.title ?? ""}</td>
                        </tr>
                        <tr>
                            <th>${this.localize.term("limboDreamBroker_channel")}</th>
                            <td>${this.#channel.name ?? ""}</td>
                        </tr>
                    </table>
                    <p>${this.localize.term("limboDreamBroker_suggestChannelQuestion")}</p>
                </uui-box>
                <div slot="actions">
                    <uui-button
                        label=${this.localize.term("general_cancel")}
                        @click=${() => this.modalContext?.reject()}></uui-button>
                    <uui-button
                        color="positive"
                        look="primary"
                        state=${this.#busy ? "waiting" : ""}
                        label=${this.localize.term("limboDreamBroker_add")}
                        @click=${this.#onSubmit}></uui-button>
                </div>
            </umb-body-layout>
        `;

    }

    static styles = css`

        p {
            line-height: 1.5;
            margin: 0 0 var(--uui-size-space-4);
        }

        p:last-of-type {
            margin-bottom: 0;
        }

        table {
            border-collapse: collapse;
            margin-bottom: var(--uui-size-space-4);
        }

        th {
            text-align: left;
            padding: 2px var(--uui-size-space-4) 2px 0;
            color: var(--uui-color-text-alt);
            font-weight: 600;
            white-space: nowrap;
        }

        td {
            padding: 2px 0;
            font-weight: bold;
        }

    `;

}

customElements.define("limbo-dreambroker-suggest-channel", LimboDreamBrokerSuggestChannelModalElement);

export default LimboDreamBrokerSuggestChannelModalElement;
