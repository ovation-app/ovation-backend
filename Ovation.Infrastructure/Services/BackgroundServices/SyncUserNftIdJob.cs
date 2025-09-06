using Microsoft.EntityFrameworkCore;
using Ovation.Application.Constants;
using Ovation.Persistence.Data;
using Quartz;

namespace Ovation.Persistence.Services.BackgroundServices
{
    [DisallowConcurrentExecution]
    sealed class SyncUserNftIdJob(OvationDbContext dbContext) : IJob
    {
        internal const string Name = nameof(SyncUserNftIdJob);
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                foreach (var item in Constant._chains)
                {
                    await dbContext.Database.ExecuteSqlRawAsync($@"
                        UPDATE user_nft_data
                        JOIN nfts_data ON user_nft_data.ContractAddress = nfts_data.ContractAddress 
                        AND user_nft_data.Type = nfts_data.Type 
                        AND user_nft_data.TokenId = nfts_data.TokenId
                        SET user_nft_data.NftId = nfts_data.Id
                        WHERE user_nft_data.Type = '{item}'
                    ");
                }

                await dbContext.Database.ExecuteSqlRawAsync($@"
                    UPDATE user_nft_data
                    JOIN nfts_data ON user_nft_data.Name = nfts_data.Name 
                    AND user_nft_data.Type = nfts_data.Type 
                    AND user_nft_data.TokenAddress = nfts_data.TokenAddress
                    SET user_nft_data.NftId = nfts_data.Id
                    WHERE user_nft_data.Type = 'solana'
                ");
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
            }
        }
    }
}
