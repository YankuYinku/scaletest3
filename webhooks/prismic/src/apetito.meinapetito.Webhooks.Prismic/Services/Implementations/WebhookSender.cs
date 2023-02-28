using System.Text;
using apetito.meinapetito.Webhooks.Prismic.Const;
using apetito.meinapetito.Webhooks.Prismic.Options;
using apetito.meinapetito.Webhooks.Prismic.Services.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace apetito.meinapetito.Webhooks.Prismic.Services.Implementations;

public class WebhookSender : IWebhookSender
{
    private readonly IConfiguration _configuration;
    private readonly IGetFeatureClustersUrls _getFeatureClustersUrls;
    private readonly ILogger<WebhookSender> _logger;
    private readonly MeinapetitoAccessOptions _meinapetitoAccessOptions;

    public WebhookSender(IConfiguration configuration, IGetFeatureClustersUrls getFeatureClustersUrls,
        ILogger<WebhookSender> logger, MeinapetitoAccessOptions meinapetitoAccessOptions)
    {
        _configuration = configuration;
        _getFeatureClustersUrls = getFeatureClustersUrls;
        _logger = logger;
        _meinapetitoAccessOptions = meinapetitoAccessOptions;
    }

    public Task SendWebhookToMeinApetitoAsync(string webhookContent, ChangeContext changeContext)
    {
        var environment = _configuration["ASPNETCORE_ENVIRONMENT"];
        Console.WriteLine("Current environment: " + environment);

        return environment switch
        {
            "Production" => SendForProduction(webhookContent, changeContext),
            "Staging" => SendForStaging(webhookContent, changeContext),
            _ => SendForDevelopment(webhookContent, changeContext)
        };
    }

    private async Task SendForProduction(string webhookContent, ChangeContext changeContext)
    {
        string productionUrl = _meinapetitoAccessOptions.BaseUrl;
        string secret = _meinapetitoAccessOptions.Secret;
        await SendRequest(productionUrl, secret, webhookContent, changeContext);
    }

    private async Task SendForStaging(string webhookContent, ChangeContext changeContext)
    {
        string stagingUrl = _meinapetitoAccessOptions.BaseUrl;
        string secret = _meinapetitoAccessOptions.Secret;
        await SendRequest(stagingUrl, secret, webhookContent, changeContext);
    }

    private async Task SendForDevelopment(string webhookContent, ChangeContext changeContext)
    {
        var secret = _meinapetitoAccessOptions.Secret;

        var urls = await _getFeatureClustersUrls.GetFeatureClustersUrlsAsync();

        var fcUrls = urls.Where(a => a.Contains("feature"));

        foreach (var fcUrl in fcUrls)
        {
            var splittedValues = fcUrl.Split(BaseUrlSplitter);

            if (splittedValues.Length <= 1)
            {
                continue;
            }

            string fcBaseUrl = splittedValues[0];

            await SendRequest(fcBaseUrl, secret, webhookContent, changeContext);
        }
    }

    private async Task SendRequest(string baseUrl, string secret, string webhookContent,
        ChangeContext changeContext)
    {
        try
        {
            using var httpHandler = new HttpClientHandler();

            httpHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            using var httpClient = new HttpClient(httpHandler);

            httpClient.DefaultRequestHeaders.Add("x-api-key", secret);

            string action = changeContext switch
            {
                ChangeContext.DocumentPublished => "reload",
                _ => "reload-deleted"
            };

            var url = UrlToNotify(baseUrl, action);
            
            _logger.Log(LogLevel.Information, $"Sending request for : {url}");

            var result = await httpClient.PostAsync(url,
                    new StringContent(webhookContent, Encoding.UTF8, "application/json"));

            _logger.Log(LogLevel.Information, $"{result.StatusCode} {result.IsSuccessStatusCode} success : {url}");
            
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
        }
    }

    public static string UrlToNotify(string baseUrl, string action) => $"{baseUrl}/api/webhooks/{action}";

    private const string BaseUrlSplitter = "/api/swagger/oauth2-redirect";
}