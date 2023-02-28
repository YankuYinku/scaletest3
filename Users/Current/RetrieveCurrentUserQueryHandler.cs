using apetito.CQS;
using apetito.iProDa3.Contracts;
using apetito.iProDa3.Contracts.Models.Sortiments;
using apetito.meinapetito.Portal.Application.Bkts.Queries;
using apetito.meinapetito.Portal.Application.Root.Authentication.UserAccessTokens.BearerToken.Queries;
using apetito.meinapetito.Portal.Application.Root.Authentication.UserAccessTokens.IbsscToken.Queries;
using apetito.meinapetito.Portal.Application.Root.Users.Claims.Queries;
using apetito.meinapetito.Portal.Application.Root.Users.Current.Customers.Queries;
using apetito.meinapetito.Portal.Application.Root.Users.Current.Permissions.Queries;
using apetito.meinapetito.Portal.Application.Root.Users.Current.Sortiments.Queries;
using apetito.meinapetito.Portal.Contracts.Bkts.Models;
using apetito.meinapetito.Portal.Contracts.Root.Authentication.UserAccessToken;
using apetito.meinapetito.Portal.Contracts.Root.Users.Claims;
using apetito.meinapetito.Portal.Contracts.Root.Users.Current;
using apetito.meinapetito.Portal.Contracts.Root.Users.Customers;
using apetito.meinapetito.Portal.Contracts.Root.Users.Permissions;
using apetito.meinapetito.Portal.Contracts.Root.Users.Sortiments;
using AutoMapper;
using CustomerDto = apetito.meinapetito.Portal.Contracts.Root.Users.Current.CustomerDto;

namespace apetito.meinapetito.Portal.Application.Root.Users.Current;

public class RetrieveCurrentUserQueryHandler : IQueryHandler<RetrieveCurrentUserQuery,UserDto>
{
    private readonly IMapper _mapper;
    private readonly IQueryHandler<RetrieveCustomersOfUserQuery, CustomersOfUserDto> _customersOfUserQuery;
    private readonly IQueryHandler<RetrieveClaimsOfUserQuery, UserAndCustomerClaimsDto> _claimsOfUserQuery;
    private readonly IQueryHandler<RetrieveClaimsPermissionsQuery, PermissionsSetDto> _permissionsOfUserQuery;
    private readonly IQueryHandler<RetrieveUserAccessBearerTokenByCustomersOfUserQuery, UserAccessBearerTokenDto> _userAccessBearerTokenQuery;
    private readonly IQueryHandler<RetrieveUserAccessIbsscTokenByUserEmailQuery, UserAccessIbsscTokenDto> _userAccessIbsscTokenQuery;
    private readonly IQueryHandler<RetrieveSortimentsOfUserEmailQuery, IEnumerable<SortimentDto>> _iProDa3SortimentQuery;
    private readonly IQueryHandler<RetrieveBktBillingQuery, BktBillingResult> _bktAccountsQuery;

    public RetrieveCurrentUserQueryHandler(
        IMapper mapper, 
        IQueryHandler<RetrieveCustomersOfUserQuery, CustomersOfUserDto> customersOfUserQuery, 
        IQueryHandler<RetrieveClaimsOfUserQuery, UserAndCustomerClaimsDto> claimsOfUserQuery,
        IQueryHandler<RetrieveClaimsPermissionsQuery, PermissionsSetDto> permissionsOfUserQuery,
        IQueryHandler<RetrieveUserAccessBearerTokenByCustomersOfUserQuery, UserAccessBearerTokenDto> userAccessBearerTokenQuery, 
        IQueryHandler<RetrieveUserAccessIbsscTokenByUserEmailQuery, UserAccessIbsscTokenDto> userAccessIbsscTokenQuery,
        IQueryHandler<RetrieveSortimentsOfUserEmailQuery, IEnumerable<SortimentDto>> iProDa3SortimentQuery,
        IQueryHandler<RetrieveBktBillingQuery, BktBillingResult> bktAccountsQuery
        )
    {
        _mapper = mapper;
        _customersOfUserQuery = customersOfUserQuery;
        _claimsOfUserQuery = claimsOfUserQuery;
        _permissionsOfUserQuery = permissionsOfUserQuery;
        _iProDa3SortimentQuery = iProDa3SortimentQuery;
        _userAccessBearerTokenQuery = userAccessBearerTokenQuery;
        _userAccessIbsscTokenQuery = userAccessIbsscTokenQuery;
        _bktAccountsQuery = bktAccountsQuery;
    }

    public async Task<UserDto> Execute(RetrieveCurrentUserQuery query)
    {
        var customersOfUser = await _customersOfUserQuery.Execute(new () { UserEmail = query.UserEmail });
        var user = new UserDto(customersOfUser.UserEmail, customersOfUser.Customers.Select(c => _mapper.Map<CustomerDto>(c)));

        var bearerTokenQuery = _userAccessBearerTokenQuery.Execute(new RetrieveUserAccessBearerTokenByCustomersOfUserQuery(customersOfUser));
        var ibsscTokenQuery = _userAccessIbsscTokenQuery.Execute(new RetrieveUserAccessIbsscTokenByUserEmailQuery() {UserEmail = query.UserEmail});

        var bktNumbers = await _bktAccountsQuery.Execute(new RetrieveBktBillingQuery()
        {
            Excludes = query.Excludes
        });
        
        var claimsQuery =  _claimsOfUserQuery.Execute(new RetrieveClaimsOfUserQuery() { UserEmail = query.UserEmail, CustomersOfUserDto = customersOfUser, BktBillings = bktNumbers} );
        var sortimentsQuery = _iProDa3SortimentQuery.Execute(new RetrieveSortimentsOfUserEmailQuery() {
                SortimentCodes = user.SortimentCodes()
            });
        
        Task.WaitAll(
            bearerTokenQuery,
            ibsscTokenQuery,
            claimsQuery,
            sortimentsQuery);
        
        var permissionSet = await _permissionsOfUserQuery.Execute(new RetrieveClaimsPermissionsQuery() { UserEmail = query.UserEmail, ClaimsDto = claimsQuery.Result });
        
        user.LegacyBearerToken = bearerTokenQuery.Result;
        user.IbsscToken = ibsscTokenQuery.Result;
        user.SetPermissions(permissionSet);
        user.Summarize();
        user.EnrichSortiments(sortimentsQuery.Result);

        return user;
    }
}