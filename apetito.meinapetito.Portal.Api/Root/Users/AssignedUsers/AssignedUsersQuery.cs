using apetito.CQS;
using apetito.meinapetito.Portal.Api.Root.Authentication.UserAccessTokens.Helpers;
using apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;
using HotChocolate;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.Root.Users.AssignedUsers;
[ExtendObjectType("Query")]
[Authorize]
public class AssignedUsersQuery
{
    public async Task<IList<RoleDto>> GetRolesOfAssignedUsersAsync(
        [Service] IQueryHandler<RetrieveRolesOfAssignedUsers, IList<RoleDto>> queryHandler)
        => await queryHandler.Execute(new RetrieveRolesOfAssignedUsers());
    [Authorize]
    public async Task<IList<AssignedUserDto>> GetAssignedUsersAsync(
        [Service] IQueryHandler<RetrieveAssignedUsers, IList<AssignedUserDto>> queryHandler
        , [Service] IHttpContextAccessor httpContextAccessor, GetAssignedUsersRequest request)
    {
        var userEmail = httpContextAccessor.HttpContext?.GetUserOrImpersonatedUserEmailAdressInGraphQl();
        
        return await queryHandler.Execute(new RetrieveAssignedUsers()
        {
            Email = userEmail,
            CustomerNumbers = request.CustomerNumbers
        });
    }
}