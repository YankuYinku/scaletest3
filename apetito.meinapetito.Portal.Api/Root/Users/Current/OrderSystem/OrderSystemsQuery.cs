using apetito.CQS;
using apetito.meinapetito.Portal.Application.Root.Users.Current.OrderSystems.Queries;
using apetito.meinapetito.Portal.Contracts.Root.Users.OrderSystems;
using HotChocolate;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.Root.Users.Current.OrderSystem;

[ExtendObjectType("Query")]
public class OrderSystemsQuery
{
    public Task<OrderSystemsDto> RetrieveOrderSystemsAsync([Service] IQueryHandler<RetrieveOrderSystemsOfUserEmailQuery,
            OrderSystemsDto> queryHandler,
        RetrieveOrderSystemsGraphQlQueryDto retrieveOrderSystemsGraphQlQuery) =>
        queryHandler.Execute(new RetrieveOrderSystemsOfUserEmailQuery
        {
            CustomerNumbers = retrieveOrderSystemsGraphQlQuery.CustomerNumbers ?? new List<int>(),
            UserEmail = retrieveOrderSystemsGraphQlQuery.UserEmail
        });
}