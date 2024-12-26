using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonAPI.Extensions;

namespace Dragon.Blueprints
{
    public class GenderEnum
    {
        public static GenericEnum FEMALE = new GenericEnum(Id: 1, Code: "Female", Name: "Female");
        public static GenericEnum MALE = new GenericEnum(Id: 2, Code: "Male", Name: "Male");
        public static List<GenericEnum> GenderEnumList = new List<GenericEnum>
        {
            FEMALE,
            MALE
        };
    }
}
