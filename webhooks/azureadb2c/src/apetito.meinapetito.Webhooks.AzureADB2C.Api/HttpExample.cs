using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace apetito.meinapetito.Webhooks.AzureADB2C.Api
{
    public class HttpExample
    {
        private readonly ILogger _logger;

        public HttpExample(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpExample>();
        }

        [Function("HttpExample")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            string toReturn = "";
            
            var servicePrincipalClientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            
            var subscriptionTenantId = 
                Environment.GetEnvironmentVariable("AZURE_TENANT_ID")
                    ;
            
            toReturn+=($"AZURE_CLIENT_ID : {servicePrincipalClientId}");
            toReturn+=($"AZURE_TENANT_ID : {subscriptionTenantId}");
            
            toReturn+=($"environment : {environment}");

            response.WriteString(toReturn);
            
            return response;
        }
    }
}
