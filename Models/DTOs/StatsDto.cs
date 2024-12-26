using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DragonAPI.Configurations;
using DragonAPI.Models.Entities;

namespace DragonAPI.Models.DTOs
{
    public class FullStatsDto : Stats
    {
        [JsonPropertyName("heatlh")]
        public double Heatlh { get; set; }
        [JsonPropertyName("cp")]
        public double Cp { get; set; }
        [JsonPropertyName("crit_rate")]
        public double CritRate { get; set; }
        [JsonPropertyName("crit_damage")]
        public double CritDamage { get; set; }
        [JsonPropertyName("damage")]
        public double Damage { get; set; }
    }
    public class StatsDto : Stats
    {
        [JsonPropertyName("heatlh")]
        public UInt64 Heatlh { get; set; }
        [JsonPropertyName("crit_rate")]
        public double CritRate { get; set; }
        [JsonPropertyName("crit_damage")]
        public double CritDamage { get; set; }
        [JsonPropertyName("damage")]
        public double Damage { get; set; }

        public static StatsDto operator *(StatsDto a, double coef)
        => new StatsDto
        {
            HP = (UInt32)(a.HP * coef),
            Speed = (UInt32)(a.Speed * coef),
            Skill = (UInt32)(a.Skill * coef),
            Morale = (UInt32)(a.Morale * coef),
            Synergy = a.Synergy,
        };
        public void ReBuildStats(StatCoef coef)
        {
            Damage = Skill * coef.SkillCoef;
            Heatlh = (ulong)(HP * coef.HealthPerHp);
            CritRate = Morale * coef.CritRatePerMorale;
            CritDamage = coef.CritDamageBase + coef.ExtraCritDamageUnit * Morale / coef.NumOfMoraleExtraCritDamage;
        }
    }
}