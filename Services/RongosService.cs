using AutoMapper;
using DotNetCore.CAP;
using JohnKnoop.MongoRepository;
using MongoDB.Driver;
using RedLockNet;
using DragonAPI.Models.DTOs;
using DragonAPI.Common;
using DragonAPI.Enums;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Models.DAOs;
using DragonAPI.Repositories;
using System.Linq.Dynamic.Core;
using Volo.Abp.Linq;
// using Dragon.Blueprints;
// using TrueSight.Lite.Common;
using DragonAPI.Models.Entities;
using DragonAPI.Helpers;
using DragonAPI.Extensions;
using StackExchange.Redis.Extensions.Core.Abstractions;
using Hangfire;
using System.Text.Json;
using MediatR;
using DragonAPI.Application.Commands;
using System.Text.RegularExpressions;
using DragonAPI.Models.Cache;
using DragonAPI.Configurations;
using DragonAPI.Utils;
using DragonAPI.Data;
using System.Security.Cryptography;

namespace DragonAPI.Services
{
    public class DragonService : BaseService<DragonService>
    {
        public const string Entity = "Dragon";
        private readonly IMongoCollection<DragonDAO> rongosRepo;
        private readonly IMongoCollection<ItemMasterDAO> itemMasterRepo;
        private readonly IMongoCollection<MasterDAO> masterRepo;
        private readonly IMongoCollection<MasterStageTrackingDAO> masterStageTrackingRepo;
        private readonly AsyncQueryableExecuter asyncQueryableExecuter;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IDistributedLockFactory lockFactory;
        private readonly UserActionService userActionService;
        private readonly MasterService masterService;
        private readonly IMongoCollection<UserDAO> userRepo;

        private readonly UserService userService;
        private readonly DragonMongoDbContext mongoDbContext;
        private readonly string KeyGroupKey = CacheKeys.CachedData + ":rongoses";
        private IItemMasterRepository _itemMasterRepository;
        private readonly IMediator mediator;
        public const long MaxLevelDragonOffchain = 50;
        public const long MaxLevelDragonOnchain = 70;
        public const long DefaultValidDurationSigningRequest = 5;
        public DragonService(
            UserActionService userActionService,
            IDistributedLockFactory lockFactory,
            IHttpContextAccessor httpContextAccessor,
            ICacheRepository cacheRepository,
            IRedisDatabase redisDatabase,
            IMongoClient client,
            AsyncQueryableExecuter asyncQueryableExecuter,
            ConfigLoader cfgLoader,
            IMapper mapper,
            ICapPublisher capPublisher,
            IMediator mediator,
            IdentityService identityService,
            UserService userService,
            MasterService masterService,
            ILogger<DragonService> logger,
            DragonMongoDbContext mongoDbContext,
            IItemMasterRepository _itemMasterRepository
            )
            : base(logger, client,
                  cfgLoader,
                  mapper,
                  capPublisher,
                  cacheRepository, identityService)
        {
            this.lockFactory = lockFactory;
            this.httpContextAccessor = httpContextAccessor;
            this.asyncQueryableExecuter = asyncQueryableExecuter;
            this.userActionService = userActionService;
            this.mediator = mediator;
            this.userService = userService;
            this.masterService = masterService;
            this.userRepo = mongoDbContext.GetCollection<UserDAO>("users");
            this.masterRepo = mongoDbContext.GetCollection<MasterDAO>("masters");
            this.rongosRepo = mongoDbContext.GetCollection<DragonDAO>("dragons");
            this.itemMasterRepo = mongoDbContext.GetCollection<ItemMasterDAO>("item_master");
            this.masterStageTrackingRepo = mongoDbContext.GetCollection<MasterStageTrackingDAO>("master_stage_tracking");
            this._itemMasterRepository = _itemMasterRepository;
        }

        public async Task<ApiResultDTO<OffsetPagniationData<DragonDTO>>> List(DragonFilter request)
        {
            var response = new ApiResultDTO<OffsetPagniationData<DragonDTO>>();
            // var key = HttpRequestCacheKeyBuilder.BuildKey(httpContextAccessor.HttpContext, KeyGroupKey);
            // var data = await cacheRepository.GetFromCache<OffsetPagniationData<DragonDTO>>(key);
            // if (data == null)
            // {
            //     var lockExpiry = TimeSpan.FromSeconds(20);
            //     var lockWait = TimeSpan.FromSeconds(5);
            //     var lockRetry = TimeSpan.FromMilliseconds(2500);
            //     // blocks until acquired or 'wait' timeout
            //     await using (var redLock = await lockFactory.CreateLockAsync(key, lockExpiry, lockWait, lockRetry)) // there are also non async Create() methods
            //     {
            //         // make sure we got the lock
            //         if (redLock.IsAcquired)
            //         {
            //             data = await cacheRepository.GetFromCache<OffsetPagniationData<DragonDTO>>(key);
            //             if (data == null)
            //             {
            var queryable = request.ApplyFilterTo(rongosRepo.AsQueryable(new AggregateOptions { AllowDiskUse = true })).FilterOwnDragonesByWallets(request.WalletId);
            var count = await asyncQueryableExecuter.LongCountAsync(queryable);
            if (string.IsNullOrEmpty(request.Sorting))
            {
                queryable = queryable.OrderByDescending(r => r.Birthday).ThenBy(r => r.Id);
            }
            else
            {
                queryable = queryable.OrderBy(request.Sorting).ThenBy(r => r.Id);
            }
            queryable = queryable.Page(request.Page, request.Size);
            logger.LogDebug($"query explain {queryable}");
            var items = await asyncQueryableExecuter.ToListAsync(queryable);
            var data = new OffsetPagniationData<DragonDTO>(
               mapper.Map<IReadOnlyList<DragonDAO>, IReadOnlyList<DragonDTO>>(items),
               count, request.Page, request.Size);
            response.Data = data;
            //                 _ = cacheRepository.SetToCache(key, data, TimeSpan.FromSeconds(30));
            //                 logger.LogDebug("hit database");BusinessException
            //             }
            //         }
            //         else
            //         {
            //             logger.LogDebug("cannot acquire the lock => retry again");
            //         }
            //     }
            // }
            // else
            // {
            //     response.Data = data;
            //     logger.LogDebug("hit cache");
            // }

            return response;
        }

