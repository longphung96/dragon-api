using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using DragonAPI.Configurations;
using DragonAPI.Data;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using System.Threading.Tasks;

namespace DragonAPI.Repositories
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class

    {

    }
    public abstract class MongoRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly IMongoDatabase Database;
        protected MongoRepository(DragonMongoDbContext context)
        {
            Database = context.Database;

        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public static FilterDefinition<TEntity> FilterId(string id)
        {
            return Builders<TEntity>.Filter.Eq("Id", id);
        }
    }
}
