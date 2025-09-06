using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Abstract
{
    public class AbstractContractNftData
    {
        [JsonPropertyName("nfts")]
        public List<Nft>? Nfts { get; set; }

        [JsonPropertyName("pageKey")]
        public string? PageKey { get; set; }
    }

    public class AbstractContractNftData2
    {
        [JsonPropertyName("nfts")]
        public List<NftSingle>? Nfts { get; set; }

        [JsonPropertyName("pageKey")]
        public string? PageKey { get; set; }
    }

    public class Attributee
    {
        [JsonPropertyName("trait_type")]
        public string? TraitType { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class Contractt
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
        public string? ContractDeployer { get; set; }

        [JsonPropertyName("deployedBlockNumber")]
        public string? DeployedBlockNumber { get; set; }

        [JsonPropertyName("openSeaMetadata")]
        public OpenSeaMetadataa OpenSeaMetadata { get; set; }
    }

    public class Imagee
    {
        [JsonPropertyName("cachedUrl")]
        public string? CachedUrl { get; set; }

        [JsonPropertyName("thumbnailUrl")]
        public string? ThumbnailUrl { get; set; }

        [JsonPropertyName("pngUrl")]
        public string? PngUrl { get; set; }

        [JsonPropertyName("contentType")]
        public string? ContentType { get; set; }

        [JsonPropertyName("originalUrl")]
        public string? OriginalUrl { get; set; }
    }

    public class Metaadata
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("attributes")]
        public List<Attribute> Attributes { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("animation_url")]
        public string AnimationUrl { get; set; }
    }

    public class Mintt
    {
        [JsonPropertyName("mintAddress")]
        public string MintAddress { get; set; }

        [JsonPropertyName("blockNumber")]
        public object BlockNumber { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("transactionHash")]
        public string TransactionHash { get; set; }
    }

    public class Nft
    {
        [JsonPropertyName("contract")]
        public Contractt? Contract { get; set; }

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
        public Imagee? Image { get; set; }

        [JsonPropertyName("raw")]
        public Raww? Raw { get; set; }

        [JsonPropertyName("collection")]
        public string? Collection { get; set; }

        [JsonPropertyName("mint")]
        public Mintt? Mint { get; set; }

        [JsonPropertyName("owners")]
        public string? Owners { get; set; }

        [JsonPropertyName("timeLastUpdated")]
        public DateTime? TimeLastUpdated { get; set; }
    }

    public class OpenSeaMetadataa
    {
        [JsonPropertyName("floorPrice")]
        public string? FloorPrice { get; set; }

        [JsonPropertyName("collectionName")]
        public string? CollectionName { get; set; }

        [JsonPropertyName("collectionSlug")]
        public string? CollectionSlug { get; set; }

        [JsonPropertyName("safelistRequestStatus")]
        public string? SafelistRequestStatus { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("externalUrl")]
        public string? ExternalUrl { get; set; }

        [JsonPropertyName("twitterUsername")]
        public string? TwitterUsername { get; set; }

        [JsonPropertyName("discordUrl")]
        public string? DiscordUrl { get; set; }

        [JsonPropertyName("bannerImageUrl")]
        public string? BannerImageUrl { get; set; }

        [JsonPropertyName("lastIngestedAt")]
        public string? LastIngestedAt { get; set; }
    }

    public class Raww
    {
        [JsonPropertyName("tokenUri")]
        public string? TokenUri { get; set; }

        [JsonPropertyName("metadata")]
        public Metaadata? Metadata { get; set; }

        [JsonPropertyName("error")]
        public object? Error { get; set; }
    }


    public class NftSingle
    {

        [JsonPropertyName("tokenId")]
        public string? TokenId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("image")]
        public Imagee? Image { get; set; }

    }
}
