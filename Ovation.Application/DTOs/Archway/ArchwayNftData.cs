using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Archway
{

    public class ArchwayNftData
    {
        [JsonPropertyName("data")]
        public Data Data { get; set; }
    }

    public class Access
    {
        [JsonPropertyName("owner")]
        public string Owner { get; set; }

        [JsonPropertyName("approvals")]
        public List<object> Approvals { get; set; }
    }

    public class Attribute
    {
        [JsonPropertyName("trait_type")]
        public string TraitType { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class Data
    {
        [JsonPropertyName("access")]
        public Access Access { get; set; }

        [JsonPropertyName("info")]
        public Info Info { get; set; }
    }

    public class Extension
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("attributes")]
        public List<Attribute> Attributes { get; set; }
    }

    public class Info
    {
        [JsonPropertyName("token_uri")]
        public object TokenUri { get; set; }

        [JsonPropertyName("extension")]
        public Extension Extension { get; set; }
    }


}
