using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.X
{
    public class XUserDto
    {
        [JsonPropertyName("data")]
        public UserData? Data { get; set; }
    }

    public class UserData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
    }

}
