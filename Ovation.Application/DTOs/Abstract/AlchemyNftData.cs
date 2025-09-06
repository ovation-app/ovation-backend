using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Abstract
{
    public class AlchemyNftData
    {
        [JsonPropertyName("ownedNfts")]
        public List<OwnedNft>? OwnedNfts { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("validAt")]
        public ValidAt? ValidAt { get; set; }

        [JsonPropertyName("pageKey")]
        public string? PageKey { get; set; }
    }


    public class AcquiredAt
    {
        [JsonPropertyName("blockTimestamp")]
        public object? BlockTimestamp { get; set; }

        [JsonPropertyName("blockNumber")]
        public object? BlockNumber { get; set; }
    }

    public class Attribute
    {
        [JsonPropertyName("value")]
        public object? Value { get; set; }

        [JsonPropertyName("trait_type")]
        public string? TraitType { get; set; }

        [JsonPropertyName("display_type")]
        public string? DisplayType { get; set; }

        [JsonPropertyName("max_value")]
        public int? MaxValue { get; set; }
    }

    public class Collection
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("family")]
        public string? Family { get; set; }
    }

    public class Contract
    {
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }

        [JsonPropertyName("totalSupply")]
        public string? TotalSupply { get; set; }

        [JsonPropertyName("tokenType")]
        public string? TokenType { get; set; }

        [JsonPropertyName("contractDeployer")]
        public object? ContractDeployer { get; set; }

        [JsonPropertyName("deployedBlockNumber")]
        public object? DeployedBlockNumber { get; set; }

        [JsonPropertyName("openSeaMetadata")]
        public OpenSeaMetadata? OpenSeaMetadata { get; set; }

        [JsonPropertyName("isSpam")]
        public bool? IsSpam { get; set; }

        [JsonPropertyName("spamClassifications")]
        public List<object>? SpamClassifications { get; set; }
    }

    public class Creator
    {
        [JsonPropertyName("share")]
        public int? Share { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }
    }

    public class CustomFields
    {
        [JsonPropertyName("date")]
        public object? Date { get; set; }

        [JsonPropertyName("edition")]
        public int? Edition { get; set; }

        [JsonPropertyName("compiler")]
        public string? Compiler { get; set; }

        [JsonPropertyName("dna")]
        public string? Dna { get; set; }
    }

    public class File
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("uri")]
        public string? Uri { get; set; }
    }

    public class Image
    {
        [JsonPropertyName("cachedUrl")]
        public string? CachedUrl { get; set; }

        [JsonPropertyName("thumbnailUrl")]
        public string? ThumbnailUrl { get; set; }

        [JsonPropertyName("pngUrl")]
        public string? PngUrl { get; set; }

        [JsonPropertyName("contentType")]
        public string? ContentType { get; set; }

        [JsonPropertyName("size")]
        public int? Size { get; set; }

        [JsonPropertyName("originalUrl")]
        public string? OriginalUrl { get; set; }
    }

    public class Metadata
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("attributes")]
        public object? Attributes { get; set; }

        [JsonPropertyName("tokenId")]
        public string? TokenId { get; set; }

        [JsonPropertyName("animation_url")]
        public string? AnimationUrl { get; set; }

        [JsonPropertyName("edition")]
        public object? Edition { get; set; }

        [JsonPropertyName("file_url")]
        public string? FileUrl { get; set; }

        [JsonPropertyName("twitter")]
        public string? Twitter { get; set; }

        [JsonPropertyName("custom_fields")]
        public object? CustomFields { get; set; }

        [JsonPropertyName("project")]
        public string? Project { get; set; }

        [JsonPropertyName("external_url")]
        public string? ExternalUrl { get; set; }

        [JsonPropertyName("date")]
        public long? Date { get; set; }

        [JsonPropertyName("revealedAt")]
        public DateTime? RevealedAt { get; set; }

        [JsonPropertyName("dna")]
        public string? Dna { get; set; }

        [JsonPropertyName("canReveal")]
        public bool? CanReveal { get; set; }

        [JsonPropertyName("isRevealed")]
        public bool? IsRevealed { get; set; }

        [JsonPropertyName("compiler")]
        public string? Compiler { get; set; }

        [JsonPropertyName("soulbound")]
        public string? Soulbound { get; set; }

        [JsonPropertyName("external_link")]
        public string? ExternalLink { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }

        [JsonPropertyName("seller_fee_basis_points")]
        public int? SellerFeeBasisPoints { get; set; }

        [JsonPropertyName("collection")]
        public Collection? Collection { get; set; }

        [JsonPropertyName("properties")]
        public object? Properties { get; set; }
    }

    public class Mint
    {
        [JsonPropertyName("mintAddress")]
        public string? MintAddress { get; set; }

        [JsonPropertyName("blockNumber")]
        public int? BlockNumber { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }

        [JsonPropertyName("transactionHash")]
        public string? TransactionHash { get; set; }
    }

    public class OpenSeaMetadata
    {
        [JsonPropertyName("floorPrice")]
        public object? FloorPrice { get; set; }

        [JsonPropertyName("collectionName")]
        public string? CollectionName { get; set; }

        [JsonPropertyName("collectionSlug")]
        public object? CollectionSlug { get; set; }

        [JsonPropertyName("safelistRequestStatus")]
        public object? SafelistRequestStatus { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("externalUrl")]
        public string? ExternalUrl { get; set; }

        [JsonPropertyName("twitterUsername")]
        public object? TwitterUsername { get; set; }

        [JsonPropertyName("discordUrl")]
        public object? DiscordUrl { get; set; }

        [JsonPropertyName("bannerImageUrl")]
        public string? BannerImageUrl { get; set; }

        [JsonPropertyName("lastIngestedAt")]
        public object? LastIngestedAt { get; set; }
    }

    public class OwnedNft
    {
        [JsonPropertyName("contract")]
        public Contract? Contract { get; set; }

        [JsonPropertyName("tokenId")]
        public string? TokenId { get; set; }

        [JsonPropertyName("tokenType")]
        public string? TokenType { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("tokenUri")]
        public string? TokenUri { get; set; }

        [JsonPropertyName("image")]
        public Image? Image { get; set; }

        [JsonPropertyName("raw")]
        public Raw? Raw { get; set; }

        [JsonPropertyName("collection")]
        public object? Collection { get; set; }

        [JsonPropertyName("mint")]
        public Mint? Mint { get; set; }

        [JsonPropertyName("owners")]
        public object? Owners { get; set; }

        [JsonPropertyName("timeLastUpdated")]
        public DateTime? TimeLastUpdated { get; set; }

        [JsonPropertyName("balance")]
        public string? Balance { get; set; }

        [JsonPropertyName("acquiredAt")]
        public AcquiredAt? AcquiredAt { get; set; }
    }

    public class Properties
    {
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("files")]
        public List<File>? Files { get; set; }

        [JsonPropertyName("creators")]
        public List<Creator>? Creators { get; set; }
    }

    public class Raw
    {
        [JsonPropertyName("tokenUri")]
        public string? TokenUri { get; set; }

        [JsonPropertyName("metadata")]
        public object? Metadata { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }

    public class ValidAt
    {
        [JsonPropertyName("blockNumber")]
        public int? BlockNumber { get; set; }

        [JsonPropertyName("blockHash")]
        public string? BlockHash { get; set; }

        [JsonPropertyName("blockTimestamp")]
        public DateTime? BlockTimestamp { get; set; }
    }
}
