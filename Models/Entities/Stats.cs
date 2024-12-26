using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using DragonAPI.Configurations;
using DragonAPI.Models.Settings;

namespace DragonAPI.Models.Entities
{
    public class Stats
    {
        [JsonPropertyName("hp")]
        public double HP { get; set; }
        [JsonPropertyName("speed")]
        public double Speed { get; set; }
        [JsonPropertyName("skill")]
        public double Skill { get; set; }
        [JsonPropertyName("morale")]
        public double Morale { get; set; }
        [JsonPropertyName("synergy")]
        public double Synergy { get; set; }
        public static Stats operator *(Stats a, double coef)
        => new Stats
        {
            HP = (a.HP * coef),
            Speed = (a.Speed * coef),
            Skill = (a.Skill * coef),
            Morale = (a.Morale * coef),
            Synergy = a.Synergy * coef,
        };
        public static Stats operator +(Stats a, Stats b)
        => new Stats
        {
            HP = a.HP + b.HP,
            Speed = a.Speed + b.Speed,
            Skill = a.Skill + b.Skill,
            Morale = a.Morale + b.Morale,
            Synergy = a.Synergy + b.Synergy,
        };

    }
    public class FullStats : Stats
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
        public static FullStats BuildFromStats(Stats stats, StatCoef coef)
        {
            var fullStats = new FullStats();
            fullStats.HP = stats.HP;
            fullStats.Speed = stats.Speed;
            fullStats.Skill = stats.Skill;
            fullStats.Morale = stats.Morale;
            fullStats.Synergy = stats.Synergy;
            fullStats.Damage = fullStats.Skill * coef.SkillCoef;
            fullStats.Heatlh = fullStats.HP * coef.HealthPerHp;
            fullStats.CritRate = fullStats.Morale * coef.CritRatePerMorale;
            fullStats.CritDamage = coef.CritDamageBase + coef.ExtraCritDamageUnit * fullStats.Morale / coef.NumOfMoraleExtraCritDamage;
            fullStats.Cp = stats.Speed + stats.Morale + stats.HP + (fullStats.Morale * coef.CritRatePerMorale * 10) + stats.Skill;
            return fullStats;
        }
        public static FullStats FromBaseStats(Stats stats, GameConfigs gameConfig, int rongosLevel)
        {
            StatCoef coef = gameConfig.StatCoef;
            var statsCoef = gameConfig.StatsCoefConfigs.FirstOrDefault(c => c.Level == rongosLevel);
            var fullStats = new FullStats();
            fullStats.HP = stats.HP;
            fullStats.Speed = stats.Speed;
            fullStats.Skill = stats.Skill;
            fullStats.Morale = stats.Morale;
            fullStats.Synergy = stats.Synergy;
            fullStats.Damage = fullStats.Skill * (statsCoef != null ? statsCoef.SkillCoef : coef.SkillCoef);
            fullStats.Heatlh = fullStats.HP * (statsCoef != null ? (uint)statsCoef.HealthPerHp : coef.HealthPerHp);
            fullStats.CritRate = fullStats.Morale * coef.CritRatePerMorale;
            fullStats.CritDamage = coef.CritDamageBase + coef.ExtraCritDamageUnit * fullStats.Morale / coef.NumOfMoraleExtraCritDamage;
            return fullStats;
        }
    }

    public class BuffStats
    {
        public double HP { get; set; }
        public double Skill { get; set; }
        public double Morale { get; set; }
        public double Speed { get; set; }
        public double CritRate { get; set; }
        public double CritDamage { get; set; }
        public double Damage { get; set; }
        public double AllStats { get; set; }
        public static BuffStats operator +(BuffStats a, BuffStats b)
        => new BuffStats
        {
            HP = a.HP + b.HP,
            Speed = a.Speed + b.Speed,
            Skill = a.Skill + b.Skill,
            Morale = a.Morale + b.Morale,
            AllStats = a.AllStats + b.AllStats,
            CritRate = a.CritRate + b.CritRate,
            CritDamage = a.CritDamage + b.CritDamage,
            Damage = a.Damage + b.Damage,
        };
    }
}
