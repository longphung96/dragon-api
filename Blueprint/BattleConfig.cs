using System;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class BattleConfig
    {
        public long Id { get; set; }
        public long TurnPresentationDuration { get; set; }
        public long NextRoundPresentationDuration { get; set; }
        public long StartDuration { get; set; }
        public long AllowedMissedTurn { get; set; }
        public String BotMasterId { get; set; }
        public long BotDelayPerform { get; set; }
        public long BattleAcceptDuration { get; set; }
        public decimal SkillCoef { get; set; }
        public decimal HealthPerHp { get; set; }
        public decimal CritRatePerMorale { get; set; }
        public decimal CritDamageBase { get; set; }
        public decimal ExtraCritDamageUnit { get; set; }
        public decimal NumOfMoraleExtraCritDamage { get; set; }
        public long ExpPerEnergy { get; set; }
        public decimal DamageReductionLimit { get; set; }
        // public long StatusId { get; set; }
        // public Status Status { get; set; }
        //public List<BattleConfigVersionMapping> BattleConfigVersionMappings { get; set; }
        // public Guid RowId { get; set; }{ get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
