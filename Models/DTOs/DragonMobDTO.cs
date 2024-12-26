using DragonAPI.Enums;
using DragonAPI.Models.DAOs;
using DragonAPI.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DragonAPI.Models.DTOs
{
    public class DragonMobDTO : IDragonBase
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("class")]
        public ClassType Class { get; set; }
        [JsonPropertyName("level")]
        public ushort Level { get; set; }
        [JsonPropertyName("rarity")]
        public int Rarity { get; set; } = 0;
        [JsonPropertyName("boss_skills")]
        public string[] BossSkills { get; set; }
        [JsonPropertyName("bodyparts")]
        public List<Bodypart> Bodyparts { get; set; }
    }
}
