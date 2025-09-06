using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.NFTScan.TON
{
    public class TonNftTransactionData
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("msg")]
        public string? Msg { get; set; }

        [JsonPropertyName("data")]
        public Data? Data { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("hash")]
        public object? Hash { get; set; }

        [JsonPropertyName("block_number")]
        public object? BlockNumber { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("contract_name")]
        public string? ContractName { get; set; }

        [JsonPropertyName("contract_address")]
        public string? ContractAddress { get; set; }

        [JsonPropertyName("token_id")]
        public string? TokenId { get; set; }

        [JsonPropertyName("token_address")]
        public string? TokenAddress { get; set; }

        [JsonPropertyName("fee")]
        public object? Fee { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("destination")]
        public string? Destination { get; set; }

        [JsonPropertyName("trade_price")]
        public string? TradePrice { get; set; }

        [JsonPropertyName("event_type")]
        public string? EventType { get; set; }

        [JsonPropertyName("exchange_name")]
        public string? ExchangeName { get; set; }

        [JsonPropertyName("nftscan_tx_id")]
        public string? NftscanTxId { get; set; }
    }

    public class Data
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("next")]
        public string? Next { get; set; }

        [JsonPropertyName("content")]
        public List<Content>? Content { get; set; }
    }
}
