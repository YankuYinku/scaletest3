using apetito.CQS;
using apetito.Customers.Contracts.Company.Models;
using apetito.meinapetito.Portal.Application.Root.Company.Queries;

namespace apetito.meinapetito.Portal.Api.Root.Company.Controllers;

[ApiController]
[Authorize]
[Route("root/company")]
public class CompanyController : ControllerBase
{
    private readonly IQueryHandler<RetrieveCompanyDataQuery, AllCompanyDataDto> _companyDataQueryHandler;

    public CompanyController(IQueryHandler<RetrieveCompanyDataQuery, AllCompanyDataDto> companyDataQueryHandler)
    {
        _companyDataQueryHandler = companyDataQueryHandler;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCompanyData([FromQuery] int customerNumber, [FromQuery] IEnumerable<string>? includes)
    {
        var companyData = await _companyDataQueryHandler.Execute(new RetrieveCompanyDataQuery()
        {
            CustomerNumber = customerNumber,
            Includes = includes ?? new List<string>()
        });

        return Ok(companyData);
    }
    
}