using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Tezos
{
    public class TezosCollectionNftsData
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("contract")]
        public TokenContract? Contract { get; set; }

        [JsonPropertyName("tokenId")]
        public string? TokenId { get; set; }

        [JsonPropertyName("standard")]
        public string? Standard { get; set; }

        [JsonPropertyName("firstMinter")]
        public FirstMinter? FirstMinter { get; set; }

        [JsonPropertyName("firstLevel")]
        public int FirstLevel { get; set; }

        [JsonPropertyName("firstTime")]
        public DateTime FirstTime { get; set; }

        [JsonPropertyName("lastLevel")]
        public int LastLevel { get; set; }

        [JsonPropertyName("lastTime")]
        public DateTime LastTime { get; set; }

        [JsonPropertyName("transfersCount")]
        public int TransfersCount { get; set; }

        [JsonPropertyName("balancesCount")]
        public int BalancesCount { get; set; }

        [JsonPropertyName("holdersCount")]
        public int HoldersCount { get; set; }

        [JsonPropertyName("totalMinted")]
        public string? TotalMinted { get; set; }

        [JsonPropertyName("totalBurned")]
        public string? TotalBurned { get; set; }

        [JsonPropertyName("totalSupply")]
        public string? TotalSupply { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata? Metadata { get; set; }
    }

    public class TokenRoyalties
    {
        [JsonPropertyName("shares")]
        public object? Shares { get; set; }

        [JsonPropertyName("decimals")]
        public string? Decimals { get; set; }
    }

    public class TokenContract
    {
        [JsonPropertyName("address")]
        public string? Address { get; set; }
    }

    public class TokenDimensions
    {
        [JsonPropertyName("unit")]
        public string? Unit { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class FirstMinter
    {
        [JsonPropertyName("address")]
        public string? Address { get; set; }
    }

    public class TokenFormat
    {
        [JsonPropertyName("uri")]
        public string? Uri { get; set; }

        [JsonPropertyName("fileName")]
        public string? FileName { get; set; }

        [JsonPropertyName("fileSize")]
        public string? FileSize { get; set; }

        [JsonPropertyName("mimeType")]
        public string? MimeType { get; set; }

        [JsonPropertyName("dimensions")]
        public TokenDimensions? Dimensions { get; set; }
    }

    public class TokenMetadata
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("tags")]
        public List<object>? Tags { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("minter")]
        public string? Minter { get; set; }

        [JsonPropertyName("rights")]
        public string? Rights { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }

        [JsonPropertyName("formats")]
        public List<TokenFormat>? Formats { get; set; }

        [JsonPropertyName("creators")]
        public List<string>? Creators { get; set; }

        [JsonPropertyName("decimals")]
        public string? Decimals { get; set; }

        [JsonPropertyName("royalties")]
        public TokenRoyalties? Royalties { get; set; }

        [JsonPropertyName("attributes")]
        public List<object>? Attributes { get; set; }

        [JsonPropertyName("displayUri")]
        public string? DisplayUri { get; set; }

        [JsonPropertyName("artifactUri")]
        public string? ArtifactUri { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("mintingTool")]
        public string? MintingTool { get; set; }

        [JsonPropertyName("thumbnailUri")]
        public string? ThumbnailUri { get; set; }
    }
}
