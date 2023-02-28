namespace apetito.meinapetito.Webhooks.ArticleChanges.Application.Services.Interface
{
    public interface IApetitoArticlesAllergeneRelevantChangeDetector
    {
        Task<IList<string>> GetChangedProductsIdentifiersAsync(DateTime lastCheckDate);
    }
}