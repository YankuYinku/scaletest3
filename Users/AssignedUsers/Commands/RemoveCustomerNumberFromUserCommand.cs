using apetito.CQS;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands;

public class RemoveCustomerNumberFromUserCommand : ICommand
{
    public Guid Id { get; }
    public IList<int> CustomerNumbers { get; }

    public RemoveCustomerNumberFromUserCommand(Guid id, IList<int> customerNumbers)
    {
        Id = id;
        CustomerNumbers = customerNumbers;
    }
}