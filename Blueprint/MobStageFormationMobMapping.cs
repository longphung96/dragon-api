using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class MobStageFormationMobMapping
    {
        public long MobStageFormationId { get; set; }
        public long MobId { get; set; }
        public long SlotInFormation { get; set; } = 0;
        // public Mob Mob { get; set; }
        // public MobStageFormation MobStageFormation { get; set; }
    }
}
