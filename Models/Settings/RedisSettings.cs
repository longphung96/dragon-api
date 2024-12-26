namespace DragonAPI.Models.Settings
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; }
        public int DefaultDatabase { get; set; }
        public string ChannelPrefix { get; set; }
    }
}