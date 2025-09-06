using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Tezos
{
    public class ObjktTransactionData
    {
        [JsonPropertyName("data")]
        public Dataa? Data { get; set; }
    }

    public class Dataa
    {
        [JsonPropertyName("event")]
        public List<Event>? Event { get; set; }
    }

    public class Event
    {
        [JsonPropertyName("event_type")]
        public string? EventType { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("price_xtz")]
        public long? Price { get; set; }

        [JsonPropertyName("creator")]
        public Creator? Creator { get; set; }

        [JsonPropertyName("recipient")]
        public Recipient? Recipient { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    public class Creator
    {
        [JsonPropertyName("address")]
        public string? Address { get; set; }
    }

    public class Recipient
    {
        [JsonPropertyName("address")]
        public string? Address { get; set; }
    }
}
