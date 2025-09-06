using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Tezos
{
    public class TezosNftTransactionData
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("level")]
        public int Level { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("token")]
        public Tokeenn? Token { get; set; }

        [JsonPropertyName("from")]
        public From? From { get; set; }

        [JsonPropertyName("to")]
        public To? To { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; } = string.Empty;

        [JsonPropertyName("transactionId")]
        public long TransactionId { get; set; }
    }


    public class Contractt
    {
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
    }

    public class Dimensionss
    {
        [JsonPropertyName("unit")]
        public string Unit { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
    }

    public class Formatt
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; } = string.Empty;

        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("fileSize")]
        public string FileSize { get; set; } = string.Empty;

        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = string.Empty;

        [JsonPropertyName("dimensions")]
        public Dimensionss? Dimensions { get; set; }
    }

    public class From
    {
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
    }

    public class Metadataa
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("tags")]
        public List<string>? Tags { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; } = string.Empty;

        [JsonPropertyName("minter")]
        public string Minter { get; set; } = string.Empty;

        [JsonPropertyName("rights")]
        public string Rights { get; set; } = string.Empty;

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("formats")]
        public List<Formatt>? Formats { get; set; }

        [JsonPropertyName("creators")]
        public List<string>? Creators { get; set; }

        [JsonPropertyName("decimals")]
        public string Decimals { get; set; } = string.Empty;

        [JsonPropertyName("royalties")]
        public Royaltiess? Royalties { get; set; }

        [JsonPropertyName("attributes")]
        public List<object>? Attributes { get; set; }

        [JsonPropertyName("displayUri")]
        public string DisplayUri { get; set; } = string.Empty;

        [JsonPropertyName("artifactUri")]
        public string ArtifactUri { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("mintingTool")]
        public string MintingTool { get; set; } = string.Empty;

        [JsonPropertyName("thumbnailUri")]
        public string ThumbnailUri { get; set; } = string.Empty;
    }

    public class Royaltiess
    {
        [JsonPropertyName("shares")]
        public object? Shares { get; set; }

        [JsonPropertyName("decimals")]
        public string Decimals { get; set; } = string.Empty;
    }


    public class To
    {
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
    }

    public class Tokeenn
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("contract")]
        public Contractt? Contract { get; set; }

        [JsonPropertyName("tokenId")]
        public string TokenId { get; set; } = string.Empty;

        [JsonPropertyName("standard")]
        public string Standard { get; set; } = string.Empty;

        [JsonPropertyName("totalSupply")]
        public string TotalSupply { get; set; } = string.Empty;

        [JsonPropertyName("metadata")]
        public Metadataa? Metadata { get; set; }
    }
}
