using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AutoMapper;
using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using JohnKnoop.MongoRepository;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using StackExchange.Redis;
using Volo.Abp.Linq;
using StackExchange.Redis.KeyspaceIsolation;
using Microsoft.Extensions.Options;
using DragonAPI.Common;
using DragonAPI.Enums;
using DragonAPI.Models.Entities;
using DragonAPI.Models.DAOs;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Configurations;
using DragonAPI.Repositories;
using DragonAPI.Data;
using DragonAPI.Models.DTOs;
using DragonAPI.Extensions;
using MediatR;
using DragonAPI.Common.Calculator;
using DragonAPI.Application.Commands;
using Hangfire;

namespace DragonAPI.Services
{
    public class BattleCacheItem
    {
        public enum BattleCachingPurpose
        {
            FOR_PVE,
            FOR_PVP
        }
        public string Id { get; set; }
        // public BattleType Type { get; set; }
        public BattlePVEMetadataCacheItem Metadata { get; set; }
        public List<BattleMasterCacheItem> Masters { get; set; }
        // public BatleStateCacheEnum State { get; set; } = BatleStateCacheEnum.Preparing;
        public BattleCachingPurpose CachingPurpose { get; set; } = BattleCachingPurpose.FOR_PVE;
        // public bool MasterVerified { get; set; } = false;
        // public bool DragonVerified { get; set; } = false;
        // public bool Prepared => MasterVerified &&  DragonVerified;
        public string Cookie { get; set; }
    }

    public class BattleMasterCacheItem
    {
        public string Id { get; set; }
        // using for mapping entity data to the mainUser
        public string MainUserId { get; set; }
        public List<string> Wallets { get; set; }
        // Who control the battle. subUserid or mainUserId
        public string UserId { get; set; }
        public string Name { get; set; }
        public string AvatarId { get; set; }
        public long Level { get; set; }
        public double Exp { get; set; }
        public bool AutoAttack { get; set; }
        public bool X3Speed { get; set; }
        public List<string> DragonIds { get; set; }
        public List<DragonDAO> Dragones { get; set; }
        public string EnvironmentId { get; set; }

    }

    public class BattleDragonCacheItem
    {
        public string Id { get; set; }
        public ulong NftId { get; set; }
        public string WalletId { get; set; }
        public string Name { get; set; }
        public long Level { get; set; }
        public double Exp { get; set; }
        public ClassType Class { get; set; }
        public Gender Gender { get; set; }
        public List<Bodypart> Bodyparts { get; set; }
        public int HappyPoint { get; set; }
        public bool IsOffChain { get; set; }
        public DragonDAO ToDragonDAO()
        {
            return new DragonDAO
            {
                Id = this.Id,
                WalletId = this.WalletId,
                Name = this.Name,
                NftId = this.NftId,
                Level = (ushort)this.Level,
                Exp = this.Exp,
                Class = this.Class,
                Bodyparts = this.Bodyparts,
            };
        }
        public static BattleDragonCacheItem FromDragonEvtItem(BattleDragonEvtItem r)
        {
            return new BattleDragonCacheItem
            {
                Id = r.Id,
                NftId = Convert.ToUInt64(r.NftId),
                WalletId = r.WalletId,
                Name = r.Name,
                Level = r.Level,
                Exp = r.Exp,
                Class = r.Class,
                Bodyparts = r.Bodyparts,
                IsOffChain = r.IsOffChain,
            };
        }
    }


    public class BattlePVPMetadataCacheItem
    {
        public MasterDAO EnemyPlayer { get; set; }
        public List<DragonDAO> DragonesEnemy { get; set; }
        public List<string> DragonEnemyIds { get; set; }
    }

    public class BattlePVEMetadataCacheItem
    {
        public string StageFormationId { get; set; }
        public string StageId { get; set; }
        public string MapId { get; set; }
    }
    public class HubMessageBattleCreated
    {
        public string SessionId { get; set; }
        public string MatchId { get; set; }
        public string Cookie { get; set; }
    }
    public class HubMessageRejectMatch
    {
        public string SessionId { get; set; }
        public string UserId { get; set; }
    }

    public class HubMessageBattleEnd
    {
        public string Id { get; set; }
        public long Rss { get; set; }
        public long LRss { get; set; }
        public BattleExpRewardDto ExpReward { get; set; }
        public List<ClaimableItemDto> PVEDropItems { get; set; }
        public BattlePVETimeDto PVETime { get; set; }


    }

    public class BattleService : BaseService<BattleService>
    {
        public enum BattleClaimItemSource
        {
            PVEDropItem,
            WorldEvent,
            Arena,
        }

        public class BattleRequestCreateClaimingItemCacheData
        {
            public BattleClaimItemSource ClaimItemSource { get; set; } = BattleClaimItemSource.PVEDropItem;
        }

        private readonly IMongoCollection<DragonDAO> dragonRepo;
        private readonly IMongoCollection<MasterDAO> masterRepo;
        private readonly IMongoCollection<MasterStageTrackingDAO> masterStageTrackingRepo;
        private readonly IMongoCollection<ItemMasterDAO> itemMasterRepo;
        private readonly IMongoCollection<RankPositionDAO> rankPositionRepo;
        private readonly IMongoCollection<PvPHistoryDAO> pvpHistoryRepo;
        private readonly MasterService masterService;
        private readonly DragonService rongosService;
        private readonly UserService userService;
        private readonly DragonCalculator rongosCalculator;
        private readonly IDatabase redisDatabase;
        private readonly AsyncQueryableExecuter asyncQueryableExecuter;
        private readonly IHttpClientFactory clientFactory;
        private readonly IMediator mediator;
        private readonly DragonMongoDbContext mongoDbContext;
        private readonly HubForwarder.HubForwarderClient hubFwdClient;
        public BattleService(
            IMongoClient client,
            ConfigLoader cfgLoader,
            HubForwarder.HubForwarderClient hubFwdClient,
            DragonService rongosService,
            MasterService masterService,
            UserService userService,
            IConnectionMultiplexer redisConn,
            AsyncQueryableExecuter asyncQueryableExecuter,
            IMediator mediator,
            IMapper mapper,
            ICapPublisher capBus,
            DragonCalculator rongosCalculator,
            IdentityService identityService,
            IHttpClientFactory clientFactory,
            IOptions<RedisSettings> redisSettingsOption,
            ICacheRepository cacheRepository,
            DragonMongoDbContext mongoDbContext,
            ILogger<BattleService> logger) : base(logger, client, cfgLoader, mapper, capBus, cacheRepository, identityService)
        {
            this.mediator = mediator;

            this.dragonRepo = mongoDbContext.GetCollection<DragonDAO>("dragons");
            this.masterRepo = mongoDbContext.GetCollection<MasterDAO>("masters");
            this.masterStageTrackingRepo = mongoDbContext.GetCollection<MasterStageTrackingDAO>("master_stage_tracking");
            this.rankPositionRepo = mongoDbContext.GetCollection<RankPositionDAO>("rankPosition");
            this.itemMasterRepo = mongoDbContext.GetCollection<ItemMasterDAO>("item_master");
            this.pvpHistoryRepo = mongoDbContext.GetCollection<PvPHistoryDAO>("pvp_history");
            // this.achievementStatsRepo = achievementStatsRepo;
            this.rongosService = rongosService;
            this.masterService = masterService;
            this.redisDatabase = redisConn.GetDatabase().WithKeyPrefix(redisSettingsOption.Value.Prefix);
            this.rongosCalculator = rongosCalculator;
            this.userService = userService;
            this.asyncQueryableExecuter = asyncQueryableExecuter;
            this.clientFactory = clientFactory;
            this.hubFwdClient = hubFwdClient;

        }

