using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCore.CAP;
using Hangfire;
using JohnKnoop.MongoRepository;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using DragonAPI.Application.Commands;
using DragonAPI.Common;
using DragonAPI.Configurations;
using DragonAPI.Data;
using DragonAPI.Enums;
using DragonAPI.Extensions;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Models.Cache;
using DragonAPI.Models.DAOs;
using DragonAPI.Models.DTOs;
using DragonAPI.Models.Entities;
using DragonAPI.Repositories;
using DragonAPI.Services;

namespace DragonAPI.IntegrationEvent.Handlers
{
    public class BlockchainIntegrationEventsHandler : ICapSubscribe
    {
        private const string BCWorkplaceCreatedEvent = "event.workplace.created";
        private const string BCWorkplaceCancelledEvent = "event.workplace.cancelled";
        private const string BCWorkplaceLeasedEvent = "event.workplace.leased";
        private const string BCWorkplaceClaimedEvent = "event.workplace.claimed";

        private readonly ILogger<BlockchainIntegrationEventsHandler> logger;
        private readonly IMongoCollection<DragonDAO> rongosRepo;
        private readonly IMongoCollection<UserDAO> userRepo;
        private readonly DragonService rongosService;
        private readonly DragonMongoDbContext mongoDbContext;
        private readonly GameConfigs configs;
        private readonly ICapPublisher capPublisher;
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        protected readonly ICacheRepository cacheRepository;

        public static List<long> ConfigEggTypes = new List<long>()
        {
            Dragon.Blueprints.ItemEnum.MatingEgg.TypeId,
            Dragon.Blueprints.ItemEnum.EggGold1.TypeId,
            Dragon.Blueprints.ItemEnum.EggWood1.TypeId,
            Dragon.Blueprints.ItemEnum.EggWater1.TypeId,
            Dragon.Blueprints.ItemEnum.EggFire1.TypeId,
            Dragon.Blueprints.ItemEnum.EggEarth1.TypeId,
            Dragon.Blueprints.ItemEnum.EggDark1.TypeId,
            Dragon.Blueprints.ItemEnum.EggLight1.TypeId,
            Dragon.Blueprints.ItemEnum.EggGold2.TypeId,
            Dragon.Blueprints.ItemEnum.EggWood2.TypeId,
            Dragon.Blueprints.ItemEnum.EggWater2.TypeId,
            Dragon.Blueprints.ItemEnum.EggFire2.TypeId,
            Dragon.Blueprints.ItemEnum.EggEarth2.TypeId,
            Dragon.Blueprints.ItemEnum.EggDark2.TypeId,
            Dragon.Blueprints.ItemEnum.EggLight2.TypeId,
            Dragon.Blueprints.ItemEnum.EggGold3.TypeId,
            Dragon.Blueprints.ItemEnum.EggWood3.TypeId,
            Dragon.Blueprints.ItemEnum.EggWater3.TypeId,
            Dragon.Blueprints.ItemEnum.EggFire3.TypeId,
            Dragon.Blueprints.ItemEnum.EggEarth3.TypeId,
            Dragon.Blueprints.ItemEnum.EggDark3.TypeId,
            Dragon.Blueprints.ItemEnum.EggLight3.TypeId,
            Dragon.Blueprints.ItemEnum.CommonEgg.TypeId,
            Dragon.Blueprints.ItemEnum.RareEgg.TypeId,
            Dragon.Blueprints.ItemEnum.UniqueEgg.TypeId,
            Dragon.Blueprints.ItemEnum.LegendaryEgg.TypeId,
            Dragon.Blueprints.ItemEnum.AncientMysteryEgg.TypeId,
        };

        public BlockchainIntegrationEventsHandler(
            ICacheRepository CacheRepository,
            DragonMongoDbContext mongoDbContext,
            DragonService rongosService,
            ConfigLoader configLoader,
            ICapPublisher capPublisher,
            IMapper mapper,
            IMediator mediator,
            ILogger<BlockchainIntegrationEventsHandler> logger)
        {
            this.cacheRepository = CacheRepository;
            this.logger = logger;
            this.rongosRepo = mongoDbContext.GetCollection<DragonDAO>("dragons");
            this.userRepo = mongoDbContext.GetCollection<UserDAO>("users");
            this.rongosService = rongosService;
            this.configs = configLoader.GameConfigs;
            this.capPublisher = capPublisher;
            this.mapper = mapper;
            this.mediator = mediator;
        }

