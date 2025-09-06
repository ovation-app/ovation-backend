using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ovation.Application.Common.Interfaces;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.NFTScan.ENV;
using Ovation.Domain.Entities;
using Ovation.Persistence.Data;
using Quartz;
using System.Globalization;
using System.Net.Http.Json;

namespace Ovation.Persistence.Services.BackgroundServices
{
    [DisallowConcurrentExecution]
    public class SyncEvmNftDataJob(IHttpClientFactory factory, OvationDbContext context, IHubContext<NotificationService, INotificationService> hubContext) : IJob
    {
        internal const string Name = nameof(SyncEvmNftDataJob);
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
        public string Chain { get; set; } = "eth";

        public async Task Execute(IJobExecutionContext contextt)
        {
            var wallets = await context.UserWallets
                .Where(w => w.Blockchain != Constant.Evm)
                .IgnoreAutoIncludes()
                .AsNoTracking()
                .ToListAsync();

            if (wallets != null && wallets.Count > 0)
            {
                foreach (var wallet in wallets)
                {
                    if (wallet != null)
                    {
                        try
                        {           
                            var client = factory.CreateClient(wallet.Chain!);

                            var collectionData = await client
                            .GetFromJsonAsync<EVMsNFTData?>($"api/v2/account/own/all/{wallet.WalletAddress}?erc_type=&show_attribute=true&sort_field=&sort_direction=");

                            if (collectionData != null && collectionData.Data != null && collectionData.Data.Count > 0)
                            {
                                var allWallets = await context.UserWallets
                                    .Where(_ => _.UserId == wallet.UserId && _.WalletId != null && _.Chain == wallet.Chain)
                                    .ToListAsync();

                                var userCollections = await context.UserNftCollectionData
                                    .Where(c => c.UserId == wallet.UserId && c.UserWalletId == wallet.Id && c.Chain == wallet.Chain).Take(20000).ToListAsync();

                                foreach (var data in collectionData.Data)
                                {
                                    long collectionId;
                                    var res = userCollections
                                        .FirstOrDefault(c => string.Equals(c.ContractName, data.ContractName, StringComparison.OrdinalIgnoreCase)
                                        && string.Equals(c.ContractName, data.ContractAddress));

                                    if (res != null)
                                    {
                                        collectionId = res.Id;
                                        userCollections.Remove(res);
                                    }
                                    else
                                    {
                                        collectionId = await AddCollectionAsync(data, wallet);
                                    }

                                    if (data.Assets != null && data.Assets.Count > 0)
                                    {
                                        var userNfts = await context.UserNftData
                                            .Where(n => n.UserId == wallet.UserId && n.Chain == Chain && n.UserWalletId == wallet.Id).Take(20000).ToListAsync();

                                        foreach (var nft in data.Assets)
                                        {
                                            var idx = userNfts.FindIndex(n => string.Equals(n.TokenId, nft.TokenId, StringComparison.OrdinalIgnoreCase)
                                            && string.Equals(n.TokenAddress, nft.ContractAddress, StringComparison.OrdinalIgnoreCase));

                                            if (idx == -1)
                                            {
                                                await AddNft(nft, collectionId, data.FloorPrice, Chain, wallet, allWallets);
                                                NftCount++;
                                            }
                                            else
                                            {

                                            }
                                        }
                                    }
                                }

                                //await SaveDataAsync(userId, chain, address, Wallet.Id);
                                //await SaveNotifications();

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
                        }
                        catch (Exception _)
                        {

                        }
                    }
                }
            }
        }


        //public async Task UpdateUserNftsAsync(string address, string chain, Guid userId)
        //{
        //    if (userId != Guid.Empty)
        //    {
        //        try
        //        {
        //            if (Constant._evmChainsToLinks.TryGetValue(chain.ToLower(), out string baseUrl))
        //            {
        //                chain = chain.ToLower();
        //                Wallet = null;

        //                Wallet = await context.UserWallets
        //                .OrderByDescending(_ => _.CreatedDate)
        //                .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.WalletAddress == address);

        //                if (Wallet != null)
        //                {
        //                    var client = factory.CreateClient();

        //                    client.DefaultRequestHeaders.Add("X-API-Key", Constant.NFTScanAPIKey);
        //                    client.DefaultRequestHeaders.Add("accept", "application/json");

        //                    client.BaseAddress = new Uri(baseUrl);

        //                    var collectionData = await client
        //                    .GetFromJsonAsync<ENVsNFTData?>($"api/v2/account/own/all/{address}?erc_type=&show_attribute=true&sort_field=&sort_direction=");

        //                    if (collectionData != null && collectionData.Data != null && collectionData.Data.Count > 0)
        //                    {
        //                        var wallets = await context.UserWallets
        //                            .Where(_ => _.UserId == userId.ToByteArray() && _.WalletId != null && _.Chain == chain)
        //                            .ToListAsync();

        //                        foreach (var data in collectionData.Data)
        //                        {
        //                            var collectionId = await AddCollectionAsync(data, chain, Wallet, userId);

        //                            if (data.Assets != null && data.Assets.Count > 0)
        //                            {
        //                                foreach (var nft in data.Assets)
        //                                {
        //                                    await AddNft(nft, collectionId, data.FloorPrice, chain, Wallet, wallets, userId);
        //                                    NftCount++;
        //                                }
        //                            }

        //                        }

        //                        await SaveDataAsync(userId, chain, address, Wallet.Id);
        //                        await SaveNotifications(userId, chain);

        //                        NftCount = 0;
        //                        NftCreated = 0;
        //                        BlueChip = 0;
        //                        FounderNft = 0;
        //                        NftValue.Clear();
        //                        Collections = 0;
        //                        Notifications.Clear();
        //                        Nfts.Clear();
        //                        UserBlueChips.Clear();
        //                        PortfolioNativeValue = 0;
        //                        PortfolioValue = 0;
        //                        PortfolioValues.Clear();
        //                    }
        //                }
        //            }

        //        }
        //        catch (Exception _)
        //        {

        //        }
        //    }
        //}

        private async Task<long> AddCollectionAsync(Datum data, UserWallet wallet)
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
                FloorPrice = data.FloorPrice?.ToString("G29"),
                Spam = (sbyte)(data.IsSpam != null && data.IsSpam.Value ? 1 : 0),
                Symbol = data.Symbol,
                Chain = Chain,
                Verified = (sbyte)(data.Verified != null && data.Verified.Value ? 1 : 0),
                UserWalletId = wallet.Id,
                UserId = wallet.UserId
            };
            //Collections.Add(collection);

