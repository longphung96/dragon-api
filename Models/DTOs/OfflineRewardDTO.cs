using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DragonAPI.Configurations;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Models.Entities;

namespace DragonAPI.Models.DTOs
{
    public enum ClaimType
    {

        NormalClaim,
        QuickClaim,
    }
    public class OfflineRewardDTO
    {
        public DateTime LastClaimAt { get; set; }
        public string StageId { get; set; }
        public double DragonFarmLevelBonus { get; set; }
        public List<long> ItemClaims { get; set; } = new();
    }
    public class ClaimOfflineRewardRequest
    {
        public ClaimType Type { get; set; }
    }

    public class AFKIdleItemPoolDTO : BaseWithTimeDTO
    {
        public long ItemId { get; set; }
        public decimal TempValue { get; set; } = 0;
        public decimal currentRate { get; set; } = 0;
        public bool Claimed { get; set; } = false;
        public DateTime LastSnapShotAt { get; set; }
        public DateTime EndFarmingAt { get; set; }
        public DateTime startedAt { get; set; }
    }
}