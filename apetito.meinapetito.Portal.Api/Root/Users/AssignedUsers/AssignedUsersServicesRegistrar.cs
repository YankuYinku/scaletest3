using apetito.BearerToken;
using apetito.CQS;
using apetito.Customers.Contracts.CustomersOfUser.ApiClients;
using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands;
using apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands.Handlers;
using apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Options;
using apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries;
using apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries.Handlers;
using apetito.meinapetito.Portal.Application.Root.Users.Current;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;
using apetito.meinapetito.Portal.Contracts.Root.Users.Current;
using RestEase;

namespace apetito.meinapetito.Portal.Api.Root.Users.AssignedUsers;

public class AssignedUsersServicesRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTransient<IQueryHandler<RetrieveRolesOfAssignedUsers, IList<RoleDto>>,
                RetrieveRolesOfAssignedUsersQueryHandler>();
        services
            .AddTransient<IQueryHandler<RetrieveAssignedUsers, IList<AssignedUserDto>>,
                RetrieveAssignedUsersQueryHandler>();

        services.AddHttpClient<ICustomerUsersServiceRestClient>(
                nameof(ICustomerUsersServiceRestClient))
            .ConfigureHttpClient((provider, httpClient) =>
            {
                using var scope = provider.CreateScope();
                var bearerTokenRequestProvider =
                    scope.ServiceProvider.GetRequiredService<IBearerTokenRequestProvider>();

                httpClient.BaseAddress = new Uri(configuration["CustomersService:BaseAddress"]);
                httpClient.DefaultRequestHeaders.Authorization = bearerTokenRequestProvider.Authorization;
            });

        services.AddTransient(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient(nameof(ICustomerUsersServiceRestClient));
            return RestClient.For<ICustomerUsersServiceRestClient>(client);
        });


        services
            .AddTransient<ICommandHandler<RemoveCustomerNumberFromUserCommand>,
                RemoveCustomerNumberFromUserCommandHandler>();
        services.AddTransient<ICommandHandler<InviteUser>, InviteUserCommandHandler>();
        services.AddTransient<ICommandHandler<CreateUser>, CreateUserCommandHandler>();
        services.AddTransient<ICommandHandler<SendMailToReinviteUser>, SendMailToReinviteUserCommandHandler>();
        services.AddTransient<ICommandHandler<CreateUserCustomerNumbers>, CreateUserCustomerNumbersCommandHandler>();
        services
            .AddTransient<ICommandHandler<CreateUserCustomerNumbersSortiments>,
                CreateUserCustomerNumbersSortimentsCommandHandler>();
        services
            .AddTransient<ICommandHandler<SendMailToUserAboutInvitationToCustomerNumbers>,
                SendMailToUserAboutInvitationToCustomerNumbersCommandHandler>();
        services.AddTransient<ICommandHandler<ResendInvitationForUser>, ResendInvitationForUserCommandHandler>();
        
        services.AddTransient<IQueryHandler<RetrieveAssignedUserById, AssignedUserDto?>, RetrieveAssignedUserByIdQueryHandler>();
        
        
        services.AddTransient<ICommandHandler<ChangeUserRole>,ChangeUserRoleCommandHandler>();
        services.AddTransient<ICommandHandler<ChangeUserRelations>,ChangeUserRelationsHandler>();
        services.AddTransient<IQueryHandler<RetrieveAssignedUsersFromSAP, IList<AssignedUserDto>>,RetrieveAssignedUsersFromSapQueryHandler>();
        services.AddTransient<IQueryHandler<RetrieveAssignedUsersFromDb,IList<AssignedUserDto>>,RetrieveAssignedUsersFromDbQueryHandler>();
        services.AddTransient<IQueryHandler<RetrieveUserById,IList<CustomerDto>>,RetrieveUserByIdQueryHandler>();
        services.AddTransient<IQueryHandler<RetrieveFilledEmailToInviteUser, EmailDto>,RetrieveFilledEmailToInviteUserQueryHandler>();
        
        services
            .AddTransient<ICommandHandler<MarkUserAsActiveAndUpdateNamesFromAzureAdb2C>,
                MarkUserAsActiveAndUpdateNamesFromAzureAdb2CCommandHandler>();

        PreparePostRegistrationWebhookOptions(services, configuration);
        PreparePrismicOptions(services, configuration);
    }

    private static void PreparePrismicOptions(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var options = new EmailSendingDocumentTypeOptions
        {
            Type = configuration["EMailTemplates:Invitation:PrismicDocumentType"]
        };
        serviceCollection.AddSingleton(options);
    }
    private static void PreparePostRegistrationWebhookOptions(IServiceCollection services, IConfiguration configuration)
    {
        string secret = configuration["AzureADB2C:PostRegistrationWebhook:Secret"];
        
        PostRegistrationWebhookOptions options = new()
        {
            Secret = string.IsNullOrWhiteSpace(secret) ? string.Empty : secret
        };
        services.AddSingleton(options);
    }
}