using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dragon.Blueprints
{
    public partial class DragonMergeFragmentConfig
    {

        public long Id { get; set; }
        public long DragonElementId { get; set; }
        public long Rss { get; set; }
        public long ItemId { get; set; }
        public long ItemQuantity { get; set; }
        public long? AmountScroll { get; set; }
        public long? AmountDust { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public virtual DragonElement DragonElement { get; set; }
        public bool Status { get; set; } = true;
    }
}
