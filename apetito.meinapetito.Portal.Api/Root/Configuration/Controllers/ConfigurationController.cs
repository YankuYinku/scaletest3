using apetito.CQS;
using apetito.meinapetito.Portal.Application.Root.Configuration.Queries;
using apetito.meinapetito.Portal.Contracts.Root.Configuration;
using Microsoft.FeatureManagement;

namespace apetito.meinapetito.Portal.Api.Root.Configuration.Controllers
{
    [Authorize]
    [Route("root/configuration")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IFeatureManagerSnapshot _featureManager;

        public ConfigurationController(IConfiguration configuration, IFeatureManagerSnapshot featureManager)
        {
            _configuration = configuration;
            _featureManager = featureManager;
        }

        [HttpGet("features")]
        public async Task<IActionResult> GetFeatureFlags(
            [FromServices] IQueryHandler<RetrieveFeatureFlagNamesQuery,FeatureFlagNamesDto> queryHandler)
        {
            return Ok(await queryHandler.Execute(new RetrieveFeatureFlagNamesQuery()));
        }

        [HttpGet("features/{key}")]
        public async Task<IActionResult> GetFeatureFlag([FromRoute] string key)
        {
            var value = await _featureManager.IsEnabledAsync(key);

            return Ok(new { feature = key, value = value});
        }
    }
}