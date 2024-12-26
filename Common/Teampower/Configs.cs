// using System.Collections.Generic;
// using DragonAPI.Configurations;
// using DragonAPI.Enums;

// namespace DragonAPI.Utils
// {
//     // public class Stats
//     // {
//     //     public double HP { get; set; }
//     //     public double Speed { get; set; }
//     //     public double Skill { get; set; }
//     //     public double Morale { get; set; }
//     //     public static Stats operator *(Stats a, double coef)
//     //     => new Stats
//     //     {
//     //         HP = (a.HP * coef),
//     //         Speed = (a.Speed * coef),
//     //         Skill = (a.Skill * coef),
//     //         Morale = (a.Morale * coef),
//     //     };
//     // }
//     // public class FullStats : Stats
//     // {
//     //     public double Heatlh { get; set; }
//     //     public double CritRate { get; set; }
//     //     public double CritDamage { get; set; }
//     //     public double Damage { get; set; }
//     //     public static FullStats operator *(FullStats a, double coef)
//     //     => new FullStats
//     //     {
//     //         HP = (a.HP * coef),
//     //         Speed = (a.Speed * coef),
//     //         Skill = (a.Skill * coef),
//     //         Morale = (a.Morale * coef),
//     //         CritRate = (a.CritRate * coef),
//     //         CritDamage = (a.CritDamage * coef),
//     //     };
//     //     public void AddBuffStats(BuffStats buffStats, StatCoef coef)
//     //     {
//     //         this.HP = (uint)(100 + buffStats.HP + buffStats.AllStats) * this.HP / 100 + buffStats.HPValue;
//     //         this.Morale = (uint)(100 + buffStats.Morale + buffStats.AllStats) * this.Morale / 100 + buffStats.Morale;
//     //         this.Speed = (uint)(100 + buffStats.Speed + buffStats.AllStats) * this.Speed / 100 + buffStats.Speed;
//     //         this.Skill = (uint)(100 + buffStats.Skill + buffStats.AllStats) * this.Skill / 100 + buffStats.Skill;
//     //         this.Damage = (100 + buffStats.Damage) * (this.Skill * coef.SkillCoef) / 100 + buffStats.DamageValue;
//     //         this.Heatlh = this.HP * coef.HealthPerHp;
//     //         this.CritRate = buffStats.CritRate + this.Morale * coef.CritRatePerMorale;
//     //         this.CritDamage = buffStats.CritDamage + coef.CritDamageBase + coef.ExtraCritDamageUnit * this.Morale / coef.NumOfMoraleExtraCritDamage;
//     //     }
//     //     public static FullStats FromBaseStats(Stats stats, GameConfigs gameConfig, int rongosLevel)
//     //     {
//     //         DragonAPI.Configurations.StatCoef coef = gameConfig.StatCoef;
//     //         var statsCoef = gameConfig.StatsCoefConfigs.FirstOrDefault(c => c.Level == rongosLevel);
//     //         var fullStats = new FullStats();
//     //         fullStats.HP = stats.HP;
//     //         fullStats.Speed = stats.Speed;
//     //         fullStats.Skill = stats.Skill;
//     //         fullStats.Morale = stats.Morale;
//     //         fullStats.Synergy = stats.Synergy;
//     //         fullStats.Damage = fullStats.Skill * (statsCoef != null ? statsCoef.SkillCoef : coef.SkillCoef);
//     //         fullStats.Heatlh = fullStats.HP * (statsCoef != null ? (uint)statsCoef.HealthPerHp : coef.HealthPerHp);
//     //         fullStats.CritRate = fullStats.Morale * coef.CritRatePerMorale;
//     //         fullStats.CritDamage = coef.CritDamageBase + coef.ExtraCritDamageUnit * fullStats.Morale / coef.NumOfMoraleExtraCritDamage;
//     //         return fullStats;
//     //     }
//     // }

// // public class BuffStats
// // {
// //     // percentage
// //     public double HP { get; set; }
// //     public double Skill { get; set; }
// //     public double Morale { get; set; }
// //     public double Speed { get; set; }
// //     public double CritRate { get; set; }
// //     public double CritDamage { get; set; }
// //     public double Damage { get; set; }
// //     public double AllStats { get; set; }

