namespace Limbo.Umbraco.DreamBroker.Exceptions;

/// <summary>
/// Exception class thrown when a requested video is not found.
/// </summary>
public class DreamBrokerVideoNotFoundException : DreamBrokerVideoException {

    /// <summary>
    /// Gets the ID of the video.
    /// </summary>
    public string VideoId { get; }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="videoId"/>.
    /// </summary>
    /// <param name="videoId">The ID of the video.</param>
    public DreamBrokerVideoNotFoundException(string videoId) : base($"DreamBroker video with ID '{videoId}' not found.") {
        VideoId = videoId;
    }

}