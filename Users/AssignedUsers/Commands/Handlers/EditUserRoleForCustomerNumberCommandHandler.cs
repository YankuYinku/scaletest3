using apetito.CQS;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands.Handlers;

public class EditUserRoleForCustomerNumberCommandHandler : ICommandHandler<EditUserRoleForCustomerNumber>
{
    public Task Handle(EditUserRoleForCustomerNumber command)
    {
        return Task.CompletedTask;
    }
}