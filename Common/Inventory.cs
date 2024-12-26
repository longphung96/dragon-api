using System;

namespace DragonAPI.Common
{
    public enum ItemTypeDragonItemType
    {
        Egg,
        EggShard,
        SealShard,
        DragonSealScroll,
        DragonSealDust,
        Hatching,
        ExpPool,
        EnergyTree,
        ALand,
        BLand,
        CLand,
        DLand,
        ELand,
        FLand,
        GLand,
        GoldAvatar,
        WoodAvatar,
        FireAvatar,
        WaterAvatar,
        EarthAvatar,
        DarkAvatar,
        LightAvatar,
        TreasureKey,
    }
    public enum ItemRarity
    {
        Uncommon,
        Common,
        Rare,
        Unique,
        Legendary,
        Acient,

        TreasureWood,
        TreasureBronze,
        TreasureSilver,
        TreasureGold,
        TreasureDiamond,
    }
    public enum InventoryScope
    {
        Exp,
        Energy,
        ElementPower,
        Function,
        RandomElement,
        Treasure,
        Egg,
        Seal
    }
    public class ItemTreasureKey
    {
        public string Id { get; set; }
        public string WalletId { get; set; }
        public ItemRarity Rarity { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}




