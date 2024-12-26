using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DragonAPI.Enums
{
    /// <summary>
    /// Các mã lỗi khi xử lý api
    /// </summary>
    public enum ErrorCodeEnum
    {
        /*
        Client
        */
        [Description("UnexpectedErr")]
        UnexpectedErr = -1,
        OK = 0,
        [Description("InvalidRequest")]
        InvalidRequest,

        [Description("PermissionDenied")]
        PermissionDenied = 10,
        [Description("Entity wasn't existed")]
        EntityNotExisted = 11,
        [Description("You aren't own dragon seal")]
        NotOwnDragonSeal = 12,
        [Description("WalletIdEmptyErr")]
        WalletIdEmptyErr = 30,


        // Connection
        [Description("MasterDuplicatedConnection")]
        MasterDuplicatedConnection = 60,

        BattleNotSetup = 100,
        [Description("BattleEmpty")]
        // setup team
        PVPTeamNotSetup,
        [Description("PVETeamNotSetup")]
        PVETeamNotSetup,
        [Description("BattleTeamEmpty")]
        BattleTeamEmpty,
        [Description("DragonUnlockClawRequired")]
        DragonUnlockClawRequired,
        [Description("DragonNotOwned")]
        DragonNotOwned,
        // Battle
        [Description("ReputationNotEnough")]
        ReputationNotEnough = 350,
        [Description("EnergyNotEnough")]
        EnergyNotEnough,
        [Description("CookieInvalid")]
        CookieInvalid,
        //--------------- PVP Solo
        [Description("SoloRoomAlreadyCreated")]
        SoloRoomAlreadyCreated,

        // breeding
        [Description("DragonGenderInvalid")]
        DragonGenderInvalid = 400,
        [Description("DragonUnlockBodypartRequireLevel")]
        DragonUnlockBodypartRequireLevel,
        [Description("DragonBodypartAlreadyUnlocked")]
        DragonBodypartAlreadyUnlocked,
        [Description("HatchingTimeNotYetFinished")]
        IncubatingTimeNotYetFinished,

        // Map-Stage
        [Description("IsNotLastStage")]
        IsNotLastStage = 500,


        // stage formations
        [Description("NoValidStageFormation")]
        NoValidStageFormation = 550,
        //seal validate
        [Description("SealSlotRequireUnlocked")]
        SealSlotRequireUnlocked = 600,

        /// RSS
        [Description("RssNotEnought")]
        RssNotEnought = 650,
        [Description("ItemNotEnought")]
        ItemNotEnought = 651,
        [Description("FragmentNotEnought")]
        FragmentNotEnought = 652,
        [Description("StageNotPass")]
        StageNotPass = 700

    }

    public class BusinessException : System.Exception
    {
        public int? HttpErrCode { get; set; }
        public ErrorCodeEnum Error { get; set; }
        public BusinessException() { }
        public BusinessException(ErrorCodeEnum error, string message) : base(message)
        {
            this.Error = error;
            this.HttpErrCode = (int)HttpStatusCode.BadRequest;
        }
    }

}