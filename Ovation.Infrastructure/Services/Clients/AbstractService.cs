using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ovation.Application.Constants;
using Ovation.Application.DTOs.Abstract;
using Ovation.Domain.Entities;
using Ovation.Persistence.Services.BackgroundServices;
using Quartz;
using System.Net.Http.Json;
using System.Text;

namespace Ovation.Persistence.Services.Clients
{
    class AbstractService(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory) 
        : BaseClientService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(AbstractService);

        public List<OwnedNft> Nfts { get; set; } = new();
        public decimal PortfolioValue { get; set; } = 0M;
        public UserNftDatum? TopNft { get; set; } = null;
        public int NftCreated { get; set; } = 0;
        public int NftCollection { get; set; } = 0;
        public string Chain { get; } = Constant.Abstract;
        public decimal SoldValue { get; set; } = 0.00M;

        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            _sentryService.AddBreadcrumb("Get Abstract NFTs initiated");

            if (jobData != null)
            {
                var address = jobData.GetString("Address");
                var userId = jobData.GetString("UserId");

                _sentryService.AddBreadcrumb("Initiated the operation to fetch NFTs.", "fetch.nfts",
                    new Dictionary<string, string> { { "address", address}, { "chain", Chain } });
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
                        .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.Chain == Chain && _.WalletAddress == address);

                    if (wallet == null) return;

                    var wallets = await _context.UserWallets
                                .Where(_ => _.UserId == userId.ToByteArray() && _.WalletId != null && _.Chain == Chain)
                                .ToListAsync();

                    _sentryService.AddBreadcrumb("Fetching NFTs...");

                    var client = _factory.CreateClient(Constant.Abstract);

                    if (client.BaseAddress == null) { return; }

                    var response = await client
                        .GetFromJsonAsync<AlchemyNftData>($"nft/v3/{Constant.AlchemyKey}/getNFTsForOwner?owner={address}&withMetadata=true&pageSize=100");

                    if (response != null && response?.OwnedNfts?.Count > 0)
                    {                      

                        foreach (var nft in response.OwnedNfts)
                        {
                            Nfts.Add(nft);
                        }

                        if (response.PageKey != null)
                        {
                            await GetNextPageNfts(address, response.PageKey);
                        }

                        _sentryService.AddBreadcrumb("Adding NFTs and collections to database started");
                        await HandleNftsAndCollections(userId, wallet);

                        _sentryService.AddBreadcrumb("Adding NFTs and collections to database completed");

                        var usd = PortfolioValue * Constant._chainsToValue[Chain];
                        wallet.NftsValue = usd.ToString("F2");

                        wallet.NftCount = response.TotalCount;
                        wallet.Migrated = 1;
                        wallet.Blockchain = Constant.Evm;
                        //wallet.NftsValue = (PortfolioValue * Constant._chainsToValueFloor[Constant.Abstract]).ToString();

                        _context.UserWallets.Update(wallet);

                        _sentryService.AddBreadcrumb("Updating user stats started");
                        await UpdateUserStatAsync(response.TotalCount, userId, address, wallet.Id);

                        _sentryService.AddBreadcrumb("Updating user stats completed");

                        _sentryService.AddBreadcrumb("Sync User badges started");
                        await SaveNotificationAsync(userId);

                        _sentryService.AddBreadcrumb("Sync User badges completed");

                        NftCreated = 0;
                        Nfts.Clear();
                        PortfolioValue = 0;
                    }
                    _sentryService.AddBreadcrumb("Get User NFTs transactions started");
                    await GetUserNftTransactionsAsync(address, userId);

                    _sentryService.AddBreadcrumb("Get User NFTs transactions completed");

                    _sentryService.AddBreadcrumb("Sync User custody date started");
                    await SyncCustodyDate(address, wallet.Id);

