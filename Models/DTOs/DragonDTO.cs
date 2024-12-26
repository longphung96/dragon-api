using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using AutoFilterer.Attributes;
using AutoFilterer.Types;
using DragonAPI.Enums;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Models.DAOs;
using DragonAPI.Models.Entities;

namespace DragonAPI.Models.DTOs
{
    public class OwnClassMapDto
    {
        public ClassType OwnClass { get; set; }
        public BlockchainClass BcOwnClass { get; set; }
    }

    public class ChangeDragonNameRequest
    {
        public string Name { get; set; }
    }

    public class DragonDTO : BaseWithTimeDTO, IDragonBase
    {
        [JsonPropertyName("wallet_id")]
        public string WalletId { get; set; }
        [JsonPropertyName("nft_id")]
        public string NftId { get; set; }
        [JsonPropertyName("class")]
        public ClassType Class { get; set; }
        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }
        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }
        [JsonPropertyName("rongosAnimations")]
        public DragonAnimationDto DragonAnimations { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("level")]
        public ushort Level { get; set; }
        [JsonPropertyName("rarity")]
        public int Rarity { get; set; } = 0;
        [JsonPropertyName("exp")]
        public double Exp { get; set; }
        [JsonPropertyName("max_exp")]
        public ulong? MaxExp { get; set; }
        [JsonPropertyName("baseStats")]
        public Stats BaseStats { get; set; }
        [JsonPropertyName("stats")]
        public FullStatsDto Stats { get; set; }
        public long DragonFarmLevel { get; set; } = 0;
        // [JsonPropertyName("is_boss_mob")]
        // public bool IsBossMob { get; set; }
        // [JsonPropertyName("boss_mob_skills")]
        // public string[] BossMobSkills { get; set; }
        [JsonPropertyName("bodyparts")]
        public List<Bodypart> Bodyparts { get; set; }
        [JsonPropertyName("cooldown_end_at")]
        public DateTime? CooldownEndAt { get; set; }
        public bool IsOffChain { get; set; }
        public bool IsF2P { get; set; }
        public SaleStatus SaleStatus { get; set; }
        public PlacedOrderInfoDTO PlacedOrderInfo { get; set; } = null;
    }

    public class DragonAnimationDto
    {
        public DragonAnimationDto() { }
        public AnimationDto Mature { get; set; }
    }

    public class AnimationDto
    {
        public AnimationDto() { }
        public string Atlas { get; private set; }
        public string Model { get; private set; }
        public string Image { get; private set; }

        public AnimationDto(string baseUrl, string entity, string nftId, string state = "", long? time = null)
        {
            var t = time == null ? DateTimeOffset.UtcNow.ToUnixTimeSeconds() : time;
            state = !string.IsNullOrEmpty(state) ? $"/{state}" : "";
            this.Atlas = $"{baseUrl}/nft/{entity}/{nftId}{state}/rongos.atlas?t={t}";
            this.Model = $"{baseUrl}/nft/{entity}/{nftId}{state}/rongos.json?t={t}";
            this.Image = $"{baseUrl}/nft/{entity}/{nftId}{state}/rongos.png?t={t}";
        }
    }

    public class DragonUnlockBodypartRequestDTO
    {
        [Required]
        public BodypartType Bodypart { get; set; }
    }

    public class DragonBodypartLevelUpRequestDTO
    {
        [Required]
        public BodypartType Bodypart { get; set; }
    }

    public class LendingInfoDTO
    {
        public string Id { get; set; }
        public string Rss { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime EndAt { get; set; }
        public long BorrowDuration { get; set; } = 0;
        public DateTime? BorrowedAt { get; set; } = null;
        public string Borrower { get; set; } = string.Empty;
    }
    public class PlacedOrderInfoDTO
    {
        public string Id { get; set; }
        public string PriceStart { get; set; }
        public string PriceEnd { get; set; }
        public DateTime ListedAt { get; set; }
        public DateTime ListedTo { get; set; }
    }



    public class DragonFilter : Common.BaseFilter
    {
        public string? Sorting { get; set; }
        [IgnoreFilter]
        public string[]? WalletId { get; set; }
        public string[]? Id { get; set; }
        public ulong[]? NftId { get; set; }
        public ClassType[]? Class { get; set; }
        public SaleStatus? SaleStatus { get; set; }
        public OperatorFilter<UInt16>? Level { get; set; }
        public long? DragonFarmLevel { get; set; }
        // public OperatorFilter<DateTime> CooldownEndAt { get; set; }
        public DragonBodypartFilterDto? Bodyparts { get; set; }
        public PlaceOrderInfoFilterDto? PlacedOrderInfo { get; set; }
        public bool? IsOffChain { get; set; }
        // public bool? IsF2P { get; set; }
    }
    public class MyDragonFilter : Common.BaseFilter
    {
        public string? Sorting { get; set; }
        [IgnoreFilter]
        public string[]? WalletId { get; set; }
        public ulong[]? NftId { get; set; }
        public ClassType[]? Class { get; set; }
        public SaleStatus? SaleStatus { get; set; }
        public OperatorFilter<UInt16>? Level { get; set; }
        // public OperatorFilter<DateTime> CooldownEndAt { get; set; }
        public DragonBodypartFilterDto? Bodyparts { get; set; }
        public PlaceOrderInfoFilterDto? PlacedOrderInfo { get; set; }
        public bool? IsOffChain { get; set; }
    }

    public class DragonBodypartFilterDto : FilterBase
    {
        public long[] Class { get; set; }
        public MutantType[] MutantType { get; set; }
    }

    public class DragonLendingInfoFilterDto : FilterBase
    {
        public StringFilter Borrower { get; set; }
        public OperatorFilter<DateTime> EndAt { get; set; }
    }

    public class PlaceOrderInfoFilterDto : FilterBase
    {
        public OperatorFilter<decimal> PriceStart { get; set; }
        public OperatorFilter<decimal> PriceEnd { get; set; }
        public OperatorFilter<DateTime> ListedTo { get; set; }
    }

    public class DragonMergeRequestDto
    {
        public string WalletId { get; set; }
        public long FragmentId { get; set; }
    }
    public class DragonCreateRequestDto
    {
        public string WalletId { get; set; }
        public long Class { get; set; }
    }


    public class WorkplaceDragonCreate
    {
        [Required]
        public long ListingDuration { get; set; }
        [Required]
        public long LeasingDuration { get; set; }
        [Required]
        public double Rss { get; set; }
    }
    public class BodypartDTO : Bodypart { }

    public class DragonOpenRequestDto
    {
        public string WalletId { get; set; }
        public long PackageId { get; set; }
    }
}