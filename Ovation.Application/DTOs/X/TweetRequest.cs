using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.X
{
    public class TweetRequest
    {
        [JsonPropertyName("for_super_followers_only")]
        public bool ForSuperFollowersOnly { get; set; } = false;

        [JsonPropertyName("nullcast")]
        public bool Nullcast { get; set; } = false;

        [JsonPropertyName("share_with_followers")]
        public bool ShareWithFollowers { get; set; } = false;

        [JsonPropertyName("reply")]
        public Reply? Reply { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty!;
    }

    public class Reply
    {
        [JsonPropertyName("in_reply_to_tweet_id")]
        public string InReplyToTweetId { get; set; } = string.Empty!;
    }

    public class  TweetResponse
    {
        [JsonPropertyName("data")]
        public TweetData? Data { get; set; }
    }

    public class TweetData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty!;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty!;

        [JsonPropertyName("edit_history_tweet_ids")]
        public List<string>? EditHistoryTweetIds { get; set; }
    }
}
