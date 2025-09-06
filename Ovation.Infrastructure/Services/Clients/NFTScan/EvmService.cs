using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.NFTScan.ENV;
using Ovation.Domain.Entities;
using Ovation.Persistence.Services.BackgroundServices;
using Quartz;
using System.Globalization;
using System.Net.Http.Json;

namespace Ovation.Persistence.Services.Clients.NFTScan
{
    class EvmService(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseClientService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(EvmService);
        public List<NotificationDto> Notifications { get; set; } = new();
        public int Collections { get; set; } = 0;
        public List<UserNftDatum> Nfts { get; set; } = new();
        public List<UserBlueChip> UserBlueChips { get; set; } = new();
        public decimal PortfolioValue { get; set; } = 0M;
        public List<PortfolioValueRecord> PortfolioValues { get; set; } = new();
        public decimal PortfolioNativeValue { get; set; } = 0M;
        public List<decimal> NftValue { get; set; } = new();
        public int NftCreated { get; set; } = 0;
        public int NftCount { get; set; } = 0;
        public int BlueChip { get; set; } = 0;
        public int FounderNft { get; set; } = 0;
        public UserWallet? Wallet { get; set; }

        public decimal TopValuedNft { get; set; } = 0.00M;


        public async Task Execute(IJobExecutionContext contextt)
        {
            var jobData = contextt.MergedJobDataMap;

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

                if (Wallet.WalletId != null || chain.Equals("evm", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var item in Constant._evmChains)
                    {
                        await GetUserNftsAsync(address, item, userId);
                    }
                }
                else
                    await GetUserNftsAsync(address, chain, userId);

                await HandleUserNewEarnedBadge(userId);

                await SyncCustodyDate(address, Wallet.Id);
            }
        }

