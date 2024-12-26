using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dragon.Blueprints
{
    public partial class DragonUplevel
    {
        public long Id { get; set; }
        public long DragonElementId { get; set; }
        public long Rss { get; set; }
        public long DragonExp { get; set; }
        public long NextLevelUpdate { get; set; }
        public decimal CoeffUpdate { get; set; }
        public long? AmountScroll { get; set; }
        public long? AmountDust { get; set; }
        public long? BodyPartTypeUpdate { get; set; }
        public long? BodyPartLevelUpdate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public virtual DragonElement DragonElement { get; set; }
        public bool Status { get; set; } = true;
    }
}
