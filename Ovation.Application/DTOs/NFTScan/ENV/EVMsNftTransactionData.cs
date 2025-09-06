using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.NFTScan.ENV
{
    public class EVMsNftTransactionData
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

        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("to")]
        public string? To { get; set; }

        [JsonPropertyName("block_number")]
        public object? BlockNumber { get; set; }

        [JsonPropertyName("block_hash")]
        public object? BlockHash { get; set; }

        [JsonPropertyName("gas_price")]
        public string? GasPrice { get; set; }

        [JsonPropertyName("gas_used")]
        public string? GasUsed { get; set; }

        [JsonPropertyName("gas_fee")]
        public object? GasFee { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("contract_address")]
        public string? ContractAddress { get; set; }

        [JsonPropertyName("contract_name")]
        public string? ContractName { get; set; }

        [JsonPropertyName("contract_token_id")]
        public string? ContractTokenId { get; set; }

        [JsonPropertyName("token_id")]
        public string? TokenId { get; set; }

        [JsonPropertyName("erc_type")]
        public string? ErcType { get; set; }

        [JsonPropertyName("send")]
        public string? Send { get; set; }

        [JsonPropertyName("receive")]
        public string? Receive { get; set; }

        [JsonPropertyName("amount")]
        public string? Amount { get; set; }

        [JsonPropertyName("trade_value")]
        public object? TradeValue { get; set; }

        [JsonPropertyName("trade_price")]
        public decimal TradePrice { get; set; }

        [JsonPropertyName("trade_symbol")]
        public string? TradeSymbol { get; set; }

        [JsonPropertyName("trade_symbol_address")]
        public string? TradeSymbolAddress { get; set; }

        [JsonPropertyName("event_type")]
        public string EventType { get; set; } = string.Empty;

        [JsonPropertyName("exchange_name")]
        public string? ExchangeName { get; set; }

        [JsonPropertyName("aggregate_exchange_name")]
        public string? AggregateExchangeName { get; set; }

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