// //     // value
// //     public double HPValue { get; set; }
// //     public double SkillValue { get; set; }
// //     public double MoraleValue { get; set; }
// //     public double SpeedValue { get; set; }
// //     public double DamageValue { get; set; }


// //     public static BuffStats operator +(BuffStats a, BuffStats b)
// //     => new BuffStats
// //     {
// //         HP = a.HP + b.HP,
// //         Speed = a.Speed + b.Speed,
// //         Skill = a.Skill + b.Skill,
// //         Morale = a.Morale + b.Morale,
// //         AllStats = a.AllStats + b.AllStats,
// //         CritRate = a.CritRate + b.CritRate,
// //         CritDamage = a.CritDamage + b.CritDamage,
// //         Damage = a.Damage + b.Damage,
// //         DamageValue = a.DamageValue + b.DamageValue,
// //         HPValue = a.HPValue + b.HPValue,
// //         MoraleValue = a.MoraleValue + b.MoraleValue,
// //         SpeedValue = a.SpeedValue + b.SpeedValue,
// //         SkillValue = a.SkillValue + b.SkillValue,
// //     };
// // }




// // public enum SealSlotPowerType
// // {
// //     Attack,
// //     BuffOrDebuff,
// //     ElementUp,
// //     StatsUp,
// //     Undefined = 1000,
// // }


// // public class RaceLevelConfig
// // {
// //     public ClassType Class { get; set; }
// //     public int RaceLevel { get; set; }
// //     public ClassType[] RequiredClasses { get; set; }
// //     public BuffStats BuffStats { get; set; }
// // }

// // public class BodypartSkinBuffStatsLevelConfig
// // {
// //     public int Level { get; set; }
// //     public ClassType Class { get; set; }
// //     public List<List<BuffStats>> ListOfMutantBuffStats { get; set; }
// //     public ClassType[] AfftectedClassGroups { get; set; }
// // }

// // public class SealSocketBuffStatsLevelConfig
// // {
// //     public int Level { get; set; }
// //     public SealSlotPowerType Power { get; set; }
// //     public ClassType ClassGroup { get; set; }
// //     public BuffStats[] BuffStats { get; set; }
// //     public ClassType[] AfftectedClassGroups { get; set; }
// // }

// // public class SkillConfig
// // {
// //     public int Level { get; set; }
// //     public ClassType Class { get; set; }
// //     public List<List<BuffStats>> ListOfMutantBuffStats { get; set; } = new List<List<BuffStats>> { };
// //     public ClassType[] AfftectedClassGroups { get; set; }
// // }
// // public class StatCoef
// // {
// //     public double SkillCoef { get; set; }
// //     public double HealthPerHp { get; set; }
// //     public double CritRatePerMorale { get; set; }
// //     public double CritDamageBase { get; set; }
// //     public double ExtraCritDamageUnit { get; set; }
// //     public double NumOfMoraleExtraCritDamage { get; set; }
// // }
// // public class DragonCalculatorConfigs
// // {
// //     public StatCoef StatCoef { get; set; }
// //     public Dictionary<ClassType, ClassType> ClassGroupMap { get; set; } = new Dictionary<ClassType, ClassType>();
// //     public List<RaceLevelConfig> RaceLevelConfigs { get; set; } = new List<RaceLevelConfig>();
// //     public List<SkillConfig> ConfiguredSkillLevels { get; private set; } = new List<SkillConfig>();
// //     public Dictionary<int, double> GemAffected { get; set; }

// //     public void SetSkillLevelConfigs(List<SkillConfig> skillLevelConfigs)
// //     {
// //         this.ConfiguredSkillLevels = skillLevelConfigs;
// //     }
// //     public void SetSealSocketPowerLevelConfigs()
// //     {
// //         GemAffected = gemSealAttributeAffectedConfig();
// //     }

// //     public DragonCalculatorConfigs() { }

// //     private Dictionary<int, double> gemSealAttributeAffectedConfig()
// //     {
// //         var sealGemAttributeConfigs = new Dictionary<int, double>();
// //         sealGemAttributeConfigs.Add(0, 100);
// //         sealGemAttributeConfigs.Add(1, 75);
// //         sealGemAttributeConfigs.Add(2, 25);
// //         return sealGemAttributeConfigs;
// //     }
// // }
// // }