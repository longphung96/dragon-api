using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace DragonAPI.Enums
{
    public enum BodypartType
    {
        Undefined = -1,
        Claw,
        Horn,
        Skin,
        Tail,
        Eye,
        Wing,
        Ultimate = 999
    }
    public enum BossSkillName
    {
        Kuberaos = 1,
        Aranyanios,
        Varunaos,
        Agnios,
        Bhumios,
        Kalios,
        Suryaos,
    }
    public class BossSkill
    {
        public string Id { get; set; }
        public BossSkillName Name { get; set; }
        public BodypartType Bodypart { get; set; }
        public ClassType Class { get; set; }
    }
    public enum Item
    {
        Scroll = 1,
        Dust,
        CharacterEXP,
        Orb,
        RSS,
        Mana = 27,
        Gem,
        ETH,
        Vip = 31
    }
}
