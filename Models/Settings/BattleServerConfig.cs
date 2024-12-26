namespace DragonAPI.Models.Settings
{
    public class BattleServerConfig
    {
        public string BattleServerConfigUrl { get; set; }
        public string ApiPVEBattleRequest { get; set; }
        public string ApiPVPBattleRequest { get; set; }
        public string ApiPVPSoloBattleRequest { get; set; }
        public bool CookieRequired { get; set; }

        public string GetPVEBattleRequestUrl()
        {
            return BattleServerConfigUrl + ApiPVEBattleRequest;
        }
        public string GetPVPBattleRequestUrl()
        {
            return BattleServerConfigUrl + ApiPVPBattleRequest;
        }
        public string GetPVPSoloBattleRequestUrl()
        {
            return BattleServerConfigUrl + ApiPVPSoloBattleRequest;
        }
    }
}
