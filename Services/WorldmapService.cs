using System;
using System.Linq.Dynamic.Core;
using AutoMapper;
using DotNetCore.CAP;
using JohnKnoop.MongoRepository;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using DragonAPI.Common;
using DragonAPI.Configurations;
using DragonAPI.Enums;
using DragonAPI.Models.DAOs;
using DragonAPI.Models.DTOs;
using DragonAPI.Repositories;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using DragonAPI.Data;
namespace DragonAPI.Services
{
    public class WorldmapService : BaseService<WorldmapService>
    {
        private readonly DragonMongoDbContext mongoDbContext;
        private readonly IDatabase redisDatabase;
        private readonly IMongoCollection<MasterDAO> masterRepo;
        private readonly IMongoCollection<MasterStageTrackingDAO> masterStageTrackingRepo;
        public WorldmapService(
            IConnectionMultiplexer redisConn,
            ICacheRepository CacheRepository,
            ILogger<WorldmapService> logger,
            IMongoClient client,
            ConfigLoader gameCfgLoader,
            IMapper mapper,
            ConfigLoader cfgLoader,
            ICapPublisher capBus,
            DragonMongoDbContext mongoDbContext,
            IOptions<RedisSettings> redisSettingsOption,
            IdentityService identityService)
            : base(logger, client, cfgLoader, mapper, capBus, CacheRepository, identityService)
        {
            this.redisDatabase = redisConn.GetDatabase().WithKeyPrefix(redisSettingsOption.Value.Prefix);
            this.masterRepo = mongoDbContext.GetCollection<MasterDAO>("masters");
            this.masterStageTrackingRepo = mongoDbContext.GetCollection<MasterStageTrackingDAO>("master_stage_tracking");
        }

        public async Task<ApiResultDTO<IEnumerable<MapDTO>>> GetMaps()
        {
            var result = new ApiResultDTO<IEnumerable<MapDTO>>();
            var master = await masterRepo.Find(m => m.MainUserId == identityService.CurrentUserId).FirstOrDefaultAsync();
            var MapDatas = new List<MapStage>();
            foreach (var cm in GameConfigs.WorldMapConfig.Maps)
            {
                bool unlock = false;
                if (master == null)
                {
                    unlock = cm.Index == 1;
                }
                else
                {
                    if (cm.Index == 1)
                    {
                        unlock = true;

                    }
                    else
                    {
                        var configedStages = GameConfigs.WorldMapConfig.Stages.FirstOrDefault(ms => ms.ParentId == (Convert.ToInt32(cm.Id) - 1).ToString() && ms.LastStage);
                        var lastStageTracking = await masterStageTrackingRepo.Find(m => m.MasterId == master.Id && m.StageId == configedStages.Id).FirstOrDefaultAsync();
                        if (lastStageTracking != null) unlock = true;
                    }
                }
                var map = new MapStage
                {
                    Id = cm.Id,
                    Index = cm.Index,
                    Name = cm.Name,
                    Unlocked = unlock,
                    RequiredLevel = cm.RequiredLevel,
                };
                MapDatas.Add(map);
            }
            result.Data = mapper.Map<IEnumerable<MapStage>, IEnumerable<MapDTO>>(MapDatas);
            return result;
        }

        public async Task<ApiResultDTO<IEnumerable<StageDTO>>> GetStages(string mapId)
        {
            var result = new ApiResultDTO<IEnumerable<StageDTO>>();
            var master = await masterRepo.Find(m => m.MainUserId == identityService.CurrentUserId).FirstOrDefaultAsync();
            var configedStages = GameConfigs.WorldMapConfig.Stages.FindAll(ms => ms.ParentId == mapId).OrderBy(ms => ms.Index);
            var map = GameConfigs.WorldMapConfig.Maps.FirstOrDefault(c => c.Id == mapId && c.Index == 1);

            var stages = new List<MapStage>();
            var lastIndexDone = 0;
            foreach (var cs in configedStages)
            {
                var stageFormation = GameConfigs.WorldMapConfig.StageFormations.FindAll(ms => ms.StageId == cs.Id).FirstOrDefault();
                bool unlock = false;
                if (master == null)
                {
                    if (map != null && cs.Index == 1)
                    {
                        unlock = true;
                    }
                }
                else
                {
                    var lastStageDone = await masterStageTrackingRepo.Find(m => m.MasterId == master.Id && m.StageId == cs.Id).FirstOrDefaultAsync();
                    if (lastStageDone != null)
                    {
                        lastIndexDone = cs.Index;
                    }
                    if (cs.Index == 1)
                    {
                        unlock = true;
                    }
                    else
                    {

                        unlock = lastIndexDone >= cs.Index - 1;
                    }

                }
                var stage = new MapStage
                {
                    Id = cs.Id,
                    Index = cs.Index,
                    Parent = cs.ParentId,
                    Energy = cs.Energy,
                    Name = cs.Name,
                    RequiredLevel = cs.RequiredLevel,
                    MobCount = cs.MobCount,
                    Unlocked = unlock,
                    Rss = (ushort)(cs.MobCount * cs.RssPerMob),
                    Exp = (int)(cs.MobCount * cs.ExpPerMob),
                    BossStage = cs.LastStage,
                    MobIds = stageFormation != null ? stageFormation.MobIds.ToList() : new List<string>(),
                    Mobs = stageFormation.Mobs != null ? stageFormation.Mobs.OrderBy(m => m.Id).ToList() : new List<MobInStageFormation>(),

                };
                stages.Add(stage);
            }
            result.Data = mapper.Map<IEnumerable<MapStage>, IEnumerable<StageDTO>>(stages);
            return result;
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
                    var keyByTime = ($"{keyByDay}:{s.Time}");
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
    }
}