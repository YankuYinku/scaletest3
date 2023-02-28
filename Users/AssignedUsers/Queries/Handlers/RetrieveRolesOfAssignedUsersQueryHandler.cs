using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries.Handlers;

public class RetrieveRolesOfAssignedUsersQueryHandler : IQueryHandler<RetrieveRolesOfAssignedUsers,
    IList<RoleDto>>
{
    public Task<IList<RoleDto>> Execute(RetrieveRolesOfAssignedUsers query)
    {
        IList<RoleDto> roleDtos = new List<RoleDto>()
        {
            new RoleDto(){Name = "Administrator"},
            new RoleDto(){Name = "Orderer"}
        };

        return Task.FromResult(roleDtos);
    }
}