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
using DragonAPI.IntegrationEvent.Events;

namespace DragonAPI.Services
{
    public interface IMarketPlaceService
    {
        Task<ApiResultDTO<bool>> BuyPackage(BuyPackageRequest param);
        Task<ApiResultDTO<bool>> BuyItem(BuyItemRequest param);
        Task<ApiResultDTO<FaucetResultDTO>> FaucetRss();
        Task ProcessPaymentItemEvent(PaymentETO param);
    }

    public class MarketPlaceService : BaseService<MarketPlaceService>, IMarketPlaceService
    {
        private readonly DragonMongoDbContext mongoDbContext;
        private IMasterRepository _masterRepository;
        private IItemMasterRepository _itemMasterRepository;
        private readonly IDatabase redisDatabase;
        private readonly IMongoCollection<MasterDAO> masterRepo;
        private readonly IMongoCollection<ItemMasterDAO> itemMasterRepo;
        private readonly IMongoCollection<MasterStageTrackingDAO> masterStageTrackingRepo;
        public MarketPlaceService(
            IConnectionMultiplexer redisConn,
            ICacheRepository CacheRepository,
            ILogger<MarketPlaceService> logger,
            IMongoClient client,
            ConfigLoader gameCfgLoader,
            IMapper mapper,
            DragonMongoDbContext mongoDbContext,
            ConfigLoader cfgLoader,
            ICapPublisher capBus,
            IOptions<RedisSettings> redisSettingsOption,
            IDatabase redisDatabase,
            IMasterRepository _masterRepository,
            IItemMasterRepository _itemMasterRepository,
            IdentityService identityService)
            : base(logger, client, cfgLoader, mapper, capBus, CacheRepository, identityService)
        {
            this.redisDatabase = redisDatabase;
            this.masterRepo = mongoDbContext.GetCollection<MasterDAO>("masters");
            this.masterStageTrackingRepo = mongoDbContext.GetCollection<MasterStageTrackingDAO>("master_stage_tracking");
            this.itemMasterRepo = mongoDbContext.GetCollection<ItemMasterDAO>("item_master");
            this._masterRepository = _masterRepository;
            this._itemMasterRepository = _itemMasterRepository;
        }


