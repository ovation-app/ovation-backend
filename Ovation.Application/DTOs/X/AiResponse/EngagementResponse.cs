using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.X.AiResponse
{
    public class EngagementResponse
    {
        [JsonPropertyName("user_handle")]
        public string UserHandle { get; set; }

        [JsonPropertyName("message_type")]
        public string MessageType { get; set; }

        [JsonPropertyName("message_content")]
        public string MessageContent { get; set; }

        [JsonPropertyName("engagement_type")]
        public string EngagementType { get; set; }

        [JsonPropertyName("featured_benefits")]
        public string[] FeaturedBenefits { get; set; }

        [JsonPropertyName("contains_cta")]
        public bool ContainsCta { get; set; }

        [JsonPropertyName("estimated_character_count")]
        public int EstimatedCharacterCount { get; set; }

        [JsonPropertyName("formatting_notes")]
        public string FormattingNotes { get; set; }

        [JsonPropertyName("brand_compliance")]
        public string BrandCompliance { get; set; }
    }
}
