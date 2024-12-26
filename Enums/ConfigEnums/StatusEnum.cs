using System.Collections.Generic;
using DragonAPI.Extensions;
namespace Dragon.Blueprints
{
    public class StatusEnum
    {
        public static GenericEnum INACTIVE = new GenericEnum(Id: 0, Code: "INACTIVE", Name: "Ngừng hoạt động");
        public static GenericEnum ACTIVE = new GenericEnum(Id: 1, Code: "ACTIVE", Name: "Hoạt động");
        public static List<GenericEnum> StatusEnumList = new List<GenericEnum>
        {
            INACTIVE, ACTIVE,
        };
    }
}
