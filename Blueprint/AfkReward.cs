using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class AfkReward
    {
        public long Id { get; set; }
        public long StageId { get; set; }
        public long Type { get; set; }
        public long ItemId { get; set; }
        public decimal Quantity { get; set; }
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
