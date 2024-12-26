using System;
using System.Collections.Generic;
using DragonAPI.Models.Settings;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Dragon.Blueprints;
using DragonAPI.Configurations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragonAPI.Models.DAOs
{
    public class MapStage : BaseDAO
    {

        [Column("name")]
        public string? Name { get; set; }
        [Column("parent")]
        public string? Parent { get; set; }
        [Column("index")]
        public int Index { get; set; }
        [Column("required_level")]
        public int RequiredLevel { get; set; }
        [Column("mob_count")]
        public int MobCount { get; set; }
        [Column("mob_ids")]
        public List<string>? MobIds { get; set; }
        [Column("mobs")]
        public List<MobInStageFormation>? Mobs { get; set; }
        [Column("drop_item_ids")]
        public List<string>? DropItemIds { get; set; }
        [Column("energy")]
        public int? Energy { get; set; }
        [Column("unlocked")]
        public bool Unlocked { get; set; }
        [Column("rss")]
        public int Rss { get; set; }
        [Column("exp")]
        public int Exp { get; set; }
        [Column("boss_stage")]
        public bool BossStage { get; set; }
    }
    public class MapStageInformation
    {
        public string FormationId { get; set; }
        public string StageId { get; set; }
        public int? Time { get; set; }
        public double MobPowerRate { get; set; }
        public string[] MobIds { get; set; }
        public List<Dragon.Blueprints.Mob> Mobs { get; set; }
        public string KeyType { get; set; }
        public string[] DropItemIds { get; set; }
        public List<DropItemConfig> DropItems { get; set; }
        public double DropItemRate { get; set; }
    }
    public class MasterStageTrackingDAO : BaseDAO
    {
        [Column("master_id")]
        public string? MasterId { get; set; }
        [Column("main_user_id")]
        public string? MainUserId { get; set; }
        [Column("wallet_id")]
        public string WalletId { get; set; }
        [Column("map_id")]
        public string MapId { get; set; }
        [Column("map_index")]
        public int MapIndex { get; set; } = 0;
        [Column("stage_id")]
        public string StageId { get; set; }
        [Column("stage_formation_id")]
        public string StageFormationId { get; set; }
        [Column("index")]
        public UInt16 Index { get; set; }
        [Column("best_time")]
        public double BestTime { get; set; } = double.MaxValue;
    }
}