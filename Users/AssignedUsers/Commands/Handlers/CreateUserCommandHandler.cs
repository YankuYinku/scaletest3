using apetito.CQS;
using apetito.meinapetito.Portal.Data.Root;
using apetito.meinapetito.Portal.Data.Root.MainCustomerAdmins.PortalUsers;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands.Handlers;

public class CreateUserCommandHandler : ICommandHandler<CreateUser>
{
    private readonly PortalDbContext _portalDbContext;

    public CreateUserCommandHandler(PortalDbContext portalDbContext)
    {
        _portalDbContext = portalDbContext;
    }

    public async Task Handle(CreateUser command)
    {
        if (_portalDbContext.PortalUsers.Any(a => a.Email == command.Email))
        {
            return;
        }
        
        var entity = new PortalUserData
        {
            Email = command.Email,
            IconUrl = string.Empty,
            PortalUserId = command.PortalUserId,
            Status = command.AlreadyExistsInIdentity ? "Active" : "Invited",
            FirstName = command.FirstName,
            LastName = command.LastName,
            AzureADB2CUserId = command.AzureADB2CId ?? string.Empty
        };

        await _portalDbContext.AddAsync(entity);
        await _portalDbContext.SaveChangesAsync();
    }
}