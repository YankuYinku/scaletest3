using apetito.CQS;
using apetito.meinapetito.Portal.Application.Infrastructure.Languages;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries;

public record RetrieveFilledEmailToInviteUser : IQuery<EmailDto>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set;} = string.Empty;
    public string Email { get; set;} = string.Empty;
    public string InvitatorEmail { get; set;} = string.Empty;
    public bool WasAlreadyInAzureADB2C { get; set;} 
    public string LanguageCode { get; set; } = Languages.German;
}