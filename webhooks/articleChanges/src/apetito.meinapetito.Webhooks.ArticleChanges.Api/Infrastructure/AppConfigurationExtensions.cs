using System;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace apetito.meinapetito.Webhooks.ArticleChanges.Api.Infrastructure
{
    public static class AppConfigurationExtensions
    {
        public static IConfigurationBuilder 
        ConfigureAzureAppConfiguration(this IConfigurationBuilder builder,
            IConfiguration? configuration)
        {
            try
            {
                string servicePrincipalClientId = "";
                string servicePrincipalclientSecret = "";
                string subscriptionTenantId = "";

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (environment == "Development" || environment == "Staging" || environment == "Production")
                {
                    servicePrincipalClientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
                    servicePrincipalclientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
                    subscriptionTenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
                    
                }
                else
                {
                    servicePrincipalClientId = configuration["AZURE_CLIENT_ID"];
                    servicePrincipalclientSecret = configuration["AZURE_CLIENT_SECRET"];
                    subscriptionTenantId = configuration["AZURE_TENANT_ID"];
                }
                
                
                var keyPrefix = "apetito:meinapetito:webhooks:azureadb2c:api:";
            
            
            Console.WriteLine($"AZURE_CLIENT_ID : {servicePrincipalClientId}");
            Console.WriteLine($"AZURE_TENANT_ID : {subscriptionTenantId}");
            
                var credentials = //new ManagedIdentityCredential(); 
                    new ClientSecretCredential(subscriptionTenantId, servicePrincipalClientId,
                        servicePrincipalclientSecret);
            
               
            
                var appSettingsUri = environment switch
                {
                    "Staging" => new Uri("https://apetitoappsettings-staging.azconfig.io"),
                    "Production" => new Uri("https://apetitoappsettings.azconfig.io"),
                    _ => new Uri("https://apetitoappsettings-development.azconfig.io"),
                };
            
                Console.WriteLine($"APP SETTINGS URL : {appSettingsUri}");
                
                builder.AddAzureAppConfiguration(options =>
                {
                    options
                        .Connect(appSettingsUri, credentials)
                        .ConfigureRefresh(refreshConfiguration =>
                        {
                            refreshConfiguration
                                .Register($"{keyPrefix}ReloadSentinel", true)
                                .SetCacheExpiration(new TimeSpan(0, 0, 3));
                        })
                        .ConfigureKeyVault(keyVaultConfiguration =>
                        {
                            keyVaultConfiguration.SetCredential(credentials);
                        })
                        .Select($"{keyPrefix}*")
                        .Select("apetito:utils:*", LabelFilter.Null)
                        // Override with any configuration values specific to current hosting env
                        .Select($"{keyPrefix}*", string.IsNullOrWhiteSpace(environment) ? "Development" : environment)
                        .TrimKeyPrefix(keyPrefix)
                        // .UseFeatureFlags(options => { options.Select("apetito.meinapetito.portal*"); })
                        ;
                });
            
                return builder;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.ToString()}");
            }
            
             return builder;
        }
    }
}