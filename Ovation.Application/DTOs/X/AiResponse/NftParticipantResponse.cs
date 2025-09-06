using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.X.AiResponse
{
    public class NftParticipantResponse
    {
        [JsonPropertyName("user_handle")]
        public string UserHandle { get; set; }

        [JsonPropertyName("answer")]
        public string Answer { get; set; }

        [JsonPropertyName("reasoning")]
        public string Reasoning { get; set; }

        [JsonPropertyName("confidence")]
        public string Confidence { get; set; }

        [JsonPropertyName("participant_type")]
        public string ParticipantType { get; set; }

        [JsonPropertyName("detected_keywords")]
        public string[] DetectedKeywords { get; set; }
    }
}
