using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Dragon.Blueprints;
using DragonAPI.Enums;

namespace DragonAPI.Models.DTOs
{
    public class MapDTO : BaseDTO
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("index")]
        public UInt16 Index { get; set; }
        [JsonPropertyName("requiredLevel")]
        public UInt16 RequiredLevel { get; set; }
        [JsonPropertyName("unlocked")]
        public bool Unlocked { get; set; } = false;
    }
    public class StageDTO : BaseDTO
    {
        [JsonPropertyName("parent")]
        public string Parent { get; set; }
        [JsonPropertyName("index")]
        public UInt16 Index { get; set; }
        [JsonPropertyName("energy")]
        public UInt16? Energy { get; set; }
        [JsonPropertyName("requiredLevel")]
        public UInt16 RequiredLevel { get; set; }
        [JsonPropertyName("mobCount")]

        public UInt16 MobCount { get; set; }
        [JsonPropertyName("MobIds")]
        public List<string> MobIds { get; set; }
        [JsonPropertyName("Mobs")]
        public List<MobStageDTO> Mobs { get; set; }
        [JsonPropertyName("DropItemIds")]
        public List<string> DropItemIds { get; set; }
        [JsonPropertyName("rss")]
        public UInt16 Rss { get; set; }
        [JsonPropertyName("exp")]
        public int Exp { get; set; }
        [JsonPropertyName("unlocked")]
        public bool Unlocked { get; set; } = false;
        [JsonPropertyName("boss_stage")]
        public bool BossStage { get; set; }
    }
    public class LastStageTimeOpenInfoDTO
    {
        [JsonPropertyName("hourUTC")]
        public int HourUTC { get; set; }
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("remaining")]
        public int Remaining { get; set; }
    }
    public class MasterStageTrackingDTO
    {
        [JsonPropertyName("best_time")]
        public UInt16 BestTime { get; set; }
    }
    public class MobStageDTO : BaseDTO
    {
        [JsonPropertyName("class")]
        public ClassType Class { get; set; }
        public ushort Level { get; set; }
        public List<BodypartDTO> Bodyparts { get; set; }
        public FullStatsDto Stats { get; set; }
        public bool IsBossMob { get; set; }
        public long IndexInFormation { get; set; }
    }
}