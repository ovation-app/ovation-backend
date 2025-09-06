using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Domain.Entities;
using Quartz;
using System.Globalization;

namespace Ovation.Persistence.Services.BackgroundServices
{
    class DeleteWalletDataJob(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseJobService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(DeleteWalletDataJob);
        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            if (jobData == null)
                return;

            _ = jobData.TryGetString("WalletId", out var walletId);
            _ = jobData.TryGetString("Group", out var action);

            if (string.IsNullOrEmpty(walletId)) return;
            
            var id = new Guid(walletId);

            if (string.IsNullOrEmpty(action))
            {
                await DeleteWalletAsync(id);
            }
            else
            {
                var wallets = await _context.UserWallets.Where(_ => _.WalletGroupId == id.ToByteArray())
                    .Select(x => x.Id)
                    .ToListAsync();

                if(wallets != null && wallets.Count > 0)
                {
                    foreach (var item in wallets)
                    {
                        var itemId = new Guid(item);

                        await DeleteWalletAsync(itemId);
                    }
                }
            }

            
        }

        private async Task DeleteWalletAsync(Guid id)
        {
            var wallet = await _context.UserWallets
                   .FirstOrDefaultAsync(_ => _.Id == id.ToByteArray());

            if (wallet == null) return;            

            var stats = await _context.UserStats
                .FirstOrDefaultAsync(_ => _.UserId == wallet.UserId);

            if (stats == null) return;

            var highestValuedNft = await _context.UserHighestNfts
                .AsNoTracking().FirstOrDefaultAsync(_ => _.WalletId == id.ToByteArray());
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var nfts = await _context.UserNftData
                    .Where(_ => _.UserId == wallet.UserId && _.UserWalletId == wallet.Id)
                    .AsNoTracking()
                    .OrderBy(_ => _.Id)
                    .Take(20000)
                    .ToListAsync();

                var collections = await _context.UserNftCollectionData
                    .Where(_ => _.UserId == wallet.UserId && _.UserWalletId == wallet.Id)
                    .CountAsync();

                var worth = 0.00M;
                var nativeValue = 0.00M;
                var values = new List<NFTValue>();
                var founderNftCount = 0;
                var nftCreated = 0;

                if (nfts != null && nfts.Count > 0)
                {
                    foreach (var nft in nfts)
                    {
                        if(nft.Chain == Constant.Solana || nft.Chain == Constant.Tezos)
                        {
                            if (!string.IsNullOrEmpty(nft.LastTradePrice))
                            {
                                decimal.TryParse(nft.LastTradePrice, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal price);
                                var symbol = !string.IsNullOrEmpty(nft.LastTradeSymbol) ? nft.LastTradeSymbol!.Trim().ToLower() : nft.Chain!.Trim().ToLower();

                                nativeValue += price;
                                var usd = Math.Round(price * Constant._chainsToValue.GetValueOrDefault(symbol, 0.00M), 2);

                                worth += usd; // * Constant._chainsToValue[nft.Type!];

                                values.Add(new NFTValue
                                {
                                    Chain = nft.Chain,
                                    Native = decimal.Parse(nft.LastTradePrice, NumberStyles.Float, CultureInfo.InvariantCulture),
                                    Usd = usd,
                                    ImageUrl = !string.IsNullOrEmpty(nft.AnimationUrl) ? nft.AnimationUrl : nft.ImageUrl,
                                    wallet = nft.UserWalletId,
                                    Name = nft.Name,
                                    TradeSymbol = nft.LastTradeSymbol
                                });
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(nft.FloorPrice))
                                {
                                    decimal.TryParse(nft.FloorPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal price);
                                    var symbol = nft.Chain!.Trim().ToLower();

                                    nativeValue += price;
                                    var usd = Math.Round(price * Constant._chainsToValueFloor.GetValueOrDefault(symbol, 0.00M), 2);

                                    worth += usd; // * Constant._chainsToValue[nft.Type!];

                                    values.Add(new NFTValue
                                    {
                                        Chain = nft.Chain,
                                        Native = decimal.Parse(nft.LastTradePrice, NumberStyles.Float, CultureInfo.InvariantCulture),
                                        Usd = usd,
                                        ImageUrl = !string.IsNullOrEmpty(nft.AnimationUrl) ? nft.AnimationUrl : nft.ImageUrl,
                                        wallet = nft.UserWalletId,
                                        Name = nft.Name,
                                        TradeSymbol = nft.LastTradeSymbol
                                    });
                                }
                            }
                        }
                        

                        if (nft.Chain.Equals("eth", StringComparison.OrdinalIgnoreCase)
                            && nft.ContractAddress.Equals(Constant._founderNft, StringComparison.OrdinalIgnoreCase)
                            && nft.TokenId.Equals(Constant._founderNftTokenId, StringComparison.OrdinalIgnoreCase))
                            founderNftCount += 1;
                        if (nft.Created == 1)
                            nftCreated += 1;
                    }

                    if (await CountUserWallets(wallet.UserId) == 1)
                    {
                        stats.TotalNft = 0;
                        stats.Networth = 0M;
                        stats.NftCreated = 0;
                        stats.FounderNft = 0;
                        stats.BluechipCount = 0;
                        stats.SoldNftsTotal = 0;
                        stats.SoldNftsValue = 0;
                        stats.NftCollections = 0;
                    }
                    else
                    {
                        var blueChipCount = await _context.UserBlueChips.Where(_ => _.UserWalletId == id.ToByteArray()).CountAsync();

                        stats.TotalNft -= nfts.Count;
                        stats.Networth -= !string.IsNullOrEmpty(wallet.NftsValue) ? decimal.Parse(wallet.NftsValue) : 0M;
                        stats.NftCreated -= nftCreated;
                        stats.FounderNft -= founderNftCount;
                        stats.BluechipCount -= blueChipCount;
                        stats.SoldNftsTotal -= (wallet.TotalSold != null) ? wallet.TotalSold.Value : 0;
                        stats.SoldNftsValue -= (wallet.TotalSales != null) ? wallet.TotalSales.Value : 0;
                        stats.NftCollections -= collections;
                    }                        

                    var mul = new List<PortfolioValueRecord>();
                    mul.AddRange(values.Select(a => new PortfolioValueRecord { Value = a.Native, Symbol = a.TradeSymbol }));

                    await _context.UserPortfolioRecords.AddAsync(new UserPortfolioRecord
                    {
                        UserId = wallet.UserId,
                        Value = values.Sum(a => a.Native).ToString("F8"),
                        Address = wallet.WalletAddress,
                        Chain = wallet.Chain,
                        UsdValue = 0 - (!string.IsNullOrEmpty(wallet.NftsValue) ? decimal.Parse(wallet.NftsValue) : 0M),
                        MultiValue = JsonConvert.SerializeObject(mul)
                    });

                    await _context.UserNftRecords.AddAsync(new UserNftRecord
                    {
                        NftCount = 0 - nfts.Count,
                        UserId = wallet.UserId,
                        Address = wallet.WalletAddress,
                        Chain = wallet.Chain
                    });
                }

                await HandleGroupDeletion(wallet.WalletGroupId, wallet.UserId);

                _context.UserWallets.Remove(wallet);

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return;
            }

            if (highestValuedNft != null)
            {
                await UpdateUserHighestValuedNft(highestValuedNft.UserId);
            }
            else
            {
                await UpdateUserStatsRelatedBadge(stats, new Guid(wallet.UserId), null);

                await UpdateBadgeCount(wallet.UserId);
            }
        }

        private async Task<int> CountUserWallets(byte[] userId)
        {
            return await _context.UserWallets
                .Where(_ => _.UserId == userId).CountAsync();
        }

        private async Task HandleGroupDeletion(byte[]? walletGroupId, byte[] userId)
        {
            var wallets = await _context.UserWallets
                    .Where(_ => _.WalletGroupId == walletGroupId && _.UserId == userId)
                    .ToListAsync();

            if (wallets.Count == 1)
            {
                var group = await _context.UserWalletGroups.IgnoreAutoIncludes().FirstOrDefaultAsync(_ => _.Id == walletGroupId);

                if (group != null)
                {
                    _context.UserWalletGroups.Remove(group);
                }

            }
        }
    }
}
