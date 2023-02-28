using apetito.CQS;
using apetito.meinapetito.Portal.Data.Root;
using Microsoft.EntityFrameworkCore;

namespace apetito.meinapetito.Portal.Application.Root.Users.Filters.Commands.Handlers;

public class EditPortalUserFilterHandler : ICommandHandler<EditPortalUserFilter>
{
    private readonly PortalDbContext _portalDbContext;

    public EditPortalUserFilterHandler(PortalDbContext portalDbContext)
    {
        _portalDbContext = portalDbContext;
    }

    public async Task Handle(EditPortalUserFilter command)
    {
        var portalUserFilter =
            await _portalDbContext.PortalUserFilters.FirstOrDefaultAsync(a =>
                a.Email == command.Email && a.Id == command.Id);

        if (portalUserFilter is null)
        {
            return;
        }

        portalUserFilter.Value = command.Value;
        portalUserFilter.Name = command.Name;

        await _portalDbContext.SaveChangesAsync();
    }
}