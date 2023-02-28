using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.IntegrationFields;

namespace apetito.meinapetito.Portal.Api.Prismic.IntegrationFields
{
    [Route("integrationFields")]
    public class IntegrationFieldsController : ControllerBase
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ISortimentsProvider _sortimentsProvider;

        public IntegrationFieldsController(IHttpClientFactory httpClientFactory, ISortimentsProvider sortimentsProvider)
        {
            this.httpClientFactory = httpClientFactory;
            _sortimentsProvider = sortimentsProvider;
        }

        [HttpGet("sortiments")]
        public async Task<IActionResult> Sortiments([FromQuery] int page)
        {
            var result = await _sortimentsProvider.RetrieveAsync();

            var integrationFieldResult = result.Skip((page - 1) * 50).Take(50).Select(s => new SortimentIntegrationFieldEntry() 
            {
                Id = s.Code,
                Title = s.Code,
                Description = s.Beschreibung,
                Blob = s
            });

            return Ok(new SortimentIntegrationFieldEntries() {
                Results_size = result.Count(),
                Results = integrationFieldResult.ToList()
            });
        }
        
    }

}