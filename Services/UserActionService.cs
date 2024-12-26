using System;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCore.CAP;
using JohnKnoop.MongoRepository;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using DragonAPI.Common;
using DragonAPI.Models.Cache;
using DragonAPI.Models.DAOs;
using DragonAPI.Models.DTOs;
using DragonAPI.Repositories;
using DragonAPI.Data;
namespace DragonAPI.Services
{

    public class UserActionService : BaseService<UserActionService>
    {
        private readonly DragonMongoDbContext mongoDbContext;
        private readonly IMongoCollection<UserDAO> userRepo;
        private const string Prefix = "userActionRequest:";
        public UserActionService(ILogger<UserActionService> logger, IMongoClient client, DragonMongoDbContext mongoDbContext, ConfigLoader gameCfgLoader, IMapper mapper, ICapPublisher capPublisher, ICacheRepository CacheRepository, IdentityService identityService) : base(logger, client, gameCfgLoader, mapper, capPublisher, CacheRepository, identityService)
        {
            this.userRepo = mongoDbContext.GetCollection<UserDAO>("users");
        }

        public async Task<ApiResultDTO<UserActionDTO>> GetAction(string requestId)
        {
            var result = new ApiResultDTO<UserActionDTO>();
            var user = await userRepo.Find(u => u.UserId == identityService.CurrentUserId).FirstOrDefaultAsync();
            if (user == null) throw new System.Exception("Invalid user");
            try
            {
                var cacheObject = await cacheRepository.GetFromCache<UserActionCacheItem>($"{Prefix}{requestId}");
                if (cacheObject.User.GetMainUserId() != user.GetMainUserId())
                {
                    throw new System.Exception("Permission denied");
                }
                result.Data = mapper.Map<UserActionDTO>(cacheObject);
            }
            catch (System.Exception ex)
            {
                logger.LogException(ex);
                throw;
            }
            return result;
        }

        public async Task<string> AddPendingUserAction(UserDAO user, ActionTypeEnum actionType, long ttl = 30)
        {
            var reqId = Guid.NewGuid().ToString();
            var cacheItem = new UserActionCacheItem
            {
                RequestId = reqId,
                User = user,
                ActionType = actionType,
                Action = actionType.ToString(),
                State = ActionState.Pending
            };
            await cacheRepository.SetToCache($"{Prefix}{reqId}", cacheItem, TimeSpan.FromMinutes(ttl));
            return reqId;
        }

        public async Task<bool> UpdateUserActionState(string reqId, ActionState state, object payload = null, long ttl = 30)
        {
            try
            {
                var cacheItem = await cacheRepository.GetFromCache<UserActionCacheItem>($"{Prefix}{reqId}");
                cacheItem.State = state;
                cacheItem.Payload = payload;
                await cacheRepository.SetToCache($"{Prefix}{reqId}", cacheItem, TimeSpan.FromMinutes(ttl));
                return true;
            }
            catch (System.Exception ex)
            {
                logger.LogException(ex);
                return false;
            }
        }
    }
}