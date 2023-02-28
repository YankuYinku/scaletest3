using apetito.CQS;

namespace apetito.meinapetito.Portal.Application.Root.Users.Filters.Commands;

public class EditPortalUserFilter : ICommand
{
    public Guid Id { get; }
    public string Email { get; }
    public string Name { get; }
    public string Value { get; }

    public EditPortalUserFilter(Guid id, string email, string value, string name)
    {
        Id = id;
        Email = email;
        Value = value;
        Name = name;
    }
}