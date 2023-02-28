using apetito.BearerToken;
using apetito.CQS;
using apetito.Customers.Contracts.Company.ApiClients;
using apetito.Customers.Contracts.Company.Models;
using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Root.Company.Queries;
using apetito.meinapetito.Portal.Application.Root.Company.Queries.Handlers;
using RestEase;

namespace apetito.meinapetito.Portal.Api.Root.Company;

public class CompanyServiceRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        if (!services.Any(x => x.ServiceType == typeof(IHttpContextAccessor)))
        {
            services.AddHttpContextAccessor();
        }
        
        services.AddHttpClient<ICompanyServiceRestClient>(
                nameof(ICompanyServiceRestClient))
            .ConfigureHttpClient((provider,httpClient) =>
            {
                using var scope = provider.CreateScope();
                var bearerTokenRequestProvider = scope.ServiceProvider.GetRequiredService<IBearerTokenRequestProvider>();

                httpClient.BaseAddress = new Uri(configuration["CustomersService:BaseAddress"]);
                httpClient.DefaultRequestHeaders.Authorization = bearerTokenRequestProvider.Authorization;
            });
        
        services.AddTransient(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient(nameof(ICompanyServiceRestClient));
            return RestClient.For<ICompanyServiceRestClient>(client);
        });
        
        services
            .AddTransient<IQueryHandler<RetrieveCompanyDataQuery, AllCompanyDataDto>, 
                RetrieveCompanyDataQueryHandler>();
    }
}