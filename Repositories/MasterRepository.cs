using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using DragonAPI.Configurations;
using DragonAPI.Data;
using DragonAPI.Models.DAOs;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using System.Threading.Tasks;

namespace DragonAPI.Repositories
{
    public interface IMasterRepository : IRepository<MasterDAO>
    {
        Task<MasterDAO> Update(IClientSessionHandle session, MasterDAO obj);
    }
    public class MasterRepository : MongoRepository<MasterDAO>, IMasterRepository
    {
        protected readonly ILoggerFactory logger;
        protected readonly IMongoCollection<MasterDAO> DbSet;
        public MasterRepository
        (DragonMongoDbContext context,
        ILoggerFactory logger
        ) : base(context)
        {
            this.logger = logger;
            this.DbSet = Database.GetCollection<MasterDAO>("masters");
        }
        public async Task<MasterDAO> Update(IClientSessionHandle session, MasterDAO master)
        {

            await DbSet.ReplaceOneAsync(session, FilterId(master.Id), master);
            return master;

        }
    }
}