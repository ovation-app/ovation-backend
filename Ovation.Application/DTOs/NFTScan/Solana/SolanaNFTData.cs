using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.NFTScan.Solana
{
    public class SolanaNFTData
    {
        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("msg")]
        public object? Msg { get; set; }

        [JsonPropertyName("data")]
        public List<Datum>? Data { get; set; }
    }

    public class Asset
    {
        [JsonPropertyName("block_number")]
        public int? BlockNumber { get; set; }

        [JsonPropertyName("interact_program")]
        public string? InteractProgram { get; set; }

        [JsonPropertyName("collection")]
        public string? Collection { get; set; }

        [JsonPropertyName("token_address")]
        public string? TokenAddress { get; set; }

        [JsonPropertyName("minter")]
        public string? Minter { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("mint_timestamp")]
        public object? MintTimestamp { get; set; }

        [JsonPropertyName("mint_transaction_hash")]
        public string? MintTransactionHash { get; set; }

        [JsonPropertyName("mint_price")]
        public decimal? MintPrice { get; set; }

        [JsonPropertyName("token_uri")]
        public string? TokenUri { get; set; }

        [JsonPropertyName("metadata_json")]
        public string? MetadataJson { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("content_type")]
        public string? ContentType { get; set; }

        [JsonPropertyName("content_uri")]
        public string? ContentUri { get; set; }

        [JsonPropertyName("image_uri")]
        public string? ImageUri { get; set; }

        [JsonPropertyName("external_link")]
        public string? ExternalLink { get; set; }

        [JsonPropertyName("latest_trade_price")]
        public decimal? LatestTradePrice { get; set; }

        [JsonPropertyName("latest_trade_symbol")]
        public string? LatestTradeSymbol { get; set; }

        [JsonPropertyName("latest_trade_token")]
        public object? LatestTradeToken { get; set; }

        [JsonPropertyName("latest_trade_timestamp")]
        public object? LatestTradeTimestamp { get; set; }

        [JsonPropertyName("latest_trade_transaction_hash")]
        public string? LatestTradeTransactionHash { get; set; }

        [JsonPropertyName("attributes")]
        public List<Attribute>? Attributes { get; set; }

        [JsonPropertyName("cnft")]
        public bool? Cnft { get; set; }
    }

    public class Attribute
    {
        [JsonPropertyName("attribute_name")]
        public string? AttributeName { get; set; }

        [JsonPropertyName("attribute_value")]
        public string? AttributeValue { get; set; }

        [JsonPropertyName("percentage")]
        public string? Percentage { get; set; }
    }

    public class Datum
    {
        [JsonPropertyName("collection")]
        public string? Collection { get; set; }

        [JsonPropertyName("logo_url")]
        public string? LogoUrl { get; set; }

        [JsonPropertyName("owns_total")]
        public int? OwnsTotal { get; set; }

        [JsonPropertyName("items_total")]
        public int? ItemsTotal { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("assets")]
        public List<Asset>? Assets { get; set; }
    }
}
