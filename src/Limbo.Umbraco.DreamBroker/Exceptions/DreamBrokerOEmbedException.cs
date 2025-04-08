using System;

#pragma warning disable CS1591

namespace Limbo.Umbraco.DreamBroker.Exceptions;

public class DreamBrokerOEmbedException : DreamBrokerException {

    public string ChannelId { get; }

    public string VideoId { get; }

    public DreamBrokerOEmbedException(string channelId, string videoId) : base($"Failed getting OEmbed details for video.\r\n\r\nChannel ID: {channelId}\r\nVideo ID: {videoId}") {
        ChannelId = channelId;
        VideoId = videoId;
    }

    public DreamBrokerOEmbedException(string channelId, string videoId, Exception? innerException) : base($"Failed getting OEmbed details for video.\r\n\r\nChannel ID: {channelId}\r\nVideo ID: {videoId}", innerException) {
        ChannelId = channelId;
        VideoId = videoId;
    }

}