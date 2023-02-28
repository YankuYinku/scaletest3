using System.Diagnostics;
using apetito.Authorization.Contracts.ClaimsPermissions;
using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.Claims;
using apetito.meinapetito.Portal.Contracts.Root.Users.Permissions;
using AutoMapper;

namespace apetito.meinapetito.Portal.Application.Root.Users.Current.Permissions.Queries.Handler;

public class RetrieveClaimsPermissionsQueryHandler : IQueryHandler<RetrieveClaimsPermissionsQuery,
    PermissionsSetDto>
{
    private readonly IClaimsPermissionsRestApi _claimsPermissionsRestApi;
    private readonly IMapper _mapper;
    private readonly ILogger<RetrieveClaimsPermissionsQueryHandler> _logger;

    public RetrieveClaimsPermissionsQueryHandler(IClaimsPermissionsRestApi claimsPermissionsRestApi,
         IMapper mapper, ILogger<RetrieveClaimsPermissionsQueryHandler> logger)
    {
        _claimsPermissionsRestApi = claimsPermissionsRestApi;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PermissionsSetDto> Execute(RetrieveClaimsPermissionsQuery query)
    {
        Stopwatch stopwatch = new ();
        stopwatch.Start();
    
        var userPermissions = await ComputeUserPermissions(query.ClaimsDto);

        stopwatch.Stop();
        _logger.LogWarning("Get Permissions data takes {0} ms", stopwatch.ElapsedMilliseconds);

        return userPermissions;
    }
 
    private async Task<PermissionsSetDto> ComputeUserPermissions(UserAndCustomerClaimsDto userClaimsDto)
    { 
        var userClaimsToAskForPermissions = ConvertUserClaimsToString(userClaimsDto.UserClaims);
        var userClaimsPermissions =
            await _claimsPermissionsRestApi.GetClaimsPermissionsAsync(userClaimsToAskForPermissions);
        var userPermissions = _mapper.Map<IList<PermissionDto>>(userClaimsPermissions.Permissions);

        var customerClaimsToAksForPermissions = ConvertUserClaimsByCustomerNumberToStrings(userClaimsDto.CustomerClaims);
        var id = await _claimsPermissionsRestApi.PostQueryPermissionsForCustomersRequiredClaimsAsync(new GetPermissionsForUserClaimsRequest
        {
            Claims = customerClaimsToAksForPermissions
        });
        var customerClaimsPermissions = await _claimsPermissionsRestApi.GetQueryPermissionsForCustomersRequiredClaimsAsync(id);
        var customerPermissions = customerClaimsPermissions.UserCustomerClaims.ToDictionary(a => a.CustomerNumber, a => _mapper.Map<IList<PermissionDto>>(a.ClaimsPermissions));
        
        var result = new PermissionsSetDto(userPermissions, customerPermissions);
        
        return result;
    }

    
    private static List<string> ConvertUserClaimsToString(IList<UserClaim> userClaims)
    {
        return userClaims.Select(ConvertUserClaimToString).ToList();
    }
    
    private static string ConvertUserClaimToString(UserClaim userClaim)
    {
        return $"{userClaim.Type}={userClaim.Value}";
    }
    
    private static List<string> ConvertUserClaimsByCustomerNumberToStrings(IDictionary<int,List<UserClaim>> claimsByCustomerNumber)
    {
        var claimsWithCustomerNumber = new List<string>();
        foreach (var customerNumberClaims in claimsByCustomerNumber)
        {
            var claims = customerNumberClaims.Value.Select(ConvertUserClaimToString);

            foreach (var claim in claims)
            { 
                claimsWithCustomerNumber.Add($"{customerNumberClaims.Key}:{claim}");
            }
        }
        return claimsWithCustomerNumber;
    }
}