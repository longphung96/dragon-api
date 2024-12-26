using System.Collections.Generic;
using DragonAPI.Extensions;

namespace Dragon.Blueprints
{
    public class OffChainBodypartTypeEnum
    {
        public static GenericEnum CLAW = new GenericEnum(Id: 1, Code: "Claw", Name: "Claw", Value: "1");
        public static GenericEnum HORN = new GenericEnum(Id: 2, Code: "Horn", Name: "Horn", Value: "1");
        public static GenericEnum SKIN = new GenericEnum(Id: 3, Code: "Skin", Name: "Skin", Value: "1");
        public static GenericEnum TAIL = new GenericEnum(Id: 4, Code: "Tail", Name: "Tail", Value: "1");
        public static GenericEnum EYE = new GenericEnum(Id: 5, Code: "Eye", Name: "Eye", Value: "1");
        public static GenericEnum WING = new GenericEnum(Id: 6, Code: "Wing", Name: "Wing", Value: "1");
        public static GenericEnum ULTIMATE = new GenericEnum(Id: 999, Code: "Ultimate", Name: "Ultimate", Value: "1");
        public static List<GenericEnum> OffChainBodypartTypeEnumList = new List<GenericEnum>
        {
            CLAW,
            HORN,
            SKIN,
            TAIL,
            EYE,
            WING,
            ULTIMATE,
        };
    }
}
