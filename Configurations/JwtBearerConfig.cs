using System.Collections.Generic;

namespace DragonAPI.Configurations
{
    public class JwtBearerConfig
    {
        public string Authority { get; set; }
        public List<string> ValidAudiences { get; set; }
        public List<string> ValidIssuers { get; set; }
    }
}