using apetito.CQS;
using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Root.Users.Filters.Commands;
using apetito.meinapetito.Portal.Application.Root.Users.Filters.Commands.Handlers;
using apetito.meinapetito.Portal.Application.Root.Users.Filters.Queries;
using apetito.meinapetito.Portal.Application.Root.Users.Filters.Queries.Handlers;
using apetito.meinapetito.Portal.Contracts.Root.Users.Filters;

namespace apetito.meinapetito.Portal.Api.Root.Users.Filters;

public class PortalUserFiltersServicesRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICommandHandler<CreatePortalUserFilter>, CreatePortalUserFilterHandler>();
        services.AddTransient<ICommandHandler<EditPortalUserFilter>, EditPortalUserFilterHandler>();
        services.AddTransient<ICommandHandler<RemovePortalUserFilter>, RemovePortalUserFilterHandler>();
        services.AddTransient<IQueryHandler<RetrievePortalUserFilters, IEnumerable<PortalUserFilterDto>>, RetrievePortalUserFiltersHandler>();
    }

}