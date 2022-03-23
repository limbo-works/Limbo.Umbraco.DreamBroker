angular.module("umbraco").controller("Limbo.Umbraco.DreamBroker.Video", function ($scope, dreamBrokerService) {

    const vm = this;

    if (!$scope.model.value) {
        $scope.model.value = { url: "" };
    }

    // Gets information about the video of the entered URL
    vm.getVideo = function() {

        const url = $scope.model.value.url.trim().replace(/\/$/, "");

        const m = url.match(/^https:\/\/(www\.|)dreambroker\.com\/channel\/([a-z0-9]+)\/([a-z0-9]+)/);

        if (m) {

            vm.loading = true;

            dreamBrokerService.getVideo(m[2], m[3]).then(function (res) {

                const channel = res.data.channel;
                const video = res.data.video;

                if (!channel.exists && channel.name) {
                    dreamBrokerService.openSuggestChannel(channel, video);
                }

                $scope.model.value.video = video;
                vm.loading = false;
                vm.updateUI();

            });

        } else {

            delete $scope.model.value.channel;
            delete $scope.model.value.video;

        }

    };

    // Triggered by the UI when the user changes the URL
    vm.updated = function () {
        vm.getVideo();
    };

    vm.refresh = function () {
        vm.getVideo();
    };

    // Opens a new overlay where the editor can search and pick videos
    vm.add = function() {
        dreamBrokerService.openAddVideo(function(video) {
            $scope.model.value.video = video;
            $scope.model.value.url = dreamBrokerService.getVideoUrl(video.channelId, video.videoId);
            vm.updateUI();
        });
    };

    // Updates the video information for the UI
    vm.updateUI = function () {

        const video = $scope.model.value.video;

        if (!video) {
            vm.thumbnail = null;
            return;
        }

        vm.duration = dreamBrokerService.getDuration(video.duration);
        vm.thumbnail = dreamBrokerService.getThumbnail(video.channelId, video.videoId);

    };

    vm.updateUI();

});