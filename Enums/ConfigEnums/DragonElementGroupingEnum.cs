using System.Collections.Generic;
using DragonAPI.Extensions;
namespace Dragon.Blueprints
{
    public class DragonElementGroupingEnum
    {
        public static GenericEnum GOLD = new GenericEnum(Id: 1, Code: "Gold", Name: "Gold");
        public static GenericEnum WOOD = new GenericEnum(Id: 2, Code: "Wood", Name: "Wood");
        public static GenericEnum WATER = new GenericEnum(Id: 3, Code: "Water", Name: "Water");
        public static GenericEnum FIRE = new GenericEnum(Id: 4, Code: "Fire", Name: "Fire");
        public static GenericEnum EARTH = new GenericEnum(Id: 5, Code: "Earth", Name: "Earth");
        public static GenericEnum DARK = new GenericEnum(Id: 6, Code: "Dark", Name: "Dark");
        public static GenericEnum LIGHT = new GenericEnum(Id: 7, Code: "Light", Name: "Light");
        public static List<GenericEnum> DragonElementGroupingEnumList = new List<GenericEnum>
        {
            GOLD,
            WOOD,
            WATER,
            FIRE,
            EARTH,
            DARK,
            LIGHT,
        };
    }
}
