using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using DragonAPI.Enums;

namespace DragonAPI.IntegrationEvent.Events
{

    public class BlockchainGenericDataInfo<T>
    {
        [JsonPropertyName("info")]
        public T Info { get; set; }
    }

    public class BlockchainIntegrationEvent
    {
        public const string InvalidId = "0";

        public string eventName { get; set; }
        public string txHash { get; set; }
        public JsonElement payload { get; set; }
    }

    public class BlockchainDragonBirthEto : BlockchainGenericDataInfo<BlockchainDragonInfo>
    {
        public string owner { get; set; }
        public string dragonId { get; set; }
        public string matronId { get; set; }
        public string sireId { get; set; }
    }

    public class BlockchainDragonMatingCooldownEto
    {
        public long cooldownEndAt { get; set; }
        public string tokenId { get; set; }
    }

    public class BlockchainBodypartUnlockedEto : BlockchainGenericDataInfo<BlockchainDragonInfo>
    {
        [JsonPropertyName("tokenId")]
        public string TokenId { get; set; }
        public bool succeed { get; set; }
        public int bodyPartIdx { get; set; }
    }



    public class BlockchainEggCreatedEto : BlockchainGenericDataInfo<BlockchainEggInfo>
    {
        public string owner { get; set; }
        public string uniId { get; set; }
    }

    public class BlockchainEggIncubatingEto : BlockchainGenericDataInfo<BlockchainEggInfo>
    {
        public string owner { get; set; }
        public string uniId { get; set; }
    }
    public class BlockchainEggHatchedEto : BlockchainGenericDataInfo<BlockchainEggInfo>
    {
        public string owner { get; set; }
        public string uniId { get; set; }
        public bool succeed { get; set; }
    }

    public class BlockchainEggInfo
    {
        public string matronId { get; set; }
        public string sireId { get; set; }
        public int breedingIndex { get; set; }
        public long birthTime { get; set; }
        public long cooldownEndAt { get; set; }
        public long selectedClass { get; set; }
    }


    public class BlockchainSealCreatedEto : BlockchainGenericDataInfo<BlockchainSealInfo>
    {
        public string owner { get; set; }
        public string tokenId { get; set; }
    }

    public class BlockchainSealSlotUnlockedEto : BlockchainGenericDataInfo<BlockchainSealInfo>
    {
        public string tokenId { get; set; }
        public int slotIdx { get; set; }
    }

    public class BlockchainSealInfo
    {
        [JsonPropertyName("totalSlot")]
        public int TotalSlot { get; set; }
        [JsonPropertyName("slots")]
        public List<BlockchainSealSlotData> Slots { get; set; }
    }

    public class BlockchainSealSlotData
    {
        [JsonPropertyName("isUnlocked")]
        public bool Unlocked { get; set; }
        [JsonPropertyName("class")]
        public BlockchainClass Class { get; set; }
        [JsonPropertyName("power")]
        public BlockchainSealSlotPowerType Power { get; set; }
    }




    // Blockchain order
    public class BlockchainOrderCreatedEto
    {
        public string itemType { get; set; }
        public string orderId { get; set; }
        public string tokenId { get; set; }
        public long amount { get; set; }
        public string price { get; set; }
        public long duration { get; set; }
        public long endedAt { get; set; }
        public string seller { get; set; }
    }

    public class BlockchainOrderCancelledEto
    {
        public string itemType { get; set; }
        public string orderId { get; set; }
        public string tokenId { get; set; }
        public long amount { get; set; }
        public string seller { get; set; }
    }


    public class BlockchainOrderFilledEto
    {
        public string itemType { get; set; }
        public string orderId { get; set; }
        public string tokenId { get; set; }
        public long amount { get; set; }
        public string buyer { get; set; }
        public string seller { get; set; }
        public string sellerProcesseds { get; set; }
        public string sellerCut { get; set; }
        public long soldAt { get; set; }
    }



    public class BlockchainWorkplaceOrderCreatedEto
    {
        public long Id { get; set; }
        public long TokenId { get; set; }
        public long ListingDuration { get; set; }
        public long LeasingDuration { get; set; }
        public long EndedAt { get; set; }
        public string Lessor { get; set; }
        public long Nonce { get; set; }
        public string TxHash { get; set; }

    }

