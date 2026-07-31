// [CHANGE: Umbraco 17 upgrade - adds the backoffice bearer token requirement to our Swagger document]
// Related: Api/DreamBrokerApiConstants.cs, Api/DreamBrokerSwaggerGenOptions.cs

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Umbraco.Cms.Api.Management.OpenApi;

namespace Limbo.Umbraco.DreamBroker.Api;

public class DreamBrokerSecurityFilter : BackOfficeSecurityRequirementsOperationFilterBase {

    // This must be the Swagger *document* name (i.e. the alias we pass to SwaggerDoc), not the human readable title -
    // the base filter compares it against context.DocumentName.
    protected override string ApiName => DreamBrokerApiConstants.Alias;

}
