namespace Limbo.Umbraco.DreamBroker.Exceptions;

/// <summary>
/// Exception class thrown when an invalid URL is encountered.
/// </summary>
public class DreamBrokerInvalidUrlException : DreamBrokerVideoException {

    /// <summary>
    /// Gets the URL.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Initialize a new instance based on the specified <paramref name="url"/>.
    /// </summary>
    /// <param name="url">The video URL.</param>
    public DreamBrokerInvalidUrlException(string url) : base("Source doesn't match a valid URL.") {
        Url = url;
    }

}