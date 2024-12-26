using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using System.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Newtonsoft.Json;
using DragonAPI.Enums;
using DragonAPI.Configurations;
using DragonAPI.Models.Entities;
// using Dragon.Blueprints;
using DragonAPI.Models.DTOs;
using DragonAPI.Extensions;
using DragonAPI.Models.Settings;
using static DragonAPI.Common.Dragon;
// using Dragon.Blueprints;

namespace DragonAPI.Services
{
    public class ConfigLoader
    {
        public GameConfigs GameConfigs { get; set; }
        public OffChainConfigs OffChainConfigs { get; set; }
        private readonly string blueprintsUrl;
        private readonly ILogger<ConfigLoader> logger;
        private readonly IMapper mapper;
        private DateTime? lastTimeLoadConfig = null;
        public DragonCalculatorConfigs CalculatorConfigs { get; set; }
        public ConfigLoader(IConfiguration configuration, IMapper mapper, ILogger<ConfigLoader> logger)
        {
            GameConfigs = new GameConfigs();
            OffChainConfigs = new OffChainConfigs();
            CalculatorConfigs = new DragonCalculatorConfigs();
            this.logger = logger;
            this.blueprintsUrl = configuration.GetSection("BlueprintsUrl").Get<string>();
            GameConfigs.BattleServerConfig = configuration.GetSection("BattleServerConfig").Get<BattleServerConfig>();
            this.mapper = mapper;

        }
        // private async Task<string> GetVersionAsync(string url)
        // {
        //     var httpClient = new HttpClient();
        //     var response = await httpClient.GetAsync(url);
        //     var json = await response.Content.ReadAsStringAsync();
        //     ApiResultDTO<string> data = json<ApiResultDTO<string>>(json);
        //     return data.Data;
        // }

        private static async Task<T> GetConfigFromUrl<T>(string url)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var data = await response.Content.ReadAsStringAsync();
            // System.Console.WriteLine($"{url} data\n{data}");
            return JsonConvert.DeserializeObject<T>(data);
        }
        private string buildUrl(string path)
        {
            return $"{blueprintsUrl}/{path}";
        }
        public async Task Reload()
        {
            var now = DateTime.UtcNow;
            if (lastTimeLoadConfig == null || (now - (DateTime)lastTimeLoadConfig).TotalSeconds > 30 && lastTimeLoadConfig != null)
            {
                lastTimeLoadConfig = now;
                logger.LogInformation($"============= [Config loader] Begin load config ");

                List<Task> tasks = new List<Task>();
                tasks.Add(loadDragonUpgradeCfg());
                tasks.Add(loadDragonMergeFragmentCfg());
                tasks.Add(loadOfflineRewardCfg());
                tasks.Add(loadLevelCfg());
                tasks.Add(loadDragonBaseStatsCfg());
                tasks.Add(loadBodypartStatsCfg());
                tasks.Add(loadBodypartLevelCfg());
                tasks.Add(loadBattleConfig());
                tasks.Add(loadDragonElements());
                tasks.Add(loadMobs());
                tasks.Add(loadSkillConfig());
                tasks.Add(loadSeasonConfig());

                tasks.Add(loadCommonSettingCfg());
                tasks.Add(loadRateConvertItemCfg());
                tasks.Add(loadDragonFarmLevelCfg());
                tasks.Add(loadShopPackageConfig());
                tasks.Add(loadDragonUpgradeCfg());
                tasks.Add(loadSeasonRankingRewardConfig());
                tasks.Add(loadDailyRankingRewardConfig());
                await Task.WhenAll(tasks);
                logger.LogInformation($"============= [Config loader] End load config");
                reconfigureDragonCalculator();
            }
        }
        private void reconfigureDragonCalculator()
        {
            logger.LogInformation($"starting reconfigureDragonCalculator");
            CalculatorConfigs.StatCoef = mapper.Map<StatCoef, DragonAPI.Common.StatCoef>(GameConfigs.StatCoef);
            CalculatorConfigs.ClassGroupMap = mapper.Map<Dictionary<ClassType, ClassType>, Dictionary<DragonAPI.Enums.ClassType, DragonAPI.Enums.ClassType>>(GameConfigs.ClassGroupMap);
            CalculatorConfigs.SetSkillLevelConfigs(mapper.Map<List<Models.Settings.BuffSkillConfig>, List<DragonAPI.Common.Dragon.SkillConfig>>(GameConfigs.SkillConfigs));
            CalculatorConfigs.SetSealSocketPowerLevelConfigs();
            logger.LogInformation($"ending reconfigureDragonCalculator");
        }

        #region Convert Enums
        private ClassType mapToInternalClass(string element)
        {
            ClassType _class = ClassType.Undefined;
            Enum.TryParse(element, out _class);
            return _class;
        }

        private ClassGrouping mapToInternalClassGrouping(string element)
        {
            ClassGrouping _class = ClassGrouping.Gold;
            Enum.TryParse(element, out _class);
            return _class;
        }

