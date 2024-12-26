using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class EnvironmentEnvironmentEffectMapping
    {
        public long EnvironmentId { get; set; }
        public long EnvironmentEffectId { get; set; }
        public Environment Environment { get; set; }
        public EnvironmentEffect EnvironmentEffect { get; set; }
    }
}