            await context.UserNftCollectionData.AddAsync(collection);
            await context.SaveChangesAsync();
            Collections += 1;

            return collection.Id;
        }

        private async Task AddNft(Asset nft, long collectionId, decimal? floorPrice, string chain, UserWallet wallet, List<UserWallet>? wallets)
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


            var usernft = new UserNftDatum
            {
                UserId = wallet.UserId,
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
                Chain = Chain,
                TokenAddress = nft.ContractTokenId,
                TokenId = nft.TokenId,
                FloorPrice = floorPrice?.ToString("G29"),
                Description = desc,
            };

            if (!string.IsNullOrEmpty(nft.LatestTradePrice.ToString()))
            {
                PortfolioNativeValue += nft.LatestTradePrice.GetValueOrDefault(0.00M);
                if (!string.IsNullOrEmpty(nft.LatestTradeSymbol))
                {
                    if (Constant._chainsToValue.TryGetValue(nft.LatestTradeSymbol, out decimal exchangeValue))
                        PortfolioValue += decimal.Parse(nft.LatestTradePrice.ToString()!, NumberStyles.Float, CultureInfo.InvariantCulture) * exchangeValue;
                    PortfolioValues.Add(new PortfolioValueRecord { Value = nft.LatestTradePrice!.Value, Symbol = nft.LatestTradeSymbol });
                }
                else
                {
                    if (Constant._chainsToValue.TryGetValue(chain, out decimal exchangeValue))
                        PortfolioValue += decimal.Parse(nft.LatestTradePrice.ToString()!, NumberStyles.Float, CultureInfo.InvariantCulture) * exchangeValue;

                    PortfolioValues.Add(new PortfolioValueRecord { Value = nft.LatestTradePrice!.Value, Symbol = chain });
                }
                //    PortfolioValue += Math.Round(nft.LatestTradePrice * Constant._evmChainsToValue.TryGetValue(), 2)
                //PortfolioValue += Math.Round(nft.LatestTradePrice * Constant._evmChainsToValue[(string.IsNullOrEmpty(nft.LatestTradeSymbol)) ? nft.LatestTradeSymbol!.Trim().ToLower() : chain!.Trim().ToLower()], 2);
                NftValue.Add(nft.LatestTradePrice.Value);
            }
            //else if (floorPrice != null)
            //{
            //    PortfolioValue += floorPrice.Value;
            //    NftValue.Add(floorPrice.Value);
            //}
            //else if (nft.MintPrice != null)
            //{
            //    PortfolioValue += nft.MintPrice.Value;
            //    NftValue.Add(nft.MintPrice.Value);
            //}

