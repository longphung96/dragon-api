using MongoDB.Bson.Serialization.Attributes;
using DragonAPI.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DragonAPI.Models.Entities
{
    public class Bodypart
    {
        [BsonElement("level")]
        [JsonPropertyName("level")]
        public int Level { get; set; } = 1;
        [BsonElement("type")]
        [JsonPropertyName("type")]
        public BodypartType Type { get; set; }
        [BsonElement("class")]
        [JsonPropertyName("class")]
        public long Class { get; set; }
        [BsonElement("mutant_type")]
        [JsonPropertyName("mutant_type")]
        public MutantType MutantType { get; set; }
        [BsonElement("unlocked")]
        [JsonPropertyName("unlocked")]
        public bool Unlocked { get; set; } = false;
        [BsonElement("unlockState")]
        [JsonPropertyName("unlockState")]
        public BodypartUnlockState UnlockState { get; set; } = BodypartUnlockState.Locked;
        [BsonElement("skill_unlocked")]
        [JsonPropertyName("skill_unlocked")]
        public bool SkillUnlocked { get; set; } = false;
        [BsonElement("genes")]
        [JsonPropertyName("genes")]
        public List<Gene> Genes { get; set; }
    }

    public enum BodypartUnlockState
    {
        Locked,
        Unlocking,
        Unlocked
    }

    public class Gene
    {
        public long Class { get; set; }
        public double Value { get; set; }
    }
}
