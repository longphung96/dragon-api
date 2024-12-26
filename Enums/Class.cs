namespace DragonAPI.Enums
{
    public enum ClassType
    {
        Fire = 0,
        Water = 1,
        Wood = 2,
        Gold = 3,
        Earth = 4,
        Light = 5,
        Dark = 6,
        Fire2 = 10,
        Fire3 = 11,
        Water2 = 30,
        Water3 = 31,
        Wood2 = 50,
        Wood3 = 51,
        Gold2 = 70,
        Gold3 = 71,
        Earth2 = 90,
        Earth3 = 91,
        Light2 = 110,
        Light3 = 111,
        Dark2 = 130,
        Dark3 = 131,
        Undefined = 1000,
    }

    public enum ClassGrouping
    {
        Fire = 0,
        Water = 1,
        Wood = 2,
        Gold = 3,
        Earth = 4,
        Light = 5,
        Dark = 6,
    }

    public enum MutantType
    {
        Normal
    }
    public enum Gender
    {
        Unknown,
        Male,
        Female
    }
    public enum EnvironmentEffectName
    {
        BuffAllStats,
        DebuffAllStats,
        HPStats,
        DebuffHpStats,
        BuffSkillStats,
        DebuffSkillStats
    }
    public enum Family
    {
        Dragon = 0,
        Kuberaos,
        Aranyanios,
        Varunaos,
        Agnios,
        Bhumios,
        Kalios,
        Suryaos,
        Spirit = 50,
        Slime,
        Spectre,
        Imp,
        Elemental,
        ElementSpirit,
        ElementSpectre
    }

    public enum ItemTypeDragon
    {
        InvalidType = -1,
        Dragon,
        Seal,
        Egg,
        TreasureKey,
        Building,
        SealBox,
        TokenSealBoxShard,
        TokenEggShard,
        TokenTreasureKeyShard,
        TokenBuildingShard,
        TokenDust,
        TokenScroll,
        NFTSalebox,
        Ticket
    }
}
