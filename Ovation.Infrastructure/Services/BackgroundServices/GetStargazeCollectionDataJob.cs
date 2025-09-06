using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ovation.Application.Constants;
using Ovation.Domain.Entities;
using Ovation.Persistence.Services.Clients;
using Quartz;

namespace Ovation.Persistence.Services.BackgroundServices
{
    class GetStargazeCollectionDataJob(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseJobService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(GetStargazeCollectionDataJob);
        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            if (jobData != null)
            {
                var collection = jobData.GetString("ContractAddress");
                var chain = jobData.GetString("Chain");
            }
        }

        public async Task<long?> AddCollection(string collectionAddress, string chain = Constant.Stargaze)
        {
            try
            {
                var entity = await _context.NftCollectionsData
                .Where(_ => _.ContractName == collectionAddress
                && _.Chain == chain)
                .Select(x => new
                {
                    x.Id
                })
                .FirstOrDefaultAsync();

                if(entity != null)
                {
                    await _context.UserNftCollectionData
                    .Where(item => item.ContractName == collectionAddress && item.Chain == chain)
                    .ExecuteUpdateAsync(updates => updates
                    .SetProperty(item => item.ParentCollection, item => entity.Id));

                    return entity.Id;
                }

                var graphQl = _serviceScope.ServiceProvider.GetRequiredService<GraphQLService>();

                var data = await graphQl.GetStargazeCollectionAsync(collectionAddress);

                if (data != null)
                {
                    var collection = new NftCollectionsDatum
                    {
                        ContractName = data?.Collection?.Name,
                        ContractAddress = data?.Collection?.ContractAddress,
                        Description = data.Collection?.Description,
                        ItemTotal = data?.Collection?.TokenCounts?.Total,
                        LogoUrl = data?.Collection?.Media.FallbackUrl,
                        Chain = chain,
                        Verified = 0,
                        MetaData = JsonConvert.SerializeObject(data),
                    };

                    await _context.NftCollectionsData.AddAsync(collection);
                    await _context.SaveChangesAsync();

                    await _context.UserNftCollectionData
                    .Where(item => item.ContractName == collectionAddress && item.Chain == chain)
                    .ExecuteUpdateAsync(updates => updates
                    .SetProperty(item => item.ParentCollection, item => collection.Id));

                    return collection.Id;
                }

                return null;
            }
            catch (Exception _)
            {
                return null;
            }
        }
    }
}
