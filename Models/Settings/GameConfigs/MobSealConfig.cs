using DragonAPI.Enums;

namespace DragonAPI.Models.Settings
{
    public class MobSealConfig
    {
        public MobSealConfig()
        {
            SealSlots = new List<MobSealSlotConfig>();
        }
        public string Id { get; set; }
        public List<MobSealSlotConfig> SealSlots { get; set; }
    }
    public class MobSealSlotConfig
    {
        public int Level { get; set; }
        public ClassType Class { get; set; }
        public SealSlotPowerType PowerType { get; set; }
    }
}
