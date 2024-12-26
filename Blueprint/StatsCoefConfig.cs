using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class StatsCoefConfig
    {
        public long Id { get; set; }
        public long Level { get; set; }
        public decimal SkillCoef { get; set; }
        public decimal HealthPerHp { get; set; }
        // public Status Status { get; set; }
        public List<StatsCoefConfigMapping> StatsCoefConfigMappings { get; set; }
    }
}
