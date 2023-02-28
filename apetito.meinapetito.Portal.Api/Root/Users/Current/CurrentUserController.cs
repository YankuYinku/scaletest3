using apetito.CQS;
using apetito.meinapetito.Portal.Api.Root.Authentication.UserAccessTokens.Helpers;
using apetito.meinapetito.Portal.Application.Root.Users.Current;
using apetito.meinapetito.Portal.Contracts.Root.Users.Current;

namespace apetito.meinapetito.Portal.Api.Root.Users.Current;

[Authorize]
[ApiController]
[Route("/root/users/current")]
public class CurrentUserController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> RetrieveCurrentUser([FromQuery(Name = "excludes[]")]string[]? excludes,
        [FromServices] IQueryHandler<RetrieveCurrentUserQuery,UserDto> queryHandler)
    {
        
        
        var mailAddress = User.GetUserOrImpersonatedUserEmailAdress();
        var retrieveCustomerNumbersResult =
            await queryHandler
                .Execute(new RetrieveCurrentUserQuery
                {
                    UserEmail = mailAddress,
                    Excludes = excludes?.ToList() ?? new List<string>()
                }); //after NKA process we need to switch to b2c sub claim! 
                    
        return Ok(retrieveCustomerNumbersResult);
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("exception")]
    public async Task<IActionResult> GetException(string exceptionMessage)
    {
        throw new Exception(exceptionMessage);
    }
}