        public async Task<ApiResultDTO<ArenaRankResponse>> GetArenaRank()
        {
            var result = new ApiResultDTO<ArenaRankResponse>();
            result.Data = new ArenaRankResponse();
            var master = await userService.GetMasterAsync(identityService.CurrentUserId);
            var myRank = await rankPositionRepo.Find(r => r.MainUserId == master.MainUserId).FirstOrDefaultAsync();
            if (myRank == null)
            {
                var currentSeason = gameCfgLoader.GameConfigs.SeasonConfigs.Where(s => s.StartingTime < DateTime.UtcNow && s.EndingTime > DateTime.UtcNow).FirstOrDefault();
                myRank = new RankPositionDAO
                {
                    SeasonId = currentSeason.Id,
                    MainUserId = master.MainUserId,
                    RankPosition = 101,
                    Tier = TierGroup.Bronze
                };
            }
            var rankStats = await rankPositionRepo.Find(r => r.Tier == myRank.Tier).SortBy(r => r.RankPosition).Limit(1000).ToListAsync();
            var listPlayer = new List<RankPositionDTO>();
            var rand = new Random();
            if (myRank.RankPosition < 1000)
            {
                var OpponentPlayerUp = rankStats.Where(r => r.RankPosition <= myRank.RankPosition && r.MainUserId != myRank.MainUserId).ToList();
                if (OpponentPlayerUp.Count > 0 && OpponentPlayerUp.Count < 5)
                {
                    foreach (var player in OpponentPlayerUp)
                    {
                        listPlayer.Add(mapper.Map<RankPositionDAO, RankPositionDTO>(player));
                    }
                }
                else if (OpponentPlayerUp.Count >= 5 && OpponentPlayerUp.Count < 30)
                {

                    for (int i = 0; i <= 4; i++)
                    {
                        int index = rand.Next(OpponentPlayerUp.Count);
                        listPlayer.Add(mapper.Map<RankPositionDAO, RankPositionDTO>(OpponentPlayerUp[index]));
                        OpponentPlayerUp.Remove(OpponentPlayerUp[index]);
                    }

                }
                else if (OpponentPlayerUp.Count >= 30)
                {
                    var OpponentPlayers = rankStats.Where(r => r.RankPosition <= myRank.RankPosition && r.RankPosition >= myRank.RankPosition - 30 && r.MainUserId != myRank.MainUserId).ToList();
                    for (int i = 0; i <= 4; i++)
                    {
                        int index = rand.Next(OpponentPlayerUp.Count);
                        listPlayer.Add(mapper.Map<RankPositionDAO, RankPositionDTO>(OpponentPlayerUp[index]));
                        OpponentPlayerUp.Remove(OpponentPlayerUp[index]);
                    }
                }
                else
                {
                    listPlayer.AddRange(mapper.Map<List<RankPositionDAO>, List<RankPositionDTO>>(rankStats.Where(r => r.RankPosition > myRank.RankPosition && r.MainUserId != myRank.MainUserId).OrderBy(r => r.RankPosition).Take(5).ToList()));
                }

                if (listPlayer.Count < 5)
                {
                    var count = 5 - listPlayer.Count;
                    var OpponentPlayersDown = rankStats.Where(r => r.RankPosition > myRank.RankPosition && r.MainUserId != myRank.MainUserId).OrderBy(r => r.RankPosition).Take(20).ToList();
                    for (int i = 0; i < count; i++)
                    {
                        int index = rand.Next(OpponentPlayersDown.Count);
                        listPlayer.Add(mapper.Map<RankPositionDAO, RankPositionDTO>(OpponentPlayersDown[index]));
                        OpponentPlayersDown.Remove(OpponentPlayersDown[index]);
                    }
                }
            }
            else
            {
                var OpponentPlayers = rankStats.Where(r => r.RankPosition <= 1000 && r.RankPosition >= 960 && r.MainUserId != myRank.MainUserId).OrderByDescending(r => r.RankPosition).ToList();
                for (int i = 0; i <= 5; i++)
                {
                    int index = rand.Next(OpponentPlayers.Count);
                    listPlayer.Add(mapper.Map<RankPositionDAO, RankPositionDTO>(OpponentPlayers[index]));
                    OpponentPlayers.Remove(OpponentPlayers[index]);
                }

            }
            listPlayer.OrderBy(r => r.RankPosition).ToList();
            foreach (var player in listPlayer)
            {
                var playerinfo = await masterRepo.Find(m => m.MainUserId == player.MainUserId).FirstOrDefaultAsync();
                var formation = master.GetFormation(BattleType.PVP.ToString());
                var idsDragon = formation.DragonIds.FindAll(id => id != "");
                var lstDragon = dragonRepo.AsQueryable().FilterAccessibleDragonByWallets(master.Wallets.ToArray()).Where(d => idsDragon.Contains(d.Id)).ToList();
                var rongos = mapper.Map<List<DragonDAO>, List<DragonDTO>>(lstDragon);
                var cp = rongos.Sum(r => r.Stats.Cp);
                var ChallengePlayer = new ArenaRankPlayer
                {
                    Player = player,
                    Dragons = rongos.OrderByDescending(r => r.Stats.Cp).Take(3).ToList(),
                    CP = (int)cp
                };
                result.Data.ChallengePlayers.Add(ChallengePlayer);
            }

            return result;
        }
        public async Task<ApiResultDTO<MyArenaRank>> GetUserArenaRank()
        {
            var result = new ApiResultDTO<MyArenaRank>();
            result.Data = new MyArenaRank();
            var master = await userService.GetMasterAsync(identityService.CurrentUserId);
            var myArena = await rankPositionRepo.Find(r => r.MainUserId == master.MainUserId).FirstOrDefaultAsync();
            if (myArena == null)
            {
                result.AddError(ErrorCodeEnum.InvalidRequest, "Invalid arena");
                return result;
            }
            var playerinfo = await masterRepo.Find(m => m.MainUserId == myArena.MainUserId).FirstOrDefaultAsync();
            var formation = master.GetFormation(BattleType.PVP.ToString());
            var idsDragon = formation.DragonIds.FindAll(id => id != "");
            var lstDragon = dragonRepo.AsQueryable().FilterAccessibleDragonByWallets(master.Wallets.ToArray()).Where(d => idsDragon.Contains(d.Id)).ToList();
            var rongos = mapper.Map<List<DragonDAO>, List<DragonDTO>>(lstDragon);
            var cp = rongos.Sum(r => r.Stats.Cp);
            var ChallengePlayer = new MyArenaRank
            {
                Player = mapper.Map<RankPositionDAO, RankPositionDTO>(myArena),
                Dragons = rongos.OrderByDescending(r => r.Stats.Cp).ToList(),
                CP = (int)cp,
                RemainingDuel = master.RemainingDuel,
                RefreshChallenge = master.RefreshChallenge
            };
            result.Data = ChallengePlayer;
            return result;
        }
        public async Task RejectDragonOutOfBattleFormation(DragonDAO rongos)
        {
            var master = await masterRepo.Find(m => m.Wallets.Any(w => w == rongos.WalletId)).FirstOrDefaultAsync();
            if (master != null)
            {
                master.Formations.ForEach(formation =>
                {
                    formation.DragonIds.Remove(rongos.Id);
                });
                await masterRepo.ReplaceOneAsync(m => m.Id == master.Id, master);
            }
        }

        /*PVE*/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stageId"></param>
        /// <returns></returns>
        public async Task<ApiResultDTO<bool?>> EnterPVEBattle(string stageId)
        {
            var result = new ApiResultDTO<bool?>();
            // Internal logic battleservice
            var stage = GameConfigs.WorldMapConfig.Stages.Find(s => s.Id == stageId);
            if (stage == null)
            {
                result.AddError(ErrorCodeEnum.InvalidRequest, "Invalid stage");
                result.Data = false;
                return result;
            }

            if (stage.LastStage)
            {
                var remainBattleValid = await isLastStageBattleValid(stage);
                if (!remainBattleValid)
                {
                    result.AddError(ErrorCodeEnum.InvalidRequest, "Reached max battle");
                    result.Data = false;
                    return result;
                }
            }

            // Get black dragons in this stage.
            var stageFormation = await GetRandomStageFormation(stageId);
            if (stageFormation == null)
            {
                result.AddError(ErrorCodeEnum.NoValidStageFormation);
                result.Data = false;
                return result;
            }
            var master = await userService.GetMasterAsync(identityService.CurrentUserId);
            var formation = master.GetFormation(BattleType.PVE.ToString());
            if (formation == null || formation.DragonIds.Count == 0)
            {
                result.AddError(ErrorCodeEnum.PVETeamNotSetup);
                return result;
            }
            var battleId = SnowflakeId.Default().NextId().ToString();
            var cachedActiveBattle = await this.GetCachedActiveBattleAsync(master.Id);
            if (cachedActiveBattle != null && battleId != cachedActiveBattle.Id)
            {
                result.AddError(ErrorCodeEnum.BattleNotSetup);
                result.Data = false;
                return result;
            }
            var idsDragon = formation.DragonIds.FindAll(id => id != "");
            var lstDragonIds = dragonRepo.AsQueryable().FilterAccessibleDragonByWallets(master.Wallets.ToArray()).Where(d => idsDragon.Contains(d.Id)).ToList();
            if (lstDragonIds.Count < idsDragon.Count)
            {
                result.AddError(ErrorCodeEnum.DragonNotOwned, "rongos not owned User");
                result.Data = false;
                return result;
            }
            var battleData = new BattleCacheItem();
            string cookie = string.Empty;
            battleData.Id = battleId;
            battleData.CachingPurpose = BattleCacheItem.BattleCachingPurpose.FOR_PVE;
            battleData.Metadata = new BattlePVEMetadataCacheItem
            {
                StageFormationId = stageFormation.FormationId,
                StageId = stageFormation.StageId,
                MapId = stage.ParentId,
            };
            battleData.Masters = new List<BattleMasterCacheItem>{new BattleMasterCacheItem
                {
                    // temp set Id here
                    Id = master.Id,
                    MainUserId = master.MainUserId,
                    Wallets = master.Wallets.ToList(),
                    UserId = identityService.CurrentUserId,
                    DragonIds = formation.DragonIds,
                    Dragones = lstDragonIds,

                }
            };

            cookie = await handlePVEBattlePrepared(battleData);
            battleData.Cookie = cookie;
            var success = await saveOrUpdateMasterBattleInfo(battleData);
            if (success)
            {
                logger.LogDebug("battleID: {0}, Cookie: {1}", battleId, cookie);
                BackgroundJob.Schedule(() => releaseMasters(battleId), DateTimeOffset.UtcNow.AddMinutes(2));
                result.Data = true;
                var hubMsg = new SendMessageRequest
                {
                    Mode = SendMessageRequest.Types.SendMessageMode.Users,

                    Payload = new SendMessageRequest.Types.SendMessagePayload
                    {
                        Type = "BattleCreated",
                        Payload = JsonSerializer.Serialize(new HubMessageBattleCreated
                        {
                            SessionId = battleId,
                            Cookie = cookie
                        })
                    }
                };
                hubMsg.UserIds.Add(master.MainUserId);
                await hubFwdClient.SendMessageAsync(hubMsg);

            }
            else
            {
                result.Data = false;
                result.AddError(ErrorCodeEnum.BattleNotSetup);
            }
            return result;
        }

