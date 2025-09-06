using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs
{
    public class NftDataDto
    {
        public long Id { get; set; }

        public string? ImageUrl { get; set; }

        public string? AnimationUrl { get; set; }

        public string? ContractName { get; set; }

        public decimal? usd { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
        public string? Type { get; set; }

        public string? TokenId { get; set; }

        public long? CollectionId { get; set; }

        public string? native { get; set; }

        public DateOnly? CustodyDate { get; set; }
        
        public string? Prices { get; set; }

        public bool isPrivate { get; set; } = false;

        public bool isFav { get; set; } = false;

        public bool ForSale { get; set; } = false;
        public decimal? SalePrice { get; set; }
        public string? SaleCurrency { get; set; }
        public string? SaleUrl { get; set; }
        public string? SaleMetadata { get; set; }
    }

    public class NftData
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("animationUrl")]
        public string? AnimationUrl { get; set; }

        [JsonPropertyName("contractName")]
        public string? ContractName { get; set; }

        [JsonPropertyName("prices")]
        public List<NftPrice>? Prices { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("tokenId")]
        public string? TokenId { get; set; }

        public long? CollectionId { get; set; }

        [JsonPropertyName("custodyDate")]
        public DateOnly? CustodyDate { get; set; }

        [JsonPropertyName("isPrivate")]
        public bool IsPrivate { get; set; } = false;
    }

    public class NftPrice
    {
        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class NftSearchData
    {
        public string? Name { get; set; }
        public long NftId { get; set; }
        public string? Chain { get; set; }
        public string? ImageUrl { get; set; }
        public string? AnimationUrl { get; set; }
        public string? ContractName { get; set; }
        public string? LogoUrl { get; set; }
        public string? ContractAddress { get; set; }
        public string? TokenAddress { get; set; }
        public string? TokenId { get; set; }
        public string? Description { get; set; }
        public List<NftPrice> Prices { get; set; } = new();
        public List<NftUser> Users { get; set; } = new();
    }

    public class NftUser
    {
        public string? Username { get; set; }
        public Guid UserId { get; set; }
        public string? ProfileImage{ get; set; }
        public bool IsUser { get; set; }
    }

    public class AssetNft
    {
        public string? ImageUrl { get; set; }
        public string? AnimationUrl { get; set; }
        public string? LastTradePrice { get; set; }
        public string? ContractAddress { get; set; }
        public string? TokenAddress { get; set; }
        public string? TokenId { get; set; }
        public string Chain { get; set; }
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int UserCount { get; set; }
        public List<NftUser> Users { get; set; } = new();
    }
}
