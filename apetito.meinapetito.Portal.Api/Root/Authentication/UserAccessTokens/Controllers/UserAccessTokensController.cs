using apetito.CQS;
using apetito.meinapetito.Portal.Api.Root.Authentication.UserAccessTokens.Helpers;
using apetito.meinapetito.Portal.Application.Root.Authentication.UserAccessTokens;
using apetito.meinapetito.Portal.Application.Root.Authentication.UserAccessTokens.BearerToken.Queries;
using apetito.meinapetito.Portal.Application.Root.Authentication.UserAccessTokens.IbsscToken.Queries;
using apetito.meinapetito.Portal.Contracts.Root.Authentication.UserAccessToken;
using Microsoft.Toolkit.Diagnostics;

namespace apetito.meinapetito.Portal.Api.Root.Authentication.UserAccessTokens.Controllers;

[Authorize]
[ApiController]
[Route("root/authentication/userAccessTokens")]
public class UserAccessTokensController : ControllerBase
{

    public UserAccessTokensController(ILogger<UserAccessTokensController> logger)
    {
        Guard.IsNotNull(logger, nameof(logger));
    }

    [HttpGet("legacy")]
    public async Task<IActionResult> GetLegacyBearerToken(
        [FromServices] IQueryHandler<RetrieveUserAccessBearerTokenByUserEmailQuery, UserAccessBearerTokenDto> userAccessBearerTokenQuery)
    {
        var mailAddress = User.GetUserOrImpersonatedUserEmailAdress();

        var userAccessBearerToken =
            await userAccessBearerTokenQuery.Execute(new RetrieveUserAccessBearerTokenByUserEmailQuery() 
                { UserEmail = mailAddress });

        return Ok(userAccessBearerToken);
    }

    [HttpGet("ibssc")]
    public async Task<IActionResult> GetIbsscToken(
        [FromServices] IQueryHandler<RetrieveUserAccessIbsscTokenByUserEmailQuery, UserAccessIbsscTokenDto> userAccessIbsscTokenQuery)
    {
        var mailAddress = User.GetUserOrImpersonatedUserEmailAdress();

        var userAccessIbsscToken =
            await userAccessIbsscTokenQuery.Execute(new RetrieveUserAccessIbsscTokenByUserEmailQuery()
                {UserEmail = mailAddress});

        return Ok(userAccessIbsscToken);
    }
}
