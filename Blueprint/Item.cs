using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class Item
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public long TypeId { get; set; }
        public string EntityType { get; set; }
        //public long? SealSlot { get; set; }
        public bool IsNFT { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
    }
}
