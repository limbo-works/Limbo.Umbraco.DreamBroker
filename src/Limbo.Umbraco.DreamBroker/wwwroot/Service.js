// Replaces the AngularJS "dreamBrokerService" factory. The endpoints now live on the versioned management API, so
// every request needs the backoffice bearer token.
import { DreamBrokerAuth } from "@limbo/dreambroker/auth";

const path = "/umbraco/management/api/v1/limbo/dreambroker";

async function request(url, init) {

    const auth = await DreamBrokerAuth.configuration();

    const config = { method: "GET", ...init };

    // The session is carried by an httpOnly cookie, so "credentials" is what actually authenticates the request
    config.credentials = auth?.credentials ?? "include";
    config.headers = {
        ...(init?.headers ?? {}),
        Authorization: `Bearer ${auth?.token ? await auth.token() : ""}`
    };

    const res = await fetch(`${auth?.base ?? ""}${url}`, config);

    const contentType = res.headers.get("content-type") || "";

    // Errors come back as "application/problem+json", so don't test for "application/json" specifically
    if (contentType.includes("json")) {
        res.data = await res.json();
    } else if (contentType.startsWith("text/")) {
        res.textContent = await res.text();
    }

    if (!res.ok) throw res;

    return res;

}

export class DreamBrokerService {

    static async getServerVariables() {
        const res = await request(`${path}/serverVariables`);
        return res.data;
    }

    /** Returns the DreamBroker channels that have been added to Umbraco. */
    static async getChannels() {
        const res = await request(`${path}/channels`);
        return res.data;
    }

    /** Adds a channel to Umbraco. */
    static async addChannel(channelId, name) {
        const query = new URLSearchParams({ channelId, name });
        const res = await request(`${path}/channels?${query}`, { method: "POST" });
        return res.data;
    }

    /** Removes a channel from Umbraco. */
    static async deleteChannel(channelId) {
        return await request(`${path}/channels/${encodeURIComponent(channelId)}`, { method: "DELETE" });
    }

    /** Returns the videos of all channels added to Umbraco, optionally filtered by title or ID. */
    static async getVideos(text) {
        const query = new URLSearchParams();
        if (text) query.set("text", text);
        const res = await request(`${path}/videos?${query}`);
        return res.data;
    }

    /** Returns a single video, along with details about its channel. */
    static async getVideo(channelId, videoId) {
        const query = new URLSearchParams({ channelId, videoId });
        const res = await request(`${path}/video?${query}`);
        return res.data;
    }

    /** Returns the URL of the video on dreambroker.com. */
    static getVideoUrl(channelId, videoId) {
        if (!channelId || !videoId) return null;
        return `https://www.dreambroker.com/channel/${channelId}/${videoId}`;
    }

    /** Returns a thumbnail object for the video. */
    static getThumbnail(channelId, videoId, width = 203, height = 114) {
        if (!channelId || !videoId) return null;
        return {
            url: `https://dreambroker.com/channel/${channelId}/${videoId}/get/poster/${width}x${height}.jpg?crop=true`,
            width,
            height
        };
    }

    /** Parses a DreamBroker video URL into its channel and video IDs. */
    static parseSource(source) {
        if (!source) return null;
        const trimmed = source.trim().replace(/\/$/, "");
        const m = trimmed.match(/^https:\/\/(?:www\.)?dreambroker\.com\/channel\/([a-z0-9]+)\/([a-z0-9]+)/i);
        return m ? { channelId: m[1], videoId: m[2] } : null;
    }

}

export default DreamBrokerService;
