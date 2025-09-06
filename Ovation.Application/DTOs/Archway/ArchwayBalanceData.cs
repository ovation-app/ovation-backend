using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Archway
{
    public class ArchwayBalanceData
    {
        [JsonPropertyName("balance")]
        public Balance? Balance { get; set; }
    }

    public class Balance
    {
        [JsonPropertyName("denom")]
        public string? Denom { get; set; }

        [JsonPropertyName("amount")]
        public string? Amount { get; set; }
    }

    public class Pagination
    {
        [JsonPropertyName("next_key")]
        public object? NextKey { get; set; }

        [JsonPropertyName("total")]
        public string? Total { get; set; }
    }
}