        internal async Task GetUserNftsAsync(string address, string chain, Guid userId)
        {
            if (userId != Guid.Empty)
            {
                try
                {
                    chain = chain.ToLower();                    

                    if (Wallet != null)
                    {
                        var client = _factory.CreateClient(chain);

                        var collectionData = await client
                        .GetFromJsonAsync<EVMsNFTData?>($"api/v2/account/own/all/{address}?erc_type=&show_attribute=true");

                        if (collectionData != null && collectionData.Data != null && collectionData.Data.Count > 0)
                        {
                            var wallets = await _context.UserWallets
                                .Where(_ => _.UserId == userId.ToByteArray() && _.WalletId != null && _.Chain == chain)
                                .ToListAsync();

                            foreach (var data in collectionData.Data)
                            {
                                var priceService = _serviceScope.ServiceProvider.GetService<CollectionPriceService>();

                                decimal? floorPrice = null;

                                if (priceService != null) floorPrice = await priceService.GetFloorPriceAsync(chain, data.ContractName, data.ContractAddress);

                                var parentCollection = await CheckCollectionAsync(data.ContractAddress, chain);

                                var collectionId = await AddCollectionAsync(data, floorPrice, chain, Wallet, userId, parentCollection);

                                if (data.Assets != null && data.Assets.Count > 0)
                                {
                                    foreach (var nft in data.Assets)
                                    {
                                        await AddNft(nft, collectionId, floorPrice, chain, Wallet, wallets, userId);
                                        NftCount++;
                                    }
                                }
                            }

                            await SaveDataAsync(userId, chain, address, Wallet.Id);
                           
                            NftCount = 0;
                            NftCreated = 0;
                            BlueChip = 0;
                            FounderNft = 0;
                            NftValue.Clear();
                            Collections = 0;
                            Notifications.Clear();
                            Nfts.Clear();
                            UserBlueChips.Clear();
                            PortfolioNativeValue = 0;
                            PortfolioValue = 0;
                            PortfolioValues.Clear();
                        }

                        await GetUserNftTransactionAsync(address, chain, userId);                        
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

            if(collection == null)
            {
                IScheduler schedular = await _schedulerFactory.GetScheduler();

                var jobData = new JobDataMap
                {
                    {"ContractAddress", contractAddress },
                    {"Chain", chain}
                };

                
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity($"get-evm-collection-data-{contractAddress}")
                    .ForJob(GetEvmsCollectionDataJob.Name)
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

        private async Task<long> AddCollectionAsync(Datum data, decimal? floorPrice, string chain, UserWallet wallet, Guid userId, long? parentCollection)
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
                FloorPrice = floorPrice != null ? floorPrice.ToString() : null,
                Spam = (sbyte)(data.IsSpam != null && data.IsSpam.Value ? 1 : 0),
                Symbol = data.Symbol,
                Chain = chain,
                Verified = (sbyte)(data.Verified != null && data.Verified.Value ? 1 : 0),
                UserWalletId = wallet.Id,
                UserId = userId.ToByteArray(),
                ParentCollection = parentCollection
            };
            //Collections.Add(collection);

            await _context.UserNftCollectionData.AddAsync(collection);
            await _context.SaveChangesAsync();
            Collections += 1;

            return collection.Id;
        }

        private async Task AddNft(Asset nft, long collectionId, decimal? floorPrice, string chain, UserWallet wallet, List<UserWallet> wallets, Guid userId)
        {
            var desc = nft.Description;
            if (string.IsNullOrEmpty(desc) && nft.MetadataJson != null)
            {
                try
                {
                    var anonyObj = JsonConvert.DeserializeObject<dynamic>(nft.MetadataJson.TrimStart('\uFEFF'));

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

            var image = nft.ImageUri;
            if (string.IsNullOrEmpty(image) && nft.MetadataJson != null)
            {
                try
                {
                    var anonyObj = JsonConvert.DeserializeObject<dynamic>(nft.MetadataJson.TrimStart('\uFEFF'));

                    if (anonyObj is JObject jsonObj)
                    {
                        if (jsonObj["image"] != null)
                            image = jsonObj["image"].ToString();
                    }
                }
                catch (Exception _)
                {

                }
            }
            else if (
                    !string.IsNullOrEmpty(image)
                    && nft.MetadataJson != null
                    && !image.Trim().StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                    && !image.Trim().StartsWith("ipfs://", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var anonyObj = JsonConvert.DeserializeObject<dynamic>(nft.MetadataJson.TrimStart('\uFEFF'));

                    if (anonyObj is JObject jsonObj)
                    {
                        if (jsonObj["image"] != null)
                            image = jsonObj["image"].ToString();
                    }
                }
                catch (Exception _)
                {

                }

            }


            var animUrl = string.Empty;
            if (nft.MetadataJson != null)
            {
                try
                {
                    var anonyObj = JsonConvert.DeserializeObject<dynamic>(nft.MetadataJson.TrimStart('\uFEFF'));

                    if (anonyObj is JObject jsonObj)
                    {
                        if (jsonObj["animation_url"] != null)
                            animUrl = jsonObj["animation_url"].ToString();
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
                //Id = SequentialGuidGenerator.Instance.NewGuid().ToByteArray(),
                UserWalletId = wallet.Id,
                CollectionId = collectionId,
                Name = !string.IsNullOrEmpty(nft.Name) ? nft.Name : nft.ContractName,
                ImageUrl = image,
                AnimationUrl = animUrl,
                LastTradePrice = nft.LatestTradePrice?.ToString("G29"),
                LastTradeSymbol = nft.LatestTradeSymbol,
                Public = 1,
                MetaData = JsonConvert.SerializeObject(nft),
                Chain = chain,
                Created = (sbyte) (isCreated ? 1 : 0),
                TokenAddress = nft.ContractTokenId,
                TokenId = nft.TokenId,
                FloorPrice = floorPrice?.ToString("G29"),
                ContractAddress = nft.ContractAddress,
                Description = desc,
            };

            if (!string.IsNullOrEmpty(floorPrice.ToString()))
            {
                if (Constant._chainsToValue.TryGetValue(chain, out decimal exchangeValue))
                    PortfolioValue += decimal.Parse(floorPrice.ToString()!, NumberStyles.Float, CultureInfo.InvariantCulture) * exchangeValue;

                PortfolioValues.Add(new PortfolioValueRecord { Value = floorPrice.Value, Symbol = chain });

                NftValue.Add(floorPrice.Value);
            }

            if (nft.ContractAddress != null && nft.ContractAddress.ToLower() == Constant._founderNft && nft.TokenId == Constant._founderNftTokenId)
                FounderNft += 1;

            if (nft.ContractAddress != null)
                if (Constant._blueChipNfts.TryGetValue(nft.ContractAddress.ToLower(), out string _))
                {
                    var bluechip = await _context.BlueChips.FirstOrDefaultAsync(_ => _.ContractAddress == nft.ContractAddress);
                    if (bluechip != null && wallet != null)
                    {
                        UserBlueChips.Add(new UserBlueChip { BluechipId = bluechip.Id, UserId = userId.ToByteArray(), UserWalletId = wallet.Id });
                        BlueChip += 1;
                    }

                }

           
            await _context.UserNftData.AddAsync(usernft);
            await _context.SaveChangesAsync();

            Nfts.Add(usernft);
        }

        private async Task SaveDataAsync(Guid userId, string chain, string address, byte[] walletId)
        {
            var transaction = _context.Database.CurrentTransaction;

            if (transaction == null)
                transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var worth = PortfolioValue;

                //await context.UserNftCollectionData.AddRangeAsync(Collections);


                //if (Nfts != null && Nfts.Count > 0) await context.BulkInsertOrUpdateAsync<UserNftDatum>(Nfts, new BulkConfig
                //{
                //    SetOutputIdentity = true, // Only insert new records; ignore updates
                //});

                if (Wallet != null)
                {
                    if (decimal.TryParse(Wallet.NftsValue, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal value))
                        Wallet.NftsValue += (value + worth).ToString();

                    Wallet.NftCount = Wallet.NftCount != null ? NftCount + Wallet.NftCount.Value : 0;
                    Wallet.Migrated = 1;
                    Wallet.Blockchain = Constant.Evm;
                }

                await _context.UserNftRecords.AddAsync(new UserNftRecord { NftCount = NftCount, UserId = userId.ToByteArray(), Address = address, Chain = chain });

                if (UserBlueChips != null && UserBlueChips.Count > 0) await _context.UserBlueChips.AddRangeAsync(UserBlueChips);

                var stats = await _context.UserStats.FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());
                if (stats != null)
                {
                    stats.Networth += worth;
                    stats.TotalNft += NftCount;
                    stats.NftCollections += Collections;
                    stats.FounderNft += FounderNft;
                    stats.NftCreated += NftCreated;
                    stats.BluechipCount += UserBlueChips != null ? UserBlueChips.Count : 0;

                    worth = stats.Networth;

                    await _context.UserPortfolioRecords.AddAsync(new UserPortfolioRecord
                    {
                        UserId = userId.ToByteArray(),
                        Value = PortfolioNativeValue.ToString("F8"),
                        UsdValue = worth,
                        Address = address,
                        Chain = chain,
                        MultiValue = JsonConvert.SerializeObject(PortfolioValues)
                    });


                    if (stats.TotalNft >= 1)
                    {

                        //if(Wallet != null && !Wallet.Chain.Equals(chain, StringComparison.OrdinalIgnoreCase))
                        //{
                        //    var id = Guid.NewGuid();
                        //    var wallet = new UserWallet
                        //    {
                        //        Id = id.ToByteArray(),
                        //        UserId = userId.ToByteArray(),
                        //        WalletAddress = address,
                        //        WalletId = Wallet != null ? Wallet.WalletId : null,
                        //        //MetaData = walletAcct.Metadata,
                        //        Chain = chain
                        //    };

                        //    await context.UserWallets.AddAsync(wallet);
                        //}

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

                    var topNft = Nfts.OrderByDescending(_ =>
                    {
                        if (double.TryParse(_.FloorPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out double numericValue))
                            return numericValue;
                        return double.MinValue; // Treat invalid numbers as very large for sorting
                    }).FirstOrDefault();

                    if (topNft != null && !string.IsNullOrEmpty(topNft.FloorPrice))
                    {
                        decimal.TryParse(topNft.FloorPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal nftWorth);

                        //
                        var userTopNft = await _context.UserHighestNfts.FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());
                        var tradSym = !string.IsNullOrEmpty(topNft.LastTradeSymbol) ? topNft.LastTradeSymbol.Trim() : chain;

                        if (userTopNft != null)
                        {
                            var newWorth = Math.Round(nftWorth * Constant._chainsToValue[tradSym.Trim().ToLower()], 2);

                            var oldWorth = Math.Round((decimal)(userTopNft.Worth * Constant._chainsToValue[userTopNft.TradeSymbol ?? userTopNft.Chain!])!, 2);

                            userTopNft.Worth = newWorth > oldWorth ? nftWorth : userTopNft.Worth;
                            userTopNft.WalletId = walletId;
                            userTopNft.Chain = newWorth > oldWorth ? chain : userTopNft.Chain;
                            userTopNft.UserId = userId.ToByteArray();
                            userTopNft.Name = newWorth > oldWorth ? topNft.Name : userTopNft.Name;
                            userTopNft.ImageUrl = newWorth > oldWorth ? topNft.ImageUrl : userTopNft.ImageUrl;
                            userTopNft.TradeSymbol = newWorth > oldWorth ? topNft.LastTradeSymbol : userTopNft.TradeSymbol;
                            userTopNft.Usd = newWorth > oldWorth ? Math.Round(newWorth, 2) : Math.Round(oldWorth, 2);
                            userTopNft.NftId = newWorth > oldWorth ? topNft.Id : userTopNft.NftId;

                            TopValuedNft = userTopNft.Usd.Value > TopValuedNft ? userTopNft.Usd.Value : TopValuedNft;
                        }
                        else
                        {
                            var userHighNft = new UserHighestNft
                            {
                                Worth = nftWorth,
                                WalletId = walletId,
                                Chain = chain,
                                UserId = userId.ToByteArray(),
                                Name = topNft.Name,
                                ImageUrl = topNft.ImageUrl,
                                TradeSymbol = topNft.LastTradeSymbol,
                                Usd = Math.Round(nftWorth * Constant._chainsToValue[tradSym], 2),
                                NftId = topNft.Id
                            };

                            await _context.UserHighestNfts.AddAsync(userHighNft);

                            TopValuedNft = userHighNft.Usd.Value > TopValuedNft ? userHighNft.Usd.Value : TopValuedNft;
                        }

                        //update wallet values based on chain
                        var walletVal = await _context.UserWalletValues
                            .FirstOrDefaultAsync(_ => _.UserWalletId == walletId && _.UserId == userId.ToByteArray() && _.Chain == chain);

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
                                Chain = chain,
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

                if(client.BaseAddress == null) { return; }

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

            if (client.BaseAddress == null) { return; }

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

            if (client.BaseAddress == null) { return null; }

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

        private async Task HandleUserNewEarnedBadge(Guid userId)
        {           
            await SaveNotifications(userId);
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
