using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class OfflineReward
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long Type { get; set; }
        public long ItemId { get; set; }
        public long DropItemPercent { get; set; }
        public decimal Rate { get; set; }
        public long MinTimeRequire { get; set; }
        public long MaxTimeRequire { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Status { get; set; } = true;
        public virtual Item Item { get; set; }
    }
}
