using System;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using DragonAPI.Configurations;
using DragonAPI.Enums;
using DragonAPI.Extensions;
using DragonAPI.Helpers;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Models.Cache;
using DragonAPI.Models.DAOs;
using DragonAPI.Models.DTOs;
using DragonAPI.Models.Entities;
using DragonAPI.Services;

namespace DragonAPI.MapperProfiles
{
    public class DtoMapperProfile : Profile
    {
        public DtoMapperProfile()
        {
            CreateMap<FullStats, FullStatsDto>();

            CreateMap<DragonDAO, DragonDTO>()
                // .ForMember(d => d.DragonAnimations, opts => opts.MapFrom<DragonAnimationsTransformResolver>())
                .ForMember(d => d.ImageUrl, opts => opts.MapFrom<DragonImageUrlTransformResolver>())
                 .ForMember(d => d.AvatarUrl, opts => opts.MapFrom<DragonAvatarUrlTransformResolver>())
                .ForMember(dest => dest.Name, opt => opt.MapFrom<DragonNameTransformResolver>())
                .ForMember(dest => dest.BaseStats, opt => opt.MapFrom<DragonBaseStatsTransformResolver>())
                .ForMember(dest => dest.Stats, opt => opt.MapFrom<DragonFullStatsTransformResolver>())
                .ForMember(dest => dest.MaxExp, opt => opt.MapFrom<DragonMaxExpTransformResolver>());

            CreateMap<OffsetPagniationData<DragonDAO>, OffsetPagniationData<DragonDTO>>();

            CreateMap<PlacedOrderInfo, PlacedOrderInfoDTO>()
                .ForMember(d => d.PriceEnd, opts => opts.MapFrom(s => s.PriceEnd.ToString()))
                .ForMember(d => d.PriceStart, opts => opts.MapFrom(s => s.PriceStart.ToString()));
            CreateMap<LendingInfo, LendingInfoDTO>()
                .ForMember(d => d.Rss, opts => opts.MapFrom(s => s.Rss.ToString()));

            CreateMap<SigningResponseETO, SigningResponseDTO>();
            CreateMap<UserActionCacheItem, UserActionDTO>();

            CreateMap<BCMetaTx, MetaTxDTO>();

            CreateMap<Mob, Configurations.DragonMob>();
            CreateMap<UserDAO, UserDTO>();
            CreateMap<OffsetPagniationData<UserDAO>, OffsetPagniationData<UserDTO>>();

            CreateMap<MasterDAO, MasterDTO>()
                .ForMember(dest => dest.MaxExp, opt => opt.MapFrom<MasterMaxExpTransformResolver>());
            CreateMap<Bodypart, BodypartDTO>();
            CreateMap<FullStats, StatsDto>();
            CreateMap<MobInStageFormation, MobStageDTO>();
            CreateMap<OffsetPagniationData<DragonDAO>, OffsetPagniationData<DragonDTO>>();
            CreateMap<SettingsDAO, SettingsDTO>();
            CreateMap<MapStage, MapDTO>();
            CreateMap<MapStage, StageDTO>();
            CreateMap<ItemMasterDAO, ItemMasterDTO>();
            CreateMap<RankPositionDAO, RankPositionDTO>();
            CreateMap<CurrencyTransactionDAO, CurrencyTransactionDTO>();
            CreateMap<IdleAfkPoolDAO, AFKIdleItemPoolDTO>();
            CreateMap<WithDrawVerificationDAO, WithDrawVerificationDTO>();
            CreateMap<OffsetPagniationData<WithDrawVerificationDAO>, OffsetPagniationData<WithDrawVerificationDTO>>();
            CreateMap<PvPHistoryDAO, PvPHistoryDTO>();

        }
    }

}


