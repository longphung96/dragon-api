using System;
using System.Text.Json.Serialization;

namespace DragonAPI.Models.DTOs
{
    public class BaseDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
    public class BaseWithTimeDTO : BaseDTO
    {
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}