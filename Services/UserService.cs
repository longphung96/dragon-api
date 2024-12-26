using System.Linq.Dynamic.Core;
using AutoMapper;
using DotNetCore.CAP;
using Hangfire;
using JohnKnoop.MongoRepository;
using MongoDB.Driver;
using DragonAPI.Common;
using DragonAPI.Models.DTOs;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Models.DAOs;
using DragonAPI.Repositories;
using StackExchange.Redis;
using Volo.Abp.Linq;
using DragonAPI.Extensions;
using MediatR;
using DragonAPI.Application.Commands;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DragonAPI.Enums;
// using TrueSight.Lite.Common;
using DragonAPI.Models.Entities;
using DragonAPI.Helpers;
using DotNetCore.CAP.Internal;
using DragonAPI.Data;
namespace DragonAPI.Services
{
    public class DragonPermissionConstants
    {
        public const string PermissionsPrefix = "permissions";
        public const string AllPermissions = "permissions:api";
        public const string PermissionBattleSolo = "permissions:api:battle:solo";
        public const string PermissionTreasuryOpenBox = "permissions:api:treasury:openBox";
        public const string PermissionMailboxMarkAsRead = "permissions:api:mailbox:markAsRead";
        public const string PermissionInventoryClaimItem = "permissions:api:inventory:item:claim";
    }
    public class UserService : BaseService<UserService>
    {
        private readonly string[] DefaultMainUserPermissions = new string[] {
            "permissions:api",
        };
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IDatabase redisDatabase;
        private readonly IMongoCollection<SettingsDAO> userSettingsRepo;
        private readonly AsyncQueryableExecuter asyncQueryableExecuter;
        private readonly IMongoCollection<MasterDAO> masterRepo;
        private readonly IMongoCollection<UserBotDAO> userBotRepo;
        private readonly IMongoCollection<DragonDAO> rongosRepo;
        private readonly IMongoCollection<UserDAO> userRepo;
        private readonly IMongoCollection<RankPositionDAO> rankPositionRepo;
        private readonly IMediator mediator;
        private readonly IMongoCollection<MasterWalletDAO> masterWalletsRepo;
        private readonly DragonMongoDbContext mongoDbContext;
        public UserService(
            IMongoClient client,
            IBackgroundJobClient backgroundJobClient,
            ICacheRepository CacheRepository,
            IConnectionMultiplexer redisConn,
            ConfigLoader cfgLoader,
            IMapper mapper,
            ICapPublisher capBus,
            IdentityService identityService,
            AsyncQueryableExecuter asyncQueryableExecuter,
            IMediator mediator,
            DragonMongoDbContext mongoDbContext,
            ILogger<UserService> logger) : base(logger, client, cfgLoader, mapper, capBus, CacheRepository, identityService)
        {
            redisDatabase = redisConn.GetDatabase();
            this.backgroundJobClient = backgroundJobClient;
            this.asyncQueryableExecuter = asyncQueryableExecuter;
            this.masterRepo = mongoDbContext.GetCollection<MasterDAO>("masters");
            this.userSettingsRepo = mongoDbContext.GetCollection<SettingsDAO>("settings");
            this.masterWalletsRepo = mongoDbContext.GetCollection<MasterWalletDAO>("masterWallets");
            this.mediator = mediator;
            this.userBotRepo = mongoDbContext.GetCollection<UserBotDAO>("user_bot");
            this.userRepo = mongoDbContext.GetCollection<UserDAO>("users");
            this.rankPositionRepo = mongoDbContext.GetCollection<RankPositionDAO>("rankPosition");
            this.rongosRepo = mongoDbContext.GetCollection<DragonDAO>("dragons");
        }
        public async Task<string[]> GetPermissions()
        {
            var userId = identityService.CurrentUserId;
            var key = $"{KeyUserGroupKey}:{userId}";

            var user = await userRepo.Find(u => u.UserId == userId).FirstOrDefaultAsync();
            return user?.Permissions == null ? new string[] { } : user.Permissions;
        }

