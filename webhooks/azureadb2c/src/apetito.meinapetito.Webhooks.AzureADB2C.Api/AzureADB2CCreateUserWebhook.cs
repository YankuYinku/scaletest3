using System.Net;
using apetito.meinapetito.Webhooks.AzureADB2C.Api.Services.Abstract;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace apetito.meinapetito.Webhooks.AzureADB2C.Api
{
    public class AzureAdb2CCreateUserWebhook
    {
        private readonly ILogger _logger;
        private readonly IWebhookSender _webhookSender;
        public AzureAdb2CCreateUserWebhook(ILoggerFactory loggerFactory, IWebhookSender webhookSender)
        {
            _webhookSender = webhookSender;
            _logger = loggerFactory.CreateLogger<AzureAdb2CCreateUserWebhook>();
        }

        [Function("meinapetito_webhook_azureadb2c_createuser")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post","get")] HttpRequestData req)
        {
            var requestBodyJson = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation(requestBodyJson);

            var user = JsonConvert.DeserializeObject<AzureAdB2CUser>(requestBodyJson);
            var continuation = new ContinuationResponse() 
            {
                DisplayName = $"{user.Surname}, {user.GivenName}"
            };

            _logger.LogInformation($"Received request for: {user.Email}");
            
            await _webhookSender.SendWebhookToMeinApetitoAsync(user);
            
            var continuationJson = JsonConvert.SerializeObject(continuation, new JsonSerializerSettings() 
            {
                ContractResolver = new DefaultContractResolver() 
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            });
            _logger.LogInformation(continuationJson);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.WriteString(continuationJson);

            return response;
        }
    }

    public class AzureAdB2CUser 
    {
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string Client_Id { get; set; }
        public string Email { get; set; }
    }

    public class ContinuationResponse
    {

        public ContinuationResponse()
        {
            DisplayName = string.Empty;
        }

        public string Version  => "1.0.0";
        public string Action => "Continue";

        public string DisplayName { get; set; }
    }
}
