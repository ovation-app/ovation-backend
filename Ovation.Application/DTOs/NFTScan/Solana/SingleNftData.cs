using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.NFTScan.Solana
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
        [JsonPropertyName("block_number")]
        public int BlockNumber { get; set; }

        [JsonPropertyName("interact_program")]
        public string InteractProgram { get; set; } = string.Empty;

        [JsonPropertyName("collection")]
        public string Collection { get; set; } = string.Empty;

        [JsonPropertyName("token_address")]
        public string TokenAddress { get; set; } = string.Empty;

        [JsonPropertyName("minter")]
        public string Minter { get; set; } = string.Empty;

        [JsonPropertyName("owner")]
        public string Owner { get; set; } = string.Empty;

        [JsonPropertyName("mint_timestamp")]
        public long MintTimestamp { get; set; }

        [JsonPropertyName("mint_transaction_hash")]
        public string MintTransactionHash { get; set; } = string.Empty;

        [JsonPropertyName("mint_price")]
        public double MintPrice { get; set; }

        [JsonPropertyName("token_uri")]
        public string TokenUri { get; set; } = string.Empty;

        [JsonPropertyName("metadata_json")]
        public string MetadataJson { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("content_type")]
        public string ContentType { get; set; } = string.Empty;

        [JsonPropertyName("content_uri")]
        public string ContentUri { get; set; } = string.Empty;

        [JsonPropertyName("image_uri")]
        public string ImageUri { get; set; } = string.Empty;

        [JsonPropertyName("external_link")]
        public string ExternalLink { get; set; } = string.Empty;

        [JsonPropertyName("latest_trade_price")]
        public object? LatestTradePrice { get; set; }

        [JsonPropertyName("latest_trade_symbol")]
        public object? LatestTradeSymbol { get; set; }

        [JsonPropertyName("latest_trade_token")]
        public object? LatestTradeToken { get; set; }

        [JsonPropertyName("latest_trade_timestamp")]
        public object? LatestTradeTimestamp { get; set; }

        [JsonPropertyName("latest_trade_transaction_hash")]
        public object? LatestTradeTransactionHash { get; set; }

        [JsonPropertyName("attributes")]
        public object? Attributes { get; set; }

        [JsonPropertyName("cnft")]
        public bool Cnft { get; set; }
    }
}
