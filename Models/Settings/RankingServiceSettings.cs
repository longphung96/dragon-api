

using System.Collections.Generic;

namespace DragonAPI.Models.Settings
{
    public class RankingServiceSettings
    {
        public string LEADERBOAR_URL { get; set; }
        public string API_UPDATE_HIGHTSCORE { get; set; }
        public string API_GET_TOP_SCORE { get; set; }
        public List<string> Topics { get; set; }
    }
}