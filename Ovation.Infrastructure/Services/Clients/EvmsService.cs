using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.Abstract;
using Ovation.Application.DTOs.NFTScan.ENV;
using Ovation.Domain.Entities;
using Ovation.Persistence.Services.BackgroundServices;
using Quartz;
using System.Globalization;
using System.Net.Http.Json;
using System.Text;

namespace Ovation.Persistence.Services.Clients
{
    class EvmsService(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseClientService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(EvmsService);
        public List<OwnedNft> Nfts { get; set; } = new();
        public decimal PortfolioValue { get; set; } = 0M;
        public UserNftDatum? TopNft { get; set; } = null;
        public int NftCreated { get; set; } = 0;
        public int NftCollection { get; set; } = 0;
        public UserWallet? Wallet { get; set; }
        public int BlueChip { get; set; } = 0;
        public int FounderNft { get; set; } = 0;
        public List<UserBlueChip> UserBlueChips { get; set; } = new();

        public async Task Execute(IJobExecutionContext contextt)
        {
            var jobData = contextt.MergedJobDataMap;

            _sentryService.AddBreadcrumb("Get Evms NFTs initiated");

            if (jobData != null)
            {
                var address = jobData.GetString("Address");
                var chain = jobData.GetString("Chain");
                var id = jobData.GetString("UserId");

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(chain))
                    return;

                var userId = new Guid(id);

                Wallet = await _context.UserWallets
                .OrderByDescending(_ => _.CreatedDate)
                .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.WalletAddress == address);

                if (Wallet == null)
                    return;

                if (Wallet.WalletId != null || chain.Equals(Constant.Evm, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var item in Constant._alchemyChains)
                    {
                        _sentryService.AddBreadcrumb("Initiated the operation to fetch NFTs.", "fetch.nfts",
                    new Dictionary<string, string> { { "address", address }, { "chain", item } });
                        await GetUserNftsAsync(address, userId, item);
                    }
                }
                else{
                    _sentryService.AddBreadcrumb("Initiated the operation to fetch NFTs.", "fetch.nfts",
                    new Dictionary<string, string> { { "address", address }, { "chain", chain } });
                    await GetUserNftsAsync(address, userId, chain);
                }

                await SaveNotificationAsync(userId);

                _sentryService.AddBreadcrumb("Sync User custody date started");
                await SyncCustodyDate(address, Wallet.Id);

                _sentryService.AddBreadcrumb("Sync User custody date completed");
                
            }
            _span?.Finish();
        }

        protected override async Task GetUserNftsAsync(string address, Guid userId, string? chain)
        {
            if (userId != Guid.Empty)
            {
                try
                {
                    chain = chain.ToLower();                    

                    if (Wallet != null)
                    {
                        var client = _factory.CreateClient($"{Constant.Alchemy}{chain}");

                        if (client == null || client.BaseAddress == null) { return; }

                        _sentryService.AddBreadcrumb("Fetching NFTs...");
                        var response = await client
                        .GetFromJsonAsync<AlchemyNftData>($"nft/v3/{Constant.AlchemyKey}/getNFTsForOwner?owner={address}&withMetadata=true&pageSize=100");

                        if (response != null && response?.OwnedNfts?.Count > 0)
                        {
                            var data = response.OwnedNfts;
                            foreach (var nft in data)
                            {
                                Nfts.Add(nft);
                            }

                            if (response.PageKey != null)
                            {
                                await GetNextPageNfts(chain, address, response.PageKey);
                            }

                            _sentryService.AddBreadcrumb("Adding NFTs and collections to database started");
                            await HandleNftsAndCollections(chain, userId, Wallet);

                            _sentryService.AddBreadcrumb("Adding NFTs and collections to database completed");

                            var usd = PortfolioValue * Constant._chainsToValue[chain];
                            if (decimal.TryParse(Wallet.NftsValue, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal value))
                                Wallet.NftsValue += (value + usd).ToString();

                            Wallet.NftCount = Wallet.NftCount != null ? Wallet.NftCount.Value  + response.TotalCount : 0;
                            Wallet.Migrated = 1;
                            Wallet.Blockchain = Constant.Evm;
                            //wallet.NftsValue = (PortfolioValue * Constant._chainsToValueFloor[Constant.Abstract]).ToString();

                            _context.UserWallets.Update(Wallet);

                            _sentryService.AddBreadcrumb("Updating user stats started");
                            await UpdateUserStatAsync(response.TotalCount, chain, userId, address, Wallet.Id);
                            _sentryService.AddBreadcrumb("Updating user stats completed");

                            NftCreated = 0;
                            Nfts.Clear();
                            PortfolioValue = 0;
                            NftCollection = 0;
                            NftCreated = 0;
                            TopNft = null;
                            BlueChip = 0;
                            FounderNft = 0;
                            UserBlueChips.Clear();
                        }

                        _sentryService.AddBreadcrumb("Get User NFTs transactions started");
                        await GetUserNftTransactionsAsync(address, userId, chain);

                        _sentryService.AddBreadcrumb("Get User NFTs transactions completed");
                    }
                    
                }
                catch (Exception _)
                {
                    SentrySdk.CaptureException(_);
                }
            }
        }

        private async Task GetNextPageNfts(string chain, string address, string cursor)
        {
            var client = _factory.CreateClient($"{Constant.Alchemy}{chain}");

            var response = await client
                .GetFromJsonAsync<AlchemyNftData>($"nft/v3/{Constant.AlchemyKey}/getNFTsForOwner?owner={address}&withMetadata=true&pageSize=100&pageKey={cursor}");

            if (response != null && response?.OwnedNfts?.Count > 0)
            {
                var data = response.OwnedNfts;
                foreach (var nft in data)
                {
                    Nfts.Add(nft);
                }

                if (response.PageKey != null)
                {
                    await GetNextPageNfts(chain, address, response.PageKey);
                }
            }
        }

        private async Task<int> GetNextPageNftsCount(string chain, string address, string cursor)
        {
            var client = _factory.CreateClient($"{Constant.Alchemy}{chain}");

            var response = await client
                .GetFromJsonAsync<AlchemyNftData>($"nft/v3/{Constant.AlchemyKey}/getNFTsForOwner?owner={address}&withMetadata=true&pageSize=100&pageKey={cursor}");

            if (response != null && response?.OwnedNfts?.Count > 0)
            {
                var data = response.OwnedNfts;
                var nftCount = data.Count;

                if (response.PageKey != null)
                {
                    nftCount += await GetNextPageNftsCount(chain, address, response.PageKey);
                }

                return nftCount;
            }

            return 0;
        }

        private async Task HandleNftsAndCollections(string chain, Guid userId, UserWallet wallet)
        {
            var collectionGrouping = Nfts.GroupBy(_ => _.Contract?.Address);
            NftCollection = collectionGrouping.Count();

            var priceService = _serviceScope.ServiceProvider.GetService<CollectionPriceService>();

            foreach (var collection in collectionGrouping)
            {
                if (collection == null) continue;

                var coll = collection?.FirstOrDefault()?.Contract;
                decimal? floorPrice = null;

                if (priceService != null) floorPrice = await priceService.GetFloorPriceAsync(chain, coll?.Name, collection?.Key);

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
                    Chain = chain,
                    UserId = userId.ToByteArray(),
                    ParentCollection = await CheckCollectionAsync(collection.Key, chain)
                };
                await _context.UserNftCollectionData.AddAsync(collectionn);
                await _context.SaveChangesAsync();

                var id = collectionn.Id;

                foreach (var nft in collection!)
                {
                    await AddNft(nft, id, chain, collectionn.FloorPrice, userId, wallet);
                    if (floorPrice != null)
                        PortfolioValue += (decimal)(floorPrice / Constant._chainsToValue[chain]);
                }
            }
        }

        private async Task AddNft(OwnedNft nft, long collectionId, string chain, string? floorPrice, Guid userId, UserWallet wallet)
        {
            var userNft = new UserNftDatum
            {
                Description = nft.Description,
                ImageUrl = !string.IsNullOrEmpty(nft?.Image?.OriginalUrl) ? nft?.Image?.OriginalUrl : nft?.Image?.CachedUrl,
                MetaData = JsonConvert.SerializeObject(nft),
                Name = nft.Name,
                Chain = chain,
                UserId = userId.ToByteArray(),
                UserWalletId = wallet != null ? wallet.Id : null,
                TokenId = nft.TokenId,
                CollectionId = collectionId,
                FloorPrice = floorPrice,
                //LastTradePrice = (!string.IsNullOrEmpty(nft.Price) && decimal.TryParse(nft.Price, out decimal value)) ? (value / Constant.StarsConvert).ToString() : null,
                //LastTradeSymbol = Chain,
                ContractAddress = nft.Contract?.Address
            };

            if (nft.Contract?.Address != null && nft.Contract?.Address.ToLower() == Constant._founderNft && nft.TokenId == Constant._founderNftTokenId)
                FounderNft += 1;

            if (!string.IsNullOrEmpty(nft.Contract?.Address))
                if (Constant._blueChipNfts.TryGetValue(nft.Contract.Address.ToLower(), out string _))
                {
                    var bluechip = await _context.BlueChips.FirstOrDefaultAsync(_ => _.ContractAddress == nft.Contract.Address!);
                    if (bluechip != null && wallet != null)
                    {
                        UserBlueChips.Add(new UserBlueChip { BluechipId = bluechip.Id, UserId = userId.ToByteArray(), UserWalletId = wallet.Id });
                        BlueChip += 1;
                    }

                }

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

        private async Task UpdateUserStatAsync(int nftCount, string chain, Guid userId, string address, byte[] walletId)
        {
            //var transaction = context.Database.CurrentTransaction;

            //if (transaction == null)
            //    transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var stats = await _context.UserStats.FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());
                if (stats != null)
                {
                    var worth = PortfolioValue * Constant._chainsToValue[chain];
                    stats.Networth += worth;
                    stats.TotalNft += nftCount;
                    stats.NftCreated += NftCreated;
                    stats.NftCollections += NftCollection;

                    worth = stats.Networth;

                    await _context.UserPortfolioRecords.AddAsync(new UserPortfolioRecord { UserId = userId.ToByteArray(), Value = PortfolioValue.ToString(), Address = address, Chain = chain, UsdValue = worth });

                    await _context.UserNftRecords.AddAsync(new UserNftRecord { NftCount = nftCount, UserId = userId.ToByteArray(), Address = address, Chain = chain });

                    if (UserBlueChips != null && UserBlueChips.Count > 0) await _context.UserBlueChips.AddRangeAsync(UserBlueChips);


                    //Handle to valued nft
                    if (TopNft != null && !string.IsNullOrEmpty(TopNft.FloorPrice))
                    {
                        var userTopNft = await _context.UserHighestNfts.FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

                        var newWorth = Math.Round(decimal.Parse(TopNft.FloorPrice) * Constant._chainsToValue[chain], 2);

                        if (userTopNft != null)
                        {

                            var oldWorth = Math.Round((decimal)(userTopNft.Worth * Constant._chainsToValue[userTopNft.TradeSymbol ?? userTopNft.Chain!])!, 2);
                            var isOld = true;
                            if (newWorth > oldWorth)
                                isOld = false;

                            userTopNft.Worth = !isOld ? decimal.Parse(TopNft.FloorPrice) : userTopNft.Worth;
                            userTopNft.WalletId = walletId;
                            userTopNft.Chain = chain;
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
                                Chain = chain,
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
                            .FirstOrDefaultAsync(_ => _.UserWalletId == walletId && _.UserId == userId.ToByteArray() && _.Chain == chain);

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
                                Chain = chain,
                                UserId = userId.ToByteArray(),
                                UserWalletId = walletId
                            };

                            await _context.UserWalletValues.AddAsync(walletValue);
                        }

                    }

                    if(nftCount > 0)
                    {
                        var meta = Wallet.MetaData;

                        if (meta != null)
                        {
                            var data = JsonConvert.DeserializeObject<UserWalletMetaData>(meta)!;

                            data.IsMultiChain = true;
                            data.MultiChains.Add(chain);

                            Wallet.MetaData = JsonConvert.SerializeObject(data);
                        }
                        else
                        {
                            var data = new UserWalletMetaData();

                            data.IsMultiChain = true;
                            data.MultiChains.Add(chain);

                            Wallet.MetaData = JsonConvert.SerializeObject(data);
                        }

                        _context.UserWallets.Update(Wallet);
                    }
                    

                    await _context.SaveChangesAsync();
                    //await transaction.CommitAsync();
                }
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
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
                TriggerKey triggerKey = new TriggerKey(triggerIdentity, "evmCollectionData");

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
            if (string.IsNullOrEmpty(chain)) return;

            if (chain.Equals(Constant.Eth, StringComparison.OrdinalIgnoreCase)
                || chain.Equals(Constant.Polygon, StringComparison.OrdinalIgnoreCase)
                || chain.Equals(Constant.Base, StringComparison.OrdinalIgnoreCase)
                || chain.Equals(Constant.Optimism, StringComparison.OrdinalIgnoreCase)
                )
                await GetUserNftTransactionAsync(address, chain, userId);
            else
                await GetUserNftAlchemyTransactionAsync(chain, address, userId);
        }

        //NftScan transaction functions

        internal async Task GetUserNftTransactionAsync(string address, string chain, Guid userId)
        {
            var wallet = await _context.UserWallets
                    .OrderByDescending(_ => _.CreatedDate)
                    .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.WalletAddress == address);

            if (wallet != null)
            {
                var client = _factory.CreateClient(chain);

                if (client == null || client.BaseAddress == null) { return; }

                var tranxData = await client
                .GetFromJsonAsync<EVMsNftTransactionData?>($"api/v2/transactions/account/{address}?event_type=Mint;Transfer;Sale;Burn&sort_direction=desc&limit=100");

                if (tranxData != null && tranxData.Data != null && tranxData.Data?.Content?.Count > 0)
                {
                    await HandleTransactionData(tranxData, address, chain, wallet.Id, userId);

                    if (!string.IsNullOrEmpty(tranxData.Data.Next))
                        await GetNextPageOfTransactions(tranxData.Data.Next, address, chain, wallet.Id, userId);
                }
            }
        }

        private async Task HandleTransactionData(EVMsNftTransactionData tranxData, string address, string chain, byte[] walletId, Guid userId)
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

            if(stats != null)
            {
                await _context.UserWalletSalesRecords.AddAsync(new UserWalletSalesRecord 
                { TotalSales = soldValue, TotalSold = soldCount, Chain = chain, WalletId = walletId, UserId = userId.ToByteArray()});

                stats.SoldNftsTotal += soldCount;
                stats.SoldNftsValue += soldValue;

                var wallet = await _context.UserWallets
                .FirstOrDefaultAsync(_ => _.Id == walletId);

                if(wallet != null)
                {
                    wallet.TotalSales += soldValue;
                    wallet.TotalSold += soldCount;
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task GetNextPageOfTransactions(string cursor, string address, string chain, byte[] walletId, Guid userId)
        {
            var client = _factory.CreateClient(chain);

            if (client == null || client.BaseAddress == null) { return; }

            var tranxData = await client
            .GetFromJsonAsync<EVMsNftTransactionData?>($"api/v2/transactions/account/{address}?event_type=Mint;Transfer;Sale;Burn&sort_direction=desc&limit=100&cursor={cursor}");

            if (tranxData != null && tranxData.Data != null && tranxData.Data?.Content?.Count > 0)
            {
                await HandleTransactionData(tranxData, address, chain, walletId, userId);

                if (!string.IsNullOrEmpty(tranxData.Data.Next))
                    await GetNextPageOfTransactions(tranxData.Data.Next, address, chain, walletId, userId);
            }
        }


        private bool IsUserTheSeller(Content data, string address, string chain, ref decimal soldValue)
        {
            int qty = !string.IsNullOrEmpty(data?.Amount) ? int.Parse(data.Amount) : 1;
            if (data != null 
                && string.Equals(data.EventType, "sale", StringComparison.OrdinalIgnoreCase)
                && string.Equals(data.Send, address, StringComparison.OrdinalIgnoreCase))
            {
                if(!string.IsNullOrEmpty(data.TradeSymbol) && Constant._chainsToValue.TryGetValue(data.TradeSymbol, out decimal rate))
                    soldValue += Math.Round(data.TradePrice * rate, 2);
                return true;
            }

            return false;
        }

        private async Task AddTransaction(Content data, string chain, Guid userId, byte[] walletId)
        {
            var name = string.Empty;
            var img = string.Empty;

            var nft = await _context.UserNftData
                .Where(_ => _.Chain == chain && _.ContractAddress == data.ContractAddress && (_.TokenAddress == data.ContractTokenId || _.TokenId == data.TokenId))
                .Select(x => new
                {
                    x.Name,
                    x.ImageUrl
                })
                .FirstOrDefaultAsync();

            if(nft == null || string.IsNullOrEmpty(nft.Name) || string.IsNullOrEmpty(nft.ImageUrl))
            {
                var singleNft = await GetSingleNftDataAsync(chain, data.ContractAddress!, data.TokenId!);

                if(singleNft != null &&singleNft.Data != null)
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
            try
            {
                var tranx = new UserNftTransaction
                {
                    TokenId = data.TokenId,
                    ContractTokenId = data.ContractTokenId,
                    ContractAddress = data.ContractAddress,
                    ContractName = data.ContractName,
                    EventType = data.EventType,
                    ExchangeName = data.ExchangeName,
                    Fee = data?.GasFee?.ToString(),
                    Image = img,
                    Name = name,
                    Qty = long.Parse(data.Amount),
                    TradePrice = data.TradePrice.ToString(),
                    TradeSymbol = data.TradeSymbol,
                    Data = JsonConvert.SerializeObject(data),
                    TranxId = data.NftscanTxId,
                    UserId = userId.ToByteArray(),
                    UserWalletId = walletId,
                    Chain = chain,
                    TranxDate = DateTimeOffset.FromUnixTimeMilliseconds(data.Timestamp).UtcDateTime,

                    From = (data.EventType.Equals("sale", StringComparison.OrdinalIgnoreCase)) ? data.Send : (data.EventType.Equals("mint", StringComparison.OrdinalIgnoreCase)) ? data.To :
                    (data.EventType.Equals("transfer", StringComparison.OrdinalIgnoreCase)) ? data.Send : (data.EventType.Equals("burn", StringComparison.OrdinalIgnoreCase)) ? data.From : "",
                    To = data.Receive
                };

                await _context.UserNftTransactions.AddAsync(tranx);
                await _context.SaveChangesAsync();
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
            }
            

        }

        private async Task<SingleNftData?> GetSingleNftDataAsync(string chain, string contractAddress, string tokenId)
        {
            var client = _factory.CreateClient(chain);

            return await client
            .GetFromJsonAsync<SingleNftData?>($"api/v2/assets/{contractAddress}/{tokenId}?show_attribute=false");
        }

        internal async Task<List<NftTransactionDto>?> GetNftTransactionAsync(string? contractAddress, string? tokenId, string? chain)
        {
            var client = _factory.CreateClient(chain);

            if (client == null || client.BaseAddress == null) { return null; }

            var tranxData = await client
            .GetFromJsonAsync<EVMsNftTransactionData?>($"api/v2/transactions/{contractAddress}/{tokenId}?event_type=Mint;Transfer;Sale;Burn&sort_direction=desc&limit=100");            

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
                        TradeSymbol = data.TradeSymbol,
                        From = (data.EventType.Equals("sale", StringComparison.OrdinalIgnoreCase)) ? data.Send 
                        : (data.EventType.Equals("mint", StringComparison.OrdinalIgnoreCase)) ? data.To :
                        (data.EventType.Equals("transfer", StringComparison.OrdinalIgnoreCase)) ? data.Send :
                        (data.EventType.Equals("burn", StringComparison.OrdinalIgnoreCase)) ? data.From : "",
                        To = data.Receive,
                        Quantity = data.Amount
                    });
                }

                return transactionData;
            }

            return null;
        }



        //Alchemy transaction functions

        internal async Task GetUserNftAlchemyTransactionAsync(string chain, string address, Guid userId)
        {
            var wallet = await _context.UserWallets
                    .OrderByDescending(_ => _.CreatedDate)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.WalletAddress == address);

            if (wallet != null)
            {
                var getFromTranx = await GetFromTransactionData(chain, address);

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

                        await AddTransaction(data, chain, userId, wallet.Id);
                    }

                    if (!string.IsNullOrEmpty(getFromTranx?.Result.PageKey))
                    {
                        await GetFromTransactionNextPageData(chain, address, getFromTranx.Result.PageKey!, userId, wallet.Id);
                    }
                }

                var getToTranx = await GetToTransactionData(chain, address);

                if (getToTranx != null && getToTranx?.Result != null && getToTranx?.Result?.Transfers?.Count > 0)
                {
                    var tranx = getToTranx?.Result?.Transfers!;
                    foreach (var data in tranx)
                    {
                        if (data.RawContract == null || string.IsNullOrEmpty(data.RawContract.Address))
                            continue;

                        await AddTransaction(data, chain, userId, wallet.Id);
                    }

                    if (!string.IsNullOrEmpty(getToTranx?.Result.PageKey))
                    {
                        await GetToTransactionNextPageData(chain, address, getToTranx.Result.PageKey!, userId, wallet.Id);
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

        private async Task AddTransaction(Transfer data, string chain, Guid userId, byte[] walletId)
        {
            var nft = await GetSignleNft(chain, data?.RawContract?.Address, data?.TokenId);

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
                TradeSymbol = chain,
                Data = JsonConvert.SerializeObject(data),
                TranxId = data?.UniqueId,
                UserId = userId.ToByteArray(),
                UserWalletId = walletId,
                Chain = chain,
                TranxDate = data?.Metadata?.BlockTimestamp,
                From = data?.From,
                To = data?.To
            };

            await _context.UserNftTransactions.AddAsync(tranx);
            await _context.SaveChangesAsync();
        }

        private async Task<AbstractTransactionData?> GetFromTransactionData(string chain, string address)
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

            var client = _factory.CreateClient($"{Constant.Alchemy}{chain}");

            if (client == null || client.BaseAddress == null) { return null; }

            var response = await client
                 .PostAsync($"v2/{Constant.AlchemyKey}", content);

            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadFromJsonAsync<AbstractTransactionData>();

            return responseData;
        }

        private async Task GetFromTransactionNextPageData(string chain, string address, string pageKey, Guid userId, byte[] walletId)
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

            var client = _factory.CreateClient($"{Constant.Alchemy}{chain}");

            if (client == null || client.BaseAddress == null) { return; }

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

                    await AddTransaction(data, chain, userId, walletId);
                }
                if (!string.IsNullOrEmpty(responseData?.Result.PageKey))
                {
                    await GetFromTransactionNextPageData(chain, address, pageKey, userId, walletId);
                }
            }
        }

        private async Task<AbstractTransactionData?> GetToTransactionData(string chain, string address)
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

            var client = _factory.CreateClient($"{Constant.Alchemy}{chain}");

            if (client == null || client.BaseAddress == null) { return null; }

            var response = await client
                 .PostAsync($"v2/{Constant.AlchemyKey}", content);

            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadFromJsonAsync<AbstractTransactionData>();

            return responseData;
        }

        private async Task GetToTransactionNextPageData(string chain, string address, string pageKey, Guid userId, byte[] walletId)
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

            var client = _factory.CreateClient($"{Constant.Alchemy}{chain}");

            if (client == null || client.BaseAddress == null) { return; }

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

                    await AddTransaction(data, chain, userId, walletId);
                }

                if (!string.IsNullOrEmpty(responseData?.Result.PageKey))
                {
                    await GetToTransactionNextPageData(chain, address, pageKey, userId, walletId);
                }
            }
        }

        private async Task<NftSingle?> GetSignleNft(string chain, string? contractAddress, string? tokenAddress)
        {
            if (string.IsNullOrEmpty(contractAddress) && string.IsNullOrEmpty(tokenAddress))
                return null;

            var client = _factory.CreateClient($"{Constant.Alchemy}{chain}");
            var response = await client
                .GetFromJsonAsync<AbstractContractNftData2>($"nft/v3/{Constant.AlchemyKey}/getNFTsForContract?contractAddress={contractAddress}&withMetadata=true&startToken={tokenAddress}&limit=1");

            if (response != null && response?.Nfts?.Count > 0)
            {
                return response.Nfts.FirstOrDefault();
            }

            return null;
        }

        internal override async Task<int> GetNftCountAsync(string address, string? chain = default)
        {
            var client = _factory.CreateClient($"{Constant.Alchemy}{chain}");

            var response = await client
            .GetFromJsonAsync<AlchemyNftData>($"nft/v3/{Constant.AlchemyKey}/getNFTsForOwner?owner={address}&withMetadata=true&pageSize=100");

            if (response != null && response?.OwnedNfts?.Count > 0)
            {
                var data = response.OwnedNfts;
                var nftCount = data.Count;

                if (response.PageKey != null)
                {
                    nftCount += await GetNextPageNftsCount(chain!, address, response.PageKey);
                }

                return nftCount;
            }
            _sentryService.AddBreadcrumb("No NFTs found for the user.", "fetch.nfts", new Dictionary<string, string> { { "address", address }, { "chain", chain! } });
            return 0;
        }
    }
}
