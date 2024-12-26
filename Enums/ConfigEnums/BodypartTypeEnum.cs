using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonAPI.Extensions;
// using TrueSight.Lite.Common;

namespace Dragon.Blueprints
{
    public class BodypartTypeEnum
    {
        public static GenericEnum CLAW = new GenericEnum(Id: 1, Code: "Claw", Name: "Claw", Value: "1");
        public static GenericEnum HORN = new GenericEnum(Id: 2, Code: "Horn", Name: "Horn", Value: "10");
        public static GenericEnum SKIN = new GenericEnum(Id: 3, Code: "Skin", Name: "Skin", Value: "30");
        public static GenericEnum TAIL = new GenericEnum(Id: 4, Code: "Tail", Name: "Tail", Value: "40");
        public static GenericEnum EYE = new GenericEnum(Id: 5, Code: "Eye", Name: "Eye", Value: "1");
        public static GenericEnum WING = new GenericEnum(Id: 6, Code: "Wing", Name: "Wing", Value: "1");
        public static GenericEnum ULTIMATE = new GenericEnum(Id: 999, Code: "Ultimate", Name: "Ultimate", Value: "1");
        public static List<GenericEnum> BodypartTypeEnumList = new List<GenericEnum>
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