        public const long VIP_DURATION_SECS = 60 * 60 * 4; // 4 hour
        public async Task<ApiResultDTO<bool>> BuyItem(BuyItemRequest param)
        {
            var result = new ApiResultDTO<bool>();

            using (var session = this.client.StartSession())
            {
                session.StartTransaction();
                try
                {
                    var master = await masterRepo.Find(m => m.MainUserId == identityService.CurrentUserId).FirstOrDefaultAsync();
                    var ChangeRateConfig = gameCfgLoader.GameConfigs.ChangeRateItem.Where(cr => cr.ExchangeItem == param.ItemExchangeId && cr.ReceivedItem == param.ItemId).FirstOrDefault();
                    if (ChangeRateConfig == null)
                    {
                        throw new BusinessException(ErrorCodeEnum.UnexpectedErr, $"Rate does not exist");
                    }
                    var exchangeRequired = param.TotalItem / ChangeRateConfig.Rate;
                    var userItemExchange = await itemMasterRepo.Find(im => im.MainUserId == master.MainUserId && im.ItemId == param.ItemExchangeId).FirstOrDefaultAsync();
                    if (userItemExchange == null)
                    {
                        throw new BusinessException(ErrorCodeEnum.UnexpectedErr, $"{((Item)param.ItemExchangeId).ToString()} does not enough");
                    }
                    else
                    {
                        var validItem = await _itemMasterRepository.TakeItem(session, userItemExchange, exchangeRequired, $"Buy item {((Item)param.ItemId).ToString()}", logger);
                        if (validItem == false)
                        {
                            throw new BusinessException(ErrorCodeEnum.UnexpectedErr, $"{((Item)param.ItemExchangeId).ToString()} does not enough");
                        }
                    }
                    var userItem = await itemMasterRepo.Find(im => im.MainUserId == master.MainUserId && im.ItemId == param.ItemId).FirstOrDefaultAsync();
                    var itemData = GameConfigs.ItemConfigs.FirstOrDefault(i => i.Id == param.ItemId);
                    if (userItem == null)
                    {
                        userItem = new ItemMasterDAO
                        {
                            MainUserId = master.MainUserId,
                            WalletId = master.MainUserId,
                            ItemId = param.ItemId,
                            Name = itemData != null ? itemData.Name : "",
                            Quantity = (long)param.TotalItem,

                        };
                        await _itemMasterRepository.Add(userItem, session);
                    }
                    else
                    {

                        userItem.Quantity += (long)param.TotalItem;
                        await _itemMasterRepository.Update(userItem, session);
                    }
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
        public async Task<ApiResultDTO<bool>> BuyPackage(BuyPackageRequest param)
        {
            var result = new ApiResultDTO<bool>();
            result.Data = false;


            using (var session = this.client.StartSession())
            {
                session.StartTransaction();
                try
                {
                    var master = await masterRepo.Find(m => m.MainUserId == identityService.CurrentUserId).FirstOrDefaultAsync();
                    var packageConfig = gameCfgLoader.GameConfigs.ShopPackageConfig.FirstOrDefault(sp => sp.Id == param.PackageId);
                    if (packageConfig == null)
                    {
                        throw new BusinessException(ErrorCodeEnum.EntityNotExisted, "package not define");
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
                    var userItem = await itemMasterRepo.Find(im => im.MainUserId == master.MainUserId && im.ItemId == packageConfig.ItemId).FirstOrDefaultAsync();
                    var itemData = GameConfigs.ItemConfigs.FirstOrDefault(i => i.Id == packageConfig.ItemId);
                    if (userItem == null)
                    {
                        userItem = new ItemMasterDAO
                        {
                            MainUserId = master.MainUserId,
                            WalletId = master.MainUserId,
                            ItemId = packageConfig.ItemId,
                            Name = itemData != null ? itemData.Name : "",
                            Quantity = (long)packageConfig.Quantity,

                        };
                        await _itemMasterRepository.Add(userItem, session);
                    }
                    else
                    {

                        userItem.Quantity += (long)packageConfig.Quantity;
                        await _itemMasterRepository.Update(userItem, session);
                    }
                    await session.CommitTransactionAsync();
                    result.Data = true;
                }
                catch (System.Exception ex)
                {
                    await session.AbortTransactionAsync();
                    logger.LogException(ex);
                    throw;

                }

            }



            return result;
        }
        public async Task ProcessPaymentItemEvent(PaymentETO param)
        {
            logger.LogInformation("=== ProcessPaymentItemEvent");
            using (var session = this.client.StartSession())
            {
                session.StartTransaction();
                try
                {
                    var master = await masterRepo.Find(m => m.Wallets.Any(w => w == param.From)).FirstOrDefaultAsync();
                    if (master == null)
                    {
                        throw new BusinessException(ErrorCodeEnum.EntityNotExisted, "Master not exist");
                    }
                    var item = GameConfigs.ItemConfigs.FirstOrDefault(i => i.Code.ToLower() == param.ItemKeys.ToLower());

                    if (item == null) throw new BusinessException(ErrorCodeEnum.EntityNotExisted, "Item not exist");
                    if (item.EntityType.ToLower() == "vip")
                    {
                        var ChangeRateConfig = gameCfgLoader.GameConfigs.ChangeRateItem.Where(cr => cr.ExchangeItem == item.Id && cr.ReceivedItem == (long)Item.Vip).FirstOrDefault();
                        master.UpdatedAt = DateTime.UtcNow;
                        master.VipLevel = Convert.ToInt32(ChangeRateConfig.Rate);
                        master.vipExpiredAt = DateTime.UtcNow.AddSeconds(VIP_DURATION_SECS);
                        await _masterRepository.Update(session, master);
                    }
                    else
                    {
                        var itemSave = new ItemConfig();
                        if (item.EntityType.ToLower() == "rsspackage")
                        {
                            var ChangeRateConfig = gameCfgLoader.GameConfigs.ChangeRateItem.Where(cr => cr.ExchangeItem == item.Id && cr.ReceivedItem == (long)Item.RSS).FirstOrDefault();
                            itemSave = GameConfigs.ItemConfigs.FirstOrDefault(i => i.Code == "rss");
                            param.Quantities = ChangeRateConfig != null ? ChangeRateConfig.Rate : 0;
                        }
                        else
                        {
                            itemSave = item;
                        }

                        var userItem = await itemMasterRepo.Find(im => im.MainUserId == master.MainUserId && im.ItemId == itemSave.Id).FirstOrDefaultAsync();
                        if (userItem == null)
                        {
                            userItem = new ItemMasterDAO
                            {
                                ItemId = item.Id,
                                Name = itemSave.Name,
                                Quantity = param.Quantities,
                                MainUserId = master.MainUserId,

                            };
                            await _itemMasterRepository.Add(userItem, session);
                        }
                        else
                        {

                            userItem.Quantity += param.Quantities;
                            await _itemMasterRepository.Update(userItem, session);
                        }
                    }
                    await session.CommitTransactionAsync();

                }
                catch (System.Exception ex)
                {
                    await session.AbortTransactionAsync();
                    logger.LogException(ex);
                    throw;

                }

            }

        }


        public async Task<ApiResultDTO<FaucetResultDTO>> FaucetRss()
        {
            var key = $"faucet_rss_{identityService.CurrentUserId}";
            var value = await redisDatabase.StringGetAsync(key);
            var timestampt = string.Empty;
            var result = false;
            if (value.IsNullOrEmpty)
            {
                var master = await masterRepo.Find(m => m.MainUserId == identityService.CurrentUserId).FirstOrDefaultAsync();
                var userItem = await itemMasterRepo.Find(im => im.MainUserId == master.MainUserId && im.ItemId == 5).FirstOrDefaultAsync();
                var itemData = GameConfigs.ItemConfigs.FirstOrDefault(i => i.Id == 5);
                if (userItem == null)
                {
                    userItem = new ItemMasterDAO
                    {
                        MainUserId = master.MainUserId,
                        WalletId = master.MainUserId,
                        ItemId = 5,
                        Name = itemData != null ? itemData.Name : "",
                        Quantity = 5000,

                    };
                    await itemMasterRepo.InsertOneAsync(userItem);
                }
                else
                {

                    userItem.Quantity += 5000;
                    await itemMasterRepo.ReplaceOneAsync(i => i.Id == userItem.Id, userItem);
                }
                await redisDatabase.StringSetAsync(key, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), TimeSpan.FromMinutes(30));
                result = true;
                timestampt = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            }
            else
            {
                result = false;
                timestampt = value.ToString();
            }
            return new ApiResultDTO<FaucetResultDTO>
            {
                Data = new FaucetResultDTO
                {
                    succeeded = result,
                    timestamp = Convert.ToInt64(timestampt)
                }
            };
        }
    }
}