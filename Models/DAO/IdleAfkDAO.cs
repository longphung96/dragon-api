using System;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DragonAPI.Models.DAOs
{
    public class IdleAfkPoolDAO : BaseDAO
    {
        [Column("user_id")]
        public string? UserId { get; set; }
        [Column("item_id")]
        public long ItemId { get; set; }
        [Column("temp_value")]
        public decimal TempValue { get; set; } = 0;
        [Column("current_rate")]
        public decimal currentRate { get; set; } = 0;
        [Column("claimed")]
        public bool Claimed { get; set; } = false;
        [Column("last_snap_shot")]
        public DateTime LastSnapShotAt { get; set; }
        [Column("end_farming_at")]
        public DateTime EndFarmingAt { get; set; }
        [Column("start_at")]
        public DateTime startedAt { get; set; }
    }
    public class HistoryActionChangeRateAfkDAO : BaseDAO
    {
        [Column("main_user_id")]
        public string? MainUserId { get; set; }
        [Column("action_name")]
        public string? ActionName { get; set; }
        [Column("value_change")]
        public decimal ValueChange { get; set; }
        [Column("base_value")]
        public decimal BaseValue { get; set; }
        [Column("pool_id")]
        public string? PoolId { get; set; }
    }

    // public class AFKPoolItemDAO : BaseDAO
    // {
    //     public string userId { get; set; }
    //     public string poolId { get; set; }
    //     public long itemId { get; set; }
    //     public decimal rate { get; set; }
    //     public decimal snapshotedValue { get; set; }
    //     public DateTime snapshotedAt { get; set; }
    //     public DateTime startAt { get; set; }
    //     public DateTime endAt { get; set; }
    //     public bool claimed { get; set; } = false;
    // }
}