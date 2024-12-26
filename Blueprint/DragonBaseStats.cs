using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class DragonBaseStats
    {
        public long Id { get; set; }
        public long DragonElementId { get; set; }
        public int Rarity { get; set; }
        public decimal BaseHp { get; set; }
        public decimal BaseSpeed { get; set; }
        public decimal BaseSkill { get; set; }
        public decimal BaseMorale { get; set; }
        public decimal BaseSynergy { get; set; }
        public decimal IncreaHp { get; set; }
        public decimal IncreaSpeed { get; set; }
        public decimal IncreaSkill { get; set; }
        public decimal IncreaMorale { get; set; }
        public decimal IncreaSynergy { get; set; }
        // // public long StatusId { get; set; }
        public DragonElement DragonElement { get; set; }
        //public bool Status { get; set; }
        // public List<DragonBaseStatsVersionMapping> DragonBaseStatsVersionMappings { get; set; }
        //  // public Guid RowId { get; set; }{ get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
