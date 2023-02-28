using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Options;
using apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Services.Implementations;
using apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Services.Interfaces;

namespace apetito.meinapetito.Portal.Api.Prismic.Webhook;

public class WebhookRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var meinapetitoAccessOptions = new PrismicWebhookOptions()
        {
            Secret = configuration["Prismic:Webhook:Secret"]
        }; 

        services.AddSingleton(meinapetitoAccessOptions);
        services.AddTransient<IPrismicWebhookProcessor,PrismicWebhookProcessor>();
    }
}