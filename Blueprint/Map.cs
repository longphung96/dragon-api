using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class Map
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long OrderNumber { get; set; }
        public long RequiredLevel { get; set; }
        // // public long StatusId { get; set; }
        // // public Status Status { get; set; }
        // public List<MapVersionMapping> MapVersionMappings { get; set; }
        //  // public Guid RowId { get; set; }{ get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
