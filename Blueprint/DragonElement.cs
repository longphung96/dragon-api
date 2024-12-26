using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class DragonElement
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long DragonElementGroupingId { get; set; }
        public DragonElementGrouping DragonElementGrouping { get; set; }
    }
}
