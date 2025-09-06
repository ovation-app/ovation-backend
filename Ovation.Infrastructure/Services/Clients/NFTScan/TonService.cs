using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.Enums;
using Ovation.Application.DTOs.Enums.Badges;
using Ovation.Application.DTOs.NFTScan.TON;
using Ovation.Domain.Entities;
using Ovation.Persistence.Services.BackgroundServices;
using Quartz;
using System.Globalization;
using System.Net.Http.Json;

namespace Ovation.Persistence.Services.Clients.NFTScan
{
    class TonService(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseClientService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(TonService);
        public List<NotificationDto> Notifications { get; set; } = new();
        public int Collections { get; set; } = 0;
        public string Chain { get; set; } = "ton";
        public List<UserNftDatum> Nfts { get; set; } = new();
        public decimal PortfolioValue { get; set; } = 0M;
        public List<decimal> NftValue { get; set; } = new();
        public int NftCreated { get; set; } = 0;
        public int NftCount { get; set; } = 0;


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


        public async Task GetUserNftsAsync(string address, Guid userId)
        {
            if (userId != Guid.Empty)
            {
                try
                {
                    var wallet = await _context.UserWallets
                        .OrderByDescending(_ => _.CreatedDate)
                        .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.WalletAddress == address);

                    if (wallet != null && wallet.Chain == Chain)
                    {
                        var client = _factory.CreateClient(Constant.Ton);

                        var collectionData = await client
                        .GetFromJsonAsync<TonNFTData?>($"api/ton/account/own/all/{address}?show_attribute=true");

                        if (collectionData != null && collectionData.Data != null && collectionData.Data.Count > 0)
                        {
                            var wallets = await _context.UserWallets
                                .Where(_ => _.UserId == userId.ToByteArray() && _.WalletId != null && _.Chain == Chain)
                                .ToListAsync();

                            foreach (var data in collectionData.Data)
                            {
                                var parentCollection = await CheckCollectionAsync(data.ContractAddress, Chain);

                                var collectionId = await AddCollectionAsync(data, wallet, userId, parentCollection);

                                if (data.Assets != null && data.Assets.Count > 0)
                                {
                                    foreach (var nft in data.Assets)
                                    {
                                        await AddNft(nft, collectionId, wallet, wallets, userId);
                                        NftCount++;
                                    }
                                }

                            }

                            //NftCount = (NftCount == NftValue.Count) ? NftCount : NftValue.Count;
                            var usd = PortfolioValue * Constant._chainsToValue[Chain];
                            wallet.NftsValue = usd.ToString("F2");
                            wallet.NftCount = NftCount;
                            wallet.Migrated = 1;
                            wallet.Blockchain = Constant.Ton;

                            await SaveDataAsync(userId, address, wallet.Id);


                            await SaveNotifications(userId);

                            NftCount = 0;
                            NftCreated = 0;
                            NftValue.Clear();
                            Collections = 0;
                            Notifications.Clear();
                            Nfts.Clear();
                            PortfolioValue = 0;
                        }
                        await GetUserNftTransactionAsync(address, Chain, userId);
                        await SyncCustodyDate(address, wallet.Id);
                    }
                }
                catch (Exception _)
                {

                }
            }
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
                    .WithIdentity($"get-ton-collection-data-{contractAddress}")
                    .ForJob(GetTonCollectionDataJob.Name)
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

        private async Task<long> AddCollectionAsync(Datum data, UserWallet wallet, Guid userId, long? parentCollection)
        {
            var collection = new UserNftCollectionDatum
            {
                CollectionId = Guid.NewGuid().ToByteArray(),
                ContractName = data.ContractName,
                ContractAddress = data.ContractAddress,
                Description = data.Description,
                ItemTotal = data.ItemsTotal,
                OwnsTotal = data.OwnsTotal,
                LogoUrl = data.LogoUrl,
                UserWalletId = wallet.Id,
                Chain = Chain,
                UserId = userId.ToByteArray(),
                ParentCollection = parentCollection
            };

            await _context.UserNftCollectionData.AddAsync(collection);
            await _context.SaveChangesAsync();
            Collections += 1;

            return collection.Id;
        }

        private async Task AddNft(Asset nft, long collectionId, UserWallet wallet, List<UserWallet> wallets, Guid userId)
        {
            var desc = nft.Description;
            if (string.IsNullOrEmpty(desc) && nft.MetadataJson != null)
            {
                try
                {
                    var anonyObj = JsonConvert.DeserializeObject<dynamic>(nft.MetadataJson);

                    if (anonyObj is JObject jsonObj)
                    {
                        if (jsonObj["description"] != null)
                            desc = jsonObj["description"].ToString();
                    }
                }
                catch (Exception _)
                {

                }
            }

            var isCreated = false;
            if (wallets != null)
            {
                foreach (var item in wallets)
                {
                    if (item.WalletId != null && item.WalletAddress != null)
                        if (item.WalletAddress.Equals(nft.Minter, StringComparison.OrdinalIgnoreCase))
                        {
                            NftCreated += 1;
                            isCreated = true;
                        }
                }
            }

            var usernft = new UserNftDatum
            {
                UserId = userId.ToByteArray(),
                //Id = SequentialGuidGenerator.Instance.NewGuid().ToByteArray(),
                UserWalletId = wallet.Id,
                CollectionId = collectionId,
                Name = nft.Name,
                ImageUrl = nft.ImageUri,
                LastTradePrice = nft.LatestTradePrice?.ToString("G29"),
                LastTradeSymbol = Chain.ToUpper(),
                Public = 1,
                Created = (sbyte)(isCreated ? 1 : 0),
                MetaData = JsonConvert.SerializeObject(nft),
                Chain = Chain,
                TokenAddress = nft.TokenAddress,
                TokenId = nft.TokenId,
                ContractAddress = nft.ContractAddress,
                Description = desc,
            };

            if (nft.LatestTradePrice != null)
            {
                PortfolioValue += nft.LatestTradePrice.Value;
                NftValue.Add(nft.LatestTradePrice.Value);
            }
            //else if (nft.MintPrice != null)
            //{
            //    PortfolioValue += nft.MintPrice.Value;
            //    NftValue.Add(nft.MintPrice.Value);
            //}

            
            await _context.UserNftData.AddAsync(usernft);
            await _context.SaveChangesAsync();

            Nfts.Add(usernft);
        }

        private async Task SaveDataAsync(Guid userId, string address, byte[] walletId)
        {
            var transaction = _context.Database.CurrentTransaction;

            if (transaction == null)
                transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var worth = PortfolioValue * Constant.TonValue;

                //await context.UserNftCollectionData.AddRangeAsync(Collections);
                //await context.UserPortfolioRecords.AddAsync(new UserPortfolioRecord { UserId = userId.ToByteArray(), Value = PortfolioValue.ToString("F8"), Address = address, Chain = Chain });

                //if (Nfts != null && Nfts.Count > 0) await context.BulkInsertOrUpdateAsync<UserNftDatum>(Nfts, new BulkConfig
                //{
                //    SetOutputIdentity = true, // Only insert new records; ignore updates
                //});
                await _context.UserNftRecords.AddAsync(new UserNftRecord { NftCount = NftCount, UserId = userId.ToByteArray(), Address = address, Chain = Chain });

                var stats = await _context.UserStats.FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());
                if (stats != null)
                {
                    stats.Networth += worth;
                    stats.TotalNft += NftCount;
                    stats.NftCollections += Collections;
                    stats.NftCreated += NftCreated;

                    worth = stats.Networth;

                    await _context.UserPortfolioRecords.AddAsync(new UserPortfolioRecord { UserId = userId.ToByteArray(), Value = PortfolioValue.ToString("F8"), Address = address, Chain = Chain, UsdValue = worth });

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

                    var topNft = Nfts.OrderByDescending(_ =>
                    {
                        if (double.TryParse(_.LastTradePrice, NumberStyles.Float, CultureInfo.InvariantCulture, out double numericValue))
                            return numericValue;
                        return double.MinValue; // Treat invalid numbers as very large for sorting
                    }).FirstOrDefault();

                    if (topNft != null && !string.IsNullOrEmpty(topNft.LastTradePrice))
                    {
                        decimal.TryParse(topNft.LastTradePrice, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal nftWorth);

                        var userTopNft = await _context.UserHighestNfts.FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

                        var tradSym = !string.IsNullOrEmpty(topNft.LastTradeSymbol) ? topNft.LastTradeSymbol.Trim() : Chain;

                        if (userTopNft != null)
                        {
                            var newWorth = Math.Round(nftWorth * Constant._chainsToValue[tradSym], 2);

                            var oldWorth = Math.Round((decimal)(userTopNft.Worth * Constant._chainsToValue[userTopNft.Chain!])!, 2);

                            userTopNft.Worth = newWorth > oldWorth ? nftWorth : userTopNft.Worth;
                            userTopNft.WalletId = walletId;
                            userTopNft.Chain = Chain;
                            userTopNft.UserId = userId.ToByteArray();
                            userTopNft.Name = newWorth > oldWorth ? topNft.Name : userTopNft.Name;
                            userTopNft.ImageUrl = newWorth > oldWorth ? topNft.ImageUrl : userTopNft.ImageUrl;
                            userTopNft.TradeSymbol = newWorth > oldWorth ? topNft.LastTradeSymbol : userTopNft.TradeSymbol;
                            userTopNft.Usd = newWorth > oldWorth ? Math.Round(newWorth, 2) : Math.Round(oldWorth, 2);
                            userTopNft.NftId = newWorth > oldWorth ? topNft.Id : userTopNft.NftId;
                        }
                        else
                        {
                            var userHighNft = new UserHighestNft
                            {
                                Worth = nftWorth,
                                WalletId = walletId,
                                Chain = Chain,
                                UserId = userId.ToByteArray(),
                                Name = topNft.Name,
                                ImageUrl = topNft.ImageUrl,
                                TradeSymbol = topNft.LastTradeSymbol,
                                Usd = Math.Round(nftWorth * Constant._chainsToValue[tradSym], 2),
                                NftId = topNft.Id
                            };

                            await _context.UserHighestNfts.AddAsync(userHighNft);
                        }

                        //update wallet values based on chain
                        var walletVal = await _context.UserWalletValues
                            .FirstOrDefaultAsync(_ => _.UserWalletId == walletId && _.UserId == userId.ToByteArray() && _.Chain == Chain);

                        if (walletVal != null)
                        {
                            walletVal.NftCount = NftCount;
                            walletVal.NativeWorth = nftWorth;
                        }
                        else
                        {
                            var walletValue = new UserWalletValue
                            {
                                NativeWorth = nftWorth,
                                NftCount = NftCount,
                                Chain = Chain,
                                UserId = userId.ToByteArray(),
                                UserWalletId = walletId
                            };

                            await _context.UserWalletValues.AddAsync(walletValue);
                        }

                        await _context.SaveChangesAsync();

                        var topValue = nftWorth * Constant._chainsToValue[Chain];

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
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception _)
            {
                await transaction.RollbackAsync();
            }
        }

        private async Task SaveNotifications(Guid userId)
        {
            await SaveNotificationAsync(userId);
        }

        internal async Task GetUserNftTransactionAsync(string address, string chain, Guid userId)
        {
            var wallet = await _context.UserWallets
                    .OrderByDescending(_ => _.CreatedDate)
                    .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.WalletAddress == address);

            if (wallet != null)
            {
                var client = _factory.CreateClient(chain);

                var tranxData = await client
                .GetFromJsonAsync<TonNftTransactionData?>($"api/ton/transactions/account/{address}?limit=100");

                if (tranxData != null && tranxData.Data != null && tranxData.Data?.Content?.Count > 0)
                {
                    await HandleTransactionData(tranxData, address, chain, wallet.Id, userId);

                    if (!string.IsNullOrEmpty(tranxData.Data.Next))
                        await GetNextPageOfTransactions(tranxData.Data.Next, address, chain, wallet.Id, userId);
                }
            }
        }

