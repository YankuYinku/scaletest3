using System.Net;
using System.Text;
using apetito.meinapetito.Webhooks.AzureADB2C.Api.Models;
using apetito.meinapetito.Webhooks.AzureADB2C.Api.Services.Abstract;
using apetito.meinapetito.Webhooks.AzureADB2C.Api.Services.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace apetito.meinapetito.Webhooks.AzureADB2C.Api.Services.Implementations;

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
    public Task SendWebhookToMeinApetitoAsync(AzureAdB2CUser user)
    {
        var environment = _configuration["ASPNETCORE_ENVIRONMENT"];
        Console.WriteLine("Current environment: " + environment);

        return environment switch
        {
            "Production" => SendForProduction(user),
            "Staging" => SendForStaging(user),
            _ => SendForDevelopment(user)
        };
    }

    private async Task SendForProduction(AzureAdB2CUser user)
    {
        string productionUrl = _meinapetitoAccessOptions.BaseUrl;
        string secret = _meinapetitoAccessOptions.Secret;
        await SendRequest(productionUrl, secret, user);
    }

    private async Task SendForStaging(AzureAdB2CUser user)
    {
        string stagingUrl = _meinapetitoAccessOptions.BaseUrl;
        string secret = _meinapetitoAccessOptions.Secret;
        await SendRequest(stagingUrl, secret, user);
    }

    private async Task SendForDevelopment(AzureAdB2CUser user)
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

            var result = await SendRequest(fcBaseUrl, secret, user);

            if (result)
            {
                break;
            }
        }
    }

    private async Task<bool> SendRequest(string baseUrl, string secret, AzureAdB2CUser user)
    {
        try
        {
            using var httpHandler = new HttpClientHandler();

            httpHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            using var httpClient = new HttpClient(httpHandler);

            httpClient.DefaultRequestHeaders.Add("x-api-key", secret);

            string url = $"{baseUrl}/api/root/users/assigned/{user.Email}/activate";

            _logger.Log(LogLevel.Information, $"Sending request for : {url}");

            var result = await httpClient.PutAsync(url,
                new StringContent(JsonConvert.SerializeObject(new AzureAdb2CWebhookContent
                {
                    Email = user.Email,
                    GivenName = user.GivenName,
                    Surname = user.Surname,
                    UserId = user.Client_Id
                }), Encoding.UTF8, "application/json"));

            _logger.Log(LogLevel.Information, $"{result.StatusCode} {result.IsSuccessStatusCode} success : {url}");

            return result.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex?.Message);
            return false;
        }
    }

    private const string BaseUrlSplitter = "/api/swagger/oauth2-redirect";
}