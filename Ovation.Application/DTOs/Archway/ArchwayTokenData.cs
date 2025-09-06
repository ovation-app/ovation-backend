using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Archway
{
    public class ArchwayTokenData
    {
        [JsonPropertyName("data")]
        public TokenData? Data { get; set; }
    }

    public class TokenData
    {
        [JsonPropertyName("tokens")]
        public List<string>? Tokens { get; set; }
    }
}
