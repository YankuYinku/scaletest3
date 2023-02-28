using System.Reflection;
using apetito.meinapetito.Webhooks.AzureADB2C.Api.Infrastructure;
using apetito.meinapetito.Webhooks.AzureADB2C.Api.Services.Abstract;
using apetito.meinapetito.Webhooks.AzureADB2C.Api.Services.Implementations;
using apetito.meinapetito.Webhooks.AzureADB2C.Api.Services.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        builder.ConfigureAppConfiguration((a, b) => b.AddUserSecrets(Assembly.GetExecutingAssembly(), true));
        builder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.ConfigureAzureAppConfiguration(config.Build());
        });
    }
    
    
    
    
    builder.ConfigureServices(s =>
        {
            var service = s.BuildServiceProvider();
    
            var conf = service.GetRequiredService<IConfiguration>();
    
            var options = new GraphApiOptions
            {
                Domain = conf["AzureADB2C:Domain"],
                TenantId = conf["AzureADB2C:TenantId"],
                ApplicationId = conf["AzureADB2C:AppRegistration:PortalApi:ObjectId"],
                ClientId = conf["AzureADB2C:AppRegistration:PortalManagement:ClientId"],
                ClientSecret = conf["AzureADB2C:AppRegistration:PortalManagement:ClientSecret"],
            };
    
            var meinapetitoAccessOptions = new MeinapetitoAccessOptions
            {
                Secret = conf["meinapetito:secret"],
                BaseUrl = conf["meinapetito:baseUrl"]
            };
    
            s.AddSingleton(options);
            s.AddSingleton(meinapetitoAccessOptions);
            s.AddTransient<IGetFeatureClustersUrls, GetFeatureClustersUrls>();
            s.AddTransient<IWebhookSender, WebhookSender>();
        }
    
    );

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