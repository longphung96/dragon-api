using System;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using DragonAPI.Configurations;
using DragonAPI.Enums;
using DragonAPI.Extensions;
using DragonAPI.Helpers;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Models.DAOs;
using DragonAPI.Models.DTOs;
using DragonAPI.Models.Entities;
using DragonAPI.Services;

namespace DragonAPI.MapperProfiles
{
    public class EtoMapperProfile : Profile
    {
        public EtoMapperProfile()
        {
            CreateMap<DragonDAO, BattleDragonEto>();
            CreateMap<MasterDAO, BattleMasterEvtItem>()
                .ForMember(dest => dest.AutoAttack, opt => opt.MapFrom(src => src.AutoRemainCount > 0))
                .ForMember(dest => dest.X3Speed, opt => opt.MapFrom(src => src.X3SpeedRemainCount > 0));

            CreateMap<DragonDAO, BattleDragonEvtItem>()
                .ForMember(dest => dest.Stats, opt => opt.MapFrom<BattleDragonStatsTransformResolver>())
                .ForMember(dest => dest.Bodyparts, opt => opt.MapFrom<BattleDragonBodypartsTransformResolver>());
            CreateMap<DragonMob, BattleDragonEvtItem>()
                .ForMember(dest => dest.Stats, opt => opt.MapFrom<BattleDragonStatsTransformResolver>())
                .ForMember(dest => dest.Bodyparts, opt => opt.MapFrom<BattleDragonMobBodypartsTransformResolver>())
                .ForMember(dest => dest.IsMob, opt => opt.MapFrom(_ => true));
            // Battle
            CreateMap<BattleMasterEvtItem, BattleMasterDTO>();
            CreateMap<BattleDragonEvtItem, BattleDragonDTO>();
            CreateMap<BattleCreatedETO, CreatedBattleDTO>().ForMember(dest => dest.ExpiredAt, opt => opt.MapFrom(src => src.ExpiredAt.UtcDateTime));

            CreateMap<BattleServerRegisteredIntegrationEvent, CreatedBattleDTO>().ForMember(dest => dest.ExpiredAt, opt => opt.MapFrom(src => src.ExpiredAt.UtcDateTime));
            CreateMap<NewMailCreatedIntegrationEto, MailDto>();
            CreateMap<ClaimableItemEto, ClaimableItemDto>();
        }
    }
    public class DragonMaxExpTransformResolver : IValueResolver<IDragonBase, DragonDTO, UInt64?>
    {
        private readonly GameConfigs gameConfigs;
        public DragonMaxExpTransformResolver(ConfigLoader gameCfgLoader)
        {
            this.gameConfigs = gameCfgLoader.GameConfigs;
        }
        public ulong? Resolve(IDragonBase source, DragonDTO destination, ulong? destMember, ResolutionContext context)
        {
            var levelConfig = gameConfigs.LevelExpConfig.EntityLevelConfigMap[Dragon.Blueprints.LevelTypeEnum.Dragon.Code];
            return (ulong)levelConfig.GetExpConfigAtLevel(source.Level).MaxExp;
        }
    }
    public class MasterMaxExpTransformResolver : IValueResolver<MasterDAO, MasterDTO, UInt64?>
    {
        private readonly GameConfigs gameConfigs;
        public MasterMaxExpTransformResolver(ConfigLoader gameCfgLoader)
        {
            this.gameConfigs = gameCfgLoader.GameConfigs;
        }
        public ulong? Resolve(MasterDAO source, MasterDTO destination, ulong? destMember, ResolutionContext context)
        {
            var levelConfig = gameConfigs.LevelExpConfig.EntityLevelConfigMap[Dragon.Blueprints.LevelTypeEnum.Master.Code];
            return (ulong)levelConfig.GetExpConfigAtLevel(source.Level).MaxExp;
        }
    }





