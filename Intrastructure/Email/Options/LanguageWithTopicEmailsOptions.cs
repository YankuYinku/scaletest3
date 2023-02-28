namespace apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Options;

public class LanguageWithTopicEmailsOptions
{
    public string Key { get; set; }
    public IList<TopicWithEmailsOptions> Value { get; set; }
}