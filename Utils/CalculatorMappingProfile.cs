using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DragonAPI.Models.Settings;
using Microsoft.Extensions.Logging;
using DragonAPI.Configurations;
using DragonAPI.Enums;
using DragonAPI.Models.DAOs;
using DragonAPI.Models.Entities;
using DragonAPI.Helpers;
using DragonAPI.Services;

namespace DragonAPI.Utils
{
    public class CalculatorMappingProfile : Profile
    {
        public CalculatorMappingProfile()
        {
            // CreateMap<ClassType, DragonAPI.Utils.ClassType>();
            // CreateMap<BodypartType, DragonAPI.Utils.BodypartType>();
            // CreateMap<MutantType, DragonAPI.Utils.MutantType>();
            // CreateMap<SealSlotPowerType, DragonAPI.Utils.SealSlotPowerType>();

            CreateMap<DragonDAO, DragonAPI.Common.Dragon>()
                .ForMember(dest => dest.Stats, opt => opt.MapFrom<DragonFullStatsTransformResolver>());
            CreateMap<DragonMob, DragonAPI.Common.Dragon>()
                .ForMember(dest => dest.Stats, opt => opt.MapFrom<DragonMobFullStatsTransformResolver>());
            // CreateMap<Bodypart, DragonAPI.Utils.Bodypart>();
            CreateMap<FullStats, DragonAPI.Common.FullStats>();
            CreateMap<BuffStats, DragonAPI.Common.BuffStats>();



            // config
            CreateMap<StatCoef, DragonAPI.Common.StatCoef>();
            CreateMap<BuffSkillConfig, DragonAPI.Common.Dragon.SkillConfig>();
        }
    }

    public class DragonFullStatsTransformResolver : IValueResolver<DragonDAO, DragonAPI.Common.Dragon, DragonAPI.Common.FullStats>
    {
        private readonly GameConfigs gameConfigs;
        public DragonFullStatsTransformResolver(ConfigLoader configLoader)
        {
            gameConfigs = configLoader.GameConfigs;
        }
        public DragonAPI.Common.FullStats Resolve(DragonDAO source, DragonAPI.Common.Dragon destination, DragonAPI.Common.FullStats destMember, ResolutionContext context)
        {
            var rongosBaseStats = StatsCalculator.BuildDragonStats(source, gameConfigs);
            var rongosFullStats = FullStats.FromBaseStats(rongosBaseStats, gameConfigs, source.Level);
            return context.Mapper.Map<FullStats, DragonAPI.Common.FullStats>(rongosFullStats);
        }
    }
    public class DragonMobFullStatsTransformResolver : IValueResolver<DragonMob, DragonAPI.Common.Dragon, DragonAPI.Common.FullStats>
    {
        private readonly GameConfigs gameConfigs;
        private readonly ILogger<DragonMobFullStatsTransformResolver> logger;
        public DragonMobFullStatsTransformResolver(ConfigLoader configLoader, ILogger<DragonMobFullStatsTransformResolver> logger)
        {
            gameConfigs = configLoader.GameConfigs;
            this.logger = logger;
        }
        public DragonAPI.Common.FullStats Resolve(DragonMob source, DragonAPI.Common.Dragon destination, DragonAPI.Common.FullStats destMember, ResolutionContext context)
        {
            // logger.LogDebug($"{source.ToRawJson()}");
            var rongosBaseStats = StatsCalculator.BuildDragonStats(source, gameConfigs);
            var rongosFullStats = FullStats.FromBaseStats(rongosBaseStats, gameConfigs, source.Level);
            return context.Mapper.Map<FullStats, DragonAPI.Common.FullStats>(rongosFullStats);
        }
    }
}