        // // [CapSubscribe(EventNameConst.BlockchainEventsEvent)]
        // public async Task OnBlockchainEvents(BlockchainIntegrationEvent data)
        // {
        //     logger.LogInformation(data.ToRawJson());
        //     switch (data.eventName)
        //     {
        //         case BCWorkplaceCreatedEvent:
        //             await BCWorkplaceCreated(data.txHash, data.payload);
        //             break;
        //         case BCWorkplaceCancelledEvent:
        //             await BCWorkplaceCancelled(data.txHash, data.payload);
        //             break;
        //         case BCWorkplaceLeasedEvent:
        //             await BCWorkplaceLeased(data.txHash, data.payload);
        //             break;
        //         case BCWorkplaceClaimedEvent:
        //             await BCWorkplaceClaimed(data.txHash, data.payload);
        //             break;
        //         default:
        //             logger.LogInformation("undefined event handler");
        //             return;
        //     }
        // }

        public async Task ProcessBCOrderCreated(OrderCreatedETO bcOrderCreated, string bcEntityType = "dragon")
        {
            var nftId = Convert.ToUInt64(bcOrderCreated.TokenId);
            var entity = string.Empty;
            if (bcEntityType == "item")
            {
                var eggCfg = Dragon.Blueprints.ItemEnum.ItemEnumList.Find(i => i.TypeId == bcOrderCreated.TypeId && i.EntityType == "egg");
                if (eggCfg == null) return;
                entity = eggCfg.EntityType;
            }
            else if (bcEntityType == "dragon")
            {
                entity = "dragon";
            }
            else
            {
                return;
            }

            var placedOrderInfo = new PlacedOrderInfo
            {
                Id = bcOrderCreated.OrderId.ToString(),
                PriceStart = bcOrderCreated.Price,
                ListedAt = DateTimeOffset.FromUnixTimeSeconds(bcOrderCreated.EndedAt - bcOrderCreated.Duration).UtcDateTime,
                ListedTo = DateTimeOffset.FromUnixTimeSeconds(bcOrderCreated.EndedAt).UtcDateTime,
            };

            var notificationData = new MailNotification_TransferOrderCreatedOrCancelledData
            {
                OrderRecord = null,
                ItemType = convertToItemTypeFromString(entity),
                Entity = entity,
            };
            if (entity == "dragon")
            {
                var orderRecord = await rongosService.SyncBlockchainOrderCreated(nftId, placedOrderInfo);
                notificationData.OrderRecord = mapper.Map<DragonDAO, DragonDTO>(orderRecord);
            }
            if (notificationData.OrderRecord != null)
            {
                var mainUser = await userRepo.Find(u => u.Wallets.Any(w => w == bcOrderCreated.Owner) && u.MainUserId == null).FirstOrDefaultAsync();
                if (mainUser != null)
                {
                    #region Publish mail notify event
                    await capPublisher.PublishAsync(EventNameConst.CreatingMailRequestEvent, new CreatingMailboxRequestIntegrationEvent
                    {
                        MainUserId = mainUser.GetMainUserId(),
                        Title = "Marketplace Order Created",
                        Content = "Marketplace Order Created",
                        Type = MailboxType.Mail_Notify_Transfer_Order_Created,
                        NotificationData = JsonSerializer.Serialize(notificationData, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        })
                    });
                    #endregion
                    BackgroundJob.Schedule(() => NotifyOrderExpired(mainUser.GetMainUserId(), notificationData.ItemType, notificationData.Entity, bcOrderCreated.OrderId.ToString()), placedOrderInfo.ListedTo);
                }
            }
        }

