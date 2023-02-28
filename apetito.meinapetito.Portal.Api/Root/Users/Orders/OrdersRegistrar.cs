using System.Net.Http.Headers;
using apetito.BearerToken;
using apetito.CQS;
using apetito.DependencyInjection.Services;
using apetito.meinapetito.Order.Contracts.Orders.ApiClients;
using apetito.meinapetito.Portal.Application.Root.Authentication.LegacyTokenExchange;
using apetito.meinapetito.Portal.Application.Root.Users.Orders.Options;
using apetito.meinapetito.Portal.Application.Root.Users.Orders.Queries;
using apetito.meinapetito.Portal.Application.Root.Users.Orders.Queries.Handlers;
using apetito.meinapetito.Portal.Contracts.Root.Users.Orders.Models;
using apetito.meinapetito.Portal.Contracts.Root.Users.Orders.Models.ApetitoOrders;
using apetito.meinapetito.Portal.Contracts.Root.Users.Orders.Models.HawaOrders;
using apetito.Order.Hawa.Contracts.ApiClient;
using RestEase;

namespace apetito.meinapetito.Portal.Api.Root.Users.Orders;

public class OrdersRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var hawaOrdersBaseUrl = configuration["Dependencies:APIs:hawaOrdersBaseUrl"];
        var apetitoOrdersAzureFunctionUrl = configuration["Dependencies:APIs:OrdersApiBaseUrl"];

        services.AddTransient<IQueryHandler<RetrieveApetitoOrders,IList<OrderSummaryDto>>, RetrieveApetitoOrdersHandler>();
        services.AddTransient<IQueryHandler<RetrieveApetitoOrderDetails,ApetitoOrderDto>, RetrieveApetitoOrderDetailsHandler>();
        services.AddTransient<IQueryHandler<RetrieveHawaOrderDetails,HawaOrderDto>, RetrieveHawaOrderDetailsHandler>();
        services.AddTransient<IQueryHandler<RetrieveHawaOrders,IList<OrderSummaryDto>>, RetrieveHawaOrdersHandler>();
        services.AddTransient<IQueryHandler<RetrieveOrders, RetrieveOrdersQueryResult>, RetrieveOrdersHandler>();

        RegisterOrderAzureFunctionApiKeys(services, configuration);
        
        AddHttpClient<IHawaOrderRestApi>(services, hawaOrdersBaseUrl);
        services.AddTransient(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(IHawaOrderRestApi));
            return RestClient.For<IHawaOrderRestApi>(httpClient);
        });
        
        AddUnprotectedHttpClient<IApetitoOrdersClient>(services,apetitoOrdersAzureFunctionUrl );
        services.AddTransient(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(IApetitoOrdersClient));
            return RestClient.For<IApetitoOrdersClient>(httpClient);
        });
    }
    
    private static void RegisterOrderAzureFunctionApiKeys(IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var options = new OrderAzureFunctionApiKeys()
        {
            RetrievieApetitoOrdersKey = configuration["root:orders:RetrieveApetitoOrderHeaders:ApiKey"],
            RetrievieApetitoOrderDetailsKey = configuration["root:orders:RetrieveApetitoOrderWithPositions:ApiKey"]
        };

        serviceCollection.AddSingleton(options);
    }
    
    private static void AddHttpClient<T>(IServiceCollection services, string baseUrl)
    {
        services.AddHttpClient(typeof(T).Name)
            .ConfigureHttpClient((provider, client) =>
            {
                var logger = provider.GetRequiredService<ILogger<Program>>();
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                var legacyToken = httpContextAccessor.HttpContext?.Request.Headers["X-Apetito-Authorization"].FirstOrDefault();

                if (legacyToken == null)
                {
                    try
                    {
                        using var scope = provider.CreateScope();
                        var bearerTokenProvider =
                            scope.ServiceProvider.GetRequiredService<IBearerTokenRequestProvider>();
                        var bearerToken = bearerTokenProvider.Authorization.Parameter;
                        logger.LogInformation("AddHttpClientWithTokenExchange Start token exchange...");
                        logger.LogInformation("AddHttpClientWithTokenExchange bearerToken: " + bearerToken);
                        var tokenExchanger = provider.GetRequiredService<IMeinApetitoTokenExchanger>();
                        legacyToken = tokenExchanger.ExchangeAsync(bearerToken).Result;
                        logger.LogInformation("AddHttpClientWithTokenExchange legacyToken: " + legacyToken);
                    }
                    catch (System.Exception ex)
                    {
                        logger.LogError(ex.ToString());
                    }
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", legacyToken);
                client.BaseAddress = new Uri(baseUrl);
            }).ConfigurePrimaryHttpMessageHandler((provider) =>
            {
                return new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                };
            });
    }
    
    private static void AddUnprotectedHttpClient<T>(IServiceCollection services, string baseUrl)
    {
        services.AddHttpClient(typeof(T).Name).ConfigureHttpClient((provider, client) =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });
    }
}