public class DragonMaxExpTransformResolver : IValueResolver<DragonDAO, DragonDTO, UInt64?>
{
    private readonly GameConfigs gameConfigs;
    public DragonMaxExpTransformResolver(ConfigLoader gameCfgLoader)
    {
        this.gameConfigs = gameCfgLoader.GameConfigs;
    }
    public ulong? Resolve(DragonDAO source, DragonDTO destination, ulong? destMember, ResolutionContext context)
    {
        var levelConfig = gameConfigs.LevelExpConfig.EntityLevelConfigMap[Dragon.Blueprints.LevelTypeEnum.Dragon.Code];
        return (ulong)levelConfig.GetExpConfigAtLevel(source.Level).MaxExp;
    }
}
public class DragonImageUrlTransformResolver : IValueResolver<DragonDAO, DragonDTO, string>
{
    private readonly string bucketUrl;
    public DragonImageUrlTransformResolver(IConfiguration configuration)
    {
        bucketUrl = configuration.GetSection("S3AssetsBucketUrl").Get<string>();
    }

    public string Resolve(DragonDAO source, DragonDTO destination, string destMember, ResolutionContext context)
    {
        return $"{bucketUrl}/nft/new-rongoses/{(source.IsOffChain ? source.Id : source.NftId)}.png?t={source.AssetGeneratedAt}";
    }
}
public class DragonAvatarUrlTransformResolver : IValueResolver<DragonDAO, DragonDTO, string>
{
    private readonly string bucketUrl;
    public DragonAvatarUrlTransformResolver(IConfiguration configuration)
    {
        bucketUrl = configuration.GetSection("S3AssetsBucketUrl").Get<string>();
    }

    public string Resolve(DragonDAO source, DragonDTO destination, string destMember, ResolutionContext context)
    {
        return $"{bucketUrl}/nft/new-rongoses/non-background/{(source.IsOffChain ? source.Id : source.NftId)}.png?t={source.AssetGeneratedAt}";
    }
}

public class DragonAnimationsTransformResolver : IValueResolver<DragonDAO, DragonDTO, DragonAnimationDto>
{
    private readonly string bucketUrl;
    public DragonAnimationsTransformResolver(IConfiguration configuration)
    {
        bucketUrl = configuration.GetSection("S3AssetsBucketUrl").Get<string>();
    }
    public DragonAnimationDto Resolve(DragonDAO source, DragonDTO destination, DragonAnimationDto destMember, ResolutionContext context)
    {
        destMember = new DragonAnimationDto();
        destMember.Mature = new AnimationDto(bucketUrl, "rongoses", source.IsOffChain ? source.Id : source.NftId.ToString(), state: "mature", time: source.AssetGeneratedAt);
        return destMember;
    }
}
public class DragonNameTransformResolver : IValueResolver<DragonDAO, DragonDTO, string>
{
    private readonly GameConfigs gameConfigs;
    public DragonNameTransformResolver(ConfigLoader configLoader)
    {
        this.gameConfigs = configLoader.GameConfigs;
    }

    public string Resolve(DragonDAO source, DragonDTO destination, string destMember, ResolutionContext context)
    {
        return string.IsNullOrEmpty(source.Name) ? gameConfigs.DragonDefaultName[source.Class] : source.Name;
    }
}

public class DragonBaseStatsTransformResolver : IValueResolver<DragonDAO, DragonDTO, Stats>
{
    private readonly GameConfigs gameConfigs;
    public DragonBaseStatsTransformResolver(ConfigLoader configLoader)
    {
        this.gameConfigs = configLoader.GameConfigs;
    }
    public Stats Resolve(DragonDAO source, DragonDTO destination, Stats destMember, ResolutionContext context)
    {
        // gameConfigs.DragonStatsConfigMap.TryGetValue(source.Class, out DragonStatsConfig statsConfig);
        var Stats = gameConfigs.DragonStatsConfigs.FirstOrDefault(r => r.Class == source.Class && r.Rarity == source.Rarity);
        return Stats.StartingStats;
    }
}

public class DragonFullStatsTransformResolver : IValueResolver<DragonDAO, DragonDTO, FullStatsDto>
{
    private readonly GameConfigs gameConfigs;
    public DragonFullStatsTransformResolver(ConfigLoader configLoader)
    {
        this.gameConfigs = configLoader.GameConfigs;
    }
    public FullStatsDto Resolve(DragonDAO source, DragonDTO destination, FullStatsDto destMember, ResolutionContext context)
    {
        var stats = StatsCalculator.BuildDragonStats(source, gameConfigs);
        var fullStats = FullStats.BuildFromStats(stats, gameConfigs.StatCoef);
        return context.Mapper.Map<FullStats, FullStatsDto>(fullStats);
    }
}

//// EGG
///









