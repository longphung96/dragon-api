using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class MobStageFormation
    {
        public long Id { get; set; }
        public long StageId { get; set; }
        public long? Time { get; set; }
        public decimal MobPowerRate { get; set; }
        public decimal KeyRate { get; set; }
        //public long EnvironmentId { get; set; }
        // public long MobSealId { get; set; }
        // public long StatusId { get; set; }
        //public Environment Environment { get; set; }
        public Stage Stage { get; set; }
        // public Status Status { get; set; }
        public List<MobStageFormationItemContent> MobStageFormationItemContents { get; set; }
        public List<MobStageFormationMobMapping> MobStageFormationMobMappings { get; set; }
        // public List<MobStageFormationVersionMapping> MobStageFormationVersionMappings { get; set; }
        // public Guid RowId { get; set; }{ get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
