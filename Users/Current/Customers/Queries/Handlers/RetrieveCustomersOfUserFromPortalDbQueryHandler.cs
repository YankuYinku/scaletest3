using System.Diagnostics;
using apetito.CQS;
using apetito.meinapetito.Portal.Application.Root.Users.Claims;
using apetito.meinapetito.Portal.Contracts.Root.Users.Customers;
using apetito.meinapetito.Portal.Contracts.Root.Users.Sortiments;
using apetito.meinapetito.Portal.Data.Root;
using Microsoft.EntityFrameworkCore;

namespace apetito.meinapetito.Portal.Application.Root.Users.Current.Customers.Queries.Handlers;

public class RetrieveCustomersOfUserFromPortalDbQueryHandler : IQueryHandler<RetrieveCustomersOfUserFromPortalDbQuery, CustomersOfUserDto>
{
    private readonly ILogger<RetrieveCustomersOfUserFromPortalDbQueryHandler> _logger;
    private readonly PortalDbContext _portalDbContext;

    public RetrieveCustomersOfUserFromPortalDbQueryHandler(
        ILogger<RetrieveCustomersOfUserFromPortalDbQueryHandler> logger,
        PortalDbContext portalDbContext)
    {
        _logger = logger;
        _portalDbContext = portalDbContext;
    }

    public async Task<CustomersOfUserDto> Execute(RetrieveCustomersOfUserFromPortalDbQuery query)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var user = await _portalDbContext.PortalUsers.FirstOrDefaultAsync(a => a.Email == query.Email);

        var result = new CustomersOfUserDto(query.Email);
        
        if (user is null)
        {
            stopwatch.Stop();
            _logger.LogWarning("Getting customer from PortalDB takes is {0} ms", stopwatch.ElapsedMilliseconds);
            
            return result;
        }
        

        var customerNumbersRow = _portalDbContext.PortalUserCustomerNumbers.Include(cn => cn.CustomerNumbers).Single(a => a.PortalUserReferenceId == user.PortalUserId);
        _logger.LogInformation($"Retrieving customers of user {query.Email}");
        _logger.LogInformation($"Retrieving customers of user customerNumbersRow {customerNumbersRow.PortalUserReferenceId}, {customerNumbersRow.PortalUserCustomerNumbersId}");        
        
        var customerNumbers = customerNumbersRow.CustomerNumbers;
        var customerNumberIds = customerNumbers.Select(cn => cn.PortalUserCustomerNumbersEntryId).ToList();
        var sortiments = _portalDbContext.PortalUserCustomerNumberEntrySortiments.Include(s => s.Sortiments).Where(a => a.PortalUserReferenceId == user.PortalUserId &&  customerNumberIds.Contains(a.PortalUserCustomerNumbersEntryReferenceId)).ToList();

        result.Customers = customerNumbers.Select(cn => new CustomerDto
        {
            CustomerNumber = cn.CustomerNumber,
            OrderSystem = cn.OrderSystem,
            EffectiveOrderSystems = new List<string>{cn.OrderSystem},
            ContactPortal = UserClaimFactory.GetContactPortalBasedOnUserRole(cn.Role),
            LanguageCode = cn.LanguageCode,
            Sortiments = sortiments.Single(a => a.PortalUserCustomerNumbersEntryReferenceId == cn.PortalUserCustomerNumbersEntryId).Sortiments.Select(s => new SortimentDto
            {
                Code = s.SortimentCode,
            }).ToList()
        }).ToList();
        
        
        stopwatch.Stop();
        _logger.LogWarning("Getting customer from PortalDB takes is {0} ms", stopwatch.ElapsedMilliseconds);
        
        return result;
    }
}