using System.Reflection;
using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Tezos
{
    public class TezosNftPriceData
    {
        [JsonPropertyName("data")]
        public Data? Data { get; set; }
    }

    public class Tokenn
    {
        [JsonPropertyName("listings")]
        public List<Listing>? Listings { get; set; }
    }

    public class Data
    {
        [JsonPropertyName("token")]
        public List<Tokenn>? Token { get; set; }
    }

    public class Listing
    {
        [JsonPropertyName("price_xtz")]
        public int Price { get; set; }
    }
}
