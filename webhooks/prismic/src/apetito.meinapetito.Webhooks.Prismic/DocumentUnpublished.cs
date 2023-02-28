using System.Net;
using apetito.meinapetito.Webhooks.Prismic.Const;
using apetito.meinapetito.Webhooks.Prismic.Services.Abstract;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace apetito.meinapetito.Webhooks.Prismic;

public class DocumentUnpublished
{
    private readonly IWebhookSender _webhookSender;

    public DocumentUnpublished(IWebhookSender webhookSender)
    {
        _webhookSender = webhookSender;
    }

    [Function("DocumentUnpublished")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("DownloadPublished");
        logger.LogInformation("C# HTTP trigger function processed a request.");
        
        StreamReader sr = new StreamReader(req.Body);

        string content = await sr.ReadToEndAsync();
        logger.LogInformation($"Received body: {content}");
        
        await _webhookSender.SendWebhookToMeinApetitoAsync(content, ChangeContext.DocumentUnpublished);
        
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString("Welcome to Azure Functions!");

        return response;
        
    }
}