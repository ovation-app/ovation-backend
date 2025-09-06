using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.NFTScan.Solana;
using Ovation.Domain.Entities;
using Ovation.Persistence.Services.BackgroundServices;
using Quartz;
using System.Globalization;
using System.Net.Http.Json;

namespace Ovation.Persistence.Services.Clients.NFTScan
{
    class SolanaService(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseClientService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(SolanaService);
        public List<NotificationDto> Notifications { get; set; } = new();
        public int Collections { get; set; } = 0;
        public string Chain { get; set; } = "solana";
        public List<UserNftDatum> Nfts { get; set; } = new();
        public decimal PortfolioValue { get; set; } = 0M;
        public decimal TopValuedNft { get; set; } = 0M;
        public List<decimal> NftValue { get; set; } = new();
        public int NftCreated { get; set; } = 0;
        public int NftCount { get; set; } = 0;

        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            _sentryService.AddBreadcrumb("Get Solana NFTs initiated");

            if (jobData != null)
            {
                var address = jobData.GetString("Address");
                var userId = jobData.GetString("UserId");

                _sentryService.AddBreadcrumb("Initiated the operation to fetch NFTs.", "fetch.nfts",
                    new Dictionary<string, string> { { "address", address }, { "chain", Chain } });

                await GetUserNftsAsync(address, new Guid(userId));
            }
            _span?.Finish();
        }


        protected override async Task GetUserNftsAsync(string address, Guid userId, string? chain = default)
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
                        var client = _factory.CreateClient(Constant.Solana);

                        _sentryService.AddBreadcrumb("Fetching NFTs...");

                        var collectionData = await client
                        .GetFromJsonAsync<SolanaNFTData?>($"api/sol/account/own/all/{address}?show_attribute=true");

                        if (collectionData != null && collectionData.Data != null && collectionData.Data.Count > 0)
                        {
                            var wallets = await _context.UserWallets
                                .Where(_ => _.UserId == userId.ToByteArray() && _.WalletId != null && _.Chain == Chain)
                                .ToListAsync();

                            _sentryService.AddBreadcrumb("Adding NFTs and collections to database started");

                            foreach (var data in collectionData.Data)
                            {
                                var parentCollection = await CheckCollectionAsync(data.Collection, Chain);

                                var collectionId = await AddCollection(data, wallet, userId, parentCollection);

                                if (data.Assets != null && data.Assets.Count > 0)
                                {
                                    //NftCount += data.Assets.Count;

                                    foreach (var nft in data.Assets)
                                    {
                                        await AddNft(nft, collectionId, wallet, wallets, userId);
                                        NftCount++;
                                    }
                                }

                            }

                            _sentryService.AddBreadcrumb("Adding NFTs and collections to database completed");

                            var usd = PortfolioValue * Constant._chainsToValue[Chain];
                            wallet.NftsValue = usd.ToString("F2");
                            wallet.NftCount = NftCount;
                            wallet.Migrated = 1;
                            wallet.Blockchain = Constant.Solana;

                            _sentryService.AddBreadcrumb("Updating user stats started");
                            await SaveDataAsync(userId, address, wallet.Id);

                            _sentryService.AddBreadcrumb("Updating user stats completed");


                            await SaveNotifications(userId);


                            NftCount = 0;
                            NftCreated = 0;
                            NftValue.Clear();
                            Collections = 0;
                            Notifications.Clear();
                            Nfts.Clear();
                            PortfolioValue = 0;
                        }

                        _sentryService.AddBreadcrumb("Get User NFTs transactions started");
                        await GetUserNftTransactionsAsync(address, userId, Chain);

                        _sentryService.AddBreadcrumb("Get User NFTs transactions completed");

