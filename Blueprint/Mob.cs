using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class Mob
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long GenderId { get; set; }
        public long DragonElementId { get; set; }
        public long FamilyId { get; set; }
        public long Level { get; set; }
        public bool BotPVP { get; set; }
        public long? EyeId { get; set; }
        public long? WingId { get; set; }
        public bool IsBoss { get; set; }
        public decimal? Hp { get; set; }
        public decimal? Speed { get; set; }
        public decimal? Skill { get; set; }
        public decimal? Morale { get; set; }
        public decimal? CritRate { get; set; }
        public decimal? CritDamage { get; set; }
        // public long StatusId { get; set; }
        public DragonElement DragonElement { get; set; }
        public Family Family { get; set; }
        // public Status Status { get; set; }
        public List<MobBodypartTypeMapping> MobBodypartTypeMappings { get; set; }
        // public List<MobVersionMapping> MobVersionMappings { get; set; }
        // public Guid RowId { get; set; }{ get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
