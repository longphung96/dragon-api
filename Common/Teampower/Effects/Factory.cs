using System;
using System.Collections.Generic;
using System.Linq;
using DragonAPI.Enums;

namespace DragonAPI.Common
{
    public static class BodypartSkinEffectFactory
    {
        public static List<IBodypartSkillEffect> CreateEffects(long classType, int level, Dragon.DragonCalculatorConfigs configs)
        {
            var bodypartEffects = new List<IBodypartSkillEffect>();
            var partEffectConfig = configs.ConfiguredSkillLevels.Find(ssc => (long)ssc.Class == classType && ssc.Level == level);
            if (partEffectConfig != null)
            {
                partEffectConfig.ListOfMutantBuffStats.ForEach(mutantBuffStats =>
                {
                    bodypartEffects.Add(new BodypartSkinBuffStatsEffectImpl { BuffStats = mutantBuffStats.ToArray(), AfftectedClassGroups = partEffectConfig.AfftectedClassGroups });
                });
            }
            return bodypartEffects;
        }
    }
}