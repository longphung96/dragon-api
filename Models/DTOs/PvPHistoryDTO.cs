using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DragonAPI.Models.DAOs;

namespace DragonAPI.Models.DTOs
{



    public class PvPHistoryDTO : BaseDTO
    {
        [BsonElement("battleid")]
        public string BattleId { get; set; }

        [BsonElement("mainUserId")]
        public string MainUserId { get; set; }
        [BsonElement("battle_status")]
        public BattleStatus BattleStatus { get; set; }
        [BsonElement("opponent")]
        public PlayerInBattle OpponentPlayer { get; set; }
        [BsonElement("rank_status")]
        public string RankStatus { get; set; }
    }

    public class PvPHistoryRequest
    {
        public string MainUserId { get; set; }
        public BattleStatus BattleStatus { get; set; }
    }
}