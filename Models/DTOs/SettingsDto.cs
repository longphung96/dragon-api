using System;
using System.Text.Json.Serialization;

namespace DragonAPI.Models.DTOs
{
    public class SettingsDTO : BaseWithTimeDTO
    {
        [JsonPropertyName("bot_animation_enabled")]
        public bool BotAnimationEnabled { get; set; }
        [JsonPropertyName("sound")]
        public int Sound { get; set; }
        [JsonPropertyName("music")]
        public int Music { get; set; }
    }
    public class ChangeSettingRequest
    {

        public int Sound { get; set; }
        public int Music { get; set; }
        public bool BotAnimationEnabled { get; set; }
    }
}