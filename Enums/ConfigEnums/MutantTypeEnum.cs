using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonAPI.Extensions;

namespace Dragon.Blueprints
{
    public class MutantTypeEnum
    {
        public static GenericEnum NORMAL = new GenericEnum(Id: 1, Code: "Normal", Name: "Normal");
        public static GenericEnum PLUS = new GenericEnum(Id: 2, Code: "Plus", Name: "Plus");
        public static GenericEnum MINUS = new GenericEnum(Id: 3, Code: "Minus", Name: "Minus");
        public static List<GenericEnum> MutantTypeEnumList = new List<GenericEnum>
        {
            NORMAL,
            PLUS,
            MINUS,
        };
    }
}
