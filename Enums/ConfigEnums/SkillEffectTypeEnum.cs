using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonAPI.Extensions;


namespace Dragon.Blueprints
{
    public class SkillEffectTypeEnum
    {
        public static GenericEnum Damage = new GenericEnum(Id: 1, Code: "Damage", Name: "Damage");
        public static GenericEnum Stun = new GenericEnum(Id: 2, Code: "Stun", Name: "Stun");
        public static GenericEnum BuffSkill = new GenericEnum(Id: 3, Code: "BuffSkill", Name: "BuffSkill");
        public static GenericEnum ExtraDamageRevicedByElement = new GenericEnum(Id: 4, Code: "ExtraDamageRevicedByElement", Name: "ExtraDamageRevicedByElement");
        public static GenericEnum Burn = new GenericEnum(Id: 5, Code: "Burn", Name: "Burn");
        public static GenericEnum IncreaBuffHealth = new GenericEnum(Id: 6, Code: "IncreaBuffHealth", Name: "IncreaBuffHealth");
        public static GenericEnum Damage2SkillPoint = new GenericEnum(Id: 7, Code: "Damage2SkillPoint", Name: "Damage2SkillPoint");
        public static GenericEnum DamageToRecoverHP = new GenericEnum(Id: 8, Code: "DamageToRecoverHP", Name: "DamageToRecoverHP");
        public static GenericEnum ExtraReceivedRecoverHPByDragonElement = new GenericEnum(Id: 9, Code: "ExtraReceivedRecoverHPByDragonElement", Name: "ExtraReceivedRecoverHPByDragonElement");
        public static GenericEnum ExtraRevicedBuffSkillByDragonElement = new GenericEnum(Id: 10, Code: "ExtraRevicedBuffSkillByDragonElement", Name: "ExtraRevicedBuffSkillByDragonElement");
        public static GenericEnum IncreaBuffStatSkill = new GenericEnum(Id: 11, Code: "IncreaBuffStatSkill", Name: "IncreaBuffStatSkill");
        public static GenericEnum Taunt = new GenericEnum(Id: 12, Code: "Taunt", Name: "Taunt");
        public static GenericEnum IncreaseTauntPoint = new GenericEnum(Id: 13, Code: "IncreaseTauntPoint", Name: "IncreaseTauntPoint");
        public static GenericEnum DamageReduction = new GenericEnum(Id: 14, Code: "DamageReduction", Name: "DamageReduction");
        public static GenericEnum ExtraDamageReviced = new GenericEnum(Id: 15, Code: "ExtraDamageReviced", Name: "ExtraDamageReviced");
        public static GenericEnum BuffHP = new GenericEnum(Id: 16, Code: "BuffHP", Name: "BuffHP");
        public static GenericEnum FlameGuard = new GenericEnum(Id: 17, Code: "FlameGuard", Name: "FlameGuard");
        public static GenericEnum BombBlind = new GenericEnum(Id: 18, Code: "BombBlind", Name: "BombBlind");
        public static GenericEnum BuffDamageHasDebuff = new GenericEnum(Id: 19, Code: "BuffDamageHasDebuff", Name: "BuffDamageHasDebuff");
        public static GenericEnum IgnoreBleed = new GenericEnum(Id: 20, Code: "IgnoreBleed", Name: "IgnoreBleed");
        public static GenericEnum Bleed = new GenericEnum(Id: 21, Code: "Bleed", Name: "Bleed");
        public static GenericEnum Weakness = new GenericEnum(Id: 22, Code: "Weakness", Name: "Weakness");
        public static GenericEnum Poison = new GenericEnum(Id: 23, Code: "Poison", Name: "Poison");
        public static GenericEnum SelfRecoverHealByEnemyPoison = new GenericEnum(Id: 24, Code: "SelfRecoverHealByEnemyPoison", Name: "SelfRecoverHealByEnemyPoison");
        public static GenericEnum RecoverHeal = new GenericEnum(Id: 25, Code: "RecoverHeal", Name: "RecoverHeal");
        public static GenericEnum Cleanse = new GenericEnum(Id: 26, Code: "Cleanse", Name: "Cleanse");
        public static GenericEnum IncreaBuffStatSpeed = new GenericEnum(Id: 27, Code: "IncreaBuffStatSpeed", Name: "IncreaBuffStatSpeed");
        public static GenericEnum Damage2SpeedPoint = new GenericEnum(Id: 28, Code: "Damage2SpeedPoint", Name: "Damage2SpeedPoint");
        public static GenericEnum BuffSpeed = new GenericEnum(Id: 29, Code: "BuffSpeed", Name: "BuffSpeed");
        public static GenericEnum Freeze = new GenericEnum(Id: 30, Code: "Freeze", Name: "Freeze");
        public static GenericEnum IncreaseFreezeChange = new GenericEnum(Id: 31, Code: "IncreaseFreezeChange", Name: "IncreaseFreezeChange");
        public static GenericEnum BuffShield = new GenericEnum(Id: 32, Code: "BuffShield", Name: "BuffShield");
        public static GenericEnum HpToShield = new GenericEnum(Id: 33, Code: "HpToShield", Name: "HpToShield");
        public static GenericEnum BuffDamageReceivedToShield = new GenericEnum(Id: 34, Code: "BuffDamageReceivedToShield", Name: "BuffDamageReceivedToShield");
        public static GenericEnum DamageReturnShield = new GenericEnum(Id: 35, Code: "DamageReturnShield", Name: "DamageReturnShield");
        public static GenericEnum Blind = new GenericEnum(Id: 36, Code: "Blind", Name: "Blind");
        public static GenericEnum BuffMorale = new GenericEnum(Id: 37, Code: "BuffMorale", Name: "BuffMorale");
        public static GenericEnum IncreaseDebuffCountdownChange = new GenericEnum(Id: 38, Code: "IncreaseDebuffCountdownChange", Name: "IncreaseDebuffCountdownChange");
        public static GenericEnum IncreaseDebuffCountDown = new GenericEnum(Id: 39, Code: "IncreaseDebuffCountDown", Name: "IncreaseDebuffCountDown");
        public static GenericEnum Damage2MoralePoint = new GenericEnum(Id: 40, Code: "Damage2MoralePoint", Name: "Damage2MoralePoint");
        public static GenericEnum Resurrect = new GenericEnum(Id: 41, Code: "Resurrect", Name: "Resurrect");
        public static GenericEnum IncreaseRecoverHealCountdownChange = new GenericEnum(Id: 42, Code: "IncreaseRecoverHealCountdownChange", Name: "IncreaseRecoverHealCountdownChange");
        public static GenericEnum ExDamageRevicedByElement = new GenericEnum(Id: 43, Code: "ExDamageRevicedByElement", Name: "ExDamageRevicedByElement");
        public static GenericEnum BuffDamageForElementSkill = new GenericEnum(Id: 44, Code: "BuffDamageForElementSkill", Name: "BuffDamageForElementSkill");
        public static List<GenericEnum> SkillEffectTypeEnumList = new List<GenericEnum>
        {
            Damage,
            Stun,
            BuffSkill,
            ExtraDamageRevicedByElement,
            Burn,
            IncreaBuffHealth,
            Damage2SkillPoint,
            DamageToRecoverHP,
            ExtraReceivedRecoverHPByDragonElement,
            ExtraRevicedBuffSkillByDragonElement,
            IncreaBuffStatSkill,
            Taunt,
            IncreaseTauntPoint,
            DamageReduction,
            ExtraDamageReviced,
            BuffHP,
            FlameGuard,
            BombBlind,
            BuffDamageHasDebuff,
            IgnoreBleed,
            Bleed,
            Weakness,
            Poison,
            SelfRecoverHealByEnemyPoison,
            RecoverHeal,
            Cleanse,
            IncreaBuffStatSpeed,
            Damage2SpeedPoint,
            BuffSpeed,
            Freeze,
            IncreaseFreezeChange,
            BuffShield,
            HpToShield,
            BuffDamageReceivedToShield,
            DamageReturnShield,
            Blind,
            BuffMorale,
            IncreaseDebuffCountdownChange,
            IncreaseDebuffCountDown,
            Damage2MoralePoint,
            Resurrect,
            IncreaseRecoverHealCountdownChange,
            ExDamageRevicedByElement,
            BuffDamageForElementSkill
        };
    }
}