        private SealSlotPowerType mapToInternalSealSlotPower(string power)
        {
            SealSlotPowerType powerType = SealSlotPowerType.Attack;
            Enum.TryParse(power, out powerType);
            return powerType;
        }
        private EnvironmentEffectName mapToEnvironmentEffectName(string name)
        {
            return name switch
            {
                nameof(EnvironmentEffectName.BuffAllStats) => EnvironmentEffectName.BuffAllStats,
                nameof(EnvironmentEffectName.DebuffAllStats) => EnvironmentEffectName.DebuffAllStats,
                nameof(EnvironmentEffectName.HPStats) => EnvironmentEffectName.HPStats,
                nameof(EnvironmentEffectName.DebuffHpStats) => EnvironmentEffectName.DebuffHpStats,
                nameof(EnvironmentEffectName.BuffSkillStats) => EnvironmentEffectName.BuffSkillStats,
                nameof(EnvironmentEffectName.DebuffSkillStats) => EnvironmentEffectName.DebuffSkillStats,

                _ => throw new Exception(),
            };
        }
        private Enums.BodypartType mapToInternalBodypart(string bp)
        {
            Enums.BodypartType bodypartType = Enums.BodypartType.Claw;
            Enum.TryParse(bp, out bodypartType);
            return bodypartType;
        }
        private Enums.Gender mapToInternalGender(string gender)
        {
            return gender switch
            {
                nameof(Enums.Gender.Female) => Enums.Gender.Female,
                nameof(Enums.Gender.Male) => Enums.Gender.Male,
                _ => throw new Exception(),
            };
        }

        private BossSkillName mapToBossSkillName(string name)
        {
            return name switch
            {
                nameof(BossSkillName.Kuberaos) => BossSkillName.Kuberaos,
                nameof(BossSkillName.Aranyanios) => BossSkillName.Aranyanios,
                nameof(BossSkillName.Varunaos) => BossSkillName.Varunaos,
                nameof(BossSkillName.Agnios) => BossSkillName.Agnios,
                nameof(BossSkillName.Bhumios) => BossSkillName.Bhumios,
                nameof(BossSkillName.Kalios) => BossSkillName.Kalios,
                nameof(BossSkillName.Suryaos) => BossSkillName.Suryaos,
                _ => throw new Exception(),
            };
        }
        private Enums.Family mapToFamily(string name)
        {
            Enums.Family family = Enums.Family.Dragon;
            Enum.TryParse(name, out family);
            return family;
        }

        private ClassType[] mapToClassTypeArray(string input)
        {
            input = input.Trim();
            string[] temp = input.Split(';');
            ClassType[] types = new ClassType[temp.Length];
            int index = 0;
            foreach (string type in temp)
            {
                types[index] = mapToInternalClass(type);
                index++;
            }
            return types;
        }
        private float[] ConvertStringToFloatArr(string input)
        {

            input = input.Trim();
            string[] temp = input.Split(';');
            if (temp.Length > 0)
            {
                float[] data = new float[temp.Length];
                for (int i = 0; i < temp.Length; i++)
                {
                    data[i] = float.Parse(temp[i]);
                }
                return data;
            }
            return null;
        }
        private int[] ConvertStringToIntArr(string input)
        {

            input = input.Trim();
            string[] temp = input.Split(';');
            if (temp.Length > 0)
            {
                int[] data = new int[temp.Length];
                for (int i = 0; i < temp.Length; i++)
                {
                    data[i] = int.Parse(temp[i]);
                }
                return data;
            }
            return null;
        }
        #endregion

        #region OnChain

        private async Task loadSeasonConfig()
        {
            var configuredSeasonList = await GetConfigFromUrl<List<Dragon.Blueprints.Season>>(buildUrl("get-season"));
            List<SeasonConfig> seasons = new List<SeasonConfig>();
            foreach (var record in configuredSeasonList)
            {
                seasons.Add(new SeasonConfig()
                {
                    Id = record.Id.ToString(),
                    EndingTime = record.EndAt,
                    StartingTime = record.StartAt,
                });
            }
            GameConfigs.SeasonConfigs = seasons;
            logger.LogInformation("-- SeasonConfigs loaded");
        }

        private async Task loadSkillConfig()
        {
            var configuredDragonSkillEffects = await GetConfigFromUrl<List<Dragon.Blueprints.DragonSkillEffect>>(buildUrl("get-rongos-skill-effect"));
            var configuredDragonSkills = await GetConfigFromUrl<List<Dragon.Blueprints.DragonSkillConfig>>(buildUrl("get-rongos-skill-config"));
            var skillConfigs = new List<Models.Settings.BuffSkillConfig>();
            var _class = new string[]
            {
                Dragon.Blueprints.DragonElementEnum.GOLD.Code,
                Dragon.Blueprints.DragonElementEnum.DARK.Code,
                Dragon.Blueprints.DragonElementEnum.LIGHT.Code,
                Dragon.Blueprints.DragonElementEnum.LIGHT2.Code,
                Dragon.Blueprints.DragonElementEnum.LIGHT3.Code,
            };
            var buffSkillTypes = new List<string>
            {
                Dragon.Blueprints.SkillEffectTypeEnum.BuffHP.Code,
                Dragon.Blueprints.SkillEffectTypeEnum.BuffSkill.Code,
                Dragon.Blueprints.SkillEffectTypeEnum.BuffSpeed.Code,
                Dragon.Blueprints.SkillEffectTypeEnum.BuffMorale.Code,
            };
            var allClassGroups = new ClassType[] {
                ClassType.Gold,
                ClassType.Wood,
                ClassType.Water,
                ClassType.Fire,
                ClassType.Earth,
                ClassType.Dark,
                ClassType.Light,
            };
            var affectedClassMap = new Dictionary<ClassType, ClassType[]>()
            {
                {ClassType.Gold, new ClassType[]{ClassType.Gold }},
                {ClassType.Light, allClassGroups},
                {ClassType.Light2, allClassGroups},
                {ClassType.Light3, allClassGroups},
                {ClassType.Dark, allClassGroups},
            };
            foreach (var record in configuredDragonSkills.FindAll(c => c.Family.Code == Dragon.Blueprints.FamilyEnum.RONGOS.Code
                                                                        && c.BodypartType.Code == Dragon.Blueprints.BodypartTypeEnum.SKIN.Code
                                                                        && _class.Contains(c.DragonElement.Code)))
            {
                var effectIdsByLevels = new string[] { record.Level1, record.Level2, record.Level3, record.Level4, };
                for (int i = 0; i < effectIdsByLevels.Length; ++i)
                {
                    var skillConfigLevel = new Models.Settings.BuffSkillConfig();
                    skillConfigLevel.Class = mapToInternalClass(record.DragonElement.Code);
                    skillConfigLevel.Level = i + 1;
                    var effs = configuredDragonSkillEffects.FindAll(c => effectIdsByLevels[i].Split(";").ToList().Contains(c.Id.ToString()));
                    effs.ForEach(eff =>
                    {
                        if (buffSkillTypes.Contains(eff.SkillEffectType.Code))
                        {
                            skillConfigLevel.ListOfMutantBuffStats.Add(buildBuffStatsForBuffSkill(eff.SkillEffectType.Code, eff.Value.Split(";").Select(i => Convert.ToDouble(i)).ToArray()));
                        };
                    });
                    skillConfigLevel.AfftectedClassGroups = affectedClassMap[mapToInternalClass(record.DragonElement.Code)];
                    skillConfigs.Add(skillConfigLevel);
                }
            }
            GameConfigs.SkillConfigs = skillConfigs;
            logger.LogDebug("-- loadSkillConfig loaded");
        }

