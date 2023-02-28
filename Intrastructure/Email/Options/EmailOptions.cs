using apetito.meinapetito.Portal.Contracts.Root.Infrastructure.Consts;

namespace apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Options;

public class EmailOptions
{
    public string EventName { get; set; }
    public string ChannelType { get; set; }
    public string ApetitoNotificationBaseUrl { get; set; }
    public string InvitationSenderEmailAddress { get; set; }
    public IDictionary<string, IDictionary<string, List<string>>> ContactFormReceiverAddress { get; set; }
    public IDictionary<EmailOptionsContextEnum, EmailOptionsEventObject> EmailOptionsEventObjectMap  { get; set; }
}