using apetito.CQS;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.ADB2C.Queries;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure.ADB2C;
using apetito.meinapetito.Portal.Data.Root;
using Microsoft.EntityFrameworkCore;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands.Handlers;

public class MarkUserAsActiveAndUpdateNamesFromAzureAdb2CCommandHandler : ICommandHandler<MarkUserAsActiveAndUpdateNamesFromAzureAdb2C>
{
    private readonly PortalDbContext _portalDbContext;
    public MarkUserAsActiveAndUpdateNamesFromAzureAdb2CCommandHandler(PortalDbContext portalDbContext)
    {
        _portalDbContext = portalDbContext;
    }

    public async Task Handle(MarkUserAsActiveAndUpdateNamesFromAzureAdb2C command)
    {
        var portalUser = await _portalDbContext.PortalUsers.FirstOrDefaultAsync(a => a.Email == command.Email);

        if (portalUser is null)
        {
            throw new Exception("User is not existing in our system");
        }
        
        portalUser.Status = "Active";
        portalUser.FirstName = command.GivenName;
        portalUser.LastName = command.SurName;
        portalUser.AzureADB2CUserId = command.UserId;
        
        await _portalDbContext.SaveChangesAsync();
    }
}