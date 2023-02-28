using System.Collections.Generic;
using System.Net;
using apetito.meinapetito.Webhooks.ArticleChanges.Application.Services.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace apetito.meinapetito.Webhooks.ArticleChanges.Api;

public class HealthCheck
{
    [Function("HealthCheck")]
    public  HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("HealthCkeck");
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString("Welcome to Azure Functions works!");

        return response;
        
    }
}