namespace apetito.meinapetito.Webhooks.AzureADB2C.Api.Models;

public class AzureAdb2CWebhookContent
{
    public string Email { get; set; }
    public string UserId { get; set; }
    public string Surname { get; set; }
    public string GivenName { get; set; }
}