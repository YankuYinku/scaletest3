using apetito.CQS;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Options;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure.Consts;
using apetito.meinapetito.Portal.Data.Root;
using apetito.Notification.Contracts.ApiClient;
using apetito.Notification.Contracts.Models.Event;
using apetito.Notification.Contracts.Models.Subscription;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Commands.Handlers;

public class RegisterEmailToSendCommandHandler : ICommandHandler<RegisterEmailToSendCommand>
{
    private readonly PortalDbContext _portalDbContext;
    private readonly INotificationsApi _notificationsApi;
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<RegisterEmailToSendCommandHandler> _logger;
    private readonly IDistributedCache _distributedCache;

    public RegisterEmailToSendCommandHandler(PortalDbContext portalDbContext, INotificationsApi notificationsApi,
        EmailOptions emailOptions, ILogger<RegisterEmailToSendCommandHandler> logger,
        IDistributedCache distributedCache)
    {
        _portalDbContext = portalDbContext;
        _notificationsApi = notificationsApi;
        _emailOptions = emailOptions;
        _logger = logger;
        _distributedCache = distributedCache;
    }

    public async Task Handle(RegisterEmailToSendCommand command)
    {
        var emailToSendFromDistributedCache = await _distributedCache.GetStringAsync(command.ReferenceId.ToString());

        var emailToSendCacheModel =
            JsonConvert.DeserializeObject<EmailToSendCacheModel>(emailToSendFromDistributedCache);

        if (emailToSendCacheModel is not null)
        {
            if (emailToSendCacheModel.ToReceivers.Any())
            {
                foreach (var receiver in emailToSendCacheModel.ToReceivers.ToList())
                {
                    await RegisterEmailToSendOnApetitoNotificationAsync(receiver, command.ReferenceId,command.Context);
                }
            }
        }
    }

    private async Task RegisterEmailToSendOnApetitoNotificationAsync(string receiver, Guid referenceId,EmailOptionsContextEnum emailOptionsContextEnum)
    {

        var emailOptionsEventObject = _emailOptions.EmailOptionsEventObjectMap[emailOptionsContextEnum];

        try
        {
            var messageRequest = new MessageRequest
            {
                EventName = emailOptionsEventObject.EventName,
                ReferenceId = referenceId.ToString(),
                Channels = new List<ChannelRequest>()
                    { new() { Type = _emailOptions.ChannelType, Receiver = receiver } }
            };


            await _notificationsApi.SendMessage(messageRequest);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex?.Message);
        }
    }
}