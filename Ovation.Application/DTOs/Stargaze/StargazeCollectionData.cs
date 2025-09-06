using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Stargaze
{
    public class StargazeCollectionData
    {
        public CollectionData? Collection { get; set; }
    }

    public class CollectionData
    {
        public string? Name { get; set; }
        public Stats? Stats { get; set; }
        public string? ContractAddress { get; set; }
        public Categories? Categories { get; set; }
        public string? ContractUri { get; set; }
        public string? CreationTime { get; set; }
        public string? Description { get; set; }
        public Creator? Creator { get; set; }
        public TokenCounts? TokenCounts { get; set; }
        public Media? Media { get; set; }
    }

    public class Stats
    {
        public int NumOwners { get; set; }
    }

    public class Categories
    {
        [JsonPropertyName("public")]
        public List<string>? Public { get; set; }

        [JsonPropertyName("private")]
        public List<object>? Private { get; set; }
    }

    public class Creator
    {
        public CreatorName? Name { get; set; }
    }

    public class CreatorName
    {
        public string? Name { get; set; }
        public string? Image { get; set; }
        public string? OwnerAddr { get; set; }
    }

    public class TokenCounts
    {
        public int Total { get; set; }
        public int Listed { get; set; }
        public int Active { get; set; }
    }

    public class Media
    {
        public string? Type { get; set; }
        public object? Urls { get; set; }
        public string? FallbackUrl { get; set; }
    }

}
