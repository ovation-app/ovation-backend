using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Constants;
using Ovation.Application.DTOs.Apis;
using Ovation.Application.DTOs.NFTScan.ENV;
using Ovation.Persistence.Services.Apis;
using Ovation.Persistence.Services.Clients;
using Quartz;
using System.Net.Http.Json;

namespace Ovation.Persistence.Services
{
    class CollectionPriceService(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseClientService(serviceScopeFactory, schedulerFactory, factory)
    {
        public async Task<decimal?> GetFloorPriceAsync(string chain, string? collection = default, string? contract = default)
        {
            List<decimal?> floorPrice = [];

            var mintifyTask = GetFromMintifyAsync(chain, contract!);

            var dappRaderTask = GetFromDappRadarAsync(chain, collection!);

            var alchemyTask = GetFromAlchemyAsync(chain, contract!);

            var nftScanTask = GetFromNftScanAsync(chain, contract!);

            floorPrice.AddRange(await Task.WhenAll<decimal?>(mintifyTask, dappRaderTask, alchemyTask, nftScanTask));

            var data =  GetLowestNonNullValue(floorPrice);

            return data;
        }

        private async Task<decimal?> GetFromDappRadarAsync(string chain, string collection)
        {
            try
            {
                if (chain.Equals("eth", StringComparison.OrdinalIgnoreCase))
                    chain = "ethereum";
                string JsonQuoted(string value) => $"\"{value}\"";

                var data = await _context.DappRadarCollectionData
                .FromSqlInterpolated($@"SELECT * FROM dapp_radar_collection_data WHERE Name = {collection} 
                    and JSON_CONTAINS(Metadata, {JsonQuoted(chain)}, '$.Chains')")
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();

                if (data != null && !string.IsNullOrEmpty(data.FloorPrice))
                {
                    return decimal.Parse(data.FloorPrice) / Constant._chainsToValue[chain];
                }

                return null;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return null;
            }
            
        }

        private async Task<decimal?> GetFromMintifyAsync(string chain, string contract)
        {
            try
            {
                var client = _factory.CreateClient($"{Constant.Mintify}{chain}");

                if (client == null || client.BaseAddress == null) return null;

                var response = await client
                    .GetFromJsonAsync<MintifyGetCollection>($"api/getContract?contract={contract}");

                if (response != null && response.Status == 200)
                {
                    //List<Application.DTOs.Apis.Datum> data = response.Data.OfType<Application.DTOs.Apis.Datum>().ToList();
                    if(decimal.TryParse(response.Data?.FirstOrDefault()?.Floor?.ToString(), out decimal price))
                        return price; 
                }

                return null;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return null; ;
            }
        }

        private async Task<decimal?> GetFromAlchemyAsync(string chain, string contract)
        {
            try
            {
                var client = _factory.CreateClient($"{Constant.Alchemy}{chain}");

                if (client == null || client.BaseAddress == null) return null;

                var response = await client
                    .GetFromJsonAsync<AlchemyCollectionData>($"nft/v3/{Constant.AlchemyKey}/getContractMetadata?contractAddress={contract}");

                if (response != null)
                {
                    if (decimal.TryParse(response?.OpenSeaMetadata?.FloorPrice?.ToString(), out decimal price))
                        return price;
                }

                return null;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return null;
            }
        }

        private async Task<decimal?> GetFromMagicEdenAsync(string chain, string contract)
        {
            try
            {
                var magicEdenApi = _serviceScope.ServiceProvider.GetService<IMagicEden>();

                if (magicEdenApi != null)
                {
                    var ch = chain;
                    if (chain.Equals("eth", StringComparison.OrdinalIgnoreCase))
                        ch = "ethereum";

                    var data = await magicEdenApi.GetCollectionsAsync(ch, contract, 1);

                    if (data != null && data.Collections != null && data.Collections.Count > 0)
                    {
                        var collection = data.Collections.First();

                        if (decimal.TryParse(collection.FloorAsk?.Price?.Amount?.Native?.ToString(), out decimal price))
                            return price;
                    }
                }

                return default;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return null;
            }
        }

        private async Task<decimal?> GetFromNftScanAsync(string chain, string contract)
        {
            try
            {
                var client = _factory.CreateClient($"{chain}");

                if (client == null || client.BaseAddress == null) return null;

                var response = await client
                    .GetFromJsonAsync<EvmsCollectionData>($"api/v2/collections/{contract}?show_attribute=false");

                if (response != null && response.Code == 200)
                {
                    if (decimal.TryParse(response?.Data?.FloorPrice?.ToString(), out decimal price))
                        return price;
                    else if (decimal.TryParse(response?.Data?.OpenseaFloorPrice?.ToString(), out decimal pricee))
                        return pricee;
                }

                return null;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return null;
            }
        }

        private decimal? GetLowestNonNullValue(List<decimal?> decimals)
        {
            if (decimals == null || decimals.All(d => d == null))
            {
                return null; // or throw an exception if preferred
            }

            return decimals
                .Where(d => d.HasValue)
                .Min();
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
