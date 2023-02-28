using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using apetito.meinapetito.Webhooks.ArticleChanges.Application.Services.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace apetito.meinapetito.Webhooks.ArticleChanges.Api.CheckArticlesChanged;

public class CheckArticlesChangedHttpTrigger
{
    
    private readonly IApetitoArticlesAllergeneRelevantChangeDetector _apetitoProductsProvider;
    
    public CheckArticlesChangedHttpTrigger(IApetitoArticlesAllergeneRelevantChangeDetector apetitoProductsProvider)
    {
        _apetitoProductsProvider = apetitoProductsProvider;
    }
    
    [Function("CheckArticlesChangedHttpTrigger")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("CheckArticlesChangedHttpTrigger");
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        var result = await _apetitoProductsProvider.GetChangedProductsIdentifiersAsync(DateTime.MinValue);
        
        await response.WriteStringAsync(string.Join(", ",result));

        return response;
    }
}