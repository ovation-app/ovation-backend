using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.Enums;
using Ovation.Application.DTOs.Enums.Badges;
using Ovation.Application.DTOs.Stargaze;
using Ovation.Domain.Entities;
using Ovation.Persistence.Services.BackgroundServices;
using Quartz;
using System.Net.Http.Json;

namespace Ovation.Persistence.Services.Clients
{
    class StargazeService(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseClientService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(StargazeService);

        public List<NotificationDto> Notifications { get; set; } = new();
        public List<Token> Nfts { get; set; } = new();
        public decimal PortfolioValue { get; set; } = 0M;
        public List<decimal> NftValue { get; set; } = new();
        public UserNftDatum? TopNft { get; set; } = null;
        public int NftCreated { get; set; } = 0;
        public int NftCollection { get; set; } = 0;
        public string Chain { get; } = Constant.Stargaze;
        public decimal SoldValue { get; set; } = 0.00M;

        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            if (jobData != null)
            {
                var address = jobData.GetString("Address");
                var userId = jobData.GetString("UserId");

                await GetUserNftsAsync(address, new Guid(userId));
            }
        }

        protected override async Task GetUserNftsAsync(string address, Guid userId, string? chain = default)
        {
            if (userId != Guid.Empty)
            {
                try
                {
                    var wallet = await _context.UserWallets
                        .OrderByDescending(_ => _.CreatedDate)
                        .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.Chain == Chain && _.WalletAddress == address);

                    if (wallet == null) return;

                    var wallets = await _context.UserWallets
                                .Where(_ => _.UserId == userId.ToByteArray() && _.WalletId != null && _.Chain == Chain)
                                .ToListAsync();

                    var client = _factory.CreateClient(Constant.Stargaze);

                    var response = await client
                        .GetFromJsonAsync<StargazeNftData>($"api/v1beta/profile/{address}/paginated_nfts?limit=100&offset=0");

                    if (response != null && response?.Tokens?.Count > 0)
                    {
                        var total = response.Total;
                        var nftCount = response.Total;
                        var offset = response.Tokens.Count;

                        foreach (var nft in response.Tokens)
                        {
                            Nfts.Add(nft);
                        }

                        total -= offset;

                        if(total > 0)
                        {
                            await GetNextPageNfts(address, offset, total);
                        }

                        await HandleNftsAndCollections(userId, wallet);

                        var usd = PortfolioValue * Constant._chainsToValue[Chain];
                        wallet.NftsValue = usd.ToString("F2");

                        wallet.NftCount = nftCount;
                        wallet.Migrated = 1;
                        wallet.Blockchain = Chain;

                        await UpdateUserStatAsync(nftCount, userId, address, wallet.Id);


                        await SaveNotifications(userId);

                        NftCreated = 0;
                        NftValue.Clear();
                        Notifications.Clear();
                        Nfts.Clear();
                        PortfolioValue = 0;
                    }
                    //await GetUserNftTransactionAsync(address, Chain, userId);

                }
                catch (Exception _)
                {

                }
            }
        }

        private async Task GetNextPageNfts(string address, int offset, int total)
        {
            var client = _factory.CreateClient(Constant.Stargaze);

            var response = await client
                .GetFromJsonAsync<StargazeNftData>($"api/v1beta/profile/{address}/paginated_nfts?limit=100&offset={offset}");

            if (response != null && response?.Tokens?.Count > 0)
            {
                foreach (var nft in response.Tokens)
                {
                    Nfts.Add(nft);
                }

                total -= response.Tokens.Count;
                offset += response.Tokens.Count;

                if (total > 0)
                {
                    await GetNextPageNfts(address, offset, total);
                }
            }
        }

        private async Task HandleNftsAndCollections(Guid userId, UserWallet wallet)
        {
            var collectionGrouping = Nfts.GroupBy(_ => _.Collection?.ContractAddress);
            NftCollection = collectionGrouping.Count();

            foreach(var collection in collectionGrouping)
            {
                if (collection == null) continue;

                var coll = collection?.FirstOrDefault()?.Collection;

                var collectionn = new UserNftCollectionDatum
                {
                    CollectionId = Guid.NewGuid().ToByteArray(),
                    ContractName = coll?.Name,
                    ContractAddress = collection?.Key,
                    Description = coll?.Description,
                    OwnsTotal = collection?.Count(),
                    LogoUrl = coll?.Image,
                    UserWalletId = wallet.Id,
                    Chain = Constant.Stargaze,
                    UserId = userId.ToByteArray(),
                };
                await _context.UserNftCollectionData.AddAsync(collectionn);
                await _context.SaveChangesAsync();

                var id = collectionn.Id;

                foreach(var nft in collection!)
                {
                    await AddNft(nft, id, userId, wallet);
                    if (!string.IsNullOrEmpty(nft.Price) && decimal.TryParse(nft.Price, out decimal value))
                        PortfolioValue += value / Constant.StarsConvert;
                }
            }
        }

        private async Task AddNft(Token nft, long collectionId, Guid userId, UserWallet wallet)
        {
            var userNft = new UserNftDatum
            {
                Description = nft.Description,
                ImageUrl = nft.Image,
                MetaData = JsonConvert.SerializeObject(nft),
                Name = nft.Name,
                Chain = Constant.Stargaze,
                UserId = userId.ToByteArray(),
                UserWalletId = wallet != null ? wallet.Id : null,
                TokenId = nft.TokenId,
                CollectionId = collectionId,
                LastTradePrice = (!string.IsNullOrEmpty(nft.Price) && decimal.TryParse(nft.Price, out decimal value)) ? (value / Constant.StarsConvert).ToString() : null,
                LastTradeSymbol = Chain,
                ContractAddress = nft.Collection?.ContractAddress
            };

            

            await _context.UserNftData.AddAsync(userNft);
            await _context.SaveChangesAsync();
            
            if(TopNft != null && !string.IsNullOrEmpty(userNft.LastTradePrice))
            {
                if(decimal.Parse(userNft.LastTradePrice) > decimal.Parse(TopNft.LastTradePrice!))
                {
                    TopNft = userNft;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(userNft.LastTradePrice))               
                    TopNft = userNft;
            }
        }

        private async Task UpdateUserStatAsync(int nftCount, Guid userId, string address, byte[] walletId)
        {
            //var transaction = context.Database.CurrentTransaction;

            //if (transaction == null)
            //    transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var stats = await _context.UserStats.FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());
                if (stats != null)
                {
                    var worth = PortfolioValue * Constant.StargazeValue;
                    stats.Networth += worth;
                    stats.TotalNft += nftCount;
                    stats.NftCreated += NftCreated;
                    stats.NftCollections += NftCollection;

                    worth = stats.Networth;

                    //if (Nfts != null && Nfts.Count > 0) await context.BulkInsertOrUpdateAsync<UserNftDatum>(Nfts, new BulkConfig
                    //{
                    //    SetOutputIdentity = true, // Only insert new records; ignore updates
                    //});

                    await _context.UserPortfolioRecords.AddAsync(new UserPortfolioRecord { UserId = userId.ToByteArray(), Value = PortfolioValue.ToString(), Address = address, Chain = Chain, UsdValue = worth });

                    await _context.UserNftRecords.AddAsync(new UserNftRecord { NftCount = nftCount, UserId = userId.ToByteArray(), Address = address, Chain = Chain });

                    if (worth >= 1000)
                    {
                        var notification = new NotificationDto
                        {
                            InitiatorId = null,
                            ReceiverId = userId,
                            Message = Constant._NotificationMessage[NotificationReference.Badge],
                            Title = Constant._NotificationTitle[NotificationReference.Badge],
                            Reference = NotificationReference.Badge.ToString(),
                            ReferenceId = worth <= 4999 ? BadgeStruct.PortfolioValue1k : worth <= 9999 ?
                            BadgeStruct.PortfolioValue5k : worth <= 24999 ? BadgeStruct.PortfolioValue10k : worth <= 49999
                            ? BadgeStruct.PortfolioValue25k : worth <= 99999 ? BadgeStruct.PortfolioValue50k : BadgeStruct.PortfolioValue100k
                        };
                        Notifications.Add(notification);
                    }

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
                        Notifications.Add(notification);
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
                        Notifications.Add(notification);
                    }

                    //Handle to valued nft
                    if(TopNft != null && !string.IsNullOrEmpty(TopNft.LastTradePrice))
                    {
                        var userTopNft = await _context.UserHighestNfts.FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

                        var newWorth = Math.Round(decimal.Parse(TopNft.LastTradePrice) * Constant._chainsToValue[Chain], 2);

                        if (userTopNft != null)
                        {                            

                            var oldWorth = Math.Round((decimal)(userTopNft.Worth * Constant._chainsToValue[userTopNft.TradeSymbol!])!, 2);
                            var isOld = true;
                            if (newWorth > oldWorth)
                                isOld = false;

                            userTopNft.Worth = !isOld ? decimal.Parse(TopNft.LastTradePrice) : userTopNft.Worth;
                            userTopNft.WalletId = walletId;
                            userTopNft.Chain = Chain;
                            userTopNft.UserId = userId.ToByteArray();
                            userTopNft.Name = !isOld ? TopNft.Name : userTopNft.Name;
                            userTopNft.ImageUrl = !isOld ? TopNft.ImageUrl : userTopNft.ImageUrl;
                            userTopNft.TradeSymbol = !isOld ? TopNft.LastTradeSymbol : userTopNft.TradeSymbol;
                            userTopNft.Usd = !isOld ? Math.Round(newWorth, 2) : Math.Round(oldWorth, 2);
                            userTopNft.NftId = newWorth > oldWorth ? TopNft.Id : userTopNft.NftId;
                        }
                        else
                        {
                            var userHighNft = new UserHighestNft
                            {
                                Worth = decimal.Parse(TopNft.LastTradePrice),
                                WalletId = walletId,
                                Chain = Chain,
                                UserId = userId.ToByteArray(),
                                Name = TopNft.Name,
                                ImageUrl = TopNft.ImageUrl,
                                TradeSymbol = TopNft.LastTradeSymbol,
                                Usd = Math.Round(newWorth, 2),
                                NftId = TopNft.Id
                            };

                            await _context.UserHighestNfts.AddAsync(userHighNft);
                        }

                        //update wallet values based on chain
                        var walletVal = await _context.UserWalletValues
                            .FirstOrDefaultAsync(_ => _.UserWalletId == walletId && _.UserId == userId.ToByteArray() && _.Chain == Chain);

                        if (walletVal != null)
                        {
                            walletVal.NftCount = nftCount;
                            walletVal.NativeWorth = decimal.Parse(TopNft.LastTradePrice);
                        }
                        else
                        {
                            var walletValue = new UserWalletValue
                            {
                                NativeWorth = decimal.Parse(TopNft.LastTradePrice),
                                NftCount = nftCount,
                                Chain = Chain,
                                UserId = userId.ToByteArray(),
                                UserWalletId = walletId
                            };

                            await _context.UserWalletValues.AddAsync(walletValue);
                        }

                        await _context.SaveChangesAsync();

                        var topValue = newWorth;

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
                            Notifications.Add(notification);
                        }
                    }                    

                    await _context.SaveChangesAsync();
                    //await transaction.CommitAsync();
                }
            }
            catch (Exception _)
            {
                //await transaction.RollbackAsync();
            }
        }

        private async Task SaveNotifications(Guid userId)
        {
            await SaveNotificationAsync(userId);
        }

        private async Task<long?> CheckCollectionAsync(string contractAddress, string chain)
        {
            var collection = await _context.NftCollectionsData
                .FirstOrDefaultAsync(context => context.ContractAddress == contractAddress && context.Chain == chain);

            if (collection == null)
            {
                IScheduler schedular = await _schedulerFactory.GetScheduler();

                var jobData = new JobDataMap
                {
                    {"ContractAddress", contractAddress },
                    {"Chain", chain}
                };


                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity($"get-stargaze-collection-data-{contractAddress}")
                    .ForJob(GetStargazeCollectionDataJob.Name)
                    .UsingJobData(jobData)
                    .StartNow()
                    .Build();

                await schedular.ScheduleJob(trigger);

                return null;
            }
            else
            {
                await _context.UserNftCollectionData
                    .Where(item => item.ContractAddress == contractAddress && item.Chain == chain)
                    .ExecuteUpdateAsync(updates => updates
                    .SetProperty(item => item.ParentCollection, item => collection.Id));

                return collection.Id;
            }
        }

        protected override Task GetUserNftTransactionsAsync(string address, Guid userId, string? chain = null)
        {
            throw new NotImplementedException();
        }

        internal override Task<int> GetNftCountAsync(string address, string? chain = null)
        {
            throw new NotImplementedException();
        }
    }
}
