using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using JohnKnoop.MongoRepository;
using DragonAPI.Data;
using MongoDB.Driver;
using DragonAPI.Common;
using DragonAPI.Enums;
using DragonAPI.Extensions;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Models.DAOs;
using DragonAPI.Models.DTOs;
using DragonAPI.Repositories;
using Volo.Abp.Linq;

namespace DragonAPI.Services
{
    public class GameService : BaseService<GameService>
    {
        private readonly IMongoCollection<SettingsDAO> userSettingsRepo;
        private readonly IMongoCollection<DragonDAO> dragonRepo;
        private readonly IMongoCollection<MasterDAO> masterRepo;
        private readonly IMongoCollection<RankPositionDAO> rankPositionRepo;
        private readonly IMongoCollection<PvPHistoryDAO> pvpHistoryRepo;
        private readonly BattleService battleService;
        private readonly AsyncQueryableExecuter asyncQueryableExecuter;
        private readonly DragonService rongosService;
        private readonly UserService userService;
        private readonly MasterService masterService;
        private readonly ConfigLoader cfgLoader;
        private readonly DragonMongoDbContext mongoDbContext;
        // private readonly RankingServiceSettings rankingServiceConfigData;
        public GameService(
            //IOptions<RankingServiceSettings> _rankingServiceConfigData,
            IMongoClient client,
            ConfigLoader cfgLoader,
            ICacheRepository cacheRepository,
            DragonMongoDbContext mongoDbContext,
            BattleService battleService,
            AsyncQueryableExecuter asyncQueryableExecuter,
            DragonService rongosService,
            MasterService masterService,
            UserService userService,
            IMapper mapper,
            ICapPublisher capBus,
            IdentityService identityService,
            ILogger<GameService> logger) : base(logger, client, cfgLoader, mapper, capBus, cacheRepository, identityService)
        {
            this.userSettingsRepo = mongoDbContext.GetCollection<SettingsDAO>("settings");
            this.dragonRepo = mongoDbContext.GetCollection<DragonDAO>("dragon");
            this.masterRepo = mongoDbContext.GetCollection<MasterDAO>("masters");
            this.rankPositionRepo = mongoDbContext.GetCollection<RankPositionDAO>("rankPosition");
            this.pvpHistoryRepo = mongoDbContext.GetCollection<PvPHistoryDAO>("pvp_history");
            this.cfgLoader = cfgLoader;
            this.asyncQueryableExecuter = asyncQueryableExecuter;
            this.battleService = battleService;
            this.masterService = masterService;
            this.rongosService = rongosService;
            this.userService = userService;
        }
        private async Task<MasterDAO> GetMasterByIdAsync(string id)
        {
            return await masterRepo.Find(master => master.Id == id).FirstOrDefaultAsync();
        }
        private async Task<List<MasterDAO>> GetListMasterByIdsAsync(string[] ids)
        {
            return await masterRepo.Find(m => ids.Contains(m.Id)).ToListAsync();
        }
        public async Task<MasterDTO> GetMasterInfoAsync(string masterId)
        {
            return mapper.Map<MasterDAO, MasterDTO>(await GetMasterByIdAsync(masterId));
        }

        public async Task<List<MasterDTO>> GetListMasterInfoAsync(string[] masterIds)
        {
            return mapper.Map<List<MasterDAO>, List<MasterDTO>>(await GetListMasterByIdsAsync(masterIds));
        }