        private List<BuffStats> buildBuffStatsForBuffSkill(string buffSkillType, double[] mutantValues)
        {
            var mutantBuffs = new List<BuffStats>();
            foreach (var value in mutantValues)
            {
                var buffstats = new BuffStats();
                if (buffSkillType == Dragon.Blueprints.SkillEffectTypeEnum.BuffHP.Code)
                    buffstats.HP = value;
                if (buffSkillType == Dragon.Blueprints.SkillEffectTypeEnum.BuffSkill.Code)
                    buffstats.Skill = value;
                if (buffSkillType == Dragon.Blueprints.SkillEffectTypeEnum.BuffSpeed.Code)
                    buffstats.Speed = value;
                if (buffSkillType == Dragon.Blueprints.SkillEffectTypeEnum.BuffMorale.Code)
                    buffstats.Morale = value;
                mutantBuffs.Add(buffstats);
            }
            return mutantBuffs;
        }
        private async Task loadBattleConfig()
        {
            var battleConfig = (await GetConfigFromUrl<List<Dragon.Blueprints.BattleConfig>>(buildUrl("get-battle-config"))).First();

            var statsCoef = new StatCoef();
            statsCoef.SkillCoef = (double)battleConfig.SkillCoef;
            statsCoef.HealthPerHp = (ushort)battleConfig.HealthPerHp;
            statsCoef.CritRatePerMorale = (double)battleConfig.CritRatePerMorale;
            statsCoef.CritDamageBase = (ushort)battleConfig.CritDamageBase;
            statsCoef.ExtraCritDamageUnit = (double)battleConfig.ExtraCritDamageUnit;
            statsCoef.NumOfMoraleExtraCritDamage = (ushort)battleConfig.NumOfMoraleExtraCritDamage;

            GameConfigs.StatCoef = statsCoef;

            GameConfigs.LevelExpConfig.ExpPerEnergy = (int)battleConfig.ExpPerEnergy;

            logger.LogInformation("Loaded loadBattleConfig");
            List<StatsCoef> statsCoefs = new List<StatsCoef>();
            var statsCoefCfgs = await GetConfigFromUrl<List<Dragon.Blueprints.StatsCoefConfig>>(buildUrl("get-statscoef-config"));
            foreach (var cfg in statsCoefCfgs)
            {
                statsCoefs.Add(new StatsCoef
                {
                    Level = (int)cfg.Level,
                    HealthPerHp = (double)cfg.HealthPerHp,
                    SkillCoef = (double)cfg.SkillCoef,
                });
            }
            GameConfigs.StatsCoefConfigs = statsCoefs;
            logger.LogInformation("Loaded loadBattleConfig");
        }
        private async Task loadLevelCfg()
        {
            var configuredLevelList = await GetConfigFromUrl<List<Dragon.Blueprints.LevelConfig>>(buildUrl("get-level-config"));
            var levelConfigMap = new Dictionary<string, LevelExpConfiguration.LevelConfig>();
            configuredLevelList.GroupBy(cfg => cfg.LevelType.Code).ToList().ForEach(cfgGroupLevels =>
            {
                var levelConfig = new LevelExpConfiguration.LevelConfig
                {
                    MaxLevel = cfgGroupLevels.Max(lc => lc.Level),
                    Exps = new List<LevelExpConfiguration.ExpConfig>(),
                };
                cfgGroupLevels.ToList().ForEach(lc =>
                {
                    levelConfig.Exps.Add(new LevelExpConfiguration.ExpConfig
                    {
                        Level = lc.Level,
                        MinExp = lc.MinValue,
                        MaxExp = lc.MaxValue
                    });
                });
                levelConfigMap.Add(cfgGroupLevels.Key, levelConfig);
            });
            GameConfigs.LevelExpConfig.EntityLevelConfigMap = levelConfigMap;
            logger.LogInformation("loadLevelCfg loaded");
        }
        private async Task loadDragonUpLevelCfg()
        {
            var configuredLevelList = await GetConfigFromUrl<List<Dragon.Blueprints.DragonUplevel>>(buildUrl("get-rongos-level-up"));
            var lstDragonLevelConfig = new List<DragonUplevel>();
            configuredLevelList.ForEach(ro =>
            {
                lstDragonLevelConfig.Add(new DragonUplevel
                {
                    Id = ro.Id,
                    DragonElementId = ro.DragonElementId,
                    Rss = (long)ro.Rss,
                    DragonExp = ro.DragonExp,
                    NextLevelUpdate = ro.NextLevelUpdate,
                    CoeffUpdate = ro.CoeffUpdate,
                    AmountDust = ro.AmountDust,
                    AmountScroll = ro.AmountScroll,
                    BodyPartLevelUpdate = ro.BodyPartLevelUpdate,
                    BodyPartTypeUpdate = ro.BodyPartTypeUpdate,
                    DragonElement = ro.DragonElement

                });
            });
            GameConfigs.DragonUplevelConfigs = lstDragonLevelConfig;
            logger.LogInformation("loadDragonUpLevelCfg loaded");
        }
        private async Task loadDragonUpgradeCfg()
        {
            var configuredLevelList = await GetConfigFromUrl<List<Dragon.Blueprints.DragonUpgrade>>(buildUrl("get-rongos-level-upgrade"));
            var lstDragonLevelConfig = new List<DragonUpgrade>();
            configuredLevelList.ForEach(ro =>
            {
                lstDragonLevelConfig.Add(new DragonUpgrade
                {
                    Id = ro.Id,
                    DragonElementId = ro.DragonElementId,
                    AmountItem1 = ro.AmountItem1,
                    ItemId1 = ro.ItemId1,
                    NextLevelUpdate = ro.NextLevelUpdate,
                    ItemId2 = ro.ItemId2,
                    AmountItem2 = ro.AmountItem2,
                    ItemId3 = ro.ItemId3,
                    AmountItem3 = ro.AmountItem3,
                    BodyPartLevelUpdate = ro.BodyPartLevelUpdate,
                    BodyPartTypeUpdate = ro.BodyPartTypeUpdate,
                    DragonElement = ro.DragonElement

                });
            });
            GameConfigs.DragonUpgradeConfigs = lstDragonLevelConfig;
            logger.LogInformation("loadDragonUpgradeCfg loaded");
        }
        private async Task loadDragonMergeFragmentCfg()
        {
            var configuredLevelList = await GetConfigFromUrl<List<Dragon.Blueprints.DragonMergeFragmentConfig>>(buildUrl("get-rongos-merge-fragment"));
            var lstDragonMergeFragmentConfig = new List<DragonMergeFragment>();
            configuredLevelList.ForEach(ro =>
            {
                lstDragonMergeFragmentConfig.Add(new DragonMergeFragment
                {
                    Id = ro.Id,
                    DragonElementId = ro.DragonElementId,
                    Rss = (long)ro.Rss,
                    ItemId = ro.ItemId,
                    ItemQuantity = ro.ItemQuantity,
                    AmountDust = ro.AmountDust,
                    AmountScroll = ro.AmountScroll,
                    DragonElement = ro.DragonElement

                });
            });
            GameConfigs.DragonMergeFragmentConfigs = lstDragonMergeFragmentConfig;
            logger.LogInformation("loadDragonMergeFragmentCfg loaded");
        }
        private async Task loadOfflineRewardCfg()
        {
            var configuredLevelList = await GetConfigFromUrl<List<Dragon.Blueprints.AfkReward>>(buildUrl("get-afk-reward"));
            var lstOfflineReward = new List<AfkReward>();
            configuredLevelList.ForEach(ro =>
            {
                lstOfflineReward.Add(new AfkReward
                {
                    Id = ro.Id,
                    StageId = ro.StageId,
                    Type = ro.Type,
                    ItemId = ro.ItemId,
                    Quantity = ro.Quantity,
                    Rate = ro.Rate,
                    MinTimeRequire = ro.MinTimeRequire,
                    MaxTimeRequire = ro.MaxTimeRequire
                });

            });
            GameConfigs.AfkRewardConfigs = lstOfflineReward;
            logger.LogInformation("loadOfflineRewardCfg loaded");
        }
        private async Task loadDragonBaseStatsCfg()
        {
            var configuredDragonBaseStatsList = await GetConfigFromUrl<List<Dragon.Blueprints.DragonBaseStats>>(buildUrl("get-dragon-base-stats"));
            var mapDragonBaseStats = new Dictionary<ClassType, DragonStatsConfig>();
            var lstDragonStatsConfig = new List<DragonStatsConfig>();
            configuredDragonBaseStatsList.ForEach(cfg =>
            {
                var classtype = mapToInternalClass(cfg.DragonElement.Code);
                // logger.LogDebug(classtype.ToString());
                mapDragonBaseStats.TryAdd(classtype, new DragonStatsConfig
                {
                    StartingStats = new Stats
                    {
                        HP = (uint)cfg.BaseHp,
                        Speed = (uint)cfg.BaseSpeed,
                        Skill = (uint)cfg.BaseSkill,
                        Morale = (uint)cfg.BaseMorale,
                        Synergy = (ushort)cfg.BaseSynergy,
                    },
                    IncreaseStatsByLevel = new Stats
                    {
                        HP = (uint)cfg.IncreaHp,
                        Speed = (uint)cfg.IncreaSpeed,
                        Skill = (uint)cfg.IncreaSkill,
                        Morale = (uint)cfg.IncreaMorale,
                        Synergy = (ushort)cfg.IncreaSynergy,
                    }
                });
                lstDragonStatsConfig.Add(new DragonStatsConfig
                {
                    Class = classtype,
                    Rarity = (int)cfg.Rarity,
                    StartingStats = new Stats
                    {
                        HP = (uint)cfg.BaseHp,
                        Speed = (uint)cfg.BaseSpeed,
                        Skill = (uint)cfg.BaseSkill,
                        Morale = (uint)cfg.BaseMorale,
                        Synergy = (ushort)cfg.BaseSynergy,
                    },
                    IncreaseStatsByLevel = new Stats
                    {
                        HP = (uint)cfg.IncreaHp,
                        Speed = (uint)cfg.IncreaSpeed,
                        Skill = (uint)cfg.IncreaSkill,
                        Morale = (uint)cfg.IncreaMorale,
                        Synergy = (ushort)cfg.IncreaSynergy,
                    }
                });
            });
            GameConfigs.DragonStatsConfigMap = mapDragonBaseStats;
            GameConfigs.DragonStatsConfigs = lstDragonStatsConfig;
            logger.LogInformation("loadDragonBaseStatsCfg loaded");
        }
        private async Task loadBodypartStatsCfg()
        {
            var configuredBodypartList = await GetConfigFromUrl<List<Dragon.Blueprints.BodypartType>>(buildUrl("get-bodypart-type"));
            var bodypartConfigMap = new Dictionary<Enums.BodypartType, BodypartTypeConfig>();
            var lstBodypartType = new List<BodypartTypeMap>();
            configuredBodypartList.ForEach(bp =>
            {
                bodypartConfigMap.TryAdd(mapToInternalBodypart(bp.Code), new BodypartTypeConfig
                {
                    RequiredLevel = (int)bp.UnlockLevel,
                    RSS = bp.Rss,
                    YNR = bp.Rocs,
                    StatsByClassMap = new Dictionary<ClassType, Stats>()
                });
                lstBodypartType.Add(new BodypartTypeMap
                {
                    Id = bp.Id,
                    Code = bp.Code,
                    UnlockLevel = bp.UnlockLevel
                });
            });
            GameConfigs.BodypartTypeMap = lstBodypartType;
            var configuredBodypartStatsList = await GetConfigFromUrl<List<Dragon.Blueprints.BodypartStats>>(buildUrl("get-bodypart-stats"));
            configuredBodypartStatsList.ForEach(bps =>
            {
                // logger.LogDebug($"{bps.BodypartType.Code}, {bps.DragonElement.Code}");
                bodypartConfigMap.TryGetValue(mapToInternalBodypart(bps.BodypartType.Code), out BodypartTypeConfig bpCfg);
                bpCfg.StatsByClassMap.TryAdd(mapToInternalClass(bps.DragonElement.Code), new Stats
                {
                    HP = (uint)bps.Hp,
                    Speed = (uint)bps.Speed,
                    Skill = (uint)bps.Skill,
                    Morale = (uint)bps.Morale,
                    Synergy = (ushort)bps.Synergy,
                });
            });
            GameConfigs.BodypartConfigMap = bodypartConfigMap;
            logger.LogInformation("loadBodypartStatsCfg loaded");
        }
        private async Task loadBodypartLevelCfg()
        {
            var configuredBodypartList = await GetConfigFromUrl<List<Dragon.Blueprints.BodypartLevel>>(buildUrl("get-bodypart-level"));
            var bodypartLevelConfigs = new List<BodypartLevelConfig>();
            configuredBodypartList.ForEach(bp =>
            {
                bodypartLevelConfigs.Add(new BodypartLevelConfig
                {
                    Level = (int)bp.Level,
                    RequiredLevel = (int)bp.RequiredLevel,
                    Rss = (long)bp.Rss,
                    Bodypart = mapToInternalBodypart(bp.BodypartType.Code),
                });
            });
            GameConfigs.BodypartLevelConfigs = bodypartLevelConfigs;
            logger.LogInformation("loadBodypartLevelCfg loaded");
        }


