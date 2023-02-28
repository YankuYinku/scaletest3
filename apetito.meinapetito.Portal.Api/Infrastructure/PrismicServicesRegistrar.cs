using apetito.DependencyInjection.Services;

namespace apetito.meinapetito.Portal.Api.Infrastructure;

public class PrismicServicesRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PrismicOptions>().Bind(configuration.GetSection("Prismic"));
    }
}