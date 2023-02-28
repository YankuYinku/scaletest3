using apetito.CQS;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands;

public class ChangeUserRole : ICommand
{
    public Guid UserId { get; }
    public string Role { get; }
    public int CustomerNumber { get; }

    public ChangeUserRole(Guid userId, string role, int customerNumber)
    {
        UserId = userId;
        Role = role;
        CustomerNumber = customerNumber;
    }
}