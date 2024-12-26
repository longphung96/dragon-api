using AutoFilterer.Attributes;
using AutoFilterer.Types;
using Microsoft.AspNetCore.Mvc;
using DragonAPI.Common;
using DragonAPI.Models.DAOs;
using System;
using System.Text.Json.Serialization;
using DragonAPI.Enums;

namespace DragonAPI.Models.DTOs
{
    public class MasterDTO : BaseWithTimeDTO
    {
        [JsonPropertyName("offchain_wallet_id")]
        public string OffchainWalletId { get; set; }
        [JsonPropertyName("onchain_wallet_id")]
        public string[] OnchainWalletIds { get; set; }
        public string[] Wallets { get; set; }
        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("level")]
        public ushort Level { get; set; }
        [JsonPropertyName("exp")]
        public ulong Exp { get; set; }
        [JsonPropertyName("max_exp")]
        public ulong? MaxExp { get; set; }
        [JsonPropertyName("total_claim_reward")]
        public uint TotalClaimReward { get; set; }
        [JsonPropertyName("max_energy")]
        public uint MaxEnergy { get; set; }
        [JsonPropertyName("energy_bonus_at")]
        public DateTime EnergyBonusAt { get; set; }
        [JsonPropertyName("last_claim_reward_at")]
        public DateTime LastClaimRewardAt { get; set; }
        [JsonPropertyName("avatar_id")]
        public string AvatarId { get; set; }
        [JsonPropertyName("completed_tutorial_steps")]
        public List<int> CompletedTutorialSteps { get; set; }
        // public DateTime LastActiveVipAt { get; set; }
        public DateTime vipExpiredAt { get; set; }
        public int VipLevel { get; set; } = 0;
    }

    public class MasterFilterDTO : BaseFilter
    {
        [FromQuery(Name = "ids")]
        public string[] Id { get; set; }
    }
    public class ListUserFilteringRequestDto : ListOffsetPagniationFiltering
    {
        [FromQuery(Name = "id")]
        public string[] UserId { get; set; }
    }
    public class SearchUserRequestDto : FilterBase
    {
        [FromQuery(Name = "text")]
        [CompareTo(typeof(ToLowerContainsComparisonAttribute), nameof(UserDAO.UserName), nameof(UserDAO.UserId), nameof(UserDAO.MainUserId))]
        public string Search { get; set; }
    }
    public class MeDTO
    {
        [JsonPropertyName("userInfo")]
        public UserDTO UserInfo { get; set; }
        [JsonPropertyName("masterInfo")]
        public MasterDTO MasterInfo { get; set; }
        [JsonPropertyName("activeBattleInfo")]
        public ActiveBattleCache ActiveBattleInfo { get; set; }
    }
    public class SetupTeamRequest
    {
        public BattleType TeamType { get; set; }
        public List<string> DragonIds { get; set; }
    }
    public class SetupTeamResponse
    {
        public string UserId { get; set; }
        public string MainUserId { get; set; }
        public bool Success { get; set; }
    }
    public class ListMasterFilteringRequestDto : ListOffsetPagniationFiltering
    {
        [FromQuery(Name = "ids")]
        public string[] Id { get; set; }
    }
    public class BuyUserManaRequest
    {
        public long TotalUserMana { get; set; }
    }

    public class BuyItemRequest
    {
        public long ItemId { get; set; }
        public long TotalItem { get; set; }
        public long ItemExchangeId { get; set; }
    }
    public class BuyPackageRequest
    {
        public long PackageId { get; set; }
        // public long TotalItem { get; set; }
        // public long ItemExchangeId { get; set; }
    }
    public class CreateBotRequest
    {
        public int Tier { get; set; }
        public long TotalBot { get; set; }
    }

    public class RankPositionDTO : BaseDTO
    {
        public string SeasonId { get; set; }
        public string MainUserId { get; set; }
        public int RankPosition { get; set; }
        public TierGroup Tier { get; set; }
    }
    public class ArenaRankPlayer
    {
        public RankPositionDTO Player { get; set; }
        public List<DragonDTO> Dragons { get; set; }
        public int CP { get; set; }
        public TierGroup Tier { get; set; }
    }
    public class MyArenaRank
    {
        public RankPositionDTO Player { get; set; }
        public List<DragonDTO> Dragons { get; set; }
        public int CP { get; set; } = 0;
        public int RemainingDuel { get; set; } = 0;
        public int RefreshChallenge { get; set; } = 0;
    }
    public class RankPositionResponse
    {
        public List<RankPositionDTO> RankPlayers { get; set; } = new List<RankPositionDTO>();
        public RankPositionDTO MyRank { get; set; } = new RankPositionDTO();
    }
    public class ArenaRankResponse
    {
        public List<ArenaRankPlayer> ChallengePlayers { get; set; } = new List<ArenaRankPlayer>();
        public ArenaRankPlayer MyArena { get; set; } = new ArenaRankPlayer();
    }


    public class UpgradeVipLevelDTO
    {
        public int level { get; set; }
    }
    public class FaucetResultDTO
    {
        public bool succeeded { get; set; }
        public long timestamp { get; set; }
    }
}