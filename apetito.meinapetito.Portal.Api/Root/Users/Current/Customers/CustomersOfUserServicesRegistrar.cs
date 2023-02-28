using apetito.BearerToken;
using apetito.CQS;
using apetito.Customers.Contracts.CustomersOfUser.ApiClients;
using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Root.Users.Current.Customers.Queries;
using apetito.meinapetito.Portal.Application.Root.Users.Current.Customers.Queries.Handlers;
using apetito.meinapetito.Portal.Contracts.Root.Users.Customers;
using RestEase;

namespace apetito.meinapetito.Portal.Api.Root.Users.Current.Customers;
public class CustomersOfUserServicesRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        if (!services.Any(x => x.ServiceType == typeof(IHttpContextAccessor)))
        {
            services.AddHttpContextAccessor();
        }
        
        services.AddHttpClient<ICustomerServiceRestClient>(
                nameof(ICustomerServiceRestClient))
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
            var client = httpClientFactory.CreateClient(nameof(ICustomerServiceRestClient));
            return RestClient.For<ICustomerServiceRestClient>(client);
        });
        
        services
            .AddTransient<IQueryHandler<RetrieveCustomersOfUserFromSAPQuery, CustomersOfUserDto>,
                RetrieveCustomersOfUserFromSAPQueryHandler>();
    }
}