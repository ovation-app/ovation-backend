using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ovation.Persistence.Data;
using Quartz;

namespace Ovation.Persistence.Services.BackgroundServices
{
    [DisallowConcurrentExecution]
    class SyncXFollowersDataJob(IServiceScopeFactory serviceScopeFactory, OvationDbContext _context) : IJob
    {
        internal const string Name = nameof(SyncXFollowersDataJob);
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var xUsers = await _context.VerifiedUsers
                .Where(_ => _.Type == "X")
                .ToListAsync();

                if (xUsers != null && xUsers.Count > 0)
                {
                    //var client = factory.CreateClient(Constant.X);
                    var xClient = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<Apis.IX>();
                    foreach (var user in xUsers)
                    {
                        if (string.IsNullOrEmpty(user.Handle))
                            continue;

                        try
                        {
                            var response = !string.IsNullOrEmpty(user.TypeId) ?
                            await xClient.GetUserPublicMetricsByUserIdAsync(user.TypeId) :
                            await xClient.GetUserPublicMetricsByUsernameAsync(user.Handle);

                            if (response != null && response.Data != null
                                && response.Data.PublicMetrics != null)
                            {
                                var userStats = await _context.UserStats.FirstOrDefaultAsync(_ => _.UserId == user.UserId);
                                if (userStats != null)
                                {
                                    userStats.Xfollowers = response.Data.PublicMetrics.FollowersCount;
                                    userStats.Xfollowing = response.Data.PublicMetrics.FollowingCount;
                                    if (string.IsNullOrEmpty(user.TypeId))
                                        user.TypeId = response.Data.Id;

                                    user.MetaData = JsonConvert.SerializeObject(response);

                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                        catch (Exception _)
                        {
                            SentrySdk.CaptureException(_);
                            continue;
                        }
                    }
                }
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
            }
        }
    }
}
