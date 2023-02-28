using System.Net.Http.Headers;
using apetito.Authorization.Contracts.ClaimsPermissions;
using apetito.BearerToken;
using apetito.CQS;
using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Root.Authentication.LegacyTokenExchange;
using apetito.meinapetito.Portal.Application.Root.Users.Current.Permissions.Queries;
using apetito.meinapetito.Portal.Application.Root.Users.Current.Permissions.Queries.Handler;
using apetito.meinapetito.Portal.Contracts.Root.Users.Permissions;
using RestEase;

namespace apetito.meinapetito.Portal.Api.Root.Users.Current.Permissions;

public class PermissionsServiceRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        string permissionsBaseUrl = configuration["Dependencies:APIs:AuthorizationApiBaseUrl"];

        services.AddTransient<IQueryHandler<RetrieveClaimsPermissionsQuery, PermissionsSetDto>,
            RetrieveClaimsPermissionsQueryHandler>();

        AddHttpClientWithTokenExchange<IClaimsPermissionsRestApi>(services, permissionsBaseUrl);
        services.AddTransient(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(IClaimsPermissionsRestApi));
            return RestClient.For<IClaimsPermissionsRestApi>(httpClient);
        });
    }

    private static void AddHttpClientWithTokenExchange<T>(IServiceCollection services, string baseUrl)
    {
        services.AddHttpClient(typeof(T).Name)
            .ConfigureHttpClient((provider, client) =>
            {
                var logger = provider.GetRequiredService<ILogger<Program>>();
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

                var legacyToken = httpContextAccessor.HttpContext?.Request.Headers["X-Apetito-Authorization"]
                    .FirstOrDefault();

                logger.LogInformation("AddHttpClientWithTokenExchange legacyToken:" + legacyToken);

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

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", legacyToken);
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