using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs
{
    public class NftTransactionDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("contractName")]
        public string? ContractName { get; set; }

        [JsonPropertyName("eventType")]
        public string? EventType { get; set; }

        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("to")]
        public string? To { get; set; }

        [JsonPropertyName("quantity")]
        public string? Quantity { get; set; }

        [JsonPropertyName("price")]
        public string? Price { get; set; }

        [JsonPropertyName("tranxDate")]
        public DateTime TranxDate { get; set; }

        [JsonPropertyName("tradeSymbol")]
        public string? TradeSymbol { get; set; }
    }
}
