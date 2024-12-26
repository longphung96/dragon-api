using Microsoft.Extensions.Configuration;
using DragonAPI.Configurations;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using System.Threading.Tasks;

namespace DragonAPI.Repositories
{
    public interface IRedisStore
    {
        IServer GetServer();
        IDatabase GetDatabase();
    }
    public class RedisStore : IRedisStore
    {
        private IConnectionMultiplexer RedisConnection;
        private string ConnectionString;
        private int DefaultDatabase;
        public RedisSettings RedisSettings;
        private readonly IDatabase database;
        public RedisStore(IConfiguration Configuration)
        {
            RedisSettings = Configuration
                 .GetSection("RedisSettings")
                 .Get<RedisSettings>();
            ConnectionString = RedisSettings.ConnectionString;
            DefaultDatabase = RedisSettings.DefaultDatabase;

            RedisConnection = ConnectionMultiplexer.Connect(RedisSettings.ConnectionString, conf =>
            {
                conf.DefaultDatabase = RedisSettings.DefaultDatabase;
                conf.ChannelPrefix = RedisSettings.Prefix;
                conf.AbortOnConnectFail = false;
            });
            database = RedisConnection.GetDatabase(DefaultDatabase).WithKeyPrefix(RedisSettings.Prefix);
        }

        public IDatabase GetDatabase()
        {
            return database;
        }

        public IServer GetServer()
        {
            return RedisConnection.GetServer(ConnectionString);
        }
    }
}
