using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class EnvironmentEffect
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Hp { get; set; }
        public decimal Skill { get; set; }
        public decimal Morale { get; set; }
        public decimal Speed { get; set; }
        public long HornCountdown { get; set; }
        public long TailCountdown { get; set; }
        public long UltimateCountdown { get; set; }
        public decimal SkinSkillBuff { get; set; }
        public decimal DiffElementRequired { get; set; }
        // public long StatusId { get; set; }
        // public Status Status { get; set; }
        public List<EnvironmentEffectDragonElementMapping> EnvironmentEffectDragonElementMappings { get; set; }
        // public Guid RowId { get; set; }{ get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
