// [CHANGE: Umbraco 17 upgrade - registers a dedicated Swagger document for the package's Management API]
// Related: Api/DreamBrokerApiConstants.cs, Composers/DreamBrokerComposer.cs

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Limbo.Umbraco.DreamBroker.Api;

public class DreamBrokerSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions> {

    public void Configure(SwaggerGenOptions options) {
        options.SwaggerDoc(DreamBrokerApiConstants.Alias, new OpenApiInfo {
            Title = DreamBrokerApiConstants.Name,
            Version = "1.0"
        });
        options.OperationFilter<DreamBrokerSecurityFilter>();
    }

}
