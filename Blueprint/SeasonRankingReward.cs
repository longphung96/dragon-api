using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dragon.Blueprints
{
    public partial class SeasonRankingReward
    {

        public long Id { get; set; }
        public string SeasonId { get; set; }
        public long RankFrom { get; set; }
        public long RankTo { get; set; }
        public long ItemId { get; set; }
        public double Quantity { get; set; }
        public bool Status { get; set; } = true;
        public virtual Item Item { get; set; }
    }
}
