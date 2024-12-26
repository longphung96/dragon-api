using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DragonAPI.Models.DAOs
{



    public class ItemMasterDAO : BaseDAO
    {
        [Column("main_user_id")]
        public string? MainUserId { get; set; }
        [Column("wallet_id")]
        public string? WalletId { get; set; }
        [Column("name")]
        public string? Name { get; set; }
        [Column("item_id")]
        public long ItemId { get; set; }
        [Column("quantity")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Quantity { get; set; }
        [Column("description")]
        public string? Description { get; set; }
    }
    public class ClaimableItemDAO : BaseDAO
    {
        [Column("season_id")]
        public string? seasonId { get; set; }
        [Column("request_id")]
        public string? RequestId { get; set; }
        [Column("main_user_id")]
        public string? MainUserId { get; set; }
        [Column("state")]
        public string State { get; set; } = ClaimState.Waiting.ToString();
        [Column("reward_type")]
        public RewardType RewardType { get; set; } = RewardType.Daily;
        [Column("life_time_hours")]
        public int? LifetimeHours { get; set; } = null;
        [Column("expired_time")]
        public DateTime? ExpiredTime { get; set; }
        [Column("item_id")]
        public long ItemId { get; set; }
        [Column("quantity")]
        public double Quantity { get; set; }
    }
    public enum ClaimState
    {
        Waiting,
        Claiming,
        ClaimedSuccess,
        Preparing,
    }
    public enum RewardType
    {
        Daily,
        Season,
    }
}