namespace DragonAPI.IntegrationEvent.Events
{
    public class IntegrationTopicConst
    {
        public const string BattleService = "rongos.svc.battle";
        public const string BattleServer = "rongos.svc.battle-server";
        public const string DragonService = "rongos.svc.rongos";
        public const string InventoryService = "rongos.svc.inventory";
        public const string MarketplaceService = "rongos.svc.marketplace";
        public const string BlockchainService = "rongos.svc.blockchain.net";
    }
    public class ActionNameConst
    {
        public const string DragonFarmLevelUp = "DragonFarmLevelUp";
        public const string BattleEnd = "BattleEnd";
        public const string SetupTeam = "SetupTeam";
        public const string UpdateVip = "UpdateVip";
        public const string EndVip = "EndVip";
    }
    public class CommonConst
    {


    }

    public class EventNameConst
    {
        ////////////////////////////////// Publish events
        /// Battleserver
        public const string BattleServerRegisteredEvent = "BattleRegistered";
        public const string BattleServerRegisterFailedEvent = "BattleRegisterFailed";
        public const string BattleServerStateUpdatedEvent = "BattleStateUpdated";
        public const string BattleServerBattlesDestroyedEvent = "BattlesDestroyed";
        // Dragon
        public const string DragonUpdateLevelEvent = "dragon.req.updateLevel";
        public const string DragonBodypartUpdateLevelEvent = "dragonBodypart.req.updateLevel";
        public const string DragonRaceUpdateLevelEvent = "dragonRace.req.updateLevel";


        // Message
        public const string RequestSendMessageEvent = "RequestSendMessage";


        ////////////// Inventory
        // F2P Package events
        public const string ClaimF2PPackageRequestEvent = "inventory.ClaimF2PPackageRequest";
        public const string ClaimF2PPackageFailedEvent = "inventory.F2PPackageClaimFailed";
        public const string ClaimF2PPackageResponseEvent = "inventory.F2PPackageClaimResponse";


        // Inventory egg fragment combination
        public const string InventoryEggFragmentCombinationEvent = "inventory.EggFragmentCombieRequestEvent";
        public const string DragonEggCreatingResultByEggFragmentCombineEvent = "EggCreatingResultByEggFragmentCombine";

        public const string CreatingClaimableItemsRequestEvent = "inventory.CreateClaimableItemsRequest";
        public const string CreateClaimableItemsResponseEvent = "gameserver.listening.CreateClaimableItemsResponse";

        public const string ClaimRssEnergyRequestEvent = "inventory.ClaimRssEnergyRequestEvent";
        public const string ClaimRssEnergyResponseEvent = "gameserver.ClaimRssEnergyResponseEvent";



        public const string UsingFruitItemEvent = "inventory.UsingFruitItemEvent";
        public const string UpdateFruitItemUsedEvent = "gameserver.UpdateFruitItemUsedEvent";

        // Mailbox
        public const string CreatingMailRequestEvent = "mailbox.CreatingMailRequest";
        public const string NewMailCreatedEvent = "mailbox.NewMailCreated";


        // Assets generator request generate assets event
        public const string GenerateNftAssetsRequestEvent = "GenerateNftAssetsRequest";
        public const string NftAssetsGeneratedResponseEvent = "NftAssetsGeneratedResponse";

        // User events

        public const string DragonAPISyncUser = "rongosapi.syncUser";
        public const string UserConnectionHistoryEvent = "user.connectionHistory";


        // Master events
        public const string MasterLevelUpHistoryEvent = "user.levelHistory";
        public const string MasterRssUpdated = "master.rss.updated";
        public const string MasterLrssUpdated = "master.lrss.updated";
        public const string SyncMasterPremiumEvent = "sync.master.premium";

        public const string MasterInGameTokenUpdated = "master.ingametoken.updated";
        public const string MasterInGameItemUpdated = "master.item.updated";
        public const string MasterUserLevelUpHistoryEvent = "user.levelHistory";
        public const string MasterInGameCurrencyConsumeRequestEvent = "master.MasterInGameCurrencyConsumeRequest";
        public const string MasterInGameCurrencyConsumeResponseEvent = "master.MasterInGameCurrencyConsumeResponse";

        public const string MasterInGameCurrencyRevertRequestEvent = "master.MasterInGameCurrencyRevertRequest";
        public const string MasterInGameCurrencyRevertResponseEvent = "master.MasterInGameCurrencyRevertResponse";



        // Blockchain events
        public const string BlockchainEventsEvent = "blockchain-game-events";



