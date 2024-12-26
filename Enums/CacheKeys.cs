namespace DragonAPI.Enums
{
    public class CacheKeys
    {
        public const string OnlineWallets = "onlineWallets";
        public const string ConnectionKey = "connection:wallet";
        public const string ActiveBattleIdKey = "battle:active:id";
        public const string ActiveBattleWalletKey = "battle:active:wallet";
        public const string LastStageWinnerKey = "battle:pve:lastStageWinners";
        public const string BattleRequestCookie = "battle:request:cookie:id:";
        public const string PvpSoloRoomTimeOutJobId = "pvp_solo_room:{0}:timeout_jobid";
        public const string PvpSoloRoomConfirmUsers = "pvp_solo_room:{0}:confirmed_users";
        public const string CachedData = "cacheddata";
        public const string Online = "onlineUsers";
        public const string UniqueConnectionMasterKey = "connection:master";
        public const string ActiveBattleMasterKey = "battle:active:master";
        public const string CachedClaimRewardData = "claim_reward";
        public const string BattleRequest = "battle:request:id:";
        public const string BattlePrepareRequestCacheKey = "battlePrepareRequest:";
        public const string FindMatchPrepareRequestCacheKey = "findMatchPrepareRequest:";
    }
}