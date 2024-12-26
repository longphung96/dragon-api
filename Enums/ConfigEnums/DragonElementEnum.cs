using System.Collections.Generic;
using DragonAPI.Extensions;
// using TrueSight.Lite.Common;

namespace Dragon.Blueprints
{
    public class DragonElementEnum
    {
        public static GenericEnum FIRE = new GenericEnum(Id: 1, Code: "Fire", Name: "Nujasis Janas", Value: "4");
        public static GenericEnum WATER = new GenericEnum(Id: 2, Code: "Water", Name: "Lamigas Jia", Value: "3");
        public static GenericEnum WOOD = new GenericEnum(Id: 3, Code: "Wood", Name: "Myskus Dangas", Value: "2");
        public static GenericEnum GOLD = new GenericEnum(Id: 4, Code: "Gold", Name: "Rafino Aukas", Value: "1");
        public static GenericEnum EARTH = new GenericEnum(Id: 5, Code: "Earth", Name: "Loslyva Butas", Value: "5");
        public static GenericEnum LIGHT = new GenericEnum(Id: 6, Code: "Light", Name: "Supasti Mystian", Value: "7");
        public static GenericEnum DARK = new GenericEnum(Id: 7, Code: "Dark", Name: "Pavy Cingo", Value: "6");
        public static GenericEnum FIRE2 = new GenericEnum(Id: 10, Code: "Fire2", Name: "Huan Janas", Value: "4");
        public static GenericEnum FIRE3 = new GenericEnum(Id: 11, Code: "Fire3", Name: "Hurn Janas", Value: "4");
        public static GenericEnum WATER2 = new GenericEnum(Id: 30, Code: "Water2", Name: "Jie Nerm", Value: "3");
        public static GenericEnum WATER3 = new GenericEnum(Id: 31, Code: "Water3", Name: "Nulism Avira", Value: "3");
        public static GenericEnum WOOD2 = new GenericEnum(Id: 50, Code: "Wood2", Name: "Dezen Linas", Value: "2");
        public static GenericEnum WOOD3 = new GenericEnum(Id: 51, Code: "Wood3", Name: "Vasan Saris", Value: "2");
        public static GenericEnum GOLD2 = new GenericEnum(Id: 70, Code: "Gold2", Name: "Ladiz Aukas", Value: "1");
        public static GenericEnum GOLD3 = new GenericEnum(Id: 71, Code: "Gold3", Name: "Orchi Mura", Value: "1");
        public static GenericEnum EARTH2 = new GenericEnum(Id: 90, Code: "Earth2", Name: "Tolad Panas", Value: "5");
        public static GenericEnum EARTH3 = new GenericEnum(Id: 91, Code: "Earth3", Name: "IIgas Drakon", Value: "5");
        public static GenericEnum LIGHT2 = new GenericEnum(Id: 110, Code: "Light2", Name: "Princas Mystians", Value: "7");
        public static GenericEnum LIGHT3 = new GenericEnum(Id: 111, Code: "Light3", Name: "Princese Mystians", Value: "7");
        public static GenericEnum DARK2 = new GenericEnum(Id: 130, Code: "Dark2", Name: "Keta Cingo", Value: "6");
        public static GenericEnum DARK3 = new GenericEnum(Id: 131, Code: "Dark3", Name: "Dios Cingo", Value: "6");
        public static List<GenericEnum> DragonElementEnumList = new List<GenericEnum>
        {
            FIRE,
            WATER,
            WOOD,
            GOLD,
            EARTH,
            LIGHT,
            DARK,
            FIRE2,
            FIRE3,
            WATER2,
            WATER3,
            WOOD2,
            WOOD3,
            GOLD2,
            GOLD3,
            EARTH2,
            EARTH3,
            LIGHT2,
            LIGHT3,
            DARK2,
            DARK3,
        };
    }
}
