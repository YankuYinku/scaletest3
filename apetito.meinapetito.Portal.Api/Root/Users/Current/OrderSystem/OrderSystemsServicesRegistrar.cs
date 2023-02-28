using apetito.CQS;
using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Root.Users.Current.OrderSystems.Queries;
using apetito.meinapetito.Portal.Application.Root.Users.Current.OrderSystems.Queries.Handlers;
using apetito.meinapetito.Portal.Contracts.Root.Users.OrderSystems;

namespace apetito.meinapetito.Portal.Api.Root.Users.Current.OrderSystem;

public class OrderSystemsServicesRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IQueryHandler<RetrieveOrderSystemsOfUserEmailQuery,
            OrderSystemsDto>, RetrieveOrderSystemsOfUserEmailQueryHandler>();
    }
}