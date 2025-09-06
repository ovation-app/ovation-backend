using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.NFTScan.Solana
{
    public class SolanaNftTransactionData
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
        [JsonPropertyName("collection")]
        public string? Collection { get; set; }

        [JsonPropertyName("hash")]
        public object? Hash { get; set; }

        [JsonPropertyName("tx_interact_program")]
        public object? TxInteractProgram { get; set; }

        [JsonPropertyName("block_number")]
        public object? BlockNumber { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("token_address")]
        public string? TokenAddress { get; set; }

        [JsonPropertyName("fee")]
        public object? Fee { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("destination")]
        public string? Destination { get; set; }

        [JsonPropertyName("trade_value")]
        public object? TradeValue { get; set; }

        [JsonPropertyName("trade_price")]
        public decimal TradePrice { get; set; }

        [JsonPropertyName("trade_symbol")]
        public string? TradeSymbol { get; set; }

        [JsonPropertyName("trade_symbol_address")]
        public string? TradeSymbolAddress { get; set; }

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