        public async Task NotifyOrderExpired(string mainUserId, ItemTypeDragon itemType, string entity, string orderId)
        {
            var notificationData = new MailNotification_TransferOrderCreatedOrCancelledData
            {
                ItemType = itemType,
                Entity = entity,
            };

            if (itemType == ItemTypeDragon.Dragon)
            {
                var data = await rongosRepo.Find(d => d.PlacedOrderInfo.Id == orderId).FirstOrDefaultAsync();
                if (data == null || data.PlacedOrderInfo == null)
                    return;
                notificationData.OrderRecord = mapper.Map<DragonDAO, DragonDTO>(data);
            }
            if (notificationData.OrderRecord != null)
            {
                #region Publish mail notify event
                await capPublisher.PublishAsync(EventNameConst.CreatingMailRequestEvent, new CreatingMailboxRequestIntegrationEvent
                {
                    MainUserId = mainUserId,
                    Title = "Marketplace Order Expired",
                    Content = "Marketplace Order Expired",
                    Type = MailboxType.Mail_Notify_Transfer_Order_Expired,
                    NotificationData = JsonSerializer.Serialize(notificationData, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    })
                });
                #endregion
            }
        }

        public async Task ProcessBCOrderCancelled(OrderCancelledETO bcOrderCancelled, string bcEntityType = "dragon")
        {
            var nftId = Convert.ToUInt64(bcOrderCancelled.TokenId);

            var entity = string.Empty;
            if (bcEntityType == "item")
            {
                var eggCfg = Dragon.Blueprints.ItemEnum.ItemEnumList.Find(i => i.TypeId == bcOrderCancelled.TypeId && i.EntityType == "egg");
                if (eggCfg == null) return;
                entity = eggCfg.EntityType;
            }
            else if (bcEntityType == "dragon")
            {
                entity = "dragon";
            }
            else
            {
                return;
            }

            var notificationData = new MailNotification_TransferOrderCreatedOrCancelledData
            {
                OrderRecord = null,
                ItemType = convertToItemTypeFromString(entity),
                Entity = entity,
            };

            var ownerWallet = string.Empty;
            if (entity == "dragon")
            {
                var orderRecord = await rongosService.SyncBlockchainOrderCancelled(nftId);
                ownerWallet = orderRecord.WalletId;
                notificationData.OrderRecord = mapper.Map<DragonDAO, DragonDTO>(orderRecord);
            }
            if (notificationData.OrderRecord != null)
            {
                var mainUser = await userRepo.Find(u => u.Wallets.Any(w => w == ownerWallet) && u.MainUserId == null).FirstOrDefaultAsync();
                if (mainUser != null)
                {
                    #region Publish mail notify event
                    await capPublisher.PublishAsync(EventNameConst.CreatingMailRequestEvent, new CreatingMailboxRequestIntegrationEvent
                    {
                        MainUserId = mainUser.GetMainUserId(),
                        Title = "Marketplace Order Cancelled",
                        Content = "Marketplace Order Cancelled",
                        Type = MailboxType.Mail_Notify_Transfer_Order_Cancelled,
                        NotificationData = JsonSerializer.Serialize(notificationData, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        })
                    });
                    #endregion
                }
            }
        }

        public async Task ProcessBCOrderFilled(OrderFilledETO data, string bcEntityType = "dragon")
        {
            var nftId = Convert.ToUInt64(data.TokenId);
            var entity = string.Empty;
            if (bcEntityType == "item")
            {
                var eggCfg = Dragon.Blueprints.ItemEnum.ItemEnumList.Find(i => i.TypeId == data.TypeId && i.EntityType == "egg");
                if (eggCfg == null) return;
                entity = eggCfg.EntityType;
            }
            else if (bcEntityType == "dragon")
            {
                entity = "dragon";
            }
            else
            {
                return;
            }

            if (entity == "dragon")
            {
                await rongosService.SyncBlockchainOrderFilled(nftId, data.Buyer);
            }
        }

        // private async Task BCWorkplaceCreated(string txHash, JsonElement payload)
        // {
        //     var data = JsonSerializer.Deserialize<BlockchainWorkplaceOrderCreatedEto>(payload);
        //     await rongosService.SyncBlockchainWorkplaceCreated(data);
        // }
        // private async Task BCWorkplaceCancelled(string txHash, JsonElement payload)
        // {
        //     var data = JsonSerializer.Deserialize<BlockchainWorkplaceOrderCancelledEto>(payload);
        //     await rongosService.SyncBlockchainWorkplaceCancelled(data);
        // }
        // private async Task BCWorkplaceLeased(string txHash, JsonElement payload)
        // {
        //     var data = JsonSerializer.Deserialize<BlockchainWorkplaceOrderLeasedEto>(payload);
        //     await rongosService.SyncBlockchainWorkplaceLeased(data);
        // }
        // private async Task BCWorkplaceClaimed(string txHash, JsonElement payload)
        // {
        //     var data = JsonSerializer.Deserialize<BlockchainWorkplaceOrderClaimedEto>(payload);
        //     await rongosService.SyncBlockChainWorkplaceClaimed(data);
        // }

        private ItemTypeDragon convertToItemTypeFromString(string entity)
        {
            return entity switch
            {
                "dragon" => ItemTypeDragon.Dragon,
                "seal" => ItemTypeDragon.Seal,
                "nftbox" => ItemTypeDragon.NFTSalebox,
                "egg" => ItemTypeDragon.Egg,
                "sealBox" => ItemTypeDragon.SealBox,
                "ticket" => ItemTypeDragon.Ticket,
                "treasureKey" => ItemTypeDragon.TreasureKey,
                "building" => ItemTypeDragon.Building,
                "scroll" => ItemTypeDragon.TokenScroll,
                "dust" => ItemTypeDragon.TokenDust,
                "eggFragment" => ItemTypeDragon.TokenEggShard,
                "sealBoxFragment" => ItemTypeDragon.TokenSealBoxShard,
                "treasureKeyFragment" => ItemTypeDragon.TokenTreasureKeyShard,
                "buildingFragment" => ItemTypeDragon.TokenBuildingShard,
                _ => throw new System.NotImplementedException()
            };
        }
    }
}