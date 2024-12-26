using System;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DragonAPI.Models.DAOs
{
    public class SettingsDAO : BaseDAO
    {
        [Column("main_user_id")]
        public string? MainUserId { get; set; }
        [Column("bot_animation_enabled")]
        public bool BotAnimationEnabled { get; set; }
        [Column("sound")]
        public int Sound { get; set; }
        [Column("music")]
        public int Music { get; set; }
    }
}