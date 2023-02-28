using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Commands.Handlers;

public class CreateEmailToSendCommandHandler : ICommandHandler<CreateEmailToSendCommand>
{
    private readonly IDistributedCache _distributedCache;
    public CreateEmailToSendCommandHandler(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }
    
    public async Task Handle(CreateEmailToSendCommand command)
    {
        var emailToSendCacheModel = new EmailToSendCacheModel
        {
            ReferenceId = command.ReferenceId,
            Context = command.Context,
            Body = command.Body,
            Subject = command.Subject,
            ToReceivers = command.ToReceivers,
            CcReceivers = command.CcReceivers,
            BccReceivers = command.BccReceivers,
            AttachmentsUris = command.AttachmentsUris,
        };
        
        var cacheEntryOptions = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

        await _distributedCache.SetStringAsync(command.ReferenceId.ToString(),
            JsonConvert.SerializeObject(emailToSendCacheModel), cacheEntryOptions);
    }
}