using apetito.CQS;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands;

public class SendMailToUserAboutInvitationToCustomerNumbers : ICommand
{
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public string InvitatorEmail { get; }
    public bool WasAlreadyInAzureADB2C { get; }
    public string LanguageCode { get; }

    public SendMailToUserAboutInvitationToCustomerNumbers(string firstName, string lastName, string email, string invitatorEmail, bool wasAlreadyInAzureAdb2C, string languageCode)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        InvitatorEmail = invitatorEmail;
        WasAlreadyInAzureADB2C = wasAlreadyInAzureAdb2C;
        LanguageCode = languageCode;
    }
}