using System;
using System.Net.Http;
using System.Reflection;
using apetito.iProDa3.Contracts.ApiClient.V1;
using apetito.meinapetito.Webhooks.ArticleChanges.Api.Infrastructure;
using apetito.meinapetito.Webhooks.ArticleChanges.Application.Services.Implementation;
using apetito.meinapetito.Webhooks.ArticleChanges.Application.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RestEase;

namespace apetito.meinapetito.Webhooks.ArticleChanges.Api
{
    public class Program
    {
        public static void Main()
        {
            try
            {
            var builder = new HostBuilder()
                    .ConfigureFunctionsWorkerDefaults()
                ;

            const string Production = "Production";
            const string Staging = "Staging";
            const string Development = "Development";
            
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            if (environment == Development ||environment == Staging || environment == Production)
            {
                builder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.ConfigureAzureAppConfiguration(null);
                });
            } 
            else
            {
                builder.ConfigureAppConfiguration((a, b) => b.AddUserSecrets(Assembly.GetExecutingAssembly(), false));
                builder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.ConfigureAzureAppConfiguration(config.Build()).AddEnvironmentVariables()
                        ;
                });
            }
            
            builder.ConfigureServices(services =>
            {
                services.AddTransient<IApetitoArticlesAllergeneRelevantChangeDetector, ApetitoArticlesAllergeneRelevantChangeDetector>();
                
                
                AddHttpClient<IiProDa3ArticlesV1RestClient>(services, "https://api.apetito.de/article/iproda");
                services.AddTransient(provider =>
                {
                    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                    var httpClient = httpClientFactory.CreateClient(nameof(IiProDa3ArticlesV1RestClient));
                    return RestClient.For<IiProDa3ArticlesV1RestClient>(httpClient);
                });
                
            });
            
            var host = builder.Build();
            
            host.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EXCEPTION HANDLED IN MAIN : {ex}");
            var inner = ex.InnerException;
            while (inner is not null)
            {
                Console.WriteLine($"EXCEPTION HANDLED IN MAIN : {inner}");
                inner = inner.InnerException;
            }
        }
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
    
    
}