        public async Task<UserDAO> Get(string userId)
        {
            var user = await userRepo.Find(u => u.UserId == userId).FirstOrDefaultAsync();
            return user;
        }
        public async Task<UserDAO> FindOrCreateUser(string userId, string userName, string accessToken)
        {
            var user = await GetUserAsync(userId);
            string walletReal = null;
            if (user == null)
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", accessToken);
                var response = await httpClient.GetStringAsync("https://id-dev.yunr.me/connect/userinfo");
                var nftInfo = (JObject)JsonConvert.DeserializeObject(response);
                if (nftInfo["web3_wallet"] != null)
                {
                    if (!string.IsNullOrEmpty(nftInfo["web3_wallet"].ToString()))
                    {
                        walletReal = nftInfo["web3_wallet"].ToString();
                    }
                }
                user = await InitializeMainUser(userId, userName, walletReal);
            }
            var rankUser = await rankPositionRepo.Find(ur => ur.MainUserId == user.UserId).FirstOrDefaultAsync();
            if (rankUser == null)
            {
                await InitRankBot(user.UserId, 0);
            }
            return user;
        }
        public async Task CreateUserBot(int Tier, long TotalUser)
        {
            for (var i = 0; i <= TotalUser; i++)
            {
                var userId = SnowflakeId.Default().NextId().ToString();
                var wallets = new HashSet<string> { userId };
                var userWallets = new List<MasterWalletDAO>();
                foreach (var wallet in wallets)
                {
                    userWallets.Add(new MasterWalletDAO
                    {
                        MainUserId = userId,
                        WalletId = wallet
                    });
                }
                await masterWalletsRepo.InsertManyAsync(userWallets);
                var user = new UserDAO
                {
                    UserId = userId,
                    UserName = userId,
                    CreatedAt = System.DateTime.UtcNow,
                    IsLock = false,
                    Permissions = null,
                    Wallets = wallets
                };
                await userRepo.InsertOneAsync(user);
                var master = new MasterDAO
                {
                    MainUserId = user.GetMainUserId(),
                    Wallets = wallets,
                    Name = userId,
                    Level = 1,
                    Exp = 0,
                    IsPremium = false,
                    LastClaimRewardAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
                var bot = new UserBotDAO
                {
                    BotUserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await userBotRepo.InsertOneAsync(bot);

                // await masterRepo.InsertAsync(master);
                await initSettings(userId);
                await InitRankBot(userId, Tier);
                await ProcessCreateDragonF2P(master);
            }
        }


        public async Task<UserDAO> GetUserAsync(string userId)
        {
            var user = await userRepo.Find(u => u.UserId == userId).FirstOrDefaultAsync();
            return user;
        }
        public async Task<ApiResultDTO<OffsetPagniationData<UserDTO>>> GetUsersAsync(ListUserFilteringRequestDto request)
        {
            var result = new ApiResultDTO<OffsetPagniationData<UserDTO>>();
            var queryable = request.ApplyFilterTo(userRepo.AsQueryable());
            var count = await asyncQueryableExecuter.LongCountAsync(queryable);
            var users = await asyncQueryableExecuter.ToListAsync(queryable.Page(request.Page, request.Size));
            result.Data = new OffsetPagniationData<UserDTO>(
                mapper.Map<IReadOnlyList<UserDAO>, IReadOnlyList<UserDTO>>(users),
                count, request.Page, request.Size);
            return result;
        }
        public async Task ProcessSyncPremiumMaster(string mainUserId, bool isPremium)
        {
            var user = await userRepo.Find(u => u.UserId == mainUserId).FirstOrDefaultAsync();
            if (user != null)
            {
                user.IsPremium = isPremium;
                await userRepo.ReplaceOneAsync(m => m.Id == user.Id, user);
            }
        }
        public async Task<ApiResultDTO<IReadOnlyList<UserDTO>>> SearchUsersAsync(SearchUserRequestDto request)
        {
            var result = new ApiResultDTO<IReadOnlyList<UserDTO>>();
            var queryable = request.ApplyFilterTo(userRepo.AsQueryable());
            var users = await asyncQueryableExecuter.ToListAsync(queryable.Page(1, 10));
            result.Data = mapper.Map<IReadOnlyList<UserDAO>, IReadOnlyList<UserDTO>>(users);
            return result;
        }
        public async Task ProcessSyncLevelMaster(string mainUserId, long level)
        {
            var user = await userRepo.Find(u => u.UserId == mainUserId).FirstOrDefaultAsync();
            if (user != null)
            {
                user.MainUserLevel = level;
                await userRepo.ReplaceOneAsync(m => m.Id == user.Id, user);
            }
        }
        private async Task<UserDAO> InitializeMainUser(string userId, string userName, string realWalletLinked = null, string[] permissions = null)
        {
            UserDAO user = null;
            try
            {
                logger.LogDebug($"Initialize default for user {userId}, {userName}");

                var wallets = new HashSet<string> { userId };
                if (realWalletLinked != null)
                {
                    wallets.Add(realWalletLinked);
                }
                var userWallets = new List<MasterWalletDAO>();
                foreach (var wallet in wallets)
                {
                    userWallets.Add(new MasterWalletDAO
                    {
                        MainUserId = userId,
                        WalletId = wallet
                    });
                }
                await masterWalletsRepo.InsertManyAsync(userWallets);
                user = new UserDAO
                {
                    UserId = userId,
                    UserName = userName,
                    CreatedAt = System.DateTime.UtcNow,
                    IsLock = false,
                    Permissions = permissions,
                    Wallets = wallets
                };
                await userRepo.InsertOneAsync(user);
                var master = new MasterDAO
                {
                    MainUserId = user.GetMainUserId(),
                    Wallets = wallets,
                    Name = userName,
                    Level = 1,
                    Exp = 0,

                    IsPremium = false,
                    LastClaimRewardAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
                if (realWalletLinked != null) master.LinkWallet = true;
                await masterRepo.InsertOneAsync(master);
                await initSettings(userId);
                await InitDragonF2P(userId);
                await PublishSyncUserEvent(user);
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex.Message);
            }

            return user;
        }
        private async Task initSettings(string userId)
        {
            var setting = new SettingsDAO
            {
                UserId = userId,
                BotAnimationEnabled = true,
                Sound = 7,
                Music = 7,
            };
            await userSettingsRepo.InsertOneAsync(setting);
            logger.LogDebug("Initialize user settings, done");
        }
        public async Task InitDragonF2P(string userId)
        {
            logger.LogDebug("buildNotifyCommandForBattleCreatedEvent");

            var command = new DragonF2PNotifyUserCommand
            {
                EventName = "CreateDragon",
                UserId = userId,

            };
            await mediator.Send(command);

        }

        public async Task InitRankBot(string userId, int Tier)
        {
            logger.LogDebug("buildNotifyCommandForBattleCreatedEvent");

            var command = new FillUserBotNotifyUserCommand
            {
                Tier = Tier,
                UserId = userId,

            };
            await mediator.Send(command);

        }
        public async Task ProcessCreateDragonF2P(MasterDAO User)
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
                    WalletId = User.MainUserId,
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
                                Class = (long) classesBodyPart.ElementAt(random.Next(0, classesBodyPart.Count)),
                                Unlocked = true,
                                SkillUnlocked = true,
                                MutantType = MutantType.Normal,
                                Genes = new List<Gene>()
                            },
                              new Bodypart
                            {
                                Type = BodypartType.Eye,
                                Class = (long) classesBodyPart.ElementAt(random.Next(0, classesBodyPart.Count)),
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



            var formationPvE = new BattleFormationWithType
            {
                DragonIds = rongoses,
                Type = BattleType.PVE.ToString()
            };
            User.Formations.Add((BattleFormationWithType)formationPvE);
            var formationPvP = new BattleFormationWithType
            {
                DragonIds = rongoses,
                Type = BattleType.PVP.ToString()
            };
            User.Formations.Add((BattleFormationWithType)formationPvP);
            User.UpdatedAt = DateTime.UtcNow;
            await masterRepo.InsertOneAsync(User);

        }

        public async Task ProcessFillUserRank(string userId, int tier)
        {
            var rankBot = await rankPositionRepo.Find(r => r.MainUserId == userId).FirstOrDefaultAsync();

            if (rankBot == null)
            {
                var lstrankBot = await rankPositionRepo.Find(r => r.Tier == TierGroup.Bronze).SortByDescending(r => r.RankPosition).ToListAsync();
                var currentSeason = gameCfgLoader.GameConfigs.SeasonConfigs.Where(s => s.StartingTime < DateTime.UtcNow && s.EndingTime > DateTime.UtcNow).FirstOrDefault();
                rankBot = new RankPositionDAO
                {
                    MainUserId = userId,
                    Tier = TierGroup.Bronze,
                    SeasonId = currentSeason.Id,
                    RankPosition = lstrankBot.Count > 0 ? lstrankBot.FirstOrDefault().RankPosition + 1 : 1

                };
                await rankPositionRepo.InsertOneAsync(rankBot);
            }

        }
        //    public async Task ProcessFillBotRank(string userId, int Tier)
        //     {
        //         var lstrankBot = await rankPositionRepo.Find(r => (int)r.Tier == Tier).SortByDescending(r => r.RankPosition).ToListAsync();
        //         var rankBot = lstrankBot.FirstOrDefault(r => r.MainUserId == userId);
        //         if (rankBot == null)
        //         {
        //             var currentSeason = gameCfgLoader.GameConfigs.SeasonConfigs.Where(s => s.StartingTime < DateTime.UtcNow && s.EndingTime > DateTime.UtcNow).FirstOrDefault();
        //             rankBot = new RankPositionDAO
        //             {
        //                 MainUserId = userId,
        //                 Tier = (Enums.TierGroup)Tier,
        //                 SeasonId = currentSeason.Id,
        //                 RankPosition = lstrankBot.Count > 0 ? lstrankBot.FirstOrDefault().RankPosition + 1 : 1

        //             };
        //             await rankPositionRepo.InsertAsync(rankBot);
        //         }

        //     }
        public async Task PublishSyncUserEvent(UserDAO user)
        {
            await eventBus.PublishAsync(EventNameConst.DragonAPISyncUser, new SyncUserEto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                MainUserId = user.MainUserId,
                Wallets = user.Wallets,
                DisplayName = user.UserName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Permissions = user.Permissions
            });
        }
        public async Task<ApiResultDTO<MeDTO>> GetMeHandler()
        {
            var result = new ApiResultDTO<MeDTO>();
            var userId = identityService.CurrentUserId;
            var user = await userRepo.Find(u => u.UserId == userId).FirstOrDefaultAsync();
            var master = await GetMasterAsync(user.UserId);
            var meData = new MeDTO();
            meData.UserInfo = mapper.Map<UserDAO, UserDTO>(user);
            if (master != null)
            {
                meData.MasterInfo = mapper.Map<MasterDAO, MasterDTO>(master);
                //meData.ActiveBattleInfo = await battleService.GetCachedActiveBattleAsync(master.Id);
            }
            result.Data = meData;
            return result;
        }
        public async Task<MasterDAO> GetMasterAsync(string userId)
        {
            var master = await masterRepo.Find(u => u.MainUserId == userId).FirstOrDefaultAsync();
            return master;
        }
        public async Task<ApiResultDTO<MeDTO>> LinkWallet(string access_token)
        {
            var result = new ApiResultDTO<MeDTO>();
            var userId = identityService.CurrentUserId;
            var user = await userRepo.Find(u => u.UserId == userId).FirstOrDefaultAsync();
            var master = await GetMasterAsync(user.UserId);
            if (!master.LinkWallet)
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", access_token);
                var response = await httpClient.GetStringAsync("https://id-dev.yunr.me/connect/userinfo");
                var nftInfo = (JObject)JsonConvert.DeserializeObject(response);
                if (string.IsNullOrEmpty(nftInfo["web3_wallet"].ToString()))
                {
                    master.Wallets.Add(nftInfo["web3_wallet"].ToString());
                    master.LinkWallet = true;
                    await masterRepo.ReplaceOneAsync(u => u.Id == master.Id, master);
                }
            }


            var meData = new MeDTO();
            meData.UserInfo = mapper.Map<UserDAO, UserDTO>(user);
            if (master != null)
            {
                meData.MasterInfo = mapper.Map<MasterDAO, MasterDTO>(master);
                //meData.ActiveBattleInfo = await battleService.GetCachedActiveBattleAsync(master.Id);
            }
            return result;
        }
        // public async Task<bool> UpdateRssMaster(MasterDAO master)
        // {
        //     await masterRepo.UpdateOneAsync(d => d.Id == master.Id, updator => updator.Set(m => m.Rss, master.Rss));
        //     return true;
        // }
        public async Task<bool> HandleUserDisconnection(string connectionId)
        {

            return true;
        }
        public async Task<ApiResultDTO<object>> HandleUserConnection(string connectionId)
        {
            var result = new ApiResultDTO<object>();

            return result;
        }
    }
}