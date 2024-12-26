using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class DragonSkillConfig
    {
        public long Id { get; set; }
        public long BodypartTypeId { get; set; }
        public long DragonElementId { get; set; }
        public long FamilyId { get; set; }
        public string Level1 { get; set; }
        public string Level2 { get; set; }
        public string Level3 { get; set; }
        public string Level4 { get; set; }
        public long EnemySynergy { get; set; }
        public long SelfSynergy { get; set; }
        public long AllySynergy { get; set; }
        // public long StatusId { get; set; }
        public BodypartType BodypartType { get; set; }
        public DragonElement DragonElement { get; set; }
        public Family Family { get; set; }
        // public Status Status { get; set; }
        //public List<DragonSkillConfigVersionMapping> DragonSkillConfigVersionMappings { get; set; }
        // public Guid RowId { get; set; }{ get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