        private async Task HandleTransactionData(TonNftTransactionData tranxData, string address, string chain, byte[] walletId, Guid userId)
        {
            var soldCount = 0;
            var soldValue = 0.00M;
            foreach (var data in tranxData.Data!.Content!)
            {
                if (IsUserTheSeller(data, address, chain, ref soldValue))
                    soldCount += 1;

                await AddTransaction(data, chain, userId, walletId);
            }

            var stats = await _context.UserStats
                .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

            if (stats != null)
            {
                stats.SoldNftsTotal += soldCount;
                stats.SoldNftsValue += soldValue;

                var wallet = await _context.UserWallets
                .FirstOrDefaultAsync(_ => _.Id == walletId);

                if (wallet != null)
                {
                    wallet.TotalSales = soldValue;
                    wallet.TotalSold = soldCount;
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task GetNextPageOfTransactions(string cursor, string address, string chain, byte[] walletId, Guid userId)
        {
            var client = _factory.CreateClient(chain);

            var tranxData = await client
                .GetFromJsonAsync<TonNftTransactionData?>($"api/ton/transactions/account/{address}?limit=100&cursor={cursor}");

            if (tranxData != null && tranxData.Data != null && tranxData.Data?.Content?.Count > 0)
            {
                await HandleTransactionData(tranxData, address, chain, walletId, userId);

                if (!string.IsNullOrEmpty(tranxData.Data.Next))
                    await GetNextPageOfTransactions(tranxData.Data.Next, address, chain, walletId, userId);
            }
        }

        private bool IsUserTheSeller(Content data, string address, string chain, ref decimal soldValue)
        {
            if (data != null
                && string.Equals(data.EventType, "sale", StringComparison.OrdinalIgnoreCase)
                && string.Equals(data.Source, address, StringComparison.OrdinalIgnoreCase))
            {
                if(decimal.TryParse(data.TradePrice, out decimal price))
                {
                    soldValue += Math.Round(price * Constant._chainsToValue[chain.Trim().ToLower()], 2);
                    return true;
                }                
            }

            return false;
        }

        private async Task AddTransaction(Content data, string chain, Guid userId, byte[] walletId)
        {
            var name = string.Empty;
            var img = string.Empty;

            var nft = await _context.UserNftData
                .Where(_ => _.Chain == chain && _.ContractAddress == data.ContractAddress && (_.TokenAddress == data.TokenAddress || _.TokenId == data.TokenId))
                .Select(x => new
                {
                    x.Name,
                    x.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (nft == null || string.IsNullOrEmpty(nft.Name) || string.IsNullOrEmpty(nft.ImageUrl))
            {
                var singleNft = await GetSingleNftDataAsync(chain, data.TokenAddress!);

                if (singleNft != null && singleNft.Data != null)
                {
                    name = singleNft.Data.Name;
                    img = singleNft.Data.ImageUri;
                }
            }
            else
            {
                name = nft.Name;
                img = nft.ImageUrl;
            }

            var tranx = new UserNftTransaction
            {
                TokenId = data.TokenId,
                ContractTokenId = data.TokenAddress,
                ContractAddress = data.ContractAddress,
                ContractName = data.ContractName,
                EventType = data.EventType,
                ExchangeName = data.ExchangeName,
                Fee = data?.Fee?.ToString(),
                Image = img,
                Name = name,
                TradePrice = data.TradePrice,
                TradeSymbol = "ton",
                Data = JsonConvert.SerializeObject(data),
                TranxId = data.NftscanTxId,
                UserId = userId.ToByteArray(),
                Chain = chain,
                UserWalletId = walletId,
                TranxDate = DateTimeOffset.FromUnixTimeMilliseconds(data.Timestamp).UtcDateTime,
                From = data?.Source,
                To = data?.Destination
            };

            await _context.UserNftTransactions.AddAsync(tranx);
            await _context.SaveChangesAsync();
        }

        private async Task<SingleNftData?> GetSingleNftDataAsync(string chain, string tokenAddress)
        {
            var client = _factory.CreateClient(chain);

            return await client
            .GetFromJsonAsync<SingleNftData?>($"api/ton/assets/{tokenAddress}?show_attribute=false");
        }

        internal async Task<List<NftTransactionDto>?> GetNftTransactionAsync(string? tokenAddress, string? chain)
        {
            var client = _factory.CreateClient(chain);

            var tranxData = await client
            .GetFromJsonAsync<TonNftTransactionData?>($"api/ton/transactions/{tokenAddress}?limit=100");

            if (tranxData != null && tranxData.Data != null && tranxData.Data?.Content?.Count > 0)
            {
                var transactionData = new List<NftTransactionDto>();

                foreach (var data in tranxData.Data.Content)
                {
                    transactionData.Add(new NftTransactionDto
                    {
                        ContractName = data.ContractName,
                        EventType = data.EventType,
                        Price = data.TradePrice.ToString(),
                        TranxDate = DateTimeOffset.FromUnixTimeMilliseconds(data.Timestamp).UtcDateTime,
                        TradeSymbol = "ton",
                        From = data.Source,
                        To = data.Destination
                    });
                }

                return transactionData;
            }

            return null;
        }

        protected override Task GetUserNftsAsync(string address, Guid userId, string? chain = null)
        {
            throw new NotImplementedException();
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
