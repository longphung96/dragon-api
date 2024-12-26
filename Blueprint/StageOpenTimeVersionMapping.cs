using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class StageOpenTimeVersionMapping
    {
        public long StageOpenTimeId { get; set; }
        public long VersionId { get; set; }
        public StageOpenTime StageOpenTime { get; set; }
        public Version Version { get; set; }
    }
}
