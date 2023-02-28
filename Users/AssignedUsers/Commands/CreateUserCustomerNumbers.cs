using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands;

public class CreateUserCustomerNumbers : ICommand
{
    public CreateUserCustomerNumbers(IDictionary<int, CustomerNumberDetails> customerNumberWithRoles, Guid userId)
    {
        CustomerNumberWithRoles = customerNumberWithRoles;
        UserId = userId;
    }

    public IDictionary<int, CustomerNumberDetails> CustomerNumberWithRoles { get; }
    public Guid UserId { get; }
}