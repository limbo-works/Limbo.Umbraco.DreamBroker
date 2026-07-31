// [CHANGE: Umbraco 17 upgrade - the AngularJS "PluginController" is gone, so the backoffice API is now
// a versioned Management API and needs its own route/Swagger document] Related: Controllers/DreamBrokerController.cs,
// Api/DreamBrokerSecurityFilter.cs, Api/DreamBrokerSwaggerGenOptions.cs, Composers/DreamBrokerComposer.cs

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.DreamBroker.Api;

public static class DreamBrokerApiConstants {

    public const string Route = "limbo/dreambroker";

    public const string Alias = "limbo-dreambroker-v1";

    public const string Name = "Limbo DreamBroker API v1";

    public const string GroupName = "Limbo DreamBroker";

}
