using System;
using DragonAPI.Enums;


namespace DragonAPI.Utils
{
    public static class ConvertDragonType
    {
        public static ClassType convertDragonTypeIdToClass(long baseId)
        {
            var ret = ClassType.Undefined;
            if (baseId == Dragon.Blueprints.ItemEnum.EggFire1.TypeId)
                ret = ClassType.Fire;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggFire2.TypeId)
                ret = ClassType.Fire2;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggFire3.TypeId)
                ret = ClassType.Fire3;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggWater1.TypeId)
                ret = ClassType.Water;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggWater2.TypeId)
                ret = ClassType.Water2;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggWater3.TypeId)
                ret = ClassType.Water3;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggWood1.TypeId)
                ret = ClassType.Wood;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggWood2.TypeId)
                ret = ClassType.Wood2;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggWood3.TypeId)
                ret = ClassType.Wood3;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggGold1.TypeId)
                ret = ClassType.Gold;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggGold2.TypeId)
                ret = ClassType.Gold2;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggGold3.TypeId)
                ret = ClassType.Gold3;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggEarth1.TypeId)
                ret = ClassType.Earth;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggEarth2.TypeId)
                ret = ClassType.Earth2;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggEarth3.TypeId)
                ret = ClassType.Earth3;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggDark1.TypeId)
                ret = ClassType.Dark;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggDark2.TypeId)
                ret = ClassType.Dark2;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggDark3.TypeId)
                ret = ClassType.Dark3;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggLight1.TypeId)
                ret = ClassType.Light;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggLight2.TypeId)
                ret = ClassType.Light2;
            else if (baseId == Dragon.Blueprints.ItemEnum.EggLight3.TypeId)
                ret = ClassType.Light3;
            else
                ret = ClassType.Undefined;
            return ret;
        }
        public static ClassType MapDragonClass(long index)
        {
            var ret = ClassType.Undefined;
            if (index == (int)ClassType.Fire)
                ret = ClassType.Fire;
            else if (index == (int)ClassType.Fire2)
                ret = ClassType.Fire2;
            else if (index == (int)ClassType.Fire3)
                ret = ClassType.Fire3;
            else if (index == (int)ClassType.Water)
                ret = ClassType.Water;
            else if (index == (int)ClassType.Water2)
                ret = ClassType.Water2;
            else if (index == (int)ClassType.Water3)
                ret = ClassType.Water3;
            else if (index == (int)ClassType.Wood)
                ret = ClassType.Wood;
            else if (index == (int)ClassType.Wood2)
                ret = ClassType.Wood2;
            else if (index == (int)ClassType.Wood3)
                ret = ClassType.Wood3;
            else if (index == (int)ClassType.Gold)
                ret = ClassType.Gold;
            else if (index == (int)ClassType.Gold2)
                ret = ClassType.Gold2;
            else if (index == (int)ClassType.Gold3)
                ret = ClassType.Gold3;
            else if (index == (int)ClassType.Earth)
                ret = ClassType.Earth;
            else if (index == (int)ClassType.Earth2)
                ret = ClassType.Earth2;
            else if (index == (int)ClassType.Earth3)
                ret = ClassType.Earth3;
            else if (index == (int)ClassType.Dark)
                ret = ClassType.Dark;
            else if (index == (int)ClassType.Dark2)
                ret = ClassType.Dark2;
            else if (index == (int)ClassType.Dark3)
                ret = ClassType.Dark3;
            else if (index == (int)ClassType.Light)
                ret = ClassType.Light;
            else if (index == (int)ClassType.Light2)
                ret = ClassType.Light2;
            else if (index == (int)ClassType.Light3)
                ret = ClassType.Light3;
            else
                ret = ClassType.Undefined;
            return ret;
        }

    }
}