using apetito.CQS;
using apetito.meinapetito.Portal.Application.Root.Users.Current.Sortiments.Queries;
using apetito.meinapetito.Portal.Contracts.Root.Users.Sortiments;
using HotChocolate;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.Root.Users.Current.Sortiments;

[ExtendObjectType("Query")]
public class SortimentsQuery
{
    public async Task<IEnumerable<SortimentDto>> GetSortimentsAsync(
        [Service] IQueryHandler<RetrieveSortimentsOfUserEmailQuery, IEnumerable<SortimentDto>> queryHandler,
        GetSortimentsGraphQlQueryDto getSortimentsQueryModel)
        => await queryHandler.Execute(new RetrieveSortimentsOfUserEmailQuery
        {
            UserEmail = getSortimentsQueryModel.UserEmail,
            CustomerNumbers = getSortimentsQueryModel.CustomerNumbers ?? new List<int>()
        });

    public async Task<IEnumerable<SortimentDto>> GetExceptionAsync(string message)
        => throw new Exception(message);
}