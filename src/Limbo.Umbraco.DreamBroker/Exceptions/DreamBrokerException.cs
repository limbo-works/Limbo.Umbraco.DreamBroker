using System;

namespace Limbo.Umbraco.DreamBroker.Exceptions;

/// <summary>
/// Class representing a generic DreamBroker exception.
/// </summary>
public class DreamBrokerException : Exception {

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The message of the exception.</param>
    public DreamBrokerException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="message"/> and <paramref name="innerException"/>.
    /// </summary>
    /// <param name="message">The message of the exception.</param>
    /// <param name="innerException">The inner exception.</param>
    public DreamBrokerException(string message, Exception? innerException) : base(message, innerException) { }

}