using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.X
{
    public class XUserTweetDto
    {
        [JsonPropertyName("data")]
        public List<Datum>? Data { get; set; }

        [JsonPropertyName("meta")]
        public Meta? Meta { get; set; }
    }

    public class Datum
    {
        [JsonPropertyName("edit_history_tweet_ids")]
        public List<string>? EditHistoryTweetIds { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }

    public class Meta
    {
        [JsonPropertyName("result_count")]
        public int ResultCount { get; set; }

        [JsonPropertyName("newest_id")]
        public string NewestId { get; set; } = string.Empty;

        [JsonPropertyName("oldest_id")]
        public string OldestId { get; set; } = string.Empty;
    }
}
