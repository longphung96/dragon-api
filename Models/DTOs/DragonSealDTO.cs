using DragonAPI.Common;
using DragonAPI.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DragonAPI.Models.DTOs
{
    public class DragonSealDTO : BaseWithTimeDTO
    {
        [JsonPropertyName("wallet_id")]
        public string WalletId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("slots")]
        public List<DragonSealSlotDTO> Slots { get; set; }
        [JsonPropertyName("use_in_battle")]
        public bool UseInBattle { get; set; }
        [JsonPropertyName("max_slot")]
        public short MaxSlot { get; set; }
        [JsonPropertyName("level")]
        public uint Level { get; set; }
        [JsonPropertyName("exp")]
        public ulong Exp { get; set; }
        [JsonPropertyName("max_exp")]
        public ulong? MaxExp { get; set; }
    }

    public class DragonSealSlotDTO
    {
        [JsonPropertyName("level")]
        public int Level { get; set; } = 1;
        [JsonPropertyName("class")]
        public ClassType Class { get; set; }
        [JsonPropertyName("power_type")]
        public SealSlotPowerType PowerType { get; set; }
        [JsonPropertyName("unlocked")]
        public bool Unlocked { get; set; } = false;
        [JsonPropertyName("activated")]
        public bool Activated { get; set; } = false;
    }

    public class SealSlotLevelUpRequestDTO
    {
        public int SlotIndex { get; set; }
    }

    public class SealsFilterDTO : BaseFilter
    {
        public string[] Id { get; set; }
        public string[] WalletId { get; set; }
        public ulong[] NftId { get; set; }

    }
}