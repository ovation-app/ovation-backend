using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Apis
{
    public class MintifyGetCollection
    {
        public int Status { get; set; }
        public List<Datum>? Data { get; set; }
        public string? Cache { get; set; }
    }

    public class Datum
    {
        public string? ContractAddress { get; set; }
        public string? Name { get; set; }
        public string? Symbol { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? BackgroundImageUrl { get; set; }
        public int Holders { get; set; }
        public int Quantity { get; set; }
        public string? OwnerAddress { get; set; }
        public string? Slug { get; set; }
        public int TotalVolume { get; set; }
        public int TotalSales { get; set; }
        public object? Floor { get; set; }
        public int IsBluechip { get; set; }
        public int Excluded { get; set; }
        public string? Chain { get; set; }
        public DateTime Updated { get; set; }
        public Metas? Metas { get; set; }
        public object? Vanity { get; set; }
        public string? FirstTransaction { get; set; }
    }

    public class Metas
    {
        [JsonPropertyName("contract-verified")]
        public string? ContractVerified { get; set; }
        public string? FirstMintDate { get; set; }
        public string? HasGifs { get; set; }
        public string? HasVideos { get; set; }
        public string? ImageCacheDone { get; set; }
        public string? IsPpv2 { get; set; }

        [JsonPropertyName("project-discord")]
        public string? ProjectDiscord { get; set; }

        [JsonPropertyName("project-twitter")]
        public string? ProjectTwitter { get; set; }
        public string? RarityDone { get; set; }
        public string? RegenerateCache { get; set; }
        public string? RevealDone { get; set; }
        public string? TraitsDone { get; set; }
        public string? TraitsDoneSimplehash { get; set; }
        public string? Url { get; set; }
    }
}
