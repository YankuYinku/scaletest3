using apetito.CQS;

namespace apetito.meinapetito.Portal.Application.Root.Users.Filters.Commands;

public class RemovePortalUserFilter : ICommand
{
    public Guid FilterId { get; }
    public string Email { get; }

    public RemovePortalUserFilter(Guid filterId, string email)
    {
        FilterId = filterId;
        Email = email;
    }
}