using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class StageOpenTime
    {
        public long Id { get; set; }
        public long StageId { get; set; }
        public long Time { get; set; }
        public long Amount { get; set; }
        // public long StatusId { get; set; }
        public Stage Stage { get; set; }
        // public Status Status { get; set; }
        //public List<StageOpenTimeVersionMapping> StageOpenTimeVersionMappings { get; set; }
        // public Guid RowId { get; set; }{ get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
