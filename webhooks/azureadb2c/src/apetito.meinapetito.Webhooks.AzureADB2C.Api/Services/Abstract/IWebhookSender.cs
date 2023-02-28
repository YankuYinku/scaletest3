namespace apetito.meinapetito.Webhooks.AzureADB2C.Api.Services.Abstract;

public interface IWebhookSender
{
    Task SendWebhookToMeinApetitoAsync(AzureAdB2CUser user);
}