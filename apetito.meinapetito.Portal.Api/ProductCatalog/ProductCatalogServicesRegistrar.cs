using System.Net.Http.Headers;
using apetito.ArticleGateway.Contracts.ApiClient.V0;
using apetito.ArticleGateway.Contracts.ApiClient.V0.Categories;
using apetito.ArticleGateway.Contracts.ApiClient.V0.Components;
using apetito.ArticleGateway.Contracts.ArticleAvailability;
using apetito.BearerToken;
using apetito.CQS;
using apetito.DependencyInjection.Services;
using apetito.iProDa3.Contracts;
using apetito.iProDa3.Contracts.ApiClient.V1;
using apetito.iProDa3.Contracts.ArticleBrowser;
using apetito.meinapetito.Cache.Articles.Contracts.ProductCatalog.ApiClient;
using apetito.meinapetito.Portal.Application.Bkts.Queries;
using apetito.meinapetito.Portal.Application.Bkts.Queries.Handlers;
using apetito.meinapetito.Portal.Application.ProductCatalog.Options;
using apetito.meinapetito.Portal.Application.ProductCatalog.Queries;
using apetito.meinapetito.Portal.Application.ProductCatalog.Queries.Handlers;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Application.Root.Authentication.LegacyTokenExchange;
using apetito.meinapetito.Portal.Contracts.Bkts.Models;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Availability;
using apetito.Notification.Contracts.ApiClient;
using RestEase;

namespace apetito.meinapetito.Portal.Api.ProductCatalog
{
    public class ProductCatalogServicesRegistrar : IServicesRegistrar
    {
        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            var meinApetitoApiBaseUrl = configuration["Dependencies:APIs:meinapetitoApiBaseUrl"];
            var iProda3ApiBaseUrl = configuration["Dependencies:APIs:iProDa3ApiBaseUrl"];
            var articleGatewayBaseUrl = configuration["Dependencies:APIs:ArticleGatewayApiBaseUrl"];
            var articlesCacheRestClient = configuration["Dependencies:APIs:articleCacheApiBaseUrl"];

            var photoBuilderOptions = new ProductCatalogPhotoBuilderOptions
            {
                MaterialImagePath = configuration["ProductCatalog:PhotoBuilder:MaterialImagePath"],
                NutriScoreImagePath = configuration["ProductCatalog:PhotoBuilder:NutriScoreImagePath"],
                ProductImagePath = new ProductCatalogProductsBySizesOptions
                {
                    Big = configuration["ProductCatalog:PhotoBuilder:ProductImagePath:Big"],
                    Middle = configuration["ProductCatalog:PhotoBuilder:ProductImagePath:Middle"],
                    Small = configuration["ProductCatalog:PhotoBuilder:ProductImagePath:Small"]
                }
            };

            services.AddSingleton(photoBuilderOptions);
            services.AddOptions<ProductCatalogMaterialSortimentOptions>()
                .Bind(configuration.GetSection("ProductCatalog:MaterialSortiment"));

            services.AddOptions<ProductCatalogForbiddenArticlePrefixOptions>()
                .Bind(configuration.GetSection("ProductCatalog:ForbiddenArticlePrefix"));

            services.AddTransient<IApetitoProductsProvider, ApetitoProductsProvider>();
            services.AddTransient<IApetitoCategoriesProvider, ApetitoCategoriesProvider>();
            services.AddTransient<IApetitoDietsProvider, ApetitoDietsProvider>();
            services.AddTransient<IMeinApetitoTokenExchanger, MeinApetitoTokenExchanger>();
            services.AddTransient<IProductCatalogPhotoPathBuilder, ProductCatalogPhotoPathBuilder>();
            services.AddTransient<IApetitoAllergensProvider, ApetitoAllergensProvider>();
            services.AddTransient<IApetitoAdditivesProvider, ApetitoAdditivesProvider>();
            services.AddTransient<IApetitoFoodFormsProvider, ApetitoFoodFormsProvider>();
            services.AddTransient<IApetitoSealsProvider, ApetitoSealsProvider>();
            services.AddTransient<IApetitoNutrientsProvider, ApetitoNutrientsProvider>();
            services.AddTransient<IItemToBasketInserter, ItemToBucketInserter>();

