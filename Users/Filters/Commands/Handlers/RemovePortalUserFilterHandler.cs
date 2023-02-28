using apetito.CQS;
using apetito.meinapetito.Portal.Data.Root;
using Microsoft.EntityFrameworkCore;

namespace apetito.meinapetito.Portal.Application.Root.Users.Filters.Commands.Handlers;

public class RemovePortalUserFilterHandler : ICommandHandler<RemovePortalUserFilter>
{
    private readonly PortalDbContext _portalDbContext;

    public RemovePortalUserFilterHandler(PortalDbContext portalDbContext)
    {
        _portalDbContext = portalDbContext;
    }

    public async Task Handle(RemovePortalUserFilter command)
    {
        var candidate =
            await _portalDbContext.PortalUserFilters.FirstOrDefaultAsync(a =>
                a.Id == command.FilterId && a.Email == command.Email);

        if (candidate is null)
        {
            return;
        }

        _portalDbContext.PortalUserFilters.Remove(candidate);

        await _portalDbContext.SaveChangesAsync();
    }
}