        private Task loadDragonElements()
        {
            var classGroupMap = new Dictionary<ClassType, ClassType>();
            Dictionary<ClassType, string> rongosDefaultName = new Dictionary<ClassType, string>();
            Dragon.Blueprints.DragonElementEnum.DragonElementEnumList.ForEach(c =>
            {
                var g = Dragon.Blueprints.DragonElementGroupingEnum.DragonElementGroupingEnumList.Find(g => g.Id.ToString() == c.Value);
                classGroupMap.TryAdd(mapToInternalClass(c.Code), mapToInternalClass(g.Code));
                rongosDefaultName.Add(mapToInternalClass(c.Code), c.Name);
            });
            GameConfigs.ClassGroupMap = classGroupMap;
            GameConfigs.DragonDefaultName = rongosDefaultName;
            logger.LogDebug("-- loadDragonElements loaded");
            return Task.CompletedTask;
        }



        #endregion

        #region OffChain




        private async Task loadMapStage()
        {
            var configuredMaps = await GetConfigFromUrl<List<Dragon.Blueprints.Map>>(buildUrl("get-map"));
            var maps = new List<MapStageConfig>();
            foreach (var map in configuredMaps)
            {
                maps.Add(new MapStageConfig
                {
                    Id = map.Id.ToString(),
                    Index = (int)map.OrderNumber,
                    Name = map.Name,
                    RequiredLevel = (int)map.RequiredLevel,
                });
            }
            GameConfigs.WorldMapConfig.Maps = maps;
            logger.LogInformation("Maps loaded");

            var configuredItems = await GetConfigFromUrl<List<Dragon.Blueprints.Item>>(buildUrl("get-item"));
            var items = new List<ItemConfig>();
            foreach (var item in configuredItems)
            {
                items.Add(new ItemConfig
                {
                    Id = item.Id,
                    TypeId = item.TypeId,
                    Name = item.Name,
                    Code = item.Code != null ? item.Code : "",
                    EntityType = item.EntityType,
                    IsNFT = item.IsNFT,
                    Url = item.Url,
                    Description = item.Description
                });
            }
            GameConfigs.ItemConfigs = items;
            logger.LogInformation("item loaded");

            var configuredStages = await GetConfigFromUrl<List<Dragon.Blueprints.Stage>>(buildUrl("get-stage"));
            var stages = new List<MapStageConfig>();
            foreach (var stage in configuredStages)
            {
                stages.Add(new MapStageConfig
                {
                    Id = stage.Id.ToString(),
                    ParentId = stage.MapId.ToString(),
                    Index = (int)stage.OrderNumber,
                    RequiredLevel = (int)stage.RequiredLevel,
                    MobCount = (int)stage.MobCount,
                    Energy = (int)stage.Energy,
                    ExpPerMob = (int)stage.ExpPerMob,
                    RssPerMob = (int)stage.RSSPerMob,
                    LastStage = stage.IsLastStage
                });
                logger.LogDebug($"loaded stage {stage.Id}, map {stage.Map.Name}");
            }
            GameConfigs.WorldMapConfig.Stages = stages;
            logger.LogInformation("Stages loaded");


            var configuredStageFormations = await GetConfigFromUrl<List<Dragon.Blueprints.MobStageFormation>>(buildUrl("get-mob-stage-formation"));
            var stageFormations = new List<StageFormationsConfig>();
            foreach (var record in configuredStageFormations)
            {
                var formation = new StageFormationsConfig
                {
                    FormationId = record.Id.ToString(),
                    StageId = record.StageId.ToString(),
                    Time = (int?)record.Time,
                    MobPowerRate = (double)record.MobPowerRate,
                    DropItemIds = new List<string>(),
                    DropItems = new List<DropItemConfig>(),
                    DropItemRate = (double)record.KeyRate,
                    MobIds = record.MobStageFormationMobMappings.OrderBy(m => m.MobId).Select(c => c.MobId.ToString()).ToArray(),
                    Mobs = new List<MobInStageFormation>()
                };
                record.MobStageFormationMobMappings.ForEach(mm =>
                {
                    var mob = GameConfigs.Mobs.FirstOrDefault(m => m.Id == mm.MobId.ToString());
                    if (mob != null)
                        formation.Mobs.Add(new MobInStageFormation
                        {
                            Id = mob.Id,
                            Class = mob.Class,
                            Level = mob.Level,
                            Bodyparts = mob.Bodyparts,
                            Stats = mob.Stats,
                            Family = mob.Family,
                            IndexInFormation = mm.SlotInFormation,

                        });
                });
                if (record.MobStageFormationItemContents.Count > 0)
                {
                    formation.DropItemIds = record.MobStageFormationItemContents.Select(c => c.ItemId.ToString()).ToList();
                    foreach (var dropItem in record.MobStageFormationItemContents)
                    {
                        var item = GameConfigs.ItemConfigs.FirstOrDefault(i => i.Id == dropItem.ItemId);
                        if (item != null)
                        {

                            formation.DropItems.Add(new DropItemConfig
                            {
                                Item = new TemplateItemConfig
                                {
                                    Id = item.Id,
                                    Entity = item.EntityType,
                                    TypeId = item.TypeId,
                                    IsNFT = item.IsNFT,
                                    ImageURL = item.Url,
                                    // SealSlot = (int?)item.SealSlot,
                                },
                                Quantity = (int)dropItem.Quantity,
                            });
                        }


                    }





                }
                stageFormations.Add(formation);
            }
            GameConfigs.WorldMapConfig.StageFormations = stageFormations;
            logger.LogInformation("StageMobs formation loaded");


            var configuredLastStageOpeningTimes = await GetConfigFromUrl<List<Dragon.Blueprints.StageOpenTime>>(buildUrl("get-stage-open-time"));
            var stageOpenTimeList = new List<StageOpenTimeConfig>();
            foreach (var record in configuredLastStageOpeningTimes)
            {
                stageOpenTimeList.Add(new StageOpenTimeConfig
                {
                    StageId = record.StageId.ToString(),
                    Time = (int)record.Time,
                    Total = (int)record.Amount,
                });
            }
            GameConfigs.WorldMapConfig.ListLastStageOpenTime = stageOpenTimeList;

            logger.LogInformation("stage-open-time loaded");

        }
        private async Task loadPVPFormations()
        {
            var configuredBotPVPFormations = await GetConfigFromUrl<List<Dragon.Blueprints.TeamPvpMobFormation>>(buildUrl("get-team-pvp-formation"));
            var teamPvpMobs = new List<TeamPVPMobsConfig>();
            foreach (var map in configuredBotPVPFormations)
            {
                TeamPVPMobsConfig config = new TeamPVPMobsConfig()
                {
                    TeamPVPId = map.Id.ToString(),
                    MobPowerRate = (double)map.MobPowerRate,
                    TierRank = map.RankPvP,
                    Name = map.Name
                };
                string[] mobs = map.TeamPvpMobFormationMobMappings.Select(c => c.MobId.ToString()).ToArray();
                map.TeamPvpMobFormationMobMappings.ForEach(mm =>
                {
                    var mob = GameConfigs.Mobs.FirstOrDefault(m => m.Id == mm.MobId.ToString());
                    if (mob != null)
                        config.Mobs.Add(new MobInStageFormation
                        {
                            Id = mob.Id,
                            Class = mob.Class,
                            Level = mob.Level,
                            Bodyparts = mob.Bodyparts,
                            Stats = mob.Stats,
                            Family = mob.Family,
                            IndexInFormation = mm.SlotInFormation,

                        });
                });
                config.DragonMobIds = mobs;
                teamPvpMobs.Add(config);
            }
            GameConfigs.TeamPVPMobConfigs = teamPvpMobs;
            logger.LogInformation("-- TeamPVPMobConfigs loaded");
        }
        private async Task loadMobs()
        {
            var configuredMobs = await GetConfigFromUrl<List<Dragon.Blueprints.Mob>>(buildUrl("get-mob"));
            var dragonMobs = new List<Mob>();
            foreach (var mobCfg in configuredMobs)
            {
                var mob = new Mob()
                {
                    Id = mobCfg.Id.ToString(),
                    //Gender = mapToInternalGender(mobCfg.Gender.Code),
                    Class = mapToInternalClass(mobCfg.DragonElement.Code),
                    Level = (ushort)mobCfg.Level,
                    Bodyparts = new List<Bodypart>(),
                    Family = mapToFamily(mobCfg.Family.Code),

                    IsBossMob = mobCfg.IsBoss,
                };
                #region load bodyparts
                foreach (var bpCfg in mobCfg.MobBodypartTypeMappings)
                {
                    var bpType = mapToInternalBodypart(bpCfg.BodypartType.Code);
                    if (bpType == BodypartType.Ultimate) continue;
                    var _class = ClassType.Undefined;
                    var _classId = (long)ClassType.Undefined;
                    if (bpType == BodypartType.Eye)
                    {
                        _classId = (long)mobCfg.EyeId;
                    }
                    else if (bpType == BodypartType.Wing)
                    {
                        _classId = (long)mobCfg.WingId;
                    }
                    else
                    {
                        _class = mapToInternalClass(bpCfg.DragonElement.Code);
                        _classId = (long)mapToInternalClass(bpCfg.DragonElement.Code);
                    }
                    mob.Bodyparts.Add(new Bodypart
                    {
                        Class = (long)_class,
                        Type = bpType,
                        MutantType = MutantType.Normal,
                        Unlocked = true,
                        Level = (int)bpCfg.Level
                    });

                }
                #endregion

                if (mobCfg.IsBoss)
                {
                    mob.Stats = new FullStats
                    {
                        HP = (uint)mobCfg.Hp,
                        Speed = (uint)mobCfg.Speed,
                        Skill = (uint)mobCfg.Skill,
                        Morale = (uint)mobCfg.Morale,
                        CritRate = (uint)mobCfg.CritRate,
                        CritDamage = (uint)mobCfg.CritDamage,
                    };
                }
                dragonMobs.Add(mob);
            }
            GameConfigs.Mobs = dragonMobs;
            logger.LogInformation("loadMobs loaded");
            await loadMapStage();
            await loadPVPFormations();
        }


