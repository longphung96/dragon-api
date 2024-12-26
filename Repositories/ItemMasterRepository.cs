using MongoDB.Driver;
using DragonAPI.Data;
using DragonAPI.Enums;
using DragonAPI.Models.DAOs;
using DragonAPI.Enums;
using DragonAPI.Extensions;
namespace DragonAPI.Repositories
{
    public interface IItemMasterRepository : IRepository<ItemMasterDAO>
    {
        Task<ItemMasterDAO> Add(ItemMasterDAO obj, IClientSessionHandle session);
        Task<ItemMasterDAO> GetById(string id);
        Task<ItemMasterDAO> GetByItemId(long id);
        Task<IEnumerable<ItemMasterDAO>> GetAll();
        Task<ItemMasterDAO> Update(ItemMasterDAO obj, IClientSessionHandle session);
        Task<bool> Remove(string id);
        Task<bool> TakeItem(IClientSessionHandle session, ItemMasterDAO obj, decimal ItemReqAmount, string? reason, ILogger logger);
        Task<bool> AddItem(IClientSessionHandle session, string mainUserId, long itemId, decimal ItemAmount, string? reason);
    }
    public class ItemMasterRepository : MongoRepository<ItemMasterDAO>, IItemMasterRepository
    {
        protected readonly IMongoCollection<MasterBalanceTransactionHistoryDAO> masterBalanceTransactionHistoryRepo;
        protected readonly IMongoCollection<MasterDAO> masterRepo;
        protected readonly IMongoDatabase Database;
        protected readonly IMongoCollection<ItemMasterDAO> itemMasterRepo;
        public ItemMasterRepository
        (DragonMongoDbContext context
        ) : base(context)
        {
            this.masterBalanceTransactionHistoryRepo = context.Database.GetCollection<MasterBalanceTransactionHistoryDAO>("masterBalanceTransactionHistories");
            this.itemMasterRepo = context.Database.GetCollection<ItemMasterDAO>("item_master");
            this.masterRepo = context.Database.GetCollection<MasterDAO>("masters");
        }


        public virtual async Task<ItemMasterDAO> Add(ItemMasterDAO obj, IClientSessionHandle? session)
        {
            await itemMasterRepo.InsertOneAsync(session, obj);
            return obj;
        }

        public virtual async Task<ItemMasterDAO> GetById(string id)
        {
            var data = await itemMasterRepo.Find(FilterId(id)).FirstOrDefaultAsync();
            return data;
        }
        public virtual async Task<ItemMasterDAO> GetByItemId(long itemId)
        {
            var data = await itemMasterRepo.Find(im => im.ItemId == itemId).FirstOrDefaultAsync();
            return data;
        }
        public virtual async Task<IEnumerable<ItemMasterDAO>> GetAll()
        {
            var all = await itemMasterRepo.FindAsync(Builders<ItemMasterDAO>.Filter.Empty);
            return all.ToList();
        }

        public async virtual Task<ItemMasterDAO> Update(ItemMasterDAO obj, IClientSessionHandle session)
        {
            await itemMasterRepo.ReplaceOneAsync(session, im => im.Id == obj.Id, obj);
            return obj;
        }

        public async virtual Task<bool> Remove(string id)
        {
            var result = await itemMasterRepo.DeleteOneAsync(FilterId(id));
            return result.IsAcknowledged;
        }

        public async Task<bool> TakeItem(IClientSessionHandle session, ItemMasterDAO item, decimal ItemReqAmount, string? reason, ILogger logger)
        {
            if (item.Quantity < ItemReqAmount) return (false);
            item.Quantity -= ItemReqAmount;
            var result = await itemMasterRepo.ReplaceOneAsync(session, FilterId(item.Id), item);
            var tx = new MasterBalanceTransactionHistoryDAO
            {
                MainUserId = item.MainUserId,
                ItemId = item.ItemId,
                Quantity = -1 * ItemReqAmount,
                Metadata = reason,
            };
            await masterBalanceTransactionHistoryRepo.InsertOneAsync(session, tx);
            var master = await masterRepo.Find(m => m.MainUserId == item.MainUserId).FirstOrDefaultAsync();
            var currencyConsumedLog = new
            {
                // WalletId = master.WalletId,
                MasterId = master.Id,
                MasterName = master.Name,
                MainUserId = item.MainUserId,
                ItemId = item.ItemId,
                Quantity = ItemReqAmount,
                Reason = reason,
            };
            logger.LogInformation($"TakeRss TakeCurrency {currencyConsumedLog.ToAnalysisLog(LoggingContextEnum.RssContext)}");
            // await serverHub.Clients.Group(master.Id).MasterRssUpdate(master.Id, (ulong)master.Rss, (ulong)master.Lrss);
            // _ = eventBus.PublishAsync(EventNameConst.MasterRssUpdated, new
            // {
            //     // WalletId = master.WalletId,
            //     MasterId = master.Id,
            //     MasterName = master.Name,
            //     MainUserId = master.MainUserId,
            //     Rss = master.Rss,
            //     LRss = master.Lrss
            // });
            return (true);
        }
        public async Task<bool> AddItem(IClientSessionHandle session, string mainUserId, long itemId, decimal ItemAmount, string? reason)
        {
            var item = await itemMasterRepo.Find(i => i.ItemId == itemId && i.MainUserId == mainUserId).FirstOrDefaultAsync();
            if (item == null)
            {
                item = new ItemMasterDAO
                {
                    MainUserId = mainUserId,
                    ItemId = itemId,
                    Quantity = ItemAmount,
                    Name = ((Item)itemId).ToString(),
                };
                await itemMasterRepo.InsertOneAsync(item);
            }
            else
            {
                await itemMasterRepo.ReplaceOneAsync(session, FilterId(item.Id), item);
            }

            var tx = new MasterBalanceTransactionHistoryDAO
            {
                MainUserId = item.MainUserId,
                ItemId = item.ItemId,
                Quantity = ItemAmount,
                Metadata = reason,
            };
            await masterBalanceTransactionHistoryRepo.InsertOneAsync(session, tx);
            var master = await masterRepo.Find(m => m.MainUserId == item.MainUserId).FirstOrDefaultAsync();
            var currencyConsumedLog = new
            {
                // WalletId = master.WalletId,
                MasterId = master.Id,
                MasterName = master.Name,
                MainUserId = item.MainUserId,
                ItemId = item.ItemId,
                Quantity = ItemAmount,
                Reason = reason,
            };
            // logger.LogInformation($"TakeRss TakeCurrency {currencyConsumedLog.ToAnalysisLog(LoggingContextEnum.RssContext)}");
            // // await serverHub.Clients.Group(master.Id).MasterRssUpdate(master.Id, (ulong)master.Rss, (ulong)master.Lrss);
            // _ = eventBus.PublishAsync(EventNameConst.MasterRssUpdated, new
            // {
            //     // WalletId = master.WalletId,
            //     MasterId = master.Id,
            //     MasterName = master.Name,
            //     MainUserId = master.MainUserId,
            //     Rss = master.Rss,
            //     LRss = master.Lrss
            // });
            return (true);
        }
    }
}