        public async Task<ApiResultDTO<bool?>> EnterPVPBattle(string EnemyPlayerId)
        {
            var result = new ApiResultDTO<bool?>();
            // Internal logic battleservice
            var master = await userService.GetMasterAsync(identityService.CurrentUserId);
            if (master.RemainingDuel > 10)
            {
                result.AddError(ErrorCodeEnum.UnexpectedErr);
                return result;
            }
            var formationMaster = master.GetFormation(BattleType.PVP.ToString());
            if (formationMaster == null || formationMaster.DragonIds.Count == 0)
            {
                result.AddError(ErrorCodeEnum.PVETeamNotSetup);
                return result;
            }
            var enemyPlayer = await userService.GetMasterAsync(EnemyPlayerId);

            if (enemyPlayer == null)
            {
                result.AddError(ErrorCodeEnum.InvalidRequest, "Invalid enemy player");
                result.Data = false;
                return result;
            }
            var enemyPlayerRank = await rankPositionRepo.Find(r => r.MainUserId == enemyPlayer.MainUserId).FirstOrDefaultAsync();
            if (enemyPlayer.LastAttackedAt.AddSeconds(20) > DateTime.UtcNow)
            {
                result.AddError(ErrorCodeEnum.InvalidRequest, "Enemy player in battle");
                result.Data = false;
                return result;
            }

            // Check Rank . 
            // if (enemyPlayer.AvatarId)
            // {
            //     var remainBattleValid = await isLastStageBattleValid(stage);
            //     if (!remainBattleValid)
            //     {
            //         result.AddError(ErrorCodeEnum.InvalidRequest, "Reached max battle");
            //         result.Data = false;
            //         return result;
            //     }
            // }

            // Get black dragons in this stage.

            var formationEnemy = enemyPlayer.GetFormation(BattleType.PVP.ToString());
            if (formationEnemy == null || formationEnemy.DragonIds.Count == 0)
            {
                result.AddError(ErrorCodeEnum.PVETeamNotSetup);
                return result;
            }
            var battleId = SnowflakeId.Default().NextId().ToString();
            var cachedActiveBattle = await this.GetCachedActiveBattleAsync(master.Id);
            if (cachedActiveBattle != null && battleId != cachedActiveBattle.Id)
            {
                result.AddError(ErrorCodeEnum.BattleNotSetup);
                result.Data = false;
                return result;
            }
            //validate rongos master
            var idsDragon = formationMaster.DragonIds.FindAll(id => id != "");
            var lstDragonIds = dragonRepo.AsQueryable().FilterAccessibleDragonByWallets(master.Wallets.ToArray()).Where(d => idsDragon.Contains(d.Id)).ToList();
            if (lstDragonIds.Count < idsDragon.Count)
            {
                result.AddError(ErrorCodeEnum.DragonNotOwned, "rongos not owned User");
                result.Data = false;
                return result;
            }
            //validate rongos enemy player
            var idsDragonEnemy = formationEnemy.DragonIds.FindAll(id => id != "");
            var lstDragonEnemyIds = dragonRepo.AsQueryable().FilterAccessibleDragonByWallets(enemyPlayer.Wallets.ToArray()).Where(d => idsDragonEnemy.Contains(d.Id)).ToList();
            if (lstDragonEnemyIds.Count < idsDragonEnemy.Count)
            {
                result.AddError(ErrorCodeEnum.DragonNotOwned, "rongos enemy not owned User enemy");
                result.Data = false;
                return result;
            }
            var battleData = new BattleCacheItem();
            string cookie = string.Empty;
            battleData.Id = battleId;
            battleData.CachingPurpose = BattleCacheItem.BattleCachingPurpose.FOR_PVE;

            battleData.Masters = new List<BattleMasterCacheItem>{new BattleMasterCacheItem
                {
                    // temp set Id here
                    Id = master.Id,
                    MainUserId = master.MainUserId,
                    Wallets = master.Wallets.ToList(),
                    UserId = identityService.CurrentUserId,
                    DragonIds = formationMaster.DragonIds,
                    Dragones = lstDragonIds,

                }
            };
            battleData.Masters.Add(new BattleMasterCacheItem
            {
                // temp set Id here
                Id = enemyPlayer.Id,
                MainUserId = enemyPlayer.MainUserId,
                Wallets = enemyPlayer.Wallets.ToList(),
                UserId = enemyPlayer.MainUserId,
                DragonIds = formationEnemy.DragonIds,
                Dragones = lstDragonEnemyIds,

            });


            cookie = await handlePVPBattlePrepared(battleData, master.Id);
            battleData.Cookie = cookie;
            var success = await saveOrUpdateMasterBattleInfo(battleData);
            if (success)
            {
                logger.LogDebug("battleID: {0}, Cookie: {1}", battleId, cookie);
                BackgroundJob.Schedule(() => releaseMasters(battleId), DateTimeOffset.UtcNow.AddMinutes(2));
                result.Data = true;
                var hubMsg = new SendMessageRequest
                {
                    Mode = SendMessageRequest.Types.SendMessageMode.Users,

                    Payload = new SendMessageRequest.Types.SendMessagePayload
                    {
                        Type = "BattleCreated",
                        Payload = JsonSerializer.Serialize(new HubMessageBattleCreated
                        {
                            SessionId = battleId,
                            Cookie = cookie
                        })
                    }
                };
                hubMsg.UserIds.Add(master.MainUserId);
                await hubFwdClient.SendMessageAsync(hubMsg);
                var pvpLog = new PvPHistoryDAO
                {
                    BattleId = battleId,
                    MainUserId = master.MainUserId,
                    OpponentPlayer = new PlayerInBattle
                    {
                        MainUserId = enemyPlayer.MainUserId,
                        Name = enemyPlayer.Name,
                        Rank = enemyPlayerRank.RankPosition,
                        CombatPower = rongosCalculator.CalculateTeamPower(mapper.Map<List<DragonDAO>, List<DragonAPI.Common.Dragon>>(lstDragonEnemyIds))
                    }

                };
                await pvpHistoryRepo.InsertOneAsync(pvpLog);
            }
            else
            {
                result.Data = false;
                result.AddError(ErrorCodeEnum.BattleNotSetup);
            }
            return result;
        }



        public async Task<ApiResultDTO<bool?>> EnterUpRankBattle()
        {
            var result = new ApiResultDTO<bool?>();
            // Internal logic battleservice
            var master = await userService.GetMasterAsync(identityService.CurrentUserId);
            var formationMaster = master.GetFormation(BattleType.PVP.ToString());
            if (formationMaster == null || formationMaster.DragonIds.Count == 0)
            {
                result.AddError(ErrorCodeEnum.PVETeamNotSetup);
                return result;
            }
            var rankPosition = await rankPositionRepo.Find(r => r.MainUserId == master.MainUserId).FirstOrDefaultAsync();
            if (rankPosition.RankPosition != 1)
            {
                result.AddError(ErrorCodeEnum.PVETeamNotSetup);
                return result;
            }
            var lstTeamPvPBot = GameConfigs.TeamPVPMobConfigs.Where(t => t.TierRank.ToLower() == rankPosition.Tier.ToString().ToLower()).ToList();
            var random = new Random();
            var teamPvPBot = lstTeamPvPBot[random.Next(lstTeamPvPBot.Count)];

            var battleId = SnowflakeId.Default().NextId().ToString();
            var cachedActiveBattle = await this.GetCachedActiveBattleAsync(master.Id);
            if (cachedActiveBattle != null && battleId != cachedActiveBattle.Id)
            {
                result.AddError(ErrorCodeEnum.BattleNotSetup);
                result.Data = false;
                return result;
            }
            //validate rongos master
            var idsDragon = formationMaster.DragonIds.FindAll(id => id != "");
            var lstDragon = dragonRepo.AsQueryable().FilterAccessibleDragonByWallets(master.Wallets.ToArray()).Where(d => idsDragon.Contains(d.Id)).ToList();
            if (lstDragon.Count < idsDragon.Count)
            {
                result.AddError(ErrorCodeEnum.DragonNotOwned, "rongos not owned User");
                result.Data = false;
                return result;
            }
            //validate rongos enemy player
            var battleData = new BattleCacheItem();
            string cookie = string.Empty;
            var battleMasterEto = new BattleMasterEvtItem
            {
                Id = master.Id,
                MainUserId = master.MainUserId,
                UserId = master.MainUserId,
                Name = master.Name,
                Level = (uint)master.Level,
                Exp = (ulong)master.Exp,
            };
            var battleMEnemyPlayerEto = new BattleMasterEvtItem
            {
                Id = teamPvPBot.TeamPVPId,
                MainUserId = teamPvPBot.TeamPVPId,
                UserId = teamPvPBot.TeamPVPId,
                Name = teamPvPBot.Name,
                Level = (uint)master.Level,
                Exp = (ulong)master.Exp
            };

            // TODO refactoring here, this temporary transform
            var (teamPower, battleDragons) = buildFormationBattleData(lstDragon, BattleType.PVP);
            battleDragons.ForEach(r =>
            {
                r.UserId = master.MainUserId;
                r.MainUserId = master.MainUserId;
                r.MasterId = master.Id;
                r.IsOffChain = lstDragon.Find(_ => _.Id == r.Id).IsOffChain;
                r.IndexFormation = formationMaster.DragonIds.IndexOf(r.Id) + 1;
            });


            var mobs = mapper.Map<List<Mob>, List<DragonMob>>(GameConfigs.Mobs.FindAll(c => teamPvPBot.DragonMobIds.Contains(c.Id)));
            mobs.ForEach(m => m.PowerRate = teamPvPBot.MobPowerRate);
            var dragonMobs = mapper.Map<List<DragonMob>, List<BattleDragonEvtItem>>(mobs);
            dragonMobs.ForEach(m =>
            {
                var slot = teamPvPBot.Mobs.FirstOrDefault(m => m.Id == m.Id);
                m.IndexFormation = slot != null ? slot.IndexInFormation : 0;
            });
            var pvpBattle = new PVPBattleRequestIntegrationEvent
            {
                Id = battleId,
                Master = battleMasterEto,
                MasterDragons = battleDragons,
                EnemyPlayer = battleMEnemyPlayerEto,
                DragonesEnemy = dragonMobs,

            };
            var json = JsonSerializer.Serialize(pvpBattle);
            var postData = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");
            var client = clientFactory.CreateClient("DragonApiClient");
            string url = gameCfgLoader.GameConfigs.BattleServerConfig.GetPVPBattleRequestUrl();
            var resp = await client.PostAsync(url, postData);
            resp.EnsureSuccessStatusCode();
            var resultContent = await resp.Content.ReadAsStringAsync();
            logger.LogInformation($"battle instance name: {resultContent}");

            if (GameConfigs.BattleServerConfig.CookieRequired)
            {
                if (resp.Headers.Contains("Set-Cookie"))
                {
                    IEnumerable<string> cookieHeader;
                    resp.Headers.TryGetValues("Set-Cookie", out cookieHeader);
                    if (cookieHeader != null && cookieHeader.Count() > 0)
                    {
                        await SaveCacheBattleRequestCookie(pvpBattle.Id, cookieHeader.First());
                        cookie = cookieHeader.First();
                    }
                }
            }
            battleData.Cookie = cookie;
            var success = await saveOrUpdateMasterBattleInfo(battleData);
            if (success)
            {
                logger.LogDebug("battleID: {0}, Cookie: {1}", battleId, cookie);
                BackgroundJob.Schedule(() => releaseMasters(battleId), DateTimeOffset.UtcNow.AddMinutes(2));
                result.Data = true;
                var hubMsg = new SendMessageRequest
                {
                    Mode = SendMessageRequest.Types.SendMessageMode.Users,

                    Payload = new SendMessageRequest.Types.SendMessagePayload
                    {
                        Type = "BattleCreated",
                        Payload = JsonSerializer.Serialize(new HubMessageBattleCreated
                        {
                            SessionId = battleId,
                            Cookie = cookie
                        })
                    }
                };
                hubMsg.UserIds.Add(master.MainUserId);
                await hubFwdClient.SendMessageAsync(hubMsg);

            }
            else
            {
                result.Data = false;
                result.AddError(ErrorCodeEnum.BattleNotSetup);
            }
            return result;
        }




