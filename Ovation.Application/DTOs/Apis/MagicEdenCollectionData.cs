using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Apis
{
    public class MagicEdenCollectionData
    {
        [JsonPropertyName("collections")]
        public List<Collection>? Collections { get; set; }
    }

    public class Amount
    {
        [JsonPropertyName("raw")]
        public string? Raw { get; set; }

        [JsonPropertyName("decimal")]
        public double Decimal { get; set; }

        [JsonPropertyName("usd")]
        public double Usd { get; set; }

        [JsonPropertyName("native")]
        public object? Native { get; set; }
    }

    public class Collection
    {
        [JsonPropertyName("floorAsk")]
        public FloorAsk? FloorAsk { get; set; }
    }

    public class FloorAsk
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("sourceDomain")]
        public string? SourceDomain { get; set; }

        [JsonPropertyName("price")]
        public Price? Price { get; set; }
    }

    public class Price
    {
        [JsonPropertyName("amount")]
        public Amount? Amount { get; set; }
    }
}
