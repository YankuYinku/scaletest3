using apetito.DependencyInjection.Services;
using apetito.meinapetito.Cache.Downloads.Contracts.Downloads.ApiClients;
using apetito.meinapetito.Cache.Faqs.Contracts.Faqs.ApiClients;
using apetito.meinapetito.Portal.Application.Downloads.Services.Implementations;
using apetito.meinapetito.Portal.Application.Downloads.Services.Interfaces;
using apetito.meinapetito.Portal.Application.Faqs.Services.Implementations;
using apetito.meinapetito.Portal.Application.Faqs.Services.Interfaces;
using RestEase;

namespace apetito.meinapetito.Portal.Api.Faqs;

public class FaqsRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var cachedFaqsBaseUrl = configuration["Dependencies:APIs:cachedFaqsBaseUrl"];

        services.AddSingleton<IFaqsItemProvider, FaqsItemProvider>();

        AddHttpClient<IFaqsCacheApiClient>(services, cachedFaqsBaseUrl);
        services.AddTransient(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(IFaqsCacheApiClient));
            return RestClient.For<IFaqsCacheApiClient>(httpClient);
        });
    }


    private static void AddHttpClient<T>(IServiceCollection services, string baseUrl)
    {
        services.AddHttpClient(typeof(T).Name)
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            }).ConfigurePrimaryHttpMessageHandler((provider) =>
            {
                return new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                };
            });
    }
}