        public async Task<ApiResultDTO<OffsetPagniationData<DragonDTO>>> GetMyDragons(MyDragonFilter request)
        {
            var response = new ApiResultDTO<OffsetPagniationData<DragonDTO>>();
            var queryable = request.ApplyFilterTo(rongosRepo.AsQueryable(new AggregateOptions { AllowDiskUse = true })).FilterOwnDragonesByWallets(request.WalletId);
            var count = await asyncQueryableExecuter.LongCountAsync(queryable);
            if (string.IsNullOrEmpty(request.Sorting))
            {
                queryable = queryable.OrderByDescending(r => r.Birthday).ThenBy(r => r.Id);
            }
            else
            {
                queryable = queryable.OrderBy(request.Sorting).ThenBy(r => r.Id);
            }
            queryable = queryable.Page(request.Page, request.Size);
            logger.LogDebug($"query explain {queryable}");
            var items = await asyncQueryableExecuter.ToListAsync(queryable);
            var data = new OffsetPagniationData<DragonDTO>(
               mapper.Map<IReadOnlyList<DragonDAO>, IReadOnlyList<DragonDTO>>(items),
               count, request.Page, request.Size);
            response.Data = data;
            return response;
        }
        public async Task<ApiResultDTO<DragonDTO>> Get(string id)
        {
            var response = new ApiResultDTO<DragonDTO>();
            var dragon = await rongosRepo.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (dragon == null)
                response.AddError(ErrorCodeEnum.InvalidRequest, $"Id does not exist");
            else
                response.Data = mapper.Map<DragonDAO, DragonDTO>(dragon);
            return response;
        }
        public async Task<ApiResultDTO<bool>> DragonLevelUp(string id)
        {
            var result = new ApiResultDTO<bool>();
            result.Data = true;
            var item1 = new ItemMasterDAO();
            var item2 = new ItemMasterDAO();
            var item3 = new ItemMasterDAO();
            var master = await userService.GetMasterAsync(identityService.CurrentUserId);
            var rongos = await rongosRepo.Find
            (
                r => r.Id == id && master.Wallets.Contains(r.WalletId) && r.SaleStatus == SaleStatus.NotSale
            ).FirstOrDefaultAsync();
            if (rongos == null)
            {
                result.AddError(ErrorCodeEnum.EntityNotExisted);
                result.Data = false;
                return result;
            }

            var config = GameConfigs.DragonUpgradeConfigs.Where(x => x.DragonElement.Code.ToLower() == rongos.Class.ToString().ToLower() && x.NextLevelUpdate == rongos.Level + 1).FirstOrDefault();
            if (config == null)
            {
                result.AddError(ErrorCodeEnum.InvalidRequest, $"level is invalid");
                result.Data = false;
                return result;
            }
            if (config.ItemId1 != null)
            {
                item1 = await itemMasterRepo.Find(i => i.ItemId == config.ItemId1 && i.MainUserId == master.MainUserId).FirstOrDefaultAsync();
                if (item1 == null || item1.Quantity < config.AmountItem1)
                {
                    result.AddError(ErrorCodeEnum.UnexpectedErr, $"Material is not enough");
                    result.Data = false;
                    return result;
                }

            }
            if (config.ItemId2 != null)
            {
                item2 = await itemMasterRepo.Find(i => i.ItemId == config.ItemId2 && i.MainUserId == master.MainUserId).FirstOrDefaultAsync();
                if (item2 == null || item2.Quantity < config.AmountItem2)
                {
                    result.AddError(ErrorCodeEnum.UnexpectedErr, $"Material is not enough");
                    result.Data = false;
                    return result;
                }

            }
            if (config.ItemId3 != null)
            {
                item3 = await itemMasterRepo.Find(i => i.ItemId == config.ItemId3 && i.MainUserId == master.MainUserId).FirstOrDefaultAsync();
                if (item3 == null || item3.Quantity < config.AmountItem3)
                {
                    result.AddError(ErrorCodeEnum.UnexpectedErr, $"Material is not enough");
                    result.Data = false;
                    return result;
                }

            }


            if (config.BodyPartTypeUpdate != null)
            {
                var type = GameConfigs.BodypartTypeMap.FirstOrDefault(bp => bp.Id == config.BodyPartTypeUpdate).Code;
                if (type != null)
                {
                    var bodypart = rongos.Bodyparts.Find(p => p.Type.ToString() == type && p.Unlocked);
                    if (bodypart == null)
                    {
                        result.AddError(ErrorCodeEnum.InvalidRequest, $"Bodypart is invalid");
                        result.Data = false;
                        return result;
                    }
                    bodypart.Level = (int)config.BodyPartLevelUpdate;
                }

            }
            rongos.Level = (ushort)config.NextLevelUpdate;
            rongos.UpdatedAt = DateTime.UtcNow;
            var resultDragon = await rongosRepo.ReplaceOneAsync(m => m.Id == id, rongos);
            if (resultDragon.ModifiedCount < 0)
            {
                result.AddError(ErrorCodeEnum.UnexpectedErr);
                result.Data = false;
                return result;
            }
            if (config.ItemId1 != null)
            {
                item1.Quantity -= config.AmountItem1 != null ? (long)config.AmountItem1 : 0;
                await itemMasterRepo.ReplaceOneAsync(m => m.Id == item1.Id, item1);
            }
            if (config.ItemId2 != null)
            {
                item2.Quantity -= config.AmountItem2 != null ? (long)config.AmountItem2 : 0;
                await itemMasterRepo.ReplaceOneAsync(m => m.Id == item2.Id, item2);
            }
            if (config.ItemId3 != null)
            {
                item3.Quantity -= config.AmountItem3 != null ? (long)config.AmountItem3 : 0;
                await itemMasterRepo.ReplaceOneAsync(m => m.Id == item3.Id, item3);
            }
            return result;
        }
        public async Task<ApiResultDTO<bool>> DragonFarmLevelUp(string id)
        {
            var result = new ApiResultDTO<bool>();
            result.Data = true;
            using (var session = this.client.StartSession())
            {
                session.StartTransaction();
                try
                {
                    var master = await userService.GetMasterAsync(identityService.CurrentUserId);
                    if (master == null)
                    {
                        throw new BusinessException(ErrorCodeEnum.EntityNotExisted, "invalid master");
                    }
                    var rongos = await rongosRepo.Find
                    (
                        r => r.Id == id && master.Wallets.Contains(r.WalletId) && r.SaleStatus == SaleStatus.NotSale
                    ).FirstOrDefaultAsync();
                    if (rongos == null)
                    {
                        throw new BusinessException(ErrorCodeEnum.EntityNotExisted, "invalid rongos");
                    }

                    var DragonFarmLevelConfig = GameConfigs.DragonFarmLevel.Where(x => x.Level == (rongos.DragonFarmLevel + 1)).ToList();
                    if (DragonFarmLevelConfig == null)
                    {
                        result.AddError(ErrorCodeEnum.InvalidRequest, $"level is invalid");
                        result.Data = false;
                        return result;
                    }
                    var rongosFarmLevelStat = GameConfigs.DragonFarmLevelStat.FirstOrDefault(u => u.Level == (rongos.DragonFarmLevel + 1));
                    if (rongosFarmLevelStat == null)
                    {
                        result.AddError(ErrorCodeEnum.InvalidRequest, $"rongosFarmLevelStat is invalid");
                        result.Data = false;
                        return result;
                    }
                    if (rongos.Level < rongosFarmLevelStat.LevelDragonReq)
                    {
                        result.AddError(ErrorCodeEnum.InvalidRequest, $"level rongos not enough");
                        result.Data = false;
                        return result;
                    }
                    //validate

                    foreach (var item in DragonFarmLevelConfig)
                    {
                        var itemData = await itemMasterRepo.Find(im => im.MainUserId == master.MainUserId && im.ItemId == item.ItemId).FirstOrDefaultAsync();
                        if (itemData is not null)
                        {
                            var validItem = await _itemMasterRepository.TakeItem(session, itemData, item.ItemQuantity, "upgrade farm level", logger);
                            if (validItem == false)
                            {
                                await session.AbortTransactionAsync();
                                result.AddError(ErrorCodeEnum.InvalidRequest, $"{item.ItemId}  not configured");
                                result.Data = false;
                                return result;
                            }
                        }

                    }
                    if (RandomNumberGenerator.GetInt32(100) < rongosFarmLevelStat.SuccessRate)
                    {
                        result.Data = true;
                        rongos.DragonFarmLevel += 1;
                        rongos.UpdatedAt = DateTime.UtcNow;
                        var resultDragon = await rongosRepo.ReplaceOneAsync(m => m.Id == id, rongos);
                        masterService.SnapshotFarmPoolJob(master.MainUserId);
                    }
                    else
                    {
                        result.Data = false;
                    }
                    await session.CommitTransactionAsync();
                }
                catch (System.Exception ex)
                {
                    logger.LogError(ex.StackTrace);
                    await session.AbortTransactionAsync();
                    result.Data = false;
                }
            }
            return result;
        }

