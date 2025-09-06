using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.NFTScan.Solana
{
    public class SolanaCollectionData
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public CollectionData? Data { get; set; }
    }

    public class CollectionAttribute
    {
        [JsonPropertyName("attributes_name")]
        public string AttributesName { get; set; } = string.Empty;

        [JsonPropertyName("attributes_values")]
        public List<AttributesValue>? AttributesValues { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    public class AttributesValue
    {
        [JsonPropertyName("attributes_value")]
        public string AttributesValuee { get; set; } = string.Empty;

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    public class CollectionData
    {
        [JsonPropertyName("collection")]
        public string Collection { get; set; } = string.Empty;

        [JsonPropertyName("collection_token_address")]
        public string CollectionTokenAddress { get; set; } = string.Empty;

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("website")]
        public string Website { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("twitter")]
        public string Twitter { get; set; } = string.Empty;

        [JsonPropertyName("discord")]
        public string Discord { get; set; } = string.Empty;

        [JsonPropertyName("telegram")]
        public string Telegram { get; set; } = string.Empty;

        [JsonPropertyName("github")]
        public string Github { get; set; } = string.Empty;

        [JsonPropertyName("instagram")]
        public string Instagram { get; set; } = string.Empty;

        [JsonPropertyName("medium")]
        public string Medium { get; set; } = string.Empty;

        [JsonPropertyName("logo_url")]
        public string LogoUrl { get; set; } = string.Empty;

        [JsonPropertyName("banner_url")]
        public string BannerUrl { get; set; } = string.Empty;

        [JsonPropertyName("featured_url")]
        public string FeaturedUrl { get; set; } = string.Empty;

        [JsonPropertyName("large_image_url")]
        public string LargeImageUrl { get; set; } = string.Empty;

        [JsonPropertyName("attributes")]
        public List<CollectionAttribute>? Attributes { get; set; }

        [JsonPropertyName("create_block_number")]
        public int CreateBlockNumber { get; set; }

        [JsonPropertyName("verified")]
        public bool Verified { get; set; }

        [JsonPropertyName("items_total")]
        public int ItemsTotal { get; set; }

        [JsonPropertyName("owners_total")]
        public int OwnersTotal { get; set; }
    }
}
