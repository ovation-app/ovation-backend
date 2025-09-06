using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ovation.WebAPI.Constants
{
    public class BlockchainPriceUpdater
    {
        // Dictionary to hold blockchain names and their current USD values
        public static readonly Dictionary<string, string> _envChainsToLinks = new Dictionary<string, string>
    {
        { "eth", "" },
        { "polygon", "" },
        { "bsc", "" },
        { "avalanche", "" },
        { "fantom", "" },
        { "cronos", "" },
        { "arbitrum", "" },
        { "base", "" },
        { "optimism", "" },
        { "linea", "" },
        { "moonbeam", "" },
        { "starknet", "" }
        // Add other chains as needed
    };

        public static async Task UpdateBlockchainPricesAsync()
        {
            string apiUrl = "https://api.coingecko.com/api/v3/simple/price?ids=ethereum,polygon,binancecoin,avalanche-2,fantom,cronos,arbitrum,base,optimism,linea,moonbeam,starknet&vs_currencies=usd";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();

                    //string responseBody = await response.Content.ReadAsStringAsync();
                    //JObject prices = JObject.Parse(responseBody);

                    //// Update the dictionary with real-time USD values
                    //_envChainsToLinks["eth"] = prices["ethereum"]["usd"].ToString();
                    //_envChainsToLinks["polygon"] = prices["polygon"]["usd"].ToString();
                    //_envChainsToLinks["bsc"] = prices["binancecoin"]["usd"].ToString();
                    //_envChainsToLinks["avalanche"] = prices["avalanche-2"]["usd"].ToString();
                    //_envChainsToLinks["fantom"] = prices["fantom"]["usd"].ToString();
                    //_envChainsToLinks["cronos"] = prices["cronos"]["usd"].ToString();
                    //_envChainsToLinks["arbitrum"] = prices["arbitrum"]["usd"].ToString();
                    //_envChainsToLinks["base"] = prices["base"]["usd"].ToString();
                    //_envChainsToLinks["optimism"] = prices["optimism"]["usd"].ToString();
                    //_envChainsToLinks["linea"] = prices["linea"]["usd"].ToString();
                    //_envChainsToLinks["moonbeam"] = prices["moonbeam"]["usd"].ToString();
                    //_envChainsToLinks["starknet"] = prices["starknet"]["usd"].ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching prices: {ex.Message}");
                }
            }
        }

        // Example main method to call the update function
        public static async Task Main(string[] args)
        {
            await UpdateBlockchainPricesAsync();

            // Print updated dictionary
            foreach (var chain in _envChainsToLinks)
            {
                Console.WriteLine($"{chain.Key}: ${chain.Value}");
            }
        }
    }

}
