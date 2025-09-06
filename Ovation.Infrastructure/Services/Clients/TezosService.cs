using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.Tezos;
using Ovation.Domain.Entities;
using Ovation.Persistence.Services.BackgroundServices;
using Quartz;
using System.Globalization;
using System.Net.Http.Json;

namespace Ovation.Persistence.Services.Clients
{
    class TezosService(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseClientService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(TezosService);
        public List<NotificationDto> Notifications { get; set; } = new();
        public List<UserNftDatum> Nfts { get; set; } = new();
        public decimal PortfolioValue { get; set; } = 0M;
        public List<decimal> NftValue { get; set; } = new();
        public int NftCreated { get; set; } = 0;
        public string Chain { get; set; } = "tezos";
        public decimal SoldValue { get; set; } = 0.00M;

        public Dictionary<string, long> _nftCollections = new(StringComparer.OrdinalIgnoreCase);


        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            _sentryService.AddBreadcrumb("Get Tezos NFTs initiated");
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
                        .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.Chain == "tezos" && _.WalletAddress == address);

                    if (wallet == null) return;

                    var wallets = await _context.UserWallets
                                .Where(_ => _.UserId == userId.ToByteArray() && _.WalletId != null && _.Chain == Chain)
                                .ToListAsync();

                    var client = _factory.CreateClient(Constant.Tezos);
                    _sentryService.AddBreadcrumb("Fetching NFTs...");

                    var response = await client
                        .GetFromJsonAsync<List<TezosNft>>($"v1/tokens/balances?account={address}&token.standard=fa2&limit=10000");

