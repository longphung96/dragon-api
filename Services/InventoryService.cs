
using AutoMapper;
using DotNetCore.CAP;
using JohnKnoop.MongoRepository;
using MediatR;
using MongoDB.Driver;
using DragonAPI.Common;
using DragonAPI.Enums;
using DragonAPI.Extensions;
using DragonAPI.Helpers;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Models.DAOs;
using DragonAPI.Models.DTOs;
using DragonAPI.Repositories;
using Volo.Abp.Linq;
using DotNetCore.CAP.Internal;
using StackExchange.Redis;
using Microsoft.Extensions.Options;
using DragonAPI.Configurations;
using StackExchange.Redis.KeyspaceIsolation;
using System.Data;
using Nethereum.Web3;
using System.Linq.Dynamic.Core;
using DragonAPI.Data;
using Hangfire;
using System.Text.Json;
using static DragonAPI.Services.BattleService;
using RedLockNet.SERedis;

namespace DragonAPI.Services
{

    public class InventoryService : BaseService<InventoryService>
    {
        private readonly UserService userService;
        private readonly IConfiguration configuration;
        private readonly IMongoCollection<MasterDAO> masterRepo;
        private readonly IMongoCollection<DragonDAO> rongosRepo;
        private readonly IMongoCollection<ItemMasterDAO> itemMasterRepo;
        private readonly IMongoCollection<CurrencyTransactionDAO> currencyTransactionRepo;
        private readonly IMongoCollection<MasterStageTrackingDAO> masterStageTrackingRepo;
        private readonly IMongoCollection<IdleAfkPoolDAO> idleAfkPoolRepo;
        private readonly IMongoCollection<WithDrawVerificationDAO> withDrawVerificationRepo;
        private readonly IMongoCollection<HistoryActionChangeRateAfkDAO> historyActionChangeRateAfkRepo;
        private readonly IMongoCollection<RankPositionDAO> rankPositionRepo;
        private readonly IMongoCollection<UserBotDAO> userBotRepo;
        private readonly IMongoCollection<ClaimableItemDAO> claimableItemRepo;
        private readonly ConfigLoader cfgLoader;
        private readonly IMediator _mediator;
        private readonly AsyncQueryableExecuter asyncQueryableExecuter;
        private readonly IMongoCollection<MasterBalanceTransactionHistoryDAO> masterBalanceTransactionHistoryRepo;
        private readonly IDatabase redisDatabase;
        private readonly DragonMongoDbContext mongoDbContext;
        private IItemMasterRepository _itemMasterRepository;
        private readonly RedLockFactory redLockFactory;
        private IMasterRepository _masterRepository;

        public const long VIP_DURATION_SECS = 60 * 60 * 4; // 4 hour

        public InventoryService(
            IMongoClient client,
            AsyncQueryableExecuter asyncQueryableExecuter,
            ICacheRepository cacheRepository,
            IConnectionMultiplexer redisConn,
            IOptions<RedisSettings> redisSettingsOption,
            IItemMasterRepository _itemMasterRepository,
            IMasterRepository _masterRepository,
            ConfigLoader cfgLoader,
            IMapper mapper,
            ICapPublisher capBus,
            IdentityService identityService,
            IConfiguration configuration,
            DragonMongoDbContext mongoDbContext,
            ILogger<InventoryService> logger, IMediator mediator) : base(logger, client, cfgLoader, mapper, capBus, cacheRepository, identityService)
        {

            this.asyncQueryableExecuter = asyncQueryableExecuter;

            this.cfgLoader = cfgLoader;
            this._mediator = mediator;
            this.masterRepo = mongoDbContext.GetCollection<MasterDAO>("masters");
            this.masterStageTrackingRepo = mongoDbContext.GetCollection<MasterStageTrackingDAO>("master_stage_tracking");
            this.itemMasterRepo = mongoDbContext.GetCollection<ItemMasterDAO>("item_master");
            this.rongosRepo = mongoDbContext.GetCollection<DragonDAO>("dragons");
            this.idleAfkPoolRepo = mongoDbContext.GetCollection<IdleAfkPoolDAO>("idle_afk_pool");
            this.historyActionChangeRateAfkRepo = mongoDbContext.GetCollection<HistoryActionChangeRateAfkDAO>("history_action_change");
            this.masterBalanceTransactionHistoryRepo = mongoDbContext.GetCollection<MasterBalanceTransactionHistoryDAO>("masterBalanceTransactionHistories");
            this.currencyTransactionRepo = mongoDbContext.GetCollection<CurrencyTransactionDAO>("currency_transaction");
            this.withDrawVerificationRepo = mongoDbContext.GetCollection<WithDrawVerificationDAO>("withdraw_verification");
            this.rankPositionRepo = mongoDbContext.GetCollection<RankPositionDAO>("rankPosition");
            this.userBotRepo = mongoDbContext.GetCollection<UserBotDAO>("user_bot");
            this.claimableItemRepo = mongoDbContext.GetCollection<ClaimableItemDAO>("claim_items");
            this.redisDatabase = redisConn.GetDatabase().WithKeyPrefix(redisSettingsOption.Value.Prefix);
            this._itemMasterRepository = _itemMasterRepository;
            this._masterRepository = _masterRepository;
        }


