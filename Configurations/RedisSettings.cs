namespace DragonAPI.Configurations
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; }
        public int DefaultDatabase { get; set; }
        public string Prefix { get; set; }
    }
}