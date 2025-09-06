using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.NFTScan.ENV
{
    public class EvmsCollectionNftsData
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public NftDatum? Data { get; set; }
    }

    public class NftAttribute
    {
        [JsonPropertyName("attribute_name")]
        public string AttributeName { get; set; } = string.Empty;

        [JsonPropertyName("attribute_value")]
        public string AttributeValue { get; set; } = string.Empty;

        [JsonPropertyName("percentage")]
        public string Percentage { get; set; } = string.Empty;
    }

    public class DataContent
    {
        [JsonPropertyName("contract_address")]
        public string ContractAddress { get; set; } = string.Empty;

        [JsonPropertyName("contract_name")]
        public string ContractName { get; set; } = string.Empty;

        [JsonPropertyName("contract_token_id")]
        public string ContractTokenId { get; set; } = string.Empty;

        [JsonPropertyName("token_id")]
        public string TokenId { get; set; } = string.Empty;

        [JsonPropertyName("erc_type")]
        public string ErcType { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public string Amount { get; set; } = string.Empty;

        [JsonPropertyName("minter")]
        public string Minter { get; set; } = string.Empty;

        [JsonPropertyName("owner")]
        public string Owner { get; set; } = string.Empty;

        [JsonPropertyName("own_timestamp")]
        public object? OwnTimestamp { get; set; }

        [JsonPropertyName("mint_timestamp")]
        public object? MintTimestamp { get; set; }

        [JsonPropertyName("mint_transaction_hash")]
        public string MintTransactionHash { get; set; } = string.Empty;

        [JsonPropertyName("mint_price")]
        public object? MintPrice { get; set; }

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

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("image_uri")]
        public string ImageUri { get; set; } = string.Empty;

        [JsonPropertyName("external_link")]
        public string ExternalLink { get; set; } = string.Empty;

        [JsonPropertyName("latest_trade_price")]
        public object? LatestTradePrice { get; set; }

        [JsonPropertyName("latest_trade_symbol")]
        public string? LatestTradeSymbol { get; set; }

        [JsonPropertyName("latest_trade_token")]
        public string? LatestTradeToken { get; set; }

        [JsonPropertyName("latest_trade_timestamp")]
        public object? LatestTradeTimestamp { get; set; }

        [JsonPropertyName("nftscan_id")]
        public string NftscanId { get; set; } = string.Empty;

        [JsonPropertyName("nftscan_uri")]
        public string NftscanUri { get; set; } = string.Empty;

        [JsonPropertyName("small_nftscan_uri")]
        public string SmallNftscanUri { get; set; } = string.Empty;

        [JsonPropertyName("attributes")]
        public List<NftAttribute>? Attributes { get; set; }

        [JsonPropertyName("rarity_score")]
        public object? RarityScore { get; set; }

        [JsonPropertyName("rarity_rank")]
        public object? RarityRank { get; set; }
    }

    public class NftDatum
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("next")]
        public string? Next { get; set; }

        [JsonPropertyName("content")]
        public List<DataContent>? Content { get; set; }
    }
}
