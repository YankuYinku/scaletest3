using apetito.CQS;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands;

public class ResendInvitationForUser : ICommand
{
    public Guid UserId { get; }
    public string CurrentUserEmail { get; }
    public ResendInvitationForUser(Guid userId, string currentUserEmail)
    {
        UserId = userId;
        CurrentUserEmail = currentUserEmail;
    }
}