
//using Dragon.Blueprints;
using Nethereum.Contracts.Standards.ENS.PublicResolver.ContractDefinition;
using DragonAPI.Enums;
using DragonAPI.Models.DAOs;
using DragonAPI.Models.Entities;
using DragonAPI.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonAPI.Configurations
{
    public partial class GameConfigs
    {
        public const string GameConfigOption = "GameConfigs";
        public Dictionary<ClassType, DragonStatsConfig> DragonStatsConfigMap { get; set; }
        public List<DragonStatsConfig> DragonStatsConfigs { get; set; }
        public WorldMapConfig WorldMapConfig { get; set; } = new WorldMapConfig();
        public Dictionary<BodypartType, BodypartTypeConfig> BodypartConfigMap { get; set; }
        public List<Mob> Mobs { get; set; }
        public LevelExpConfiguration LevelExpConfig { get; set; } = new LevelExpConfiguration();
        public StatCoef StatCoef { get; set; }
        public List<SeasonConfig> SeasonConfigs { get; set; }
        public List<SeasonRewardConfig> seasonRewardConfigs { get; set; }
        public List<ItemConfig> ItemConfigs { get; set; }
        public List<BodypartLevelConfig> BodypartLevelConfigs { get; set; } = new List<BodypartLevelConfig>();
        public Dictionary<ClassType, ClassType> ClassGroupMap { get; set; } = new Dictionary<ClassType, ClassType>();
        public Dictionary<ClassType, string> DragonDefaultName { get; set; } = new Dictionary<ClassType, string>();
        public List<StatsCoef> StatsCoefConfigs { get; set; }
        public BattleServerConfig BattleServerConfig { get; set; }
        public List<BuffSkillConfig> SkillConfigs { get; set; }
        public List<DragonUplevel> DragonUplevelConfigs { get; set; }
        public List<DragonUpgrade> DragonUpgradeConfigs { get; set; }
        public List<DragonMergeFragment> DragonMergeFragmentConfigs { get; set; }
        public List<AfkReward> AfkRewardConfigs { get; set; }
        public List<BodypartTypeMap> BodypartTypeMap { get; set; }
        public List<RateConvertItem> ChangeRateItem { get; set; } = new();
        public List<DragonFarmLevel> DragonFarmLevel { get; set; } = new();
        public List<DragonFarmLevelStat> DragonFarmLevelStat { get; set; } = new();
        public List<CommonSetting> CommonSetting { get; set; } = new();
        public List<ShopPackageConfig> ShopPackageConfig { get; set; } = new();
        public List<TeamPVPMobsConfig> TeamPVPMobConfigs { get; set; } = new List<TeamPVPMobsConfig>();
        public List<BotPVPCommonConfig> BotPVPCommonConfigs { get; set; } = new();
        public List<UpgradeRateDragonFarmLevel> UpgradeRateDragonFarmLevel { get; set; } = new();
        public List<MergeFragmentItemConfig> MergeFragmentItemConfig { get; set; } = new();
        public List<DragonOpenPackageConfig> DragonOpenPackageConfig { get; set; } = new();
        public List<SeasonRankingReward> SeasonRankingRewardConfig { get; set; } = new();
        public List<DailyRankingReward> DailyRankingRewardConfig { get; set; } = new();
    }
    public class RateConvertItem
    {
        public long Id { get; set; }
        public long ExchangeItem { get; set; }
        public long ReceivedItem { get; set; }
        public decimal Rate { get; set; }
    }
    public class BodypartTypeMap
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long UnlockLevel { get; set; }
        public long Rss { get; set; }
        public long Rocs { get; set; }
    }
    public class DragonFarmLevel
    {
        public long Id { get; set; }
        public long DragonElementId { get; set; }
        public long Level { get; set; }
        public long ItemId { get; set; }
        public long ItemQuantity { get; set; }
    }
    public partial class DragonFarmLevelStat
    {
        public long Id { get; set; }
        public long Level { get; set; }
        public long LevelDragonReq { get; set; }
        public double FarmRateBonus { get; set; }
        public double SuccessRate { get; set; }
        public long TotalUpFail { get; set; }
        public double UpFailRateBonus { get; set; }
        public bool Status { get; set; }


    }
    public class UpgradeRateDragonFarmLevel
    {
        public long Id { get; set; }
        public long Level { get; set; }
        public double Rate { get; set; }
        public string Description { get; set; }
    }
    public class DragonStressConfig
    {
        public int LevelApply { get; set; } = 30;
        public int MaxHappyPoint { get; set; } = 1000;
        public int NotHappyPoint { get; set; } = 800;
        public int StressPoint { get; set; } = 600;
        public double NotHappyEffectValue { get; set; } = 0.3;
        public int NumOfHappyPointConsumePVE { get; set; } = 10;
        public int NumOfHappyPointRegeneration { get; set; } = 4;
        public int NumOfHappyPointRegenerationPVP { get; set; } = 20;
        public int SecondOfHappyPointRegeneration { get; set; } = 600;
        public int SecondOfNotHappyPointRegeneration { get; set; } = 1200;
    }

    public class DragonStatsConfig
    {

        public Stats StartingStats { get; set; }
        public Stats IncreaseStatsByLevel { get; set; }
    }
    public class DragonStatsConfig
    {
        public ClassType Class { get; set; }
        public int Rarity { get; set; }

        public Stats StartingStats { get; set; }
        public Stats IncreaseStatsByLevel { get; set; }
    }
    public class BodypartTypeConfig
    {
        public BodypartType Bodypart { get; set; }
        public int RequiredLevel { get; set; }
        public long Lrss { get; set; }
        public long RSS { get; set; }
        public long YNR { get; set; } = 0;
        public long LRSSUnlockSkill { get; set; }
        public long RSSUnlockSkill { get; set; }
        public long YNRUnlockSkill { get; set; } = 0;
        public Dictionary<ClassType, Stats> StatsByClassMap { get; set; }
    }
    public class EnergyConfig
    {
        public class EnergyMaxLevelConfig
        {
            public int Level { get; set; }
            public int MaxEnergy { get; set; }
        }
        public int EnergyPerBattle { get; set; }
        public int BeginEnergyDefault { get; set; }
        public int EnergyBonusSeconds { get; set; }
        public int EnergyBonusAmount { get; set; }
        public List<EnergyMaxLevelConfig> EnergyMaxLevelConfigs { get; set; }

        public int MaxEnergyAtLevel(int level)
        {
            var maxEnergyValue = BeginEnergyDefault;
            var firstGreaterLevelIdx = EnergyMaxLevelConfigs.FindIndex(0, c => c.Level > level);
            var index = 0;
            if (firstGreaterLevelIdx == -1)
            {
                index = EnergyMaxLevelConfigs.Count - 1;
            }
            else
            {
                index = firstGreaterLevelIdx - 1;
            }
            return EnergyMaxLevelConfigs[index].MaxEnergy;
        }
    }
    public class WorldMapConfig
    {
        public List<MapStageConfig> Maps { get; set; }
        public List<MapStageConfig> Stages { get; set; }
        public List<StageFormationsConfig> StageFormations { get; set; }
        public List<StageOpenTimeConfig> ListLastStageOpenTime { get; set; }
    }
    public class LevelExpConfiguration
    {
        public class ExpConfig
        {
            public long Level { get; set; }
            public long MinExp { get; set; }
            public long MaxExp { get; set; }
        }
        public class LevelConfig
        {
            public long MaxLevel { get; set; }
            public List<ExpConfig> Exps { get; set; }
            public ExpConfig GetExpConfigAtLevel(long level)
            {
                return Exps.Find(expc => expc.Level == level);
            }
            public ExpConfig GetExpConfigByExp(double exp)
            {
                var expC = Exps.Find(lc => lc.MinExp <= exp && exp < lc.MaxExp);
                if (expC == null)
                    expC = Exps.Last();
                return expC;
            }
        }
        public int ExpPerEnergy { get; set; }
        public Dictionary<string, LevelConfig> EntityLevelConfigMap { get; set; } = new Dictionary<string, LevelConfig>();
    }
    public class MapStageConfig
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public bool LastStage { get; set; }
        public int Index { get; set; }
        public int RequiredLevel { get; set; }
        public int? Energy { get; set; }
        public int? ExpPerMob { get; set; }
        public int? RssPerMob { get; set; }
        public int MobCount { get; set; }
    }
    public class ItemConfig
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; } = "";
        public long TypeId { get; set; }
        public string EntityType { get; set; }
        //public long? SealSlot { get; set; }
        public bool IsNFT { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
    }
    public class StageOpenTimeConfig
    {
        public string StageId { get; set; }
        public int Time { get; set; }
        public int Total { get; set; }
    }

    public class Mob : IDragonBase
    {
        public string Id { get; set; }
        public Gender Gender { get; set; } = Gender.Unknown;
        public ClassType Class { get; set; }
        public ushort Level { get; set; }
        public int Rarity { get; set; } = 0;
        public List<Bodypart> Bodyparts { get; set; }
        public FullStats Stats { get; set; }
        public Family Family { get; set; }

        public bool IsBossMob { get; set; }
    }
    public class MobInStageFormation : Mob
    {
        public long IndexInFormation { get; set; }
    }


    public class ReputationConfig
    {
        public int MaxReputation { get; set; }
        public int NumOfReputationRegeneration { get; set; }
        public int SecondOfReputationRegeneration { get; set; }
    }

    public class PvPRankConfig
    {
        public int Id { get; set; }
        public string RankName { get; set; }
        public long LrssBonus { get; set; }
        public int RankUpPointBonus { get; set; }
        public bool RankUpRequired { get; set; }
        public int RankUpIncessantWinRequired { get; set; }
        public int RankPointRequired { get; set; }
        public int AfkReputationDesc { get; set; }
        public int CancelMatchReputationDesc { get; set; }
        public int DoesNotAcceptReputationDesc { get; set; }
    }

    public class PVPRankingConfig
    {
        public int MinimumReputationRequired { get; set; }
        public int RankPointWin { get; set; }
        public int RankPointLoss { get; set; }
        public int MinRankDefined { get; set; }
        public int MaxRankDefined { get; set; }
        public Dictionary<int, PvPRankConfig> RankConfigMap { get; set; }
    }
    public class EnvironmentConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        // public string EnvEffectIds { get; set; }
    }
    public class SeasonConfig
    {
        public string Id { get; set; }
        public DateTime StartingTime { get; set; }
        public DateTime EndingTime { get; set; }
    }
    public class SeasonRewardConfig
    {
        public long Id { get; set; }
        public string SeasonId { get; set; }
        public string TypeReward { get; set; }
        public string Tier { get; set; }
        public long PositionFrom { get; set; }
        public long PositionTo { get; set; }
        public long ItemId { get; set; }
        public long Quantity { get; set; }
    }

    /// <summary>
    /// PVP SOLO
    /// </summary>
    public class PVPSoloRoomConfigs
    {
        public List<RssPackage> RssPackages { get; set; } = new List<RssPackage>
        {
            new RssPackage{Level=10,Rss=100},
            new RssPackage{Level=20,Rss=500},
            new RssPackage{Level=30,Rss=1500},
            new RssPackage{Level=40,Rss=4000},
            new RssPackage{Level=50,Rss=6000},
            new RssPackage{Level=60,Rss=8000},
            new RssPackage{Level=70,Rss=10000},
        };
        public double Fee { get; set; } = 10.5; // unit: %
    }
    public class RssPackage
    {
        public int Level { get; set; }
        public int Rss { get; set; }
    }

    public class RaceLevelConfig
    {
        public ClassType Class { get; set; }
        public int RaceLevel { get; set; }
        public ClassType[] RequiredClasses { get; set; }
        public long LRss { get; set; }
        public long Rss { get; set; }
        public long YNR { get; set; }
        public int RequiredLevel { get; set; }
        public int Breed { get; set; }
        public DragonAPI.Common.BuffStats BuffStats { get; set; }
    }

    public class BodypartLevelConfig
    {
        public BodypartType Bodypart { get; set; }
        public int Level { get; set; }
        public long Rss { get; set; }
        public int RequiredLevel { get; set; }
    }

    public class TemplateItem
    {
        public long Id { get; set; }
        public string Entity { get; set; }
        public long TypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsNft { get; set; }
    }


    public class StatCoef
    {
        public double SkillCoef { get; set; }
        public double HealthPerHp { get; set; }
        public double CritRatePerMorale { get; set; }
        public double CritDamageBase { get; set; }
        public double ExtraCritDamageUnit { get; set; }
        public double NumOfMoraleExtraCritDamage { get; set; }
    }
    public class StatsCoef
    {
        public int Level { get; set; }
        public double SkillCoef { get; set; }
        public double HealthPerHp { get; set; }
    }

    public class BodypartBonusConfig
    {
        public int RequiredLevel { get; set; }
        public long Rss { get; set; }
        public Dictionary<ClassType, Stats> StatsByClassMap { get; set; }
    }

    public class GameEnergyConfiguration
    {
        public EnergyConfig Premium { get; set; } = new();
        public EnergyConfig Offchain { get; set; } = new();

        public EnergyConfig GetEnergyConfig(bool premium) => premium ? Premium : Offchain;
    }




    public class StageFormationsConfig
    {
        public string FormationId { get; set; }
        public string StageId { get; set; }
        public int? Time { get; set; }
        public double MobPowerRate { get; set; }
        public string[] MobIds { get; set; }
        public List<MobInStageFormation> Mobs { get; set; }
        public string KeyType { get; set; }
        public List<string> DropItemIds { get; set; }
        public List<DropItemConfig> DropItems { get; set; }
        public double DropItemRate { get; set; }
        public string EnvironmentId { get; set; }
        //public string MobSealId { get; set; }
    }

    public class DragonMob : Mob
    {
        public double PowerRate { get; set; }
    }




    public class BattlePassConfig
    {
        public string SeasonId { get; set; }
        public int AchievementPoint { get; set; }
        public int RSS { get; set; }
        public int ROCS { get; set; }
        public int Amount { get; set; }
        public Common.ItemRarity[] Keys { get; set; }
    }

    public class TeamPVPMobsConfig
    {
        public string TeamPVPId { get; set; }
        public string Name { get; set; }
        public double MobPowerRate { get; set; }
        public string TierRank { get; set; }
        public List<MobInStageFormation> Mobs { get; set; }
        public string[] DragonMobIds { get; set; }
        public double TeamPower { get; set; }
    }
    public partial class DragonUplevel
    {
        public long Id { get; set; }
        public long DragonElementId { get; set; }
        public long Rss { get; set; }
        public long DragonExp { get; set; }
        public long NextLevelUpdate { get; set; }
        public decimal CoeffUpdate { get; set; }
        public long? AmountScroll { get; set; }
        public long? AmountDust { get; set; }
        public long? BodyPartTypeUpdate { get; set; }
        public long? BodyPartLevelUpdate { get; set; }
        public bool Status { get; set; } = true;
        public virtual Dragon.Blueprints.DragonElement DragonElement { get; set; }

    }
    public partial class DragonUpgrade
    {
        public long Id { get; set; }
        public long DragonElementId { get; set; }
        public long NextLevelUpdate { get; set; }
        public long? ItemId1 { get; set; }
        public long? AmountItem1 { get; set; }
        public long? ItemId2 { get; set; }
        public long? AmountItem2 { get; set; }
        public long? ItemId3 { get; set; }
        public long? AmountItem3 { get; set; }
        public long? BodyPartTypeUpdate { get; set; }
        public long? BodyPartLevelUpdate { get; set; }
        public bool Status { get; set; } = true;
        public virtual Dragon.Blueprints.DragonElement DragonElement { get; set; }

    }
    public class CommonSetting
    {
        public long Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool Status { get; set; }
    }
    public class AfkReward
    {
        public long Id { get; set; }
        public long StageId { get; set; }
        public long Type { get; set; }
        public long ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public long MinTimeRequire { get; set; }
        public long MaxTimeRequire { get; set; }
        public bool Status { get; set; } = true;
    }
    public partial class DragonMergeFragment
    {

        public long Id { get; set; }
        public long DragonElementId { get; set; }
        public long Rss { get; set; }
        public long ItemId { get; set; }
        public long ItemQuantity { get; set; }
        public long? AmountScroll { get; set; }
        public long? AmountDust { get; set; }
        public bool Status { get; set; } = true;
        public virtual Dragon.Blueprints.DragonElement DragonElement { get; set; }
    }
    public partial class MergeFragmentItemConfig
    {

        public long Id { get; set; }
        public long ItemReceivedId { get; set; }
        public decimal ItemReceivedAmount { get; set; }
        public long ItemConsumedId { get; set; }
        public decimal ItemConsumedAmount { get; set; }
        public long ItemMaterial { get; set; }
        public decimal ItemMaterialAmount { get; set; }
        public bool Status { get; set; } = true;
    }
    public class OffChainBodypartLevelConfig
    {
        public BodypartType Bodypart { get; set; }
        public long Level { get; set; }
        public decimal Lrss { get; set; }
        public long RequiredLevel { get; set; }
    }
    public class ShopPackageConfig
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal BasePrice { get; set; }
        public decimal FinalPrice { get; set; }
        public long CurrencyId { get; set; }
        public bool Status { get; set; } = true;
    }
    public partial class DragonOpenPackageConfig
    {

        public long Id { get; set; }
        public long DragonElementId { get; set; }
        public long ItemId { get; set; }
        public long ItemQuantity { get; set; }
        public long? ItemFee { get; set; }
        public long? AmountFee { get; set; }
        public bool Status { get; set; } = true;
        public virtual Dragon.Blueprints.DragonElement DragonElement { get; set; }
    }
    public partial class DailyRankingReward
    {
        public long Id { get; set; }
        public long RankFrom { get; set; }
        public long RankTo { get; set; }
        public long ItemId { get; set; }
        public double Quantity { get; set; }
        public bool Status { get; set; } = true;
        public virtual Item Item { get; set; }
    }
    public partial class SeasonRankingReward
    {

        public long Id { get; set; }
        public string SeasonId { get; set; }
        public long RankFrom { get; set; }
        public long RankTo { get; set; }
        public long ItemId { get; set; }
        public double Quantity { get; set; }
        public bool Status { get; set; } = true;
        public virtual Item Item { get; set; }
    }
}