using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class DragonElementGrouping
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<DragonElement> DragonElements { get; set; }
    }
}
