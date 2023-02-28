using apetito.CQS;
using apetito.DependencyInjection.Services;
using apetito.iProDa3.Contracts;
using apetito.meinapetito.Portal.Application.Root.Users.Current.Sortiments.Queries;
using apetito.meinapetito.Portal.Application.Root.Users.Current.Sortiments.Queries.Handlers;
using apetito.meinapetito.Portal.Contracts.Root.Users.Sortiments;
using RestEase;

namespace apetito.meinapetito.Portal.Api.Root.Users.Current.Sortiments;

public class SortimentsServicesRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTransient<IQueryHandler<RetrieveSortimentsOfUserEmailQuery, IEnumerable<SortimentDto>>,
                RetrieveSortimentsOfUserEmailQueryHandler>();


        services.AddHttpClient<IIProDa3SortimentRestClient>(
                nameof(IIProDa3SortimentRestClient))
            .ConfigureHttpClient(c => { c.BaseAddress = new Uri(configuration["Dependencies:APIs:iProDa3ApiBaseUrl"]); });

        services.AddTransient(provider =>
        {
            var clientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = clientFactory.CreateClient(nameof(IIProDa3SortimentRestClient));
            return RestClient.For<IIProDa3SortimentRestClient>(httpClient);
        });
    }
}