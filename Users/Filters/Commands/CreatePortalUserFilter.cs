using apetito.CQS;
using apetito.meinapetito.Portal.Application.Root.Users.Filters.Const;

namespace apetito.meinapetito.Portal.Application.Root.Users.Filters.Commands;

public class CreatePortalUserFilter : ICommand
{
    public Guid Id { get; }
    public string Email { get; }
    public PortalUserFilterContext Context { get; }
    public string Name { get; }
    public string Value { get;  }

    public CreatePortalUserFilter(Guid id, string email, PortalUserFilterContext context, string name, string value)
    {
        Id = id;
        Email = email;
        Context = context;
        Name = name;
        Value = value;
    }
}