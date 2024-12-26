using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class TeamPvpMobFormation
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal MobPowerRate { get; set; }
        public string RankPvP { get; set; }
        public bool Status { get; set; } = true;
        public List<TeamPvpMobFormationMobMapping> TeamPvpMobFormationMobMappings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
