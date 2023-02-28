using apetito.CQS;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands;

public class EditUserRoleForCustomerNumber : ICommand
{
    public string Role { get; set; }
    public Guid UserId { get; set; }
    public int CustomerNumber { get; set; }
}