using Limbo.Umbraco.DreamBroker.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

#pragma warning disable 1591

namespace Limbo.Umbraco.DreamBroker.Composers {
    
    public class DreamBrokerComposer : IComposer {

        public void Compose(IUmbracoBuilder builder) {
            builder.Services.AddTransient<DreamBrokerService>();
        }

    }

}