using apetito.CQS;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Queries;
using apetito.Notification.Contracts.Models.Message;

namespace apetito.meinapetito.Portal.Api.Root.Infrastructure.Email.Controllers;

[ApiController]
[Route("/root/infrastructure/email")]
public class EmailController : ControllerBase
{
    [HttpGet("{referenceId}")]
    public async Task<IActionResult> GetEmail([FromServices] IQueryHandler<RetrieveEmailToSendQuery, Notification.Contracts.Models.Message.SendGrid> emailToSendQuery, string referenceId)
    {
        var result = await emailToSendQuery.Execute(new RetrieveEmailToSendQuery {ReferenceId = referenceId});

        if (string.IsNullOrWhiteSpace(result.Sender))
            return NotFound();
    
        return Ok(result);

    }
}