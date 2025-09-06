using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Ovation.Domain.Entities;
using Ovation.Persistence.Data;
using Quartz;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Ovation.Application.DTOs.NFTScan.TON;

namespace Ovation.Persistence.Services.BackgroundServices
{
    internal class GetTonCollectionDataJob(IHttpClientFactory factory, OvationDbContext dbContext) : IJob
    {
        internal const string Name = nameof(GetTonCollectionDataJob);
        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            if (jobData != null)
            {
                var contractAddress = jobData.GetString("ContractAddress");
                var chain = jobData.GetString("Chain");
                
                var client = factory.CreateClient(chain!);

                var collectionData = await client
                    .GetFromJsonAsync<TonCollectionData?>($"api/ton/collections/{contractAddress}");

                if (collectionData != null && collectionData.Data != null)
                {
                    var id = await AddCollectionAsync(collectionData.Data, chain!);

                    var nftData = await client
                    .GetFromJsonAsync<TonCollectionNftsData?>($"api/ton/assets/contract/{contractAddress}?show_attribute=true&limit=100");

                    if (nftData != null && nftData.Data != null && nftData.Data.Content != null)
                    {
                        foreach (var nft in nftData.Data.Content)
                        {
                            await AddNft(nft, id, chain!);
                        }

                        if (!string.IsNullOrEmpty(nftData.Data.Next))
                        {
                            await GetNextPageData(nftData.Data.Next, contractAddress!, chain!, id);
                        }
                    }

                    //await dbContext.UserNftCollectionData
                    //.Where(item => item.ContractAddress == contractAddress && item.Chain == chain)
                    //.ExecuteUpdateAsync(updates => updates
                    //.SetProperty(item => item.ParentCollection, item => id));
                }

            }
        }

        private async Task<long> AddCollectionAsync(CollectionData data, string chain)
        {
            var entity = await dbContext.NftCollectionsData
                .FirstOrDefaultAsync(context => context.ContractAddress == data.ContractAddress && context.Chain == chain);

            if (entity != null)
            {
                await dbContext.UserNftCollectionData
                .Where(item => item.ContractAddress == data.ContractAddress && item.Chain == chain)
                .ExecuteUpdateAsync(updates => updates
                .SetProperty(item => item.ParentCollection, item => entity.Id));

                return entity.Id;
            }


            var collection = new NftCollectionsDatum
            {
                ContractName = data.ContractName,
                ContractAddress = data.ContractAddress,
                Description = data.Description,
                ItemTotal = data.ItemsTotal,
                LogoUrl = data.LogoUrl,
                Chain = chain,
                Verified = (sbyte)(data.Verified != null && data.Verified ? 1 : 0),
                MetaData = JsonConvert.SerializeObject(data),
            };

            await dbContext.NftCollectionsData.AddAsync(collection);
            await dbContext.SaveChangesAsync();

            return collection.Id;
        }

        private async Task AddNft(DataContent nft, long collectionId, string chain)
        {
            var entity = await dbContext.NftsData
                .FirstOrDefaultAsync(_ => _.ContractAddress == nft.ContractAddress
                && _.Type == chain && _.TokenId == nft.TokenId);

            if (entity != null) return;

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
                MintPrice = nft.MintPrice,
                MinterAddress = nft.Minter,
                LastTradePrice = nft.LatestTradePrice.ToString(),
                LastTradeSymbol = "ton",
                MetaData = JsonConvert.SerializeObject(nft),
                Type = chain,
                TokenAddress = nft.TokenAddress,
                TokenId = nft.TokenId,
                ContractAddress = nft.ContractAddress,
                Description = desc,
            };

            await dbContext.NftsData.AddAsync(nftData);
            await dbContext.SaveChangesAsync();
        }

        private async Task GetNextPageData(string cursor, string contractAddress, string chain, long id)
        {
            var client = factory.CreateClient(chain);

            var nftData = await client
                    .GetFromJsonAsync<TonCollectionNftsData?>($"api/ton/assets/contract/{contractAddress}?show_attribute=true&limit=100&cursor={cursor}");

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
