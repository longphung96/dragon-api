using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dragon.Blueprints
{
    public class UpgradeRateDragonFarmLevelConfig
    {
        public long Id { get; set; }
        public long Level { get; set; }
        public double Rate { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; } = true;


    }
}

