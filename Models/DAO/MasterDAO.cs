using System.ComponentModel.DataAnnotations.Schema;
using DragonAPI.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DragonAPI.Models.DAOs
{

    public class BattleFormation
    {
        public List<string> DragonIds { get; set; } = new List<string> { };
        // Maybe use this later
        public string SealId { get; set; }
    }
    public class BattleFormationWithType : BattleFormation
    {
        public string Type { get; set; }
    }

    public class MasterDAO : BaseDAO
    {
        // [Column("offchain_wallet_id")]
        // public string OffchainWalletId { get; set; }
        // [Column("onchain_wallet_id")]
        // public string[] OnchainWalletIds { get; set; }
        [Column("main_user_id")]
        public string? MainUserId { get; set; }
        [Column("wallets")]
        public HashSet<string> Wallets { get; set; } = new();
        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }
        [Column("name")]
        public string? Name { get; set; }
        [Column("level")]
        public ushort Level { get; set; }
        [Column("exp")]
        public ulong Exp { get; set; }
        [Column("vip_level")]
        public int VipLevel { get; set; } = 0;

        [Column("pve_team")]
        public string[] PvETeam { get; set; }
        [Column("combination_time")]
        public ushort CombinationTime { get; set; } = 0;
        [Column("free_dragon_received")]
        public bool FreeDragonReceived { get; set; } = false;
        [Column("is_premium")]
        public bool IsPremium { get; set; } = false;
        [Column("link_wallet")]
        public bool LinkWallet { get; set; } = false;
        [Column("completed_tutorial_steps")]
        public List<int> CompletedTutorialSteps { get; set; } = new();
        [Column("auto_remain_count")]
        public int AutoRemainCount { get; set; }
        [Column("x3_speed_remain_count")]
        public int X3SpeedRemainCount { get; set; }
        [Column("formations")]
        public List<BattleFormationWithType> Formations { get; set; } = new List<BattleFormationWithType>();
        [Column("last_claim_reward_at")]
        public DateTime LastClaimRewardAt { get; set; }
        [Column("total_quick_reward")]
        public int TotalQuickReward { get; set; }
        [Column("total_claim_reward")]
        public int TotalClaimReward { get; set; } = 0;
        [Column("remaining_duel")]
        public int RemainingDuel { get; set; } = 0;
        [Column("refesh_chanllenge")]
        public int RefreshChallenge { get; set; } = 0;
        [Column("last_attacked_at")]
        public DateTime LastAttackedAt { get; set; } = DateTime.UtcNow;
        // public DateTime LastActiveVipAt { get; set; }
        [Column("vip_expired_at")]
        public DateTime vipExpiredAt { get; set; }
        public BattleFormation GetFormation(string battleType)
        {
            return Formations.Find(f => f.Type == battleType);
        }
        [Column("avatar_id")]
        public string AvatarId { get; set; }
    }

    public class RankPositionDAO : BaseDAO
    {
        [Column("season_id")]
        public string SeasonId { get; set; }
        [Column("main_user_id")]
        public string MainUserId { get; set; }
        [Column("rank_position")]
        public int RankPosition { get; set; }
        [Column("tier")]
        public TierGroup Tier { get; set; }
    }

    public class BattlePass
    {
        public int AchievementPoint { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class MasterBalanceTransactionHistoryDAO : BaseDAO
    {
        [Column("main_user_id")]
        public string MainUserId { get; set; }
        [Column("item_id")]
        public long ItemId { get; set; }
        [Column("quantity")]
        public decimal Quantity { get; set; }
        [Column("meta_data")]
        public string? MetaData { get; set; }
        [Column("reverted")]
        public bool Reverted { get; set; } = false;
    }
}