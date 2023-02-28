using apetito.CQS;
using apetito.meinapetito.Portal.Data.Root;
using apetito.meinapetito.Portal.Data.Root.Users;

namespace apetito.meinapetito.Portal.Application.Root.Users.Filters.Commands.Handlers;

public class CreatePortalUserFilterHandler : ICommandHandler<CreatePortalUserFilter>
{
    private readonly PortalDbContext _portalDbContext;

    public CreatePortalUserFilterHandler(PortalDbContext portalDbContext)
    {
        _portalDbContext = portalDbContext;
    }

    public async Task Handle(CreatePortalUserFilter command)
    {
        await _portalDbContext.PortalUserFilters.AddAsync(new PortalUserFilter()
        {
            Context = command.Context.ToString(),
            Email = command.Email,
            Id = command.Id,
            Name = command.Name,
            Value = command.Value
        });

        await _portalDbContext.SaveChangesAsync();
    }
}