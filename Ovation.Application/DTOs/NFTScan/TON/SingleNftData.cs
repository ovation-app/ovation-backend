using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.NFTScan.TON
{
    public class SingleNftData
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("msg")]
        public object? Msg { get; set; }

        [JsonPropertyName("data")]
        public Datta? Data { get; set; }
    }

    public class Datta
    {
        //[JsonPropertyName("token_address")]
        //public string TokenAddress { get; set; } = string.Empty;

        //[JsonPropertyName("contract_name")]
        //public string ContractName { get; set; } = string.Empty;

        //[JsonPropertyName("contract_address")]
        //public string ContractAddress { get; set; } = string.Empty;

        //[JsonPropertyName("token_id")]
        //public string TokenId { get; set; } = string.Empty;

        //[JsonPropertyName("block_number")]
        //public int BlockNumber { get; set; }

        //[JsonPropertyName("minter")]
        //public string Minter { get; set; } = string.Empty;

        //[JsonPropertyName("owner")]
        //public string Owner { get; set; } = string.Empty;

        //[JsonPropertyName("mint_timestamp")]
        //public object? MintTimestamp { get; set; }

        //[JsonPropertyName("mint_transaction_hash")]
        //public object? MintTransactionHash { get; set; } = string.Empty;

        //[JsonPropertyName("mint_price")]
        //public object? MintPrice { get; set; }

        //[JsonPropertyName("token_uri")]
        //public object? TokenUri { get; set; }

        //[JsonPropertyName("metadata_json")]
        //public string MetadataJson { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string? Name { get; set; } = string.Empty;

        //[JsonPropertyName("content_type")]
        //public string ContentType { get; set; } = string.Empty;

        //[JsonPropertyName("content_uri")]
        //public string ContentUri { get; set; } = string.Empty;

        [JsonPropertyName("image_uri")]
        public string ImageUri { get; set; } = string.Empty;

        //[JsonPropertyName("description")]
        //public string Description { get; set; } = string.Empty;

        //[JsonPropertyName("external_link")]
        //public object? ExternalLink { get; set; }

        //[JsonPropertyName("latest_trade_price")]
        //public object? LatestTradePrice { get; set; }

        //[JsonPropertyName("latest_trade_timestamp")]
        //public object? LatestTradeTimestamp { get; set; }

        //[JsonPropertyName("latest_trade_transaction_hash")]
        //public object? LatestTradeTransactionHash { get; set; }

        //[JsonPropertyName("attributes")]
        //public object? Attributes { get; set; }
    }
}