        public async Task<ApiResultDTO<DragonDTO>> DragonMergeFragment(DragonMergeRequestDto param)
        {
            var result = new ApiResultDTO<DragonDTO>();
            result.Data = new DragonDTO();
            var master = await userService.GetMasterAsync(identityService.CurrentUserId);
            var MergeFragmentConfig = GameConfigs.DragonMergeFragmentConfigs.Where(x => x.ItemId == param.FragmentId).FirstOrDefault();
            var totalFragment = await itemMasterRepo.Find(i => i.ItemId == param.FragmentId && i.MainUserId == master.MainUserId).FirstOrDefaultAsync();
            if (totalFragment == null)
            {
                result.AddError(ErrorCodeEnum.InvalidRequest, $"Merge config is invalid");
                return result;
            }
            if (MergeFragmentConfig == null)
            {
                result.AddError(ErrorCodeEnum.InvalidRequest, $"config is invalid");

                return result;
            }
            if (MergeFragmentConfig.ItemQuantity > totalFragment.Quantity)
            {
                result.AddError(ErrorCodeEnum.InvalidRequest, $"Fragment is not enough");

                return result;
            }
            if (MergeFragmentConfig == null)
            {
                result.AddError(ErrorCodeEnum.FragmentNotEnought, $"Merge config is invalid");

                return result;
            }
            long Rss = MergeFragmentConfig.Rss;
            var userItem = await itemMasterRepo.Find(im => im.MainUserId == master.MainUserId && im.ItemId == 5).FirstOrDefaultAsync();

            if (Rss > userItem.Quantity)
            {
                result.AddError(ErrorCodeEnum.RssNotEnought, $"Gold is not enough");

                return result;
            }
            var classType = mapToInternalClass(MergeFragmentConfig.DragonElement.Code);

            DragonDAO rongos = new DragonDAO
            {
                WalletId = param.WalletId,
                NftId = (ulong)StaticParams.DateTimeNow.ToBinary(),
                Birthday = StaticParams.DateTimeNow,
                Class = classType,
                CreatedAt = StaticParams.DateTimeNow,
                UpdatedAt = StaticParams.DateTimeNow,
                IsOffChain = true,
                IsF2P = true,
                Level = 1,
                Exp = 0,
                Bodyparts = new List<Bodypart>
                        {
                            new Bodypart
                            {
                                Type = BodypartType.Claw,
                                Class = (long)classType,
                                Unlocked = true,
                                SkillUnlocked = true,
                                UnlockState = BodypartUnlockState.Unlocked,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                            new Bodypart
                            {
                                Type = BodypartType.Wing,
                                Class = (long)classType,
                                Unlocked = true,
                                SkillUnlocked = true,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                              new Bodypart
                            {
                                Type = BodypartType.Eye,
                                Class = (long)classType,
                                Unlocked = true,
                                SkillUnlocked = true,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                            new Bodypart
                            {
                                Type = BodypartType.Horn,
                                Class = (long)classType,
                                Unlocked = false,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                            new Bodypart
                            {
                                Type = BodypartType.Skin,
                                Class = (long)classType,
                                Unlocked = false,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                            new Bodypart
                            {
                                Type = BodypartType.Tail,
                                Class = (long)classType,
                                Unlocked = false,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                            new Bodypart
                            {
                                Type = BodypartType.Ultimate,
                                Class = (long)classType,
                                Unlocked = true,
                                SkillUnlocked = true,
                                UnlockState = BodypartUnlockState.Unlocked,
                                MutantType = MutantType.Normal,
                            },
                            // TODO handle eye part
                        }
            };

            var Claw = rongos.Bodyparts.Where(x => x.Type == BodypartType.Claw).FirstOrDefault();
            var Horn = rongos.Bodyparts.Where(x => x.Type == BodypartType.Horn).FirstOrDefault();
            var Skin = rongos.Bodyparts.Where(x => x.Type == BodypartType.Skin).FirstOrDefault();
            var Tail = rongos.Bodyparts.Where(x => x.Type == BodypartType.Tail).FirstOrDefault();

            foreach (var DragonElement in Dragon.Blueprints.DragonElementEnum.DragonElementEnumList)
            {
                Gene gene = new Gene();
                gene.Class = (long)(Enum.TryParse(DragonElement.Code, out ClassType c) ? c : ClassType.Undefined);

                if (gene.Class == (long)rongos.Class)
                    gene.Value = 1;
                else
                    gene.Value = 0;

                Claw.Genes.Add(gene);
                Horn.Genes.Add(gene);
                Skin.Genes.Add(gene);
                Tail.Genes.Add(gene);
            }

            await rongosRepo.InsertOneAsync(rongos);
            //await RequestGenerateAssetsAsync(rongos);
            result.Data = mapper.Map<DragonDAO, DragonDTO>(rongos);
            totalFragment.Quantity -= (long)MergeFragmentConfig.ItemQuantity;
            await itemMasterRepo.ReplaceOneAsync(d => d.Id == totalFragment.Id, totalFragment);
            var itemData = GameConfigs.ItemConfigs.FirstOrDefault(i => i.Id == 5);


            userItem.Quantity -= Rss;
            await itemMasterRepo.ReplaceOneAsync(i => i.Id == userItem.Id, userItem);

            // await userService.UpdateRssMaster(master);
            return result;
        }
        public async Task<ApiResultDTO<DragonDTO>> DragonOpenPackage(DragonOpenRequestDto param)
        {
            var result = new ApiResultDTO<DragonDTO>();
            result.Data = new DragonDTO();
            var master = await userService.GetMasterAsync(identityService.CurrentUserId);
            var rongosOpenConfig = GameConfigs.DragonOpenPackageConfig.Where(x => x.Id == param.PackageId).FirstOrDefault();
            if (rongosOpenConfig == null)
            {
                throw new BusinessException(ErrorCodeEnum.EntityNotExisted, $"Dragon open config is invalid");

            }
            using (var session = this.client.StartSession())
            {
                session.StartTransaction();
                try
                {
                    var classType = mapToInternalClass(rongosOpenConfig.DragonElement.Code);

                    var itemData = await itemMasterRepo.Find(im => im.MainUserId == master.MainUserId && im.ItemId == rongosOpenConfig.ItemId).FirstOrDefaultAsync();
                    if (itemData is not null)
                    {
                        var validItem = await _itemMasterRepository.TakeItem(session, itemData, rongosOpenConfig.ItemQuantity, $"Open package Dragon{classType.ToString()}", logger);
                        if (validItem == false)
                        {
                            await session.AbortTransactionAsync();
                            result.AddError(ErrorCodeEnum.InvalidRequest, $"{rongosOpenConfig.ItemId}  not enough");
                            return result;
                        }
                    }

                    DragonDAO rongos = new DragonDAO
                    {
                        WalletId = param.WalletId,
                        NftId = (ulong)StaticParams.DateTimeNow.ToBinary(),
                        Birthday = StaticParams.DateTimeNow,
                        Class = classType,
                        CreatedAt = StaticParams.DateTimeNow,
                        UpdatedAt = StaticParams.DateTimeNow,
                        IsOffChain = true,
                        IsF2P = true,
                        Level = 1,
                        Exp = 0,
                        Bodyparts = new List<Bodypart>
                        {
                            new Bodypart
                            {
                                Type = BodypartType.Claw,
                                Class = (long)classType,
                                Unlocked = true,
                                SkillUnlocked = true,
                                UnlockState = BodypartUnlockState.Unlocked,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                            new Bodypart
                            {
                                Type = BodypartType.Wing,
                                Class = (long)classType,
                                Unlocked = true,
                                SkillUnlocked = true,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                              new Bodypart
                            {
                                Type = BodypartType.Eye,
                                Class = (long)classType,
                                Unlocked = true,
                                SkillUnlocked = true,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                            new Bodypart
                            {
                                Type = BodypartType.Horn,
                                Class = (long)classType,
                                Unlocked = false,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                            new Bodypart
                            {
                                Type = BodypartType.Skin,
                                Class = (long)classType,
                                Unlocked = false,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                            new Bodypart
                            {
                                Type = BodypartType.Tail,
                                Class = (long)classType,
                                Unlocked = false,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                            new Bodypart
                            {
                                Type = BodypartType.Ultimate,
                                Class = (long)classType,
                                Unlocked = true,
                                SkillUnlocked = true,
                                UnlockState = BodypartUnlockState.Unlocked,
                                MutantType = MutantType.Normal,
                            },
                            // TODO handle eye part
                        }
                    };

                    var Claw = rongos.Bodyparts.Where(x => x.Type == BodypartType.Claw).FirstOrDefault();
                    var Horn = rongos.Bodyparts.Where(x => x.Type == BodypartType.Horn).FirstOrDefault();
                    var Skin = rongos.Bodyparts.Where(x => x.Type == BodypartType.Skin).FirstOrDefault();
                    var Tail = rongos.Bodyparts.Where(x => x.Type == BodypartType.Tail).FirstOrDefault();

                    foreach (var DragonElement in Dragon.Blueprints.DragonElementEnum.DragonElementEnumList)
                    {
                        Gene gene = new Gene();
                        gene.Class = (long)(Enum.TryParse(DragonElement.Code, out ClassType c) ? c : ClassType.Undefined);

                        if (gene.Class == (long)rongos.Class)
                            gene.Value = 1;
                        else
                            gene.Value = 0;

                        Claw.Genes.Add(gene);
                        Horn.Genes.Add(gene);
                        Skin.Genes.Add(gene);
                        Tail.Genes.Add(gene);
                    }

                    await rongosRepo.InsertOneAsync(rongos);
                    //await RequestGenerateAssetsAsync(rongos);
                    result.Data = mapper.Map<DragonDAO, DragonDTO>(rongos);
                    return result;
                }
                catch (System.Exception ex)
                {
                    logger.LogError(ex.StackTrace);
                    await session.AbortTransactionAsync();
                    return result;
                }

            }
        }
        private ClassType mapToInternalClass(string element)
        {
            ClassType _class = ClassType.Undefined;
            Enum.TryParse(element, out _class);
            return _class;
        }
        public async Task ProcessMasterInGameCurrencyResponse(MasterInGameCurrencyConsumeRequestResponseETO response)
        {
            try
            {
                var sagaItem = await cacheRepository.GetFromCache<SagaItem>(response.RequestId);
                var success = false;
                object userActionSuccessPayload = null;
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }
        }

        private long getMaxLevel(bool isOffchain)
        {
            if (isOffchain) return MaxLevelDragonOffchain;
            return MaxLevelDragonOnchain;
        }

        public async Task<bool> AddExp(DragonDAO dragon, double exp)
        {
            var maxLevel = getMaxLevel(dragon.IsOffChain);
            if (dragon.Level < maxLevel)
            {
                var oldLevel = dragon.Level;
                var levelConfig = GameConfigs.LevelExpConfig.EntityLevelConfigMap[Dragon.Blueprints.LevelTypeEnum.Dragon.Code];
                var (level, finalExp) = LevelCalculator.AddExp(dragon.Exp, exp, maxLevel, levelConfig);
                dragon.Level = (ushort)level;
                dragon.Exp = finalExp;
                var result = await rongosRepo.ReplaceOneAsync(m => m.Id == dragon.Id, dragon);
                if (result.ModifiedCount > 0)
                {
                    if (oldLevel != level)
                    {
                        var maxExp = levelConfig.GetExpConfigAtLevel(level).MaxExp;

                        // TODO: Must  re-handle noticeWalletId in case of workplace
                        var noticeWalletId = dragon.WalletId;
                        //_ = serverHub.Clients.Group(noticeWalletId).DragonLevelUp(dragon.Id, dragon.Level, finalExp, (ulong)maxExp);
                        if (!dragon.IsOffChain)
                        {
                            _ = eventBus.PublishAsync(EventNameConst.DragonUpdateLevelEvent, new DragonLevelUpdatedETO
                            {
                                id = dragon.Id,
                                tokenId = Convert.ToUInt64(dragon.NftId),
                                level = dragon.Level,
                            });
                        }
                    }
                    return true;
                }
                return false;
            }
            return false;
        }

        public async Task TakeHappyPoint(DragonDAO rongos, int amount, string reason)
        {
            var builder = Builders<DragonDAO>.Update;
            var updateDefs = new List<UpdateDefinition<DragonDAO>>();
            var burnedHappyPoint = new
            {
                WalletId = rongos.WalletId,
                NftId = rongos.NftId,
                Id = rongos.Id,
                Amount = amount,
                Reason = reason,
            };
            logger.LogAnalysis(burnedHappyPoint, LoggingContextEnum.DragonServiceContext, "rongos", "happy_point_took");
        }
        public async Task AddHappyPoint(DragonDAO rongos, int amount, string reason)
        {
            var builder = Builders<DragonDAO>.Update;
            var updateDefs = new List<UpdateDefinition<DragonDAO>>();
            var burnedEnergy = new
            {
                WalletId = rongos.WalletId,
                NftId = rongos.NftId,
                Id = rongos.Id,
                Amount = amount,
                Reason = reason,
            };
            logger.LogAnalysis(burnedEnergy, LoggingContextEnum.DragonServiceContext, "rongos", "happy_point_added");
        }

        private long RandomBodypartClass(List<Gene> genes)
        {
            var validGenes = genes.FindAll(g => g.Value > 0);
            Random random = new Random();
            var idx = random.Next(0, validGenes.Count);
            return validGenes[idx].Class;
        }
        private MutantType RandonMutantType()
        {
            Random random = new Random();
            return (MutantType)random.Next(0, 3);
        }
        public async Task RequestGenerateAssetsAsync(DragonDAO rongos)
        {

            await eventBus.PublishAsync(EventNameConst.GenerateNftAssetsRequestEvent, new AssetsGenerationIntegrationEvent
            {
                Id = rongos.Id,
                NftId = rongos.NftId.ToString(),
                Entity = "dragon",
                DetailedData = rongos,
            });
        }

        public async Task PublishDragonBirthMailNotify(DragonDAO rongos)
        {
            // handle mailbox notify
            var mainUser = await userRepo.Find(m => m.Wallets.Any(w => w == rongos.WalletId)).FirstOrDefaultAsync();
            if (mainUser != null)
            {
                await eventBus.PublishAsync(EventNameConst.CreatingMailRequestEvent, new CreatingMailboxRequestIntegrationEvent
                {
                    MainUserId = mainUser.GetMainUserId(),
                    Title = "Dragon Borned",
                    Content = "Congratulations on baby rongos was borned!!!",
                    Type = MailboxType.Mail_Notify_Dragon_Birth,
                    NotificationData = JsonSerializer.Serialize(new
                    {
                        id = rongos.Id,
                        nft_id = rongos.NftId,
                        rongos = mapper.Map<DragonDAO, DragonDTO>(rongos)
                    }, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    }),
                });
            }
        }




        public async Task<DragonDAO> SyncBlockchainOrderCreated(ulong nftId, PlacedOrderInfo placedOrderInfo)
        {

            var rongos = await rongosRepo.Find(d => d.NftId == nftId).FirstOrDefaultAsync();
            rongos.SaleStatus = SaleStatus.Sale;
            rongos.PlacedOrderInfo = placedOrderInfo;
            rongos.UpdatedAt = DateTime.UtcNow;
            await rongosRepo.ReplaceOneAsync(m => m.Id == rongos.Id, rongos);
            return rongos;
        }

        public async Task<DragonDAO> SyncBlockchainOrderCancelled(ulong nftId)
        {

            var rongos = await rongosRepo.Find(d => d.NftId == nftId).FirstOrDefaultAsync();
            rongos.SaleStatus = SaleStatus.NotSale;
            rongos.PlacedOrderInfo = null;
            rongos.UpdatedAt = DateTime.UtcNow;
            await rongosRepo.ReplaceOneAsync(m => m.Id == rongos.Id, rongos);
            return rongos;
        }

        public async Task SyncBlockchainOrderFilled(ulong nftId, string buyer)
        {
            var rongos = await rongosRepo.Find(d => d.NftId == nftId).FirstOrDefaultAsync();
            rongos.SaleStatus = SaleStatus.NotSale;
            rongos.PlacedOrderInfo = null;
            rongos.UpdatedAt = DateTime.UtcNow;
            rongos.Reset(this.GameConfigs);
            await rongosRepo.ReplaceOneAsync(m => m.Id == rongos.Id, rongos);
        }




        // public async Task<bool> validateBattlePrepareRequest(List<string> MainUserWallets, List<string> DragonIds)
        // {
        //     var queryable = rongosRepo.Query();
        //     var preparedDragonForMasters = new List<BattlePreparedMasterDragonETO>();

        //     var validDragonIds = DragonIds.FindAll(id => id != "");
        //     var q = queryable.FilterAccessibleDragonByWallets(MainUserWallets.ToArray())
        //         .Where(r => validDragonIds.Contains(r.Id) && r.CooldownEndAt < DateTime.UtcNow);
        //     var rongoses = await asyncQueryableExecuter.ToListAsync(q);
        //     if (rongoses.Count < validDragonIds.Count)
        //     {
        //         return false;
        //     }

        //     foreach (var rongos in rongoses)
        //     {
        //         if (rongos.Bodyparts == null || (!rongos.Bodyparts.FirstOrDefault()?.Unlocked ?? false))
        //         {
        //             return false;
        //         }
        //     }
        //     return true;
        // }

        // public async Task AddExpDragonCompletedEvent(BattleCompletedETO data)
        // {
        //     if (data.BonusDragonExp.HasValue && data.BonusDragonExp.Value > 0)
        //     {
        //         var rongoIds = data.Dragons.FindAll(r => !r.IsMob && !data.AfkIds.Contains(r.MasterId) && !data.LoserIds.Contains(r.MasterId)).Select(r => r.Id).ToList();
        //         var rongoses = await rongosRepo.Find(r => rongoIds.Contains(r.Id)).ToListAsync();
        //         var tasks = new List<Task>();
        //         foreach (var rongos in rongoses)
        //         {
        //             tasks.Add(AddExp(rongos, data.BonusDragonExp.Value));
        //         }
        //         await Task.WhenAll(tasks);
        //     }
        // }

        public async Task<BattleSetupDragonFormationAwaitValidationResponse> ProcessBattleSetupDragonFormationValidation(BattleSetupDragonFormationAwaitValidationResponse data)
        {
            data.Success = true;
            var validDragonIds = data.DragonIds.FindAll(id => id != "");
            var q = rongosRepo.AsQueryable().FilterAccessibleDragonByWallets(data.MainUserWallets.ToArray())
                .Where(r => validDragonIds.Contains(r.Id) && r.CooldownEndAt < DateTime.UtcNow);
            var rongoses = await asyncQueryableExecuter.ToListAsync(q);
            if (rongoses.Count < validDragonIds.Count())
            {
                data.Success = false;
            }
            else
            {
                // foreach (var rongos in rongoses)
                // {
                //     if (rongos.Bodyparts == null)
                //     {
                //         data.Success = false;
                //         data.Error = "Bodyparts null???";
                //         break;
                //     }
                //     if (!rongos.Bodyparts.FirstOrDefault()?.Unlocked ?? false)
                //     {
                //         data.Success = false;
                //         data.Error = "Claw on unlock";
                //         break;
                //     }
                // }
            }

            return data;
        }

        public async Task<ApiResultDTO<long>> Count(DragonFilter request)
        {
            var response = new ApiResultDTO<long>();
            var queryable = request.ApplyFilterTo(rongosRepo.AsQueryable()).FilterOwnDragonesByWallets(request.WalletId);
            var count = await asyncQueryableExecuter.LongCountAsync(queryable);
            response.Data = count;
            return response;
        }

        // public async Task<ApiResultDTO<List<OwnClassMapDto>>> GetUniqueClassGroups(bool isOffchain = true)
        // {
        //     var response = new ApiResultDTO<List<OwnClassMapDto>>();

        //     var dragonCollection = client.GetCollection<DragonDAO>();
        //     var user = await userRepo.Find(u => u.UserId == identityService.CurrentUserId).FirstOrDefaultAsync();
        //     var classes = await (await dragonCollection.DistinctAsync(r => r.Class, r => r.IsOffChain == isOffchain && user.Wallets.Contains(r.WalletId))).ToListAsync();
        //     var classGroup = new Dictionary<ClassType, ClassType>
        //     {
        //         {ClassType.Gold,  ClassType.Gold},
        //         {ClassType.Gold2, ClassType.Gold},
        //         {ClassType.Gold3, ClassType.Gold},
        //         {ClassType.Wood,  ClassType.Wood},
        //         {ClassType.Wood2, ClassType.Wood},
        //         {ClassType.Wood3, ClassType.Wood},
        //         {ClassType.Water,  ClassType.Water},
        //         {ClassType.Water2, ClassType.Water},
        //         {ClassType.Water3, ClassType.Water},
        //         {ClassType.Fire,  ClassType.Fire},
        //         {ClassType.Fire2, ClassType.Fire},
        //         {ClassType.Fire3, ClassType.Fire},
        //         {ClassType.Earth,  ClassType.Earth},
        //         {ClassType.Earth2, ClassType.Earth},
        //         {ClassType.Earth3, ClassType.Earth},
        //         {ClassType.Dark,  ClassType.Dark},
        //         {ClassType.Dark2, ClassType.Dark},
        //         {ClassType.Dark3, ClassType.Dark},
        //         {ClassType.Light,  ClassType.Light},
        //         {ClassType.Light2, ClassType.Light},
        //         {ClassType.Light3, ClassType.Light},
        //     };

        //     response.Data = new List<OwnClassMapDto>();
        //     foreach (var c in classes)
        //     {
        //         var cg = classGroup[c];
        //         if (!response.Data.Any(_cg => _cg.OwnClass == cg))
        //         {
        //             BlockchainClass bcClass = BlockchainClass.Gold;
        //             if (Enum.TryParse(cg.ToString(), out bcClass))
        //             {
        //                 response.Data.Add(new OwnClassMapDto
        //                 {
        //                     OwnClass = cg,
        //                     BcOwnClass = bcClass
        //                 });
        //             }
        //         }
        //     }

        //     return response;
        // }

        public async Task<ApiResultDTO<bool>> ChangeName(string id, string name)
        {
            var response = new ApiResultDTO<bool>();

            var user = await userRepo.Find(u => u.UserId == identityService.CurrentUserId).FirstOrDefaultAsync();
            var rongos = await rongosRepo.Find(r => r.Id == id && user.Wallets.Contains(r.WalletId)).FirstOrDefaultAsync();
            if (rongos == null)
            {
                throw new System.Exception("Dragon is not existed");
            }
            string pattern = "^[a-zA-Z0-9]{6,16}$";
            // start with a letter, allow letter or number 

            Regex regex = new Regex(pattern);
            if (!regex.IsMatch(name))
            {
                throw new System.Exception("The name is not valid policy");
            }
            rongos.Name = name;
            await rongosRepo.ReplaceOneAsync(m => m.Id == id, rongos);
            response.Data = true;
            return response;
        }

        private async Task publishEventToStoreMetaTx(SigningType type, string userId, string signature, BCMetaTx metaTxData)
        {
            await eventBus.PublishAsync(IntegrationTopicConst.MarketplaceService, new IntegrationMessageETO
            {
                EventName = EventNameConst.MkpStoreMetaTxSigningRequestEvent,
                Payload = new StoreMetaTxSigningRequestETO
                {
                    Type = type,
                    MainUserId = userId,
                    SignedData = new SignedDataETO
                    {
                        Signature = signature,
                        DataRequest = new MetaTxDataRequestETO
                        {
                            Operator = metaTxData.Operator,
                            From = metaTxData.From,
                            Nonce = metaTxData.Nonce,
                            ExpiredAt = metaTxData.ExpiredAt,
                            Data = metaTxData.Data,
                        }
                    },
                    WalletId = metaTxData.From
                }
            });
        }

        public async Task ProcessSigningMetaTxResponse(SigningResponseETO data)
        {
            var sagaItem = await cacheRepository.GetFromCache<SagaItem>(data.RequestId);
            if (sagaItem != null)
            {
                if (sagaItem.ActionType == ActionTypeEnum.SigningRequest_DragonWorkplaceCreating)
                {
                    object signingData = null;
                    if (data.Success)
                    {
                        signingData = mapper.Map<SigningResponseETO, SigningResponseDTO>(data);
                        DateTime sagaDataExpiredAt = DateTime.UtcNow.AddMinutes(30);
                        await cacheRepository.SetToCache($"{CachePrefixConstants.CacheKeyPrefixMetaTx}{data.MetaTx.Nonce}", data.RequestId, sagaDataExpiredAt.AddMinutes(10));
                        await publishEventToStoreMetaTx(SigningType.WorkplaceCreateDragon, sagaItem.User.GetMainUserId(), data.Signature, data.MetaTx);
                    }
                    await userActionService.UpdateUserActionState(data.RequestId, data.Success ? ActionState.Succeed : ActionState.Failed, signingData);
                }
                else if (sagaItem.ActionType == ActionTypeEnum.SigningRequest_DragonWorkplaceLeasing)
                {
                    var sagaPayload = JsonSerializer.Deserialize<DragonWorkplaceLeaseSagaData>((JsonElement)sagaItem.Payload);
                    if (data.Success)
                    {
                        sagaPayload.SigningSuccess = true;
                        sagaPayload.SigingMetaTx.Wallet = data.MetaTx.From;
                        sagaPayload.SigingMetaTx.Data = data;
                        sagaItem.Payload = sagaPayload;
                        if (sagaPayload.SagaSuccess)
                        {
                            await userActionService.UpdateUserActionState(data.RequestId, ActionState.Succeed, mapper.Map<SigningResponseETO, SigningResponseDTO>(data));
                            await publishEventToStoreMetaTx(SigningType.WorkplaceLeaseDragon, sagaItem.User.GetMainUserId(), data.Signature, data.MetaTx);
                        }
                        DateTime sagaDataExpiredAt = DateTimeOffset.FromUnixTimeSeconds(data.MetaTx.ExpiredAt).AddMinutes(10).UtcDateTime;
                        await cacheRepository.SetToCache(data.RequestId, sagaItem, sagaDataExpiredAt);
                        await cacheRepository.SetToCache($"{CachePrefixConstants.CacheKeyPrefixMetaTx}{data.MetaTx.Nonce}", data.RequestId, sagaDataExpiredAt.AddMinutes(10));
                        BackgroundJob.Schedule(() => RecheckSigningTxTimeoutJob(data.RequestId), sagaDataExpiredAt.AddMinutes(-5));
                    }
                    else
                    {
                        await userActionService.UpdateUserActionState(data.RequestId, ActionState.Failed);
                        await revertWorkplaceLeasing(sagaItem.User, sagaPayload);
                    }
                }
            }
        }

        public async Task RecheckSigningTxTimeoutJob(string reqId)
        {
            var sagaItem = await cacheRepository.GetFromCache<SagaItem>(reqId);
            if (sagaItem != null)
            {
                if (sagaItem.ActionType == ActionTypeEnum.SigningRequest_DragonWorkplaceLeasing)
                {
                    var sagaPayload = JsonSerializer.Deserialize<DragonWorkplaceLeaseSagaData>((JsonElement)sagaItem.Payload);
                    await revertWorkplaceLeasing(sagaItem.User, sagaPayload);
                }
            }
        }

        private async Task revertWorkplaceLeasing(UserDAO user, DragonWorkplaceLeaseSagaData data)
        {
            if (data.ConsumedCurrencySuccess && !string.IsNullOrEmpty(data.ConsumedCurrencyTxId))
            {
                logger.LogInformation($"revert workplace action on ConsumedCurrencyTxId {data.ConsumedCurrencyTxId}");
                await eventBus.PublishAsync(EventNameConst.MasterInGameCurrencyRevertRequestEvent, new MasterInGameCurrencyRevertRequestETO
                {
                    MainUserId = user.GetMainUserId(),
                    TxId = data.ConsumedCurrencyTxId
                });
            }
        }
        public async Task ProcessDragonCreatedEvent(BirthCreateETO data)
        {
            // data.TokenId = SnowflakeId.Default().NextId();
            // for (var i = 1; i <= 100; i++)
            // {
            //     var rongos = await HandleDragonCreated("0xCB767E70f41A6aCf13f7126Fa8eF50e57624Aeca", i, data.TypeId);
            //     if (rongos != null)
            //     {
            //         await mediator.Send(new RequestGenerateAssetCommand
            //         {
            //             Id = rongos.Id,
            //             NftId = rongos.NftId.ToString(),
            //             Entity = "dragon",
            //             EntityData = rongos
            //         });
            //         await rongosRepo.InsertAsync(rongos);
            //         await RequestGenerateAssetsAsync(rongos);
            //     }
            // }
            var rongos = await HandleDragonCreated(data.Owner, data.TokenId, data.TypeId);
            if (rongos != null)
            {
                await mediator.Send(new RequestGenerateAssetCommand
                {
                    Id = rongos.Id,
                    NftId = rongos.NftId.ToString(),
                    Entity = "dragon",
                    EntityData = rongos
                });
                await rongosRepo.InsertOneAsync(rongos);
                await RequestGenerateAssetsAsync(rongos);
            }

        }
        public async Task<DragonDAO> HandleDragonCreated(string WalletId, long? NftId, long TypeId)
        {
            DragonDAO rongosDao = null;
            try
            {

                // ClassType classType = ConvertDragonType.convertDragonTypeIdToClass(TypeId);
                // if (TypeId != Dragon.Blueprints.ItemEnum.MatingEgg.TypeId)
                // {
                var random = new Random();
                var index = random.Next((int)ClassType.Fire, (int)ClassType.Dark);
                ClassType classType = ConvertDragonType.MapDragonClass(index);
                // }
                rongosDao = new DragonDAO
                {
                    WalletId = WalletId,
                    NftId = NftId != null ? (ulong)NftId : (ulong)Helpers.IdGen.GenId(),
                    Birthday = StaticParams.DateTimeNow,
                    Class = classType,
                    CreatedAt = StaticParams.DateTimeNow,
                    UpdatedAt = StaticParams.DateTimeNow,
                    IsOffChain = NftId != null ? false : true,
                    IsF2P = false,
                    Level = 1,
                    Exp = 0,
                    Bodyparts = new List<Bodypart>
                    {
                        new Bodypart
                        {
                            Type = BodypartType.Claw,
                            Class = (long)classType,
                            Unlocked = true,
                            UnlockState = BodypartUnlockState.Unlocked,
                            MutantType = MutantType.Normal,
                            Genes = new List<Gene>()
                        },
                        new Bodypart
                            {
                                Type = BodypartType.Wing,
                                Class = (long)classType,
                                Unlocked = true,
                                SkillUnlocked = true,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                        new Bodypart
                            {
                                Type = BodypartType.Eye,
                                Class = (long)classType,
                                Unlocked = true,
                                SkillUnlocked = true,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                        new Bodypart
                        {
                            Type = BodypartType.Horn,
                            Class = (long)classType,
                             Unlocked = true,
                            UnlockState = BodypartUnlockState.Unlocked,
                            MutantType = MutantType.Normal,
                            Genes = new List<Gene>()
                        },
                        new Bodypart
                        {
                            Type = BodypartType.Skin,
                            Class = (long)classType,
                            Unlocked = true,
                            UnlockState = BodypartUnlockState.Unlocked,
                            MutantType = MutantType.Normal,
                            Genes = new List<Gene>()
                        },
                        new Bodypart
                        {
                            Type = BodypartType.Tail,
                            Class = (long)classType,
                             Unlocked = true,
                            UnlockState = BodypartUnlockState.Unlocked,
                            MutantType = MutantType.Normal,
                            Genes = new List<Gene>()
                        },
                        new Bodypart
                        {
                            Type = BodypartType.Ultimate,
                            Class = (long)classType,
                            Unlocked = true,
                            UnlockState = BodypartUnlockState.Unlocked,
                            MutantType = MutantType.Normal,
                        },
                    }
                };
                var Claw = rongosDao.Bodyparts.Where(x => x.Type == BodypartType.Claw).FirstOrDefault();
                var Horn = rongosDao.Bodyparts.Where(x => x.Type == BodypartType.Horn).FirstOrDefault();
                var Skin = rongosDao.Bodyparts.Where(x => x.Type == BodypartType.Skin).FirstOrDefault();
                var Tail = rongosDao.Bodyparts.Where(x => x.Type == BodypartType.Tail).FirstOrDefault();
                // if (TypeId != Dragon.Blueprints.ItemEnum.MatingEgg.TypeId)
                // {
                //     //rồng Genesis => các gen thuần chủng
                //     foreach (var DragonElement in Dragon.Blueprints.DragonElementEnum.DragonElementEnumList)
                //     {
                //         Gene gene = new Gene();
                //         gene.Class = (long)(Enum.TryParse(DragonElement.Code, out ClassType _class) ? _class : ClassType.Undefined);

                //         if (gene.Class == (long)rongosDao.Class)
                //             gene.Value = 1;
                //         else
                //             gene.Value = 0;

                //         Claw.Genes.Add(gene);
                //         Horn.Genes.Add(gene);
                //         Skin.Genes.Add(gene);
                //         Tail.Genes.Add(gene);
                //     }
                // }

            }
            catch (System.Exception ex)
            {
                logger.LogException(ex);
            }
            return rongosDao;
        }
        public async Task<DragonDAO> AdminCreatedDragon(BirthCreateETO data)
        {
            DragonDAO rongosDao = null;
            try
            {


                ClassType classType = ConvertDragonType.MapDragonClass(data.TypeId);

                rongosDao = new DragonDAO
                {
                    WalletId = data.Owner,
                    NftId = (ulong)Helpers.IdGen.GenId(),
                    Birthday = StaticParams.DateTimeNow,
                    Class = classType,
                    CreatedAt = StaticParams.DateTimeNow,
                    UpdatedAt = StaticParams.DateTimeNow,
                    IsOffChain = false,
                    IsF2P = false,
                    Level = 1,
                    Exp = 0,
                    Bodyparts = new List<Bodypart>
                    {
                        new Bodypart
                        {
                            Type = BodypartType.Claw,
                            Class = (long)classType,
                            Unlocked = true,
                            UnlockState = BodypartUnlockState.Unlocked,
                            MutantType = MutantType.Normal,
                            Genes = new List<Gene>()
                        },
                        new Bodypart
                            {
                                Type = BodypartType.Wing,
                                Class = (long)classType,
                                Unlocked = true,
                                SkillUnlocked = true,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                        new Bodypart
                            {
                                Type = BodypartType.Eye,
                                Class = (long)classType,
                                Unlocked = true,
                                SkillUnlocked = true,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                        new Bodypart
                        {
                            Type = BodypartType.Horn,
                            Class = (long)classType,
                             Unlocked = true,
                            UnlockState = BodypartUnlockState.Unlocked,
                            MutantType = MutantType.Normal,
                            Genes = new List<Gene>()
                        },
                        new Bodypart
                        {
                            Type = BodypartType.Skin,
                            Class = (long)classType,
                            Unlocked = true,
                            UnlockState = BodypartUnlockState.Unlocked,
                            MutantType = MutantType.Normal,
                            Genes = new List<Gene>()
                        },
                        new Bodypart
                        {
                            Type = BodypartType.Tail,
                            Class = (long)classType,
                             Unlocked = true,
                            UnlockState = BodypartUnlockState.Unlocked,
                            MutantType = MutantType.Normal,
                            Genes = new List<Gene>()
                        },
                        new Bodypart
                        {
                            Type = BodypartType.Ultimate,
                            Class = (long)classType,
                            Unlocked = true,
                            UnlockState = BodypartUnlockState.Unlocked,
                            MutantType = MutantType.Normal,
                        },
                    }
                };
                var Claw = rongosDao.Bodyparts.Where(x => x.Type == BodypartType.Claw).FirstOrDefault();
                var Horn = rongosDao.Bodyparts.Where(x => x.Type == BodypartType.Horn).FirstOrDefault();
                var Skin = rongosDao.Bodyparts.Where(x => x.Type == BodypartType.Skin).FirstOrDefault();
                var Tail = rongosDao.Bodyparts.Where(x => x.Type == BodypartType.Tail).FirstOrDefault();
                await rongosRepo.InsertOneAsync(rongosDao);

            }
            catch (System.Exception ex)
            {
                logger.LogException(ex);
            }
            return rongosDao;
        }
        public async Task ProcessDragonTransferEvent(TransferETO data)
        {
            var rongos = await rongosRepo.Find(r => r.NftId == (ulong)data.TokenId && r.WalletId == data.From).FirstOrDefaultAsync();
            if (rongos != null)
            {
                rongos.WalletId = data.To;
                await rongosRepo.ReplaceOneAsync(r => r.Id == rongos.Id, rongos);
            }

        }
        public async Task ProcessCreateDragonF2P(string UserId)
        {

            var classesDragon = new List<ClassType>
                {
                    ClassType.Gold,
                    ClassType.Wood,
                    ClassType.Water,
                    ClassType.Fire,
                    ClassType.Earth,
                    ClassType.Light,
                    ClassType.Dark,

                };
            var classesBodyPart = new List<ClassType>
                {
                    ClassType.Gold,
                    ClassType.Wood,
                    ClassType.Water,
                    ClassType.Fire,
                    ClassType.Earth,
                    ClassType.Gold2,
                    ClassType.Wood2,
                    ClassType.Water2,
                    ClassType.Fire2,
                    ClassType.Earth2,
                    ClassType.Gold3,
                    ClassType.Wood3,
                    ClassType.Water3,
                    ClassType.Fire3,
                    ClassType.Earth3,
                    ClassType.Light,
                    ClassType.Light2,
                    ClassType.Light3,
                    ClassType.Dark,
                    ClassType.Dark2,
                    ClassType.Dark3,
                };
            var expectedDragonCount = 3;
            var rongoses = new List<string>();
            for (int i = 0; i < expectedDragonCount; i++)
            {
                var random = new Random();
                var rndClass = classesDragon.ElementAt(random.Next(0, classesDragon.Count));
                classesDragon.Remove(rndClass);
                DragonDAO rongos = new DragonDAO
                {
                    WalletId = UserId,
                    NftId = (ulong)StaticParams.DateTimeNow.ToBinary(),
                    Birthday = StaticParams.DateTimeNow,
                    Class = rndClass,
                    CreatedAt = StaticParams.DateTimeNow,
                    UpdatedAt = StaticParams.DateTimeNow,
                    IsOffChain = true,
                    IsF2P = true,
                    Level = 1,
                    Exp = 0,
                    Bodyparts = new List<Bodypart>
                        {
                            new Bodypart
                            {
                                Type = BodypartType.Claw,
                                Class = (long)rndClass,
                                Unlocked = true,
                                SkillUnlocked = true,
                                UnlockState = BodypartUnlockState.Unlocked,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                            new Bodypart
                            {
                                Type = BodypartType.Wing,
                                Class = (long)rndClass,
                                Unlocked = true,
                                SkillUnlocked = true,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                              new Bodypart
                            {
                                Type = BodypartType.Eye,
                                Class = (long)rndClass,
                                Unlocked = true,
                                SkillUnlocked = true,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                            new Bodypart
                            {
                                Type = BodypartType.Horn,
                                Class = (long)rndClass,
                                Unlocked = false,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                            new Bodypart
                            {
                                Type = BodypartType.Skin,
                                Class = (long)rndClass,
                                Unlocked = false,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                            new Bodypart
                            {
                                Type = BodypartType.Tail,
                                Class = (long)rndClass,
                                Unlocked = false,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                            new Bodypart
                            {
                                Type = BodypartType.Ultimate,
                                Class = (long)rndClass,
                                Unlocked = true,
                                SkillUnlocked = true,
                                UnlockState = BodypartUnlockState.Unlocked,
                                MutantType = MutantType.Normal,
                            },
                            // TODO handle eye part
                        }
                };

                var Claw = rongos.Bodyparts.Where(x => x.Type == BodypartType.Claw).FirstOrDefault();
                var Horn = rongos.Bodyparts.Where(x => x.Type == BodypartType.Horn).FirstOrDefault();
                var Skin = rongos.Bodyparts.Where(x => x.Type == BodypartType.Skin).FirstOrDefault();
                var Tail = rongos.Bodyparts.Where(x => x.Type == BodypartType.Tail).FirstOrDefault();

                foreach (var DragonElement in Dragon.Blueprints.DragonElementEnum.DragonElementEnumList)
                {
                    Gene gene = new Gene();
                    gene.Class = (long)(Enum.TryParse(DragonElement.Code, out ClassType c) ? c : ClassType.Undefined);

                    if (gene.Class == (long)rongos.Class)
                        gene.Value = 1;
                    else
                        gene.Value = 0;

                    Claw.Genes.Add(gene);
                    Horn.Genes.Add(gene);
                    Skin.Genes.Add(gene);
                    Tail.Genes.Add(gene);
                }
                await rongosRepo.InsertOneAsync(rongos);
                //await RequestGenerateAssetsAsync(rongos);
                rongoses.Add(rongos.Id);

            }
            BackgroundJob.Schedule(() => ProcessCreateFormation(UserId, rongoses), DateTimeOffset.UtcNow.AddSeconds(5));

        }
        public async Task ProcessCreateFormation(string UserId, List<string> rongosIds)
        {
            var master = await userService.GetMasterAsync(UserId);
            var formation = master.GetFormation(BattleType.PVE.ToString());
            if (formation == null)
            {
                var formationPvE = new BattleFormationWithType
                {
                    Type = BattleType.PVE.ToString()
                };
                formationPvE.DragonIds = rongosIds;
                master.Formations.Add((BattleFormationWithType)formationPvE);
                var formationPvP = new BattleFormationWithType
                {
                    Type = BattleType.PVP.ToString()
                };
                formationPvP.DragonIds = rongosIds;
                master.Formations.Add((BattleFormationWithType)formationPvP);
            }
            // formation.DragonIds = rongosIds;
            master.UpdatedAt = DateTime.UtcNow;
            await masterRepo.ReplaceOneAsync(m => m.Id == master.Id, master);
        }
    }
}