        private (double, List<BattleDragonEvtItem>) buildFormationBattleData(List<DragonDAO> rongoses, BattleType battleType)
        {
            var _rongoses = mapper.Map<List<DragonDAO>, List<DragonAPI.Common.Dragon>>(rongoses);
            var teamPower = rongosCalculator.CalculateTeamPower(_rongoses);
            var battleDragones = mapper.Map<List<DragonDAO>, List<BattleDragonEvtItem>>(rongoses, opts =>
            {
                opts.AfterMap((src, dest) =>
                {
                    for (int i = 0; i < dest.Count; ++i)
                    {
                        logger.LogDebug($"rongos nftId {dest[i].NftId}");
                        if (battleType == BattleType.PVE)
                        {
                            dest[i].Stats.ReBuildStats(GameConfigs.StatCoef);
                        }
                    }
                });
            });

            return (teamPower, battleDragones);
        }

        public Task<ApiResultDTO<IEnumerable<LastStageTimeOpenInfoDTO>>> GetOpenTimeLastStage(string stageId)
        {
            var result = new ApiResultDTO<IEnumerable<LastStageTimeOpenInfoDTO>>();
            var stage = GameConfigs.WorldMapConfig.Stages.Find(s => s.Id == stageId);
            if (!stage.LastStage)
            {
                result.AddError(ErrorCodeEnum.IsNotLastStage);
            }
            else
            {
                var keyByDay = CacheKeys.LastStageWinnerKey + $":{DateTime.UtcNow.ToShortDateString()}:{stageId}";
                var sots = GameConfigs.WorldMapConfig.ListLastStageOpenTime.FindAll(sot => sot.StageId == stageId).Select(s =>
                {
                    var keyByTime = $"{keyByDay}:{s.Time}";
                    var winners = this.redisDatabase.ListRange(keyByTime);
                    var remaining = s.Total - winners.Count();
                    return new LastStageTimeOpenInfoDTO
                    {
                        HourUTC = s.Time,
                        Total = s.Total,
                        Remaining = remaining > 0 ? remaining : 0,
                    };
                });
                result.Data = sots;
            }
            return Task.FromResult(result);
        }
        private async Task<StageFormationsConfig> GetRandomStageFormation(string stageId)
        {
            var stage = GameConfigs.WorldMapConfig.Stages.Find(s => s.Id == stageId);
            int? timeWindow = null;
            // if (stage.LastStage)
            // {
            //     var openTimes = GameConfigs.WorldMapConfig.ListLastStageOpenTime.FindAll(sot => sot.StageId == stageId && sot.Time <= DateTime.UtcNow.Hour);
            //     if (openTimes.Count > 0)
            //         timeWindow = openTimes.OrderByDescending(sot => sot.Time).First().Time;
            // }
            var stageMobFormation = GameConfigs.WorldMapConfig.StageFormations.FirstOrDefault(s => s.StageId == stageId);
            // if (stageMobFormations.Count > 0)
            // {
            //     var rand = new Random();
            //     var idx = rand.Next(0, stageMobFormations.Count);
            //     return stageMobFormations[idx];
            // }
            return stageMobFormation;
        }
        private Task<List<BattleDragonEvtItem>> getMobs(StageFormationsConfig stageFormationConfig)
        {
            var mobs = mapper.Map<List<Mob>, List<DragonMob>>(GameConfigs.Mobs.FindAll(c => stageFormationConfig.MobIds.Contains(c.Id)));
            mobs.ForEach(m => m.PowerRate = stageFormationConfig.MobPowerRate);
            var dragonMobs = mapper.Map<List<DragonMob>, List<BattleDragonEvtItem>>(mobs);
            dragonMobs.ForEach(m =>
            {
                var slot = stageFormationConfig.Mobs.FirstOrDefault(m => m.Id == m.Id);
                m.IndexFormation = slot != null ? slot.IndexInFormation : 0;
            });
            return Task.FromResult(dragonMobs.OrderBy(m => m.Id).ToList());
        }

