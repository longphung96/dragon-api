using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonAPI.Extensions;

namespace Dragon.Blueprints
{
    public class LevelTypeEnum
    {
        public static GenericEnum Master = new GenericEnum(Id: 1, Code: "Master", Name: "Master");
        public static GenericEnum Dragon = new GenericEnum(Id: 2, Code: "Dragon", Name: "Dragon");
        public static GenericEnum Seal = new GenericEnum(Id: 3, Code: "Seal", Name: "Seal");
        public static List<GenericEnum> LevelTypeEnumList = new List<GenericEnum>
        {
            Master,
            Dragon,
            Seal,
        };
    }
}
