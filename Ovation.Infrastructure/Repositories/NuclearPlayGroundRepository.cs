using CoreTweet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.Enums;
using Ovation.Application.DTOs.Enums.Badges;
using Ovation.Application.DTOs.Tezos;
using Ovation.Application.DTOs.X;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;
using Ovation.Persistence.Common.Interfaces.Apis;
using Ovation.Persistence.Services;
using Ovation.Persistence.Services.BackgroundServices;
using Quartz;
using System.Globalization;
using System.Net.Http.Json;

namespace Ovation.Persistence.Repositories
{
    class NuclearPlayGroundRepository(IServiceScopeFactory serviceScopeFactory, IHttpClientFactory factory, IDappRadar dappRadar) : BaseRepository<Newsletter>(serviceScopeFactory), INuclearPlayGroundRepository
    {
        private async Task AddCollectionDetailsObjktAsync(string contractAddress, int ownsTotal, byte[] walletId, byte[] userId)
        {
            var clientt = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://data.objkt.com/v3/graphql");
            var content = new StringContent("query { fa(where: {contract: {_eq: \"" + contractAddress + "\"}}) { name description floor_price items logo short_name } }", null, "application/json");
            request.Content = content;

            try
            {
                HttpResponseMessage response = await clientt.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<TezosObjktCollectionData?>(jsonResponse);
                    var coll = data?.Data?.Fa?.FirstOrDefault();

                    if (coll != null)
                    {
                        var collection = new UserNftCollectionDatum
                        {
                            CollectionId = Guid.NewGuid().ToByteArray(),
                            ContractName = coll.Name,
                            ContractAddress = contractAddress,
                            Description = coll.Description,
                            ItemTotal = coll.Items,
                            OwnsTotal = ownsTotal,
                            FloorPrice = (coll.FloorPrice / Constant.Microtez).ToString(),
                            LogoUrl = coll.Logo,
                            UserWalletId = walletId,
                            Symbol = coll.ShortName,
                            Chain = Constant.Tezos,
                            UserId = userId,
                        };
                        await _context.UserNftCollectionData.AddAsync(collection);
                        await _context.SaveChangesAsync();

                        await _context.UserNftData
                    .Where(w => w.Chain == Constant.Tezos && w.UserId == userId && w.ContractAddress == contractAddress && w.UserWalletId == walletId)
                    .ExecuteUpdateAsync(s => s.SetProperty(b => b.CollectionId, collection.Id));
                    }
                    else
                    {
                        await AddCollectionAsync(contractAddress, ownsTotal, walletId, userId);
                    }
                }
                else
                {
                    await AddCollectionAsync(contractAddress, ownsTotal, walletId, userId);
                }
            }
            catch (Exception _)
            {
            }
        }

        private async Task AddCollectionAsync(string contractAddress, int ownsTotal, byte[] walletId, byte[] userId)
        {
            try
            {
                var client = factory.CreateClient(Constant.Tezos!);

                var response = await client
                    .GetFromJsonAsync<TezosCollectionData>($"v1/accounts/{contractAddress}?legacy=false");

                if (response != null)
                {
                    var collection = new UserNftCollectionDatum
                    {
                        CollectionId = Guid.NewGuid().ToByteArray(),
                        ContractName = response?.Metadata?.Name,
                        ContractAddress = contractAddress,
                        Description = response?.Metadata?.Description,
                        ItemTotal = response?.TokensCount,
                        OwnsTotal = ownsTotal,
                        LogoUrl = response?.Metadata?.ImageUri,
                        UserWalletId = walletId,
                        Chain = Constant.Tezos,
                        UserId = userId,
                    };
                    await _context.UserNftCollectionData.AddAsync(collection);
                    await _context.SaveChangesAsync();

                    await _context.UserNftData
                    .Where(w => w.Chain == Constant.Tezos && w.UserId == userId && w.ContractAddress == contractAddress && w.UserWalletId == walletId)
                    .ExecuteUpdateAsync(s => s.SetProperty(b => b.CollectionId, collection.Id));
                }
            }
            catch (Exception _)
            {
            }
        }