    public class BattleDragonStatsTransformResolver : IValueResolver<IDragonBase, BattleDragonEvtItem, StatsDto>
    {
        private readonly GameConfigs gameConfigs;
        public BattleDragonStatsTransformResolver(ConfigLoader gameCfgLoader)
        {
            this.gameConfigs = gameCfgLoader.GameConfigs;
        }
        public StatsDto Resolve(IDragonBase source, BattleDragonEvtItem destination, StatsDto destMember, ResolutionContext context)
        {
            var mob = source as DragonMob;
            if (mob != null)
            {
                if (mob.IsBossMob)
                {
                    var fullStats = FullStats.FromBaseStats(mob.Stats * mob.PowerRate, gameConfigs, source.Level);
                    destMember = context.Mapper.Map<FullStats, StatsDto>(mob.Stats);
                    destMember.Heatlh = (ulong)(gameConfigs.StatCoef.HealthPerHp * destMember.HP);
                    destMember.Damage = (ulong)gameConfigs.StatCoef.SkillCoef * destMember.Skill;
                }
                else
                {
                    var stats = StatsCalculator.BuildDragonStats(source, gameConfigs) * mob.PowerRate;
                    var fullStats = FullStats.FromBaseStats(stats, gameConfigs, source.Level);
                    destMember = context.Mapper.Map<FullStats, StatsDto>(fullStats);
                }
            }
            else
            {
                var stats = StatsCalculator.BuildDragonStats(source, gameConfigs);
                var fullStats = FullStats.FromBaseStats(stats, gameConfigs, source.Level);
                destMember = context.Mapper.Map<FullStats, StatsDto>(fullStats);
            }
            return destMember;
        }
    }
    public class BattleDragonBodypartsTransformResolver : IValueResolver<IDragonBase, BattleDragonEvtItem, List<Bodypart>>
    {
        protected readonly GameConfigs gameConfigs;
        public BattleDragonBodypartsTransformResolver(ConfigLoader gameCfgLoader)
        {
            this.gameConfigs = gameCfgLoader.GameConfigs;
        }
        public virtual List<Bodypart> Resolve(IDragonBase source, BattleDragonEvtItem destination, List<Bodypart> destMember, ResolutionContext context)
        {
            if (source.Bodyparts == null && source.Bodyparts.Count == 0)
                return null;
            var validBodyParts = new List<Bodypart>();
            foreach (var bp in source.Bodyparts.FindAll(bp => bp.Unlocked == true))
            {
                if (bp.Type == Enums.BodypartType.Wing || bp.Type == Enums.BodypartType.Eye || bp.Type == Enums.BodypartType.Ultimate)
                {
                    validBodyParts.Add(bp);
                }
                else
                {
                    if (source.Level >= gameConfigs.BodypartConfigMap[bp.Type].RequiredLevel)
                        validBodyParts.Add(bp);
                }
            }
            return validBodyParts;
        }
    }

    public class BattleDragonMobBodypartsTransformResolver : BattleDragonBodypartsTransformResolver
    {
        public BattleDragonMobBodypartsTransformResolver(ConfigLoader gameCfgLoader)
        : base(gameCfgLoader)
        {
        }
        public override List<Bodypart> Resolve(IDragonBase source, BattleDragonEvtItem destination, List<Bodypart> destMember, ResolutionContext context)
        {
            if (source.Bodyparts == null && source.Bodyparts.Count == 0)
                return null;
            var validBodyParts = new List<Bodypart>();
            foreach (var bp in source.Bodyparts)
            {
                if (bp.Type == Enums.BodypartType.Wing || bp.Type == Enums.BodypartType.Eye || bp.Type == Enums.BodypartType.Ultimate)
                {
                    validBodyParts.Add(bp);
                }
                else
                {
                    if (source.Level >= gameConfigs.BodypartConfigMap[bp.Type].RequiredLevel)
                        validBodyParts.Add(bp);
                }
            }
            return validBodyParts;
        }
    }
    // public class InventoryItemDataTransformResolver : IValueResolver<InventoryItemDAO, InventoryItemDetailDTO, object>
    // {
    //     public object Resolve(InventoryItemDAO source, InventoryItemDetailDTO destination, object destMember, ResolutionContext context)
    //     {
    //         return source.Type switch
    //         {
    //             ItemType.TreasureKey => getTransformDataType<ItemTreasureKey, TreasureKeyDto>(source.Data, context),
    //             _ => null,
    //         };
    //     }
    //     private object getTransformDataType<TSource, TDTO>(BsonDocument document, ResolutionContext context)
    //     {
    //         var data = BsonSerializer.Deserialize<TSource>(document);
    //         return context.Mapper.Map<TSource, TDTO>(data);
    //     }
    // }

}