using System.Collections.Generic;
using DragonAPI.Enums;

namespace DragonAPI.Common
{
    public interface IEffect
    {
    }

    public interface IBuffStatsEffect : IEffect
    {
        BuffStats[] BuffStats { get; set; }
        ClassType[] AfftectedClassGroups { get; set; }
    }
}