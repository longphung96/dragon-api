using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class BodypartLevel
    {
        public long Id { get; set; }
        public long BodypartTypeId { get; set; }
        public long Level { get; set; }
        public decimal Rss { get; set; }
        public long RequiredLevel { get; set; }
        //// public long StatusId { get; set; }
        public BodypartType BodypartType { get; set; }
        //public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
