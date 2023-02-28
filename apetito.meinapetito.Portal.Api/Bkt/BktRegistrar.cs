using System.Net.Http.Headers;
using apetito.BearerToken;
using apetito.BKT.Contracts.ApiClient;
using apetito.CQS;
using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Bkts.Queries;
using apetito.meinapetito.Portal.Application.Bkts.Queries.Handlers;
using apetito.meinapetito.Portal.Application.Bkts.Services.Implementations;
using apetito.meinapetito.Portal.Application.Bkts.Services.Interfaces;
using apetito.meinapetito.Portal.Application.Root.Authentication.LegacyTokenExchange;
using apetito.meinapetito.Portal.Contracts.Bkts.Models;
using RestEase;

namespace apetito.meinapetito.Portal.Api.Bkt;

public class BktRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IMonthlyRecordsProvider, MonthlyRecordsProvider>();
        services.AddTransient<IBktRecordUpdater, BktRecordUpdater>();
        services.AddTransient<IQueryHandler<RetrieveToleranceQuery,BktToleranceCheckResult>, RetrieveToleranceQueryHandler>();

        var cachedDownloadsBaseUrl = configuration["Dependencies:APIs:BKTApiBaseUrl"];
        
        AddHttpClientWithTokenExchange<IBktRestApi>(services, cachedDownloadsBaseUrl);
        services.AddTransient(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(IBktRestApi));
            return RestClient.For<IBktRestApi>(httpClient);
        });
    }
    
    private static void AddHttpClientWithTokenExchange<T>(IServiceCollection services, string baseUrl)
        {
            services.AddHttpClient(typeof(T).Name)
                .ConfigureHttpClient((provider, client) =>
                {
                    var logger = provider.GetRequiredService<ILogger<Program>>();
                    var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

                    var legacyToken = httpContextAccessor.HttpContext?.Request.Headers["X-Apetito-Authorization"].FirstOrDefault();

                    logger.LogInformation("AddHttpClientWithTokenExchange legacyToken:" + legacyToken);

                    if (legacyToken == null)
                    {
                        try
                        {
                            using var scope = provider.CreateScope();
                            var bearerTokenProvider = scope.ServiceProvider.GetRequiredService<IBearerTokenRequestProvider>();
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
                })
                .ConfigurePrimaryHttpMessageHandler((provider) =>
                {
                    return new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                    };
                });
        }
}