        // Config version service
        public const string ConfigVersionUpdate = "config.version.update";

        /// <summary>
        /// Battle events
        /// </summary>
        public const string BattlePrepareRequestEvent = "battle.prepare.request";
        public const string BattlePrepareResponseEvent = "battle.prepare.response";

        public const string FindMatchAwaitValidationRequestEvent = "battle.FindMatchAwaitValidationRequest";
        public const string FindMatchValidationResponseEvent = "battle.FindMatchValidationResponse";

        public const string BattleCreatedEvent = "battle.created";
        public const string BattleCreateFailedEvent = "battle.create.failed";
        public const string BattleCompletedEvent = "battle.completed";
        public const string BattleCancelledEvent = "battle.cancelled";
        public const string BattleTimeoutEvent = "battle.timeout";
        public const string BattleSetupDragonFormationAwaitValidationRequestEvent = "battle.setupDragonFormation.request";
        public const string BattleSetupDragonFormationValidatedResponseEvent = "battle.setupDragonFormation.response";


        // batle
        public const string FindMatchRequestEvent = "FindMatchRequest";
        public const string CancelFindMatchRequestEvent = "CancelFindMatchRequest";
        public const string PVEBattleRequestEvent = "PVEBattleRequest";
        public const string PVPSoloBattleRequestEvent = "PVPSoloBattleRequest";
        public const string MatchFoundEvent = "MatchFound";
        public const string BattleSetupDragonFormationResultEvent = "battle.setupDragonFormation.result";

        // Blockchain event definitions

        public const string BCETHDeposit = "event.eth.deposit";
        public const string BCETHWithdraw = "event.eth.withdraw";
        public const string BCSigningResponseEvent = "event.res.signingMetaTx";
        public const string BCDragonBodypartUnlockSigningRequest = "DragonBodypartUnlockSigningRequest";
        public const string BCDragonBodypartUnlockSkillSigningRequest = "DragonBodypartUnlockSkillSigningRequest";
        public const string BCDragonRaceLevelUpSigningRequest = "DragonRaceLevelUpSigningRequest";
        public const string BCDragonBodypartLevelUpSigningRequest = "DragonBodypartLevelUpSigningRequest";

        public const string BCDragonMatingSigningRequest = "DragonMatingSigningRequest";
        public const string BCDragonWorkplaceCreateSigningRequest = "DragonWorkplaceCreateSigningRequest";
        public const string BCDragonWorkplaceLeasingSigningRequest = "DragonWorkplaceLeaseSigningRequest";
        public const string BCEggIncubatingSigningRequest = "EggIncubatingSigningRequest";
        public const string BCEggHatchingSigningRequest = "EggHatchingSigningRequest";


        public const string BCEggHatchedEvent = "event.egg.hatched";
        public const string BCIncubatedEvent = "event.egg.incubating";
        public const string BCSelfMatedEvent = "event.selfmate.created";

        public const string BCOrderMetaItemCreatedEvent = "event.order.metaitemcreated";
        public const string BCOrderMetaItemCancelledEvent = "event.order.metaitemcancelled";
        public const string BCOrderMetaItemFilledEvent = "event.order.metaitemfilled";
        public const string BCOrderDragonCreatedEvent = "event.order.rongoscreated";
        public const string BCOrderDragonCancelledEvent = "event.order.rongoscancelled";
        public const string BCOrderDragonFilledEvent = "event.order.rongosfilled";


        public const string BCCreateWorkplaceEvent = "event.workplace.create";
        public const string BCLeaseWorkplaceEvent = "event.workplace.lease";
        public const string BCClaimWorkplaceEvent = "event.workplace.claim";
        public const string BCCancelWorkplaceEvent = "event.workplace.cancel";

        public const string BCDragonCreated = "event.rongos.create";
        public const string BCEggCreated = "event.egg.create";
        public const string BCTransferDragonItem = "event.item.transfer.rongos";
        public const string BCTransferMetaItem = "event.item.transfer.meta";
        public const string WorkplaceClaimingRssEvent = "WorkplaceClaimingRssEvent";

        public const string MkpStoreMetaTxSigningRequestEvent = "mkp.StoreMetaTxSigningRequest";
        public const string MkpUserLinkWalletEvent = "mkp.user.linkWallet";
        public const string MkpSubUserSyncEvent = "mkp.syncSubUser";

        public const string AdminTransferItemRequest = "admin.TransferItemRequest";
        public const string PaymentItemEvent = "event.item.payment";
    }
}