using System;
using System.Collections.Generic;
using DragonAPI.Enums;
using DragonAPI.Models.Entities;

namespace DragonAPI.Models.Settings
{


    public class BuffSkillConfig
    {
        public int Level { get; set; }
        public ClassType Class { get; set; }
        public List<List<BuffStats>> ListOfMutantBuffStats { get; set; } = new List<List<BuffStats>> { };
        public ClassType[] AfftectedClassGroups { get; set; }
    }
}