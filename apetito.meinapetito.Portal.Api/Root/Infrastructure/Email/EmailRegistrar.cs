using System.Net.Http.Headers;
using apetito.BearerToken;
using apetito.CQS;
using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Root.Authentication.LegacyTokenExchange;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Commands;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Commands.Handlers;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Options;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Queries;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Queries.Handlers;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure.Consts;
using apetito.Notification.Contracts.ApiClient;
using RestEase;

namespace apetito.meinapetito.Portal.Api.Root.Infrastructure.Email;

public class EmailRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var apetitoNotificationBaseUrl = RegisterOptions(services, configuration);

        services.AddTransient<ICommandHandler<CreateEmailToSendCommand>, CreateEmailToSendCommandHandler>();
        services.AddTransient<ICommandHandler<RegisterEmailToSendCommand>, RegisterEmailToSendCommandHandler>();
        services
            .AddTransient<IQueryHandler<RetrieveEmailToSendQuery, Notification.Contracts.Models.Message.SendGrid>,
                RetrieveEmailToSendQueryHandler>();

        services.AddDistributedSqlServerCache(options =>
        {
            options.ConnectionString = configuration.GetConnectionString("DistributedCache");
            options.SchemaName = "dbo";
            options.TableName = "Caches";
        });

        AddHttpClientWithTokenExchange<INotificationsApi>(services, apetitoNotificationBaseUrl);
        services.AddTransient(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(INotificationsApi));
            return RestClient.For<INotificationsApi>(httpClient);
        });
    }

    private string RegisterOptions(IServiceCollection services, IConfiguration configuration)
    {
        var eventName = configuration["Email:EventName"];
        var channelType = configuration["Email:ChannelType"];
        var apetitoNotificationBaseUrl = configuration["Email:ApetitoNotificationBaseUrl"];
        var invitationSenderEmailAddress = configuration["Email:InvitationSenderEmailAddress"];
        var items = configuration.GetSection("Email:ContactFormReceiverAddress");
            
        var parsedItems = items.Get<List<LanguageWithTopicEmailsOptions>>();

        var dictionary = new Dictionary<string, IDictionary<string, List<string>>>();

         foreach (var pair in parsedItems)
         {
             foreach (var topicEmails in pair.Value)
             {
                 var dict = new Dictionary<string, List<string>>();
                 var emails = topicEmails.Value.Split(";").ToList();
                 
                 if (dictionary.ContainsKey(pair.Key))
                 {
                     dictionary[pair.Key].Add(topicEmails.Key, emails);
                     continue;
                 }
                 
                 dict.Add(topicEmails.Key, emails);
                 dictionary.Add(pair.Key, dict);    
             }
         }
        
        IList<EmailOptionsEventObject> emailOptionsEventObject =
            configuration.GetSection("Email:Events").Get<List<EmailOptionsEventObject>>();
        
        IDictionary<EmailOptionsContextEnum, EmailOptionsEventObject> emailOptionsEventObjectMap =
            new Dictionary<EmailOptionsContextEnum, EmailOptionsEventObject>();

        foreach (var emailOptionsEvent in emailOptionsEventObject)
        {
            if (Enum.TryParse<EmailOptionsContextEnum>(emailOptionsEvent.Context, out var emailOptionsContextEnum))
            {
                emailOptionsEventObjectMap.Add(emailOptionsContextEnum, emailOptionsEvent);
            }
        }

        var options = new EmailOptions
        {
            EventName = eventName,
            ChannelType = channelType,
            ApetitoNotificationBaseUrl = apetitoNotificationBaseUrl,
            InvitationSenderEmailAddress = invitationSenderEmailAddress,
            EmailOptionsEventObjectMap = emailOptionsEventObjectMap,
            ContactFormReceiverAddress = dictionary
        };

        services.AddSingleton(options);

        return apetitoNotificationBaseUrl;
    }

    private static void AddHttpClientWithTokenExchange<T>(IServiceCollection services, string baseUrl)
    {
        services.AddHttpClient(typeof(T).Name)
            .ConfigureHttpClient((provider, client) =>
            {
                var logger = provider.GetRequiredService<ILogger<Program>>();
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

                var legacyToken = httpContextAccessor.HttpContext?.Request.Headers["X-Apetito-Authorization"]
                    .FirstOrDefault();

                logger.LogInformation("AddHttpClientWithTokenExchange legacyToken:" + legacyToken);

                if (legacyToken == null)
                {
                    try
                    {
                        using var scope = provider.CreateScope();
                        var bearerTokenProvider =
                            scope.ServiceProvider.GetRequiredService<IBearerTokenRequestProvider>();
                        var bearerToken = bearerTokenProvider.Authorization.Parameter;
                        logger.LogInformation("AddHttpClientWithTokenExchange Start token exchange...");
                        logger.LogInformation("AddHttpClientWithTokenExchange bearerToken: " + bearerToken);
                        var tokenExchanger = provider.GetRequiredService<IMeinApetitoTokenExchanger>();
                        legacyToken = tokenExchanger.ExchangeAsync(bearerToken).Result;
                        logger.LogInformation("AddHttpClientWithTokenExchange legacyToken: " + legacyToken);
                    }
                    catch (System.Exception ex)
                    {
                        logger.LogError(ex.ToString());
                    }
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", legacyToken);
                client.BaseAddress = new Uri(baseUrl);
            })
            .ConfigurePrimaryHttpMessageHandler((provider) =>
            {
                return new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                };
            });
    }
}