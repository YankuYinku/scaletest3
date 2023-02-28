using apetito.DependencyInjection.Services;
using apetito.meinapetito.Cache.News.Contracts.News.ApiClients;
using apetito.meinapetito.Portal.Application.News.Services.Implementations;
using apetito.meinapetito.Portal.Application.News.Services.Interfaces;
using RestEase;

namespace apetito.meinapetito.Portal.Api.News;

public class NewsRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var cachedDownloadsBaseUrl = configuration["Dependencies:APIs:newsCacheApiBaseUrl"];

        services.AddSingleton<INewsItemProvider, NewsItemProvider>();

        AddHttpClient<INewsCacheApiClient>(services, cachedDownloadsBaseUrl);
        services.AddTransient(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(INewsCacheApiClient));
            return RestClient.For<INewsCacheApiClient>(httpClient);
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