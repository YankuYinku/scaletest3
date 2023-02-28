using apetito.meinapetito.Portal.Contracts.Root.Users.Filters;
using apetito.meinapetito.Portal.Data.Root.Users;
using AutoMapper;

namespace apetito.meinapetito.Portal.Api.Root.Users.Filters;

public class FilterAutomapperProfile : Profile
{
    public FilterAutomapperProfile()
    {
        CreateMap<PortalUserFilter, PortalUserFilterDto>();
    }
}