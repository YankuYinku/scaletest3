using System.Text;
using apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Services.Interfaces;

namespace apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Services.Implementations;

public class PrismicWebhookProcessor : IPrismicWebhookProcessor
{
    private readonly ILogger<PrismicWebhookProcessor> _logger;

    public PrismicWebhookProcessor(ILogger<PrismicWebhookProcessor> logger)
    {
        _logger = logger;
    }

    public async Task ProcessPublishedAsync(string body, string apiKey)
        => await ProcessAsync(body, apiKey, PublishedAction);

    public async Task ProcessUnpublishedAsync(string body, string apiKey)
        => await ProcessAsync(body, apiKey, UnpublishedAction);


    private async Task ProcessAsync(string body, string apiKey, string action)
    {
        _logger.LogWarning($"handled body : {body}, handled secret: {apiKey}, handled action: {action}");
        
        var urlsToNotify = UrlsToNotify(action);
        
        foreach (var url in urlsToNotify)
        {
            await SendRequestAsync(body, apiKey, url);
        }
    }
    
    private async Task SendRequestAsync(string body, string secret, string url)
    {
        try
        {
            using var httpHandler = new HttpClientHandler();

            httpHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            using var httpClient = new HttpClient(httpHandler);

            httpClient.DefaultRequestHeaders.Add("x-api-key", secret);

            _logger.Log(LogLevel.Information, $"Sending request for : {url}");

            var result = await httpClient.PostAsync(url,
                new StringContent(body, Encoding.UTF8, "application/json"));

            _logger.Log(LogLevel.Information, $"{result.StatusCode} {result.IsSuccessStatusCode} success : {url}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex?.Message);
        }
    }

    private const string PublishedAction = "reload";
    private const string UnpublishedAction = "reload-deleted";

    public static IList<string> UrlsToNotify(string action) => new List<string>()
    {
        $"http://portal-cache-downloads/downloads/{action}",
        $"http://portal-cache-news/news/{action}",
        $"http://portal-cache-faqs/faqs/{action}",
    };
}