        private async Task AddCollectionAsync(ArchwayCollection coll, byte[] walletId, byte[] userId, int owned)
        {
            var collection = new UserNftCollectionDatum
            {
                CollectionId = Guid.NewGuid().ToByteArray(),
                ContractName = coll.CollectionName,
                ContractAddress = coll.ContractAddress,
                Description = "",
                ItemTotal = coll.Supply,
                OwnsTotal = owned,
                LogoUrl = coll.Image,
                UserWalletId = walletId,
                Chain = Constant.Archway,
                UserId = userId,
            };
            await _context.UserNftCollectionData.AddAsync(collection);
            await _context.SaveChangesAsync();

            await _context.UserNftData
            .Where(w => w.Chain == Constant.Archway && w.UserId == userId && w.ContractAddress == coll.ContractAddress && w.UserWalletId == walletId)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.CollectionId, collection.Id));
        }

        public async Task NFTDataAsync(int page)
        {
            //await UpdateUsersPortfolioRecord();
            //await SyncSalesData(page);
            //await SyncWalletGroupAsync();
            //await UpdateAllUserHighestNft();
            //await PullDappRadarCollectionData();
            //await UpdateUserCollectionFloorPrice();
            //await AddNftToTestAccount();

            var x = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<XService>();

            await x.XMarketingSchemeAsync();
        }

        private async Task HandleSolana(string collection)
        {
            try
            {
                var scope = _serviceScopeFactory.CreateScope();

                var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();

                IScheduler schedular = await schedulerFactory.GetScheduler();

                var jobData = new JobDataMap
                {
                    {"Collection", collection },
                    {"Chain", Constant.Solana}
                };

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity($"get-solana-collection-data-{collection}")
                    .ForJob(GetSolanaCollectionDataJob.Name)
                    .UsingJobData(jobData)
                    .StartNow()
                    .Build();

                await schedular.ScheduleJob(trigger);
            }
            catch (Exception)
            {

            }
        }

        private async Task HandleEvms(string contract, string chain)
        {
            try
            {
                var scope = _serviceScopeFactory.CreateScope();
                var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();

                IScheduler schedular = await schedulerFactory.GetScheduler();

                var jobData = new JobDataMap
                {
                    {"ContractAddress", contract },
                    {"Chain", chain}
                };

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity($"get-evm-collection-data-{contract}")
                    .ForJob(GetEvmsCollectionDataJob.Name)
                    .UsingJobData(jobData)
                    .StartNow()
                    .Build();

                await schedular.ScheduleJob(trigger);
            }
            catch (Exception)
            {

            }
        }

        private async Task HandleTon(string contract, string chain)
        {
            try
            {
                var scope = _serviceScopeFactory.CreateScope();
                var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();

                IScheduler schedular = await schedulerFactory.GetScheduler();

                var jobData = new JobDataMap
                {
                    {"ContractAddress", contract },
                    {"Chain", chain}
                };

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity($"get-ton-collection-data-{contract}")
                    .ForJob(GetTonCollectionDataJob.Name)
                    .UsingJobData(jobData)
                    .StartNow()
                    .Build();

                await schedular.ScheduleJob(trigger);
            }
            catch (Exception)
            {

            }
        }


        private async Task HandleTezos(string contract, string chain)
        {
            try
            {
                var scope = _serviceScopeFactory.CreateScope();
                var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();

                IScheduler schedular = await schedulerFactory.GetScheduler();

                var jobData = new JobDataMap
                {
                    {"ContractAddress", contract },
                    {"Chain", chain}
                };

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity($"get-tezos-collection-data-{contract}")
                    .ForJob(GetTezosCollectionDataJob.Name)
                    .UsingJobData(jobData)
                    .StartNow()
                    .Build();

                await schedular.ScheduleJob(trigger);
            }
            catch (Exception)
            {

            }
        }

        private async Task UpdateFavNftFromOldTableToNftDataTable()
        {
            var favs = await _context.UserFavoriteNfts
                .Where(_ => _.FavoriteNfts != null)
                .Select(_ => new
                {
                    _.FavoriteNfts,
                    _.UserId
                })
                .ToListAsync();

            if (favs != null)
            {
                foreach (var item in favs)
                {
                    var nft = JsonConvert.DeserializeObject<List<FavNfts>>(item.FavoriteNfts);
                    if (nft.Count > 0)
                    {
                        foreach (var fav in nft)
                        {
                            var nf = await _context.UserNftData
                                .Where(n => n.ContractAddress == fav.ContractAddress && (fav.Type == Constant.Solana) ? n.TokenAddress == fav.TokenId : n.TokenId == fav.TokenId
                                && n.Chain == fav.Type && n.Name == fav.Name && n.UserId == item.UserId)
                                .FirstOrDefaultAsync();
                            if (nf != null)
                            {
                                nf.Favorite = 1;

                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
        }

        private async Task GetSpecificCollectionNdNftForTezos()
        {
            var nfts = new List<string> { "KT1RJ6PbjHpwc3M5rw5s2Nbmefwbuwbdxton", "KT1U6EHmNxJTkvaWJ4ThczG4FSDaHC21ssvi" };

            foreach (var nft in nfts)
            {
                if (!string.IsNullOrEmpty(nft))
                    await HandleTezos(nft, Constant.Tezos);
            }
        }

        private async Task UpdateTezosNftsContractAddressColumn()
        {
            var res = await _context.UserNftData
                .Where(t => t.Chain == Constant.Tezos)
                .ToListAsync();

            if (res != null)
            {
                await _unitOfWork.BeginTransactionAsync();
                foreach (var n in res)
                {
                    if (!string.IsNullOrEmpty(n.MetaData))
                    {
                        var meta = JsonConvert.DeserializeObject<TezosNft>(n.MetaData);
                        n.ContractAddress = meta?.Token?.Contract?.Address?.Trim();
                    }
                }
                var rows = await _context.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
        }

        private async Task AddUserArchwayCollectionToUserCollectionTable()
        {
            var nfts = await _context.UserNftData
                .Where(_ => _.Chain == Constant.Archway && _.ContractAddress != null && _.CollectionId == null)
                .GroupBy(n => new { n.ContractAddress, n.UserId, n.UserWalletId })
                .OrderBy(_ => _.Key.UserId)
                .Take(1000)
                .Select(g => new
                {
                    g.Key.ContractAddress,
                    g.Key.UserId,
                    WalletId = g.Key.UserWalletId,
                    TotalNfts = g.Count()
                })
                .ToListAsync();

            if (nfts != null)
            {
                foreach (var nft in nfts)
                {
                    var entity = await _context.UserNftCollectionData
                         .FirstOrDefaultAsync(u => u.ContractAddress == nft.ContractAddress && u.UserId == nft.UserId && u.UserWalletId == nft.WalletId);

                    if (entity == null)
                    {
                        var coll = await _context.ArchwayCollections.FirstOrDefaultAsync(_ => _.ContractAddress == nft.ContractAddress);
                        if (coll != null)
                            await AddCollectionAsync(coll, nft.WalletId, nft.UserId, nft.TotalNfts);
                    }
                }

            }
        }

        private async Task GetAllUserTransactions(int page)
        {
            var wallets = await GetAllUserWallets(page);

            if (wallets != null)
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();

                var assetRepository = scope.ServiceProvider.GetRequiredService<IAssetRepository>();
                foreach (var wallet in wallets)
                {
                    var en = await _context.UserNftTransactions.FirstOrDefaultAsync(w => w.UserWalletId == wallet.Id);
                    if (en != null) continue;

                    Guid? id = (wallet.WalletId != null) ? new Guid(wallet.WalletId) : null;

                    await assetRepository.GetUserTransactions(new Guid(wallet.UserId), wallet.WalletAddress!, wallet.Chain!, id);
                }
            }
        }

        private async Task GetAllCollectionWithZeroChildCount()
        {
            var res = await _context.NftCollectionsData
                .Where(nftCollection => nftCollection.Chain != "archway")
                .GroupJoin(
                    _context.NftsData,
                    nftCollection => nftCollection.Id,
                    nft => nft.CollectionId,
                    (nftCollection, nfts) => new
                    {
                        nftCollection.ContractName,
                        nftCollection.ItemTotal,
                        ChildCount = nfts.Count(),
                        nftCollection.Chain
                    })
                .Where(x => x.ChildCount == 0)
            .ToListAsync();
        }

        private async Task GetCollectionAndNftData()
        {
            var res = await _context.UserNftCollectionData
                .Where(nftCollection => nftCollection.ParentCollection == null && nftCollection.ContractName != null && nftCollection.ContractAddress != null)
                //.DistinctBy(d => d.ContractAddress)
                .OrderBy(o => o.Id)
                //.Skip(1000 * (page - 1))
                .Take(3000)
                .Select(x => new
                {
                    x.Chain,
                    x.ContractName,
                    x.ContractAddress
                }).ToListAsync();

            if (res != null)
            {
                foreach (var result in res)
                {
                    switch (result.Chain)
                    {
                        case "solana":
                            if (!string.IsNullOrEmpty(result.ContractName))
                                await HandleSolana(result.ContractName);
                            break;

                        case "ton":
                            if (!string.IsNullOrEmpty(result.ContractAddress))
                                await HandleTon(result.ContractAddress, result.Chain);
                            break;

                        case "tezos":
                            if (!string.IsNullOrEmpty(result.ContractAddress))
                                await HandleTezos(result.ContractAddress, result.Chain);
                            break;
                        default:
                            if (!string.IsNullOrEmpty(result.ContractAddress))
                                await HandleEvms(result.ContractAddress, result.Chain);
                            break;
                    }

                }
            }
        }

        private async Task UpdateNftCustodyDate()
        {
            var wallets = await _context.UserWallets
                .Select(x => new
                {
                    x.UserId,
                    x.Id,
                    x.WalletAddress
                })
                .ToListAsync();

            if(wallets != null && wallets.Count > 0)
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

                    if(transactions != null && transactions.Count > 0)
                    {
                        var nfts = await _context.UserNftData
                            .Where(_ => _.UserWalletId == wallet.Id)
                            .ToListAsync();

                        if(nfts != null && nfts.Count > 0)
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

        internal async Task BulkUpdate()
        {
            try
            {
                await _context.UserWallets
                    .Where(w => w.Chain == Constant.Solana)
                    .ExecuteUpdateAsync(s => s.SetProperty(b => b.Blockchain, Constant.Solana));

                await _context.UserWallets
                    .Where(w => w.Chain == Constant.Ton)
                    .ExecuteUpdateAsync(s => s.SetProperty(b => b.Blockchain, Constant.Ton));

                await _context.UserWallets
                    .Where(w => w.Chain == Constant.Archway)
                    .ExecuteUpdateAsync(s => s.SetProperty(b => b.Blockchain, Constant.Archway));

                await _context.UserWallets
                    .Where(w => w.Chain == Constant.Tezos)
                    .ExecuteUpdateAsync(s => s.SetProperty(b => b.Blockchain, Constant.Tezos));

                await _context.UserWallets
                    .Where(w => w.Chain == Constant.Cosmos)
                    .ExecuteUpdateAsync(s => s.SetProperty(b => b.Blockchain, Constant.Cosmos));
            }
            catch (Exception _)
            {

            }
        }

        internal async Task BulkUpdate2()
        {
            try
            {
                await _context.UserWallets
                    .Where(w => w.Blockchain == null && w.Chain != null)
                    .ExecuteUpdateAsync(s => s.SetProperty(b => b.Blockchain, Constant.Evm));
            }
            catch (Exception _)
            {

            }
        }

        internal async Task<ResponseData> ResetStatsAsync(Guid userId)
        {
            var transaction = _context.Database.CurrentTransaction;

            if (transaction == null)
                transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var wallet = await _context.UserWallets
                    .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.Migrated == 1);

                if (wallet != null) return new ResponseData { Status = true, Message = "User already migrated" };

                var stats = await _context.UserStats
                        .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

                if (stats == null) return new ResponseData { Message = "User stats data not found" };

                stats.Networth = 0.00M;
                stats.TotalNft = 0;
                stats.NftCollections = 0;
                stats.FounderNft = 0;
                stats.NftCreated = 0;
                stats.BluechipCount = 0;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new ResponseData { Status = true, Message = "Stats data reset" };
            }
            catch (Exception _)
            {
                await transaction.RollbackAsync();
                return new();
            }
        }

        internal async Task UpdateNftRelatedStatAndBadge(byte[] userId)
        {
            var transaction = _context.Database.CurrentTransaction;

            if (transaction == null)
                transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //var inActibeBadges = await _context.UserBadges.Where(_ => _.UserId == userId && _.Active == 0).CountAsync();
                //var wallet = await _context.UserWallets
                //    .FirstOrDefaultAsync(_ => _.UserId == userId && _.Migrated == 1);

                //if (wallet != null) return new ResponseData { Status = true, Message = "User already migrated" };



                //if (stats == null) return new ResponseData { Message = "User stats data not found" };

                //stats.Networth = 0.00M;
                //stats.TotalNft = 0;
                //stats.NftCollections = 0;
                //stats.FounderNft = 0;
                //stats.NftCreated = 0;
                //stats.BluechipCount = 0;

                //if (inActibeBadges < 1)
                //    return new();

                //stats.BadgeEarned -= inActibeBadges;                

                var stats = await _context.UserStats
                        .FirstOrDefaultAsync(_ => _.UserId == userId);

                if (stats == null) return;

                var query = $"%{BadgeStruct.TopValueNft1k.Split('-').First().Trim()}%";
                var topnft = await _context.UserBadges
                .Where(_ => EF.Functions.Like(_.BadgeName, query) && _.UserId == userId && _.Active == 1)
                .ToListAsync();

                var query2 = $"%{BadgeStruct.PortfolioValue1k.Split('-').First().Trim()}%";
                var portfolio = await _context.UserBadges
                .Where(_ => EF.Functions.Like(_.BadgeName, query2) && _.UserId == userId && _.Active == 1)
                .ToListAsync();

                var query3 = $"%{BadgeStruct.NumberOfNft1.Split('-').First().Trim()}%";
                var totalNft = await _context.UserBadges
                .Where(_ => EF.Functions.Like(_.BadgeName, query3) && _.UserId == userId && _.Active == 1)
                .ToListAsync();

                var nfts = await _context.UserNftData
                    .Where(_ => _.UserId == userId)
                    .Take(4000)
                    .IgnoreAutoIncludes()
                    .ToListAsync();

                if (nfts == null || nfts.Count < 1)
                {
                    stats.Networth = 0.00M;
                    stats.BadgeEarned -= topnft.Count + portfolio.Count;
                }
                else
                {
                    var notif = new List<NotificationDto>();

                    var exchange = Constant._chainsToValue;
                    var values = new List<NFTValue>();
                    var worth = 0.00M;
                    foreach (var item in nfts)
                    {
                        if (!string.IsNullOrEmpty(item.LastTradePrice))
                        {
                            var usd = Math.Round(decimal.Parse(item.LastTradePrice, NumberStyles.Float, CultureInfo.InvariantCulture) * exchange.GetValueOrDefault(!string.IsNullOrEmpty(item.LastTradeSymbol) ? item.LastTradeSymbol!.Trim().ToLower() : item.Chain!.Trim().ToLower(), 0.00M), 2);
                            values.Add(new NFTValue
                            {
                                Chain = item.Chain,
                                Native = decimal.Parse(item.LastTradePrice, NumberStyles.Float, CultureInfo.InvariantCulture),
                                Usd = usd,
                                ImageUrl = !string.IsNullOrEmpty(item.AnimationUrl) ? item.AnimationUrl : item.ImageUrl,
                                wallet = item.UserWalletId,
                                Name = item.Name,
                                TradeSymbol = item.LastTradeSymbol
                            });

                            worth += usd;
                        }
                    }

                    var top = values.OrderByDescending(x => x.Usd).FirstOrDefault();

                    if (top != null)
                    {
                        stats.Networth = worth;
                        stats.TotalNft = nfts.Count;

                        var userTopNft = await _context.UserHighestNfts.FirstOrDefaultAsync(_ => _.UserId == userId);

                        if (userTopNft != null)
                        {
                            var newWorth = top.Usd;

                            var oldWorth = Math.Round((decimal)(userTopNft.Worth * Constant._chainsToValue.GetValueOrDefault(!string.IsNullOrEmpty(userTopNft.TradeSymbol) ? userTopNft.TradeSymbol!.Trim().ToLower() : userTopNft.Chain!.Trim().ToLower(), 0.00M))!, 2);

                            userTopNft.Worth = newWorth > oldWorth ? top.Native : userTopNft.Worth;
                            userTopNft.WalletId = top.wallet;
                            userTopNft.Chain = top.Chain;
                            userTopNft.UserId = userId;
                            userTopNft.Name = top.Name;
                            userTopNft.ImageUrl = top.ImageUrl;
                            userTopNft.TradeSymbol = top.TradeSymbol;
                            userTopNft.Usd = newWorth > oldWorth ? Math.Round(newWorth, 2) : Math.Round(oldWorth, 2);
                        }
                        else
                        {
                            var userHighNft = new UserHighestNft
                            {
                                Worth = top.Native,
                                WalletId = top.wallet,
                                Chain = top.Chain,
                                UserId = userId,
                                Name = top.Name,
                                ImageUrl = top.ImageUrl,
                                TradeSymbol = top.TradeSymbol,
                                Usd = Math.Round(top.Native * Constant._chainsToValue[top.Chain.Trim().ToLower()], 2)
                            };

                            await _context.UserHighestNfts.AddAsync(userHighNft);
                        }
                        var mul = new List<PortfolioValueRecord>();
                        mul.AddRange(values.Select(a => new PortfolioValueRecord { Value = a.Native, Symbol = a.TradeSymbol }));

                        await _context.UserPortfolioRecords.AddAsync(new UserPortfolioRecord
                        {
                            UserId = userId,
                            Value = values.Sum(a => a.Native).ToString("F8"),
                            Address = null,
                            Chain = "all",
                            UsdValue = worth,
                            MultiValue = JsonConvert.SerializeObject(mul)
                        });

                        var referenceType = NotificationReference.Badge;

                        if (top.Usd >= 100)
                        {
                            var notification = new NotificationDto
                            {
                                InitiatorId = null,
                                ReceiverId = new Guid(userId),
                                Message = Constant._NotificationMessage[referenceType],
                                Title = Constant._NotificationTitle[referenceType],
                                Reference = NotificationReference.Badge.ToString(),
                                ReferenceId = top.Usd <= 499 ? BadgeStruct.TopValueNft100 : top.Usd <= 999 ? BadgeStruct.TopValueNft500 : top.Usd <= 9999 ? BadgeStruct.TopValueNft1k :
                                top.Usd <= 24999 ? BadgeStruct.TopValueNft10k : top.Usd <= 49999 ? BadgeStruct.TopValueNft25k : top.Usd <= 99999 ? BadgeStruct.TopValueNft50k :
                                BadgeStruct.TopValueNft100k
                            };
                            notif.Add(notification);
                        }

                        if (worth >= 1000)
                        {
                            var notification = new NotificationDto
                            {
                                InitiatorId = null,
                                ReceiverId = new Guid(userId),
                                Message = Constant._NotificationMessage[referenceType],
                                Title = Constant._NotificationTitle[referenceType],
                                Reference = NotificationReference.Badge.ToString(),
                                ReferenceId = worth <= 4999 ? BadgeStruct.PortfolioValue1k : worth <= 9999 ?
                                BadgeStruct.PortfolioValue5k : worth <= 24999 ? BadgeStruct.PortfolioValue10k : worth <= 49999
                                ? BadgeStruct.PortfolioValue25k : worth <= 99999 ? BadgeStruct.PortfolioValue50k : BadgeStruct.PortfolioValue100k
                            };
                            notif.Add(notification);
                        }

                        if (stats.TotalNft >= 1)
                        {
                            var notification = new NotificationDto
                            {
                                InitiatorId = null,
                                ReceiverId = new Guid(userId),
                                Message = Constant._NotificationMessage[referenceType],
                                Title = Constant._NotificationTitle[referenceType],
                                Reference = NotificationReference.Badge.ToString(),
                                ReferenceId = stats.TotalNft <= 4 ? BadgeStruct.NumberOfNft1 : stats.TotalNft <= 9 ? BadgeStruct.NumberOfNft5 : stats.TotalNft <= 24 ? BadgeStruct.NumberOfNft10 :
                                stats.TotalNft <= 49 ? BadgeStruct.NumberOfNft25 : stats.TotalNft <= 99 ? BadgeStruct.NumberOfNft50 : stats.TotalNft <= 249 ? BadgeStruct.NumberOfNft100 :
                                stats.TotalNft <= 499 ? BadgeStruct.NumberOfNft250 : BadgeStruct.NumberOfNft500
                            };
                            notif.Add(notification);
                        }

                        foreach (var notification in notif)
                        {
                            var badge = new UserBadge
                            {
                                BadgeName = notification.ReferenceId!,
                                EarnedAt = DateTime.UtcNow,
                                Active = 1,
                                UserId = userId
                            };

                            await _context.UserBadges.AddAsync(badge);
                        }


                    }
                }

                if (topnft != null && topnft.Count > 0) _context.UserBadges.RemoveRange(topnft);
                if (portfolio != null && portfolio.Count > 0) _context.UserBadges.RemoveRange(portfolio);
                if (totalNft != null && totalNft.Count > 0) _context.UserBadges.RemoveRange(totalNft);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                //return new ResponseData { Status = true, Message = "Stats data reset" };
            }
            catch (Exception _)
            {
                await transaction.RollbackAsync();
                //return new();
            }
        }

        internal async Task<List<UserWallet>> GetUserWallets(byte[] userId)
        {
            return await _context.UserWallets.Where(_ => _.UserId == userId).ToListAsync();
        }

        internal async Task UpdateHighestNftUsdValue()
        {
            var res = await _context.UserHighestNfts.Take(perPage).ToListAsync();

            var transaction = _context.Database.CurrentTransaction;

            if (transaction == null)
                transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (res != null)
                {
                    foreach (var item in res)
                    {
                        var tradSym = !string.IsNullOrEmpty(item.TradeSymbol) ? item.TradeSymbol.Trim() : item.Chain;
                        item.Usd = Math.Round(item.Worth.Value * Constant._chainsToValue[tradSym!], 2);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch (Exception _)
            {
                await transaction.RollbackAsync();
            }
        }

        internal async Task UpdateStatAndBadge()
        {
            var users = await GetAllUsers();

            foreach (var userId in users)
            {
                //var fav = await GetFavNftsAsync(new Guid(userId));

                //if(fav != null && fav.Count > 0)
                //{
                //    var favData = await _context.UserFavoriteNfts.FirstOrDefaultAsync(_ => _.UserId == userId);

                //    if(favData != null)
                //    {
                //        favData.FavoriteNfts = JsonConvert.SerializeObject(fav);

                //        await _context.SaveChangesAsync();
                //    }
                //}

                await UpdateNftRelatedStatAndBadge(userId);

                var stats = await _context.UserStats
                        .FirstOrDefaultAsync(_ => _.UserId == userId);

                if (stats == null) continue;

                var bd = await _context.UserBadges.Where(_ => _.UserId == userId && _.Active == 1).CountAsync();

                stats.BadgeEarned = bd;

                await _context.SaveChangesAsync();
            }
        }

        private async Task SyncUsersHighestValuedNft()
        {
            var highest = await _context.UserHighestNfts
                .ToListAsync();

            if (highest != null && highest.Count > 0)
            {
                foreach (var item in highest)
                {
                    try
                    {
                        var nfts = await _context.UserNftData
                    .Where(_ => _.UserId == item.UserId)
                    .ToListAsync();

                        UserNftDatum? topNft = null;

                        if (nfts != null && nfts.Count > 0)
                        {
                            foreach (var nft in nfts)
                            {
                                if (!string.IsNullOrEmpty(nft.LastTradePrice))
                                {
                                    if (topNft == null)
                                    {
                                        topNft = nft;
                                    }
                                    else
                                    {
                                        var tradSymNew = !string.IsNullOrEmpty(nft.LastTradeSymbol) ? nft.LastTradeSymbol.Trim() : nft.Chain;
                                        var tradSym = !string.IsNullOrEmpty(topNft.LastTradeSymbol) ? topNft.LastTradeSymbol.Trim() : topNft.Chain;

                                        decimal.TryParse(topNft.LastTradePrice, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal topNftWorth);
                                        decimal.TryParse(nft.LastTradePrice, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal nftWorth);

                                        var newWorth = Math.Round(nftWorth * Constant._chainsToValue[tradSymNew?.Trim()?.ToLower()], 2);

                                        var oldWorth = Math.Round(topNftWorth * Constant._chainsToValue[tradSym?.Trim()?.ToLower()]!, 2);

                                        topNft = newWorth > oldWorth ? nft : topNft;
                                    }
                                }
                            }

                            decimal.TryParse(topNft.LastTradePrice, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal nftWorthh);
                            var tradSymm = !string.IsNullOrEmpty(topNft.LastTradeSymbol) ? topNft.LastTradeSymbol.Trim() : topNft.Chain;

                            item.Worth = nftWorthh;
                            item.WalletId = topNft.UserWalletId;
                            item.Chain = topNft.Chain;
                            item.UserId = topNft.UserId;
                            item.Name = topNft.Name;
                            item.ImageUrl = !string.IsNullOrEmpty(topNft.AnimationUrl) ? topNft.AnimationUrl  : topNft.ImageUrl;
                            item.TradeSymbol = topNft.LastTradeSymbol ?? topNft.Chain;
                            item.Usd = Math.Round(nftWorthh * Constant._chainsToValue[tradSymm?.Trim()?.ToLower()]!, 2);
                            item.NftId = topNft.Id;

                            _context.UserHighestNfts.Update(item);
                        }

                    }
                    catch (Exception _)
                    {
                        continue;
                    }

                    await _context.SaveChangesAsync();
                }
            }
        }


        private async Task<List<byte[]>> GetAllUsers()
        {
            return await _context.Users.Select(x => x.UserId).Take(900).ToListAsync();
        }

        private async Task<List<UserWallet>> GetAllUserWallets(int page)
        {
            return await _context.UserWallets.Where(w => w.Chain != Constant.Archway)
                .OrderBy(_ => _.CreatedDate).Skip(perPage * (page - 1)).Take(perPage).ToListAsync();
        }

        private async Task UpdateUserWalletNftCountAndValue(List<UserWallet> wallets)
        {
            foreach (var wallet in wallets)
            {
                try
                {
                    var nfts = await _context.UserNftData.Where(_ => _.UserWalletId == wallet.Id).Take(5000).ToListAsync(); ;

                    if (nfts != null && nfts.Count > 0)
                    {
                        var native = 0.00M;

                        foreach (var nft in nfts)
                        {
                            if (!string.IsNullOrEmpty(nft.LastTradePrice))
                            {
                                decimal.TryParse(nft.LastTradePrice, out decimal price);

                                native += price * Constant._chainsToValue[nft.Chain!];
                            }
                        }
                        wallet.NftCount = nfts.Count;
                        wallet.NftsValue = native.ToString("F2");

                        await _context.SaveChangesAsync();
                    }

                }
                catch (Exception _)
                {

                }
            }
        }

        internal async Task<List<byte[]>> GetAllUsers(int page)
        {
            return await _context.Users
                .OrderBy(p => p.CreatedDate)
                .Skip(1 * (page - 1))
                .Take(1)
                .Select(x => x.UserId).ToListAsync();
        }

        private async Task TestBadgeDuplicate()
        {
            var notification = new List<NotificationDto>
            {

                new NotificationDto
                {
                    InitiatorId = null,
                    ReceiverId = new Guid("91f6b18d-1525-49f9-bdc5-8af9f33a4e24"),
                    Message = Constant._NotificationMessage[NotificationReference.Badge],
                    Title = Constant._NotificationTitle[NotificationReference.Badge],
                    Reference = NotificationReference.Badge.ToString(),
                    ReferenceId = BadgeStruct.TopValueNft100k
                },
                new NotificationDto
                {
                    InitiatorId = null,
                    ReceiverId = new Guid("91f6b18d-1525-49f9-bdc5-8af9f33a4e24"),
                    Message = Constant._NotificationMessage[NotificationReference.Badge],
                    Title = Constant._NotificationTitle[NotificationReference.Badge],
                    Reference = NotificationReference.Badge.ToString(),
                    ReferenceId = BadgeStruct.ProfileComplete
                },
            };

            await SaveNotificationAsync(notification);
        }

        private async Task UpdateAllUserHighestNft()
        {

            var users = await GetAllUsers();

            if(users != null)
            {
                foreach (var userId in users)
                {
                    var nfts = await _context.UserNftData
                        .Where(_ => _.UserId == userId && _.Public == 1).AsNoTracking().Take(6000).ToListAsync();

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

                            if(value > 0)
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

                                await _context.UserPortfolioRecords.AddAsync(new UserPortfolioRecord
                                {
                                    UserId = stat.UserId,
                                    Value = "0.00",
                                    UsdValue = stat.Networth,
                                    Address = "all",
                                    Chain = "all",
                                });
                            }

                            await _context.SaveChangesAsync();

                            await UpdateUserStatsRelatedBadge(stat, new Guid(userId), value);

                            await UpdateBadgeCount(userId);

                        }
                    }
                    catch (Exception _)
                    {
                        continue;
                    }
                }
            }
        }

        private async Task UpdateUserCollectionFloorPrice()
        {
            var collections = await _context.UserNftCollectionData
                .Where(c => c.Chain != Constant.Solana && c.Chain != Constant.Archway && c.Chain != Constant.Tezos
                && c.Chain != Constant.Stargaze && c.Chain != Constant.Ton)
                .OrderBy(_ => _.Id)
                .Take(10000)
                .ToListAsync();

            try
            {
                //await _unitOfWork.BeginTransactionAsync();

                Dictionary<string, decimal?> collectionToFloor = new Dictionary<string, decimal?>(StringComparer.OrdinalIgnoreCase);
                
                var priceService = _serviceScopeFactory.CreateScope().ServiceProvider.GetService<CollectionPriceService>();
                var count = 0;
                foreach (var collection in collections)
                {
                    if (string.IsNullOrEmpty(collection.ContractName)
                        && string.IsNullOrEmpty(collection.ContractAddress)) continue;

                    if (collection.Chain!.Equals(Constant.Solana, StringComparison.OrdinalIgnoreCase)
                        && string.IsNullOrEmpty(collection.ContractName))
                        continue;

                    decimal? floorPrice = null;
                    bool isExist = false;

                    if(collection.Chain!.Equals(Constant.Solana, StringComparison.OrdinalIgnoreCase)
                        && collectionToFloor.TryGetValue(collection.ContractName, out decimal? price))
                    {
                        floorPrice = price;
                        isExist = true;
                    }else if(collectionToFloor.TryGetValue(collection.ContractAddress, out decimal? pricee))
                    {
                        floorPrice = pricee;
                        isExist = true;
                    }

                    if (priceService != null && !isExist) floorPrice =
                        await priceService.GetFloorPriceAsync(collection.Chain!, collection.ContractName, collection.ContractAddress);

                    collection.FloorPrice = floorPrice != null ? floorPrice.ToString() : null;

                    if (!isExist)
                    {
                        if (collection.Chain!.Equals(Constant.Solana, StringComparison.OrdinalIgnoreCase))
                            collectionToFloor.TryAdd(collection.ContractName, floorPrice);
                        else
                            collectionToFloor.TryAdd(collection.ContractAddress, floorPrice);
                    }

                    count++;
                    await _context.SaveChangesAsync();
                }

                
                //await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception _)
            {
                //await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        private async Task SyncSalesData(int page)
        {
            var wallets = await GetAllUserWallets(page);

            if(wallets != null)
            {
                //await _context.UserStats
                //    .Where(item => item.UserId != null)
                //    .ExecuteUpdateAsync(updates => updates
                //    .SetProperty(item => item.SoldNftsTotal, item => 0)
                //    .SetProperty(item => item.SoldNftsValue, item => 0.00M));
                try
                {
                    foreach (var wallet in wallets)
                    {
                        var stats = await _context.UserStats.FirstOrDefaultAsync(_ => _.UserId == wallet.UserId);

                        var transactions = await _context.UserNftTransactions
                            .Where(w => w.EventType == "Sale" && w.From == wallet.WalletAddress
                            && w.UserWalletId == wallet.Id)
                            .OrderBy(o => o.Id)
                            .Take(10000)
                            .ToListAsync();

                        var soldValue = 0.00M;
                        var soldCount = 0;

                        if (transactions != null && stats != null)
                        {
                            foreach (var transaction in transactions)
                            {
                                int qty = !string.IsNullOrEmpty(transaction.Qty.ToString()) ? (int)transaction.Qty : 1;
                                if (!string.IsNullOrEmpty(transaction.TradePrice) && decimal.TryParse(transaction.TradePrice, out decimal price))
                                {
                                    if (!string.IsNullOrEmpty(transaction.TradeSymbol) && Constant._chainsToValue.TryGetValue(transaction.TradeSymbol.Trim().ToLower(), out decimal rate))
                                        soldValue += Math.Round((price * qty) * rate, 2);
                                    else if (Constant._chainsToValue.TryGetValue(transaction.Chain.Trim().ToLower(), out decimal ratee))
                                        soldValue += Math.Round((price * qty) * ratee, 2);
                                }

                                soldCount += qty;
                            }
                            wallet.TotalSales = soldValue;
                            wallet.TotalSold = soldCount;

                            stats.SoldNftsValue += soldValue;
                            stats.SoldNftsTotal += soldCount;

                            _context.UserWallets.Update(wallet);
                            _context.UserStats.Update(stats);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (Exception _)
                {
                }
            }
        }

        private async Task UpdateUserStatsRelatedBadge(UserStat? stats, Guid userId, decimal topValuedNft)
        {
            if (stats == null)
                return;

            await ResetNftRelatedBadge(userId);

            var notifications = new List<NotificationDto>();
            var referenceType = NotificationReference.Badge;
            if (stats.TotalNft >= 1)
            {
                var notification = new NotificationDto
                {
                    InitiatorId = null,
                    ReceiverId = userId,
                    Message = Constant._NotificationMessage[referenceType],
                    Title = Constant._NotificationTitle[referenceType],
                    Reference = NotificationReference.Badge.ToString(),
                    ReferenceId = stats.TotalNft <= 4 ? BadgeStruct.NumberOfNft1 : stats.TotalNft <= 9 ? BadgeStruct.NumberOfNft5 : stats.TotalNft <= 24 ? BadgeStruct.NumberOfNft10 :
                    stats.TotalNft <= 49 ? BadgeStruct.NumberOfNft25 : stats.TotalNft <= 99 ? BadgeStruct.NumberOfNft50 : stats.TotalNft <= 249 ? BadgeStruct.NumberOfNft100 :
                    stats.TotalNft <= 499 ? BadgeStruct.NumberOfNft250 : BadgeStruct.NumberOfNft500
                };
                notifications.Add(notification);
            }

            if (stats.NftCreated >= 10)
            {
                var notification = new NotificationDto
                {
                    InitiatorId = null,
                    ReceiverId = userId,
                    Message = Constant._NotificationMessage[referenceType],
                    Title = Constant._NotificationTitle[referenceType],
                    Reference = NotificationReference.Badge.ToString(),
                    ReferenceId = stats.NftCreated <= 49 ? BadgeStruct.Creator10 : stats.NftCreated <= 99 ?
                    BadgeStruct.Creator50 : stats.NftCreated <= 199 ? BadgeStruct.Creator100 : stats.NftCreated <= 499 ? BadgeStruct.Creator200 : BadgeStruct.Creator500
                };
                notifications.Add(notification);
            }

            if (stats.BluechipCount >= 1)
            {
                var notification = new NotificationDto
                {
                    InitiatorId = null,
                    ReceiverId = userId,
                    Message = Constant._NotificationMessage[referenceType],
                    Title = Constant._NotificationTitle[referenceType],
                    Reference = NotificationReference.Badge.ToString(),
                    ReferenceId = stats.BluechipCount <= 2 ? BadgeStruct.BlueChip1 : stats.BluechipCount <= 4 ? BadgeStruct.BlueChip3 : stats.BluechipCount <= 9 ?
                    BadgeStruct.BlueChip5 : BadgeStruct.BlueChip10
                };
                notifications.Add(notification);
            }

            var worth = stats.Networth;
            if (worth >= 1000)
            {
                var notification = new NotificationDto
                {
                    InitiatorId = null,
                    ReceiverId = userId,
                    Message = Constant._NotificationMessage[referenceType],
                    Title = Constant._NotificationTitle[referenceType],
                    Reference = NotificationReference.Badge.ToString(),
                    ReferenceId = worth <= 4999 ? BadgeStruct.PortfolioValue1k : worth <= 9999 ?
                    BadgeStruct.PortfolioValue5k : worth <= 24999 ? BadgeStruct.PortfolioValue10k : worth <= 49999
                    ? BadgeStruct.PortfolioValue25k : worth <= 99999 ? BadgeStruct.PortfolioValue50k : BadgeStruct.PortfolioValue100k
                };
                notifications.Add(notification);
            }

            var topValue = topValuedNft;
            if (topValue >= 100)
            {
                var notification = new NotificationDto
                {
                    InitiatorId = null,
                    ReceiverId = userId,
                    Message = Constant._NotificationMessage[referenceType],
                    Title = Constant._NotificationTitle[referenceType],
                    Reference = NotificationReference.Badge.ToString(),
                    ReferenceId = topValue <= 499 ? BadgeStruct.TopValueNft100 : topValue <= 999 ? BadgeStruct.TopValueNft500 : topValue <= 9999 ? BadgeStruct.TopValueNft1k :
                    topValue <= 24999 ? BadgeStruct.TopValueNft10k : topValue <= 49999 ? BadgeStruct.TopValueNft25k : topValue <= 99999 ? BadgeStruct.TopValueNft50k :
                    BadgeStruct.TopValueNft100k
                };
                notifications.Add(notification);
            }

            await HandleBadgesSyncAsync(notifications);
        }

        private async Task HandleBadgesSyncAsync(List<NotificationDto> notifications)
        {
            foreach (var notification in notifications)
            {
                var prefix = notification.ReferenceId?.Split('-')?.FirstOrDefault()?.Trim();

                string pattern = $"{prefix}%";
                await _context.Database.ExecuteSqlRawAsync(@"
                    UPDATE user_badges
                    SET Active = 0
                    WHERE BadgeName LIKE {0} AND UserId = {1}", pattern, notification.ReceiverId.ToByteArray());

                var badge = await _context.UserBadges
                    .FirstOrDefaultAsync(_ => _.UserId == notification.ReceiverId.ToByteArray() && _.BadgeName == notification.ReferenceId);

                if(badge != null)
                {
                    badge.Active = 1;
                }
                else
                {
                    var badgee = new UserBadge
                    {
                        BadgeName = notification.ReferenceId,
                        EarnedAt = DateTime.UtcNow,
                        Active = 1,
                        UserId = notification.ReceiverId.ToByteArray()
                    };

                    await _context.UserBadges.AddAsync(badgee);
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task UpdateBadgeCount(byte[] userId)
        {
            var badgeCount = await _context.UserBadges.Where(_ => _.UserId == userId && _.Active == 1).CountAsync();

            var stat = await _context.UserStats.FirstOrDefaultAsync(_ => _.UserId == userId);

            if(stat != null)
            {
                stat.BadgeEarned = badgeCount;

                await _context.SaveChangesAsync();
            }
        }

        private async Task ResetNftRelatedBadge(Guid userId)
        {
            var badges = new List<string>
            {
                BadgeStruct.BlueChip1,
                BadgeStruct.Creator10,
                BadgeStruct.NumberOfNft1,
                BadgeStruct.PortfolioValue1k,
                BadgeStruct.TopValueNft1k
            };

            foreach (var item in badges)
            {
                var prefix = item.Split('-')?.FirstOrDefault()?.Trim();

                string pattern = $"{prefix}%";
                await _context.Database.ExecuteSqlRawAsync(@"
                    UPDATE user_badges
                    SET Active = 0
                    WHERE BadgeName LIKE {0} AND UserId = {1}", pattern, userId.ToByteArray());
            }
        }

        private async Task UpdateUsersPortfolioRecord()
        {
            var usersStat = await _context.UserStats.Select(_ => new
            {
                _.Networth, _.UserId
            })
            .ToListAsync();

            if(usersStat != null && usersStat.Count > 0)
            {
                foreach (var stat in usersStat)
                {
                    await _context.UserPortfolioRecords.AddAsync(new UserPortfolioRecord
                    {
                        UserId = stat.UserId,
                        Value = "0.00",
                        UsdValue = stat.Networth,
                        Address = "all",
                        Chain = "all",
                    });
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task SyncWalletGroupAsync()
        {
            var wallets = await _context.UserWallets.ToListAsync();

            if(wallets != null && wallets.Count > 0)
            {
                try
                {
                    foreach (var item in wallets)
                    {
                        var group = await _context.UserWalletGroups.FirstOrDefaultAsync(_ => _.UserId == item.UserId && _.WalletId == item.WalletId);

                        if (group == null)
                        {
                            var groupId = Guid.NewGuid().ToByteArray();
                            var walletGroup = new UserWalletGroup
                            {
                                Id = groupId,
                                WalletId = item.WalletId != null ? item.WalletId : null,
                                UserId = item.UserId
                            };

                            await _context.UserWalletGroups.AddAsync(walletGroup);

                            item.WalletGroupId = groupId;
                        }
                        else
                            item.WalletGroupId = group.Id;

                        _context.UserWallets.Update(item);

                        await _context.SaveChangesAsync();
                    }                    
                }
                catch (Exception _)
                {

                }                
            }
        }

        private async Task PullDappRadarCollectionData(int page = 1)
        {
            try
            {
                var data = await dappRadar.GetCollectionsAsync(50, page);

                if (data != null && data.Success && data.Results != null)
                {
                    try
                    {
                        await _unitOfWork.BeginTransactionAsync();

                        foreach (var item in data.Results)
                        {
                            var collection = new DappRadarCollectionDatum
                            {
                                AveragePrice = item.AvgPrice.ToString(),
                                CollectionId = item.CollectionId,
                                DappId = item.DappId,
                                FloorPrice = item.FloorPrice.ToString(),
                                Link = item.Link,
                                Logo = item.Logo,
                                MarketCap = item.MarketCap.ToString(),
                                Metadata = JsonConvert.SerializeObject(item),
                                Name = item.Name,
                                Sales = item.Sales,
                                Traders = item.Traders.ToString(),
                                Volume = item.Volume.ToString()
                            };
                            await _context.AddAsync(collection);
                        }

                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitTransactionAsync();

                        var nextPage = data.Page + 1;

                        //await Task.Delay(1000);
                        await PullDappRadarCollectionData(nextPage);
                    }
                    catch (Exception _)
                    {
                        await _unitOfWork.RollbackAsync();
                    }
                }
            }
            catch (Exception _)
            {

            }
            
        } 

        private async Task AddNftToTestAccount()
        {
            var nftIds = new List<long> 
            {
                27881,
                17,
                18,
                15,
                18156,
                18135,
                18133,
                10586,
                10587,
                10584,
                10712,
                10711,
                10708,
                10706,
                10683,
                10682,
                10666,
                18893,
                18892,
                18891,
                18890,
                18885,
                18877,
                18869,
                18868,
                18867,
                18872,
                13987,
                13986,
                13985,
                13984,
                13967,
                10394,
                4816,
                5132,
                5134, 5139, 5140, 5141, 5142, 5143, 5144, 5145, 5146, 5147, 5148, 5149,
                5150, 5151, 5152, 5153, 5154, 5155, 5156, 5157, 5158, 5159, 5160, 5161
            };

            var userid = new Guid("75506c72-0576-44fb-877e-01a4716fa522");

            var nfts = await _context.UserNftData
                .Where(n => nftIds.Contains(n.Id))
                .ToListAsync();

            if(nfts is not null)
            {
                foreach (var item in nfts)
                {
                    await _context.UserNftData.AddAsync(new UserNftDatum
                    {
                        UserId = userid.ToByteArray(),
                        UserWalletId = item.UserWalletId,
                        CollectionId = item.CollectionId,
                        Name = item.Name,
                        ImageUrl = item.ImageUrl,
                        AnimationUrl = item.AnimationUrl,
                        LastTradePrice = item.LastTradePrice,
                        LastTradeSymbol = item.LastTradeSymbol,
                        Public = 0,
                        MetaData = item.MetaData,
                        Chain = item.Chain,
                        Created = 0,
                        TokenAddress = item.TokenAddress,
                        TokenId = item.TokenId,
                        FloorPrice = item.FloorPrice,
                        ContractAddress = item.ContractAddress,
                        Description = item.Description,
                    });
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
