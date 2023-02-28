using apetito.CQS;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.ADB2C.Queries;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Commands;
using apetito.meinapetito.Portal.Application.Root.Users.Current;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure.ADB2C;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;
using apetito.meinapetito.Portal.Data.Root;
using Microsoft.EntityFrameworkCore;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands.Handlers;

public class InviteUserCommandHandler : ICommandHandler<InviteUser>
{
    private readonly ILogger<InviteUserCommandHandler> _logger;
    private readonly ICommandHandler<CreateUser> _createUserCommandHandler;
    private readonly ICommandHandler<CreateUserCustomerNumbers> _createUserCustomerNumbersCommandHandler;
    private readonly PortalDbContext _portalDbContext;
    private readonly IQueryHandler<RetrieveAzureADB2CUserQuery, UserDto?> _queryHandler;
    private readonly ICommandHandler<SendMailToUserAboutInvitationToCustomerNumbers> _sendCommandHandler;

    private readonly
        IQueryHandler<RetrieveCurrentUserQuery, apetito.meinapetito.Portal.Contracts.Root.Users.Current.UserDto>
        _currentUserQueryHandler;

    public InviteUserCommandHandler(ILogger<InviteUserCommandHandler> logger,
        ICommandHandler<CreateUser> createUserCommandHandler,
        ICommandHandler<CreateUserCustomerNumbers> createUserCustomerNumbersCommandHandler,
        PortalDbContext portalDbContext, IQueryHandler<RetrieveAzureADB2CUserQuery, UserDto?> queryHandler,
        IQueryHandler<RetrieveCurrentUserQuery, Contracts.Root.Users.Current.UserDto> currentUserQueryHandler, ICommandHandler<SendMailToUserAboutInvitationToCustomerNumbers> sendCommandHandler)
    {
        _logger = logger;
        _createUserCommandHandler = createUserCommandHandler;
        _createUserCustomerNumbersCommandHandler = createUserCustomerNumbersCommandHandler;
        _portalDbContext = portalDbContext;
        _queryHandler = queryHandler;
        _currentUserQueryHandler = currentUserQueryHandler;
        _sendCommandHandler = sendCommandHandler;
    }

    public async Task Handle(InviteUser command)
    {
        var userFromAzureADB2C = await _queryHandler.Execute(new RetrieveAzureADB2CUserQuery
        {
            Email = command.Email
        });
        
        Guid userId = await ProcessPortalUser(command,userFromAzureADB2C);

        await ProcessUserCustomerNumbers(command, userId);

        await SendMail(userFromAzureADB2C,command);
    }

    private async Task SendMail(UserDto? userFromAzureAdb2C, InviteUser command)
    {
        var commandToSendMail = PrepareCommandToSendMail(userFromAzureAdb2C, command);

        await _sendCommandHandler.Handle(commandToSendMail);
    }

    private async Task ProcessUserCustomerNumbers(InviteUser command, Guid userId)
    {
        IDictionary<int, string> customerNumberWithRole = BuildCustomerNumberWithRoleDictionary(command);

        IDictionary<int, CustomerNumberDetails> customerNumberDetailsMap = 
            await BuildCustomerNumberDetailsMap(command.MainCustomerAddressEmail, customerNumberWithRole);

        CreateUserCustomerNumbers createUserCustomerNumbers = new(customerNumberDetailsMap, userId);

        await _createUserCustomerNumbersCommandHandler.Handle(createUserCustomerNumbers);
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

    private static IDictionary<int, string> BuildCustomerNumberWithRoleDictionary(InviteUser command)
    {
        IDictionary<int, string> customerNumberWithRole = new Dictionary<int, string>();

        foreach (var customerNumber in command.CustomerNumbers)
        {
            customerNumberWithRole.Add(customerNumber.CustomerNumber, customerNumber.Role);
        }

        return customerNumberWithRole;
    }


    private async Task<Guid> ProcessPortalUser(InviteUser command, UserDto? userFromAzureAdb2C)
    {
        var portalUser = await _portalDbContext.PortalUsers.FirstOrDefaultAsync(a => a.Email == command.Email);

        if (portalUser is not null)
        {
            return portalUser.PortalUserId;
        }

        Guid newPortalUserId = Guid.NewGuid();
        
        CreateUser createUser = CreateCommand(newPortalUserId, userFromAzureAdb2C, command);

        await _createUserCommandHandler.Handle(createUser);

        return newPortalUserId;
    }

    private CreateUser CreateCommand(Guid newPortalUserId, UserDto? userFromAzureAdb2C, InviteUser command)
    {
        if (userFromAzureAdb2C is null)
        {
            CreateUser createUserWithoutAdb2C =
                new CreateUser(newPortalUserId, command.Email, command.FirstName, command.LastName);
            return createUserWithoutAdb2C;
        }
        
        CreateUser createUser =
            new CreateUser(newPortalUserId, command.Email, userFromAzureAdb2C.FirstName, userFromAzureAdb2C.LastName,userFromAzureAdb2C.AzureADB2CId);
        return createUser;
    }

    private SendMailToUserAboutInvitationToCustomerNumbers PrepareCommandToSendMail(UserDto? userFromAzureAdb2C, InviteUser command)
    {
        if (userFromAzureAdb2C is null)
        {
            return new SendMailToUserAboutInvitationToCustomerNumbers(command.FirstName, command.LastName,
                command.Email, command.MainCustomerAddressEmail, false,command.LanguageCode);
        }
        
        return new SendMailToUserAboutInvitationToCustomerNumbers(userFromAzureAdb2C.FirstName, userFromAzureAdb2C.LastName,
            userFromAzureAdb2C.Email, command.MainCustomerAddressEmail, true,command.LanguageCode);
    }
}