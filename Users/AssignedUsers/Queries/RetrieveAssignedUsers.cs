using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries;

public class RetrieveAssignedUsers : IQuery<IList<AssignedUserDto>>
{
    public string Email { get; set; }
    public IList<int> CustomerNumbers { get; set; }
}