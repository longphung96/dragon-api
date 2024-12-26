namespace DragonAPI.IntegrationEvent.Events
{
    public enum SigningType
    {
        DragonMate,
        IncubateEgg,
        HatchEgg,

        WorkplaceCreateDragon,
        WorkplaceLeaseDragon,

        SealMerge,

        GemMerge,
        UnlockSealContainer,

        InventoryCombineItem,

        OpenNftSaleBox,
    }

    public class StoreMetaTxSigningRequestETO
    {
        public SigningType Type { get; set; }
        public string MainUserId { get; set; }
        public SignedDataETO SignedData { get; set; }
        public string WalletId { get; set; }
    }

    public class SignedDataETO
    {
        public string Signature { get; set; }
        public MetaTxDataRequestETO DataRequest { get; set; }
    }

    public class MetaTxDataRequestETO
    {
        public string Operator { get; set; }
        public string From { get; set; }
        public long Nonce { get; set; }
        public long ExpiredAt { get; set; }
        public string Data { get; set; }
    }
    public class MkpSyncUserIntegrationEvent
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string MainUserId { get; set; }
        public string WalletId { get; set; }
        public string[] Permissions { get; set; }
    }
}
