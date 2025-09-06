namespace Ovation.Application.DTOs.Tezos
{
    public class Token
    {
        public long? Id { get; set; }
        public Contract? Contract { get; set; }
        public string? TokenId { get; set; }
        public string? Standard { get; set; }
        public string? TotalSupply { get; set; }
        public Metadata? Metadata { get; set; }
    }

    public class Contract
    {
        public string? Address { get; set; }
    }

    public class Metadata
    {
        public DateTime? Date { get; set; }
        public string? Name { get; set; }
        public List<string>? Tags { get; set; }
        public string? Image { get; set; }
        public string? Minter { get; set; }
        public string? Rights { get; set; }
        public string? Symbol { get; set; }
        public List<Format>? Formats { get; set; }
        public List<string>? Creators { get; set; }
        public string? Decimals { get; set; }
        public Royalties? Royalties { get; set; }
        public List<object>? Attributes { get; set; }
        public string? DisplayUri { get; set; }
        public string? ArtifactUri { get; set; }
        public string? Description { get; set; }
        public string? MintingTool { get; set; }
        public string? ThumbnailUri { get; set; }
    }

    public class Dimensions
    {
        public string? Unit { get; set; }
        public string? Value { get; set; }
    }

    public class Format
    {
        public string? Uri { get; set; }
        public string? FileName { get; set; }
        public string? FileSize { get; set; }
        public string? MimeType { get; set; }
        public Dimensions? Dimensions { get; set; }
    }

    public class Royalties
    {
        public object? Shares { get; set; }
        public string? Decimals { get; set; }
    }
}
