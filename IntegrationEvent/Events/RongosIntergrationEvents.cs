using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DragonAPI.IntegrationEvent.Events
{
    public class DragonLevelUpdatedETO
    {
        public string id { get; set; }
        public ulong tokenId { get; set; }
        public long level { get; set; }
    }


    public class DragonWorkplaceClaimedRssETO
    {
        public string MainUserId { get; set; }
        public double Rss { get; set; }
    }
}
