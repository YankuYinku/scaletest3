using System;

namespace apetito.meinapetito.Webhooks.ArticleChanges.Api.CheckArticlesChanged;

public class ScheduleStatus
{
    public DateTime Last { get; set; }

    public DateTime Next { get; set; }

    public DateTime LastUpdated { get; set; }
}