using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class MobBodypartTypeMapping
    {
        public long MobId { get; set; }
        public long BodypartTypeId { get; set; }
        public long DragonElementId { get; set; }

        public long Level { get; set; }
        public BodypartType BodypartType { get; set; }
        public DragonElement DragonElement { get; set; }
        public Mob Mob { get; set; }

    }
}