    public class BlockchainWorkplaceOrderLeasedEto
    {
        public long Id { get; set; }
        public long TokenId { get; set; }
        public long EndedAt { get; set; }
        public string Lessee { get; set; }
        public string Lessor { get; set; }
        public long Nonce { get; set; }
        public string TxHash { get; set; }
    }

    public class BlockchainWorkplaceOrderCancelledEto
    {
        public long Id { get; set; }
        public long TokenId { get; set; }
        public string TxHash { get; set; }
    }

    public class BlockchainWorkplaceOrderClaimedEto
    {
        public long Id { get; set; }
        public long TokenId { get; set; }
        public string TxHash { get; set; }
    }






    //////////////////////// Response
    /// <summary>
    /// 
    /// </summary>
    public enum BlockchainMutant
    {
        Normal = 1,
        Plus = 2,
        Minus = 9
    }
    public enum BlockchainClass
    {
        Undefined,
        Gold,
        Wood,
        Water,
        Fire,
        Earth,
        Dark,
        Light,
        Gold2,
        Wood2,
        Water2,
        Fire2,
        Earth2,
        Dark2,
        Light2,
        Gold3,
        Wood3,
        Water3,
        Fire3,
        Earth3,
        Dark3,
        Light3,
    }
    public enum BlockchainGender
    {
        Male,
        Female,
    }
    public enum BlockchainBodypartOrder
    {
        Undefined = -1,
        Claw,
        Horn,
        Skin,
        Tail,
        Eye,
        Wing,
    }
    public enum BlockchainSealSlotPowerType
    {
        Attack,
        BuffOrDebuff,
        ElementUp,
        StatsUp,
    }
    public static class BlockchainTypeEnumsTranformation
    {
        public static BodypartType ToInternal(this BlockchainBodypartOrder part)
        {
            BodypartType bodyPartType = BodypartType.Undefined;
            Enum.TryParse(part.ToString(), out bodyPartType);
            return bodyPartType;
        }
        public static SealSlotPowerType ToInternal(this BlockchainSealSlotPowerType power)
        {
            return (SealSlotPowerType)(power);
        }
        public static Gender ToInternal(this BlockchainGender gender)
        {
            Gender ret = Gender.Male;
            Enum.TryParse(gender.ToString(), out ret);
            return ret;
        }
        public static MutantType ToInternal(this BlockchainMutant mutant)
        {
            MutantType ret = MutantType.Normal;
            Enum.TryParse(mutant.ToString(), out ret);
            return ret;
        }
        public static ClassType ToInternal(this BlockchainClass _class)
        {
            ClassType type = ClassType.Undefined;
            Enum.TryParse(_class.ToString(), out type);
            return type;
        }
    }
    public class BlockchainBodypart
    {
        [JsonPropertyName("classId")]
        public UInt16 ClassId { get; set; }
        [JsonPropertyName("mutationId")]
        public BlockchainMutant Mutation { get; set; }
        [JsonPropertyName("percentages")]
        public List<string> Percentages { get; set; }
        [JsonPropertyName("isUnlocked")]
        public bool Unlocked { get; set; }
    }
    public class BlockchainDragonInfo
    {
        [JsonPropertyName("classId")]
        public BlockchainClass Class { get; set; }
        [JsonPropertyName("sex")]
        public BlockchainGender Gender { get; set; }
        [JsonPropertyName("matingTimes")]
        public int BreedCount { get; set; }
        [JsonPropertyName("cooldownEndAt")]
        public long CoolDownEndAt { get; set; }
        [JsonPropertyName("birthTime")]
        public long BirthTime { get; set; }
        [JsonPropertyName("traits")]
        public IEnumerable<BlockchainBodypart> Traits { get; set; }
    }


    ///////////////////////////////////////////////
    /// NEW
    //////////////////////////////////////////////

    public class PaymentSigningRequestBase
    {
        public string RequestId { get; set; }
        public string WalletId { get; set; }
        public long YNR { get; set; } = 0;
        public string SourceEntity { get; set; }
        public DateTime ExpiredAt { get; set; }
    }

    public class DragonWorkplaceCreateSigningETO : PaymentSigningRequestBase
    {
        public long NftId { get; set; }
        public long ListingDuration { get; set; }
        public long LeasingDuration { get; set; }
    }

