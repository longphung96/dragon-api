using System.Text.Json.Serialization;
using DragonAPI.Common;
using DragonAPI.Enums;
using DragonAPI.Models.DAOs;
using DragonAPI.Models.Entities;

namespace DragonAPI.Models.DTOs
{
    public class BaseBattleDto
    {
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [JsonPropertyName("metadata")]
        public object Metadata { get; set; }
    }

    public class CreateFailedBattleDTO
    {
        public string Id { get; set; }
    }

    public class CreatedBattleDTO : BaseBattleDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("battle_server_instance_id")]
        public string BattleServerInstanceId { get; set; }

        [JsonPropertyName("type")]
        public BattleType Type { get; set; }

        [JsonPropertyName("expired_at")]
        public DateTime ExpiredAt { get; set; }

        [JsonPropertyName("stage_id")]
        public string StageId { get; set; }

        [JsonPropertyName("masters")]
        public List<BattleMasterDTO> Masters { get; set; }

        [JsonPropertyName("dragons")]
        public List<BattleDragonDTO> Dragons { get; set; }
        [JsonPropertyName("cookie")]
        public string Cookie { get; set; }
        [JsonPropertyName("background_id")]
        public int BackgroundId { get; set; }

    }



    public class BattleMasterDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("level")]
        public UInt16 Level { get; set; }

        [JsonPropertyName("exp")]
        public long Exp { get; set; }
        [JsonPropertyName("auto_attack")]
        public bool AutoAttack { get; set; }
        [JsonPropertyName("x3_speed")]
        public bool X3Speed { get; set; }
    }

    public class BattleDragonDTO : IDragonBase
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("master_id")]
        public string MasterId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("level")]
        public UInt16 Level { get; set; }
        [JsonPropertyName("rarity")]
        public int Rarity { get; set; } = 0;

        [JsonPropertyName("exp")]
        public double Exp { get; set; }
        [JsonPropertyName("activated_race_level")]
        public int ActivatedRaceLevel { get; set; } = 0;

        [JsonPropertyName("stats")]
        public StatsDto Stats { get; set; }

        [JsonPropertyName("class")]
        public ClassType Class { get; set; }


        [JsonPropertyName("is_mob")]
        public bool IsMob { get; set; }
        [JsonPropertyName("is_boss_mob")]
        public bool IsBossMob { get; set; }
        [JsonPropertyName("boss_mob_skills")]
        public string[] BossMobSkills { get; set; }
        [JsonPropertyName("bodyparts")]
        public List<Bodypart> Bodyparts { get; set; }

        [JsonPropertyName("family")]
        public Family Family { get; set; }
        public bool IsOffChain { get; set; }
    }

    public class BattleExpRewardDto
    {
        public class RewardExp
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("level")]
            public int Level { get; set; }

            [JsonPropertyName("exp")]
            public double? Exp { get; set; }

            [JsonPropertyName("max_exp")]
            public UInt64? MaxExp { get; set; }

            [JsonPropertyName("bonus_exp")]
            public double? BonusExp { get; set; }
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("master")]
        public RewardExp Master { get; set; }
        [JsonPropertyName("dragons")]
        public List<RewardExp> Dragons { get; set; }
        [JsonPropertyName("seal")]
        public RewardExp Seal { get; set; }
    }

    public class WinningBattleResultDto
    {
        public string Id { get; set; }
        public long Rss { get; set; }
        public long LRss { get; set; }
        public BattleExpRewardDto ExpReward { get; set; }
        public List<ClaimableItemDto> PVEDropItems { get; set; }
        public BattlePVETimeDto PVETime { get; set; }
        // public List<AchievementStatsDTO> NewMasterAchievements { get; set; }
    }

    public class BattlePVETimeDto
    {
        public bool IsBest { get; set; }
        public long Seconds { get; set; }
        public long BestSeconds { get; set; }
    }

    public class BattleSealDTO
    {
        public string Id { get; set; }
        public string MasterId { get; set; }
        public string Name { get; set; }
        public List<DragonSealSlotDTO> Slots { get; set; }
        public Int16 MaxSlot { get; set; }
        public uint Level { get; set; }
        public bool IsOffChain { get; set; }
    }


    public class BattleSetupDragonFormationResultDTO
    {
        public string RequestId { get; set; }
        public bool Success { get; set; }
    }
    public class BattleSetupSealFormationResultDTO
    {
        public string RequestId { get; set; }
        public bool Success { get; set; }
    }
}