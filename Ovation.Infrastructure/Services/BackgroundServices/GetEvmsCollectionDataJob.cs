using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ovation.Application.DTOs.NFTScan.ENV;
using Ovation.Domain.Entities;
using Quartz;
using System.Net.Http.Json;

namespace Ovation.Persistence.Services.BackgroundServices
{
    internal sealed class GetEvmsCollectionDataJob(IServiceScopeFactory serviceScopeFactory,
        ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseJobService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(GetEvmsCollectionDataJob);
        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            try
            {
                if (jobData != null)
                {
                    var contractAddress = jobData.GetString("ContractAddress") ?? string.Empty;
                    var chain = jobData.GetString("Chain") ?? string.Empty;

                    if (string.IsNullOrEmpty(contractAddress) || string.IsNullOrEmpty(chain))
                    {
                        return;
                    }

                    var entity = await _context.NftCollectionsData
                        .AsNoTracking()
                        .Where(context => context.ContractAddress == contractAddress && context.Chain == chain)
                        .Select(x => new
                        {
                            x.ContractName,
                            x.Id,
                            x.Chain,
                            ChildCount = x.NftsData.Count,
                            x.ItemTotal
                        })
                        .FirstOrDefaultAsync();

                    long? id = null;
                    bool isEqual = false;

                    if (entity != null)
                    {
                        await _context.UserNftCollectionData
                        .Where(item => item.ContractAddress == contractAddress && item.Chain == chain)
                        .ExecuteUpdateAsync(updates => updates
                        .SetProperty(item => item.ParentCollection, item => entity.Id));

                        id = entity.Id;

                        if (entity.ChildCount >= entity.ItemTotal)
                            isEqual = true;
                    }

                    var client = _factory.CreateClient(chain!);

                    if (client.BaseAddress == null)
                    {
                        return;
                    }

                    var collectionData = await client
                        .GetFromJsonAsync<EvmsCollectionData?>($"api/v2/collections/{contractAddress}?show_attribute=true");

                    if (collectionData != null && collectionData.Data != null)
                    {
                        if (id == null)
                            id = await AddCollectionAsync(collectionData.Data, chain!);

                        if (!isEqual)
                        {
                            var nftData = await client
                            .GetFromJsonAsync<EvmsCollectionNftsData?>($"api/v2/assets/{contractAddress}?show_attribute=true&sort_field=&sort_direction=&limit=100");

                            if (nftData != null && nftData.Data != null && nftData.Data.Content != null)
                            {
                                foreach (var nft in nftData.Data.Content)
                                {
                                    await AddNft(nft, id.Value, chain!);
                                }

                                if (!string.IsNullOrEmpty(nftData.Data.Next))
                                {
                                    await GetNextPageData(nftData.Data.Next, contractAddress!, chain!, id.Value);
                                }
                            }
                        }

                        //await dbContext.UserNftCollectionData
                        //.Where(item => item.ContractAddress == contractAddress && item.Chain == chain)
                        //.ExecuteUpdateAsync(updates => updates
                        //.SetProperty(item => item.ParentCollection, item => id.Value));
                    }

                }
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
            }
        }

        private async Task<long> AddCollectionAsync(CollectionData data, string chain)
        {            
            var collection = new NftCollectionsDatum
            {
                ContractName = data.Name,
                ContractAddress = data.ContractAddress,
                Description = data.Description,
                ItemTotal = (int)data.ItemsTotal,
                LogoUrl = data.LogoUrl,
                FloorPrice = data?.FloorPrice?.ToString(),
                Spam = (sbyte)(data.IsSpam != null && data.IsSpam ? 1 : 0),
                Symbol = data.Symbol,
                Chain = chain,
                Verified = (sbyte)(data.Verified != null && data.Verified ? 1 : 0),
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
                .FirstOrDefaultAsync(_ => _.ContractAddress == nft.ContractAddress
                && _.Type == chain && _.TokenId == nft.TokenId);

            if (entity != null)
                return;

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

            var nftData = new NftsDatum
            {
                CollectionId = collectionId,
                Cnft = 0,
                Name = !string.IsNullOrEmpty(nft.Name) ? nft.Name : nft.ContractName,
                ImageUrl = image,
                MintPrice = nft.MintPrice?.ToString(),
                MinterAddress = nft.Minter,
                LastTradePrice = nft?.LatestTradePrice?.ToString(),
                LastTradeSymbol = nft?.LatestTradeSymbol,
                MetaData = JsonConvert.SerializeObject(nft),
                Type = chain,
                TokenAddress = nft.ContractTokenId,
                TokenId = nft.TokenId,
                ContractAddress = nft.ContractAddress,
                Description = desc,
            };

            await _context.NftsData.AddAsync(nftData);
            await _context.SaveChangesAsync();
        }

        private async Task GetNextPageData(string cursor, string contractAddress, string chain, long id)
        {
            var client = _factory.CreateClient(chain);

            if(client.BaseAddress == null)
            {
                return;
            }

            var nftData = await client
                    .GetFromJsonAsync<EvmsCollectionNftsData?>($"api/v2/assets/{contractAddress}?show_attribute=true&sort_field=&sort_direction=&limit=100&cursor={cursor}");

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
