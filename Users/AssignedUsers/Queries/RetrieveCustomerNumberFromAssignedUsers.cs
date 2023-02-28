using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.Current;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries;

public class RetrieveCustomerNumberFromAssignedUsers : IQuery<IList<CustomerDto>>
{
    public string Email { get; set; }
}