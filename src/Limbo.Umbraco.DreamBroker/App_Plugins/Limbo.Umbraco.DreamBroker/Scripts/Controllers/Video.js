angular.module("umbraco").controller("Limbo.Umbraco.DreamBroker.Video", function ($scope, dreamBrokerService) {

    const vm = this;

    // Gets information about the video of the entered URL
    vm.getVideo = function() {

        const source = $scope.model.value && $scope.model.value.source && $scope.model.value.source.trim().replace(/\/$/, "");

        const m = source.match(/^https:\/\/(www\.|)dreambroker\.com\/channel\/([a-z0-9]+)\/([a-z0-9]+)/);

        if (m) {

            vm.loading = true;

            dreamBrokerService.getVideo(m[2], m[3]).then(function (res) {

                const channel = res.data.channel;
                const video = res.data.video;

                if (!channel.exists && channel.name) {
                    dreamBrokerService.openSuggestChannel(channel, video);
                }

                if (!$scope.model.value) $scope.model.value = {};
                $scope.model.value.video = video;
                vm.loading = false;
                vm.update();

            });

        } else {

            delete $scope.model.value.channel;
            delete $scope.model.value.video;
            vm.update();

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
        dreamBrokerService.openAddVideo(function (video) {
            if (!$scope.model.value) $scope.model.value = {};
            $scope.model.value.video = video;
            $scope.model.value.source = dreamBrokerService.getVideoUrl(video.channelId, video.videoId);
            vm.update();
        });
    };

    // Updates the video information for the UI
    vm.update = function () {

        const url = $scope.model.value && $scope.model.value.url;
        const source = $scope.model.value && $scope.model.value.source;
        const video = $scope.model.value && $scope.model.value.video;

        if (!url && !source && !video) {
            $scope.model.value = null;
            vm.duration = null;
            vm.thumbnail = null;
            return;
        }

        if (!video) {
            vm.thumbnail = null;
            return;
        }

        vm.duration = dreamBrokerService.getDuration(video.duration);
        vm.thumbnail = dreamBrokerService.getThumbnail(video.channelId, video.videoId);

    };

    function init() {

        if ($scope.model.value) {

            // Fix "legacy" values
            if ($scope.model.value.url && !$scope.model.value.source) {
                $scope.model.value.source = $scope.model.value.url;
                delete $scope.model.value.url;
            }

        } else {

            $scope.model.value = null;

        }

        vm.update();

    }

    init();

});