using Microsoft.EntityFrameworkCore;
using Ovation.Application.Constants;
using Ovation.Persistence.Data;
using Quartz;

namespace Ovation.Persistence.Services.BackgroundServices
{
    [DisallowConcurrentExecution]
    class GetUserNftCustodyDateJob(OvationDbContext _context) : IJob
    {
        internal const string Name = nameof(GetUserNftCustodyDateJob);
        public async Task Execute(IJobExecutionContext context)
        {
            
            try
			{
                var wallets = await _context.UserWallets
                .Select(x => new
                {
                    x.UserId,
                    x.Id,
                    x.WalletAddress
                })
                .ToListAsync();

                if (wallets != null && wallets.Count > 0)
                {
                    foreach (var wallet in wallets)
                    {
                        var transactions = await _context.UserNftTransactions
                            .Where(_ => _.UserWalletId == wallet.Id)
                            .Select(x => new
                            {
                                x.Name,
                                x.TranxDate,
                                x.Chain,
                                x.ContractAddress,
                                x.ContractName,
                                x.ContractTokenId,
                                x.TokenId,
                                x.To
                            })
                            .ToListAsync();

                        if (transactions != null && transactions.Count > 0)
                        {
                            var nfts = await _context.UserNftData
                                .Where(_ => _.UserWalletId == wallet.Id)
                                .ToListAsync();

                            if (nfts != null && nfts.Count > 0)
                            {
                                foreach (var nft in nfts)
                                {
                                    try
                                    {
                                        var trnxDate = transactions.FirstOrDefault(_ => _.Chain == nft.Chain && _.To == wallet.WalletAddress &&
                                        (nft.Chain == Constant.Solana) ? _.ContractTokenId == nft.TokenAddress && _.Name == nft.Name :
                                            _.ContractAddress == nft.ContractAddress && (_.TokenId == nft.TokenId || _.ContractTokenId == nft.TokenAddress))?.TranxDate;

                                        if (trnxDate != null)
                                            nft.CustodyDate = DateOnly.FromDateTime(trnxDate.Value);
                                    }
                                    catch (Exception _)
                                    {
                                        continue;
                                    }
                                }

                                await _context.SaveChangesAsync();
                            }
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
