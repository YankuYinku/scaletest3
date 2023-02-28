using apetito.meinapetito.Portal.Application.Bkts.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.Bkts.Models;
using HotChocolate;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.Bkt.Mutations;

[ExtendObjectType("Mutation")]
public class BktMutation
{
    public async Task<BktRecordResult> UpdateBktRecordAsync(
        [Service] IBktRecordUpdater updater,
        BktRecordRequest bktRecordRequest)
    {
        return await updater.UpdateBktRecordAsync(bktRecordRequest);
    }

    public async Task<BktRecordDayResult> UpdateBktRecordDayStatusAsync(
        [Service] IBktRecordUpdater updater,
        BktRecordDayRequest btBktRecordDayRequest)
    {
        return await updater.UpdateBktRecordDayAsync(btBktRecordDayRequest);
    }

    public async Task<int> CloseMonthAsync([Service] IBktRecordUpdater updater, CloseMonthRequest request)
    {
        await updater.CloseMonthAsync(request);
        
        return 0;
    }
    
    public async Task<int> OpenMonthAsync([Service] IBktRecordUpdater updater, OpenMonthRequest request)
    {
        await updater.OpenMonthAsync(request);
        
        return 0;
    }
    
    public async Task<int> SubmitMonthAsync([Service] IBktRecordUpdater updater, SubmitMonthRequest request)
    {
        await updater.SubmitMonthAsync(request);
        
        return 0;
    }
}