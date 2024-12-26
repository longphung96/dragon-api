using System.Collections.Generic;
using DragonAPI.Common;
using DragonAPI.Enums;
using DragonAPI.Models.DTOs;
using DragonAPI.Models.Entities;

namespace DragonAPI.IntegrationEvent.Events
{

    public class BattlePreparingRequestETO
    {
        public string Id { get; set; }
        public List<BattlePreparingRequestMasterETO> Masters { get; set; }
    }

    public class BattlePreparingRequestMasterETO
    {
        public string MainUserId { get; set; }
        public string UserId { get; set; }
        public List<string> MainUserWallets { get; set; }
        public List<string> DragonIds { get; set; }

    }

    public enum BattlePreparedDataTypeEnum
    {
        BattlePreparedDataType_Master,
        BattlePreparedDataType_Dragon,
        BattlePreparedDataType_Seal,
        BattlePreparedDataType_Fruit,
    }

    public class BattlePrepareResponseETO
    {
        public string Id { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public BattlePreparedDataTypeEnum PreparedDataType { get; set; } = BattlePreparedDataTypeEnum.BattlePreparedDataType_Dragon;
        public object PreparedData { get; set; }
    }

    public class BattlePreparedDragonETO
    {
        public List<BattlePreparedMasterDragonETO> Masters { get; set; }
    }

    public class BattlePreparedMasterDragonETO : BattlePreparingRequestMasterETO
    {
        public List<BattleDragonEto> Dragones { get; set; }
    }

    public class BattleDragonEto
    {
        public string Id { get; set; }
        public string WalletId { get; set; }
        public string NftId { get; set; }
        public string Name { get; set; }
        public long Level { get; set; }
        public double Exp { get; set; }
        public ClassType Class { get; set; }
        public Gender Gender { get; set; }
        public List<Bodypart> Bodyparts { get; set; }
        public int HappyPoint { get; set; }
        public bool IsOffChain { get; set; }
    }

    public class BattleCreatedETO : BattleServerRegisteredIntegrationEvent { }


    public class BattleCompletedETO
    {
        public string Id { get; set; }
        // public List<BattleMasterEvtItem> Masters { get; set; }
        // public List<BattleDragonEvtItem> Dragons { get; set; }
        public string[] LoserIds { get; set; }
        public string[] AfkIds { get; set; }
        // public double? BonusDragonExp { get; set; }
        public WinningBattleResultDto WinningResult { get; set; }
    }
    public class BattlePvPCompletedETO
    {
        public string Id { get; set; }
        public string WinnerId { get; set; }
        public int WinPosition { get; set; }
    }


    //////////////////////////////////////////////////////
    public class BattleSetupDragonFormationAwaitValidationRequest
    {
        public string MainUserId { get; set; }
        public List<string> MainUserWallets { get; set; }
        public List<string> DragonIds { get; set; }
    }
    public class BattleSetupDragonFormationAwaitValidationResponse : BattleSetupDragonFormationAwaitValidationRequest
    {
        public bool Success { get; set; } = true;
        public string Error { get; set; } = string.Empty;
    }

    //////////////////////////////////////////////////////


    public class BattleDragonEvtItem
    {
        public string Id { get; set; }
        public string MasterId { get; set; }
        public string MainUserId { get; set; }
        public string UserId { get; set; }
        public string NftId { get; set; }
        public string Name { get; set; }
        public bool IsMob { get; set; }
        public string WalletId { get; set; }
        public UInt16 Level { get; set; }
        public double Exp { get; set; }
        public bool Died { get; set; }
        public ClassType Class { get; set; }
        public List<Bodypart> Bodyparts { get; set; }
        public bool IsOffChain { get; set; }
        public long IndexFormation { get; set; } = 0;
        public StatsDto Stats { get; set; }
    }
    public class BattleSetupDragonFormationResultETO
    {
        public string UserId { get; set; }
        public string MainUserId { get; set; }
        public string RequestId { get; set; }
        public bool Success { get; set; }
    }
    public class BattleMasterEvtItem
    {
        public string Id { get; set; }
        public string MainUserId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public uint Level { get; set; }
        public ulong Exp { get; set; }
        public ConnectionState ConnectionState { get; set; }
        public ConfirmationState ConfirmationState { get; set; }
        public bool IsBot { get; set; }
        public bool AutoAttack { get; set; } = true;
        public bool X3Speed { get; set; } = false;
    }
    public class PVEBattleRequestIntegrationEvent : BaseBattleData
    {
        public class PVEMetadata
        {
            public string MapId { get; set; }
            public string StageId { get; set; }
            public string StageMobFormationId { get; set; }
        }
        public string Id { get; set; }
        public BattleMasterEvtItem Master { get; set; }
        public List<BattleDragonEvtItem> MasterDragons { get; set; }
        public List<BattleDragonEvtItem> DragonMobs { get; set; }
        public string StageId { get; set; }
        public string StageMobFormationId { get; set; }
        public string EnvironmentId { get; set; }

    }
    public class PVPBattleRequestIntegrationEvent : BaseBattleData
    {
        public class PVPMetadata
        {


        }
        public string Id { get; set; }
        public BattleMasterEvtItem Master { get; set; }
        public List<BattleDragonEvtItem> MasterDragons { get; set; }
        public BattleMasterEvtItem EnemyPlayer { get; set; }
        public List<BattleDragonEvtItem> DragonesEnemy { get; set; }

    }
    public class BaseBattleData
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public object Metadata { get; set; }
    }
    public class BattleServerStateChangedIntegrationEvent : BaseBattleData
    {
        public string Id { get; set; }
        public List<BattleMasterEvtItem> Masters { get; set; }
        public List<BattleDragonEvtItem> Dragons { get; set; }
        public BattleType Type { get; set; }
        public string[] LoserIds { get; set; }
        public string[] AfkIds { get; set; }
        public string StageId { get; set; }
        public string StageMobFormationId { get; set; }
        public BattleState State { get; set; }
    }
    public class BattleServerRegisteredIntegrationEvent : BaseBattleData
    {
        public string Id { get; set; }
        public string BattleServierInstanceId { get; set; }
        public BattleType Type { get; set; }
        public List<BattleMasterEvtItem> Masters { get; set; }
        public List<BattleDragonEvtItem> Dragons { get; set; }
        public DateTimeOffset ExpiredAt { get; set; }
        public string StageId { get; set; }
        public string StageMobFormationId { get; set; }
        public string EnvironmentId { get; set; }
        public string Cookie { get; set; }
        public int BackgroundId { get; set; }

    }

    public class BattleServerRegisterFailedETO
    {
        public string Id { get; set; }
        public List<BattleMasterEvtItem> Masters { get; set; }
    }
    public class BattleCreateFailedETO
    {
        public string Id { get; set; }
        public List<BattleMasterEvtItem> Masters { get; set; }
    }
    public class BattlesDestroyedIntegrationEvent
    {
        public List<string> BattleIds { get; set; }
    }
    // public class BattleRegister
    // {
    //     public string Id { get; set; }
    //     public BattleMasterEvtItem Master { get; set; }
    //     public List<BattleDragonEvtItem> MasterDragons { get; set; }
    //     public List<BattleDragonEvtItem> DragonMobs { get; set; }
    //     public string StageId { get; set; }
    //     public string StageMobFormationId { get; set; }
    //     public string EnvironmentId { get; set; }
    //     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //     public object Metadata { get; set; }
    //     // public List<BattleMasterEvtItem> Masters { get; set; }
    // }
}