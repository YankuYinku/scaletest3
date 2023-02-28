using apetito.CQS;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands;

public class CreateUser : ICommand
{
    public Guid PortalUserId { get; }
    public string Email { get; }
    public bool AlreadyExistsInIdentity { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string? AzureADB2CId { get; }

    public CreateUser(Guid portalUserId, string email, string firstName, string lastName)
    {
        PortalUserId = portalUserId;
        Email = email;
        AlreadyExistsInIdentity = false;
        FirstName = firstName;
        LastName = lastName;
        AzureADB2CId = null;
    }
    
    public CreateUser(Guid portalUserId, string email, string firstName, string lastName, string azureAdb2CId)
    {
        PortalUserId = portalUserId;
        Email = email;
        AlreadyExistsInIdentity = true;
        FirstName = firstName;
        LastName = lastName;
        AzureADB2CId = azureAdb2CId;
    }
}