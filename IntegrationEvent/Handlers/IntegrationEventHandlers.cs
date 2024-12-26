using DotNetCore.CAP;
using JohnKnoop.MongoRepository;
using Microsoft.Extensions.Logging;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Models.DAOs;
using DragonAPI.Services;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using DragonAPI.Extensions;
using AutoMapper;
using DragonAPI.Models.DTOs;
using DragonAPI.Configurations;
namespace DragonAPI.IntegrationEvent.Handlers
{
    public class IntegrationEventHandlers : ICapSubscribe
    {
        private readonly UserActionService userActionService;
        private readonly DragonService rongosService;
        private readonly MasterService masterService;
        private readonly IMarketPlaceService marketPlaceService;
        private readonly BlockchainIntegrationEventsHandler bcEventHandler;
        private readonly ILogger<IntegrationEventHandlers> logger;
        private readonly ConfigLoader gameCfgLoader;
        private readonly IMapper mapper;
        public IntegrationEventHandlers(
            IMapper mapper,
            UserActionService userActionService,
            DragonService rongosService,
            MasterService masterService,
            IMarketPlaceService marketPlaceService,
            ConfigLoader gameCfgLoader,
            BlockchainIntegrationEventsHandler bcEventHandler,
            ILogger<IntegrationEventHandlers> logger)
        {
            this.bcEventHandler = bcEventHandler;
            this.mapper = mapper;
            this.userActionService = userActionService;
            this.rongosService = rongosService;
            this.masterService = masterService;
            this.marketPlaceService = marketPlaceService;
            this.logger = logger;
            this.gameCfgLoader = gameCfgLoader;
        }
        protected GameConfigs GameConfigs => gameCfgLoader.GameConfigs;
        public T ToData<T>(object payload)
        {
            return JsonSerializer.Deserialize<T>((JsonElement)payload);
        }

        [CapSubscribe(IntegrationTopicConst.BattleService)]
        [CapSubscribe(IntegrationTopicConst.InventoryService)]
        [CapSubscribe(IntegrationTopicConst.BlockchainService)]
        public async Task OnIntegrationEventReceived(IntegrationMessageETO evtMessage)
        {
            logger.LogInformation($"OnIntegrationEventReceived {evtMessage.ToRawJson()}");
            switch (evtMessage.EventName)
            {
                // case EventNameConst.BattleCreatedEvent:
                //     logger.LogInformation("Handle logic on BattleCreatedEvent");
                //     break;
                // case EventNameConst.BattleCreateFailedEvent:
                //     logger.LogInformation("Handle logic on BattleCreateFailedEvent");
                //     break;
                // case EventNameConst.BattleCompletedEvent:
                //     logger.LogInformation("Handle logic on BattleCompletedEvent");
                //     await rongosService.ProcessBattleCompletedEvent(ToData<BattleCompletedETO>(evtMessage.Payload));
                //     break;
                // case EventNameConst.BattleSetupDragonFormationAwaitValidationRequestEvent:
                //     await rongosService.ProcessBattleSetupDragonFormationValidation(ToData<BattleSetupDragonFormationAwaitValidationResponseETO>(evtMessage.Payload));
                //     break;

                case EventNameConst.BCSigningResponseEvent:
                    {
                        logger.LogInformation("Handle logic on signing response");
                        var signingResult = ToData<SigningResponseETO>(evtMessage.Payload);
                        if (signingResult.SourceEntity == DragonService.Entity)
                        {
                            await rongosService.ProcessSigningMetaTxResponse(signingResult);
                        }

                        break;
                    }

                case EventNameConst.BCDragonCreated:
                    {
                        var data = ToData<BirthCreateETO>(evtMessage.Payload);
                        await rongosService.ProcessDragonCreatedEvent(data);
                        break;
                    }
                case EventNameConst.BCOrderMetaItemCreatedEvent:
                case EventNameConst.BCOrderDragonCreatedEvent:
                    {
                        var data = ToData<OrderCreatedETO>(evtMessage.Payload);
                        var entity = evtMessage.EventName == EventNameConst.BCOrderDragonCreatedEvent ? "dragon" : "item";
                        await bcEventHandler.ProcessBCOrderCreated(data, entity);
                        break;
                    }
                case EventNameConst.BCOrderMetaItemCancelledEvent:
                case EventNameConst.BCOrderDragonCancelledEvent:
                    {
                        var data = ToData<OrderCancelledETO>(evtMessage.Payload);
                        var entity = evtMessage.EventName == EventNameConst.BCOrderDragonCancelledEvent ? "dragon" : "item";
                        await bcEventHandler.ProcessBCOrderCancelled(data, entity);
                        break;
                    }
                case EventNameConst.BCOrderMetaItemFilledEvent:
                case EventNameConst.BCOrderDragonFilledEvent:
                    {
                        var data = ToData<OrderFilledETO>(evtMessage.Payload);
                        var entity = evtMessage.EventName == EventNameConst.BCOrderDragonFilledEvent ? "dragon" : "item";
                        await bcEventHandler.ProcessBCOrderFilled(data, entity);
                        break;
                    }
                case EventNameConst.BCETHDeposit:
                    {
                        var data = ToData<ETHDepositedETO>(evtMessage.Payload);
                        await masterService.ProcessETHDepositedEvent(data);
                        break;
                    }
                // case EventNameConst.BCCreateWorkplaceEvent:
                //     {
                //         var data = ToData<BlockchainWorkplaceOrderCreatedEto>(evtMessage.Payload);
                //         await rongosService.SyncBlockchainWorkplaceCreated(data);
                //         break;
                //     }
                // case EventNameConst.BCLeaseWorkplaceEvent:
                //     {
                //         var data = ToData<BlockchainWorkplaceOrderLeasedEto>(evtMessage.Payload);
                //         await rongosService.SyncBlockchainWorkplaceLeased(data);
                //         break;
                //     }
                // case EventNameConst.BCClaimWorkplaceEvent:
                //     {
                //         var data = ToData<BlockchainWorkplaceOrderClaimedEto>(evtMessage.Payload);
                //         await rongosService.SyncBlockChainWorkplaceClaimed(data);
                //         break;
                //     }
                // case EventNameConst.BCCancelWorkplaceEvent:
                //     {
                //         var data = ToData<BlockchainWorkplaceOrderCancelledEto>(evtMessage.Payload);
                //         await rongosService.SyncBlockchainWorkplaceCancelled(data);
                //         break;
                //     }
                case EventNameConst.BCTransferDragonItem:
                    {
                        var data = ToData<TransferETO>(evtMessage.Payload);
                        await rongosService.ProcessDragonTransferEvent(data);
                        break;
                    }
                case EventNameConst.PaymentItemEvent:
                    {
                        var data = ToData<PaymentETO>(evtMessage.Payload);
                        if (data.PartnerCode != null && data.ItemKeys != null)
                        {
                            var partnerCode = GameConfigs.CommonSetting.FirstOrDefault(c => c.Key == "PartnerCode").Value;
                            if (data.PartnerCode.ToLower() == partnerCode.ToLower())
                                await marketPlaceService.ProcessPaymentItemEvent(data);
                        }

                        break;
                    }
                default:
                    logger.LogDebug("non-handler found");
                    break;
            }
        }
    }
}
