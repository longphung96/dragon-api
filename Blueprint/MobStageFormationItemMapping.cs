using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class MobStageFormationItemMapping
    {
        public long MobStageFormationId { get; set; }
        public long ItemId { get; set; }
        public Item Item { get; set; }
        public MobStageFormation MobStageFormation { get; set; }
    }
}
