using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class Version
    {
        public long Id { get; set; }
        public string VersionNumber { get; set; }
        public string Description { get; set; }
        // public long StatusId { get; set; }
        // public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
