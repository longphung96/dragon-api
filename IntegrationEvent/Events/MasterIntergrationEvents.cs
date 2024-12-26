using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DragonAPI.IntegrationEvent.Events
{
    public class MkpSyncUserEto
    {
        public string UserId { get; set; }
        public string MainUserId { get; set; }
        public string UserName { get; set; }
        public string WalletId { get; set; }
        public string[] Permissions { get; set; }
    }

    public class GameserverSyncUserETO
    {
        public string MainUserId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public HashSet<string> Wallets { get; set; }
        public string DisplayName { get; set; }
        public string[] Permissions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SyncUserEto
    {
        public string MainUserId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public HashSet<string> Wallets { get; set; }
        public string DisplayName { get; set; }
        public string[] Permissions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class MasterInGameCurrencyConsumeRequestResponseETO
    {
        public bool Success { get; set; } = false;
        public string TxId { get; set; }
        public string ErrorMessage { get; set; }
        public string RequestId { get; set; }
        public string MainUserId { get; set; }
        public decimal Rss { get; set; } = 0;
        public decimal LRss { get; set; } = 0;
        public string SourceEntity { get; set; }
        public string Action { get; set; }
    }

    public class MasterInGameCurrencyRevertRequestETO
    {
        public string RequestId { get; set; }
        public string MainUserId { get; set; }
        public string TxId { get; set; }
    }

    public class MasterPremiumSyncEto
    {
        public string MainUserId { get; set; }
        public bool IsPremium { get; set; } = false;
    }

    public class MasterLevelSyncEto
    {
        public string MainUserId { get; set; }
        public long Level { get; set; }
    }
    public class MasterSyncEto
    {
        public string MainUserId { get; set; }
        public HashSet<string> Wallets { get; set; }
        public bool IsPremium { get; set; } = false;
    }

    public class MasterUserLevelUpHistoryIntegrationEvent
    {
        [Obsolete("Please use `MainUserId` instead")]
        public string WalletId { get; set; }
        public string MainUserId { get; set; }
        public long Level { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class MasterInGameCurrencyRevertETO
    {
        public string MainUserId { get; set; }
        public string TxId { get; set; }
        public string SourceEntity { get; set; }
        public string Action { get; set; }
    }
}
