using DragonAPI.Enums;
using System.Collections.Generic;

namespace DragonAPI.Helpers
{
    public static class EnumConverter
    {
        public static ClassGrouping ConvertToGrouping(this ClassType classType)
        {
            if (classType == ClassType.Gold || classType == ClassType.Gold2 || classType == ClassType.Gold3)
                return ClassGrouping.Gold;
            else if (classType == ClassType.Wood || classType == ClassType.Wood2 || classType == ClassType.Wood3)
                return ClassGrouping.Wood;
            else if (classType == ClassType.Water || classType == ClassType.Water2 || classType == ClassType.Water3)
                return ClassGrouping.Water;
            else if (classType == ClassType.Fire || classType == ClassType.Fire2 || classType == ClassType.Fire3)
                return ClassGrouping.Fire;
            else if (classType == ClassType.Earth || classType == ClassType.Earth2 || classType == ClassType.Earth3)
                return ClassGrouping.Earth;
            else if (classType == ClassType.Dark || classType == ClassType.Dark2 || classType == ClassType.Dark3)
                return ClassGrouping.Dark;
            else
                return ClassGrouping.Light;
        }

        /// <summary>
        /// Funct tính tương sinh tương khắc của 2 class,
        /// Output có 2 case: (class1 khắc class 2 hoặc class1 sinh class2 => tăng 1 giảm 2) và ngược lại
        /// </summary>
        /// <param name="class1"></param>
        /// <param name="class2"></param>
        /// <returns></returns>
        public static ElementEnhancementCode ElementEnhancementCalculate(ClassGrouping class1, ClassGrouping class2)
        {
            if (class1 == ClassGrouping.Dark || class1 == ClassGrouping.Light || class2 == ClassGrouping.Dark || class2 == ClassGrouping.Light)
                return ElementEnhancementCode.None;
            //0: Gold
            //1: Wood
            //2: Earth
            //3: Water
            //4: Fire
            var element = new List<ClassGrouping> { ClassGrouping.Gold, ClassGrouping.Wood, ClassGrouping.Earth, ClassGrouping.Water, ClassGrouping.Fire };
            var index1 = element.IndexOf(class1);
            var index2 = element.IndexOf(class2);

            if ((index1 + 1) % 5 == index2 || (index1 + 3) % 5 == index2)
                return ElementEnhancementCode.IncreaseAReducedB;
            if ((index2 + 1) % 5 == index1 || (index2 + 3) % 5 == index1)
                return ElementEnhancementCode.IncreaseBReducedA;
            return ElementEnhancementCode.None;
        }

        public enum ElementEnhancementCode
        {
            None = 0,
            IncreaseAReducedB = 1,
            IncreaseBReducedA = 2,
        }
    }
}
