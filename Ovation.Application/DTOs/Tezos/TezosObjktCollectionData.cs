using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Tezos
{
    public class TezosObjktCollectionData
    {
        [JsonPropertyName("data")]
        public Datta? Data { get; set; }
    }

    public class Datta
    {
        [JsonPropertyName("fa")]
        public List<Fa>? Fa { get; set; }
    }

    public class Fa
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("floor_price")]
        public int FloorPrice { get; set; }

        [JsonPropertyName("items")]
        public int Items { get; set; }

        [JsonPropertyName("logo")]
        public string Logo { get; set; } = string.Empty;

        [JsonPropertyName("short_name")]
        public string ShortName { get; set; } = string.Empty;
    }
}
