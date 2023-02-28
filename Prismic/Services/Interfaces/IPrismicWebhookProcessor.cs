namespace apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Services.Interfaces;

public interface IPrismicWebhookProcessor
{
    Task ProcessPublishedAsync(string body, string apiKey);
    Task ProcessUnpublishedAsync(string body, string apiKey);
}