using apetito.CQS;
using apetito.meinapetito.Portal.Api.Root.Authentication.UserAccessTokens.Helpers;
using apetito.meinapetito.Portal.Application.Root.Users.Filters.Const;
using apetito.meinapetito.Portal.Application.Root.Users.Filters.Queries;
using apetito.meinapetito.Portal.Contracts.Root.Users.Filters;
using HotChocolate;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.Root.Users.Filters;

[ExtendObjectType("Query")]
public class PortalUserFilterQuery
{
    public async Task<IEnumerable<PortalUserFilterDto>> RetrievePortalUserFiltersAsync(
        [Service] IQueryHandler<RetrievePortalUserFilters,IEnumerable<PortalUserFilterDto>> portalUserFilterQueryHandler,
        [Service] IHttpContextAccessor httpContextAccessor,RetrievePortalUserFiltersRequest request )
    {
        var userEmail = httpContextAccessor.HttpContext?.GetUserOrImpersonatedUserEmailAdressInGraphQl();

        var result = await portalUserFilterQueryHandler.Execute(new RetrievePortalUserFilters
        {
            Context = new PortalUserFilterContext(request.Context),
            Email = userEmail
        });

        return result;
    }
}