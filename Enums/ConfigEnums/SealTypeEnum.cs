using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonAPI.Extensions;

namespace Dragon.Blueprints
{
    public class SealTypeEnum
    {
        public static GenericEnum Dragon = new GenericEnum(Id: 1, Code: "Dragon", Name: "Dragon");
        public static GenericEnum Mob = new GenericEnum(Id: 2, Code: "Mob", Name: "Mob");
        public static List<GenericEnum> SealTypeEnumList = new List<GenericEnum>
        {
            Dragon,Mob
        };
    }
}
