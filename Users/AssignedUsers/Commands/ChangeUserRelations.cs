using apetito.CQS;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands;

public class ChangeUserRelations : ICommand
{
    public string CurrentUserEmail { get; }
    public Guid UserId { get; }
    public IDictionary<int,string> Relations { get; }

    public ChangeUserRelations(Guid userId, IDictionary<int, string> relations, string currentUserEmail)
    {
        UserId = userId;
        Relations = relations;
        CurrentUserEmail = currentUserEmail;
    }
}