                    _sentryService.AddBreadcrumb("Sync User custody date completed");
                }
                catch (Exception _)
                {
                    SentrySdk.CaptureException(_);
                }
            }
        }

        private async Task GetNextPageNfts(string address, string cursor)
        {
            var client = _factory.CreateClient(Constant.Abstract);

            var response = await client
                .GetFromJsonAsync<AlchemyNftData>($"nft/v3/{Constant.AlchemyKey}/getNFTsForOwner?owner={address}&withMetadata=true&pageSize=100&pageKey={cursor}");

            if (response != null && response?.OwnedNfts?.Count > 0)
            {
                foreach (var nft in response.OwnedNfts)
                {
                    Nfts.Add(nft);
                }

                if (response.PageKey != null)
                {
                    await GetNextPageNfts(address, response.PageKey);
                }
            }
        }

        private async Task<int> GetNextPageNftsCount(string address, string cursor)
        {
            var client = _factory.CreateClient(Constant.Abstract);

            var response = await client
                .GetFromJsonAsync<AlchemyNftData>($"nft/v3/{Constant.AlchemyKey}/getNFTsForOwner?owner={address}&withMetadata=true&pageSize=100&pageKey={cursor}");

            if (response != null && response?.OwnedNfts?.Count > 0)
            {
                var nftCount = response.OwnedNfts.Count;

                if (response.PageKey != null)
                {
                    nftCount += await GetNextPageNftsCount(address, response.PageKey);
                }

                return nftCount;
            }

            return 0;
        }

        private async Task HandleNftsAndCollections(Guid userId, UserWallet wallet)
        {
            var collectionGrouping = Nfts.GroupBy(_ => _.Contract?.Address);
            NftCollection = collectionGrouping.Count();

            var priceService = _serviceScope.ServiceProvider.GetService<CollectionPriceService>();

            foreach (var collection in collectionGrouping)
            {
                if (collection == null) continue;

                var coll = collection?.FirstOrDefault()?.Contract;
                decimal? floorPrice = null;

                if(priceService != null) floorPrice = await priceService.GetFloorPriceAsync(Chain, coll?.Name, collection?.Key);
                
                var collectionn = new UserNftCollectionDatum
                {
                    CollectionId = Guid.NewGuid().ToByteArray(),
                    ContractName = coll?.Name,
                    ContractAddress = collection?.Key,
                    Description = coll?.OpenSeaMetadata?.Description,
                    OwnsTotal = collection?.Count(),
                    LogoUrl = coll?.OpenSeaMetadata?.ImageUrl ?? coll?.OpenSeaMetadata?.BannerImageUrl,
                    UserWalletId = wallet.Id,
                    FloorPrice = floorPrice.ToString(),                    
                    Chain = Chain,
                    UserId = userId.ToByteArray(),
                };
                await _context.UserNftCollectionData.AddAsync(collectionn);
                await _context.SaveChangesAsync();

                var id = collectionn.Id;

                foreach (var nft in collection!)
                {
                    await AddNft(nft, id, collectionn.FloorPrice, userId, wallet);
                    if (floorPrice != null)
                        PortfolioValue += (decimal)(floorPrice / Constant.EthValue);
                }
            }
        }

        private async Task AddNft(OwnedNft nft, long collectionId, string? floorPrice, Guid userId, UserWallet wallet)
        {
            var userNft = new UserNftDatum
            {
                Description = nft.Description,
                ImageUrl = !string.IsNullOrEmpty(nft?.Image?.OriginalUrl) ? nft?.Image?.OriginalUrl : nft?.Image?.CachedUrl,
                MetaData = JsonConvert.SerializeObject(nft),
                Name = nft.Name,
                Chain = Chain,
                UserId = userId.ToByteArray(),
                UserWalletId = wallet != null ? wallet.Id : null,
                TokenId = nft.TokenId,
                CollectionId = collectionId,
                FloorPrice = floorPrice,
                //LastTradePrice = (!string.IsNullOrEmpty(nft.Price) && decimal.TryParse(nft.Price, out decimal value)) ? (value / Constant.StarsConvert).ToString() : null,
                //LastTradeSymbol = Chain,
                ContractAddress = nft.Contract?.Address
            };



            await _context.UserNftData.AddAsync(userNft);
            await _context.SaveChangesAsync();

            if (TopNft != null && !string.IsNullOrEmpty(userNft.FloorPrice))
            {
                if (decimal.Parse(userNft.FloorPrice) > decimal.Parse(TopNft.FloorPrice!))
                {
                    TopNft = userNft;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(userNft.FloorPrice))
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
                    var worth = PortfolioValue * Constant.EthValue;
                    stats.Networth += worth;
                    stats.TotalNft += nftCount;
                    stats.NftCreated += NftCreated;
                    stats.NftCollections += NftCollection;

                    worth = stats.Networth;

                    await _context.UserPortfolioRecords.AddAsync(new UserPortfolioRecord { UserId = userId.ToByteArray(), Value = PortfolioValue.ToString(), Address = address, Chain = Chain, UsdValue = worth });

                    await _context.UserNftRecords.AddAsync(new UserNftRecord { NftCount = nftCount, UserId = userId.ToByteArray(), Address = address, Chain = Chain });

             

                    //Handle to valued nft
                    if (TopNft != null && !string.IsNullOrEmpty(TopNft.FloorPrice))
                    {
                        var userTopNft = await _context.UserHighestNfts.FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

                        var newWorth = Math.Round(decimal.Parse(TopNft.FloorPrice) * Constant._chainsToValue[Chain], 2);

                        if (userTopNft != null)
                        {

                            var oldWorth = Math.Round((decimal)(userTopNft.Worth * Constant._chainsToValue[userTopNft.TradeSymbol ?? userTopNft.Chain!])!, 2);
                            var isOld = true;
                            if (newWorth > oldWorth)
                                isOld = false;

                            userTopNft.Worth = !isOld ? decimal.Parse(TopNft.FloorPrice) : userTopNft.Worth;
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
                                Worth = decimal.Parse(TopNft.FloorPrice),
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
                            walletVal.NativeWorth = decimal.Parse(TopNft.FloorPrice);
                        }
                        else
                        {
                            var walletValue = new UserWalletValue
                            {
                                NativeWorth = decimal.Parse(TopNft.FloorPrice),
                                NftCount = nftCount,
                                Chain = Chain,
                                UserId = userId.ToByteArray(),
                                UserWalletId = walletId
                            };

                            await _context.UserWalletValues.AddAsync(walletValue);
                        }

                        await _context.SaveChangesAsync();

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

                string triggerIdentity = $"get-evm-collection-data-{contractAddress}";
                TriggerKey triggerKey = new TriggerKey(triggerIdentity, "abstractCollectionData");

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
                    .Where(item => item.ContractAddress == contractAddress && item.Chain == chain)
                    .ExecuteUpdateAsync(updates => updates
                    .SetProperty(item => item.ParentCollection, item => collection.Id));

                return collection.Id;
            }

        }


        protected override async Task GetUserNftTransactionsAsync(string address, Guid userId, string? chain = default)
        {
            var wallet = await _context.UserWallets
                    .OrderByDescending(_ => _.CreatedDate)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.WalletAddress == address);

            if (wallet != null)
            {
                var getFromTranx = await GetFromTransactionData(address); 

                if (getFromTranx != null && getFromTranx?.Result != null && getFromTranx?.Result?.Transfers?.Count > 0)
                {
                    var tranx = getFromTranx?.Result?.Transfers!;
                    //var soldCount = 0;
                    foreach (var data in tranx)
                    {
                        //if (await IsUserTheSeller(data, address, Chain))
                        //    soldCount += 1;

                        if (data.RawContract == null || string.IsNullOrEmpty(data.RawContract.Address))
                            continue;

                        await AddTransaction(data, userId, wallet.Id);
                    }

                    if (!string.IsNullOrEmpty(getFromTranx?.Result.PageKey))
                    {
                        await GetFromTransactionNextPageData(address, getFromTranx.Result.PageKey!, userId, wallet.Id);
                    }
                }

                var getToTranx = await GetToTransactionData(address);

                if (getToTranx != null && getToTranx?.Result != null && getToTranx?.Result?.Transfers?.Count > 0)
                {
                    var tranx = getToTranx?.Result?.Transfers!;
                    foreach (var data in tranx)
                    {                        
                        if (data.RawContract == null || string.IsNullOrEmpty(data.RawContract.Address))
                            continue;

                        await AddTransaction(data, userId, wallet.Id);
                    }

                    if (!string.IsNullOrEmpty(getToTranx?.Result.PageKey))
                    {
                        await GetToTransactionNextPageData(address, getToTranx.Result.PageKey!, userId, wallet.Id);
                    }
                }


                //var stats = await context.UserStats
                //    .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

                //if (stats != null)
                //{
                //    //stats.SoldNftsTotal += soldCount;
                //    //stats.SoldNftsValue += SoldValue;

                //    await context.SaveChangesAsync();
                //}
            }
        }

        //private async Task<bool> IsUserTheSeller(TezosNftTransactionData data, string address, string chain)
        //{
        //    if (data != null && data.From != null
        //        && data.From.Address.Equals(address, StringComparison.OrdinalIgnoreCase))
        //    {
        //        var contractAddress = data?.Token?.Contract?.Address;
        //        var tokenId = data?.Token?.TokenId;
        //        if (!string.IsNullOrWhiteSpace(contractAddress) && !string.IsNullOrWhiteSpace(tokenId))
        //            SoldValue += await GetNftPriceAsync(contractAddress, tokenId);
        //        return true;
        //    }

        //    return false;
        //}

        private async Task AddTransaction(Transfer data, Guid userId, byte[] walletId)
        {
            var nft = await GetSignleNft(data?.RawContract?.Address, data?.TokenId);

            var tranx = new UserNftTransaction
            {
                TokenId = !string.IsNullOrEmpty(nft?.TokenId) ? nft?.TokenId : HexToBigInteger(data?.TokenId),
                ContractTokenId = data?.TokenId,
                ContractAddress = data?.RawContract?.Address,
                ContractName = null,
                EventType = "transfer",
                ExchangeName = null,
                Fee = null,
                Image = !string.IsNullOrEmpty(nft?.Image?.OriginalUrl) ? nft?.Image?.OriginalUrl : nft?.Image?.CachedUrl,
                Name = nft?.Name,
                Qty = 1,
                TradePrice = null,
                TradeSymbol = Chain,
                Data = JsonConvert.SerializeObject(data),
                TranxId = data?.UniqueId,
                UserId = userId.ToByteArray(),
                UserWalletId = walletId,
                Chain = Chain,
                TranxDate = data?.Metadata?.BlockTimestamp,
                From = data?.From,
                To = data?.To
            };

            await _context.UserNftTransactions.AddAsync(tranx);
            await _context.SaveChangesAsync();
        }

        //internal async Task<List<NftTransactionDto>?> GetNftTransactionAsync(string? contractAddress, string? tokenId)
        //{
            
        //}

        private async Task<AbstractTransactionData?> GetFromTransactionData(string address)
        {
            var body = $@"
                {{
                  ""id"": 1,
                  ""jsonrpc"": ""2.0"",
                  ""method"": ""alchemy_getAssetTransfers"",
                  ""params"": [
                    {{
                      ""fromBlock"": ""0x0"",
                      ""toBlock"": ""latest"",
                      ""fromAddress"": ""{address}"",
                      ""category"": [
                        ""erc721"",
                        ""erc1155""
                      ],
                      ""order"": ""asc"",
                      ""withMetadata"": true,
                      ""excludeZeroValue"": true,
                      ""maxCount"": ""0x3e8""
                    }}
                  ]
                }}
            ";

            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var client = _factory.CreateClient(Chain);

            var response = await client
                 .PostAsync($"v2/{Constant.AlchemyKey}", content);

            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadFromJsonAsync<AbstractTransactionData>();

            return responseData;
        }

        private async Task GetFromTransactionNextPageData(string address, string pageKey, Guid userId, byte[] walletId)
        {
            var body = $@"
                {{
                  ""id"": 1,
                  ""jsonrpc"": ""2.0"",
                  ""method"": ""alchemy_getAssetTransfers"",
                  ""params"": [
                    {{
                      ""fromBlock"": ""0x0"",
                      ""toBlock"": ""latest"",
                      ""fromAddress"": ""{address}"",
                      ""category"": [
                        ""erc721"",
                        ""erc1155""
                      ],
                      ""order"": ""asc"",
                      ""withMetadata"": true,
                      ""excludeZeroValue"": true,
                      ""maxCount"": ""0x3e8"",
                      ""pageKey"": ""{pageKey}""
                    }}
                  ]
                }}
            ";

            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var client = _factory.CreateClient(Chain);

            var response = await client
                 .PostAsync($"v2/{Constant.AlchemyKey}", content);

            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadFromJsonAsync<AbstractTransactionData>();

            if (responseData != null && responseData?.Result != null && responseData?.Result?.Transfers?.Count > 0)
            {
                var tranx = responseData?.Result?.Transfers!;
                foreach (var data in tranx)
                {
                    if (data.RawContract == null || string.IsNullOrEmpty(data.RawContract.Address))
                        continue;

                    await AddTransaction(data, userId, walletId);
                }
                if (!string.IsNullOrEmpty(responseData?.Result.PageKey))
                {
                    await GetFromTransactionNextPageData(address, pageKey, userId, walletId);
                }
            }
        }

        private async Task<AbstractTransactionData?> GetToTransactionData(string address)
        {
            var body = $@"
                {{
                  ""id"": 1,
                  ""jsonrpc"": ""2.0"",
                  ""method"": ""alchemy_getAssetTransfers"",
                  ""params"": [
                    {{
                      ""fromBlock"": ""0x0"",
                      ""toBlock"": ""latest"",
                      ""toAddress"": ""{address}"",
                      ""category"": [
                        ""erc721"",
                        ""erc1155""
                      ],
                      ""order"": ""asc"",
                      ""withMetadata"": true,
                      ""excludeZeroValue"": true,
                      ""maxCount"": ""0x3e8""
                    }}
                  ]
                }}
            ";

            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var client = _factory.CreateClient(Chain);

            var response = await client
                 .PostAsync($"v2/{Constant.AlchemyKey}", content);

            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadFromJsonAsync<AbstractTransactionData>();

            return responseData;
        }

        private async Task GetToTransactionNextPageData(string address, string pageKey, Guid userId, byte[] walletId)
        {
            var body = $@"
                {{
                  ""id"": 1,
                  ""jsonrpc"": ""2.0"",
                  ""method"": ""alchemy_getAssetTransfers"",
                  ""params"": [
                    {{
                      ""fromBlock"": ""0x0"",
                      ""toBlock"": ""latest"",
                      ""toAddress"": ""{address}"",
                      ""category"": [
                        ""erc721"",
                        ""erc1155""
                      ],
                      ""order"": ""asc"",
                      ""withMetadata"": true,
                      ""excludeZeroValue"": true,
                      ""maxCount"": ""0x3e8"",
                      ""pageKey"": ""{pageKey}""
                    }}
                  ]
                }}
            ";

            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var client = _factory.CreateClient(Chain);

            var response = await client
                 .PostAsync($"v2/{Constant.AlchemyKey}", content);

            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadFromJsonAsync<AbstractTransactionData>();

            if (responseData != null && responseData?.Result != null && responseData?.Result?.Transfers?.Count > 0)
            {
                var tranx = responseData?.Result?.Transfers!;
                foreach (var data in tranx)
                {
                    if (data.RawContract == null || string.IsNullOrEmpty(data.RawContract.Address))
                        continue;

                    await AddTransaction(data, userId, walletId);
                }

                if (!string.IsNullOrEmpty(responseData?.Result.PageKey))
                {
                    await GetToTransactionNextPageData(address, pageKey, userId, walletId);
                }
            }
        }

        private async Task<NftSingle?> GetSignleNft(string? contractAddress, string? tokenAddress)
        {
            if (string.IsNullOrEmpty(contractAddress) && string.IsNullOrEmpty(tokenAddress))
                return null;

            var client = _factory.CreateClient(Constant.Abstract);
            var response = await client
                .GetFromJsonAsync<AbstractContractNftData2>($"nft/v3/{Constant.AlchemyKey}/getNFTsForContract?contractAddress={contractAddress}&withMetadata=true&startToken={tokenAddress}&limit=1");

            if (response != null && response?.Nfts?.Count > 0)
            {
                return response.Nfts.FirstOrDefault();
            }

            return null;
        }

        internal override async Task<int> GetNftCountAsync(string address, string? chain = null)
        {
            var client = _factory.CreateClient(Constant.Abstract);

            var response = await client
                .GetFromJsonAsync<AlchemyNftData>($"nft/v3/{Constant.AlchemyKey}/getNFTsForOwner?owner={address}&withMetadata=true&pageSize=100");

            if (response != null && response?.OwnedNfts?.Count > 0)
            {
                var data = response.OwnedNfts;
                var nftCount = data.Count;

                if (response.PageKey != null)
                {
                    nftCount += await GetNextPageNftsCount(address, response.PageKey);
                }

                return nftCount;
            }
            _sentryService.AddBreadcrumb("No NFTs found for the user.", "fetch.nfts", new Dictionary<string, string> { { "address", address }, { "chain", Chain } });
            return 0;
        }

    }
}
