using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands;

public class CreateUserCustomerNumbersSortiments : ICommand
{
    public Guid PortalUserCustomerNumbersEntryId { get; }
    public Guid PortalUserReferenceId { get; }
    public CustomerNumberDetails CustomerNumberDetails { get; }

    public CreateUserCustomerNumbersSortiments(Guid portalUserCustomerNumbersEntryId, Guid portalUserReferenceId, CustomerNumberDetails customerNumberDetails)
    {
        PortalUserCustomerNumbersEntryId = portalUserCustomerNumbersEntryId;
        PortalUserReferenceId = portalUserReferenceId;
        CustomerNumberDetails = customerNumberDetails;
    }
}