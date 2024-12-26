using System.Collections.Generic;
using System.Linq;
using DragonAPI.Enums;

namespace DragonAPI.Common
{
    public interface IGemSealAttributeEffect : IEffect
    {
        void Activate(List<Dragon> rongoses, Dragon.DragonCalculatorConfigs config);
    }

    public interface IGemSealAttributeBuffStatsEffect : IBuffStatsEffect, IGemSealAttributeEffect { }

    public class GemSealAttributeBuffStatsEffectImpl : IGemSealAttributeBuffStatsEffect
    {
        public BuffStats[] BuffStats { get; set; }
        public ClassType[] AfftectedClassGroups { get; set; }
        public void Activate(List<Dragon> rongoses, Dragon.DragonCalculatorConfigs config)
        {
            var affectedDragones = rongoses.FindAll(r => this.AfftectedClassGroups.Contains(r.Class));
            affectedDragones.ForEach(r =>
            {
                r.BuffedStats += this.BuffStats[0];
            });
        }
    }
}