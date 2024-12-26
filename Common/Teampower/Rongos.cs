using System;
using System.Collections.Generic;
using System.Linq;
using DragonAPI.Configurations;
using DragonAPI.Enums;
using DragonAPI.Models.Entities;

namespace DragonAPI.Common
{

    public class Dragon
    {
        public string Id { get; set; }
        public string WalletId { get; set; }
        public string NftId { get; set; }
        public UInt16 Level { get; set; } = 1;
        public int RaceLevelUnlocked { get; set; } = 0;
        public int ActivatedRaceLevel { get; set; } = 0;
        public ClassType Class { get; set; }
        public FullStats Stats { get; set; }
        public BuffStats BuffedStats { get; set; } = new BuffStats { };
        public List<Bodypart> Bodyparts { get; set; } = new List<Bodypart>();
        public int HappyPoint { get; set; }
        public void ActivePassiveSkill(List<Dragon> rongoses, DragonCalculatorConfigs configs)
        {
            var skinPart = this.Bodyparts.Find(bp => bp.Type == BodypartType.Skin && bp.Unlocked);
            if (skinPart != null)
            {
                var skinPartEffects = BodypartSkinEffectFactory.CreateEffects(skinPart.Class, skinPart.Level, configs);
                skinPartEffects.ForEach(eff =>
                {
                    eff.Activate(rongoses, configs);
                });
            }
        }
        public int FindActivatedRaceLevel(List<Dragon> rongoses, DragonCalculatorConfigs configs)
        {
            var activatedRaceLevel = 0;
            if (this.RaceLevelUnlocked > 0)
            {
                var raceLevelConfigs = configs.RaceLevelConfigs.FindAll(rl => rl.RaceLevel <= this.RaceLevelUnlocked && rl.Class == configs.ClassGroupMap[this.Class]).OrderByDescending(r => r.RaceLevel).ToList();
                if (raceLevelConfigs.Count > 0)
                {
                    foreach (var raceLevelConfig in raceLevelConfigs)
                    {
                        var rongosesTeamMate = rongoses.FindAll(r => r.Id != this.Id);
                        var classCountGroups = raceLevelConfig.RequiredClasses.GroupBy(cg => cg).ToList();
                        var raceValid = true;
                        foreach (var ccg in classCountGroups)
                        {
                            if (ccg.Count() != rongosesTeamMate.Count(r => configs.ClassGroupMap[r.Class] == ccg.Key))
                            {
                                raceValid = false;
                                break;
                            }
                        }
                        if (raceValid)
                        {
                            activatedRaceLevel = raceLevelConfig.RaceLevel;
                            break;
                        }
                    }
                }
            }
            return activatedRaceLevel;
        }
        public void ActiveRace(List<Dragon> rongoses, DragonCalculatorConfigs configs)
        {
            var activatedRaceLevel = FindActivatedRaceLevel(rongoses, configs);
            if (activatedRaceLevel > 0)
            {
                var raceConf = configs.RaceLevelConfigs.Find(rl => rl.RaceLevel == activatedRaceLevel && rl.Class == configs.ClassGroupMap[this.Class]);
                this.ActivatedRaceLevel = raceConf.RaceLevel;
                this.BuffedStats += raceConf.BuffStats;
            }
        }
        public class DragonCalculatorConfigs
        {
            public StatCoef StatCoef { get; set; } = new StatCoef();
            public Dictionary<ClassType, ClassType> ClassGroupMap { get; set; } = new Dictionary<ClassType, ClassType>();
            public List<RaceLevelConfig> RaceLevelConfigs { get; set; } = new List<RaceLevelConfig>();
            public List<SkillConfig> ConfiguredSkillLevels { get; private set; } = new List<SkillConfig>();
            public Dictionary<int, double> GemAffected { get; set; }

            public void SetSkillLevelConfigs(List<SkillConfig> skillLevelConfigs)
            {
                this.ConfiguredSkillLevels = skillLevelConfigs;
            }
            public void SetSealSocketPowerLevelConfigs()
            {
                GemAffected = gemSealAttributeAffectedConfig();
            }

            public DragonCalculatorConfigs() { }

            private Dictionary<int, double> gemSealAttributeAffectedConfig()
            {
                var sealGemAttributeConfigs = new Dictionary<int, double>();
                sealGemAttributeConfigs.Add(0, 100);
                sealGemAttributeConfigs.Add(1, 75);
                sealGemAttributeConfigs.Add(2, 25);
                return sealGemAttributeConfigs;
            }
        }
        public class SkillConfig
        {
            public int Level { get; set; }
            public ClassType Class { get; set; }
            public List<List<BuffStats>> ListOfMutantBuffStats { get; set; } = new List<List<BuffStats>> { };
            public ClassType[] AfftectedClassGroups { get; set; }
        }
    }
}