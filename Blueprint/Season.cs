using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class Season
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        // public long StatusId { get; set; }
        // public Status Status { get; set; }
        //public List<SeasonVersionMapping> SeasonVersionMappings { get; set; }
        // public Guid RowId { get; set; }{ get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
