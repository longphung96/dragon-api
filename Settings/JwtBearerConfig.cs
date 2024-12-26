using System.Collections.Generic;

namespace DragonAPI.Application.Settings
{
    public class JwtBearerConfig
    {
        public string Authority { get; set; }
        public string ValidAudiences { get; set; }
        public string ValidIssuers { get; set; }
        public OAuth OAuth { get; set; }
    }

    public class OAuth
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
    }
}