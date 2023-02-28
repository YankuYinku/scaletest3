using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.Filters;
using apetito.meinapetito.Portal.Data.Root;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace apetito.meinapetito.Portal.Application.Root.Users.Filters.Queries.Handlers;

public class RetrievePortalUserFiltersHandler : IQueryHandler<RetrievePortalUserFilters, IEnumerable<PortalUserFilterDto>>
{
    private readonly IMapper _mapper;
    private readonly PortalDbContext _portalDbContext;

    public RetrievePortalUserFiltersHandler(IMapper mapper, PortalDbContext portalDbContext)
    {
        _mapper = mapper;
        _portalDbContext = portalDbContext;
    }

    public async Task<IEnumerable<PortalUserFilterDto>> Execute(RetrievePortalUserFilters query)
    {
        var candidates = await _portalDbContext.PortalUserFilters
            .Where(a => a.Context == query.Context.ToString() && a.Email == query.Email).ToListAsync();

        return candidates.Select(a => _mapper.Map<PortalUserFilterDto>(a));
    }
}