        public async Task<ApiResultDTO<bool>> InsertItem(string userId, long itemId, long Amount)
        {
            var result = new ApiResultDTO<bool>();
            var item = GameConfigs.ItemConfigs.Find(i => i.Id == itemId);
            var userItem = new ItemMasterDAO
            {
                ItemId = item.Id,
                Name = item.Name,
                Quantity = (long)Amount,
                MainUserId = userId,
            };
            await itemMasterRepo.InsertOneAsync(userItem);
            return result;
        }

        public async Task<ApiResultDTO<ItemMasterDTO>> GetDetailItems(string Id)
        {
            var response = new ApiResultDTO<ItemMasterDTO>();
            var item = await itemMasterRepo.Find(m => m.Id == Id).FirstOrDefaultAsync();
            var data = mapper.Map<ItemMasterDAO, ItemMasterDTO>(item);
            response.Data = data;
            return response;
        }

        public async Task<bool> ClaimItems(string RequestId)
        {



            await using (var redLock = await redLockFactory.CreateLockAsync(RequestId, TimeSpan.FromSeconds(10))) // there are also non async Create() methods
            {
                // make sure we got the lock
                if (redLock.IsAcquired)
                {
                    var master = await masterRepo.Find(m => m.MainUserId == identityService.CurrentUserId).FirstOrDefaultAsync();
                    var checkClaims = await claimableItemRepo.Find(r => r.RequestId == RequestId && r.State == ClaimState.Waiting.ToString()).ToListAsync();
                    if (checkClaims.Count == 0) return false;
                    foreach (var claimItem in checkClaims)
                    {
                        if (claimItem.ExpiredTime < DateTime.UtcNow) return false;
                        var userItem = await itemMasterRepo.Find(im => im.ItemId == claimItem.ItemId && im.MainUserId == master.MainUserId).FirstOrDefaultAsync();
                        var item = GameConfigs.ItemConfigs.FirstOrDefault(i => i.Id == claimItem.ItemId);
                        if (userItem == null)
                        {
                            userItem = new ItemMasterDAO
                            {
                                ItemId = claimItem.ItemId,
                                Name = item.Name,
                                Quantity = (long)claimItem.Quantity,
                                MainUserId = master.MainUserId,
                            };
                            await itemMasterRepo.InsertOneAsync(userItem);

                        }
                        else
                        {
                            userItem.Quantity += (long)claimItem.Quantity;
                            await itemMasterRepo.ReplaceOneAsync(m => m.Id == userItem.Id, userItem);
                        }
                    }
                }
            }

            return true;
        }




