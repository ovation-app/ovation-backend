using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;
using Ovation.Persistence.Services;
using Ovation.Persistence.Services.Clients;
using Ovation.Persistence.Services.Clients.NFTScan;
using Quartz;
using System.Globalization;

namespace Ovation.Persistence.Repositories
{
    internal class AssetRepository(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory) : BaseRepository<NftCollectionsDatum>(serviceScopeFactory), IAssetRepository
    {
        public async Task<ResponseData> GetCollectionAsync(int collectionId)
        {
            try
            {
                var response = new ResponseData();

                response.Data = await _context.NftCollectionsData
                    .Where(_ => _.Id == collectionId)
                    .Include(i => i.UserNftCollectionData)
                    .ThenInclude(u => u.User)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .Select(x => new
                    {
                        ContractDetails = new
                        {
                            x.ContractName,
                            x.Description,
                            x.LogoUrl,
                            x.ItemTotal,
                            x.Chain,
                            x.ContractAddress,
                            x.FloorPrice,
                            Website = EF.Functions.JsonExtract<string?>(x.MetaData, "$.Website"),
                            TokenStandard = EF.Functions.JsonExtract<string?>(x.MetaData, "$.ErcType"),
                            Royalty = EF.Functions.JsonExtract<string?>(x.MetaData, "$.Royalty.ValueKind")
                        },
                        UserCount = x.UserNftCollectionData.Count,
                        Users = x.UserNftCollectionData.OrderByDescending(d => d.OwnsTotal).Take(50).Distinct().Select(u => new
                        {
                            UserId = new Guid(u.UserId), u.User.Username, u.User.UserProfile.ProfileImage,
                            u.User.UserProfile.DisplayName,
                            u.OwnsTotal, isVerified = u.User.VerifiedUsers.Select(v => v.Type).ToList()
                        }).ToList()
                    }).FirstOrDefaultAsync();

                response.Status = true;
                response.Message = "Data Fetched";
                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetCollectionOwnerDistributionAsync(int collectionId)
        {
            try
            {
                var response = new ResponseData();

                response.Data = await _context.UserNftCollectionData
                    .Where(unc => unc.ParentCollection == collectionId)
                    .GroupBy(unc => unc.ItemTotal) // Group by collection (ItemTotal is same for all users in a collection)
                    .Select(g => new
                    {
                        CollectionId = collectionId,
                        TotalNftsInCollection = g.Key, // ItemTotal (Total NFTs in collection)
                        TotalOwners = g.Count(), // Total number of users owning NFTs in this collection
                        OwnershipDistribution = new
                        {
                            OneItem = g.Count(unc => unc.OwnsTotal == 1) * 100.0 / g.Count(),
                            TwoToTenItems = g.Count(unc => unc.OwnsTotal >= 2 && unc.OwnsTotal <= 10) * 100.0 / g.Count(),
                            ElevenToTwentyItems = g.Count(unc => unc.OwnsTotal >= 11 && unc.OwnsTotal <= 20) * 100.0 / g.Count(),
                            MoreThanTwentyItems = g.Count(unc => unc.OwnsTotal > 20) * 100.0 / g.Count()
                        }
                    })
                    .FirstOrDefaultAsync();

                response.Status = true;
                response.Message = "Data Fetched";
                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetCollectionTokensAsync(int collectionId, TokenQueryParametersDto parameters)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(parameters.Next))
                id = DecodeBase64ToInteger(parameters.Next);
            try
            {
                var response = new ResponseData();
                var data = await _context.NftsData
                    .Where( n  => n.CollectionId == collectionId && n.Id > id)
                    .Include(c => c.UserNftData)
                    .AsSplitQuery()
                    .OrderBy(_ => _.Id)
                    .Take(perPage)
                    .Select(x => new AssetNft
                    {
                        ImageUrl = x.ImageUrl,
                        AnimationUrl = x.AnimationUrl,
                        LastTradePrice = x.Type == Constant.Solana || x.Type == Constant.Tezos ? x.LastTradePrice : x.FloorPrice,
                        Chain = x.Type,
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        TokenAddress = x.TokenAddress,
                        ContractAddress = x.ContractAddress,
                        TokenId = x.TokenId,
                        UserCount = x.UserNftData.Count,
                        Users = x.UserNftData.Select(u => new NftUser
                        {
                            Username = u.User.Username,
                            UserId = new Guid(u.User.UserId),
                            ProfileImage = u.User.UserProfile.ProfileImage,
                        }).Take(3).ToList()
                    }).ToListAsync<AssetNft>();

                if (data != null && data.Count == 30)
                    response.Cursor = EncodeIntegerToBase64(data.Last().Id);

                response.Data = data;
                response.Status = true;
                response.Message = "Data Fetched";
                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetTokenAsync(int tokenId)
        {
            try
            {
                var exchange = Constant._chainsToValue;
                //var exchange2 = Constant._chainsToValueFloor;

                var response = new ResponseData();
                response.Data = await _context.NftsData
                    .Where(n => n.Id == tokenId)
                    .Include(c => c.UserNftData)
                    .AsSplitQuery()
                    .Select(x => new
                    {
                        x.ImageUrl,
                        x.AnimationUrl,
                        LastTradePrice = x.Type == Constant.Solana || x.Type == Constant.Tezos ? x.LastTradePrice : x.FloorPrice,
                        Usd = CalculatePrice(new Trade(x.LastTradeSymbol, x.LastTradePrice, x.Collection.FloorPrice, x.Type)),
                        x.Id,
                        Chain = x.Type,
                        x.Description,
                        x.ContractAddress,
                        x.TokenId,
                        x.TokenAddress,
                        x.Name,
                        x.Collection.ContractName,
                        x.CollectionId,
                        Website = EF.Functions.JsonExtract<string?>(x.Collection.MetaData, "$.Website"),
                        TokenStandard = EF.Functions.JsonExtract<string?>(x.Collection.MetaData, "$.ErcType"),
                        Royalty = EF.Functions.JsonExtract<string?>(x.Collection.MetaData, "$.Royalty.ValueKind"),

                        UserCount = x.UserNftData.Count,
                        Users = x.UserNftData.Distinct().OrderBy(_ => _.Id).Take(10).Select(u => new
                        {
                            u.User.UserProfile.ProfileImage,
                            u.User.UserProfile.DisplayName,
                            u.User.Username
                        })
                    }).FirstOrDefaultAsync();

                response.Status = true;
                response.Message = "Data Fetched";
                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetTokenTransactionActivitiesAsync(int tokenId)
        {
            var nft = await _context.NftsData.Where(_ => _.Id == (long)tokenId)
                .AsNoTracking()
                .Select(x => new
                {
                    x.Type,
                    x.ContractAddress,
                    x.TokenId,
                    x.TokenAddress
                }).FirstOrDefaultAsync();

            if (nft == null) return new ResponseData { StatusCode = 404, Message = "NFT not found"};

            var response = new ResponseData();

            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            switch (nft.Type?.ToLower())
            {
                case Constant.Solana:

                    var solanaService = scope.ServiceProvider.GetRequiredService<SolanaService>();
                    response.Data = await solanaService.GetNftTransactionAsync(nft.TokenAddress, Constant.Solana);
                    break;

                case Constant.Archway:

                    //var archwayService = scope.ServiceProvider.GetRequiredService<ArchwayService>();
                    //await archwayService.GetUserNftsAsync(address, userId);
                    break;
                case Constant.Cosmos:

                    break;
                case Constant.Tezos:

                    var tezosService = scope.ServiceProvider.GetRequiredService<TezosService>();
                    response.Data = await tezosService.GetNftTransactionAsync(nft.ContractAddress, nft.TokenId);
                    break;
                case Constant.Ton:

                    var tonService = scope.ServiceProvider.GetRequiredService<TonService>();
                    response.Data = await tonService.GetNftTransactionAsync(nft.TokenAddress, Constant.Ton);
                    break;

                default:
                    var evmService = scope.ServiceProvider.GetRequiredService<EvmsService>();
                    response.Data = await evmService.GetNftTransactionAsync(nft.ContractAddress, nft.TokenId, nft.Type);

                    break;
            }

            response.Status = true;
            response.Message = "Data Fetched";
            return response;
        }

        public async Task GetUserNfts(Guid userId, string address, string chain, Guid? walletId = null)
        {
            if (string.IsNullOrEmpty(chain)) return;
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IScheduler schedular = await schedulerFactory.GetScheduler();

            switch (chain.ToLower())
            {
                case Constant.Solana:

                    //var solanaService = scope.ServiceProvider.GetRequiredService<SolanaService>();
                    //await solanaService.GetUserNftsAsync(address, userId);

                    schedular = await schedulerFactory.GetScheduler();

                    var solData = new JobDataMap
                    {
                        {"Address", address },
                        {"UserId", userId.ToString()}
                    };


                    ITrigger solTrigger = TriggerBuilder.Create()
                        .WithIdentity($"get-solana-user-nft-data-{address}")
                        .ForJob(SolanaService.Name)
                        .UsingJobData(solData)
                        .StartNow()
                        .Build();

                    await schedular.ScheduleJob(solTrigger);
                    break;

                case Constant.Archway:

                    //var archwayService = scope.ServiceProvider.GetRequiredService<ArchwayService>();
                    //await archwayService.GetUserNftsAsync(address, userId);

                    schedular = await schedulerFactory.GetScheduler();

                    var archData = new JobDataMap
                    {
                        {"Address", address },
                        {"UserId", userId.ToString()}
                    };


                    ITrigger archTrigger = TriggerBuilder.Create()
                        .WithIdentity($"get-archway-user-nft-data-{address}")
                        .ForJob(ArchwayService.Name)
                        .UsingJobData(archData)
                        .StartNow()
                        .Build();

                    await schedular.ScheduleJob(archTrigger);

                    break;
                case Constant.Cosmos:

                    break;
                case Constant.Tezos:

                    //var tezosService = scope.ServiceProvider.GetRequiredService<TezosService>();
                    //await tezosService.GetUserNftsAsync(address, userId);

                    schedular = await schedulerFactory.GetScheduler();

                    var tezosData = new JobDataMap
                    {
                        {"Address", address },
                        {"UserId", userId.ToString()}
                    };


                    ITrigger tezosTrigger = TriggerBuilder.Create()
                        .WithIdentity($"get-tezos-user-nft-data-{address}")
                        .ForJob(TezosService.Name)
                        .UsingJobData(tezosData)
                        .StartNow()
                        .Build();

                    await schedular.ScheduleJob(tezosTrigger);
                    break;
                case Constant.Ton:

                    //var tonService = scope.ServiceProvider.GetRequiredService<TonService>();
                    //await tonService.GetUserNftsAsync(address, userId);
                    //schedular = await schedulerFactory.GetScheduler();

                    //var tonData = new JobDataMap
                    //{
                    //    {"Address", address },
                    //    {"UserId", userId.ToString()}
                    //};


                    //ITrigger tonTrigger = TriggerBuilder.Create()
                    //    .WithIdentity($"get-ton-user-nft-data-{address}")
                    //    .ForJob(TonService.Name)
                    //    .UsingJobData(tonData)
                    //    .StartNow()
                    //    .Build();

                    //await schedular.ScheduleJob(tonTrigger);

                    break;
                case Constant.Stargaze:

                    //var stargazeService = scope.ServiceProvider.GetRequiredService<StargazeService>();

                    //schedular = await schedulerFactory.GetScheduler();

                    //var jobData = new JobDataMap
                    //{
                    //    {"Address", address },
                    //    {"UserId", userId.ToString()}
                    //};


                    //ITrigger trigger = TriggerBuilder.Create()
                    //    .WithIdentity($"get-stargaze-user-nft-data-{address}")
                    //    .ForJob(StargazeService.Name)
                    //    .UsingJobData(jobData)
                    //    .StartNow()
                    //    .Build();

                    //await schedular.ScheduleJob(trigger);
                    //await stargazeService.GetUserNftsAsync(address, userId);
                    break;

                case Constant.Abstract:

                    //var stargazeService = scope.ServiceProvider.GetRequiredService<StargazeService>();

                    schedular = await schedulerFactory.GetScheduler();

                    var jobbData = new JobDataMap
                    {
                        {"Address", address },
                        {"UserId", userId.ToString()}
                    };


                    ITrigger triggerr = TriggerBuilder.Create()
                        .WithIdentity($"get-abstract-user-nft-data-{address}")
                        .ForJob(AbstractService.Name)
                        .UsingJobData(jobbData)
                        .StartNow()
                        .Build();

                    await schedular.ScheduleJob(triggerr);
                    //await stargazeService.GetUserNftsAsync(address, userId);
                    break;

                default:
                    var blockChain = "";

                    if (walletId != null)
                        blockChain = "evm";
                    else
                        blockChain = chain;

                        var jobDataa = new JobDataMap
                    {
                        {"Address", address },
                        {"Chain", blockChain },
                        {"UserId", userId.ToString()}
                    };

                    var evmTtrigger = TriggerBuilder.Create()
                        .WithIdentity($"get-evm-user-nft-data-{address}")
                        .ForJob(EvmsService.Name)
                        .UsingJobData(jobDataa)
                        .StartNow()
                        .Build();

                    await schedular.ScheduleJob(evmTtrigger);

                    break;
            }
        }

        public async Task GetUserTransactions(Guid userId, string address, string chain, Guid? walletId = null)
        {
            if (string.IsNullOrEmpty(chain)) return;
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            switch (chain.ToLower())
            {
                case Constant.Solana:

                    var solanaService = scope.ServiceProvider.GetRequiredService<SolanaService>();
                    //await solanaService.GetUserNftTransactionsAsync(address, chain, userId);
                    break;

                case Constant.Archway:

                    //var archwayService = scope.ServiceProvider.GetRequiredService<ArchwayService>();
                    //await archwayService.GetUserNftsAsync(address, userId);
                    break;
                case Constant.Cosmos:

                    break;

                case Constant.Stargaze:

                    break;

                case Constant.Abstract:

                    break;

                case Constant.Tezos:

                    var tezosService = scope.ServiceProvider.GetRequiredService<TezosService>();
                    //await tezosService.GetUserNftTransactionAsync(address, chain, userId);
                    break;

                case Constant.Ton:

                    //var tonService = scope.ServiceProvider.GetRequiredService<TonService>();
                    //await tonService.GetUserNftTransactionAsync(address, chain, userId);
                    break;

                default:
                    var evmService = scope.ServiceProvider.GetRequiredService<EvmsService>();
                    if (walletId != default)
                    {
                        foreach (var item in Constant._evmChains)
                        {
                            await evmService.GetUserNftTransactionAsync(address, item, userId);
                        }
                    }
                    else
                        await evmService.GetUserNftTransactionAsync(address, chain, userId);

                    break;
            }
        }
    }
}
