using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands;

public class InviteUser : ICommand
{
    public string Email { get; }
    public string MainCustomerAddressEmail { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string LanguageCode { get; }
    public IList<CustomerNumberWithRoleRequest> CustomerNumbers { get; }

    public InviteUser(string email, string firstName, string lastName, IList<CustomerNumberWithRoleRequest> customerNumbers, string mainCustomerAddressEmail, string languageCode)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        CustomerNumbers = customerNumbers;
        MainCustomerAddressEmail = mainCustomerAddressEmail;
        LanguageCode = languageCode;
    }
}