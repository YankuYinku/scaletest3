using apetito.meinapetito.Portal.Application.Dashboard.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.Dashboard.Models;
using HotChocolate;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.Dashboard.Queries
{
    [ExtendObjectType("Query")]
    public class SliderQuery
    {
        public async Task<IList<DashboardArticleDto>> GetSliderProductsAsync(
            [Service] ISliderProductDataProvider sliderProductDataProvider,
            [Service] IDashboardSliderProductsServiceClient dashboardSliderProductsServiceClient
            )
        {
            var result = await sliderProductDataProvider.GetSliderDataAsync();

            var sliderArticles = await dashboardSliderProductsServiceClient.GetProductsAsync(
                new GetProductsRequestModel
                {
                    ArticleIds = result.Select(z => z.Value.ArticleNumber).ToList()
                });

            foreach (var sliderArticle in sliderArticles)
            {
                if (sliderArticle.Id is null)
                {
                    throw new InvalidProgramException("slider article id cannot be empty");
                }

                if (sliderArticle.Details is null)
                {
                    throw new InvalidProgramException("slider article details cannot be empty");
                }
                sliderArticle.Details.OldPrice = result[sliderArticle.Id].OldPrice;
            }
            
            return sliderArticles;
        }
    }
}