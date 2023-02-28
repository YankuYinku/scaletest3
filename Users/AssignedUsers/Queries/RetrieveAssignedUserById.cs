using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries;

public class RetrieveAssignedUserById : IQuery<AssignedUserDto?>
{
    public Guid Id { get; set; }
}