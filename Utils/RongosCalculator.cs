using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DragonAPI.Enums;
using DragonAPI.Models.Entities;
using DragonAPI.Common;

namespace DragonAPI.Common.Calculator
{
    public class DragonCalculator
    {
        private readonly Common.Dragon.DragonCalculatorConfigs configs;
        public DragonCalculator(Common.Dragon.DragonCalculatorConfigs configs)
        {
            this.configs = configs;
        }

        public void Example()
        {
            var rongoses = new List<Dragon>
            {
                new Dragon
                {
                    Id = "1",
                    NftId = "111",
                    RaceLevelUnlocked = 3,
                    Class = ClassType.Gold,
                    Stats = new FullStats
                    {
                        HP = 30,
                        Skill = 20,
                        Morale = 50,
                        Speed = 10,
                        CritDamage = 110,
                        CritRate = 3,
                        Damage = 200,
                    },
                    Bodyparts = new List<Bodypart>
                    {
                        new Bodypart{
                            Level = 1,
                            Class = (long)ClassType.Fire2,
                            MutantType = MutantType.Normal,
                            Type = BodypartType.Claw,
                            Unlocked = true,
                        },
                        new Bodypart{
                            Level = 1,
                            Class = (long)ClassType.Water,
                            MutantType = MutantType.Normal,
                            Type = BodypartType.Horn,
                            Unlocked = true,
                        },
                        new Bodypart{
                            Level = 1,
                            Class = (long)ClassType.Gold,
                            MutantType = MutantType.Normal,
                            Type = BodypartType.Skin,
                            Unlocked = true,
                        },
                    }
                },
                new Dragon
                {
                    Id = "2",
                    NftId = "211",
                    RaceLevelUnlocked = 4,
                    Class = ClassType.Light2,
                    Stats = new FullStats
                    {
                        HP = 30,
                        Skill = 20,
                        Morale = 50,
                        Speed = 10,
                        CritDamage = 110,
                        CritRate = 3,
                        Damage = 200,
                    },
                    Bodyparts = new List<Bodypart>
                    {
                        new Bodypart{
                            Level = 1,
                            Class = (long)ClassType.Fire2,
                            MutantType = MutantType.Normal,
                            Type = BodypartType.Claw,
                            Unlocked = true,
                        },
                        new Bodypart{
                            Level = 1,
                            Class = (long)ClassType.Water,
                            MutantType = MutantType.Normal,
                            Type = BodypartType.Horn,
                            Unlocked = true,
                        },
                        new Bodypart{
                            Level = 1,
                            Class = (long)ClassType.Light2,
                            MutantType = MutantType.Normal,
                            Type = BodypartType.Skin,
                            Unlocked = true,
                        },
                    }
                },
                new Dragon
                {
                    Id = "3",
                    NftId = "311",
                    RaceLevelUnlocked = 4,
                    Class = ClassType.Dark,
                    Stats = new FullStats
                    {
                        HP = 30,
                        Skill = 20,
                        Morale = 50,
                        Speed = 10,
                        CritDamage = 110,
                        CritRate = 3,
                        Damage = 200,
                    },
                    Bodyparts = new List<Bodypart>
                    {
                        new Bodypart{
                            Level = 1,
                            Class = (long)ClassType.Fire2,
                            MutantType = MutantType.Normal,
                            Type = BodypartType.Claw,
                            Unlocked = true,
                        },
                        new Bodypart{
                            Level = 1,
                            Class = (long)ClassType.Water,
                            MutantType = MutantType.Normal,
                            Type = BodypartType.Horn,
                            Unlocked = true,
                        },
                        new Bodypart{
                            Level = 1,
                            Class = (long)ClassType.Dark,
                            MutantType = MutantType.Normal,
                            Type = BodypartType.Skin,
                            Unlocked = true,
                        },
                    }
                },
            };


            var power = CalculateTeamPower(rongoses);
            System.Console.WriteLine($"power {power}");
        }


        public double CalculateTeamPower(List<Dragon> rongoses)
        {
            double teampower = 0;
            rongoses.ForEach(rongos =>
            {
                // rongos.ActiveRace(rongoses, this.configs);
                rongos.ActivePassiveSkill(rongoses, this.configs);
            });

            rongoses.ForEach(rongos =>
            {
                teampower += calculateTeamPowerDragon(rongos);
            });

            return teampower;
        }

        private double calculateTeamPowerDragon(Dragon rongos)
        {
            // var stats = JsonSerializer.Serialize(rongos.Stats);
            // System.Console.WriteLine($"before {stats}");
            rongos.Stats.AddBuffStats(rongos.BuffedStats, configs.StatCoef);
            // stats = JsonSerializer.Serialize(rongos.Stats);
            // System.Console.WriteLine($"after {stats}");
            var teampower = (
                rongos.Stats.Speed
                + rongos.Stats.Morale
                + rongos.Stats.HP
                + rongos.Stats.CritRate * 10
                + rongos.Stats.Damage / this.configs.StatCoef.SkillCoef
            );
            // System.Console.WriteLine($"Dragon ID {rongos.NftId}, power {teampower}");
            return teampower;
        }
    }
}