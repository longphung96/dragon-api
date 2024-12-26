using System;
using DragonAPI.Configurations;
using DragonAPI.Enums;
using DragonAPI.Models.DAOs;
using DragonAPI.Models.Entities;

namespace DragonAPI.Helpers
{
    public static class StatsCalculator
    {
        public static Stats BuildDragonStats(IDragonBase dragon, GameConfigs gameConfigs)
        {
            var stats = new Stats();
            //gameConfigs.DragonStatsConfigMap.TryGetValue(dragon.Class, out DragonStatsConfig statsConfig);
            var Stats = gameConfigs.DragonStatsConfigs.FirstOrDefault(r => r.Class == dragon.Class && r.Rarity == dragon.Rarity);
            uint coefficient = (uint)(dragon.Level - 1);
            stats = Stats.StartingStats + Stats.IncreaseStatsByLevel * coefficient;
            foreach (var bp in dragon.Bodyparts)
            {
                if (bp.Type == BodypartType.Wing || bp.Type == BodypartType.Eye || bp.Type == BodypartType.Ultimate)
                    continue;
                if (!bp.Unlocked)
                    continue;
                gameConfigs.BodypartConfigMap.TryGetValue(bp.Type, out BodypartTypeConfig bonusConfig);
                if (dragon.Level >= bonusConfig.RequiredLevel)
                {
                    bonusConfig.StatsByClassMap.TryGetValue((ClassType)bp.Class, out Stats statsByClass);
                    if (statsByClass == null)
                    {
                        System.Console.WriteLine($"ERROR: rongos id {dragon.Id}, {bp.Class.ToString()}");
                        continue;
                    }
                    uint bpcoef = (uint)(dragon.Level - bonusConfig.RequiredLevel + 1);
                    stats += statsByClass * bpcoef;
                }
            }

            return stats;
        }
    }

    public static class LevelCalculator
    {
        // public static (UInt16, ulong) AddMasterExp(UInt16 level, UInt64 currExp, UInt32 bonusExp, GameConfigs gameConfig)
        // {
        //     if (level >= gameConfig.LevelConfig.MaxMasterLevel)
        //         return ((ushort)gameConfig.LevelConfig.MaxMasterLevel, 0);
        //     var maxExpAtLevel = gameConfig.LevelConfig.MaxExpMaster[level - 1];
        //     var finalExp = (ulong)(currExp + bonusExp);
        //     while (finalExp >= maxExpAtLevel)
        //     {
        //         finalExp -= maxExpAtLevel;
        //         if (++level == gameConfig.LevelConfig.MaxMasterLevel)
        //         {
        //             finalExp = 0;
        //             break;
        //         }
        //         maxExpAtLevel = gameConfig.LevelConfig.MaxExpMaster[level - 1];
        //     }
        //     return (level, (ulong)finalExp);
        // }
        // public static (UInt16, double) AddDragonExp(UInt16 level, double currExp, double bonusExp, GameConfigs gameConfig)
        // {
        //     if (level >= gameConfig.LevelConfig.MaxDragonLevel)
        //         return ((ushort)gameConfig.LevelConfig.MaxDragonLevel, 0);
        //     var maxExpAtLevel = gameConfig.LevelConfig.MaxExpDragon[level - 1];
        //     var finalExp = (double)(currExp + bonusExp);
        //     while (finalExp >= maxExpAtLevel)
        //     {
        //         finalExp -= maxExpAtLevel;
        //         if (++level == gameConfig.LevelConfig.MaxDragonLevel)
        //         {
        //             finalExp = 0;
        //             break;
        //         }
        //         maxExpAtLevel = gameConfig.LevelConfig.MaxExpDragon[level - 1];
        //     }
        //     return (level, finalExp);
        // }
        public static (ushort, double) AddExp(double currExp, double bonusExp, long maxLevel, LevelExpConfiguration.LevelConfig levelConfig)
        {
            var finalExp = currExp + bonusExp;
            var expMaxLevelConfig = levelConfig.GetExpConfigAtLevel(maxLevel);
            var expC = levelConfig.GetExpConfigByExp(finalExp);
            if (expC.Level > expMaxLevelConfig.Level) expC = expMaxLevelConfig;
            if (finalExp >= expC.MaxExp) finalExp = expC.MaxExp;
            return ((ushort)expC.Level, finalExp);
        }
    }
}