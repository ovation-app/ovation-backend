using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.SignUp;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;
using Ovation.Persistence.Services.Clients;
using Ovation.Persistence.Services.Clients.NFTScan;

namespace Ovation.Persistence.Repositories
{
    class HomeRepository(IServiceScopeFactory serviceScopeFactory) : BaseRepository<UserProfile>(serviceScopeFactory), IHomeRepository
    {
        public async Task<ResponseData> GetNfts()
        {
            var response = new ResponseData();

            try
            {
                response.Data = await _context.UserNftData
                    .AsSplitQuery()
                    .Where(_ => _.Favorite == active && _.Public == active && _.ImageUrl != null)
                    .OrderByDescending(_ => _.LastTradePrice)
                    .Take(30)
                    .Select(x => new
                    {
                        x.Name,
                        x.Description,
                        x.ImageUrl,
                        x.AnimationUrl,
                        Chain = x.Chain
                    })
                    .ToListAsync();

                response.Status = true;

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetNftsFromWallet(WalletAcct walletAcct)
        {
            try
            {
                var nftData = new List<WalletNftsCount>();
                var tasks = new List<Task<List<WalletNftsCount>>>();
                var walletWithNfts = 0; 

                foreach (var data in walletAcct.Data)
                {
                    if (string.IsNullOrEmpty(data.WalletAddress) || string.IsNullOrEmpty(data.Chain))
                    {
                        return new ResponseData
                        {
                            Message = "Invalid wallet address or chain.",
                            Status = false
                        };
                    }

                    tasks.Add(Task.Run(async () =>
                    {
                        var resultList = new List<WalletNftsCount>();
                        switch (data.Chain)
                        {
                            case Constant.Solana:
                                using (var scope = _serviceScopeFactory.CreateScope())
                                {
                                    var solana = scope.ServiceProvider.GetRequiredService<SolanaService>();
                                    var count = await solana.GetNftCountAsync(data.WalletAddress);
                                    
                                    if (count > 0)
                                    { 
                                        walletWithNfts++;
                                        resultList.Add(new WalletNftsCount(data.Chain, count));
                                    }
                                }
                                break;
                            case Constant.Archway:
                                using (var scope = _serviceScopeFactory.CreateScope())
                                {
                                    var archway = scope.ServiceProvider.GetRequiredService<ArchwayService>();
                                    var count = await archway.GetNftCountAsync(data.WalletAddress);

                                    if (count > 0)
                                    {
                                        walletWithNfts++;
                                        resultList.Add(new WalletNftsCount(data.Chain, count));
                                    }
                                }
                                break;
                            case Constant.Tezos:
                                using (var scope = _serviceScopeFactory.CreateScope())
                                {
                                    var tezos = scope.ServiceProvider.GetRequiredService<TezosService>();
                                    var count = await tezos.GetNftCountAsync(data.WalletAddress);

                                    if (count > 0)
                                    {
                                        walletWithNfts++;
                                        resultList.Add(new WalletNftsCount(data.Chain, count));
                                    }
                                }
                                break;
                            case Constant.Abstract:
                                using (var scope = _serviceScopeFactory.CreateScope())
                                {
                                    var abstractService = scope.ServiceProvider.GetRequiredService<AbstractService>();
                                    var count = await abstractService.GetNftCountAsync(data.WalletAddress);

                                    if (count > 0)
                                    {
                                        walletWithNfts++;
                                        resultList.Add(new WalletNftsCount(data.Chain, count));
                                    }
                                }
                                break;
                            default:
                                using (var scope = _serviceScopeFactory.CreateScope())
                                {
                                    var evm = scope.ServiceProvider.GetRequiredService<EvmsService>();
                                    if (data.Chain.Equals(Constant.Evm, StringComparison.OrdinalIgnoreCase))
                                    {
                                        var evmTasks = Constant._alchemyChains.Select(async item =>
                                        {
                                            var count = await evm.GetNftCountAsync(data.WalletAddress, item);
                                            return count > 0 ? new WalletNftsCount(item, count) : null;
                                        });
                                        var results = await Task.WhenAll(evmTasks);
                                        resultList.AddRange(results.Where(r => r != null)!);
                                        
                                        if(results.Any(r => r != null))
                                        {
                                            walletWithNfts++;
                                        }
                                    }
                                    else
                                    {
                                        var count = await evm.GetNftCountAsync(data.WalletAddress, data.Chain);

                                        if (count > 0)
                                        {
                                            walletWithNfts++;
                                            resultList.Add(new WalletNftsCount(data.Chain, count));
                                        }
                                    }
                                }
                                break;
                        }
                        return resultList;
                    }));
                }

                var results = await Task.WhenAll(tasks);
                nftData.AddRange(results.SelectMany(r => r).Where(r => r != null)!);

                int profileCompletion = CalculateProfileCompletion(nftData, walletWithNfts);

                return new ResponseData
                {
                    Data = nftData,
                    IntValue = profileCompletion,
                    Status = true
                };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        private int CalculateProfileCompletion(List<WalletNftsCount> nftData, int walletWithNfts)
        {
            int connectedWalletsWithNfts = nftData
                .Where(w => w.Count > 0)
                .Select(w => w.Chain)
                .Count();

            int totalNfts = nftData.Sum(w => w.Count);

            // Gold Standard: 100+ NFTs = 100%
            if (totalNfts >= 100)
                return 100;

            // NFT Wallet Connection: 10% per wallet with NFTs, up to 5 wallets (max 50%)
            int walletScore = Math.Min(walletWithNfts, 5) * 10;

            // NFT Collection Depth: 5% per 10 NFTs, up to 50% (max 100 NFTs counted)
            int nftScore = Math.Min(totalNfts / 10, 10) * 5;

            // Total completion (max 100%)
            int total = walletScore + nftScore;
            return Math.Min(total, 100);
        }

        public async Task<ResponseData> GetUsers()
        {
            var response = new ResponseData();

            try
            {
                response.Data = await _context.Users
                    .Include(_ => _.UserProfile)
                    .Include(_ => _.UserStat)
                    .AsSplitQuery()
                    .Where(_ => _.Active == active && _.UserProfile.ProfileImage != null 
                    && _.UserProfile.ProfileImage != "")
                    .OrderByDescending(_ => _.UserStat.TotalNft)
                    .Take(30)
                    .Select(x => new
                    {
                        x.Username,
                        x.UserProfile.DisplayName,
                        x.UserProfile.ProfileImage
                    })
                    .ToListAsync();

                response.Status = true;

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }
    }
}
