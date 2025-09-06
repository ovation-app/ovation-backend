using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Abstract
{
    public class AbstractTransactionData
    {
        [JsonPropertyName("jsonrpc")]
        public string? Jsonrpc { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("result")]
        public Result? Result { get; set; }
    }

    public class Metadataa
    {
        [JsonPropertyName("blockTimestamp")]
        public DateTime BlockTimestamp { get; set; }
    }

    public class RawContract
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("decimal")]
        public string? Decimal { get; set; }
    }

    public class Result
    {
        [JsonPropertyName("transfers")]
        public List<Transfer>? Transfers { get; set; }

        [JsonPropertyName("pageKey")]
        public string? PageKey { get; set; }
    }

    public class Transfer
    {
        [JsonPropertyName("blockNum")]
        public string? BlockNum { get; set; }

        [JsonPropertyName("uniqueId")]
        public string? UniqueId { get; set; }

        [JsonPropertyName("hash")]
        public string? Hash { get; set; }

        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("to")]
        public string? To { get; set; }

        [JsonPropertyName("value")]
        public object? Value { get; set; }

        [JsonPropertyName("erc721TokenId")]
        public object? Erc721TokenId { get; set; }

        [JsonPropertyName("erc1155Metadata")]
        public object? Erc1155Metadata { get; set; }

        [JsonPropertyName("tokenId")]
        public string? TokenId { get; set; }

        [JsonPropertyName("asset")]
        public string? Asset { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("rawContract")]
        public RawContract? RawContract { get; set; }

        [JsonPropertyName("metadata")]
        public Metadataa? Metadata { get; set; }
    }
}