                        _sentryService.AddBreadcrumb("Sync User custody date started");
                        await SyncCustodyDate(address, wallet.Id);
                        _sentryService.AddBreadcrumb("Sync User custody date completed");
                    }
                }
                catch (Exception _)
                {
                    SentrySdk.CaptureException(_);
                }
            }
        }

        private async Task<long?> CheckCollectionAsync(string collectionName, string chain)
        {
            if (collectionName == null) return null;

            var collection = await _context.NftCollectionsData
                .FirstOrDefaultAsync(context => context.ContractName == collectionName && context.Chain == chain);

            if (collection == null)
            {
                IScheduler schedular = await _schedulerFactory.GetScheduler();

                var jobData = new JobDataMap
                {
                    {"Collection", collectionName },
                    {"Chain", chain}
                };

                string triggerIdentity = $"get-solana-collection-data-{collectionName}";
                TriggerKey triggerKey = new TriggerKey(triggerIdentity, "solanaCollectionData");

                bool triggerExists = await schedular.CheckExists(triggerKey);

                if (!triggerExists)
                {
                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity(triggerIdentity)
                        .ForJob(GetEvmsCollectionDataJob.Name)
                        .UsingJobData(jobData)
                        .StartNow()
                        .Build();

                    await schedular.ScheduleJob(trigger);
                }

                return null;
            }
            else
            {
                await _context.UserNftCollectionData
                    .Where(item => item.ContractName == collectionName && item.Chain == chain)
                    .ExecuteUpdateAsync(updates => updates
                    .SetProperty(item => item.ParentCollection, item => collection.Id));

                return collection.Id;
                //return null;
            }
        }

        private async Task<long> AddCollection(Datum data, UserWallet wallet, Guid userId, long? parentCollection)
        {
            var priceService = _serviceScope.ServiceProvider.GetService<CollectionPriceService>();

            decimal? floorPrice = null;

            if (priceService != null) floorPrice = await priceService.GetFloorPriceAsync(Chain, default, data.Collection);

            var collection = new UserNftCollectionDatum
            {
                CollectionId = Guid.NewGuid().ToByteArray(),
                ContractName = data.Collection,
                Description = data.Description,
                ItemTotal = data.ItemsTotal,
                OwnsTotal = data.OwnsTotal,
                LogoUrl = data.LogoUrl,
                FloorPrice = floorPrice != null ? floorPrice.ToString() : null,
                Chain = Chain,
                UserWalletId = wallet.Id,
                UserId = userId.ToByteArray(),
                ParentCollection = parentCollection
            };

            await _context.UserNftCollectionData.AddAsync(collection);
            await _context.SaveChangesAsync();
            Collections += 1;

            return collection.Id;
            //Collections.Add(collection);
        }

        private async Task AddNft(Asset nft, long collectionId, UserWallet wallet, List<UserWallet> wallets, Guid userId)
        {
            var desc = "";
            if (nft.MetadataJson != null)
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
            //if (wallets != null)
            //{
            //    foreach (var item in wallets)
            //    {
            //        if (item.WalletId != null && item.WalletAddress != null)
            //            if (item.WalletAddress.Equals(nft.Minter, StringComparison.OrdinalIgnoreCase))
            //            {
            //                NftCreated += 1;
            //                isCreated = true;
            //            }
            //    }
            //}

            var usernft = new UserNftDatum
            {
                UserId = userId.ToByteArray(),
                UserWalletId = wallet.Id,
                CollectionId = collectionId,
                Name = nft.Name,
                ImageUrl = nft.ImageUri,
                LastTradePrice = nft.LatestTradePrice?.ToString("G29"),
                LastTradeSymbol = nft.LatestTradeSymbol,
                Created = (sbyte)(isCreated ? 1 : 0),
                Public = 1,
                MetaData = JsonConvert.SerializeObject(nft),
                Chain = Chain,
                TokenAddress = nft.TokenAddress,
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
                var worth = PortfolioValue * Constant.SolanaValue;

                //await context.UserNftCollectionData.AddRangeAsync(Collections);                


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

                        var topValue = 0.00M;

                        if (userTopNft != null)
                        {
                            var newWorth = Math.Round(nftWorth * Constant._chainsToValue[tradSym], 2);

                            var oldWorth = Math.Round((decimal)(userTopNft.Worth * Constant._chainsToValue[userTopNft.TradeSymbol ?? userTopNft.Chain!])!, 2);

                            userTopNft.Worth = newWorth > oldWorth ? nftWorth : userTopNft.Worth;
                            userTopNft.WalletId = walletId;
                            userTopNft.Chain = newWorth > oldWorth ? Chain : userTopNft.Chain;
                            userTopNft.UserId = userId.ToByteArray();
                            userTopNft.Name = newWorth > oldWorth ? topNft.Name : userTopNft.Name;
                            userTopNft.ImageUrl = newWorth > oldWorth ? topNft.ImageUrl : userTopNft.ImageUrl;
                            userTopNft.TradeSymbol = newWorth > oldWorth ? topNft.LastTradeSymbol : userTopNft.TradeSymbol;
                            userTopNft.Usd = newWorth > oldWorth ? Math.Round(newWorth, 2) : Math.Round(oldWorth, 2);
                            userTopNft.NftId = newWorth > oldWorth ? topNft.Id : userTopNft.NftId;

                            TopValuedNft = userTopNft.Usd.Value;
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

                            topValue = (decimal)userHighNft.Usd;
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

                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await transaction.RollbackAsync();
            }
        }

        private async Task SaveNotifications(Guid userId)
        {
            await SaveNotificationAsync(userId);
        }

        protected override async Task GetUserNftTransactionsAsync(string address, Guid userId, string? chain = default)
        {
            var wallet = await _context.UserWallets
                    .OrderByDescending(_ => _.CreatedDate)
                    .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.WalletAddress == address);

            if (wallet != null)
            {
                var client = _factory.CreateClient(chain);

                var tranxData = await client
                .GetFromJsonAsync<SolanaNftTransactionData?>($"api/sol/transactions/account/{address}?limit=100");

                if (tranxData != null && tranxData.Data != null && tranxData.Data?.Content?.Count > 0)
                {
                    await HandleTransactionData(tranxData, address, chain, wallet.Id, userId);

                    if (!string.IsNullOrEmpty(tranxData.Data.Next))
                        await GetNextPageOfTransactions(tranxData.Data.Next, address, chain, wallet.Id, userId);
                }
            }
        }

        private async Task HandleTransactionData(SolanaNftTransactionData tranxData, string address, string chain, byte[] walletId, Guid userId)
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
                .GetFromJsonAsync<SolanaNftTransactionData?>($"api/sol/transactions/account/{address}?limit=100&cursor={cursor}");

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
                soldValue += Math.Round(data.TradePrice * Constant._chainsToValue[chain.Trim().ToLower()], 2);
                return true;
            }

            return false;
        }

        private async Task AddTransaction(Content data, string chain, Guid userId, byte[] walletId)
        {
            var name = string.Empty;
            var img = string.Empty;

            var nft = await _context.UserNftData
                .Where(_ => _.Chain == chain && _.TokenAddress == data.TokenAddress)
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
                TokenId = null,
                ContractTokenId = data.TokenAddress,
                ContractAddress = null,
                ContractName = data.Collection,
                EventType = data.EventType,
                ExchangeName = data.ExchangeName,
                Fee = data?.Fee?.ToString(),
                Image = img,
                Name = name,
                TradePrice = data.TradePrice.ToString(),
                TradeSymbol = data.TradeSymbol,
                Data = JsonConvert.SerializeObject(data),
                TranxId = data.NftscanTxId,
                UserId = userId.ToByteArray(),
                Chain = chain,
                UserWalletId = walletId,
                TranxDate = DateTimeOffset.FromUnixTimeMilliseconds(data.Timestamp).UtcDateTime,
                From = data.Source,
                To = data.Destination
            };

            await _context.UserNftTransactions.AddAsync(tranx);
            await _context.SaveChangesAsync();
        }

        private async Task<SingleNftData?> GetSingleNftDataAsync(string chain, string tokenAddress)
        {
            var client = _factory.CreateClient(chain);

            return await client
            .GetFromJsonAsync<SingleNftData?>($"api/sol/assets/{tokenAddress}?show_attribute=false");
        }

        internal async Task<List<NftTransactionDto>?> GetNftTransactionAsync(string? tokenAddress, string chain)
        {
            var client = _factory.CreateClient(chain);

            var tranxData = await client
            .GetFromJsonAsync<SolanaNftTransactionData?>($"api/sol/transactions/{tokenAddress}?limit=100");

            if (tranxData != null && tranxData.Data != null && tranxData.Data?.Content?.Count > 0)
            {
                var transactionData = new List<NftTransactionDto>();

                foreach (var data in tranxData.Data.Content)
                {
                    transactionData.Add(new NftTransactionDto
                    {
                        ContractName = data.Collection,
                        EventType = data.EventType,
                        Price = data.TradePrice.ToString(),
                        TranxDate = DateTimeOffset.FromUnixTimeMilliseconds(data.Timestamp).UtcDateTime,
                        TradeSymbol = data.TradeSymbol,
                        From = data.Source,
                        To = data.Destination
                    });
                }

                return transactionData;
            }

            return null;
        }

        internal override async Task<int> GetNftCountAsync(string address, string? chain = default)
        {
            var client = _factory.CreateClient(Chain);
            var collectionData = await client
                .GetFromJsonAsync<SolanaNFTData?>($"api/sol/account/own/all/{address}?show_attribute=true");
            if (collectionData != null && collectionData.Data != null && collectionData.Data.Count > 0)
            {
                return collectionData.Data.Sum(data => data.Assets?.Count ?? 0);
            }
            _sentryService.AddBreadcrumb("No NFTs found for the user.", "fetch.nfts",
                new Dictionary<string, string> { { "address", address }, { "chain", Chain } });
            return 0;
        }
    }
}