                    if (response != null && response.Count > 0)
                    {
                        _sentryService.AddBreadcrumb("Adding NFTs and collections to database started");
                        foreach (var nft in response)
                        {
                            await AddNft(nft, response, wallet, userId, wallets);                            
                        }

                        _sentryService.AddBreadcrumb("Adding NFTs and collections to database completed");

                        var nftCount = Nfts.Count;

                        var usd = PortfolioValue * Constant._chainsToValue[Chain];
                        wallet.NftsValue = usd.ToString("F2");

                        wallet.NftCount = nftCount;
                        wallet.Migrated = 1;
                        wallet.Blockchain = Constant.Tezos;

                        _sentryService.AddBreadcrumb("Updating user stats started");
                        await UpdateUserStatAsync(nftCount, userId, address, wallet.Id);

                        _sentryService.AddBreadcrumb("Updating user stats completed");


                        await SaveNotifications(userId);

                        NftCreated = 0;
                        NftValue.Clear();
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
                catch (Exception _)
                {
                    SentrySdk.CaptureException(_);
                }
            }
        }

        private async Task AddNft(TezosNft nft, List<TezosNft> response, UserWallet wallet, Guid userId, List<UserWallet> wallets)
        {
            var contractAddress = nft?.Token?.Contract?.Address?.Trim();
            var tokenId = nft?.Token?.TokenId?.Trim();
            var price = 0M;

            if (!string.IsNullOrEmpty(contractAddress))
            {
                if (!_nftCollections.ContainsKey(contractAddress))
                {
                    var owned = response.Count(_ => _.Token?.Contract?.Address == contractAddress);

                    var psrentCollection = await CheckCollectionAsync(contractAddress, Chain);

                    await AddCollectionDetailsObjktAsync(contractAddress, owned, wallet!.Id, userId, psrentCollection);

                }

                if (!string.IsNullOrEmpty(tokenId))
                {
                    price = await GetNftPriceAsync(contractAddress, tokenId);

                    PortfolioValue += price;
                    NftValue.Add(price);
                }
            }

            var isCreated = false;
            if (wallets != null)
            {
                foreach (var item in wallets)
                {
                    if (item.WalletId != null && item.WalletAddress != null)
                        if (item.WalletAddress.Equals(nft?.Token?.Metadata?.Creators?.FirstOrDefault(), StringComparison.OrdinalIgnoreCase))
                        {
                            NftCreated += 1;
                            isCreated = true;
                        }                            
                }
            }

            var collId = _nftCollections.TryGetValue(contractAddress, out var id);

            var userNft = new UserNftDatum
            {
                Description = nft?.Token?.Metadata?.Description,
                CollectionId = (collId) ? id : null,
                ImageUrl = !string.IsNullOrEmpty(nft?.Token?.Metadata?.Image) ? nft.Token.Metadata.Image :
                !string.IsNullOrEmpty(nft?.Token?.Metadata?.DisplayUri) ? nft.Token.Metadata.DisplayUri : nft?.Token?.Metadata?.ThumbnailUri,
                ContractAddress = contractAddress,
                MetaData = JsonConvert.SerializeObject(nft),
                Name = nft?.Token?.Metadata?.Name,
                Chain = Chain,
                UserId = userId.ToByteArray(),
                UserWalletId = wallet != null ? wallet.Id : null,
                TokenId = nft?.Token?.TokenId,
                Public = 1,
                Created = (sbyte)(isCreated ? 1 : 0),
                LastTradePrice = price.ToString("G29"),
                LastTradeSymbol = Chain
            };
            await _context.UserNftData.AddAsync(userNft);
            await _context.SaveChangesAsync();

            Nfts.Add(userNft);            
            await _context.SaveChangesAsync();
        }

        private async Task<decimal> GetNftPriceAsync(string contractAddress, string tokenId)
        {
            var clientt = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://data.objkt.com/v3/graphql");
            var content = new StringContent("query { token(where: {fa_contract: {_eq: \""+contractAddress+"\"}, token_id: {_eq: \""+tokenId+"\"}}) { listings(where: {status: {_eq: \"active\"}}) { price_xtz  } } }", null, "application/json");
            request.Content = content;

            try
            {
                HttpResponseMessage response = await clientt.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<TezosNftPriceData?>(jsonResponse);

                    if (data == null) return 0;

                    var firstToken = data.Data?.Token?.FirstOrDefault();
                    var firstListing = firstToken?.Listings?.FirstOrDefault();

                    if (firstListing?.Price != null && Constant.Microtez != 0)
                    {
                        return Math.Round(firstListing.Price / Constant.Microtez, 8);
                    }

                    return Math.Round(0M, 8);
                }
                else
                {
                    return Math.Round(0M, 8);
                }
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return Math.Round(0M, 8);
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
                    .WithIdentity($"get-tezos-collection-data-{contractAddress}")
                    .ForJob(GetTezosCollectionDataJob.Name)
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

        private async Task AddCollectionDetailsObjktAsync(string contractAddress, int ownsTotal, byte[] walletId, Guid userId, long? collectionId)
        {
            var clientt = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://data.objkt.com/v3/graphql");
            var content = new StringContent("query { fa(where: {contract: {_eq: \""+contractAddress+"\"}}) { name description floor_price items logo short_name } }", null, "application/json");
            request.Content = content;

            try
            {
                HttpResponseMessage response = await clientt.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<TezosObjktCollectionData?>(jsonResponse);
                    var coll = data?.Data?.Fa?.FirstOrDefault();

                    if(coll != null)
                    {
                        if (!_nftCollections.ContainsKey(contractAddress))
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
                                Chain = Chain,
                                UserId = userId.ToByteArray(),
                                ParentCollection = collectionId
                            };
                            await _context.UserNftCollectionData.AddAsync(collection);
                            await _context.SaveChangesAsync();

                            _nftCollections.Add(contractAddress, collection.Id);
                        }
                    }
                    else
                    {
                        await AddCollectionAsync(contractAddress, ownsTotal, walletId, userId, collectionId);
                    }
                }
                else
                {
                    await AddCollectionAsync(contractAddress, ownsTotal, walletId, userId, collectionId);
                }
            }
            catch (Exception _)
            {
            }
        }

        private async Task AddCollectionAsync(string contractAddress, int ownsTotal, byte[] walletId, Guid userId, long? collectionId)
        {
            try
            {
                var client = _factory.CreateClient(Chain!);

                var response = await client
                    .GetFromJsonAsync<TezosCollectionData>($"v1/accounts/{contractAddress}?legacy=false");

                if (response != null && !_nftCollections.ContainsKey(contractAddress))
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
                        Chain = Chain,
                        UserId = userId.ToByteArray(),
                        ParentCollection = collectionId
                    };
                    await _context.UserNftCollectionData.AddAsync(collection);
                    await _context.SaveChangesAsync();

                    _nftCollections.Add(contractAddress, collection.Id);
                }
            }
            catch (Exception _)
            {
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
                    var worth = PortfolioValue * Constant.TezosValue;
                    stats.Networth += worth;
                    stats.TotalNft += Nfts.Count;
                    stats.NftCreated += NftCreated;

                    worth = stats.Networth;

                    //if (Nfts != null && Nfts.Count > 0) await context.BulkInsertOrUpdateAsync<UserNftDatum>(Nfts, new BulkConfig
                    //{
                    //    SetOutputIdentity = true, // Only insert new records; ignore updates
                    //});

                    await _context.UserPortfolioRecords.AddAsync(new UserPortfolioRecord { UserId = userId.ToByteArray(), Value = PortfolioValue.ToString("F8"), Address = address, Chain = Chain, UsdValue = worth });

                    await _context.UserNftRecords.AddAsync(new UserNftRecord { NftCount = nftCount, UserId = userId.ToByteArray(), Address = address, Chain = Chain });


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
                            userTopNft.Chain = Chain;
                            userTopNft.UserId = userId.ToByteArray();
                            userTopNft.Name = newWorth > oldWorth ? topNft.Name : userTopNft.Name;
                            userTopNft.ImageUrl = newWorth > oldWorth ? topNft.ImageUrl : userTopNft.ImageUrl;
                            userTopNft.TradeSymbol = newWorth > oldWorth ? topNft.LastTradeSymbol : userTopNft.TradeSymbol;
                            userTopNft.Usd = newWorth > oldWorth ? Math.Round(newWorth, 2) : Math.Round(oldWorth, 2);
                            userTopNft.NftId = newWorth > oldWorth ? topNft.Id : userTopNft.NftId;

                            topValue = userTopNft.Usd.Value;
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

                            topValue = userHighNft.Usd.Value;
                        }

                        //update wallet values based on chain
                        var walletVal = await _context.UserWalletValues
                            .FirstOrDefaultAsync(_ => _.UserWalletId == walletId && _.UserId == userId.ToByteArray() && _.Chain == Chain);

                        if (walletVal != null)
                        {
                            walletVal.NftCount = nftCount;
                            walletVal.NativeWorth = nftWorth;
                        }
                        else
                        {
                            var walletValue = new UserWalletValue
                            {
                                NativeWorth = nftWorth,
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
                SentrySdk.CaptureException(_);
                //await transaction.RollbackAsync();
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
                .GetFromJsonAsync<List<TezosNftTransactionData>?>($"v1/tokens/transfers?anyof.from.to={address}&sort.desc=id");

                if (tranxData != null && tranxData?.Count > 0)
                {
                    var soldCount = 0;
                    foreach (var data in tranxData)
                    {
                        if (await IsUserTheSeller(data, address, chain))
                            soldCount += 1;

                        await AddTransaction(data, chain, userId, wallet.Id);
                    }

                    var stats = await _context.UserStats
                        .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

                    if (stats != null)
                    {
                        stats.SoldNftsTotal += soldCount;
                        stats.SoldNftsValue += SoldValue;
                      
                        wallet.TotalSales = SoldValue;
                        wallet.TotalSold = soldCount;

                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        private async Task<bool> IsUserTheSeller(TezosNftTransactionData data, string address, string chain)
        {
            if (data != null && data.From != null 
                && data.From.Address.Equals(address, StringComparison.OrdinalIgnoreCase))
            {
                var contractAddress = data?.Token?.Contract?.Address;
                var tokenId = data?.Token?.TokenId;
                if(!string.IsNullOrWhiteSpace(contractAddress) && !string.IsNullOrWhiteSpace(tokenId))
                    SoldValue += await GetNftPriceAsync(contractAddress, tokenId);
                return true;
            }

            return false;
        }

        private async Task AddTransaction(TezosNftTransactionData data, string chain, Guid userId, byte[] walletId)
        {
            var tranx = new UserNftTransaction
            {
                TokenId = data?.Token?.TokenId,
                ContractTokenId = null,
                ContractAddress = data?.Token?.Contract?.Address,
                ContractName = null,
                EventType = null,
                ExchangeName = data?.Token?.Metadata?.Symbol,
                Fee = null,
                Image = data?.Token?.Metadata?.DisplayUri,
                Name = data?.Token?.Metadata?.Name,
                Qty = int.Parse(data?.Amount),
                TradePrice = null,
                TradeSymbol = "tezos",
                Data = JsonConvert.SerializeObject(data),
                TranxId = data?.TransactionId.ToString(),
                UserId = userId.ToByteArray(),
                UserWalletId = walletId,
                Chain = chain,
                TranxDate = data?.Timestamp,
                From = data?.From?.Address,
                To = data?.To?.Address
            };

            await _context.UserNftTransactions.AddAsync(tranx);
            await _context.SaveChangesAsync();
        }

        internal async Task<List<NftTransactionDto>?> GetNftTransactionAsync(string? contractAddress, string? tokenId)
        {
            var clientt = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://data.objkt.com/v3/graphql");
            var content = new StringContent("query { event( where: { token: { fa_contract: { _eq:  \"" + contractAddress + "\"  }, " +
                "token_id: { _eq: \"" + tokenId + "\" } } }, order_by: { timestamp: desc } ) { event_type amount price_xtz creator " +
                "{ address } recipient { address } timestamp } }", null, "application/json");
            request.Content = content;

            try
            {
                HttpResponseMessage response = await clientt.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var transactionData = JsonConvert.DeserializeObject<ObjktTransactionData?>(jsonResponse);

                    if(transactionData != null && transactionData?.Data?.Event?.Count > 0)
                    {
                        var transactionsData = new List<NftTransactionDto>();
                        foreach (var data in transactionData.Data.Event)
                        {
                            transactionsData.Add(new NftTransactionDto
                            {
                                EventType = data.EventType,
                                Price = (data?.Price / Constant.Microtez).ToString(),
                                TranxDate = data.Timestamp,
                                TradeSymbol = "ton",
                                From = data?.Creator?.Address,
                                To = data?.Recipient?.Address
                            });
                        }

                        return transactionsData;
                    }
                }
                return null;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return null;
            }
        }

        internal override async Task<int> GetNftCountAsync(string address, string? chain = null)
        {
            var client = _factory.CreateClient(Constant.Tezos);
            _sentryService.AddBreadcrumb("Fetching NFTs...");

            var response = await client
                .GetFromJsonAsync<List<TezosNft>>($"v1/tokens/balances?account={address}&token.standard=fa2&limit=10000");

            if (response != null && response.Count > 0)
            {
                return response.Count;
            }

            _sentryService.AddBreadcrumb("No NFTs found for the user.", "fetch.nfts", new Dictionary<string, string> { { "address", address }, { "chain", chain ?? Constant.Tezos } });
            return 0;
        }
    }
}
