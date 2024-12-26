using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class DragonSkillEffect
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long SkillEffectTypeId { get; set; }
        public string Round { get; set; }
        public string Value { get; set; }
        public string Value2 { get; set; }
        public string Rate { get; set; }
        public long TypeEffectId { get; set; }
        // public long StatusId { get; set; }
        public SkillEffectType SkillEffectType { get; set; }
        // public Status Status { get; set; }
        public TypeEffect TypeEffect { get; set; }
        // public Guid RowId { get; set; }{ get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
