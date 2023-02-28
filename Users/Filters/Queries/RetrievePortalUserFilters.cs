using apetito.CQS;
using apetito.meinapetito.Portal.Application.Root.Users.Filters.Const;
using apetito.meinapetito.Portal.Contracts.Root.Users.Filters;

namespace apetito.meinapetito.Portal.Application.Root.Users.Filters.Queries;

public class RetrievePortalUserFilters : IQuery<IEnumerable<PortalUserFilterDto>>
{
    public PortalUserFilterContext Context { get; set; }
    public string Email { get; set; }
}