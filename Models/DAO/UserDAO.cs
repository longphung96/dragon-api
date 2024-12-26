using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DragonAPI.Models.DAOs
{
    public class UserDAO : BaseDAO
    {
        [Column("main_user_id")]
        public string MainUserId { get; set; }
        [Column("user_name")]
        public string UserName { get; set; }
        [Column("wallets")]
        public HashSet<string> Wallets { get; set; }
        [Column("permissions")]
        public string[] Permissions { get; set; }
        [Column("is_lock")]
        public bool IsLock { get; set; }
        [Column("is_premium")]
        public bool IsPremium { get; set; }
        [Column("is_f2p_dragon_generated")]
        public bool IsF2PDragonGenerated { get; set; } = true;
        [Column("main_user_level")]
        public long MainUserLevel { get; set; } = 1;
    }
    public class MasterWalletDAO : BaseDAO
    {
        [Column("main_user_id")]
        public string? MainUserId { get; set; }
        [Column("wallet_id")]
        public string? WalletId { get; set; }
    }
    public class UserBotDAO : BaseDAO
    {
        public string BotUserId { get; set; }
    }
}