using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCore.CAP;
using AutoMapper;
using JohnKnoop.MongoRepository;
using System;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Linq;
using DragonAPI.Configurations;
using DragonAPI.Enums;
using DragonAPI.Models.DAOs;
using DragonAPI.Repositories;
using DragonAPI.Data;
namespace DragonAPI.Services
{
    public class BaseService<T>
    {
        protected readonly ILogger<T> logger;
        protected readonly ConfigLoader gameCfgLoader;
        protected readonly IMongoClient client;

        protected readonly IdentityService identityService;
        protected readonly ICapPublisher eventBus;
        protected readonly IMapper mapper;
        protected readonly ICacheRepository cacheRepository;
        public const string KeyUserGroupKey = CacheKeys.CachedData + ":users";
        public const string KeyMasterGroupKey = CacheKeys.CachedData + ":masters";
        public BaseService(
            ILogger<T> logger,
            IMongoClient client,
            ConfigLoader gameCfgLoader,
            IMapper mapper,
            ICapPublisher capPublisher,
            ICacheRepository CacheRepository,
            IdentityService identityService)
        {
            this.logger = logger;
            this.client = client;
            this.mapper = mapper;
            eventBus = capPublisher;
            this.identityService = identityService;
            this.cacheRepository = CacheRepository;
            this.gameCfgLoader = gameCfgLoader;
        }
        protected GameConfigs GameConfigs => gameCfgLoader.GameConfigs;
        protected OffChainConfigs OffChainConfigs => gameCfgLoader.OffChainConfigs;
        // protected async Task<MasterDAO> GetCurrentMaster()
        // {
        //     var user = await GetCurrentUser();
        //     if (string.IsNullOrEmpty(user.MainUserId)) return null;
        //     return await GetMaster(user.MainUserId);
        // }
        // protected async Task<UserDAO> GetCurrentUser()
        // {
        //     return await GetUser(identityService.CurrentUserId);
        // }
        // protected string GetSeasonId()
        // {
        //     DateTime now = DateTime.UtcNow;
        //     SeasonConfig seaSon = GameConfigs.SeasonConfigs.FirstOrDefault(c => c.StartingTime <= now && now <= c.EndingTime);
        //     if (seaSon != null)
        //     {
        //         return seaSon.Id;
        //     }
        //     return "season_default";
        // }

        // protected async Task<UserDAO> GetUser(string userId)
        // {
        //     var key = $"{KeyUserGroupKey}:{userId}";
        //     var user = await cacheRepository.GetFromCache<UserDAO>(key);
        //     if (user == null)
        //     {
        //         user = await userRepo.Find(u => u.UserId == userId).FirstOrDefaultAsync();
        //         if (user != null)
        //         {
        //             await cacheRepository.SetToCache(key, user, TimeSpan.FromSeconds(120));
        //         }
        //     }
        //     return user;
        // }

        // // protected async Task<MasterDAO> GetMaster(string MasterUserId)
        // // {
        // //     var key = $"{KeyMasterGroupKey}:{MasterUserId}";
        // //     var master = await CacheRepository.GetFromCache<MasterDAO>(key);
        // //     if (master == null)
        // //     {
        // //         master = await masterRepo.FindOneAsync(u => u.Id == MasterUserId);
        // //         if (master != null)
        // //         {
        // //             await CacheRepository.SetToCache(key, master, TimeSpan.FromSeconds(120));
        // //         }
        // //     }
        // //     return master;
        // // }
    }
}
