using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.Archway;
using Ovation.Domain.Entities;
using Quartz;
using System.Globalization;
using System.Net.Http.Json;
using System.Text;

namespace Ovation.Persistence.Services.Clients
{
    class ArchwayService(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseClientService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(ArchwayService);

        public List<NotificationDto> Notifications { get; set; } = new();

        public List<UserNftDatum> Nfts { get; set; } = new();
        public string Chain { get; set; } = Constant.Archway;
        public int NftCreated { get; set; } = 0;
        public int Collections { get; set; } = 0;
        public decimal PortfolioValue { get; set; } = 0M;

        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            _sentryService.AddBreadcrumb("Get Archway NFTs initiated");

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

                    if (wallet == null) return;

                    var client = _factory.CreateClient(Constant.Archway);

                    var archwayCollections = await _context.ArchwayCollections.ToListAsync();

                    _sentryService.AddBreadcrumb("Fetching NFTs...");
                    foreach (var collection in archwayCollections)
                    {
                        var response = await client
                        .GetFromJsonAsync<ArchwayTokenData?>
                        ($"cosmwasm/wasm/v1/contract/{collection.ContractAddress}/smart/{ConvertTokenQueryToBase64(address)}");


                        if (response != null && response.Data != null && response.Data.Tokens != null && response.Data.Tokens.Count > 0)
                        {                                                        
                            var collectionId = await AddCollectionAsync(collection, wallet!.Id, userId, response.Data.Tokens.Count);
                            Collections += 1;

                            _sentryService.AddBreadcrumb("Adding NFTs and collections to database started");
                            foreach (var token in response.Data.Tokens)
                            {
                                var nft = await client
                                    .GetFromJsonAsync<ArchwayNftData?>
                                    ($"cosmwasm/wasm/v1/contract/{collection.ContractAddress}/smart/{ConvertNftInfoQueryToBase64(int.Parse(token))}");

                                if (nft != null && nft.Data.Info.Extension != null)
                                {
                                    address = nft.Data.Access.Owner;
                                    var userNft = new UserNftDatum
                                    {
                                        Description = nft.Data.Info.Extension.Description,
                                        ImageUrl = nft.Data.Info.Extension.Image,
                                        MetaData = JsonConvert.SerializeObject(nft),
                                        Name = nft.Data.Info.Extension.Name,
                                        Chain = Constant.Archway,
                                        UserId = userId.ToByteArray(),
                                        UserWalletId = wallet != null ? wallet.Id : null,
                                        TokenId = $"{token}",
                                        CollectionId = collectionId,
                                        FloorPrice = collection.FloorPrice.ToString(),
                                        ContractAddress = collection.ContractAddress
                                    };
                                    await _context.UserNftData.AddAsync(userNft);
                                    await _context.SaveChangesAsync();

                                    Nfts.Add(userNft);
                                    PortfolioValue += collection.FloorPrice;


                                }
                            }
                            _sentryService.AddBreadcrumb("Adding NFTs and collections to database completed");
                        }
                    }

                    var nftCount = Nfts.Count;
                    wallet.NftCount += nftCount;
                    wallet.Migrated = 1;
                    wallet.Blockchain = Constant.Archway;
                    wallet.NftsValue = (PortfolioValue * Constant._chainsToValueFloor[Constant.Archway]).ToString();

                    _sentryService.AddBreadcrumb("Updating user stats started");
                    await UpdateUserStatAsync(nftCount, userId, address, wallet.Id);

                    _sentryService.AddBreadcrumb("Updating user stats completed");

                    await SaveNotifications(userId);

                    Collections = 0;
                    NftCreated = 0;
                    Nfts.Clear();
                    Notifications.Clear();
                    PortfolioValue = 0.00M;
                }
                catch (Exception _)
                {
                    SentrySdk.CaptureException(_);
                }
            }
        }

        private async Task<ArchwayBalanceData?> GetBalanceAsync(string address)
        {
            try
            {
                var client = _factory.CreateClient(Constant.Archway);
                var balance = await client
                .GetFromJsonAsync<ArchwayBalanceData?>($"cosmos/bank/v1beta1/spendable_balances/{address}/by_denom?denom=aarch");

                return balance;
            }
            catch (Exception _)
            {
                return null;
            }
        }

        private async Task SaveNftDataAsync(List<UserNft> nfts, Guid userId, UserWallet? wallet)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {


                var col = new UserNftCollection
                {
                    //CollectionTotal = (nfts != null) ? nfts.Count : 0,
                    UserId = userId.ToByteArray(),
                    //Collections = JsonConvert.SerializeObject(nfts),
                    UserWalletId = wallet != null ? wallet.Id : null,
                };

                await _context.UserNftCollections.AddAsync(col);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception _)
            {
                await transaction.RollbackAsync();
            }
        }

        private async Task UpdateUserStatAsync(int nftCount, Guid userId, string address, byte[] walletId)
        {
            var transaction = _context.Database.CurrentTransaction;

            if (transaction == null)
                transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var stats = await _context.UserStats.FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());
                if (stats != null)
                {
                    var worth = PortfolioValue * Constant._chainsToValueFloor[Chain];

                    stats.Networth += worth;
                    stats.TotalNft += nftCount;
                    stats.NftCollections += Collections;
                    stats.NftCreated += NftCreated;

                    await _context.UserNftRecords.AddAsync(new UserNftRecord { NftCount = nftCount, UserId = userId.ToByteArray(), Address = address, Chain = Chain });

                    await _context.UserPortfolioRecords.AddAsync(new UserPortfolioRecord { UserId = userId.ToByteArray(),
                        Value = PortfolioValue.ToString("F8"), Address = address, Chain = Chain, UsdValue = worth });

                    var topNft = Nfts.OrderByDescending(_ =>
                    {
                        if (double.TryParse(_.FloorPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out double numericValue))
                            return numericValue;
                        return double.MinValue; // Treat invalid numbers as very large for sorting
                    }).FirstOrDefault();

                    if (topNft != null && !string.IsNullOrEmpty(topNft.FloorPrice))
                    {
                        decimal.TryParse(topNft.FloorPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal nftWorth);

                        var userTopNft = await _context.UserHighestNfts.FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

                        var tradSym = !string.IsNullOrEmpty(topNft.LastTradeSymbol) ? topNft.LastTradeSymbol.Trim() : Chain;

                        var topValue = 0.00M;

                        if (userTopNft != null)
                        {
                            var newWorth = Math.Round(nftWorth * Constant._chainsToValueFloor[tradSym], 2);

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
                                Usd = Math.Round(nftWorth * Constant._chainsToValueFloor[tradSym], 2),
                                NftId = topNft.Id
                            };

                            await _context.UserHighestNfts.AddAsync(userHighNft);

                            topValue = userHighNft.Usd.Value;
                        }

                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
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

        private string ConvertNftInfoQueryToBase64(int token)
        {
            var jsonObject = new
            {
                all_nft_info = new
                {
                    token_id = $"{token}"
                }
            };

            string jsonString = JsonConvert.SerializeObject(jsonObject);

            // Convert JSON string to Base64
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
        }

        private string ConvertTokenQueryToBase64(string address)
        {
            var jsonObj = new
            {
                tokens = new
                {
                    owner = $"{address}",
                    limit = 4000,
                    start_after = "1"
                }
            };

            string jsonString = JsonConvert.SerializeObject(jsonObj);

            // Convert JSON string to Base64
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
        }

        private async Task<long> AddCollectionAsync(ArchwayCollection coll, byte[] walletId, Guid userId, int owned)
        {
            var collection = new UserNftCollectionDatum
            {
                CollectionId = Guid.NewGuid().ToByteArray(),
                ContractName = coll.CollectionName,
                ContractAddress = coll.ContractAddress,
                Description = "",
                ItemTotal = coll.Supply,
                OwnsTotal = owned,
                FloorPrice = coll.FloorPrice.ToString(),
                LogoUrl = coll.Image,
                UserWalletId = walletId,
                Chain = "archway",
                UserId = userId.ToByteArray(),
            };
            await _context.UserNftCollectionData.AddAsync(collection);
            await _context.SaveChangesAsync();

            return collection.Id;
        }

        protected override Task GetUserNftTransactionsAsync(string address, Guid userId, string? chain = null)
        {
            throw new NotImplementedException();
        }

        internal override async Task<int> GetNftCountAsync(string address, string? chain = null)
        {
            var client = _factory.CreateClient(Constant.Archway);

            var archwayCollections = await _context.ArchwayCollections.ToListAsync();

            var nftCount = 0;

            _sentryService.AddBreadcrumb("Fetching NFTs...");
            foreach (var collection in archwayCollections)
            {
                var response = await client
                .GetFromJsonAsync<ArchwayTokenData?>
                ($"cosmwasm/wasm/v1/contract/{collection.ContractAddress}/smart/{ConvertTokenQueryToBase64(address)}");


                if (response != null && response.Data != null && response.Data.Tokens != null && response.Data.Tokens.Count > 0)
                {
                    nftCount += response.Data.Tokens.Count;  
                }
            }

            return nftCount;
        }
    }
}
