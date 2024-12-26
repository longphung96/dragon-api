using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DragonAPI.IntegrationEvent.Events
{
    public class InventoryClaimF2PPackageETO
    {
        public enum SourceEventTypeEnum
        {
            None,
            Inventory,
            Dragon,
            Seal
        }
        public string ClaimableItemId { get; set; }
        public string MainUserId { get; set; }
        public bool IsSuccess { get; set; }
        public SourceEventTypeEnum SourceEventType { get; set; }
    }

    public class InventoryEggFragmentCombinationPayloadETO
    {
        public bool Success { get; set; }
        public string RequestId { get; set; }
        [Obsolete]
        public string MainUserId { get; set; }
        public string WalletId { get; set; }
        public long TypeId { get; set; }
        public long Index { get; set; }
        public string EggId { get; set; }
        public long NftId { get; set; }
    }
    public class ClaimableItemEto
    {
        public string Id { get; set; }
        public string Entity { get; set; }
        public long? Type { get; set; } = -1;
        public long TemplateItemId { get; set; }
        public DateTime? ExpiredTime { get; set; } = null;
        public int? LifetimeHours { get; set; }
        public int Amount { get; set; } = 1;
        public object Metadata { get; set; }
    }
    public class ClaimableItemsIntegrationEto
    {
        [Obsolete("Please use `MainUserId`")]
        public string WalletId { get; set; }
        public string MainUserId { get; set; }
        public List<ClaimableItemEto> Items { get; set; }
    }
    public class CreateClaimableItemsRequestIntegrationEvent : ClaimableItemsIntegrationEto
    {
        public string RequestId { get; set; }
    }
    public class CreateClaimableItemsResponseIntegrationEvent : ClaimableItemsIntegrationEto
    {
        public string RequestId { get; set; }
        public bool Success { get; set; }
    }
    public class AdminTransferItemEto
    {
        public string ReceiveWalletId { get; set; }
        public long ItemId { get; set; }
        public decimal Amount { get; set; }
        public string WithdrawVerifyId { get; set; }
    }
}
