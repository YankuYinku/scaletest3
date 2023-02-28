using System.Diagnostics;
using apetito.CQS;
using apetito.Customers.Contracts.CustomersOfUser.ApiClients;
using apetito.Customers.Contracts.CustomersOfUser.Models;
using apetito.meinapetito.Portal.Contracts.Root.Users.Customers;
using AutoMapper;
using CustomersOfUserDto = apetito.meinapetito.Portal.Contracts.Root.Users.Customers.CustomersOfUserDto;

namespace apetito.meinapetito.Portal.Application.Root.Users.Current.Customers.Queries.Handlers;
public class RetrieveCustomersOfUserFromSAPQueryHandler : IQueryHandler<RetrieveCustomersOfUserFromSAPQuery, CustomersOfUserDto>
{
    private readonly IMapper _mapper;
    private readonly ILogger<RetrieveCustomersOfUserFromSAPQueryHandler> _logger;
    private readonly ICustomerServiceRestClient _customerServiceRestClient;

    public RetrieveCustomersOfUserFromSAPQueryHandler(IMapper mapper, ICustomerServiceRestClient customerServiceRestClient, ILogger<RetrieveCustomersOfUserFromSAPQueryHandler> logger)
    {
        _mapper = mapper;
        _customerServiceRestClient = customerServiceRestClient;
        _logger = logger;
    }

    public async Task<CustomersOfUserDto> Execute(RetrieveCustomersOfUserFromSAPQuery fromSapQuery)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = await RetrieveCustomersOfUser(fromSapQuery.UserEmail);
        stopwatch.Stop();
        _logger.LogWarning("Getting customer from SAP takes is {0} ms", stopwatch.ElapsedMilliseconds);
        return result;
    }

    private async Task<CustomersOfUserDto> RetrieveCustomersOfUser(string email) 
    {
        var customersOfUserResult = await _customerServiceRestClient.RetrieveCustomersOfUserAsync(email, new apetito.Customers.Contracts.CustomersOfUser.ApiClients.RequestModels.RetrieveCustomersOfUserQuery());
       
        var customersOfUserDtos = _mapper.Map<apetito.Customers.Contracts.CustomersOfUser.Models.CustomersOfUserDto, CustomersOfUserDto>(customersOfUserResult);
        return customersOfUserDtos;
    }
}