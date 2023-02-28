using System;
using System.Threading.Tasks;
using apetito.meinapetito.Webhooks.ArticleChanges.Application.Services.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace apetito.meinapetito.Webhooks.ArticleChanges.Api.CheckArticlesChanged;

public class CheckArticlesChangedTrigger
{

    private readonly IApetitoArticlesAllergeneRelevantChangeDetector _apetitoProductsProvider;
    
    public CheckArticlesChangedTrigger(IApetitoArticlesAllergeneRelevantChangeDetector apetitoProductsProvider)
    {
        _apetitoProductsProvider = apetitoProductsProvider;
    }

    [Function("CheckArticlesChangedTrigger")]
    public async Task Run([TimerTrigger("0 * 8 * * *")] TimerTriggerStatus myTimer, FunctionContext context)
    {
        var lastExecuted = myTimer.ScheduleStatus.Last;
        
        var logger = context.GetLogger("CheckArticlesChangedTrigger");
        logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        
        var changedItems = await _apetitoProductsProvider.GetChangedProductsIdentifiersAsync(lastExecuted);
        
        logger.LogCritical(JsonConvert.SerializeObject(changedItems));
    }
}
