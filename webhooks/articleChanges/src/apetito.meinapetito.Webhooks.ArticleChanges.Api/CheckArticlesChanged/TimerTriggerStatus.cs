namespace apetito.meinapetito.Webhooks.ArticleChanges.Api.CheckArticlesChanged;

public class TimerTriggerStatus
{
    public ScheduleStatus ScheduleStatus { get; set; }

    public bool IsPastDue { get; set; }
}
