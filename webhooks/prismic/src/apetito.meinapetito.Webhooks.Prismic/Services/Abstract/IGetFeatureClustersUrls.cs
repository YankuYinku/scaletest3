namespace apetito.meinapetito.Webhooks.Prismic.Services.Abstract;

public interface IGetFeatureClustersUrls
{
    Task<IList<string>> GetFeatureClustersUrlsAsync();
}