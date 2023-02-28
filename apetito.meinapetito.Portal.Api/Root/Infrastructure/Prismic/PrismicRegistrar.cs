using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Services.Implementations;
using apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Services.Interfaces;

namespace apetito.meinapetito.Portal.Api.Root.Infrastructure.Prismic;

public class PrismicRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IPrismicApiClientCallExecutor, PrismicApiClientCallExecutor>();
        
        RegisterBasePrismicOptionsFromKeyVault(configuration,services);
    }
    private static void RegisterBasePrismicOptionsFromKeyVault(IConfiguration? configuration, IServiceCollection serviceCollection)
    {
        var withDataFromKeyVault = new apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Options.PrismicOptions()
        {
            AccessToken = configuration?["Prismic:AccessToken"],
            Endpoint = configuration?["Prismic:Endpoint"]
        };
        serviceCollection.AddSingleton(withDataFromKeyVault);
    }
}