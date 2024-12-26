using DragonAPI.Enums;


namespace DragonAPI.IntegrationEvent.Events
{
    public enum MailboxType
    {
        Mail_Direction,
        Mail_Direction_PVE,
        Mail_Direction_PVP,
        Mail_Direction_TreasureIsland,

        Mail_Reward = 30,
        Mail_Reward_ItemDrop,
        Mail_Reward_TreasureIsland,
        Mail_Reward_WorldEvent,
        Mail_Reward_Arena,
        Mail_Reward_Activity_Event = 50,

        Mail_Notify = 200,

        Mail_Notify_Dragon_Mating,
        Mail_Notify_Dragon_Birth,
        Mail_Notify_Dragon_Bodypart_Unlocked,

        Mail_Notify_Seal_Socket_Unlocked,
        Mail_Notify_Seal_Socket_Rerolled,

        Mail_Notify_Egg_Incubated,

        Mail_Notify_Seal_Created,


        Mail_Notify_Egg_Created,
        Mail_Notify_Egg_Hatched,
        Mail_Notify_Egg_Incubating,



        Mail_Notify_Transfer_Order_Completed = 220,
        Mail_Notify_Transfer_Order_Created,
        Mail_Notify_Transfer_Order_Cancelled,
        Mail_Notify_Transfer_Order_Expired,

        Mail_Notify_Workplace_Created = 230,
        Mail_Notify_Workplace_Leased,
        Mail_Notify_Workplace_Claimed,
        Mail_Notify_Workplace_Cancelled,
        Mail_Notify_Workplace_Expired,
        Mail_Notify_Workplace_Leasing_Expired,

        Mail_Notify_Withdraw = 250,

        Mail_Notify_Inventory = 270,
        Mail_Notify_Inventory_ClaimItem_Result,
        Mail_Notify_Inventory_MetaNft_Created,
        Mail_Notify_Inventory_Fragment_Combination,
    }

    // public class NewMailCreatedIntegrationEto
    // {
    //     public string Id { get; set; }
    //     public string MainUserId { get; set; }
    //     [Obsolete("Will be removed")]
    //     public string WalletId { get; set; }
    //     public string Title { get; set; }
    //     public string Content { get; set; }
    //     public MailboxType Type { get; set; }
    //     public DateTime? ExpiredTime { get; set; }
    // }


    // public class MailRewardDataEto
    // {
    //     public List<ClaimableItemEto> Items { get; set; }
    // }

    // public class MailDirectionDataEto
    // {

    // }

    // public class MailNotificationDataEto
    // {

    // }
    public class NewMailCreatedIntegrationEto
    {
        public string Id { get; set; }
        public string MainUserId { get; set; }
        [Obsolete("Will be removed")]
        public string WalletId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public MailboxType Type { get; set; }
        public DateTime? ExpiredTime { get; set; }
    }
    public class CreatingMailboxRequestIntegrationEvent
    {
        [Obsolete("Please use `MainUserId`")]

        public string WalletId { get; set; }
        public string MainUserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string RequestId { get; set; }
        public MailboxType Type { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public MailRewardDataEto RewardData { get; set; }
        public string DirectionData { get; set; }
        public string NotificationData { get; set; }
    }

    public class MailNotification_TransferOrderCreatedOrCancelledData
    {
        public ItemTypeDragon ItemType { get; set; }
        public long? TypeId { get; set; } = null;
        public string Entity { get; set; }
        public object OrderRecord { get; set; }
        public double Amount { get; set; } = 1;
    }
    public class MailRewardDataEto
    {
        public List<ClaimableItemEto> Items { get; set; }
    }
}