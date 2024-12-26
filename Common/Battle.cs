using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DragonAPI.Enums;
using DragonAPI.Models.DTOs;
using DragonAPI.Models.Entities;

namespace DragonAPI.Common
{
    public enum BattleType
    {
        PVE,
        PVP
    }
    public enum ConnectionState
    {
        Waiting,
        Disconnected,
        Connected,
    }
    public enum ConfirmationState
    {
        Waiting,
        Cancelled,
        Accepted,
    }
    public enum BattleState
    {
        Initialized,
        Ready,
        Running,
        Completed,
        Cancelled,
        Timeout,
    }
    public class ActiveBattleCache
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("masterId")]
        public string MasterId { get; set; }
        [JsonPropertyName("mainUserId")]
        public string MainUserId { get; set; }
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        [Obsolete("Please use MasterId")]
        [JsonPropertyName("walletId")]
        public string WalletId { get; set; }
        [JsonPropertyName("battleType")]
        public string BattleType { get; set; }
        [JsonPropertyName("cookie")]
        public string Cookie { get; set; }
    }

    public class BattleAnalysis
    {
        public string Id { get; set; }
        public BattleType Type { get; set; }
        public string StageId { get; set; }
        public string StageMobFormationId { get; set; }
    }

    public class BattleDragonAnalysis
    {
        public string WalletId { get; set; }
        public string UserId { get; set; }
        public UInt64 NftId { get; set; }
        public UInt16 Level { get; set; }
        public ClassType Class { get; set; }
        public List<Bodypart> Bodyparts { get; set; }
    }
    public class BattleSealAnalysis
    {
        public string WalletId { get; set; }
        public string UserId { get; set; }
        public UInt64 NftId { get; set; }
        public List<DragonSealSlotDTO> Slots { get; set; }
        public Int16 MaxSlot { get; set; }
        public uint Level { get; set; }
    }
}
