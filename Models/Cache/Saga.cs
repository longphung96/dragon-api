using DragonAPI.Models.DAOs;
using DragonAPI.Models.Entities;
using DragonAPI.Enums;
using DragonAPI.Models.DTOs;
using System;
using DragonAPI.IntegrationEvent.Events;

namespace DragonAPI.Models.Cache
{
    public class SagaItem
    {
        public string CorrelationId { get; set; }
        public ActionTypeEnum ActionType { get; set; }
        public object Payload { get; set; }
        public UserDAO User { get; set; }
    }

    public class DragonBodypartUnlockingData
    {
        public string Id { get; set; }
        public BodypartType BodypartType { get; set; }
    }

    public class DragonBodypartUnlockSkillData
    {
        public string Id { get; set; }
        public BodypartType BodypartType { get; set; }
    }

    public class DragonBodypartLevelUpData
    {
        public string Id { get; set; }
        public Bodypart Bodypart { get; set; }
    }
    public class DragonRaceLevelUpData
    {
        public string Id { get; set; }
    }

    public class SigingMetaTx
    {
        public bool Success { get; set; } = false;
        public string Wallet { get; set; }
        public DateTime ExpiredAt { get; set; }
        public SigningResponseETO Data { get; set; }
    }

    public class DragonMatingSagaData
    {
        public DragonDAO father { get; set; }
        public DragonDAO mother { get; set; }
        public long fatherBreakTime { get; set; }
        public long motherBreakTime { get; set; }
        public bool RssConsumtionSuccess { get; set; } = false;
        public string RssConsumeTxId { get; set; }
        public SigingMetaTx SigingMetaTx { get; set; } = new();
    }

    public class DragonWorkplaceCreatingSagaData
    {
        public DragonDAO Dragon { get; set; }
        public long ListingDuration { get; set; }
        public long LeasingDuration { get; set; }
        public double Rss { get; set; }
        public SigingMetaTx SigingMetaTx { get; set; } = new();
    }

    public class DragonWorkplaceLeaseSagaData
    {
        public DragonDAO Dragon { get; set; }
        public SigingMetaTx SigingMetaTx { get; set; } = new();
        public bool SagaSuccess => SigningSuccess && ConsumedCurrencySuccess;
        public bool SigningSuccess { get; set; } = false;
        public bool ConsumedCurrencySuccess { get; set; } = false;
        public string ConsumedCurrencyTxId { get; set; }
    }


    public class EggIncubatingSagaData
    {
        public string Id { get; set; }
        public bool RssConsumtionSuccess { get; set; } = false;
        public string RssConsumeTxId { get; set; }
        public SigingMetaTx SigingMetaTx { get; set; } = new();
    }

    public class EggHatchingSagaData
    {
        public string Id { get; set; }
        public SigingMetaTx SigingMetaTx { get; set; } = new();
    }
}
