using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DragonAPI.Models.DAOs
{



    public class PvPHistoryDAO : BaseDAO
    {
        [Column("battle_id")]
        public string BattleId { get; set; }

        [Column("main_user_id")]
        public string MainUserId { get; set; }
        [Column("battle_status")]
        public BattleStatus BattleStatus { get; set; }
        [Column("opponent")]
        public PlayerInBattle OpponentPlayer { get; set; }
        [Column("rank_status")]
        public string RankStatus { get; set; }
    }
    public enum BattleStatus
    {
        Lose,
        Win
    }
    public class PlayerInBattle
    {
        [Column("main_user_id")]
        public string MainUserId { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("rank")]
        public long Rank { get; set; }
        [Column("combat_power")]
        public double CombatPower { get; set; }
    }
}