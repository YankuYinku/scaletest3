using apetito.meinapetito.Webhooks.Prismic.Const;

namespace apetito.meinapetito.Webhooks.Prismic.Services.Abstract;

public interface IWebhookSender
{
    Task SendWebhookToMeinApetitoAsync(string webhookContent, ChangeContext changeContext);
}