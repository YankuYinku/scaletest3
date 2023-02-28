using apetito.CQS;
using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Root.Users.Claims.Queries;
using apetito.meinapetito.Portal.Application.Root.Users.Claims.Queries.Handlers;
using apetito.meinapetito.Portal.Contracts.Root.Users.Claims;

namespace apetito.meinapetito.Portal.Api.Root.Users.Current.Claims;
public class ClaimsOfUserServicesRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
         services
            .AddTransient<IQueryHandler<RetrieveClaimsOfUserQuery, UserAndCustomerClaimsDto>,
                RetrieveClaimsOfUserQueryHandler>();
    }
}