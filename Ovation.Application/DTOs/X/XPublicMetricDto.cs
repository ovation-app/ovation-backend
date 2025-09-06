using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.X
{
    public class XPublicMetricDto
    {
        [JsonPropertyName("data")]
        public Data? Data { get; set; }
    }

    public class Data
    {
        [JsonPropertyName("public_metrics")]
        public PublicMetrics? PublicMetrics { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }

    public class PublicMetrics
    {
        [JsonPropertyName("followers_count")]
        public int FollowersCount { get; set; }

        [JsonPropertyName("following_count")]
        public int FollowingCount { get; set; }

        [JsonPropertyName("tweet_count")]
        public int TweetCount { get; set; }

        [JsonPropertyName("listed_count")]
        public int ListedCount { get; set; }

        [JsonPropertyName("like_count")]
        public int LikeCount { get; set; }

        [JsonPropertyName("media_count")]
        public int MediaCount { get; set; }
    }
}
