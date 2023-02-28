using apetito.CQS;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands;

public class MarkUserAsActiveAndUpdateNamesFromAzureAdb2C : ICommand
{
    public string Email { get; }
    public string GivenName { get; }
    public string SurName { get; }
    public string UserId { get; }

    public MarkUserAsActiveAndUpdateNamesFromAzureAdb2C(string email, string givenName, string surName, string userId)
    {
        Email = email;
        GivenName = givenName;
        SurName = surName;
        UserId = userId;
    }
}