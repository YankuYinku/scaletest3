using apetito.CQS;
using apetito.meinapetito.Portal.Api.Root.Authentication.UserAccessTokens.Helpers;
using apetito.meinapetito.Portal.Application.Root.Users.Current;
using apetito.meinapetito.Portal.Application.Root.Users.Orders.Queries;
using apetito.meinapetito.Portal.Contracts.Root.Users.Current;
using apetito.meinapetito.Portal.Contracts.Root.Users.Orders.Models;
using apetito.meinapetito.Portal.Contracts.Root.Users.Orders.Models.ApetitoOrders;
using apetito.meinapetito.Portal.Contracts.Root.Users.Orders.Models.HawaOrders;
using HotChocolate;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.Root.Users.Orders.Queries
{
    [ExtendObjectType("Query")]
    public class OrdersQuery
    {
        public async Task<RetrieveOrdersQueryResult> GetOrdersAsync([Service] IHttpContextAccessor httpContextAccessor,
            [Service] IQueryHandler<RetrieveCurrentUserQuery, UserDto> queryHandler,
            [Service] IQueryHandler<RetrieveOrders, RetrieveOrdersQueryResult> retrieveOrdersQueryHandler,
            OrdersQueryRequest request)
        {
            var userEmail = httpContextAccessor.HttpContext?.GetUserOrImpersonatedUserEmailAdressInGraphQl();

            return await retrieveOrdersQueryHandler.Execute(new RetrieveOrders
            {
                SearchTerm = request.SearchTerm,
                Page = request.Page,
                PageSize = request.PageSize,
                CustomerNumbers = await DetermineCustomerNumbers(userEmail ?? string.Empty, queryHandler,
                    request.CustomerNumbers ?? new List<int>()),
                OrderDateFrom = request.OrderDateFrom,
                OrderDateTo = request.OrderDateTo,
                Status = new OrderStatus(request.Status),
                Supplier = request.Supplier
            });
        }

        public async Task<ApetitoOrderDto> GetApetitoOrderDetails([Service] IHttpContextAccessor httpContextAccessor,
            [Service] IQueryHandler<RetrieveCurrentUserQuery, UserDto> queryHandler,
            [Service] IQueryHandler<RetrieveApetitoOrderDetails, ApetitoOrderDto> apetitoOrderProvider,
            OrderDetailsQueryRequest request)
        {
            var userEmail = httpContextAccessor.HttpContext?.GetUserOrImpersonatedUserEmailAdressInGraphQl();

            return await apetitoOrderProvider.Execute(new RetrieveApetitoOrderDetails()
            {
                CustomerNumbers =
                    await DetermineCustomerNumbers(userEmail ?? string.Empty, queryHandler, request.CustomerNumbers),
                OrderId = int.Parse(request.OrderId)
            });
        }


        public async Task<HawaOrderDto> GetHawaOrderDetails([Service] IHttpContextAccessor httpContextAccessor,
            [Service] IQueryHandler<RetrieveCurrentUserQuery, UserDto> queryHandler,
            [Service] IQueryHandler<RetrieveHawaOrderDetails, HawaOrderDto> apetitoOrderProvider,
            OrderDetailsQueryRequest request)
        {
            var userEmail = httpContextAccessor.HttpContext?.GetUserOrImpersonatedUserEmailAdressInGraphQl();

            return await apetitoOrderProvider.Execute(new RetrieveHawaOrderDetails()
            {
                Id = request.OrderId
            });
        }

        private static async Task<IList<int>> DetermineCustomerNumbers(string email,
            IQueryHandler<RetrieveCurrentUserQuery, UserDto> queryHandler, IList<int> requestedCustomerNumbers)
        {
            var currentUser = await queryHandler.Execute(new RetrieveCurrentUserQuery
            {
                UserEmail = email
            });

            if (!requestedCustomerNumbers.Any())
            {
                return currentUser.Customers.Select(a => a.CustomerNumber).ToList();
            }
            
            IList<int> correctCustomerNumbers = new List<int>();

            foreach (var customerNumber in requestedCustomerNumbers ?? new List<int>())
            {
                if (currentUser.Customers.All(c => c.CustomerNumber != customerNumber))
                {
                    continue;
                }

                correctCustomerNumbers.Add(customerNumber);
            }

            return correctCustomerNumbers;
        }
    }
}