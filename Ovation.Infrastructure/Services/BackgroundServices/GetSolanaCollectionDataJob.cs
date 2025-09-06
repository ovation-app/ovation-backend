using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ovation.Application.DTOs.NFTScan.Solana;
using Ovation.Domain.Entities;
using Quartz;
using System.Net.Http.Json;

namespace Ovation.Persistence.Services.BackgroundServices
{
    internal class GetSolanaCollectionDataJob(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseJobService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(GetSolanaCollectionDataJob);

        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            if (jobData != null)
            {
                var collection = jobData.GetString("Collection");
                var chain = jobData.GetString("Chain");


                var entity = await _context.NftCollectionsData
                .AsNoTracking()
                .Where(_ => _.ContractName == collection
                && _.Chain == chain)
                .Select(x => new
                {
                    x.ContractName, x.Id, x.Chain, ChildCount = x.NftsData.Count, x.ItemTotal
                })
                .FirstOrDefaultAsync();

                long? id = null;
                bool isEqual = false;

                if (entity != null)
                {
                    await _context.UserNftCollectionData
                    .Where(item => item.ContractName == collection && item.Chain == chain)
                    .ExecuteUpdateAsync(updates => updates
                    .SetProperty(item => item.ParentCollection, item => entity.Id));

                    id = entity.Id;

                    if (entity.ChildCount >= entity.ItemTotal)
                        isEqual = true;
                }

                var client = factory.CreateClient(chain!);

                var collectionData = await client
                    .GetFromJsonAsync<SolanaCollectionData?>($"api/sol/collections/{collection}");

                if (collectionData != null && collectionData.Data != null)
                {
                    if(id == null)
                        id = await AddCollectionAsync(collectionData.Data, chain!);

                    //await dbContext.UserNftCollectionData
                    //.Where(item => item.ContractName == collection && item.Chain == chain)
                    //.ExecuteUpdateAsync(updates => updates
                    //.SetProperty(item => item.ParentCollection, item => id));

                    if (!isEqual)
                    {
                        var nftData = await client
                        .GetFromJsonAsync<SolanaCollectionNftsData?>($"api/sol/assets/collection/{collection}?show_attribute=true&limit=100");

                        if (nftData != null && nftData.Data != null && nftData.Data.Content != null)
                        {
                            foreach (var nft in nftData.Data.Content)
                            {
                                await AddNft(nft, id.Value, chain!);
                            }

                            if (!string.IsNullOrEmpty(nftData.Data.Next))
                            {
                                await GetNextPageData(nftData.Data.Next, collection!, chain!, id.Value);
                            }
                        }
                    }
                    
                }

            }
        }

        private async Task<long> AddCollectionAsync(CollectionData data, string chain)
        {            
            var collection = new NftCollectionsDatum
            {
                ContractName = data.Collection,
                ContractAddress = data.CollectionTokenAddress,
                Description = data.Description,
                ItemTotal = data.ItemsTotal,
                LogoUrl = data.LogoUrl,
                Symbol = data.Symbol,
                Chain = chain,
                Verified = (sbyte)(data.Verified ? 1 : 0),
                MetaData = JsonConvert.SerializeObject(data),
            };

            await _context.NftCollectionsData.AddAsync(collection);
            await _context.SaveChangesAsync();

            return collection.Id;
        }

        private async Task AddNft(DataContent nft, long collectionId, string chain)
        {
            var entity = await _context.NftsData
                .AsNoTracking()
                .FirstOrDefaultAsync(_ => _.Name == nft.Name
                && _.Type == chain);

            if (entity != null) return;

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

            var nftData = new NftsDatum
            {
                CollectionId = collectionId,
                Cnft = 0,
                Name = nft.Name,
                ImageUrl = image,
                MintPrice = nft.MintPrice.ToString("G29"),
                MinterAddress = nft.Minter,
                LastTradePrice = nft?.LatestTradePrice?.ToString(),
                LastTradeSymbol = nft?.LatestTradeSymbol,
                MetaData = JsonConvert.SerializeObject(nft),
                Type = chain,
                TokenAddress = nft?.TokenAddress,
                Description = desc,
            };

            await _context.NftsData.AddAsync(nftData);
            await _context.SaveChangesAsync();
        }

        private async Task GetNextPageData(string cursor, string contractAddress, string chain, long id)
        {
            var client = factory.CreateClient(chain);

            var nftData = await client
                    .GetFromJsonAsync<SolanaCollectionNftsData?>($"api/sol/assets/collection/Okay Bears?show_attribute=true&limit=100&cursor={cursor}");

            if (nftData != null && nftData.Data != null && nftData.Data.Content != null)
            {
                foreach (var nft in nftData.Data.Content)
                {
                    await AddNft(nft, id, chain!);
                }

                if (!string.IsNullOrEmpty(nftData.Data.Next))
                {
                    await GetNextPageData(nftData.Data.Next, contractAddress, chain, id);
                }
            }
        }
    }
}
