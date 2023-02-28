using apetito.CQS;
using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Application.Root.Configuration.Queries;
using apetito.meinapetito.Portal.Application.Root.Configuration.Queries.Handlers;
using apetito.meinapetito.Portal.Contracts.Root.Configuration;
using Microsoft.FeatureManagement;

namespace apetito.meinapetito.Portal.Api.Root.Configuration;

public class ConfigurationServicesRegistrar  : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureAppConfiguration();
        services.AddFeatureManagement();
        
        services.AddTransient<IQueryHandler<RetrieveFeatureFlagNamesQuery,FeatureFlagNamesDto>, RetrieveFeatureFlagNamesQueryHandler>();
    }
}