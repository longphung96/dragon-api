using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class TeamPvpMobFormationMobMapping
    {
        public long TeamPvpMobFormationId { get; set; }
        public long MobId { get; set; }
        public Mob Mob { get; set; }
        public long SlotInFormation { get; set; } = 0;
    }
}
