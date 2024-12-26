using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dragon.Blueprints
{
    public partial class MergeFragmentItemConfig
    {

        public long Id { get; set; }
        public long ItemReceivedId { get; set; }
        public decimal ItemReceivedAmount { get; set; }
        public long ItemConsumedId { get; set; }
        public decimal ItemConsumedAmount { get; set; }
        public long ItemMaterial { get; set; }
        public decimal ItemMaterialAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Status { get; set; } = true;
    }
}
