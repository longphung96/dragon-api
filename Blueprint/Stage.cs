using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class Stage
    {
        public long Id { get; set; }
        public long MapId { get; set; }
        public long Energy { get; set; }
        public decimal ExpPerMob { get; set; }
        public long RSSPerMob { get; set; }
        public long MobCount { get; set; }
        public long RequiredLevel { get; set; }
        public long OrderNumber { get; set; }
        public bool IsLastStage { get; set; }
        // public long StatusId { get; set; }
        public Map Map { get; set; }
        // public Status Status { get; set; }
        //public List<StageVersionMapping> StageVersionMappings { get; set; }
        // public Guid RowId { get; set; }{ get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
