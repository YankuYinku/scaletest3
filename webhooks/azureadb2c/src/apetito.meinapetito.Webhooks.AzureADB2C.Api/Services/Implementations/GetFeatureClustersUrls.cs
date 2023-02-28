using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apetito.meinapetito.Webhooks.AzureADB2C.Api.Services.Abstract;
using apetito.meinapetito.Webhooks.AzureADB2C.Api.Services.Options;
using Azure.Identity;
using Microsoft.Graph;
using Newtonsoft.Json;

namespace apetito.meinapetito.Webhooks.AzureADB2C.Api.Services.Implementations;

public class GetFeatureClustersUrls : IGetFeatureClustersUrls
{
    private readonly GraphApiOptions _options;
    private readonly string[] _scopes = new[] {"https://graph.microsoft.com/.default"};
    public GetFeatureClustersUrls(GraphApiOptions graphApiOptions)
    {
        _options = graphApiOptions;
    }

    public async Task<IList<string>> GetFeatureClustersUrlsAsync()
    {
        var graphClient = PrepareAzureGraphClient();

        var request = await graphClient.Applications.Request().Top(100)
            .Select(a => new
            {
                a.Id,
                a.Web,
            })
            .GetAsync();

        var allWebs = request.CurrentPage.ToList();
        
        var nextPageRequest = request.NextPageRequest;

        while (nextPageRequest is not null)
        {
            var result = await nextPageRequest.GetAsync();

            allWebs.AddRange(result.CurrentPage.ToList());

            nextPageRequest = result.NextPageRequest;
        }

        var app = allWebs.FirstOrDefault(a => a.Id == _options.ApplicationId);
        
        return app.Web.RedirectUris.ToList();
    }

    private GraphServiceClient PrepareAzureGraphClient()
    {
        var tenantId = _options.TenantId;
        var clientId = _options.ClientId;
        var clientSecret = _options.ClientSecret;

        var options = new TokenCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        };

        var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);

        var token = new TokenCredentialAuthProvider(clientSecretCredential, _scopes);

        var graphClient = new GraphServiceClient(token);
        
        return graphClient;
    }
}