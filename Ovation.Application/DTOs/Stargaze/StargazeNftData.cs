using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Stargaze
{
    public class StargazeNftData
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("tokens")]
        public List<Token>? Tokens { get; set; }
    }

    public class Collection
    {
        [JsonPropertyName("creator")]
        public string? Creator { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("royalty_info")]
        public RoyaltyInfo? RoyaltyInfo { get; set; }

        [JsonPropertyName("start_trading_time")]
        public string? StartTradingTime { get; set; }

        [JsonPropertyName("minter_config")]
        public object? MinterConfig { get; set; }

        [JsonPropertyName("whitelist_config")]
        public object? WhitelistConfig { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }

        [JsonPropertyName("contractAddress")]
        public string? ContractAddress { get; set; }

        [JsonPropertyName("provenance")]
        public object? Provenance { get; set; }
    }

    public class RoyaltyInfo
    {
        [JsonPropertyName("payment_address")]
        public string? PaymentAddress { get; set; }

        [JsonPropertyName("share")]
        public string? Share { get; set; }
    }

    public class Token
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("tokenId")]
        public string? TokenId { get; set; }

        [JsonPropertyName("creator")]
        public string? Creator { get; set; }

        [JsonPropertyName("createdBy")]
        public object? CreatedBy { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("ownedBy")]
        public string? OwnedBy { get; set; }

        [JsonPropertyName("tokenUri")]
        public string? TokenUri { get; set; }

        [JsonPropertyName("rarityScore")]
        public string? RarityScore { get; set; }

        [JsonPropertyName("collection")]
        public Collection? Collection { get; set; }

        [JsonPropertyName("price")]
        public string? Price { get; set; }

        [JsonPropertyName("reserveFor")]
        public object? ReserveFor { get; set; }

        [JsonPropertyName("expiresAt")]
        public object? ExpiresAt { get; set; }

        [JsonPropertyName("expiresAtDateTime")]
        public object? ExpiresAtDateTime { get; set; }
    }
}
