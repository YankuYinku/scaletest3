using apetito.meinapetito.Portal.Api.Infrastructure;
using apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Options;
using apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Services.Interfaces;

namespace apetito.meinapetito.Portal.Api.Prismic.Webhook;
[Route("webhooks")]
public class WebhookController : ControllerBase
{
    private readonly IPrismicWebhookProcessor _prismicWebhookProcessor;
    private readonly PrismicWebhookOptions _prismicWebhookOptions;

    public WebhookController(PrismicWebhookOptions prismicWebhookOptions,
        IPrismicWebhookProcessor prismicWebhookProcessor)
    {
        _prismicWebhookOptions = prismicWebhookOptions;
        _prismicWebhookProcessor = prismicWebhookProcessor;
    }

    [AuthorizeApiKey(typeof(PrismicWebhookOptions))]
    [HttpPost("reload")]
    public async Task<IActionResult> ProcessAsync()
    {
        AuthorizeApiKeyAttribute.GetApiKey(HttpContext, out var apiKey);
        
        string content = await ReadBodyAsync();

        await _prismicWebhookProcessor.ProcessPublishedAsync(content, apiKey);
        
        return Ok();
    }

    [AuthorizeApiKey(typeof(PrismicWebhookOptions))]
    [HttpPost("reload-deleted")]
    public async Task<IActionResult> ProcessDeletedAsync()
    {
        AuthorizeApiKeyAttribute.GetApiKey(HttpContext, out var apiKey);
        var content = await ReadBodyAsync();

        await _prismicWebhookProcessor.ProcessUnpublishedAsync(content, apiKey);

        return Ok();
    }

    private async Task<string> ReadBodyAsync()
    {
        return await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
    }

  
}