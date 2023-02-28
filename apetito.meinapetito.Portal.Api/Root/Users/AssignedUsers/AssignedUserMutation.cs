using apetito.CQS;
using apetito.meinapetito.Portal.Api.Root.Authentication.UserAccessTokens.Helpers;
using apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.Root.Users.AssignedUsers;

[ExtendObjectType("Mutation")]
public class AssignedUserMutation
{
    public async Task<int> InviteUser([FromServices] ICommandHandler<InviteUser> commandHandler,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        InviteUserRequest inviteUserRequest)
    {
        var userEmail = httpContextAccessor.HttpContext?.GetUserOrImpersonatedUserEmailAdressInGraphQl();

        await commandHandler.Handle(new InviteUser(inviteUserRequest.Email, inviteUserRequest.FirstName,
            inviteUserRequest.LastName, inviteUserRequest.CustomerNumberRoleAssignments,
            userEmail,inviteUserRequest.LanguageCode));

        return 0;
    }


    public async Task<int> RemoveUserAsync(
        [FromServices] ICommandHandler<RemoveCustomerNumberFromUserCommand> removeUserCommandHandler,
        RemoveCustomerNumberFromUserRequest request){
        await removeUserCommandHandler.Handle(
            new RemoveCustomerNumberFromUserCommand(request.UserId, request.CustomerNumbers));

        return 0;
    }

    public async Task<int> ChangeRoleAsync([FromServices] ICommandHandler<ChangeUserRole> changeRoleCommandHandler,
        ChangeUserRoleRequest request)
    {
        await changeRoleCommandHandler.Handle(new ChangeUserRole(request.UserId,
            request.Role, request.CustomerNumber));
        return 0;
    }

    public async Task<int> EditUserAsync([FromServices] IHttpContextAccessor httpContextAccessor,[FromServices] ICommandHandler<ChangeUserRelations> changeUserRelationHandler, 
        EditUserRequest request)
    {
        IList<CustomerNumberWithRoleRequest>
            list = request.CustomerNumberRoleAssignments ?? new List<CustomerNumberWithRoleRequest>();
        
        var userEmail = httpContextAccessor.HttpContext?.GetUserOrImpersonatedUserEmailAdressInGraphQl();
        
        IDictionary<int, string> customerNumbersWithRoles = new Dictionary<int, string>();

        foreach (var customerNumber in list)
        {
            customerNumbersWithRoles.Add(customerNumber.CustomerNumber,customerNumber.Role);
        }

        await changeUserRelationHandler.Handle(new ChangeUserRelations(request.UserId, customerNumbersWithRoles,userEmail));
        
        return 0;
    }

    public async Task<int> ResendInvitationAsync([FromServices] IHttpContextAccessor httpContextAccessor, [FromServices] ICommandHandler<ResendInvitationForUser> commandHandler,
        ResendInvitationForUserRequest request)
    {
        var userEmail = httpContextAccessor.HttpContext?.GetUserOrImpersonatedUserEmailAdressInGraphQl();

        await commandHandler.Handle(new ResendInvitationForUser(request.UserId,userEmail));

        return 0;
    }
}