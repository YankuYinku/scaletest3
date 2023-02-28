using System.Collections.Generic;
using System.Threading.Tasks;

namespace apetito.meinapetito.Webhooks.AzureADB2C.Api.Services.Abstract;

public interface IGetFeatureClustersUrls
{
    Task<IList<string>> GetFeatureClustersUrlsAsync();
}