        public async Task<SettingsDTO> GetMySettings()
        {
            var master = await GetMasterByIdAsync(identityService.CurrentUserId);
            return mapper.Map<SettingsDAO, SettingsDTO>(await userSettingsRepo.Find(r => r.UserId == master.Id).FirstOrDefaultAsync());
        }
        public async Task<SettingsDTO> ChangeMySettings(ChangeSettingRequest param)
        {
            var master = await GetMasterByIdAsync(identityService.CurrentUserId);
            var setting = await userSettingsRepo.Find(r => r.UserId == master.Id).FirstOrDefaultAsync();
            setting.Music = param.Music;
            setting.Sound = param.Sound;
            setting.BotAnimationEnabled = param.BotAnimationEnabled;
            await userSettingsRepo.ReplaceOneAsync(s => s.Id == setting.Id, setting);
            return mapper.Map<SettingsDAO, SettingsDTO>(await userSettingsRepo.Find(r => r.UserId == master.Id).FirstOrDefaultAsync());
        }
        public async Task<ApiResultDTO<List<PvPHistoryDTO>>> GetHistoryBattle(PvPHistoryRequest param)
        {
            var result = new ApiResultDTO<List<PvPHistoryDTO>>();
            result.Data = new List<PvPHistoryDTO>();
            if (string.IsNullOrEmpty(param.MainUserId)) param.MainUserId = identityService.CurrentUserId;
            var master = await GetMasterByIdAsync(param.MainUserId);
            if (master != null)
            {
                var listHistory = await pvpHistoryRepo.Find(r => r.MainUserId == master.Id && r.CreatedAt >= DateTime.UtcNow.AddHours(-2)).ToListAsync();
                if (param.BattleStatus != null)
                {
                    listHistory = listHistory.Where(h => h.BattleStatus == param.BattleStatus).ToList();
                }
                result.Data = mapper.Map<List<PvPHistoryDAO>, List<PvPHistoryDTO>>(listHistory);
            }

            return result;
        }
        public async Task<ApiResultDTO<RankPositionResponse>> GetTopRank()
        {
            var result = new ApiResultDTO<RankPositionResponse>();
            result.Data = new RankPositionResponse();
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
            var rankStats = await rankPositionRepo.Find(r => r.Tier == myRank.Tier).SortBy(r => r.RankPosition).Limit(100).ToListAsync();
            result.Data.MyRank = mapper.Map<RankPositionDAO, RankPositionDTO>(myRank);
            result.Data.RankPlayers = mapper.Map<List<RankPositionDAO>, List<RankPositionDTO>>(rankStats);
            return result;
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


            return result;
        }
        public async Task<ApiResultDTO<string[]>> GetBattleTeam(BattleType battleTeamType)
        {
            var result = new ApiResultDTO<string[]>();
            var master = await userService.GetMasterAsync(identityService.CurrentUserId);
            var formation = master.GetFormation(battleTeamType.ToString());
            if (formation == null)
            {
                result.AddError(ErrorCodeEnum.InvalidRequest, "There is no formation for this kind of battle");
                return result;
            }
            logger.LogDebug("formation {0}", JsonSerializer.Serialize(formation));
            result.Data = formation.DragonIds.ToArray();
            return result;
        }
        public async Task<ApiResultDTO<SetupTeamResponse>> SetupTeam(SetupTeamRequest request)
        {
            var result = new ApiResultDTO<SetupTeamResponse>();
            var isChange = false;
            var FormationOld = new List<string>();
            if (request.DragonIds.Count <= 0)
            {
                result.AddError(ErrorCodeEnum.PVETeamNotSetup);
            }
            else
            {
                var master = await userService.GetMasterAsync(identityService.CurrentUserId);
                if (master == null)
                {
                    result.AddError(ErrorCodeEnum.UnexpectedErr);
                    return result;

                }
                var requestPayload = new BattleSetupDragonFormationAwaitValidationResponse
                {
                    MainUserId = master.MainUserId,
                    MainUserWallets = master.Wallets.ToList(),
                    DragonIds = request.DragonIds.FindAll(id => id != ""),
                };

                var data = await rongosService.ProcessBattleSetupDragonFormationValidation(requestPayload);
                if (data.Success)
                {

                    var formation = master.GetFormation(request.TeamType.ToString());
                    if (formation == null)
                    {
                        formation = new BattleFormationWithType
                        {
                            Type = request.TeamType.ToString()
                        };
                        master.Formations.Add((BattleFormationWithType)formation);
                    }

                    if (formation.DragonIds.FindAll(id => id != "").Count() == request.DragonIds.FindAll(id => id != "").Count())
                    {
                        foreach (var id in formation.DragonIds.FindAll(id => id != ""))
                        {
                            if (!request.DragonIds.FindAll(id => id != "").Contains(id))
                            {
                                isChange = true;
                                FormationOld = formation.DragonIds.FindAll(id => id != "");
                            }
                        }
                    }
                    formation.DragonIds = request.DragonIds;
                    master.UpdatedAt = DateTime.UtcNow;
                    await masterRepo.ReplaceOneAsync(m => m.Id == master.Id, master);

                }

                result.Data = new SetupTeamResponse
                {
                    Success = data.Success,
                    MainUserId = data.MainUserId
                };
                if (request.TeamType == BattleType.PVE)
                {
                    masterService.SnapshotFarmPoolJob(master.MainUserId);
                }
                // await masterService.SnapShotAfk(master.MainUserId, ActionNameConst.SetupTeam, FormationOld, null);
            }
            return result;
        }

        ///////////////
        /////////////// Battle
        /////////////// PVE
        public async Task<ApiResultDTO<bool?>> EnterPVEBattle(string stageId)
        {
            return await battleService.EnterPVEBattle(stageId);
        }
        public async Task<ApiResultDTO<bool?>> EnterPVPBattle(string EnemyPlayerId)
        {
            return await battleService.EnterPVPBattle(EnemyPlayerId);
        }
        public async Task<ApiResultDTO<ArenaRankResponse>> GetArena()
        {
            return await battleService.GetArenaRank();
        }
        public async Task<ApiResultDTO<MyArenaRank>> GetUserArena()
        {
            return await battleService.GetUserArenaRank();
        }
        public async Task TestConnectionHub()
        {
            // await battleService.TestConnectionHub();

        }
        public async Task<ApiResultDTO<IEnumerable<LastStageTimeOpenInfoDTO>>> GetOpenTimeLastStage(string stageId)
        {
            return await battleService.GetOpenTimeLastStage(stageId);
        }
        // public async Task ProcessEndSeason(string stageId)
        // {
        //     var currentSeason = gameCfgLoader.GameConfigs.SeasonConfigs.Where(s => s.StartingTime < DateTime.UtcNow && s.EndingTime > DateTime.UtcNow).FirstOrDefault();
        //     BackgroundJob.Schedule(() => EndSeason(currentSeason.Id), currentSeason.EndingTime);
        // }
        // public async Task EndSeason(string SeasonId)
        // {
        //     var endSeason = gameCfgLoader.GameConfigs.SeasonConfigs.Where(s => s.Id == SeasonId).FirstOrDefault();
        //     if (endSeason.EndingTime < DateTime.UtcNow)
        //     {
        //         get
        //     }
        // }
    }
}