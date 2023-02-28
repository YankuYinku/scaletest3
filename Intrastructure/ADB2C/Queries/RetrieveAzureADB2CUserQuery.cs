using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure.ADB2C;

namespace apetito.meinapetito.Portal.Application.Root.Intrastructure.ADB2C.Queries
{
    public class RetrieveAzureADB2CUserQuery : IQuery<UserDto?>
    {
        public string Email { get; set; }
    }
}