            if (nft.ContractAddress != null && nft.ContractAddress.ToLower() == Constant._founderNft && nft.TokenId == Constant._founderNftTokenId)
                FounderNft += 1;

            if (nft.ContractAddress != null)
                if (Constant._blueChipNfts.TryGetValue(nft.ContractAddress.ToLower(), out string? _))
                {
                    var bluechip = await context.BlueChips.FirstOrDefaultAsync(_ => _.ContractAddress == nft.ContractAddress);
                    if (bluechip != null)
                    {
                        UserBlueChips.Add(new UserBlueChip { BluechipId = bluechip.Id, UserId = wallet.UserId });
                        //BlueChip += 1;
                    }

                }


            if (wallets != null)
            {
                foreach (var item in wallets)
                {
                    if (item.WalletId != null && item.WalletAddress != null)
                        if (item.WalletAddress.Equals(nft.Minter, StringComparison.OrdinalIgnoreCase))
                            NftCreated += 1;
                }

            }
            await context.UserNftData.AddAsync(usernft);
            await context.SaveChangesAsync();
            Nfts.Add(usernft);
        }

        //private async Task SaveDataAsync(Guid userId, string chain, string address, byte[] walletId)
        //{
        //    var transaction = context.Database.CurrentTransaction;

        //    if (transaction == null)
        //        transaction = await context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        var worth = PortfolioValue;

        //        //await context.UserNftCollectionData.AddRangeAsync(Collections);


        //        //if (Nfts != null && Nfts.Count > 0) await context.BulkInsertOrUpdateAsync<UserNftDatum>(Nfts, new BulkConfig
        //        //{
        //        //    SetOutputIdentity = true, // Only insert new records; ignore updates
        //        //});

        //        if (Wallet != null)
        //        {
        //            if (decimal.TryParse(Wallet.NftsValue, out decimal value))
        //                Wallet.NftsValue = (value + worth).ToString();
        //            Wallet.NftCount += NftCount;
        //            Wallet.Migrated = 1;
        //            Wallet.Blockchain = Constant.Evm;
        //        }
        //        await context.UserNftRecords.AddAsync(new UserNftRecord { NftCount = NftCount, UserId = userId.ToByteArray(), Address = address, Chain = chain });
        //        if (UserBlueChips != null && UserBlueChips.Count > 0) await context.UserBlueChips.AddRangeAsync(UserBlueChips);

        //        var stats = await context.UserStats.FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());
        //        if (stats != null)
        //        {
        //            stats.Networth += worth;
        //            stats.TotalNft += NftCount;
        //            stats.NftCollections += Collections;
        //            stats.FounderNft += FounderNft;
        //            stats.NftCreated += NftCreated;
        //            stats.BluechipCount += (UserBlueChips != null) ? UserBlueChips.Count : 0;

        //            worth = stats.Networth;

        //            await context.UserPortfolioRecords.AddAsync(new UserPortfolioRecord
        //            {
        //                UserId = userId.ToByteArray(),
        //                Value = PortfolioNativeValue.ToString("F8"),
        //                UsdValue = worth,
        //                Address = address,
        //                Chain = chain,
        //                MultiValue = JsonConvert.SerializeObject(PortfolioValues)
        //            });

        //            var referenceType = NotificationReference.Badge;

        //            if (stats.TotalNft >= 1)
        //            {
        //                var notification = new NotificationDto
        //                {
        //                    InitiatorId = null,
        //                    ReceiverId = userId,
        //                    Message = Constant._NotificationMessage[referenceType],
        //                    Title = Constant._NotificationTitle[referenceType],
        //                    Reference = NotificationReference.Badge.ToString(),
        //                    ReferenceId = (stats.TotalNft <= 4) ? BadgeStruct.NumberOfNft1 : (stats.TotalNft <= 9) ? BadgeStruct.NumberOfNft5 : (stats.TotalNft <= 24) ? BadgeStruct.NumberOfNft10 :
        //                    (stats.TotalNft <= 49) ? BadgeStruct.NumberOfNft25 : (stats.TotalNft <= 99) ? BadgeStruct.NumberOfNft50 : (stats.TotalNft <= 249) ? BadgeStruct.NumberOfNft100 :
        //                    (stats.TotalNft <= 499) ? BadgeStruct.NumberOfNft250 : BadgeStruct.NumberOfNft500
        //                };
        //                Notifications.Add(notification);
        //            }

        //            if (stats.NftCreated >= 10)
        //            {
        //                var notification = new NotificationDto
        //                {
        //                    InitiatorId = null,
        //                    ReceiverId = userId,
        //                    Message = Constant._NotificationMessage[referenceType],
        //                    Title = Constant._NotificationTitle[referenceType],
        //                    Reference = NotificationReference.Badge.ToString(),
        //                    ReferenceId = (stats.NftCreated <= 49) ? BadgeStruct.Creator10 : (stats.NftCreated <= 99) ?
        //                    BadgeStruct.Creator50 : (stats.NftCreated <= 199) ? BadgeStruct.Creator100 : (stats.NftCreated <= 499) ? BadgeStruct.Creator200 : BadgeStruct.Creator500
        //                };
        //                Notifications.Add(notification);
        //            }

        //            if (stats.BluechipCount >= 1)
        //            {
        //                var notification = new NotificationDto
        //                {
        //                    InitiatorId = null,
        //                    ReceiverId = userId,
        //                    Message = Constant._NotificationMessage[referenceType],
        //                    Title = Constant._NotificationTitle[referenceType],
        //                    Reference = NotificationReference.Badge.ToString(),
        //                    ReferenceId = (stats.BluechipCount <= 2) ? BadgeStruct.BlueChip1 : (stats.BluechipCount <= 4) ? BadgeStruct.BlueChip3 : (stats.BluechipCount <= 9) ?
        //                    BadgeStruct.BlueChip5 : BadgeStruct.BlueChip10
        //                };
        //                Notifications.Add(notification);
        //            }

        //            if (worth >= 1000)
        //            {
        //                var notification = new NotificationDto
        //                {
        //                    InitiatorId = null,
        //                    ReceiverId = userId,
        //                    Message = Constant._NotificationMessage[referenceType],
        //                    Title = Constant._NotificationTitle[referenceType],
        //                    Reference = NotificationReference.Badge.ToString(),
        //                    ReferenceId = (worth <= 4999) ? BadgeStruct.PortfolioValue1k : (worth <= 9999) ?
        //                    BadgeStruct.PortfolioValue5k : (worth <= 24999) ? BadgeStruct.PortfolioValue10k : (worth <= 49999)
        //                    ? BadgeStruct.PortfolioValue25k : (worth <= 99999) ? BadgeStruct.PortfolioValue50k : BadgeStruct.PortfolioValue100k
        //                };
        //                Notifications.Add(notification);
        //            }

        //            var topNft = Nfts.OrderByDescending(_ => {
        //                if (double.TryParse(_.LastTradePrice, NumberStyles.Float, CultureInfo.InvariantCulture, out double numericValue))
        //                    return numericValue;
        //                return double.MinValue; // Treat invalid numbers as very large for sorting
        //            }).FirstOrDefault();

        //            if (topNft != null && !string.IsNullOrEmpty(topNft.LastTradePrice))
        //            {
        //                decimal.TryParse(topNft.LastTradePrice, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal nftWorth);

        //                //
        //                var userTopNft = await context.UserHighestNfts.FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());
        //                var tradSym = (!string.IsNullOrEmpty(topNft.LastTradeSymbol)) ? topNft.LastTradeSymbol.Trim() : chain;

        //                if (userTopNft != null)
        //                {
        //                    var newWorth = Math.Round(nftWorth * Constant._evmChainsToValue[chain.Trim().ToLower()], 2);

        //                    var oldWorth = Math.Round((decimal)(userTopNft.Worth * Constant._evmChainsToValue[tradSym])!, 2);

        //                    userTopNft.Worth = (newWorth > oldWorth) ? nftWorth : userTopNft.Worth;
        //                    userTopNft.WalletId = walletId;
        //                    userTopNft.Chain = chain;
        //                    userTopNft.UserId = userId.ToByteArray();
        //                    userTopNft.Name = topNft.Name;
        //                    userTopNft.ImageUrl = topNft.ImageUrl;
        //                    userTopNft.TradeSymbol = topNft.LastTradeSymbol;
        //                    userTopNft.Usd = (newWorth > oldWorth) ? Math.Round(newWorth, 2) : Math.Round(oldWorth, 2);
        //                }
        //                else
        //                {
        //                    var userHighNft = new UserHighestNft
        //                    {
        //                        Worth = nftWorth,
        //                        WalletId = walletId,
        //                        Chain = chain,
        //                        UserId = userId.ToByteArray(),
        //                        Name = topNft.Name,
        //                        ImageUrl = topNft.ImageUrl,
        //                        TradeSymbol = topNft.LastTradeSymbol,
        //                        Usd = Math.Round(nftWorth * Constant._evmChainsToValue[tradSym], 2)
        //                    };

        //                    await context.UserHighestNfts.AddAsync(userHighNft);
        //                }

        //                //update wallet values based on chain
        //                var walletVal = await context.UserWalletValues
        //                    .FirstOrDefaultAsync(_ => _.UserWalletId == walletId && _.UserId == userId.ToByteArray() && _.Chain == chain);

        //                if (walletVal != null)
        //                {
        //                    walletVal.NftCount = NftCount;
        //                    walletVal.NativeWorth = nftWorth;
        //                }
        //                else
        //                {
        //                    var walletValue = new UserWalletValue
        //                    {
        //                        NativeWorth = nftWorth,
        //                        NftCount = NftCount,
        //                        Chain = chain,
        //                        UserId = userId.ToByteArray(),
        //                        UserWalletId = walletId
        //                    };

        //                    await context.UserWalletValues.AddAsync(walletValue);
        //                }

        //                await context.SaveChangesAsync();

        //                var topValue = nftWorth * Constant._evmChainsToValue[chain];

        //                if (topValue >= 100)
        //                {
        //                    var notification = new NotificationDto
        //                    {
        //                        InitiatorId = null,
        //                        ReceiverId = userId,
        //                        Message = Constant._NotificationMessage[referenceType],
        //                        Title = Constant._NotificationTitle[referenceType],
        //                        Reference = NotificationReference.Badge.ToString(),
        //                        ReferenceId = (topValue <= 499) ? BadgeStruct.TopValueNft100 : (topValue <= 999) ? BadgeStruct.TopValueNft500 : (topValue <= 9999) ? BadgeStruct.TopValueNft1k :
        //                        (topValue <= 24999) ? BadgeStruct.TopValueNft10k : (topValue <= 49999) ? BadgeStruct.TopValueNft25k : (topValue <= 99999) ? BadgeStruct.TopValueNft50k :
        //                        BadgeStruct.TopValueNft100k
        //                    };
        //                    Notifications.Add(notification);
        //                }
        //            }
        //        }

        //        await context.SaveChangesAsync();
        //        await transaction.CommitAsync();
        //    }
        //    catch (Exception _)
        //    {
        //        await transaction.RollbackAsync();
        //    }
        //}

        //private async Task SaveNotifications()
        //{
        //    await HelperFunctions.SaveNotificationAsync(Notifications, _context, hubContext);
        //}
    }
}