        // private Task<List<StageFormationsConfig>> GetStageMobFormations(string stageId, int? timeWindow)
        // {
        //     var formations = GameConfigs.WorldMapConfig.StageFormations.FirstOrDefault(s => s.StageId == stageId);
        //     return Task.FromResult(formations);
        // }
        private async Task<string> handlePVEBattlePrepared(BattleCacheItem battle)
        {

            var stageFormation = GameConfigs.WorldMapConfig.StageFormations.Find(sf => sf.FormationId == battle.Metadata.StageFormationId);
            var Mobs = await getMobs(stageFormation);
            var cachedBattleMaster = battle.Masters.First();
            var battleMasterEto = new BattleMasterEvtItem
            {
                Id = cachedBattleMaster.Id,
                MainUserId = cachedBattleMaster.MainUserId,
                UserId = cachedBattleMaster.UserId,
                Name = cachedBattleMaster.Name,
                Level = (uint)cachedBattleMaster.Level,
                Exp = (ulong)cachedBattleMaster.Exp,
                AutoAttack = cachedBattleMaster.AutoAttack,
                X3Speed = cachedBattleMaster.X3Speed,
            };

            // TODO refactoring here, this temporary transform
            var (teamPower, battleDragons) = buildFormationBattleData(cachedBattleMaster.Dragones, BattleType.PVE);
            battleDragons.ForEach(r =>
            {
                r.UserId = cachedBattleMaster.UserId;
                r.MainUserId = cachedBattleMaster.MainUserId;
                r.MasterId = cachedBattleMaster.Id;
                r.IsOffChain = cachedBattleMaster.Dragones.Find(_ => _.Id == r.Id).IsOffChain;
                r.IndexFormation = cachedBattleMaster.DragonIds.IndexOf(r.Id) + 1;
            });
            var pveBattle = new PVEBattleRequestIntegrationEvent
            {
                Id = battle.Id,
                Master = battleMasterEto,
                MasterDragons = battleDragons,
                Metadata = new PVEBattleRequestIntegrationEvent.PVEMetadata
                {
                    StageId = stageFormation.StageId,
                    MapId = battle.Metadata.MapId,
                    StageMobFormationId = stageFormation.FormationId,
                },
                StageId = stageFormation.StageId,
                StageMobFormationId = stageFormation.FormationId,
                EnvironmentId = stageFormation.EnvironmentId ?? string.Empty,
                DragonMobs = Mobs

            };
            var json = JsonSerializer.Serialize(pveBattle);
            var postData = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");
            var client = clientFactory.CreateClient("DragonApiClient");
            string url = gameCfgLoader.GameConfigs.BattleServerConfig.GetPVEBattleRequestUrl();
            var resp = await client.PostAsync(url, postData);
            resp.EnsureSuccessStatusCode();
            var resultContent = await resp.Content.ReadAsStringAsync();
            logger.LogInformation($"battle instance name: {resultContent}");

            if (GameConfigs.BattleServerConfig.CookieRequired)
            {
                if (resp.Headers.Contains("Set-Cookie"))
                {
                    IEnumerable<string> cookieHeader;
                    resp.Headers.TryGetValues("Set-Cookie", out cookieHeader);
                    if (cookieHeader != null && cookieHeader.Count() > 0)
                    {
                        await SaveCacheBattleRequestCookie(pveBattle.Id, cookieHeader.First());
                        return cookieHeader.First();
                    }
                }
            }
            return string.Empty;
        }
        private async Task<string> handlePVPBattlePrepared(BattleCacheItem battle, string masterId)
        {
            var cachedBattleMaster = battle.Masters.FirstOrDefault(m => m.Id == masterId);
            var battleMasterEto = new BattleMasterEvtItem
            {
                Id = cachedBattleMaster.Id,
                MainUserId = cachedBattleMaster.MainUserId,
                UserId = cachedBattleMaster.UserId,
                Name = cachedBattleMaster.Name,
                Level = (uint)cachedBattleMaster.Level,
                Exp = (ulong)cachedBattleMaster.Exp,
                AutoAttack = cachedBattleMaster.AutoAttack,
                X3Speed = cachedBattleMaster.X3Speed,
            };
            var cachedBattleEnemy = battle.Masters.FirstOrDefault(m => m.Id != masterId);
            var battleMEnemyPlayerEto = new BattleMasterEvtItem
            {
                Id = cachedBattleEnemy.Id,
                MainUserId = cachedBattleEnemy.MainUserId,
                UserId = cachedBattleEnemy.MainUserId,
                Name = cachedBattleEnemy.Name,
                Level = (uint)cachedBattleEnemy.Level,
                Exp = (ulong)cachedBattleEnemy.Exp,
                AutoAttack = cachedBattleEnemy.AutoAttack,
                X3Speed = cachedBattleEnemy.X3Speed,
            };

            // TODO refactoring here, this temporary transform
            var (teamPower, battleDragons) = buildFormationBattleData(cachedBattleMaster.Dragones, BattleType.PVE);
            battleDragons.ForEach(r =>
            {
                r.UserId = cachedBattleMaster.UserId;
                r.MainUserId = cachedBattleMaster.MainUserId;
                r.MasterId = cachedBattleMaster.Id;
                r.IsOffChain = cachedBattleMaster.Dragones.Find(_ => _.Id == r.Id).IsOffChain;
                r.IndexFormation = cachedBattleMaster.DragonIds.IndexOf(r.Id) + 1;
            });

            var (teamPowerEnemy, battleDragonsEnemy) = buildFormationBattleData(cachedBattleEnemy.Dragones, BattleType.PVP);
            battleDragons.ForEach(r =>
            {
                r.UserId = cachedBattleEnemy.MainUserId;
                r.MainUserId = cachedBattleEnemy.MainUserId;
                r.MasterId = cachedBattleEnemy.Id;
                r.IsOffChain = cachedBattleMaster.Dragones.Find(_ => _.Id == r.Id).IsOffChain;
                r.IndexFormation = cachedBattleMaster.DragonIds.IndexOf(r.Id) + 1;
            });
            var pvpBattle = new PVPBattleRequestIntegrationEvent
            {
                Id = battle.Id,
                Master = battleMasterEto,
                MasterDragons = battleDragons,
                EnemyPlayer = battleMEnemyPlayerEto,
                DragonesEnemy = battleDragonsEnemy,

            };
            var json = JsonSerializer.Serialize(pvpBattle);
            var postData = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");
            var client = clientFactory.CreateClient("DragonApiClient");
            string url = gameCfgLoader.GameConfigs.BattleServerConfig.GetPVPBattleRequestUrl();
            var resp = await client.PostAsync(url, postData);
            resp.EnsureSuccessStatusCode();
            var resultContent = await resp.Content.ReadAsStringAsync();
            logger.LogInformation($"battle instance name: {resultContent}");

            if (GameConfigs.BattleServerConfig.CookieRequired)
            {
                if (resp.Headers.Contains("Set-Cookie"))
                {
                    IEnumerable<string> cookieHeader;
                    resp.Headers.TryGetValues("Set-Cookie", out cookieHeader);
                    if (cookieHeader != null && cookieHeader.Count() > 0)
                    {
                        await SaveCacheBattleRequestCookie(pvpBattle.Id, cookieHeader.First());
                        return cookieHeader.First();
                    }
                }
            }
            return string.Empty;
        }


        // public async Task ProcessBattleRegistered(BattleServerRegisteredIntegrationEvent registeredBattle)
        // {
        //     // if (GameConfigs.BattleServerConfig.CookieRequired)
        //     // {
        //     //     string cookie = await GetDeleteCacheBattleRequestCookie(registeredBattle.Id);
        //     //     if (string.IsNullOrEmpty(cookie))
        //     //     {
        //     //         logger.LogError($"Battle {registeredBattle.Id} cookie request not exist");
        //     //         return;
        //     //     }
        //     //     registeredBattle.Cookie = cookie;
        //     // }

        //     // // // Take energy
        //     // // var energyConsumed = GetRequiredEnergy(registeredBattle);
        //     // foreach (var master in registeredBattle.Masters)
        //     // {
        //     //     // await masterService.TakeEnergyCost(master.Id, energyConsumed, "EnergyForBattle_" + registeredBattle.Type.ToString());
        //     //     await masterService.UpdateAutoAndSpeedRun(master.Id, master.AutoAttack, master.X3Speed);

        //     //     /// tracing log
        //     //     registeredBattle.Dragons.FindAll(d => master.UserId == d.UserId).ForEach(d =>
        //     //     {
        //     //         logger.LogInformation($"DragonInBattle {mapper.Map<BattleDragonEvtItem, BattleDragonAnalysis>(d).ToAnalysisLog(LoggingContextEnum.BattleContext)}");
        //     //     });
        //     //     // registeredBattle.DragonSeals.FindAll(s => master.UserId == s.UserId).ForEach(s =>
        //     //     // {
        //     //     //     logger.LogInformation($"SealInBattle {mapper.Map<BattleDragonSealEvtItem, BattleSealAnalysis>(s).ToAnalysisLog(LoggingContextEnum.BattleContext)}");
        //     //     // });
        //     // }
        //     // //buildNotifyCommandForBattleCreatedEvent(registeredBattle);


        // }
        public async Task ProcessBattleStateUpdatedEvt(BattleServerStateChangedIntegrationEvent battleData)
        {
            logger.LogDebug($"Battle {battleData.Id} state {battleData.State.ToString()}");
            if (battleData.State == BattleState.Completed)
            {
                await processBattleCompleted(battleData);
                await releaseMasters(battleData.Id);
            }
            else if (battleData.State == BattleState.Cancelled)
            {
                await processBattleCancelled(battleData);
                await releaseMasters(battleData.Id);
            }
            else if (battleData.State == BattleState.Timeout)
            {
                await processBattleTimeout(battleData);
                await releaseMasters(battleData.Id);
            }
        }
        public async Task ProcessBattlesDestroyedEvt(BattlesDestroyedIntegrationEvent evtData)
        {
            var releaseTasks = new List<Task>();
            evtData.BattleIds.ForEach(id =>
            {
                releaseTasks.Add(releaseMasters(id));
            });
            await Task.WhenAll(releaseTasks);
        }






        //--------------------------
        private async Task<bool> saveOrUpdateMasterBattleInfo(BattleCacheItem battle)
        {
            foreach (var master in battle.Masters)
            {
                var success = await this.redisDatabase.SetAddAsync($"{CacheKeys.ActiveBattleIdKey}:{battle.Id}", master.Id);
                if (!success) return false;
                var activeBattle = new ActiveBattleCache
                {
                    Id = battle.Id,
                    MasterId = master.Id,
                    MainUserId = master.MainUserId,
                    UserId = master.UserId,
                    BattleType = BattleType.PVE.ToString(),
                    Cookie = battle.Cookie,
                };
                var ret = await this.redisDatabase.StringSetAsync($"{CacheKeys.ActiveBattleMasterKey}:{master.Id}", activeBattle.ToRawJson());
                if (!ret) return false;
            }
            return true;
        }

        // private async Task lockMasters(BattleRegisteredIntegrationEvent registeredBattle)
        // {
        //     var battleMasters = registeredBattle.Masters.Where(c => c.IsBot == false);
        //     var masterIds = battleMasters.Select(bm => bm.Id).ToList();
        //     var masters = await masterRepo.Find(m => masterIds.Contains(m.Id)).ToListAsync();
        //     foreach (var battleMaster in battleMasters)
        //     {
        //         var master = masters.Find(m => m.Id == battleMaster.Id);
        //         var success = await this.redisDatabase.SetAddAsync($"{CacheKeys.ActiveBattleIdKey)}:{registeredBattle.Id}", battleMaster.Id);
        //         if (success)
        //         {
        //             var activeBattle = new ActiveBattleCache
        //             {
        //                 Id = registeredBattle.Id,
        //                 MasterId = battleMaster.Id,
        //                 MainUserId = battleMaster.MainUserId,
        //                 UserId = battleMaster.UserId,
        //                 BattleType = registeredBattle.Type.ToString(),
        //                 Cookie = registeredBattle.Cookie,
        //             };
        //             await this.redisDatabase.StringSetAsync($"{CacheKeys.ActiveBattleMasterKey)}:{battleMaster.Id}", activeBattle.ToRawJson());




        //             // // After storing active battle into cache system, consume energy now
        //             // var energyCost = GameConfigs.GameEnergyConfig.GetEnergyConfig(master.IsPremium).EnergyPerPVPBattle;

