using System;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.DreamBroker.Exceptions;

public class DreamBrokerVideoException : DreamBrokerException {

    public DreamBrokerVideoException() : base("Failed retrieving video information from the DreamBroker API.") { }

    public DreamBrokerVideoException(string message) : base(message) { }

    public DreamBrokerVideoException(Exception? innerException) : base("Failed retrieving video information from the DreamBroker API.", innerException) { }

    public DreamBrokerVideoException(string message, Exception? innerException) : base(message, innerException) { }

}