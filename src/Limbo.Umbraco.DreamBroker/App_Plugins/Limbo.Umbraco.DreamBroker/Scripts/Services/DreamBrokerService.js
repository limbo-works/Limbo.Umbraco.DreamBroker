angular.module("umbraco.services").factory("dreamBrokerService", function ($http, editorService, overlayService) {

    // Get relevant settings from Umbraco's server variables
    const cacheBuster = Umbraco.Sys.ServerVariables.application.cacheBuster;
    const umbracoPath = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath;

    // Fetches information about the video from our underlying API
    function getVideo(channelId, videoId) {
        return $http.get(`${umbracoPath}/backoffice/Limbo/DreamBroker/GetVideo?channelId=${channelId}&videoId=${videoId}`);
    }

    // Fetches a list of known videos from our underlying API
    function getVideos() {
        return $http.get(`${umbracoPath}/backoffice/Limbo/DreamBroker/GetVideos`);
    }

    // Returns the URL of the video
    function getVideoUrl(channelId, videoId) {
        if (!channelId || !videoId) return null;
        return `https://www.dreambroker.com/channel/${channelId}/${videoId}`;
    }

    // Returns a thumbnail object for the video
    function getThumbnail(channelId, videoId) {
        if (!channelId || !videoId) return null;
        const width = 203;
        const height = 114;
        return {
            url: `https://dreambroker.com/channel/${channelId}/${videoId}/get/poster/${width}x${height}.jpg?crop=true`,
            width: width,
            height: height
        };
    }

    // Opens a new overlay where the editor can search and pick videos
    function openAddVideo(options) {

        if (typeof options === "function") {
            const callback = options;
            options = {
                submit: function (video) {
                    callback(video);
                    editorService.close();
                }
            };
        }

        if (!options) options = {};
        if (!options.title) options.title = "Vælg video";
        if (!options.view) options.view = `/App_Plugins/Limbo.Umbraco.DreamBroker/Views/VideoOverlay.html?v=${cacheBuster}`;
        if (!options.size) options.size = "medium";
        if (!options.close) options.close = function () { editorService.close(); };

        editorService.open(options);

    }

    function openSuggestChannel(channel, video) {

        const channelTitle = channel.name;
        const videoTitle = video.title;


        var message = "Videoen <strong>" + video.title + "</strong> er en del af kananel <strong>" + channelTitle + "</strong>, som ikke allerede findes i Umbraco.<br /><br />Vil du tilføje kanalen i Umbraco?";



        const options = {
            title: "Tilføj kanal",
            view: `/App_Plugins/Limbo.Umbraco.DreamBroker/Views/SuggestChannelOverlay.html?v=${cacheBuster}`,
            channel: channel,
            video: video,
            message,
            close: function() {
                overlayService.close();
            }
        }

        overlayService.open(options);

    }
    
    return {
        getVideo: getVideo,
        getVideos: getVideos,
        getVideoUrl: getVideoUrl,
        getThumbnail: getThumbnail,
        openAddVideo: openAddVideo,
        openSuggestChannel: openSuggestChannel,
        getDuration: function (seconds) {

            const duration = [];

            const hours = Math.floor(seconds / 60 / 60);
            seconds = seconds - (hours * 60 * 60);

            const minutes = Math.floor(seconds / 60);
            seconds = seconds - (minutes * 60);

            if (hours === 1) {
                duration.push({ value: 1, text: "time", suffix: "t" });
            } else if (hours > 1) {
                duration.push({ value: hours, text: "timer", suffix: "t" });
            }

            if (minutes === 1) {
                duration.push({ value: 1, text: "minut", suffix: "m" });
            } else if (minutes > 1) {
                duration.push({ value: minutes, text: "minutter", suffix: "m" });
            }

            if (seconds === 1) {
                duration.push({ value: 1, text: "sekund", suffix: "s" });
            } else if (seconds > 1) {
                duration.push({ value: Math.floor(seconds), text: "sekunder", suffix: "s" });
            }

            for (let i = 0; i < duration.length - 1; i++) {
                if (i === duration.length - 2) {
                    duration[i].text += " og ";
                    duration[i].suffix += " og ";
                } else {
                    duration[i].text += ",";
                    duration[i].suffix += ",";
                }
            }

            return duration;

        }
    }
});