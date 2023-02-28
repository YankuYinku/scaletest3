using apetito.meinapetito.Portal.Application.Faqs.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.Faqs.Models;
using HotChocolate;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.Faqs.Queries;

[ExtendObjectType("Query")]
public class FaqsQuery
{
    public async Task<GetFaqsItemsResult> GetFaqsAsync(
        [Service] IFaqsItemProvider faqsItemProvider, FaqsQueryRequest request)
    {
        return (await faqsItemProvider.GetAsync(new Contracts.Faqs.Models.FaqsQuery()
        {
            LanguageCode = string.IsNullOrWhiteSpace(request.LanguageCode) ? "de-de" : request.LanguageCode,
            Sortiments = request.Sortiments ?? new List<string>(),
            OrderSystems = request.OrderSystems ?? new List<string>(),
            SaleChannels = request.SaleChannels ?? new List<string>()
        }));
    }
    
    public async Task<GetFaqsItemsResult> GetFaqsByIdAsync(
        [Service] IFaqsItemProvider faqsItemProvider, FaqsQueryRequestById request)
    {
        return (await faqsItemProvider.GetByIdAsync(request.Id,new Contracts.Faqs.Models.FaqsQuery()
        {
            Sortiments = request.Sortiments ?? new List<string>(),
            OrderSystems = request.OrderSystems ?? new List<string>(),
            SaleChannels = request.SaleChannels ?? new List<string>()
        }));
    }
    
    
    public async Task<GetFaqsItemsResult> GetFaqsBySlugAsync(
        [Service] IFaqsItemProvider faqsItemProvider, FaqsQueryRequestBySlug request)
    {
        return (await faqsItemProvider.GetBySlugAsync(request.Slug,new Contracts.Faqs.Models.FaqsQuery()
        {
            
            Sortiments = request.Sortiments ?? new List<string>(),
            OrderSystems = request.OrderSystems ?? new List<string>(),
            SaleChannels = request.SaleChannels ?? new List<string>()
        }));
    }
}