        private async Task loadRateConvertItemCfg()
        {
            var configuredLevelList = await GetConfigFromUrl<List<Dragon.Blueprints.RateConvertItemConfig>>(buildUrl("get-rate-convert-item"));
            var lstRateConvertItemConfig = new List<RateConvertItem>();
            configuredLevelList.ForEach(ro =>
            {
                lstRateConvertItemConfig.Add(new RateConvertItem
                {
                    Id = ro.Id,
                    ExchangeItem = ro.ExchangeItem,
                    ReceivedItem = ro.ReceivedItem,
                    Rate = ro.Rate

                });
            });
            GameConfigs.ChangeRateItem = lstRateConvertItemConfig;
            logger.LogInformation("loadDragonUpLevelCfg loaded");
        }
        private async Task loadDragonFarmLevelCfg()
        {
            var configuredLevelList = await GetConfigFromUrl<List<Dragon.Blueprints.DragonFarmLevelConfig>>(buildUrl("get-rongos-farm-level-up"));
            var lstDragonFarmLevelConfig = new List<DragonFarmLevel>();
            configuredLevelList.ForEach(ro =>
            {
                lstDragonFarmLevelConfig.Add(new DragonFarmLevel
                {
                    Id = ro.Id,
                    DragonElementId = ro.DragonElementId,
                    Level = ro.Level,
                    ItemId = ro.ItemId,
                    ItemQuantity = ro.ItemQuantity

                });
            });
            GameConfigs.DragonFarmLevel = lstDragonFarmLevelConfig;
            logger.LogInformation("loadDragonUpFarmLevelCfg loaded");
            var configuredFarmStatsList = await GetConfigFromUrl<List<Dragon.Blueprints.DragonFarmLevelStatConfig>>(buildUrl("get-rongos-farm-level-stats"));
            var lstDragonFarmLevelStatsConfig = new List<DragonFarmLevelStat>();
            configuredFarmStatsList.ForEach(ro =>
            {
                lstDragonFarmLevelStatsConfig.Add(new DragonFarmLevelStat
                {
                    Id = ro.Id,
                    LevelDragonReq = ro.LevelDragonReq,
                    Level = ro.Level,
                    FarmRateBonus = ro.FarmRateBonus,
                    UpFailRateBonus = ro.UpFailRateBonus,
                    TotalUpFail = ro.TotalUpFail,
                    SuccessRate = ro.SuccessRate

                });
            });
            GameConfigs.DragonFarmLevelStat = lstDragonFarmLevelStatsConfig;
            logger.LogInformation("loadDragonFarmLevelStatsCfg loaded");
        }
        private async Task loadDragonUpFarmLevelRateCfg()
        {
            var configuredLevelList = await GetConfigFromUrl<List<Dragon.Blueprints.UpgradeRateDragonFarmLevelConfig>>(buildUrl("get-rongos-farm-level-up-rate"));
            var lstConfig = new List<UpgradeRateDragonFarmLevel>();
            configuredLevelList.ForEach(ro =>
            {
                lstConfig.Add(new UpgradeRateDragonFarmLevel
                {
                    Id = ro.Id,
                    Level = ro.Level,
                    Rate = ro.Rate,
                    Description = ro.Description


                });
            });
            GameConfigs.UpgradeRateDragonFarmLevel = lstConfig;
            logger.LogInformation("loadDragonUpFarmLevelCfg loaded");
        }
        private async Task loadCommonSettingCfg()
        {
            var configuredCommonSetting = await GetConfigFromUrl<List<Dragon.Blueprints.CommonSetting>>(buildUrl("get-common-setting"));
            var lstCommonSetting = new List<CommonSetting>();
            configuredCommonSetting.ForEach(ro =>
            {
                lstCommonSetting.Add(new CommonSetting
                {
                    Id = ro.Id,
                    Key = ro.Key,
                    Value = ro.Value,
                    Status = ro.Status

                });
            });
            GameConfigs.CommonSetting = lstCommonSetting;
            logger.LogInformation("loadCommonCfg loaded");
        }
        private async Task loadShopPackageConfig()
        {
            var configuredCommonSetting = await GetConfigFromUrl<List<Dragon.Blueprints.ShopPackageConfig>>(buildUrl("get-shop-package-config"));
            var lstPackage = new List<ShopPackageConfig>();
            configuredCommonSetting.ForEach(ro =>
            {
                lstPackage.Add(new ShopPackageConfig
                {
                    Id = ro.Id,
                    ItemId = ro.ItemId,
                    Quantity = ro.Quantity,
                    BasePrice = ro.BasePrice,
                    FinalPrice = ro.FinalPrice,
                    Discount = ro.Discount,
                    CurrencyId = ro.CurrencyId,
                    Status = ro.Status

                });
            });
            GameConfigs.ShopPackageConfig = lstPackage;
            logger.LogInformation("loadPackageCfg loaded");
        }
        private async Task loadDragonOpenPackageConfig()
        {
            var configuredCommonSetting = await GetConfigFromUrl<List<Dragon.Blueprints.DragonOpenPackageConfig>>(buildUrl("get-rongos-open-package"));
            var lstPackage = new List<DragonOpenPackageConfig>();
            configuredCommonSetting.ForEach(ro =>
            {
                lstPackage.Add(new DragonOpenPackageConfig
                {
                    Id = ro.Id,
                    DragonElementId = ro.DragonElementId,
                    ItemId = ro.ItemId,
                    ItemQuantity = ro.ItemQuantity,
                    ItemFee = ro.ItemFee,
                    AmountFee = ro.AmountFee,
                    Status = ro.Status,
                    DragonElement = ro.DragonElement
                });
            });
            GameConfigs.DragonOpenPackageConfig = lstPackage;
            logger.LogInformation("loadDragonOpenPackageConfig loaded");
        }
        private async Task loadMergeFragmentItemConfig()
        {
            var configuredCommonSetting = await GetConfigFromUrl<List<Dragon.Blueprints.MergeFragmentItemConfig>>(buildUrl("get-merge-item"));
            var lstMergeConfig = new List<MergeFragmentItemConfig>();
            configuredCommonSetting.ForEach(ro =>
            {
                lstMergeConfig.Add(new MergeFragmentItemConfig
                {
                    Id = ro.Id,
                    ItemReceivedId = ro.ItemReceivedId,
                    ItemReceivedAmount = ro.ItemReceivedAmount,
                    ItemConsumedId = ro.ItemConsumedId,
                    ItemConsumedAmount = ro.ItemConsumedAmount,
                    ItemMaterial = ro.ItemMaterial,
                    ItemMaterialAmount = ro.ItemMaterialAmount,
                    Status = ro.Status

                });
            });
            GameConfigs.MergeFragmentItemConfig = lstMergeConfig;
            logger.LogInformation("loadMergeFragmentItemConfig loaded");
        }
        private async Task loadSeasonRankingRewardConfig()
        {
            var configuredCommonSetting = await GetConfigFromUrl<List<Dragon.Blueprints.SeasonRankingReward>>(buildUrl("get-season-ranking-reward"));
            var lstSeasonRankingRewardConfig = new List<SeasonRankingReward>();
            configuredCommonSetting.ForEach(ro =>
            {
                lstSeasonRankingRewardConfig.Add(new SeasonRankingReward
                {
                    Id = ro.Id,
                    SeasonId = ro.SeasonId,
                    RankFrom = ro.RankFrom,
                    RankTo = ro.RankTo,
                    ItemId = ro.ItemId,
                    Quantity = ro.Quantity,
                    Status = ro.Status

                });
            });
            GameConfigs.SeasonRankingRewardConfig = lstSeasonRankingRewardConfig;
            logger.LogInformation("loadSeasonRankingRewardConfig loaded");
        }
        private async Task loadDailyRankingRewardConfig()
        {
            var configuredCommonSetting = await GetConfigFromUrl<List<Dragon.Blueprints.DailyRankingReward>>(buildUrl("get-daily-ranking-reward"));
            var lstDailyRankingRewardConfig = new List<DailyRankingReward>();
            configuredCommonSetting.ForEach(ro =>
            {
                lstDailyRankingRewardConfig.Add(new DailyRankingReward
                {
                    Id = ro.Id,
                    RankFrom = ro.RankFrom,
                    RankTo = ro.RankTo,
                    ItemId = ro.ItemId,
                    Quantity = ro.Quantity,
                    Status = ro.Status

                });
            });
            GameConfigs.DailyRankingRewardConfig = lstDailyRankingRewardConfig;
            logger.LogInformation("loadDailyRankingRewardConfigConfig loaded");
        }
        #endregion
    }
}