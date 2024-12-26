using System;

namespace DragonAPI.Models.Settings
{
    public class BotPVPCommonConfig
    {
        public string Id { get; set; }
        public double MinPowerRate { get; set; }
        public double MaxPowerRate { get; set; }
        public int PVPRank { get; set; }
        public string[] Hours { get; set; }
        // public DateTime StartDate { get; set; }
        // public DateTime EndDate { get; set; }
    }
}