        //             // if (registeredBattle.Type == BattleType.PVE)
        //             // {
        //             //     var stage = GameConfigs.WorldMapConfig.Stages.Find(s => s.Id == registeredBattle.StageId);
        //             //     energyCost = (int)stage.Energy;

        //             //     // var rongoses = registeredBattle.Dragons.Where(c => !c.IsMob && c.Level >= GameConfigs.DragonStressConfig.LevelApply).ToList();
        //             //     // foreach (var rongos in rongoses)
        //             //     // {
        //             //     //     await rongosService.TakeHappyPoint(rongos.NftId, GameConfigs.DragonStressConfig.NumOfHappyPointConsumePVE, "HappyPointForBattle_" + registeredBattle.Type.ToString());
        //             //     // }
        //             // }
        //             // else
        //             // {
        //             //     if (registeredBattle.Type == BattleType.PVP)
        //             //     {
        //             //         // var rongoses = registeredBattle.Dragons.Where(c => !c.IsMob && c.Level >= GameConfigs.DragonStressConfig.LevelApply).ToList();
        //             //         // foreach (var rongos in rongoses)
        //             //         // {
        //             //         //     await rongosService.AddHappyPoint(rongos.NftId, GameConfigs.DragonStressConfig.NumOfHappyPointRegenerationPVP);
        //             //         // }
        //             //     }
        //             // }

        //             // await masterService.TakeEnergyCost(battleMaster.Id, energyCost, "EnergyForBattle_" + registeredBattle.Type.ToString());

        //             /// tracing log
        //             logger.LogInformation($"BattleLocked {activeBattle.ToAnalysisLog(LoggingContextEnum.BattleContext)}");
        //             registeredBattle.Dragons.FindAll(d => battleMaster.UserId == d.UserId).ForEach(d =>
        //             {
        //                 logger.LogInformation($"DragonInBattle {mapper.Map<BattleDragonEvtItem, BattleDragonAnalysis>(d).ToAnalysisLog(LoggingContextEnum.BattleContext)}");
        //             });
        //             registeredBattle.DragonSeals.FindAll(s => battleMaster.UserId == s.UserId).ForEach(s =>
        //             {
        //                 logger.LogInformation($"SealInBattle {mapper.Map<BattleDragonSealEvtItem, BattleSealAnalysis>(s).ToAnalysisLog(LoggingContextEnum.BattleContext)}");
        //             });
        //         }
        //     }
        //     logger.LogInformation($"{(registeredBattle.Type.ToString() + "_Battle")} {mapper.Map<BattleRegisteredIntegrationEvent, BattleAnalysis>(registeredBattle).ToAnalysisLog(LoggingContextEnum.BattleContext)}");
        // }
        public async Task releaseMasters(string battleId)
        {
            var battleActiveKey = $"{CacheKeys.ActiveBattleIdKey}:{battleId}";
            var masterIds = await this.redisDatabase.SetMembersAsync(battleActiveKey);
            foreach (var masterId in masterIds)
            {
                var cachedActiveBattle = await this.GetCachedActiveBattleAsync(masterId);
                if (cachedActiveBattle != null && cachedActiveBattle.Id == battleId)
                {
                    await this.redisDatabase.KeyDeleteAsync($"{CacheKeys.ActiveBattleMasterKey}:{masterId}");
                    logger.LogInformation($"BattleReleased {cachedActiveBattle.ToAnalysisLog(LoggingContextEnum.BattleContext)}");
                }
            }
            await this.redisDatabase.KeyDeleteAsync(battleActiveKey);
        }

        private async Task processPVEBattleCompleted(BattleServerStateChangedIntegrationEvent battleData)
        {
            if (string.IsNullOrEmpty(battleData.StageId))
            {
                logger.LogDebug("Wrong PVE battle data, lack map stage id data");
                return;
            }

            var masterIds = battleData.Masters.Where(c => !c.IsBot).Select(m => m.Id).ToList();
            var master = await masterRepo.Find(m => masterIds.Contains(m.Id)).FirstOrDefaultAsync();
            if (master != null)
            {

                var battleCompletedEto = new BattleCompletedETO
                {
                    Id = battleData.Id,
                    LoserIds = battleData.LoserIds,
                    AfkIds = battleData.AfkIds,
                    // Masters = battleData.Masters,
                    // Dragons = battleData.Dragons,
                };

                var bMaster = battleData.Masters.Find(m => m.Id == master.Id);

                bool isWon = !battleData.LoserIds.Contains(bMaster.Id);
                var hubMsg = new SendMessageRequest();
                var winningBattleResult = new WinningBattleResultDto();
                if (isWon)
                {
                    winningBattleResult = new WinningBattleResultDto
                    {
                        Id = battleData.Id,
                        Rss = 0,
                        LRss = 0,
                        ExpReward = new BattleExpRewardDto
                        {
                            Id = battleData.Id,
                            Master = await handleMasterAfterBattle(0, 0, 0, master),
                        },
                        PVETime = await handlePVETime(master, battleData),
                        PVEDropItems = await handlePVEDropItemsForWinner(master, battleData)
                    };

                    var stageReq = GameConfigs.CommonSetting.FirstOrDefault(c => c.Key == "StageUnLockAfkFarming").Value;
                    if (Convert.ToInt64(battleData.StageId) == Convert.ToInt64(stageReq))
                    {
                        masterService.CreatePoolFarmJob(master.MainUserId);
                    }
                    else
                    {
                        masterService.SnapshotFarmPoolJob(master.MainUserId);
                    }
                }

                battleCompletedEto.WinningResult = winningBattleResult;
                hubMsg.Mode = SendMessageRequest.Types.SendMessageMode.Users;

                hubMsg.Payload = new SendMessageRequest.Types.SendMessagePayload
                {
                    Type = "BattleEnd",
                    Payload = JsonSerializer.Serialize(battleCompletedEto)
                };
                hubMsg.UserIds.Add(master.MainUserId);
                await hubFwdClient.SendMessageAsync(hubMsg);
                // await eventBus.PublishAsync(IntegrationTopicConst.BattleService, new IntegrationMessageETO
                // {
                //     EventName = EventNameConst.BattleCompletedEvent,
                //     Payload = battleCompletedEto
                // });
                // await rongosService.AddExpDragonCompletedEvent(battleCompletedEto);
            }
        }
        private async Task processPVPBattleCompleted(BattleServerStateChangedIntegrationEvent battleData)
        {


            var currentSeason = gameCfgLoader.GameConfigs.SeasonConfigs.Where(s => s.StartingTime < DateTime.UtcNow && s.EndingTime > DateTime.UtcNow).FirstOrDefault();
            var loserPlayer = await masterRepo.Find(m => battleData.LoserIds.Contains(m.Id)).FirstOrDefaultAsync();
            var winnerId = battleData.Masters.Where(c => c.Id != loserPlayer.Id).Select(m => m.Id).FirstOrDefault();
            var winnerPlayer = await masterRepo.Find(m => m.Id == winnerId).FirstOrDefaultAsync();
            if (winnerPlayer != null && loserPlayer != null)
            {

                var battleCompletedEto = new BattlePvPCompletedETO
                {
                    Id = battleData.Id,
                    WinnerId = winnerPlayer.Id
                };
                var rankPositionWinner = await rankPositionRepo.Find(r => r.MainUserId == winnerPlayer.MainUserId).FirstOrDefaultAsync();
                var rankPositionLoser = await rankPositionRepo.Find(r => r.MainUserId == loserPlayer.MainUserId).FirstOrDefaultAsync();

                if (rankPositionWinner == null)
                {
                    logger.LogDebug("Wrong rank battle data");
                    return;
                }
                if (rankPositionLoser == null)
                {
                    logger.LogDebug("Wrong rank battle data");
                    return;
                }

                if (rankPositionWinner.RankPosition > rankPositionLoser.RankPosition)
                {
                    rankPositionLoser.MainUserId = winnerPlayer.MainUserId;
                    rankPositionWinner.MainUserId = loserPlayer.MainUserId;
                    await rankPositionRepo.ReplaceOneAsync(m => m.Id == rankPositionLoser.Id, rankPositionLoser);
                    await rankPositionRepo.ReplaceOneAsync(m => m.Id == rankPositionWinner.Id, rankPositionWinner);
                    battleCompletedEto.WinPosition = rankPositionLoser.RankPosition;
                }
                // var bMaster = battleData.Masters.Find(m => m.Id == master.Id);

                // bool isWon = !battleData.LoserIds.Contains(bMaster.Id);

                // var winningBattleResult = new WinningBattleResultDto();
                // if (isWon)
                // {
                //     //battleCompletedEto.BonusDragonExp = GetBonusExpForDragon(battleData);
                //     var bonusRss = GetBonusRssForMaster(master, battleData);
                //     var bonusExpForMaster = GetBonusExpForMaster(master, battleData);

                //     winningBattleResult = new WinningBattleResultDto
                //     {
                //         Id = battleData.Id,
                //         Rss = bonusRss,
                //         LRss = 0,
                //         ExpReward = new BattleExpRewardDto
                //         {
                //             Id = battleData.Id,
                //             Master = await handleMasterAfterBattle(bonusRss, 0, bonusExpForMaster, master),
                //         },
                //         PVETime = await handlePVETime(master, battleData),
                //         PVEDropItems = await handlePVEDropItemsForWinner(master, battleData)
                //     };

                // }

                var hubMsg = new SendMessageRequest();
                hubMsg.Mode = SendMessageRequest.Types.SendMessageMode.Users;

                hubMsg.Payload = new SendMessageRequest.Types.SendMessagePayload
                {
                    Type = "BattleEnd",
                    Payload = JsonSerializer.Serialize(battleCompletedEto)
                };



                hubMsg.UserIds.Add(winnerPlayer.MainUserId);
                hubMsg.UserIds.Add(loserPlayer.MainUserId);
                await hubFwdClient.SendMessageAsync(hubMsg);



                var pvpLog = await pvpHistoryRepo.Find(p => p.BattleId == battleData.Id).FirstOrDefaultAsync();
                if (pvpLog != null)
                {
                    pvpLog.BattleStatus = winnerPlayer.MainUserId == pvpLog.MainUserId ? BattleStatus.Win : BattleStatus.Lose;
                    if (pvpLog.BattleStatus == BattleStatus.Win)
                    {
                        pvpLog.RankStatus = "Tăng" + (rankPositionLoser.RankPosition - rankPositionWinner.RankPosition);
                    }
                }
                // await eventBus.PublishAsync(IntegrationTopicConst.BattleService, new IntegrationMeageETO
                // {
                //     EventName = EventNameConst.BattleCompletedEvent,
                //     Payload = battleCompletedEto
                // });
                // await rongosService.AddExpDragonCompletedEvent(battleCompletedEto);
            }
        }
        private async Task processBattleCompleted(BattleServerStateChangedIntegrationEvent battleData)
        {
            if (battleData.Type == BattleType.PVE)
            {
                await processPVEBattleCompleted(battleData);
            }
            if (battleData.Type == BattleType.PVP)
            {
                await processPVPBattleCompleted(battleData);
            }
            else
            {
                logger.LogDebug("Invalid battle");
            }
        }

