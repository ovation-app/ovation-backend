using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.NFTScan.ENV
{
    public class EvmsCollectionData
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public CollectionData? Data { get; set; }
    }

    public class Attributee
    {
        [JsonPropertyName("attributes_name")]
        public string AttributesName { get; set; } = string.Empty;

        [JsonPropertyName("attributes_values")]
        public List<AttributesValuee>? AttributesValues { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    public class AttributesValuee
    {
        [JsonPropertyName("attributes_value")]
        public string AttributesValue { get; set; } = string.Empty;

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    public class CollectionData
    {
        [JsonPropertyName("contract_address")]
        public string ContractAddress { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("website")]
        public string Website { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public object Email { get; set; } = string.Empty;

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
        public List<Attributee> Attributes { get; set; } = new();

        [JsonPropertyName("erc_type")]
        public string ErcType { get; set; } = string.Empty;

        [JsonPropertyName("deploy_block_number")]
        public int DeployBlockNumber { get; set; }

        [JsonPropertyName("owner")]
        public string Owner { get; set; } = string.Empty;

        [JsonPropertyName("verified")]
        public bool Verified { get; set; }

        [JsonPropertyName("opensea_verified")]
        public bool OpenseaVerified { get; set; }

        [JsonPropertyName("is_spam")]
        public bool IsSpam { get; set; }

        [JsonPropertyName("royalty")]
        public object? Royalty { get; set; }

        [JsonPropertyName("items_total")]
        public int? ItemsTotal { get; set; }

        [JsonPropertyName("amounts_total")]
        public object? AmountsTotal { get; set; }

        [JsonPropertyName("owners_total")]
        public object? OwnersTotal { get; set; }

        [JsonPropertyName("opensea_floor_price")]
        public object? OpenseaFloorPrice { get; set; }

        [JsonPropertyName("opensea_slug")]
        public string OpenseaSlug { get; set; } = string.Empty;

        [JsonPropertyName("floor_price")]
        public object? FloorPrice { get; set; }

        [JsonPropertyName("collections_with_same_name")]
        public List<string>? CollectionsWithSameName { get; set; }

        [JsonPropertyName("price_symbol")]
        public string PriceSymbol { get; set; } = string.Empty;
    }
}
