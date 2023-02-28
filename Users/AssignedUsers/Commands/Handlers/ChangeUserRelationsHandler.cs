using apetito.CQS;
using apetito.meinapetito.Portal.Application.Root.Users.Current;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;
using apetito.meinapetito.Portal.Contracts.Root.Users.Current;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands.Handlers;

public class ChangeUserRelationsHandler : ICommandHandler<ChangeUserRelations>
{
    private readonly IQueryHandler<RetrieveCurrentUserQuery, UserDto> _currentUserQueryHandler;
    private readonly IQueryHandler<RetrieveUserById, IList<CustomerDto>> _handlerFromDb;
    private readonly ICommandHandler<CreateUserCustomerNumbers> _createCustomerNumbersHandler;
    private readonly ICommandHandler<RemoveCustomerNumberFromUserCommand> _removeCustomerNumbersHandler;

    public ChangeUserRelationsHandler(ICommandHandler<CreateUserCustomerNumbers> createCustomerNumbersHandler, IQueryHandler<RetrieveCurrentUserQuery, UserDto> currentUserQueryHandler, IQueryHandler<RetrieveUserById, IList<CustomerDto>> handlerFromDb, ICommandHandler<RemoveCustomerNumberFromUserCommand> removeCustomerNumbersHandler)
    {
        _createCustomerNumbersHandler = createCustomerNumbersHandler;
        _currentUserQueryHandler = currentUserQueryHandler;
        _handlerFromDb = handlerFromDb;
        _removeCustomerNumbersHandler = removeCustomerNumbersHandler;
    }

    public async Task Handle(ChangeUserRelations command)
    {
        var result = await _handlerFromDb.Execute(new RetrieveUserById()
        {
            Id = command.UserId
        });

        IList<int> currentDbCustomerNumbers = result.Select(a => a.CustomerNumber).ToList();

        IList<int> customerNumbersToRemove =
            currentDbCustomerNumbers.Where(a => !command.Relations.ContainsKey(a)).ToList();

        await RemoveAsync(command.UserId, customerNumbersToRemove);
        
        IDictionary<int, string> toAdd = new Dictionary<int, string>();
        
        foreach (var item in command.Relations)
        {
            if (currentDbCustomerNumbers.Contains(item.Key))
            {
                continue;
            }
            toAdd.Add(item);
        }

        await AddAsync(command.CurrentUserEmail, command.UserId, toAdd);
    }

    private async Task RemoveAsync(Guid userId, IList<int> customerNumbers)
    {
        RemoveCustomerNumberFromUserCommand command = new (userId, customerNumbers);

        await _removeCustomerNumbersHandler.Handle(command);
    }

    private async Task AddAsync(string currentUserEmail,Guid userId, IDictionary<int, string> items)
    {
        var customerNumbers = await BuildCustomerNumberDetailsMap(currentUserEmail, items);

        CreateUserCustomerNumbers command = new (customerNumbers,userId);

        await _createCustomerNumbersHandler.Handle(command);

    }
    
    private async Task<IDictionary<int, CustomerNumberDetails>> BuildCustomerNumberDetailsMap(
        string commandMainCustomerAddressEmail, IDictionary<int, string> customerNumberWithRole)
    {
        var result = await _currentUserQueryHandler.Execute(new RetrieveCurrentUserQuery()
        {
            UserEmail = commandMainCustomerAddressEmail
        });

        IDictionary<int, CustomerNumberDetails> dictionary = new Dictionary<int, CustomerNumberDetails>();
        
        foreach (var customer in customerNumberWithRole)
        {
            var firstOrDefault = result.Customers.FirstOrDefault(a => a.CustomerNumber == customer.Key);

            if (firstOrDefault is null)
            {
                continue;
            }
            
            dictionary.Add(new KeyValuePair<int, CustomerNumberDetails>(customer.Key,new CustomerNumberDetails()
            {
                Role = customer.Value,
                Sortiments =firstOrDefault.Sortiments?.Select(a => a.Code).ToList() ?? new List<string>(),
                LanguageCode = firstOrDefault.LanguageCode,
                OrderSystem = firstOrDefault.OrderSystem
            }));
        }

        return dictionary;
    }
}