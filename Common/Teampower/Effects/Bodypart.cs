using System.Collections.Generic;
using System.Linq;
using DragonAPI.Utils;
using DragonAPI.Enums;


namespace DragonAPI.Common
{
    public interface IBodypartSkillEffect : IEffect
    {
        void Activate(List<Dragon> rongoses, Dragon.DragonCalculatorConfigs config);
    }

    public interface IBodypartSkinBuffStatsEffect : IBuffStatsEffect, IBodypartSkillEffect { }

    public class BodypartSkinBuffStatsEffectImpl : IBodypartSkinBuffStatsEffect
    {
        public BuffStats[] BuffStats { get; set; }
        public ClassType[] AfftectedClassGroups { get; set; }
        public void Activate(List<Dragon> rongoses, Dragon.DragonCalculatorConfigs config)
        {
            var affectedDragones = rongoses.FindAll(r => this.AfftectedClassGroups.Contains(config.ClassGroupMap[r.Class]));
            affectedDragones.ForEach(r =>
            {
                r.BuffedStats += this.BuffStats[0];
            });
        }
    }
}