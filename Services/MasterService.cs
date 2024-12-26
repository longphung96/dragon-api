
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

namespace DragonAPI.Services
{

    public class MasterService : BaseService<MasterService>
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
        private readonly ConfigLoader cfgLoader;
        private readonly IMediator _mediator;
        private readonly AsyncQueryableExecuter asyncQueryableExecuter;
        private readonly IMongoCollection<MasterBalanceTransactionHistoryDAO> masterBalanceTransactionHistoryRepo;
        private readonly IDatabase redisDatabase;
        private readonly DragonMongoDbContext mongoDbContext;
        private IItemMasterRepository _itemMasterRepository;
        private IMasterRepository _masterRepository;

        public const long VIP_DURATION_SECS = 60 * 60 * 4; // 4 hour

        public MasterService(
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
            ILogger<MasterService> logger, IMediator mediator) : base(logger, client, cfgLoader, mapper, capBus, cacheRepository, identityService)
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
            this.redisDatabase = redisConn.GetDatabase().WithKeyPrefix(redisSettingsOption.Value.Prefix);
            this._itemMasterRepository = _itemMasterRepository;
            this._masterRepository = _masterRepository;
        }
        public async Task<ApiResultDTO<string>> ChangeAvatar(string avatarid)
        {
            var result = new ApiResultDTO<string>();
            if (string.IsNullOrEmpty(avatarid))
            {
                result.AddError(ErrorCodeEnum.InvalidRequest);
            }
            var master = await userService.GetMasterAsync(identityService.CurrentUserId);
            if (master == null)
            {
                result.AddError(ErrorCodeEnum.InvalidRequest);
            }
            master.AvatarId = avatarid;
            await masterRepo.ReplaceOneAsync(m => m.Id == master.Id, master);
            result.Data = avatarid;

            return result;
        }
        public async Task<ApiResultDTO<bool>> UpgradeVIP(BuyPackageRequest param)
        {
            var result = new ApiResultDTO<bool>();
            result.Data = true;
            using (var session = this.client.StartSession())
            {
                session.StartTransaction();
                try
                {
                    var master = await masterRepo.Find(m => m.MainUserId == identityService.CurrentUserId).FirstOrDefaultAsync();
                    if (master == null)
                    {
                        throw new BusinessException(ErrorCodeEnum.EntityNotExisted, $"User does not exist");
                    }

                    if (master.vipExpiredAt > DateTime.UtcNow)
                    {
                        throw new BusinessException(ErrorCodeEnum.UnexpectedErr, $"Vip already activated");
                    }
                    var packageConfig = gameCfgLoader.GameConfigs.ShopPackageConfig.FirstOrDefault(sp => sp.Id == param.PackageId);
                    if (packageConfig == null)
                    {
                        throw new BusinessException(ErrorCodeEnum.UnexpectedErr, $"Package Config does not define");
                    }


                    var userItemExchange = await itemMasterRepo.Find(im => im.MainUserId == master.MainUserId && im.ItemId == packageConfig.CurrencyId).FirstOrDefaultAsync();
                    if (userItemExchange == null)
                    {
                        throw new BusinessException(ErrorCodeEnum.ItemNotEnought, $"{((Item)packageConfig.CurrencyId).ToString()} does not enough");
                    }
                    else
                    {
                        var validItem = await _itemMasterRepository.TakeItem(session, userItemExchange, packageConfig.FinalPrice, $"Buy package item {((Item)packageConfig.ItemId).ToString()}", logger);
                        if (validItem == false)
                        {
                            throw new BusinessException(ErrorCodeEnum.ItemNotEnought, $"{((Item)packageConfig.CurrencyId).ToString()} does not enough");
                        }
                    }
                    master.UpdatedAt = DateTime.UtcNow;
                    master.VipLevel = (int)userItemExchange.Quantity;
                    master.vipExpiredAt = DateTime.UtcNow.AddSeconds(VIP_DURATION_SECS);
                    await _masterRepository.Update(session, master);
                    await session.CommitTransactionAsync();
                    result.Data = true;
                }
                catch (System.Exception ex)
                {
                    logger.LogError(ex.StackTrace);
                    await session.AbortTransactionAsync();
                }
            }

            return result;
        }
        public async Task<ApiResultDTO<List<int>>> UpdateTutorial(int stepId)
        {
            var result = new ApiResultDTO<List<int>>();
            if (stepId < 0)
            {
                result.AddError(ErrorCodeEnum.InvalidRequest);
                return result;
            }
            var master = await masterRepo.Find(m => m.MainUserId == identityService.CurrentUserId).FirstOrDefaultAsync();
            if (master == null)
            {
                result.AddError(ErrorCodeEnum.InvalidRequest);
                return result;
            }
            if (!master.CompletedTutorialSteps.Contains(stepId))
            {
                master.CompletedTutorialSteps.Add(stepId);
                master.CompletedTutorialSteps = master.CompletedTutorialSteps.OrderBy(x => x).ToList();
                await masterRepo.ReplaceOneAsync(m => m.Id == master.Id, master);
            }
            result.Data = master.CompletedTutorialSteps;
            return result;
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
        public async Task RefreshTotalQuickClaim()
        {
            // var builder = Builders<MasterDAO>.Update;
            // await masterRepo.UpdateManyAsync(x => x.MainUserId != "", builder.Inc(x => x.TotalClaimReward, 0));
        }

        public async Task<ApiResultDTO<List<AFKIdleItemPoolDTO>>> GetIdleFarmingPools()
        {
            var runningPools = await idleAfkPoolRepo.Find(p => !p.Claimed && p.UserId == identityService.CurrentUserId).ToListAsync();
            var data = mapper.Map<List<IdleAfkPoolDAO>, List<AFKIdleItemPoolDTO>>(runningPools);
            return new ApiResultDTO<List<AFKIdleItemPoolDTO>> { Data = data };
        }



        public async Task<ApiResultDTO<bool>> ClaimOfflineReward(ClaimOfflineRewardRequest param)
        {
            var result = new ApiResultDTO<bool> { Data = true };
            using (var session = this.client.StartSession())
            {
                session.StartTransaction();
                try
                {
                    var master = await masterRepo.Find(m => m.MainUserId == identityService.CurrentUserId).FirstOrDefaultAsync();
                    if (master == null)
                    {
                        throw new BusinessException(ErrorCodeEnum.EntityNotExisted, "invalid user");
                    }
                    var runningPools = await idleAfkPoolRepo.Find(p => !p.Claimed && p.UserId == identityService.CurrentUserId).ToListAsync();
                    if (runningPools == null || runningPools?.Count <= 0)
                    {
                        throw new BusinessException(ErrorCodeEnum.EntityNotExisted, "No valid pool");
                    }


                    foreach (var runningPool in runningPools)
                    {
                        if (runningPool.EndFarmingAt > DateTime.UtcNow) throw new BusinessException(ErrorCodeEnum.InvalidRequest, "Please waiting for poool is full");

                        var duration = (runningPool.EndFarmingAt - runningPool.LastSnapShotAt).TotalSeconds;
                        runningPool.TempValue += (decimal)duration * runningPool.currentRate;
                        runningPool.Claimed = true;
                        runningPool.UpdatedAt = DateTime.UtcNow;
                        runningPool.LastSnapShotAt = DateTime.UtcNow;
                        await idleAfkPoolRepo.ReplaceOneAsync(p => p.Id == runningPool.Id && !p.Claimed, runningPool);

                        if (runningPool.ItemId == (long)Item.RSS)
                        {
                            await BonusCurrency(master.MainUserId, runningPool.ItemId, runningPool.TempValue, "farming pool claimed");
                        }
                        else if (runningPool.ItemId == (long)Item.CharacterEXP)
                        {
                            master.Exp += (ulong)runningPool.TempValue;
                        }
                    }

                    master.UpdatedAt = DateTime.UtcNow;
                    master.LastClaimRewardAt = DateTime.UtcNow;
                    await masterRepo.ReplaceOneAsync(m => m.Id == master.Id, master);

                    await session.CommitTransactionAsync();

                }
                catch (System.Exception ex)
                {
                    logger.LogError(ex.StackTrace);
                    await session.AbortTransactionAsync();
                    result.Data = false;
                }

            }
            // if everything is ok, create new pool
            CreatePoolFarmJob(identityService.CurrentUserId);

            return new ApiResultDTO<bool> { Data = true };
        }

        public async Task<ApiResultDTO<OffsetPagniationData<MasterDTO>>> GetMastersAsync(ListMasterFilteringRequestDto request)
        {
            var response = new ApiResultDTO<OffsetPagniationData<MasterDTO>>();
            var queryable = request.ApplyFilterTo(masterRepo.AsQueryable(new AggregateOptions { AllowDiskUse = true }));
            var count = await asyncQueryableExecuter.LongCountAsync(queryable);
            var items = await asyncQueryableExecuter.ToListAsync(queryable);
            var data = new OffsetPagniationData<MasterDTO>(
                mapper.Map<IReadOnlyList<MasterDAO>, IReadOnlyList<MasterDTO>>(items),
                count, request.Page, request.Size);
            response.Data = data;
            return response;

        }
        public async Task<ApiResultDTO<OffsetPagniationData<ItemMasterDTO>>> GetItemMastersAsync(ListItemMasterFilteringRequestDto request)
        {
            var response = new ApiResultDTO<OffsetPagniationData<ItemMasterDTO>>();
            var queryable = request.ApplyFilterTo(itemMasterRepo.AsQueryable(new AggregateOptions { AllowDiskUse = true }));
            var count = await asyncQueryableExecuter.LongCountAsync(queryable);
            queryable = queryable.Page(request.Page, request.Size);
            var items = await asyncQueryableExecuter.ToListAsync(queryable);
            var data = new OffsetPagniationData<ItemMasterDTO>(
                mapper.Map<IReadOnlyList<ItemMasterDAO>, IReadOnlyList<ItemMasterDTO>>(items),
                count, request.Page, request.Size);
            response.Data = data;
            return response;

        }
        public async Task<ApiResultDTO<ItemMasterDTO>> GetDetailItems(string Id)
        {
            var response = new ApiResultDTO<ItemMasterDTO>();
            var item = await itemMasterRepo.Find(m => m.Id == Id).FirstOrDefaultAsync();
            var data = mapper.Map<ItemMasterDAO, ItemMasterDTO>(item);
            response.Data = data;
            return response;
        }

        public async Task<List<MasterDAO>> GetMastersAsync(string[] masterIds)
        {
            var masters = await masterRepo.Find(m => masterIds.Contains(m.Id)).ToListAsync();
            return masters;
        }


        // public async Task<bool> BonusCurrency(string mainUserId, decimal rss, decimal lRss, string metadata = null)
        // {
        //     var master = await masterRepo.Find(m => m.MainUserId == mainUserId).FirstOrDefaultAsync();
        //     return await BonusCurrency(master, rss, lRss, metadata);
        // }
        public async Task<bool> BonusCurrency(string mainUserId, long itemId, decimal itemAmount, string metadata = null)
        {
            var item = await itemMasterRepo.Find(i => i.ItemId == itemId && i.MainUserId == mainUserId).FirstOrDefaultAsync();
            if (item == null)
            {
                item = new ItemMasterDAO
                {
                    MainUserId = mainUserId,
                    ItemId = itemId,
                    Quantity = itemAmount,
                    Name = ((Item)itemId).ToString(),
                };
                await itemMasterRepo.InsertOneAsync(item);
            }
            else
            {
                var result = await itemMasterRepo.ReplaceOneAsync(item.MainUserId, item);
                if (result.ModifiedCount <= 0) return false;
            }



            var tx = new MasterBalanceTransactionHistoryDAO
            {
                MainUserId = mainUserId,
                ItemId = itemId,
                Quantity = itemAmount,
                Metadata = metadata
            };
            await masterBalanceTransactionHistoryRepo.InsertOneAsync(tx);
            var master = await masterRepo.Find(m => m.MainUserId == item.MainUserId).FirstOrDefaultAsync();
            var logData = new
            {
                // WalletId = master.WalletId,
                MasterId = master.Id,
                MasterName = master.Name,
                MainUserId = master.MainUserId,
                ItemId = itemId,
                Quantity = itemAmount,
            };
            logger.LogInformation($"BonusRss {logData.ToAnalysisLog(LoggingContextEnum.RssContext)}");
            //await serverHub.Clients.Group(master.Id).MasterRssBonus(master.Id, (ulong)rss, (long)lRss, (ulong)master.Rss, (long)master.Lrss);
            // await serverHub.Clients.Group(master.Id).MasterRssUpdate(master.Id, (ulong)master.Rss);
            _ = eventBus.PublishAsync(EventNameConst.MasterInGameItemUpdated, new
            {
                MasterId = master.Id,
                MasterName = master.Name,
                MainUserId = master.MainUserId,
                ItemId = itemId,
                Quantity = itemAmount,
            });
            return true;
        }

        public async Task<bool> AddExp(MasterDAO master, long exp)
        {
            var oldLevel = master.Level;
            var levelConfig = GameConfigs.LevelExpConfig.EntityLevelConfigMap[Dragon.Blueprints.LevelTypeEnum.Master.Code];
            var (finalLevel, finalExp) = LevelCalculator.AddExp(master.Exp, exp, levelConfig.MaxLevel, levelConfig);
            master.Level = (ushort)finalLevel;
            master.Exp = (ulong)finalExp;
            var shouldTriggerJobEnergy = false;

            var result = await masterRepo.ReplaceOneAsync(m => m.Id == master.Id, master);
            if (result.ModifiedCount > 0)
            {
                if (oldLevel != finalLevel)
                {
                    var maxExp = levelConfig.GetExpConfigAtLevel(finalLevel).MaxExp;
                    //_ = serverHub.Clients.Group(master.Id).MasterLevelUp(master.Id, (ushort)finalLevel, (ulong)finalExp, (ulong)maxExp);
                    _ = eventBus.PublishAsync(EventNameConst.MasterUserLevelUpHistoryEvent, new MasterUserLevelUpHistoryIntegrationEvent
                    {
                        // WalletId = master.WalletId,
                        MainUserId = master.MainUserId,
                        Level = master.Level,
                        UpdatedAt = DateTime.UtcNow
                    });

                }
                return true;
            }
            return false;
        }


        public async Task ProcessETHDepositedEvent(ETHDepositedETO param)
        {
            var result = new ApiResultDTO<bool>();
            var master = await masterRepo.Find(m => m.Wallets.Any(w => w == param.AddressDeposit)).FirstOrDefaultAsync();
            var ChangeRateConfig = gameCfgLoader.GameConfigs.ChangeRateItem.Where(cr => cr.ExchangeItem == (long)Item.ETH && cr.ReceivedItem == (long)Item.RSS).FirstOrDefault();
            var rss = param.Amount * ChangeRateConfig.Rate;
            var userItem = await itemMasterRepo.Find(im => im.MainUserId == master.MainUserId && im.ItemId == (long)Item.RSS).FirstOrDefaultAsync();
            var itemData = GameConfigs.ItemConfigs.FirstOrDefault(i => i.Id == (long)Item.RSS);
            if (userItem == null)
            {
                userItem = new ItemMasterDAO
                {
                    MainUserId = master.MainUserId,
                    WalletId = master.MainUserId,
                    ItemId = (long)Item.RSS,
                    Name = itemData != null ? itemData.Name : "",
                    Quantity = rss,

                };
                await itemMasterRepo.InsertOneAsync(userItem);
            }
            else
            {

                userItem.Quantity += rss;
                await itemMasterRepo.ReplaceOneAsync(i => i.Id == userItem.Id, userItem);
            }
            master.UpdatedAt = DateTime.UtcNow;

            await masterRepo.ReplaceOneAsync(m => m.Id == master.Id, master);

        }
        public async Task<ApiResultDTO<string>> MasterWithdrawETH(string mainUserId, ETHWithdrawRequest param)
        {
            var result = new ApiResultDTO<string>();
            result.Data = string.Empty;
            var txId = SnowflakeId.Default().NextId().ToString();
            var master = await masterRepo.Find(m => m.MainUserId == mainUserId).FirstOrDefaultAsync();
            if (master.Wallets.Any(w => w == param.WalletId))
            {
                var logTransaction = new CurrencyTransactionDAO
                {
                    MainUserId = master.MainUserId,
                    WalletId = param.WalletId,
                    TxId = txId,
                    TransactionType = Enums.TransactionType.WithDraw,
                    ItemId = (long)Item.ETH,
                    Amount = param.Amount,
                    Status = Status.Processing,

                };
                await currencyTransactionRepo.InsertOneAsync(logTransaction);
                var withdrawVerify = new WithDrawVerificationDAO
                {
                    CurrencyTransactionId = logTransaction.Id,
                    TxId = txId,
                    Status = StatusVerify.Processing,
                };
                await withDrawVerificationRepo.InsertOneAsync(withdrawVerify);
                result.Data = txId;
            }
            result.Data = txId;
            return result;
        }





        public async Task<double> ProcessUpDragonFarmLevelRate(string UserId, string rongosId)
        {
            var master = await masterRepo.Find(m => m.MainUserId == UserId).FirstOrDefaultAsync();
            var formation = master.GetFormation(BattleType.PVE.ToString());
            double totalFarmLevel = 1;
            var rongosIds = formation.DragonIds.FindAll(id => id != "");
            if (rongosIds.Contains(rongosId))
            {
                var lstDragon = await rongosRepo.Find(r => rongosIds.Contains(r.Id)).ToListAsync();
                lstDragon.ForEach(m =>
                {
                    if (m.Id == rongosId) m.DragonFarmLevel -= 1;

                    var rongosFarmStat = GameConfigs.DragonFarmLevelStat.FirstOrDefault(l => l.Level == m.DragonFarmLevel);
                    if (rongosFarmStat != null)
                        totalFarmLevel += rongosFarmStat.FarmRateBonus;
                });
            }
            return totalFarmLevel;
        }
        public async Task<double> ProcessChangeDragonFormation(string UserId, List<string> rongosIds)
        {
            var master = await masterRepo.Find(m => m.MainUserId == UserId).FirstOrDefaultAsync();
            var formation = master.GetFormation(BattleType.PVE.ToString());
            double totalFarmLevel = 1;

            var lstDragon = await rongosRepo.Find(r => rongosIds.Contains(r.Id)).ToListAsync();
            lstDragon.ForEach(m =>
            {
                var rongosFarmStat = GameConfigs.DragonFarmLevelStat.FirstOrDefault(l => l.Level == m.DragonFarmLevel);
                if (rongosFarmStat != null)
                    totalFarmLevel += rongosFarmStat.FarmRateBonus;
            });

            return totalFarmLevel;
        }

        public async Task ProcessChangeOrCreatePoolAfk(string UserId)
        {
            var master = await masterRepo.Find(m => m.MainUserId == UserId).FirstOrDefaultAsync();
            var isVip = master.VipLevel > 0 && master.vipExpiredAt > DateTime.UtcNow;

            // TODO move to config
            var maxHour = GameConfigs.CommonSetting.FirstOrDefault(c => c.Key == "MaxHourAfkNormal").Value;
            if (isVip) maxHour = GameConfigs.CommonSetting.FirstOrDefault(c => c.Key == "MaxHourAfkVip").Value;

            var startTime = DateTime.UtcNow;
            var endtime = startTime.AddHours(Convert.ToInt32(maxHour));
            endtime = DateTimeMin(master.vipExpiredAt, endtime);
            if (endtime < startTime.AddHours(2))
                endtime = startTime.AddHours(2);

            var masterStagesTracking = await masterStageTrackingRepo.Find(t => t.MasterId == master.Id).ToListAsync();
            var mapIndex = masterStagesTracking.Max(m => m.MapIndex);
            var listStageMap = masterStagesTracking.Where(m => m.MapIndex == mapIndex).ToList();
            var Stage = listStageMap.MaxBy(x => x.Index);
            double farmLevelRate = 0;
            var formation = master.GetFormation(BattleType.PVE.ToString());
            var formationDragonIds = formation.DragonIds.FindAll(id => id != "");
            var lstDragon = await rongosRepo.Find(r => formationDragonIds.Contains(r.Id)).ToListAsync();
            lstDragon.ForEach(m =>
            {
                var rongosFarmStat = GameConfigs.DragonFarmLevelStat.FirstOrDefault(l => l.Level == m.DragonFarmLevel);
                if (rongosFarmStat != null)
                    farmLevelRate += rongosFarmStat.FarmRateBonus;
            });
            var lstrewards = GameConfigs.AfkRewardConfigs.FindAll(o => o.StageId.ToString() == Stage.StageId && o.Type == 0);
            foreach (var item in lstrewards)
            {
                var runningPool = await idleAfkPoolRepo.Find(p => p.UserId == UserId && p.Claimed && p.EndFarmingAt > startTime && p.ItemId == item.ItemId).FirstOrDefaultAsync();
                if (runningPool == null)
                {
                    runningPool = new IdleAfkPoolDAO
                    {
                        UserId = UserId,
                        ItemId = item.ItemId,
                        currentRate = item.Rate * (isVip ? 1.0M : 3.0M) * item.Quantity * (1 + (decimal)farmLevelRate), // 10% - 0.1
                        LastSnapShotAt = startTime,
                        startedAt = startTime,
                        EndFarmingAt = endtime
                    };
                    await idleAfkPoolRepo.InsertOneAsync(runningPool);
                }
                else
                {
                    runningPool.currentRate = item.Rate * (isVip ? 1.0M : 3.0M) * item.Quantity * (1 + (decimal)farmLevelRate);
                    runningPool.LastSnapShotAt = startTime;
                    runningPool.startedAt = startTime;
                    runningPool.EndFarmingAt = endtime;
                    await idleAfkPoolRepo.ReplaceOneAsync(p => p.Id == runningPool.Id, runningPool);
                }



            }
        }


        public void CreatePoolFarmJob(string UserId)
        {
            BackgroundJob.Enqueue(() => ProcessChangeOrCreatePoolAfk(UserId));
        }

        public async Task updateCurrentRatePoolItem(string userId, long itemId, decimal newRate, DateTime? toTime = null)
        {
            logger.LogDebug("newRate {0}", newRate);
            logger.LogDebug("itemId {0}", itemId);
            logger.LogDebug("userId {0}", userId);
            if (toTime == null)
                toTime = DateTime.UtcNow;
            var itemPool = await idleAfkPoolRepo.Find(p => p.UserId == userId && !p.Claimed && p.ItemId == itemId).FirstOrDefaultAsync();
            if (itemPool != null)
            {
                var duration = (DateTimeMin((DateTime)toTime, itemPool.EndFarmingAt) - itemPool.LastSnapShotAt).TotalSeconds;
                logger.LogDebug("toTime {0}", toTime);
                logger.LogDebug("itemPool.EndFarmingAt {0}", itemPool.EndFarmingAt);
                logger.LogDebug("itemPool.LastSnapShotAt {0}", itemPool.LastSnapShotAt);
                logger.LogDebug("duration {0}", duration);
                if (duration > 0)
                {
                    var snapshotValue = (decimal)duration * itemPool.currentRate;
                    itemPool.TempValue += snapshotValue;
                    itemPool.LastSnapShotAt = (DateTime)toTime;
                    itemPool.currentRate = newRate;
                    itemPool.UpdatedAt = DateTime.UtcNow;
                    await idleAfkPoolRepo.ReplaceOneAsync(p => p.Id == itemPool.Id, itemPool);
                }
            }
        }

        public async Task RefreshRateRunningPools(string userId)
        {
            // TODO adding distributed lock here
            var now = DateTime.UtcNow;
            var runningPools = await idleAfkPoolRepo.Find(p => p.UserId == userId && !p.Claimed && p.EndFarmingAt > now).ToListAsync();
            if (runningPools == null || runningPools.Count <= 0) return;

            var master = await masterRepo.Find(m => m.MainUserId == userId).FirstOrDefaultAsync();
            var isVip = master.VipLevel > 0 && master.vipExpiredAt > now;

            var masterStagesTracking = await masterStageTrackingRepo.Find(t => t.MasterId == master.Id).ToListAsync();
            var mapIndex = masterStagesTracking.Max(m => m.MapIndex);
            var listStageMap = masterStagesTracking.Where(m => m.MapIndex == mapIndex).ToList();
            var stage = listStageMap.MaxBy(x => x.Index);

            double rongosBonusFarmRate = 0;
            var formation = master.GetFormation(BattleType.PVE.ToString());
            var formationDragonIds = formation.DragonIds.FindAll(id => id != "");
            var lstDragon = await rongosRepo.Find(r => formationDragonIds.Contains(r.Id)).ToListAsync();
            lstDragon.ForEach(m =>
            {
                var rongosFarmStat = GameConfigs.DragonFarmLevelStat.FirstOrDefault(l => l.Level == m.DragonFarmLevel);
                if (rongosFarmStat != null)
                    rongosBonusFarmRate += rongosFarmStat.FarmRateBonus;
            });

            using (var session = this.client.StartSession())
            {
                session.StartTransaction();
                try
                {
                    foreach (var runningPool in runningPools)
                    {
                        var itemPool = GameConfigs.AfkRewardConfigs.FindAll(o => o.StageId.ToString() == stage.StageId && o.ItemId == runningPool.ItemId && o.Type == 0).FirstOrDefault();
                        if (itemPool != null)
                        {
                            var newRate = itemPool.Rate * (isVip ? 1.0M : 3.0M) * itemPool.Quantity * (decimal)(1 + rongosBonusFarmRate);
                            await updateCurrentRatePoolItem(userId, itemPool.ItemId, newRate);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    logger.LogError(ex.StackTrace);
                    await session.AbortTransactionAsync();

                }

            }
        }

        public void SnapshotFarmPoolJob(string userId)
        {
            BackgroundJob.Enqueue(() => RefreshRateRunningPools(userId));
        }

        public static DateTime DateTimeMin(DateTime time1, DateTime time2)
        {
            if (time1 < time2) return time1;
            return time2;
        }


        public async Task RecheckMasterVipLevelJobHourly()
        {
            var checkTime = DateTime.UtcNow.AddHours(1);
            var expiringInHourMasters = await masterRepo.Find(m => m.vipExpiredAt <= checkTime && m.VipLevel > 0).ToListAsync();
            foreach (var master in expiringInHourMasters)
            {
                BackgroundJob.Schedule(() => ClearVipLevelJob(master.MainUserId), master.vipExpiredAt);
            }
        }

        public async Task ClearVipLevelJob(string userId)
        {
            var master = await masterRepo.Find(m => m.MainUserId == userId && m.VipLevel > 0).FirstOrDefaultAsync();
            if (master is null) return;
            master.VipLevel = 0;
            master.UpdatedAt = DateTime.UtcNow;
            await masterRepo.ReplaceOneAsync(m => m.MainUserId == userId, master);
            SnapshotFarmPoolJob(userId);
        }


        public async Task CheckFarmPoolInit(string mainUserId)
        {
            logger.LogInformation("=== CheckFarmPoolInitJob {0}", mainUserId);
            var master = await masterRepo.Find(m => m.MainUserId == mainUserId).FirstOrDefaultAsync();
            if (master != null)
            {
                var stageIdReq = GameConfigs.CommonSetting.FirstOrDefault(c => c.Key == "StageUnLockAfkFarming").Value;
                var passedStage = await masterStageTrackingRepo.Find(t => t.MasterId == master.Id && t.StageId == stageIdReq).FirstOrDefaultAsync();
                if (passedStage is not null)
                {
                    var anyExisted = await idleAfkPoolRepo.Find(m => m.UserId == mainUserId && !m.Claimed).FirstOrDefaultAsync();
                    if (anyExisted == null)
                    {
                        await ProcessChangeOrCreatePoolAfk(mainUserId);
                    }
                }
            }
        }

        public void CheckFarmPoolInitJob(string mainUserId)
        {
            BackgroundJob.Enqueue(() => CheckFarmPoolInit(mainUserId));
        }
    }
}