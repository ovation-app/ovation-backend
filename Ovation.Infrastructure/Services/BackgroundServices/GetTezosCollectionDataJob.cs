using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ovation.Application.Constants;
using Ovation.Application.DTOs.Tezos;
using Ovation.Domain.Entities;
using Quartz;
using System.Net.Http.Json;

namespace Ovation.Persistence.Services.BackgroundServices
{
    internal class GetTezosCollectionDataJob(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseJobService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(GetTezosCollectionDataJob);
        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            if (jobData != null)
            {
                var contractAddress = jobData.GetString("ContractAddress");
                var chain = jobData.GetString("Chain");                

                var id = await AddCollectionObjktAsync(contractAddress, chain!);

                if (id != null)
                {
                    var client = _factory.CreateClient(chain!);

                    var response = await client
                        .GetFromJsonAsync<List<TezosCollectionNftsData>>($"v1/tokens?contract={contractAddress}&limit=10000");

                    if (response != null && response.Count > 0)
                    {
                        //foreach (var nft in response)
                        //{
                        //    await AddNft(nft, chain, contractAddress, id);
                        //}
                        var cursor = response?.LastOrDefault()?.Id;

                        if (cursor != null)
                        {
                            await GetNextPageData(cursor, contractAddress!, chain!, id);
                        }

                        await _context.UserNftCollectionData
                        .Where(item => item.ContractAddress == contractAddress && item.Chain == chain)
                        .ExecuteUpdateAsync(updates => updates
                        .SetProperty(item => item.ParentCollection, item => id));
                    }
                }
            }
        }

        private async Task<long?> AddCollectionObjktAsync(string contractAddress, string chain)
        {
            var entity = await _context.NftCollectionsData.AsNoTracking()
                        .FirstOrDefaultAsync(context => context.ContractAddress == contractAddress && context.Chain == chain);

            if (entity != null)
            {
                await _context.UserNftCollectionData
                .Where(item => item.ContractAddress == contractAddress && item.Chain == chain)
                .ExecuteUpdateAsync(updates => updates
                .SetProperty(item => item.ParentCollection, item => entity.Id));

                return entity.Id;
            }

            var clientt = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://data.objkt.com/v3/graphql");
            var content = new StringContent("query { fa(where: {contract: {_eq: \"" + contractAddress + "\"}}) { name description floor_price items logo short_name } }", null, "application/json");
            request.Content = content;

            try
            {
                HttpResponseMessage response = await clientt.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<TezosObjktCollectionData?>(jsonResponse);
                    var coll = data?.Data?.Fa?.FirstOrDefault();                    

                    if (coll != null)
                    {
                        var collection = new NftCollectionsDatum
                        {
                            ContractName = coll.Name,
                            ContractAddress = contractAddress,
                            Description = coll.Description,
                            ItemTotal = coll.Items,
                            LogoUrl = coll.Logo,
                            FloorPrice = (coll.FloorPrice / Constant.Microtez).ToString(),
                            MetaData = JsonConvert.SerializeObject(coll),
                            Symbol = coll.ShortName,
                            Chain = chain,
                        };
                        await _context.NftCollectionsData.AddAsync(collection);
                        await _context.SaveChangesAsync();

                        return collection.Id;
                    }
                    else
                        return await AddCollectionAsync(contractAddress, chain);
                }
                else
                {
                    return await AddCollectionAsync(contractAddress, chain);
                }
            }
            catch (Exception _)
            {
                return null;
            }
        }

        private async Task<long?> AddCollectionAsync(string contractAddress, string chain)
        {
            //var entity = await dbContext.NftCollectionsData
            //            .FirstOrDefaultAsync(context => context.ContractAddress == contractAddress && context.Chain == chain);

            //if (entity != null)
            //{
            //    await dbContext.UserNftCollectionData
            //    .Where(item => item.ContractAddress == contractAddress && item.Chain == chain)
            //    .ExecuteUpdateAsync(updates => updates
            //    .SetProperty(item => item.ParentCollection, item => entity.Id));

            //    return entity.Id;
            //}

            try
            {
                var client = _factory.CreateClient(chain!);

                var response = await client
                    .GetFromJsonAsync<TezosCollectionData>($"v1/accounts/{contractAddress}?legacy=false");

                if (response != null)
                {                    
                    var collection = new NftCollectionsDatum
                    {
                        ContractName = response?.Metadata?.Name,
                        ContractAddress = contractAddress,
                        Description = response?.Metadata?.Description,
                        ItemTotal = response?.TokensCount,
                        LogoUrl = response?.Metadata?.ImageUri,
                        FloorPrice = null,
                        MetaData = JsonConvert.SerializeObject(response),
                        Symbol = null,
                        Chain = chain,
                    };
                    await _context.NftCollectionsData.AddAsync(collection);
                    await _context.SaveChangesAsync();

                    return collection.Id;
                }
                return null;
            }
            catch (Exception _)
            {
                return null;
            }
        }

        private async Task AddNft(TezosCollectionNftsData nft, string chain, string contractAddress, long? collectionId)
        {
            var entity = await _context.NftsData
                .AsNoTracking()
                .FirstOrDefaultAsync(_ => _.ContractAddress == contractAddress
                && _.Type == chain && _.TokenId == nft.TokenId);

            if (entity != null) return;

            var nftData = new NftsDatum
            {
                Description = nft?.Metadata?.Description,
                CollectionId = collectionId,
                ImageUrl = !string.IsNullOrEmpty(nft?.Metadata?.Image) ? nft.Metadata.Image :
                !string.IsNullOrEmpty(nft?.Metadata?.DisplayUri) ? nft.Metadata.DisplayUri : nft?.Metadata?.ThumbnailUri,
                ContractAddress = contractAddress,
                MetaData = JsonConvert.SerializeObject(nft),
                Name = nft?.Metadata?.Name,
                Type = chain,
                TokenId = nft?.TokenId,
                MinterAddress = nft?.Metadata?.Minter,
                Cnft = 0,
            };


            await _context.NftsData.AddAsync(nftData);
            await _context.SaveChangesAsync();
        }

        private async Task GetNextPageData(long? cursor, string contractAddress, string chain, long? id)
        {
            var client = _factory.CreateClient(chain);

            var response = await client
                        .GetFromJsonAsync<List<TezosCollectionNftsData>>($"v1/tokens?contract={contractAddress}&limit=10000&offset.cr={cursor}");

            if (response != null && response.Count > 0)
            {
                foreach (var nft in response)
                {
                    await AddNft(nft, chain, contractAddress, id);
                }

                cursor = response?.LastOrDefault()?.Id;

                if (cursor != null)
                {
                    await GetNextPageData(cursor, contractAddress!, chain!, id);
                }
            }
        }
    }
}
