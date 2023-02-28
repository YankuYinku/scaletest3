using apetito.CQS;
using apetito.meinapetito.Portal.Api.Infrastructure;
using apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands;
using apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Options;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;

namespace apetito.meinapetito.Portal.Api.Root.Users.AssignedUsers;

[ApiController]
[Route("/root/users/assigned")]
public class AssignedUsersController : ControllerBase
{
    private readonly ICommandHandler<MarkUserAsActiveAndUpdateNamesFromAzureAdb2C> _markUserAsActiveCommandHandler;
    private readonly ILogger<AssignedUsersController> _logger;
    public AssignedUsersController(ICommandHandler<MarkUserAsActiveAndUpdateNamesFromAzureAdb2C> markUserAsActiveCommandHandler, ILogger<AssignedUsersController> logger)
    {
        _markUserAsActiveCommandHandler = markUserAsActiveCommandHandler;
        _logger = logger;
    }

    [AuthorizeApiKey(typeof(PostRegistrationWebhookOptions))]
    [HttpPut("{email}/activate")]
    public async Task<IActionResult> ActivateAsync(string email, [FromBody] AzureAdb2CWebhookContent body)
    {
        _logger.LogInformation($"Received email: {email}");

        await _markUserAsActiveCommandHandler.Handle(new MarkUserAsActiveAndUpdateNamesFromAzureAdb2C(email, body.GivenName, body.Surname, body.UserId));

        return NoContent();
    }
}