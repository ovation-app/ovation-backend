using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quartz;

namespace Ovation.Persistence.Services.BackgroundServices
{
    class GetUserXMetricJob(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseJobService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(GetUserXMetricJob);
        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            if (jobData == null)
                return;
            
            var id = jobData.GetString("UserId");
            if (string.IsNullOrEmpty(id))
                return;

            try
            {
                var userId = new Guid(id);
                var xUser = await _context.VerifiedUsers
                .FirstOrDefaultAsync(_ => _.Type == "X" && _.UserId == userId.ToByteArray());

                if (xUser != null)
                {
                    //var client = factory.CreateClient(Constant.X);
                    var xClient = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<Apis.IX>();

                    if (string.IsNullOrEmpty(xUser.Handle))
                        return;

                    try
                    {
                        var response = !string.IsNullOrEmpty(xUser.TypeId) ? 
                            await xClient.GetUserPublicMetricsByUserIdAsync(xUser.TypeId) : 
                            await xClient.GetUserPublicMetricsByUsernameAsync(xUser.Handle);

                        if (response != null && response.Data != null
                            && response.Data.PublicMetrics != null)
                        {
                            var userStats = await _context.UserStats.FirstOrDefaultAsync(_ => _.UserId == xUser.UserId);
                            if (userStats != null)
                            {
                                userStats.Xfollowers = response.Data.PublicMetrics.FollowersCount;
                                userStats.Xfollowing = response.Data.PublicMetrics.FollowingCount;
                                if (string.IsNullOrEmpty(xUser.TypeId))
                                    xUser.TypeId = response.Data.Id;

                                xUser.MetaData = JsonConvert.SerializeObject(response);

                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    catch (Exception _)
                    {
                        SentrySdk.CaptureException(_);
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
