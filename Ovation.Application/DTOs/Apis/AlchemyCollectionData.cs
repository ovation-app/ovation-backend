namespace Ovation.Application.DTOs.Apis
{
    public class AlchemyCollectionData
    {
        public string? Address { get; set; }
        public string? Name { get; set; }
        public string? Symbol { get; set; }
        public string? TotalSupply { get; set; }
        public string? TokenType { get; set; }
        public string? ContractDeployer { get; set; }
        public object? DeployedBlockNumber { get; set; }
        public OpenSeaMetadata? OpenSeaMetadata { get; set; }
    }

    public class OpenSeaMetadata
    {
        public object? FloorPrice { get; set; }
        public string? CollectionName { get; set; }
        public string? CollectionSlug { get; set; }
        public string? SafelistRequestStatus { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? ExternalUrl { get; set; }
        public string? TwitterUsername { get; set; }
        public string? DiscordUrl { get; set; }
        public string? BannerImageUrl { get; set; }
        public DateTime? LastIngestedAt { get; set; }
    }
}
