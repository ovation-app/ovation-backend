using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Constants;
using Ovation.Domain.Entities;
using Quartz;
using System.Globalization;

namespace Ovation.Persistence.Services.BackgroundServices
{
    class NftPrivacyChangedDataJob(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseJobService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(NftPrivacyChangedDataJob);
        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            if (jobData == null)
                return;

            var userId = jobData.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
                return;

            await SyncStatData(new Guid(userId).ToByteArray());
        }

        private async Task SyncStatData(byte[] userId)
        {
            await _unitOfWork.BeginTransactionAsync();

            var nfts = await _context.UserNftData.Where(_ => _.UserId == userId && _.Public == 1)
                .AsNoTracking().Take(6000).ToListAsync();

            var netWorth = 0.00M;

            try
            {
                if (nfts != null && nfts.Count > 0)
                {
                    var topNft = new UserNftDatum();
                    var value = 0.00M;
                    var native = 0.00M;
                    foreach (var nft in nfts)
                    {
                        if (nft.Chain == Constant.Solana || nft.Chain == Constant.Tezos)
                        {
                            if (!string.IsNullOrEmpty(nft.LastTradePrice))
                            {
                                if (topNft == null)
                                {
                                    topNft = nft;
                                    value = Math.Round(decimal.Parse(nft.LastTradePrice ?? "0.00", NumberStyles.Float, CultureInfo.InvariantCulture) *
                                        (decimal)Constant._chainsToValue[nft.Chain!.Trim().ToLower()], 2);

                                    native = decimal.Parse(nft.LastTradePrice ?? "0.00", NumberStyles.Float, CultureInfo.InvariantCulture);

                                    netWorth += value;
                                }
                                else
                                {
                                    var newValue = Math.Round(decimal.Parse(nft.LastTradePrice ?? "0.00", NumberStyles.Float, CultureInfo.InvariantCulture) *
                                        (decimal)Constant._chainsToValue[nft.Chain!.Trim().ToLower()], 2);

                                    netWorth += newValue;

                                    if (newValue > value)
                                    {
                                        topNft = nft;
                                        value = newValue;

                                        native = decimal.Parse(nft.LastTradePrice ?? "0.00", NumberStyles.Float, CultureInfo.InvariantCulture);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(nft.FloorPrice))
                            {
                                if (topNft == null)
                                {
                                    topNft = nft;
                                    value = Math.Round(decimal.Parse(nft.FloorPrice, NumberStyles.Float, CultureInfo.InvariantCulture) *
                                        (decimal)Constant._chainsToValueFloor[nft.Chain!.Trim().ToLower()], 2);

                                    netWorth += value;

                                    native = decimal.Parse(nft.FloorPrice, NumberStyles.Float, CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    var newValue = Math.Round(decimal.Parse(nft.FloorPrice, NumberStyles.Float, CultureInfo.InvariantCulture) *
                                        (decimal)Constant._chainsToValueFloor[nft.Chain!.Trim().ToLower()], 2);

                                    if (newValue > value)
                                    {
                                        topNft = nft;
                                        value = newValue;
                                        native = decimal.Parse(nft.FloorPrice, NumberStyles.Float, CultureInfo.InvariantCulture);
                                    }

                                    netWorth += newValue;
                                }
                            }
                        }
                    }

                    var stat = await _context.UserStats.FirstOrDefaultAsync(_ => _.UserId == userId);

                    if (value > 0)
                    {
                        var highNft = await _context.UserHighestNfts.FirstOrDefaultAsync(_ => _.UserId == userId);

                        if (highNft != null)
                        {

                            highNft.Worth = native;
                            highNft.WalletId = topNft.UserWalletId;
                            highNft.Chain = topNft.Chain;
                            highNft.UserId = userId;
                            highNft.Name = topNft.Name;
                            highNft.ImageUrl = topNft.ImageUrl;
                            highNft.TradeSymbol = topNft.LastTradeSymbol;
                            highNft.Usd = value;
                            highNft.NftId = topNft.Id;
                        }
                        else
                        {
                            var userHighNft = new UserHighestNft
                            {
                                Worth = native,
                                WalletId = topNft.UserWalletId,
                                Chain = topNft.Chain,
                                UserId = userId,
                                Name = topNft.Name,
                                ImageUrl = topNft.ImageUrl,
                                TradeSymbol = topNft.LastTradeSymbol,
                                Usd = value,
                                NftId = topNft.Id
                            };

                            await _context.UserHighestNfts.AddAsync(userHighNft);
                        }
                    }

                    if (stat != null)
                    {
                        stat.Networth = netWorth;
                        stat.TotalNft = nfts.Count();
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    await UpdateUserStatsRelatedBadge(stat, new Guid(userId), value);

                    await UpdateBadgeCount(userId);
                }
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
            }
        }
    }
}