        public async Task ProcessSendDailyRankingReward()
        {
            var listBot = await userBotRepo.Find(Builders<UserBotDAO>.Filter.Empty).ToListAsync();
            var bots = listBot.Select(b => b.BotUserId).ToList();
            var rankStats = await rankPositionRepo.Find(Builders<RankPositionDAO>.Filter.Empty).SortBy(r => r.RankPosition).Limit(100).ToListAsync();
            foreach (var rank in rankStats)
            {
                if (bots.Contains(rank.MainUserId)) continue;
                var dailyReward = GameConfigs.DailyRankingRewardConfig.Where(d => d.RankFrom <= rank.RankPosition && d.RankTo >= rank.RankPosition).ToList();
                var claimItems = new List<ClaimableItemEto>();
                var reqId = SnowflakeId.Default().NextId().ToString();
                foreach (var dropItem in dailyReward)
                {
                    claimItems.Add(new ClaimableItemEto
                    {
                        TemplateItemId = dropItem.ItemId,
                        Amount = (int)dropItem.Quantity,
                        Entity = "",
                        Type = -1,
                        ExpiredTime = DateTime.UtcNow.AddDays(7)
                    });
                    logger.LogInformation("send request to create claim items");

                    var ClaimRequestItem = new ClaimableItemDAO
                    {
                        RequestId = reqId,
                        MainUserId = rank.MainUserId,
                        LifetimeHours = 168,
                        ExpiredTime = DateTime.UtcNow.AddDays(7),
                        ItemId = dropItem.ItemId,
                        Quantity = (int)dropItem.Quantity,
                    };
                    await claimableItemRepo.InsertOneAsync(ClaimRequestItem);
                }
                if (claimItems.Count > 0)
                {

                    _ = eventBus.PublishAsync(EventNameConst.CreatingMailRequestEvent, new CreatingMailboxRequestIntegrationEvent
                    {

                        MainUserId = rank.MainUserId,
                        RequestId = reqId,
                        Title = "Daily ranking reward Items: Getting rewards",
                        Content = "Congratulations on getting rewards from the drop items",
                        Type = MailboxType.Mail_Reward_ItemDrop,
                        ExpiredTime = DateTime.UtcNow.AddDays(7),
                        RewardData = new MailRewardDataEto
                        {
                            Items = claimItems,
                        }
                    });
                }
            }

        }
        public async Task ProcessSendSeasonRankingReward()
        {
            var listBot = await userBotRepo.Find(Builders<UserBotDAO>.Filter.Empty).ToListAsync();
            var bots = listBot.Select(b => b.BotUserId).ToList();
            var rankStats = await rankPositionRepo.Find(Builders<RankPositionDAO>.Filter.Empty).SortBy(r => r.RankPosition).Limit(1000).ToListAsync();

            foreach (var rank in rankStats)
            {
                if (bots.Contains(rank.MainUserId)) continue;
                var season = gameCfgLoader.GameConfigs.SeasonConfigs.Where(s => s.Id == rank.SeasonId).FirstOrDefault();
                if (season.EndingTime > DateTime.UtcNow) continue;
                var checkSeasonDuplicate = await claimableItemRepo.Find(c => c.MainUserId == rank.MainUserId && c.seasonId == rank.SeasonId && c.RewardType == RewardType.Season).FirstOrDefaultAsync();
                if (checkSeasonDuplicate != null) continue;
                var seasonReward = GameConfigs.SeasonRankingRewardConfig.Where(d => d.RankFrom <= rank.RankPosition && d.RankTo >= rank.RankPosition && rank.SeasonId == rank.SeasonId).ToList();
                var claimItems = new List<ClaimableItemEto>();
                var reqId = SnowflakeId.Default().NextId().ToString();
                foreach (var dropItem in seasonReward)
                {
                    claimItems.Add(new ClaimableItemEto
                    {
                        TemplateItemId = dropItem.ItemId,
                        Amount = (int)dropItem.Quantity,
                        Entity = "",
                        Type = -1,
                        ExpiredTime = DateTime.UtcNow.AddDays(7)
                    });
                    logger.LogInformation("send request to create claim items");

                    var ClaimRequestItem = new ClaimableItemDAO
                    {
                        RequestId = reqId,
                        MainUserId = rank.MainUserId,
                        LifetimeHours = 168,
                        ExpiredTime = DateTime.UtcNow.AddDays(7),
                        ItemId = dropItem.ItemId,
                        Quantity = (int)dropItem.Quantity,
                        seasonId = rank.SeasonId,
                        RewardType = RewardType.Season
                    };
                    await claimableItemRepo.InsertOneAsync(ClaimRequestItem);
                }
                if (claimItems.Count > 0)
                {

                    _ = eventBus.PublishAsync(EventNameConst.CreatingMailRequestEvent, new CreatingMailboxRequestIntegrationEvent
                    {

                        MainUserId = rank.MainUserId,
                        RequestId = reqId,
                        Title = "Season ranking reward Items: Getting rewards",
                        Content = "Congratulations on getting rewards from the drop items",
                        Type = MailboxType.Mail_Reward_ItemDrop,
                        ExpiredTime = DateTime.UtcNow.AddDays(7),
                        RewardData = new MailRewardDataEto
                        {
                            Items = claimItems,
                        }
                    });
                }
            }

        }

    }
}