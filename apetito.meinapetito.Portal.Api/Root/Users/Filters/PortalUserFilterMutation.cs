using apetito.CQS;
using apetito.meinapetito.Portal.Api.Root.Authentication.UserAccessTokens.Helpers;
using apetito.meinapetito.Portal.Application.Root.Users.Filters.Commands;
using apetito.meinapetito.Portal.Application.Root.Users.Filters.Const;
using apetito.meinapetito.Portal.Contracts.Root.Users.Filters;
using HotChocolate;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.Root.Users.Filters;

[ExtendObjectType("Mutation")]
public class PortalUserFilterMutation
{
    public async Task<int> AddFilterAsync(
        [Service] ICommandHandler<CreatePortalUserFilter> commandHandler,
        [Service] IHttpContextAccessor httpContextAccessor,
        AddFilterRequest request)
    {
        var userEmail = httpContextAccessor.HttpContext?.GetUserOrImpersonatedUserEmailAdressInGraphQl();
        
        var newFilterId = Guid.NewGuid();

        await commandHandler.Handle(new CreatePortalUserFilter(newFilterId, userEmail,
            new PortalUserFilterContext(request.Context), request.Name, request.Value));

        return 0;
    }
    
    public async Task<int> EditFilterAsync(
        [Service] ICommandHandler<EditPortalUserFilter> commandHandler,
        [Service] IHttpContextAccessor httpContextAccessor,
        EditFilterRequest request)
    {
        var userEmail = httpContextAccessor.HttpContext?.GetUserOrImpersonatedUserEmailAdressInGraphQl();
        
        await commandHandler.Handle(new EditPortalUserFilter(request.FilterId, userEmail,request.Value,request.Name));

        return 0;
    }

    public async Task<int> DeleteFilterAsync(
        [Service] ICommandHandler<RemovePortalUserFilter> commandHandler,
        [Service] IHttpContextAccessor httpContextAccessor,
            DeleteFilterRequest request)
    {
        var userEmail = httpContextAccessor.HttpContext?.GetUserOrImpersonatedUserEmailAdressInGraphQl();
        
        await commandHandler.Handle(new RemovePortalUserFilter(request.FilterId, userEmail));

        return 0;
    }
}