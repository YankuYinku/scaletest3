using apetito.CQS;
using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Root.Users.Current;
using apetito.meinapetito.Portal.Application.Root.Users.Current.Customers.Queries;
using apetito.meinapetito.Portal.Application.Root.Users.Current.Customers.Queries.Handlers;
using apetito.meinapetito.Portal.Contracts.Root.Users.Current;
using apetito.meinapetito.Portal.Contracts.Root.Users.Customers;

namespace apetito.meinapetito.Portal.Api.Root.Users.Current;

public class CurrentUserServicesRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTransient<IQueryHandler<RetrieveCurrentUserQuery,UserDto>,
                RetrieveCurrentUserQueryHandler>();
        services
            .AddTransient<IQueryHandler<RetrieveCustomersOfUserQuery, CustomersOfUserDto>,
                RetrieveCustomersOfUserQueryHandler>();
        services
            .AddTransient<IQueryHandler<RetrieveCustomersOfUserFromSAPQuery, CustomersOfUserDto>,
                RetrieveCustomersOfUserFromSAPQueryHandler>();
        services
            .AddTransient<IQueryHandler<RetrieveCustomersOfUserFromPortalDbQuery, CustomersOfUserDto>,
                RetrieveCustomersOfUserFromPortalDbQueryHandler>();
    }
}