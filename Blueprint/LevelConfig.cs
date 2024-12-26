using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class LevelConfig
    {
        public long Id { get; set; }
        public long Level { get; set; }
        public long LevelTypeId { get; set; }
        public long MinValue { get; set; }
        public long MaxValue { get; set; }
        public LevelType LevelType { get; set; }
    }
}