    public class DragonWorkplaceLeaseSigningETO : PaymentSigningRequestBase
    {
        public long NftId { get; set; }
    }


    public class DragonMatingSigningETO : PaymentSigningRequestBase
    {
        public long FatherId { get; set; }
        public long MotherId { get; set; }
        public long FatherLockDuration { get; set; }
        public long MotherLockDuration { get; set; }
    }

    public class EggIncubatingSigningETO : PaymentSigningRequestBase
    {
        public long NftId { get; set; }
        public long Duration { get; set; } // unit: second
    }

    public class EggHatchingSigningETO : PaymentSigningRequestBase
    {
        public long NftId { get; set; }
        public long OpeningRate { get; set; }
    }


    public class BCMetaTx
    {
        public string Operator { get; set; }
        public string From { get; set; }
        public long Nonce { get; set; }
        public long ExpiredAt { get; set; }
        public string Data { get; set; }
    }

    public class SigningResponseETO
    {
        public string RequestId { get; set; }
        public bool Success { get; set; }
        public string SourceEntity { get; set; }
        [JsonPropertyName("Tx")]
        public BCMetaTx MetaTx { get; set; }
        public string Signature { get; set; }
    }

    public class SelfMatedETO
    {
        public string Owner { get; set; }
        public long TypeId { get; set; }
        public long TokenId { get; set; }
        public long EggId { get; set; }
        public long MatronId { get; set; }
        public long SireId { get; set; }
        public long Nonce { get; set; }
        public string From { get; set; }
        public string Operator { get; set; }
        public string TxHash { get; set; }
    }

    public class EggIncubatedETO
    {

        public string Owner { get; set; }
        public long TokenId { get; set; }
        public string TxHash { get; set; }
        public long Nonce { get; set; }
        public string From { get; set; }
        public string Operator { get; set; }
    }

    public class EggHatchETO
    {
        public string Owner { get; set; }
        public long DragonId { get; set; }
        public long TicketId { get; set; }
        public long EggId { get; set; }
        public long TokenId { get; set; }
        public bool Succeed { get; set; }
        public long Nonce { get; set; }
        public long Amount { get; set; }
        public long Index { get; set; }
        public string From { get; set; }
        public string Operator { get; set; }
        public long ItemSize { get; set; }
    }


    public class OrderCreatedETO
    {
        public string Owner { get; set; }
        public long TokenId { get; set; }
        public long TypeId { get; set; }
        public long OrderId { get; set; }
        public decimal Price { get; set; }
        public long Duration { get; set; }
        public long EndedAt { get; set; }
    }

    public class OrderCancelledETO
    {
        public long TokenId { get; set; }
        public long TypeId { get; set; }
        public long OrderId { get; set; }
    }

    public class OrderFilledETO
    {
        public long TokenId { get; set; }
        public long TypeId { get; set; }
        public long OrderId { get; set; }
        public string Buyer { get; set; }
        public string Seller { get; set; }
    }
    public class BirthCreateETO
    {
        public string Owner { get; set; }
        public long TypeId { get; set; }
        public long TokenId { get; set; }
        public string TxHash { get; set; }
    }
    public class TransferETO
    {
        public string From { get; set; }
        public string To { get; set; }
        public long TokenId { get; set; }
        public string TxHash { get; set; }

    }
    public class ETHWithdrawETO
    {
        public string AddressReceiver { get; set; }
        public decimal Amount { get; set; }
        public string TxId { get; set; }
        public string TxHash { get; set; }

    }
    public class ETHDepositedETO
    {
        public string AddressDeposit { get; set; }
        public decimal Amount { get; set; }
        public string TxHash { get; set; }

    }
    public class PaymentETO
    {
        public string PartnerCode { get; set; }
        public string ItemKeys { get; set; }
        public decimal Quantities { get; set; }
        public string From { get; set; }
        public string TxHash { get; set; }

    }
    [Function("transfer", "bool")]
    public class TransferFunction : FunctionMessage
    {
        [Parameter("address", "_to", 1)]
        public string To { get; set; }

        [Parameter("uint256", "_value", 2)]
        public BigInteger TokenAmount { get; set; }
    }
}