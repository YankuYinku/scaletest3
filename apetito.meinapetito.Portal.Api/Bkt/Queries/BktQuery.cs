using apetito.CQS;
using apetito.meinapetito.Portal.Application.Bkts.Queries;
using apetito.meinapetito.Portal.Application.Bkts.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.Bkts.Models;
using HotChocolate;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.Bkt.Queries;

[ExtendObjectType("Query")]
public class BktQuery
{
    public async Task<IList<MonthInfoDto>> GetMonthsAsync([Service] IMonthlyRecordsProvider provider,
        GetBktMonthsRequest request)
    {
        var data = await provider.GetMonthsAsync(request.ContractAccountId);

        return data;
    }

    public async Task<GetMonthlyRecordsResult> GetMonthlyRecordsAsync(
        [Service] IMonthlyRecordsProvider provider,
        MonthlyRecordsRequest monthlyRecordsRequest)
    {
        var data = await provider.GetMonthlyRecords(monthlyRecordsRequest);

        return data;
    }

    public async Task<BktMonthSummarizationModel> GetMonthlyRecordsSummarizationAsync(
        [Service] IMonthlyRecordsProvider provider,
        MonthlyRecordsRequest monthlyRecordsRequest)
    {
        var monthlyRecords = await provider.GetMonthlyRecords(monthlyRecordsRequest);

        var days = monthlyRecords.Days.SelectMany(a => a.MealInfos).ToList();

        var records = days.SelectMany(a => a.MealParticipantInfos).ToList();

        var amounts = records.Select(a => a.NumberOfParticipants).ToList();

        return new BktMonthSummarizationModel
        {
            Amount = amounts.Sum(a => a ?? 0)
        };
    }

    public async Task<BktBillingResult> GetUserBillingTypeQuery(
        [Service] IQueryHandler<RetrieveBktBillingQuery, BktBillingResult> billingQueryHandler)
    {
        return await billingQueryHandler.Execute(new RetrieveBktBillingQuery());
    }

    public async Task<BktToleranceDeviationResult> GetToleranceDeviationQuery(
        [Service]
        IQueryHandler<RetrieveToleranceDeviationQuery, BktToleranceDeviationResult> toleranceDeviationQueryHandler,
        MonthlyRecordsRequest monthlyRecordsRequest
    )
    {
        return await toleranceDeviationQueryHandler.Execute(new RetrieveToleranceDeviationQuery(monthlyRecordsRequest));
    }

    public async Task<BktToleranceCheckResult> CheckToleranceQuery(MonthlyRecordsRequest request,
        [Service]IQueryHandler<RetrieveToleranceQuery,BktToleranceCheckResult> queryHandler)
    {
        return await queryHandler.Execute(new RetrieveToleranceQuery(request));
    }
}