            services.AddTransient<ICurrentUserIdProvider, CurrentUserIdProvider>();
            services.AddTransient<IIngredietsForArticlesFromArticleGatewayProvider,
                IngredietsForArticlesFromArticleGatewayProvider>();
            services
                .AddTransient<IApetitoProductsWithoutCategoriesProvider, ApetitoProductsWithoutCategoriesProvider>();
            services.AddTransient<IApetitoProductsFromCacheProvider, ApetitoProductsFromCacheProvider>();
            services.AddTransient<IApetitoFiltersProvider, ApetitoFiltersProvider>();
            services.AddTransient<ISortimentsProvider, SortimentsProvider>();
            services
                .AddTransient<IQueryHandler<RetrieveArticlesAvailabilityQuery, AvailabilityCheckResultDto>,
                    RetrieveArticlesAvailabilityQueryHandler>();

            services
                .AddTransient<IQueryHandler<RetrieveBktBillingQuery, BktBillingResult>,
                    RetrieveBktBillingQueryHandler>();
            
            services
                .AddTransient<IQueryHandler<RetrieveToleranceDeviationQuery,BktToleranceDeviationResult>,
                    RetrieveBktToleranceDeviationQueryHandler>();
            

            AddHttpClientWithTokenExchange<IArticleAvailabilityRestClient>(services, articleGatewayBaseUrl);
            services.AddTransient(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(nameof(IArticleAvailabilityRestClient));
                return RestClient.For<IArticleAvailabilityRestClient>(httpClient);
            });

            AddHttpClientWithTokenExchange<IIProDa3ArticleBrowserRestClient>(services, iProda3ApiBaseUrl);
            services.AddTransient(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(nameof(IIProDa3ArticleBrowserRestClient));
                return RestClient.For<IIProDa3ArticleBrowserRestClient>(httpClient);
            });

            AddHttpClientWithTokenExchange<IFiltersRestClient>(services, articlesCacheRestClient);
            services.AddTransient(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(nameof(IFiltersRestClient));
                return RestClient.For<IFiltersRestClient>(httpClient);
            });

            AddHttpClient<IiProDa3ArticlesV1RestClient>(services, iProda3ApiBaseUrl);
            services.AddTransient(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(nameof(IiProDa3ArticlesV1RestClient));
                return RestClient.For<IiProDa3ArticlesV1RestClient>(httpClient);
            });

            AddHttpClient<IIProDa3SortimentRestClient>(services, iProda3ApiBaseUrl);
            services.AddTransient(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(nameof(IIProDa3SortimentRestClient));
                return RestClient.For<IIProDa3SortimentRestClient>(httpClient);
            });

            AddHttpClient<IArticlesRestClient>(services, articlesCacheRestClient);
            services.AddTransient(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(nameof(IArticlesRestClient));
                return RestClient.For<IArticlesRestClient>(httpClient);
            });
            AddHttpClient<IArticleGatewayComponentsRestApi>(services, articleGatewayBaseUrl);
            services.AddTransient(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(nameof(IArticleGatewayComponentsRestApi));
                return RestClient.For<IArticleGatewayComponentsRestApi>(httpClient);
            });

            AddHttpClient<IArticleGatewayArticlesV0RestApi>(services, articleGatewayBaseUrl);
            services.AddTransient(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(nameof(IArticleGatewayArticlesV0RestApi));
                return RestClient.For<IArticleGatewayArticlesV0RestApi>(httpClient);
            });

            AddHttpClientWithTokenExchange<IArticleGatewayCategoriesRestApi>(services, articleGatewayBaseUrl);
            services.AddTransient(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(nameof(IArticleGatewayCategoriesRestApi));
                return RestClient.For<IArticleGatewayCategoriesRestApi>(httpClient);
            });


            AddHttpClient<IMeinApetitoApi>(services, meinApetitoApiBaseUrl);
            services.AddTransient(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(nameof(IMeinApetitoApi));
                return RestClient.For<IMeinApetitoApi>(httpClient);
            });
        }

        private static void AddHttpClient<T>(IServiceCollection services, string baseUrl)
        {
            services.AddHttpClient(typeof(T).Name)
                .ConfigureHttpClient(client => { client.BaseAddress = new Uri(baseUrl); })
                .ConfigurePrimaryHttpMessageHandler((provider) =>
                {
                    return new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                    };
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
}