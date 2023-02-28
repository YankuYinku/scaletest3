using apetito.CQS;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.Queries;
using apetito.meinapetito.Portal.Contracts.Root.Users.Current;

namespace apetito.meinapetito.Portal.Application.Root.Users.Current;

public class RetrieveCurrentUserQuery : UserQueryBase, IQuery<UserDto>
{
    public IList<string> Excludes { get; set; } = new List<string>();
}