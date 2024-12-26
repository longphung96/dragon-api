using DragonAPI.Models.DAOs;

namespace DragonAPI.Models.Cache
{
    public enum ActionTypeEnum
    {
        None,
        DragonRaceLevelUp,
        DragonBodypartUnlock,
        DragonBodypartLevelUp,
        DragonBodypartSkillUnlock,
        DragonMating,

        EggIncubating,
        EggHatching,

        SigningRequest_DragonMating,
        SigningRequest_EggIncubating,
        SigningRequest_EggHatching,
        SigningRequest_DragonWorkplaceCreating,
        SigningRequest_DragonWorkplaceLeasing,
    }

    public enum ActionState
    {
        Pending,
        Succeed,
        Failed,
    }
    public class UserActionCacheItem
    {
        public string RequestId { get; set; }
        public UserDAO User { get; set; }
        public ActionTypeEnum ActionType { get; set; }
        public string Action { get; set; }
        public ActionState State { get; set; } = ActionState.Pending;
        public object Payload { get; set; }
    }
}