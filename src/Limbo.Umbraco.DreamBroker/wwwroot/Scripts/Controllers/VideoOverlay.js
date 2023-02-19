angular.module("umbraco").controller("Limbo.Umbraco.DreamBroker.VideoOverlay", function ($scope, dreamBrokerService) {

    const vm = this;

    vm.loading = true;

    dreamBrokerService.getVideos().then(function (res) {

        vm.videos = [];
        vm.channelCount = 0;

        res.data.channels.forEach(function(channel) {
            channel.videos.forEach(function (video) {
                vm.videos.push(video);
                vm.channelCount++;
                video.$channelName = channel.name;
                video.$duration = dreamBrokerService.getDuration(video.duration);
            });
        });

        vm.videos.sort(function (a, b) {
            a = a.title.toUpperCase();
            b = b.title.toUpperCase();
            if (a < b) return -1;
            if (a > b) return 1;
            return 0;
        });

        vm.loading = false;

    });

    vm.select = function(video) {
        $scope.model.submit(video);
    };

});