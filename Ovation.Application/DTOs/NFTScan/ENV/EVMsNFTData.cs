using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.NFTScan.ENV
{
    public class EVMsNFTData
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

        [JsonPropertyName("amount")]
        public string? Amount { get; set; }

        [JsonPropertyName("minter")]
        public string? Minter { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("own_timestamp")]
        public long? OwnTimestamp { get; set; }

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

        [JsonPropertyName("description")]
        public string? Description { get; set; }

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

        [JsonPropertyName("nftscan_id")]
        public string? NftscanId { get; set; }

        [JsonPropertyName("nftscan_uri")]
        public string? NftscanUri { get; set; }

        [JsonPropertyName("small_nftscan_uri")]
        public string? SmallNftscanUri { get; set; }

        [JsonPropertyName("attributes")]
        public List<object> Attributes { get; set; }

        [JsonPropertyName("rarity_score")]
        public decimal? RarityScore { get; set; }

        [JsonPropertyName("rarity_rank")]
        public int? RarityRank { get; set; }
    }

    public class Datum
    {
        [JsonPropertyName("contract_address")]
        public string? ContractAddress { get; set; }

        [JsonPropertyName("contract_name")]
        public string? ContractName { get; set; }

        [JsonPropertyName("logo_url")]
        public string? LogoUrl { get; set; }

        [JsonPropertyName("owns_total")]
        public int? OwnsTotal { get; set; }

        [JsonPropertyName("items_total")]
        public int? ItemsTotal { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("floor_price")]
        public decimal? FloorPrice { get; set; }

        [JsonPropertyName("verified")]
        public bool? Verified { get; set; }

        [JsonPropertyName("opensea_verified")]
        public bool? OpenseaVerified { get; set; }

        [JsonPropertyName("is_spam")]
        public bool? IsSpam { get; set; }

        [JsonPropertyName("assets")]
        public List<Asset>? Assets { get; set; }
    }
}
