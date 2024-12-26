using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dragon.Blueprints
{
    public partial class DragonUpgrade
    {
        public long Id { get; set; }
        public long DragonElementId { get; set; }
        public long NextLevelUpdate { get; set; }
        public long? ItemId1 { get; set; }
        public long? AmountItem1 { get; set; }
        public long? ItemId2 { get; set; }
        public long? AmountItem2 { get; set; }
        public long? ItemId3 { get; set; }
        public long? AmountItem3 { get; set; }
        public long? BodyPartTypeUpdate { get; set; }
        public long? BodyPartLevelUpdate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public virtual DragonElement DragonElement { get; set; }
        public bool Status { get; set; } = true;
    }
}
