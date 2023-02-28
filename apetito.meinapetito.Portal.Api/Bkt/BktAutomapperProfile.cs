using apetito.BKT.Contracts.Models;
using apetito.meinapetito.Portal.Contracts.Bkts.Models;
using AutoMapper;

namespace apetito.meinapetito.Portal.Api.Bkt;

public class BktAutomapperProfile: Profile
{
    public BktAutomapperProfile()
    {
        CreateMap<DayInfo, DailyInfoDto>();
        CreateMap<MealInfo, MealInfoDto>();
        CreateMap<MealParticipantInfo, MealParticipantInfoDto>();
        CreateMap<MonthInfo, MonthInfoDto>();
    }
}