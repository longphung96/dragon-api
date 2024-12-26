using System;
using System.Linq;
using System.Text.Json.Serialization;
using DragonAPI.Models.Settings;
using MongoDB.Bson.Serialization.Attributes;
using DragonAPI.Configurations;

namespace DragonAPI.Common
{
    public class Stats
    {
        [JsonPropertyName("hp")]
        [BsonElement("hp")]
        public UInt32 HP { get; set; }
        [JsonPropertyName("speed")]
        [BsonElement("speed")]
        public UInt32 Speed { get; set; }
        [JsonPropertyName("skill")]
        [BsonElement("skill")]
        public UInt32 Skill { get; set; }
        [JsonPropertyName("morale")]
        [BsonElement("morale")]
        public UInt32 Morale { get; set; }
        [JsonPropertyName("synergy")]
        [BsonElement("synergy")]
        public UInt16 Synergy { get; set; }
        public static Stats operator *(Stats a, double coef)
        => new Stats
        {
            HP = (UInt32)(a.HP * coef),
            Speed = (UInt32)(a.Speed * coef),
            Skill = (UInt32)(a.Skill * coef),
            Morale = (UInt32)(a.Morale * coef),
            Synergy = a.Synergy,
        };
    }
    public class FullStats : Stats
    {
        [JsonPropertyName("heatlh")]
        public UInt64 Heatlh { get; set; }
        [JsonPropertyName("crit_rate")]
        public double CritRate { get; set; }
        [JsonPropertyName("crit_damage")]
        public double CritDamage { get; set; }
        [JsonPropertyName("damage")]
        public double Damage { get; set; }
        public static FullStats FromBaseStats(Stats stats, GameConfigs gameConfig, int rongosLevel)
        {
            DragonAPI.Configurations.StatCoef coef = gameConfig.StatCoef;
            var statsCoef = gameConfig.StatsCoefConfigs.FirstOrDefault(c => c.Level == rongosLevel);
            var fullStats = new FullStats();
            fullStats.HP = stats.HP;
            fullStats.Speed = stats.Speed;
            fullStats.Skill = stats.Skill;
            fullStats.Morale = stats.Morale;
            fullStats.Synergy = stats.Synergy;
            fullStats.Damage = fullStats.Skill * (statsCoef != null ? statsCoef.SkillCoef : coef.SkillCoef);
            fullStats.Heatlh = (ulong)(fullStats.HP * (statsCoef != null ? (uint)statsCoef.HealthPerHp : coef.HealthPerHp));
            fullStats.CritRate = fullStats.Morale * coef.CritRatePerMorale;
            fullStats.CritDamage = coef.CritDamageBase + coef.ExtraCritDamageUnit * fullStats.Morale / coef.NumOfMoraleExtraCritDamage;
            return fullStats;
        }

        public static FullStats operator *(FullStats a, double coef)
        => new FullStats
        {
            HP = (UInt32)(a.HP * coef),
            Speed = (UInt32)(a.Speed * coef),
            Skill = (UInt32)(a.Skill * coef),
            Morale = (UInt32)(a.Morale * coef),
            Synergy = a.Synergy,
            CritRate = (UInt32)(a.CritRate * coef),
            CritDamage = (UInt32)(a.CritDamage * coef),
        };
        public FullStats Extra(BuffStats buffStats, StatCoef coef)
        {
            var finalStats = new FullStats();
            finalStats.HP = (uint)(100 + buffStats.HP + buffStats.AllStats) * this.HP / 100;
            finalStats.Morale = (uint)(100 + buffStats.Morale + buffStats.AllStats) * this.Morale / 100;
            finalStats.Speed = (uint)(100 + buffStats.Speed + buffStats.AllStats) * this.Speed / 100;
            finalStats.Skill = (uint)(100 + buffStats.Skill + buffStats.AllStats) * this.Skill / 100;
            finalStats.Damage = (100 + buffStats.Damage) * (finalStats.Skill * coef.SkillCoef) / 100;
            finalStats.Heatlh = (ulong)finalStats.HP * (ulong)coef.HealthPerHp;
            finalStats.CritRate = buffStats.CritRate + finalStats.Morale * coef.CritRatePerMorale;
            finalStats.CritDamage = buffStats.CritDamage + coef.CritDamageBase + coef.ExtraCritDamageUnit * (int)(finalStats.Morale / coef.NumOfMoraleExtraCritDamage);
            return finalStats;
        }
        public void AddBuffStats(BuffStats buffStats, StatCoef coef)
        {
            this.HP = (uint)((100 + buffStats.HP + buffStats.AllStats) * this.HP / 100 + buffStats.HPValue);
            this.Morale = (uint)((100 + buffStats.Morale + buffStats.AllStats) * this.Morale / 100 + buffStats.Morale);
            this.Speed = (uint)((100 + buffStats.Speed + buffStats.AllStats) * this.Speed / 100 + buffStats.Speed);
            this.Skill = (uint)((100 + buffStats.Skill + buffStats.AllStats) * this.Skill / 100 + buffStats.Skill);
            this.Damage = (100 + buffStats.Damage) * (this.Skill * coef.SkillCoef) / 100 + buffStats.DamageValue;
            this.Heatlh = (ulong)(this.HP * coef.HealthPerHp);
            this.CritRate = buffStats.CritRate + this.Morale * coef.CritRatePerMorale;
            this.CritDamage = buffStats.CritDamage + coef.CritDamageBase + coef.ExtraCritDamageUnit * this.Morale / coef.NumOfMoraleExtraCritDamage;
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
        public double HPValue { get; set; }
        public double SkillValue { get; set; }
        public double MoraleValue { get; set; }
        public double SpeedValue { get; set; }
        public double DamageValue { get; set; }
    }
    public class StatCoef
    {
        public double SkillCoef { get; set; }
        public double HealthPerHp { get; set; }
        public double CritRatePerMorale { get; set; }
        public double CritDamageBase { get; set; }
        public double ExtraCritDamageUnit { get; set; }
        public double NumOfMoraleExtraCritDamage { get; set; }

    }
}
