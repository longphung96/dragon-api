using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class SeasonReward
    {
        public long Id { get; set; }
        public string SeasonId { get; set; }
        public string TypeReward { get; set; }
        public string Tier { get; set; }
        public long PositionFrom { get; set; }
        public long PositionTo { get; set; }
        public long ItemId { get; set; }
        public long Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Status { get; set; } = true;
        public virtual Item Item { get; set; }
    }
}
