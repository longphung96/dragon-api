using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class ShopPackageConfig
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal BasePrice { get; set; }
        public decimal FinalPrice { get; set; }
        public long CurrencyId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Status { get; set; } = true;
        public virtual Item Item { get; set; }
    }
}
