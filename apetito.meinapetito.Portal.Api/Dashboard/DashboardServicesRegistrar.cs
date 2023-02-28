using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Dashboard.Services.Implementations;
using apetito.meinapetito.Portal.Application.Dashboard.Services.Implementations.Prismic;
using apetito.meinapetito.Portal.Application.Dashboard.Services.Interfaces;

namespace apetito.meinapetito.Portal.Api.Dashboard;

public class DashboardServicesRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
            services.AddTransient<IDashboardPhotoPathBuilder, DashboardPhotoPathBuilder>();
            services.AddTransient<IPrismicApiClientCallExecutor, PrismicApiClientCallExecutor>();

            services.AddTransient<ISliderProductDataProvider, SliderProductDataProvider>();

            services.AddTransient<IDashboardSliderProductsServiceClient, DashboardSliderProductsProvider>();
            services.BuildServiceProvider();

            services.AddOptions<DashboardSliderProductApiOptions>()
                .Bind(configuration.GetSection("ProductCatalog:ApetitoIProdaApi"));

            services.AddOptions<DashboardPhotoBuilderOptions>()
                .Bind(configuration.GetSection("ProductCatalog:PhotoBuilder"));

            RegisterPrismicOptionsFromKeyVault(configuration, services);

        }

        private static void RegisterPrismicOptionsFromKeyVault(IConfiguration? configuration, IServiceCollection serviceCollection)
        {
            var withDataFromKeyVault = new PrismicOptions()
            {
                AccessToken = configuration?["Prismic:AccessToken"],
                Endpoint = configuration?["Prismic:Endpoint"],
                DocumentId = configuration?["Dashbaord:Prismic:Documents:RemnantsSliderId"]
            };
            serviceCollection.AddSingleton(withDataFromKeyVault);
        }
}