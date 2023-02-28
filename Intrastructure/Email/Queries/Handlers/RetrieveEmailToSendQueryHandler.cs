using apetito.CQS;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Options;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure.Consts;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Queries.Handlers;

public class RetrieveEmailToSendQueryHandler : IQueryHandler<RetrieveEmailToSendQuery, Notification.Contracts.Models.Message.SendGrid>
{
    private readonly EmailOptions _emailOptions;
    private readonly IDistributedCache _distributedCache;
    public RetrieveEmailToSendQueryHandler(EmailOptions emailOptions, IDistributedCache distributedCache)
    {
        _emailOptions = emailOptions;
        _distributedCache = distributedCache;
    }
    
    public async Task<Notification.Contracts.Models.Message.SendGrid> Execute(RetrieveEmailToSendQuery query)
    {
        var referenceId = Guid.Parse(query.ReferenceId);
        
        var emailToSendFromDistributedCache = await _distributedCache.GetStringAsync(referenceId.ToString());

        var emailToSendCacheModel = JsonConvert.DeserializeObject<EmailToSendCacheModel>(emailToSendFromDistributedCache);

        var options =
            _emailOptions.EmailOptionsEventObjectMap[
                emailToSendCacheModel.Context ?? EmailOptionsContextEnum.InvitationMail];
        
        
        if (emailToSendCacheModel is not null)
        {
            var cc = string.Empty;
            var bcc = string.Empty;
            var attachments = new List<string>();
            
            if (emailToSendCacheModel.CcReceivers != null && emailToSendCacheModel.CcReceivers.ToList().Any())
            {
                cc = emailToSendCacheModel.CcReceivers.FirstOrDefault();
            }
            
            if (emailToSendCacheModel.BccReceivers != null && emailToSendCacheModel.BccReceivers.ToList().Any())
            {
                bcc = emailToSendCacheModel.BccReceivers.FirstOrDefault();
            }

            if (emailToSendCacheModel.AttachmentsUris != null && emailToSendCacheModel.AttachmentsUris.Any())
            {
                foreach (var attachment in emailToSendCacheModel.AttachmentsUris)
                {
                    attachments.Add(attachment);
                }
            }

            var sendGridResponse = new Notification.Contracts.Models.Message.SendGrid
            {
                Sender = options.SenderEmailAddress,
                Subject = emailToSendCacheModel.Subject,
                Body = emailToSendCacheModel.Body,
                Cc = cc,
                Bcc = bcc,
                Attachments = attachments
            };

            return sendGridResponse;
        }

        return new Notification.Contracts.Models.Message.SendGrid();
    }
}