using MongoDB.Driver;

namespace DragonAPI.Data;

public interface IMongoContext
{
    IMongoDatabase Database { get; }
}
public class DragonMongoDbContext : IMongoContext
{
    private readonly IMongoClient client;
    private readonly string dbName;
    public DragonMongoDbContext(IConfiguration configuration)
    {
        this.client = new MongoClient(configuration["DatabaseSettings:ConnectionString"]);
        this.dbName = configuration["DatabaseSettings:DatabaseName"];
        Database = this.client.GetDatabase(this.dbName);
    }

    public IMongoClient GetClient()
    {
        return client;
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return this.Database.GetCollection<T>(collectionName);
    }
    public IMongoDatabase Database { get; }
}