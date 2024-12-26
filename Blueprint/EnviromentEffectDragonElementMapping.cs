using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class EnvironmentEffectDragonElementMapping
    {
        public long EnvironmentEffectId { get; set; }
        public long DragonElementId { get; set; }
        public DragonElement DragonElement { get; set; }
        public EnvironmentEffect EnvironmentEffect { get; set; }
    }
}
