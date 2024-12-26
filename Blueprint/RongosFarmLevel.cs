using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dragon.Blueprints
{
    public class DragonFarmLevelConfig
    {
        public long Id { get; set; }
        public long DragonElementId { get; set; }
        public long Level { get; set; }
        public long ItemId { get; set; }
        public long ItemQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Status { get; set; } = true;
    }
    public partial class DragonFarmLevelStatConfig
    {
        public long Id { get; set; }
        public long Level { get; set; }
        public long LevelDragonReq { get; set; }
        public double FarmRateBonus { get; set; }
        public double SuccessRate { get; set; }
        public long TotalUpFail { get; set; }
        public double UpFailRateBonus { get; set; }
        public bool Status { get; set; } = true;


    }
}