        // private async Task<(AchievementStatsDAO, AchievementStatsDAO)> handlePVPRankAfterBattle(bool isWon, MasterDAO master, BattleServerStateChangedIntegrationEvent battleData)
        // {
        //     if (!isWon && battleData.AfkIds != null && battleData.AfkIds.Length != 0)
        //     {
        //         _ = masterService.DecreaseReputation(battleData.AfkIds);
        //     }
        //     var (prevAchievementStats, newAchievementStats) = await masterService.ProcessPvPRankingResult(master.Id, isWon);
        //     if (prevAchievementStats != null && newAchievementStats != null)
        //     {
        //         _ = masterService.GenerateAchievementEventsToClient(prevAchievementStats, newAchievementStats, isWon);
        //     }
        //     return (prevAchievementStats, newAchievementStats);
        // }

        private async Task<BattleExpRewardDto.RewardExp> handleMasterAfterBattle(int bonusRss, int bonusLrss, long bonusExp, MasterDAO winnerMaster)
        {
            BattleExpRewardDto.RewardExp expReward = new BattleExpRewardDto.RewardExp
            {
                Id = winnerMaster.Id,
                Level = winnerMaster.Level,
            };

            // if (bonusRss > 0 || bonusLrss > 0)
            // {
            //     await masterService.BonusCurrency(winnerMaster, bonusRss, bonusLrss);
            // }

            var levelConfig = GameConfigs.LevelExpConfig.EntityLevelConfigMap[Dragon.Blueprints.LevelTypeEnum.Master.Code];
            if (bonusExp > 0 && winnerMaster.Level < levelConfig.MaxLevel)
            {
                var success = await masterService.AddExp(winnerMaster, bonusExp);
                if (success)
                {
                    expReward.Level = winnerMaster.Level;
                    expReward.Exp = winnerMaster.Exp;
                    expReward.BonusExp = bonusExp;
                    expReward.MaxExp = (ulong)levelConfig.GetExpConfigAtLevel(winnerMaster.Level).MaxExp;
                }
            }
            return expReward;
        }

        // private List<BattleExpRewardDto.RewardExp> handleDragonsAfterBattle(MasterDAO winnerMaster, BattleMasterEvtItem battleWinnerMaster, BattleServerStateChangedIntegrationEvent battleData)
        // {
        //     var levelConfig = GameConfigs.LevelExpConfig.EntityLevelConfigMap[Dragon.Blueprints.LevelTypeEnum.Dragon.Code];
        //     var playerDragons = battleData.Dragons.FindAll(d => d.IsMob == false && (d.MasterId == winnerMaster.Id || d.UserId == battleWinnerMaster.UserId));
        //     var bonusExp = GetBonusExpForDragon(battleData);
        //     var expRewardDragones = new List<BattleExpRewardDto.RewardExp>();
        //     foreach (BattleDragonEvtItem dragonEvtItem in playerDragons)
        //     {
        //         if (dragonEvtItem != null)
        //         {
        //             var expReward = new BattleExpRewardDto.RewardExp
        //             {
        //                 Id = dragonEvtItem.Id,
        //                 Level = dragonEvtItem.Level,
        //             };
        //             if (dragonEvtItem.Level < levelConfig.MaxLevel)
        //             {
        //                 var (level, finalExp) = LevelCalculator.AddExp(dragonEvtItem.Exp, bonusExp, dragonEvtItem.IsOffChain ? 50 : levelConfig.MaxLevel, levelConfig);
        //                 expReward.Level = level;
        //                 expReward.Exp = finalExp;
        //                 expReward.BonusExp = bonusExp;
        //                 expReward.MaxExp = (ulong)levelConfig.GetExpConfigAtLevel(level).MaxExp;
        //             }
        //             expRewardDragones.Add(expReward);
        //         }
        //     }
        //     return expRewardDragones;
        // }



        private async Task<BattlePVETimeDto> handlePVETime(MasterDAO master, BattleServerStateChangedIntegrationEvent battleData)
        {
            var isBestTime = true;
            var seconds = (long)(DateTime.UtcNow - battleData.CreatedAt).TotalSeconds;
            var masterStageTracking = await masterStageTrackingRepo.Find(t => t.MasterId == master.Id && t.StageId == battleData.StageId).FirstOrDefaultAsync();
            if (masterStageTracking != null)
            {
                if (seconds < masterStageTracking.BestTime)
                {
                    masterStageTracking.BestTime = seconds;
                    masterStageTracking.StageFormationId = battleData.StageMobFormationId;
                    await masterStageTrackingRepo.ReplaceOneAsync(m => m.Id == masterStageTracking.Id, masterStageTracking);
                }
                else
                {
                    isBestTime = false;
                }
            }
            else
            {
                var configedStages = GameConfigs.WorldMapConfig.Stages.FindAll(ms => ms.Id == battleData.StageId).FirstOrDefault();
                masterStageTracking = new MasterStageTrackingDAO
                {
                    MasterId = master.Id,
                    StageId = battleData.StageId,
                    Index = (ushort)configedStages.Index,
                    StageFormationId = battleData.StageMobFormationId,
                    BestTime = seconds,
                };
                await masterStageTrackingRepo.InsertOneAsync(masterStageTracking);
            }

            var timeResult = new BattlePVETimeDto
            {
                IsBest = isBestTime,
                Seconds = seconds,
                BestSeconds = (long)masterStageTracking.BestTime,
            };
            return timeResult;
        }

        private async Task<bool> isLastStageBattleValid(MapStageConfig stage)
        {
            var currentStageOpenTime = GameConfigs.WorldMapConfig.ListLastStageOpenTime.FindAll(sot => sot.StageId == stage.Id && sot.Time <= DateTime.UtcNow.Hour).OrderByDescending(sot => sot.Time).FirstOrDefault();
            if (currentStageOpenTime == null)
            {
                return false;
            }
            var keyByDay = CacheKeys.LastStageWinnerKey + $":{DateTime.UtcNow.ToShortDateString()}:{stage.Id}";
            var keyByTime = $"{keyByDay}:{currentStageOpenTime.Time}";
            var winners = await this.redisDatabase.ListRangeAsync(keyByTime);
            return winners.Count() < currentStageOpenTime.Total;
        }

        private async Task<List<ClaimableItemDto>> handlePVEDropItemsForWinner(MasterDAO master, BattleServerStateChangedIntegrationEvent battleData)
        {
            var claimItems = new List<ClaimableItemDto>();
            var stage = GameConfigs.WorldMapConfig.Stages.First(s => s.Id == battleData.StageId);
            var formationConfig = GameConfigs.WorldMapConfig.StageFormations.Find(s => s.FormationId == battleData.StageMobFormationId);
            if (stage != null && formationConfig != null)
            {
                int randValue = RandomNumberGenerator.GetInt32(0, 100);
                if (randValue <= 100 * formationConfig.DropItemRate)
                {
                    var ableToGetItems = true;
                    if (stage.LastStage)
                    {
                        // ableToGetItems = await isLastStageBattleValid(stage);
                        // if (ableToGetItems)
                        // {
                        //     var currentStageOpenTime = GameConfigs.WorldMapConfig.ListLastStageOpenTime.FindAll(sot => sot.StageId == battleData.StageId && sot.Time <= DateTime.UtcNow.Hour).OrderByDescending(sot => sot.Time).FirstOrDefault();
                        //     var keyByDay = CacheKeys.LastStageWinnerKey + $":{DateTime.UtcNow.ToShortDateString()}:{battleData.StageId}";
                        //     var keyByTime = $"{keyByDay}:{currentStageOpenTime.Time}";
                        //     await this.redisDatabase.ListRightPushAsync(keyByTime, master.Id);
                        //     await this.redisDatabase.KeyExpireAsync(keyByTime, DateTime.Today.AddDays(1).AddSeconds(-1));
                        // }
                        // else
                        // {
                        //     logger.LogDebug("You are late");
                        // }
                    }
                    if (ableToGetItems)
                    {
                        foreach (var dropItem in formationConfig.DropItems.Where(d => d.Item.Id != (long)Item.RSS && d.Item.Id != (long)Item.CharacterEXP).ToList())
                        {

                            var item = GameConfigs.ItemConfigs.Find(i => i.Id == dropItem.Item.Id);
                            if (item == null)
                            {
                                continue;
                            }
                            claimItems.Add(new ClaimableItemDto
                            {
                                ItemId = item.Id,
                                Amount = dropItem.Quantity,
                                Entity = item.EntityType,
                                Type = item.TypeId,
                                Name = item.Name

                            });
                            var userItem = await itemMasterRepo.Find(im => im.ItemId == item.Id && im.MainUserId == master.MainUserId).FirstOrDefaultAsync();

                            if (userItem == null)
                            {
                                userItem = new ItemMasterDAO
                                {
                                    ItemId = dropItem.Item.Id,
                                    Name = item.Name,
                                    Quantity = (long)dropItem.Quantity,
                                    MainUserId = master.MainUserId,
                                };
                                await itemMasterRepo.InsertOneAsync(userItem);

                            }
                            else
                            {
                                userItem.Quantity += (long)dropItem.Quantity;
                                await itemMasterRepo.ReplaceOneAsync(m => m.Id == userItem.Id, userItem);
                            }

                        }

                    }
                }
            }
            return claimItems;
        }

