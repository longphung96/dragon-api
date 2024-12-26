using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class BodypartStats
    {
        public long Id { get; set; }
        public long BodypartTypeId { get; set; }
        public long DragonElementId { get; set; }
        public decimal Hp { get; set; }
        public decimal Speed { get; set; }
        public decimal Skill { get; set; }
        public decimal Morale { get; set; }
        public decimal Synergy { get; set; }
        //// public long StatusId { get; set; }
        public BodypartType BodypartType { get; set; }
        public DragonElement DragonElement { get; set; }
        //public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
