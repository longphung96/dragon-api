using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class Environment
    {
        public long Id { get; set; }
        public string Name { get; set; }
        // public long StatusId { get; set; }
        // public Status Status { get; set; }
        public List<EnvironmentEnvironmentEffectMapping> EnvironmentEnvironmentEffectMappings { get; set; }
        //public List<EnvironmentVersionMapping> EnvironmentVersionMappings { get; set; }
        // public Guid RowId { get; set; }{ get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