        public async Task ProcessInventoryCreateClaimingItemsResponse(CreateClaimableItemsResponseIntegrationEvent evtData)
        {
            if (!evtData.Success) return;
            var dataStr = await redisDatabase.StringGetAsync(evtData.RequestId);
            if (string.IsNullOrEmpty(dataStr)) return;
            var data = JsonSerializer.Deserialize<BattleRequestCreateClaimingItemCacheData>(dataStr);

            // creating mail request
            var title = "default title";
            var content = "default content";
            if (data.ClaimItemSource == BattleClaimItemSource.PVEDropItem)
            {
                title = "Drop Items: Getting rewards";
                content = "Congratulations on getting rewards from the drop items";
            }
            _ = eventBus.PublishAsync(EventNameConst.CreatingMailRequestEvent, new CreatingMailboxRequestIntegrationEvent
            {
                // WalletId = evtData.WalletId,
                MainUserId = evtData.MainUserId,
                Title = title,
                Content = content,
                Type = MailboxType.Mail_Reward_ItemDrop,
                ExpiredTime = DateTime.UtcNow.AddDays(7),
                RewardData = new MailRewardDataEto
                {
                    Items = evtData.Items,
                }
            });
            _ = redisDatabase.KeyDeleteAsync(evtData.RequestId);
        }

        private async Task processBattleCancelled(BattleServerStateChangedIntegrationEvent battleData)
        {
            logger.LogInformation("processBattleCancelled");
            await releaseMasters(battleData.Id);
            // Logic for cancelled battle
            var connectedMasters = battleData.Masters.FindAll(m => m.ConnectionState != ConnectionState.Waiting && m.ConfirmationState == ConfirmationState.Cancelled);
            // await masterService.DecreaseReputation(connectedMasters.Select(m => m.Id).ToArray());
        }
        private async Task processBattleTimeout(BattleServerStateChangedIntegrationEvent battleData)
        {
            logger.LogInformation("processBattleTimeout");
            await releaseMasters(battleData.Id);

            // // TODO - should refactoring here later
            // if (battleData.Type == BattleType.PVP_SOLO)
            // {
            //     await pvpSoloRoomService.ProcessBattleTimeout(battleData);
            //     return;
            // }

            // Logic for timeout battle
            var connectedMasters = battleData.Masters.FindAll(m => m.ConnectionState != ConnectionState.Waiting && m.ConfirmationState != ConfirmationState.Accepted);
            // await masterService.DecreaseReputation(connectedMasters.Select(m => m.Id).ToArray());
        }

        private int GetBonusRssForMaster(MasterDAO master, BattleServerStateChangedIntegrationEvent battleData)
        {
            var gainedRss = 0;
            if (battleData.Type == BattleType.PVE)
            {

                var stageFormation = GameConfigs.WorldMapConfig.StageFormations.FirstOrDefault(sf => sf.StageId == battleData.StageId);
                if (stageFormation != null)
                {
                    var itemRss = stageFormation.DropItems.FirstOrDefault(i => i.Item.Id == (long)Item.RSS);


                    if (itemRss != null)
                    {
                        logger.LogInformation($"RssGainByEnergyBurned {new
                        {
                            MasterLevel = master.Level,
                            MasterId = master.Id,
                            MasterName = master.Name,
                            RssGain = itemRss.Quantity,


                        }.ToAnalysisLog(LoggingContextEnum.RssContext)}");
                        return itemRss.Quantity;
                    }
                }




                var stage = GameConfigs.WorldMapConfig.Stages.Find(s => s.Id == battleData.StageId);

                var total = (int)(stage.MobCount * stage.RssPerMob);

                var rongoses = battleData.Dragons.FindAll(r => r.MasterId == master.Id);
                var rongosGroups = rongoses.GroupBy(r => r.IsOffChain);
                foreach (var group in rongosGroups)
                {
                    gainedRss += total * group.Count() / rongoses.Count;
                }
                logger.LogInformation($"RssGainByEnergyBurned {new
                {
                    MasterLevel = master.Level,
                    MasterId = master.Id,
                    MasterName = master.Name,
                    RssGain = gainedRss,


                }.ToAnalysisLog(LoggingContextEnum.RssContext)}");
            }
            return gainedRss;
        }
        private long GetBonusExpForMaster(MasterDAO master, BattleServerStateChangedIntegrationEvent battleData)
        {
            if (battleData.Type == BattleType.PVE)
            {
                var stage = GameConfigs.WorldMapConfig.Stages.Find(s => s.Id == battleData.StageId);
                var stageFormation = GameConfigs.WorldMapConfig.StageFormations.FirstOrDefault(sf => sf.StageId == battleData.StageId);
                var itemExp = stageFormation.DropItems.FirstOrDefault(i => i.Item.Id == (long)Item.CharacterEXP);
                if (itemExp != null) return itemExp.Quantity;
                return 10;
            }
            return 0;
        }

        // TODO Should refactor this later
        // private async Task emitLevelUpMaster(string walletId, string id, UInt16 level, UInt64 exp)
        // {
        //     var maxExp = GameConfigs.LevelConfig.MaxExpMaster[level - 1];
        //     await serverHub.Clients.Group(walletId).MasterLevelUp(id, level, exp, maxExp);
        //     await eventBus.PublishAsync(EventNameConst.MasterUserLevelUpHistoryEvent, new MasterUserLevelUpHistoryIntegrationEvent
        //     {
        //         WalletId = walletId,
        //         Level = level,
        //         UpdatedAt = DateTime.UtcNow
        //     });
        // }
        // private async Task emitRssBonusMaster(MasterDAO master, ulong bonusRss)
        // {
        //     await masterService.BonusRss(master, (int)bonusRss);
        // }
        // private async Task emitLevelUpDragon(string walletId, DragonDAO dragon, UInt16 level, double exp)
        // {
        //     var maxExp = GameConfigs.LevelConfig.MaxExpDragon[level - 1];
        //     await serverHub.Clients.Group(walletId).DragonLevelUp(dragon.Id, level, exp, maxExp);
        //     await eventBus.PublishAsync(EventNameConst.DragonUpdateLevelEvent, new
        //     {
        //         tokenId = Convert.ToUInt64(dragon.NftId),
        //         level = level,
        //     });
        // }
        // private async Task emitLevelUpSeal(DragonSealDAO seal, UInt16 level, UInt64 exp)
        // {
        //     var maxExp = GameConfigs.LevelConfig.MaxExpDragon[level - 1];
        //     await serverHub.Clients.Group(seal.WalletId).DragonSealLevelUpdated(seal.Id, level, exp, maxExp);
        //     await eventBus.PublishAsync(EventNameConst.SealUpdateLevelEvent, new
        //     {
        //         tokenId = seal.NftId,
        //         level = level,
        //     });
        // }
        public async Task<ActiveBattleCache> GetCachedActiveBattleAsync(string masterId)
        {
            var cachedBattleString = await this.redisDatabase.StringGetAsync($"{CacheKeys.ActiveBattleMasterKey}:{masterId}");
            try
            {
                return JsonSerializer.Deserialize<ActiveBattleCache>(cachedBattleString);
            }
            catch
            {
                logger.LogInformation($"There is no active battle in master {masterId}");
                return null;
            }
        }

        public async Task SaveCacheBattleRequestCookie(string battleId, string cookie)
        {
            TimeSpan expire = TimeSpan.FromSeconds(60);
            await this.redisDatabase.StringSetAsync($"{CacheKeys.BattleRequestCookie}:{battleId}", cookie, expire);
        }
        public async Task<string> GetDeleteCacheBattleRequestCookie(string battleId)
        {
            var cookie = await this.redisDatabase.StringGetDeleteAsync($"{CacheKeys.BattleRequestCookie}:{battleId}");
            return cookie;
        }
        public async Task SaveCacheBattleRequest(string battleId, string battleInfo)
        {
            await this.redisDatabase.StringSetAsync($"{CacheKeys.BattleRequest}:{battleId}", battleInfo);
        }
        public async Task<string> GetDeleteCacheBattleRequest(string battleId)
        {
            var battleInfo = await this.redisDatabase.StringGetDeleteAsync($"{CacheKeys.BattleRequest}:{battleId}");
            return battleInfo;
        }
    }
}