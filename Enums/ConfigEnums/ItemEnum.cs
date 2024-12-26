using Dragon.Blueprints;
using System.Collections.Generic;

namespace Dragon.Blueprints
{
    public class ItemEnum
    {
        public static Item SCROLL = new Item { Id = 1, Name = "Scroll", TypeId = 0, EntityType = "scroll", IsNFT = false, Url = "/items/scroll/scroll.png" };
        public static Item DUST = new Item { Id = 2, Name = "Dust", TypeId = 1, EntityType = "dust", IsNFT = false, Url = "/items/dust/dust.png" };

        public static Item DragonSealBoxCommonFragment = new Item { Id = 3, Name = "Dragon Seal Box Common fragment", TypeId = 50, EntityType = "sealBoxFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item DragonSealBoxGiftFragment = new Item { Id = 203, Name = "Dragon Seal Box Gift fragment", TypeId = 51, EntityType = "sealBoxFragment", IsNFT = false, Url = "/items/sealboxFragment/gift.png" };
        public static Item DragonSealBoxFragment = new Item { Id = 204, Name = "Dragon Seal Box fragment", TypeId = 52, EntityType = "sealBoxFragment", IsNFT = false, Url = "/items/sealboxFragment/normal.png" };
        public static Item DragonSealBoxMysteryFragment = new Item { Id = 205, Name = "Dragon Seal Box Mystery fragment", TypeId = 53, EntityType = "sealBoxFragment", IsNFT = false, Url = "/items/sealboxFragment/mystery.png" };
        public static Item DragonSealLunafragment = new Item { Id = 4, Name = "Dragon Seal Box Luna fragment", TypeId = 54, EntityType = "sealBoxFragment", IsNFT = false, Url = "/items/sealboxFragment/luna.png" };
        public static Item DragonSealBoxSolfragment = new Item { Id = 5, Name = "Dragon Seal Box Sol fragment", TypeId = 55, EntityType = "sealBoxFragment", IsNFT = false, Url = "/items/sealboxFragment/sol.png" };
        public static Item DragonSealBoxJadefragment = new Item { Id = 6, Name = "Dragon Seal Box Jade fragment", TypeId = 56, EntityType = "sealBoxFragment", IsNFT = false, Url = "/items/sealboxFragment/jade.png" };
        public static Item DragonSealBoxCrownfragment = new Item { Id = 7, Name = "Dragon Seal Box Crown fragment", TypeId = 57, EntityType = "sealBoxFragment", IsNFT = false, Url = "/items/sealboxFragment/crown.png" };
        public static Item DragonSealBoxZhengfragment = new Item { Id = 8, Name = "Dragon Seal Box Zheng fragment", TypeId = 58, EntityType = "sealBoxFragment", IsNFT = false, Url = "/items/sealboxFragment/zheng.png" };

        public static Item DragonSealCommonBox = new Item { Id = 9, Name = "Dragon Seal Common Box", TypeId = 50, EntityType = "sealBox", IsNFT = true, Url = "/items/sealbox/common.png" };
        public static Item DragonSealGiftBox = new Item { Id = 10, Name = "Dragon Seal Gift Box", TypeId = 51, EntityType = "sealBox", IsNFT = true, Url = "/items/sealbox/gift.png" };
        public static Item DragonSealBox = new Item { Id = 11, Name = "Dragon Seal Box", TypeId = 52, EntityType = "sealBox", IsNFT = true, Url = "/items/sealbox/normal.png" };
        public static Item DragonMysterySealBox = new Item { Id = 12, Name = "Dragon Mystery Seal Box", TypeId = 53, EntityType = "sealBox", IsNFT = true, Url = "/items/sealbox/mystery.png" };
        public static Item DragonSealLunaBox = new Item { Id = 13, Name = "Dragon Seal Luna Box", TypeId = 54, EntityType = "sealBox", IsNFT = true, Url = "/items/sealbox/luna.png" };
        public static Item DragonSealSolBox = new Item { Id = 14, Name = "Dragon Seal Sol Box", TypeId = 55, EntityType = "sealBox", IsNFT = true, Url = "/items/sealbox/sol.png" };
        public static Item DragonSealJadeBox = new Item { Id = 15, Name = "Dragon Seal Jade Box", TypeId = 56, EntityType = "sealBox", IsNFT = true, Url = "/items/sealbox/jade.png" };
        public static Item DragonSealCrownBox = new Item { Id = 16, Name = "Dragon Seal Crown Box", TypeId = 57, EntityType = "sealBox", IsNFT = true, Url = "/items/sealbox/crown.png" };
        public static Item DragonSealZhengBox = new Item { Id = 17, Name = "Dragon Seal Zheng Box", TypeId = 58, EntityType = "sealBox", IsNFT = true, Url = "/items/sealbox/zheng.png" };

        public static Item EggFragmentGold1 = new Item { Id = 18, Name = "Egg Fragment Gold 1", TypeId = 129, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/gold1.png" };
        public static Item EggFragmentWood1 = new Item { Id = 19, Name = "Egg Fragment Wood 1", TypeId = 130, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/wood1.png" };
        public static Item EggFragmentWater1 = new Item { Id = 20, Name = "Egg Fragment Water 1", TypeId = 131, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/water1.png" };
        public static Item EggFragmentFire1 = new Item { Id = 21, Name = "Egg Fragment Fire 1", TypeId = 132, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/fire1.png" };
        public static Item EggFragmentEarth1 = new Item { Id = 22, Name = "Egg Fragment Earth 1", TypeId = 133, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/earth1.png" };
        public static Item EggFragmentDark1 = new Item { Id = 23, Name = "Egg Fragment Dark 1", TypeId = 134, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/dark1.png" };
        public static Item EggFragmentLight1 = new Item { Id = 24, Name = "Egg Fragment Light 1", TypeId = 135, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/light1.png" };
        public static Item EggFragmentGold2 = new Item { Id = 25, Name = "Egg Fragment Gold 2", TypeId = 136, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/gold2.png" };
        public static Item EggFragmentWood2 = new Item { Id = 26, Name = "Egg Fragment Wood 2", TypeId = 137, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/wood2.png" };
        public static Item EggFragmentWater2 = new Item { Id = 27, Name = "Egg Fragment Water 2", TypeId = 138, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/water2.png" };
        public static Item EggFragmentFire2 = new Item { Id = 28, Name = "Egg Fragment Fire 2", TypeId = 139, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/fire2.png" };
        public static Item EggFragmentEarth2 = new Item { Id = 29, Name = "Egg Fragment Earth 2", TypeId = 140, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/earth2.png" };
        public static Item EggFragmentDark2 = new Item { Id = 30, Name = "Egg Fragment Dark 2", TypeId = 141, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/dark2.png" };
        public static Item EggFragmentLight2 = new Item { Id = 31, Name = "Egg Fragment Light 2", TypeId = 142, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/light2.png" };
        public static Item EggFragmentGold3 = new Item { Id = 32, Name = "Egg Fragment Gold 3", TypeId = 143, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/gold3.png" };
        public static Item EggFragmentWood3 = new Item { Id = 33, Name = "Egg Fragment Wood 3", TypeId = 144, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/wood3.png" };
        public static Item EggFragmentWater3 = new Item { Id = 34, Name = "Egg Fragment Water 3", TypeId = 145, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/water3.png" };
        public static Item EggFragmentFire3 = new Item { Id = 35, Name = "Egg Fragment Fire 3", TypeId = 146, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/fire3.png" };
        public static Item EggFragmentEarth3 = new Item { Id = 36, Name = "Egg Fragment Earth 3", TypeId = 147, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/earth3.png" };
        public static Item EggFragmentDark3 = new Item { Id = 37, Name = "Egg Fragment Dark 3", TypeId = 148, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/dark3.png" };
        public static Item EggFragmentLight3 = new Item { Id = 38, Name = "Egg Fragment Light 3", TypeId = 149, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/light3.png" };
        public static Item CommonEggFragment = new Item { Id = 39, Name = "Common Egg Fragment - 15 elements", TypeId = 150, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/common.png" };
        public static Item RareEggFragment = new Item { Id = 40, Name = "Rare Egg Fragment - 15 elements", TypeId = 151, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/rare.png" };
        public static Item UniqueEggFragment = new Item { Id = 41, Name = "Unique Egg Fragment - 15 elements", TypeId = 152, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/unique.png" };
        public static Item LegendaryEggFragment = new Item { Id = 42, Name = "Legendary Egg Fragment - 15 elements", TypeId = 153, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/legendary.png" };
        public static Item AncientMysteryEggFragment = new Item { Id = 43, Name = "Ancient Mystery Egg Fragment - 21 elements", TypeId = 154, EntityType = "eggFragment", IsNFT = false, Url = "/items/eggFragment/ancient.png" };
        public static Item MatingEgg = new Item { Id = 44, Name = "Default Egg", TypeId = 128, EntityType = "egg", IsNFT = true, Url = "/items/egg/common.png" };
        public static Item EggGold1 = new Item { Id = 45, Name = "Egg Gold 1", TypeId = 129, EntityType = "egg", IsNFT = true, Url = "/items/egg/gold1.png" };
        public static Item EggWood1 = new Item { Id = 46, Name = "Egg Wood 1", TypeId = 130, EntityType = "egg", IsNFT = true, Url = "/items/egg/wood1.png" };
        public static Item EggWater1 = new Item { Id = 47, Name = "Egg Water 1", TypeId = 131, EntityType = "egg", IsNFT = true, Url = "/items/egg/water1.png" };
        public static Item EggFire1 = new Item { Id = 48, Name = "Egg Fire 1", TypeId = 132, EntityType = "egg", IsNFT = true, Url = "/items/egg/fire1.png" };
        public static Item EggEarth1 = new Item { Id = 49, Name = "Egg Earth 1", TypeId = 133, EntityType = "egg", IsNFT = true, Url = "/items/egg/earth1.png" };
        public static Item EggDark1 = new Item { Id = 50, Name = "Egg Dark 1", TypeId = 134, EntityType = "egg", IsNFT = true, Url = "/items/egg/dark1.png" };
        public static Item EggLight1 = new Item { Id = 51, Name = "Egg Light 1", TypeId = 135, EntityType = "egg", IsNFT = true, Url = "/items/egg/light1.png" };
        public static Item EggGold2 = new Item { Id = 52, Name = "Egg Gold 2", TypeId = 136, EntityType = "egg", IsNFT = true, Url = "/items/egg/gold2.png" };
        public static Item EggWood2 = new Item { Id = 53, Name = "Egg Wood 2", TypeId = 137, EntityType = "egg", IsNFT = true, Url = "/items/egg/wood2.png" };
        public static Item EggWater2 = new Item { Id = 54, Name = "Egg Water 2", TypeId = 138, EntityType = "egg", IsNFT = true, Url = "/items/egg/water2.png" };
        public static Item EggFire2 = new Item { Id = 55, Name = "Egg Fire 2", TypeId = 139, EntityType = "egg", IsNFT = true, Url = "/items/egg/fire2.png" };
        public static Item EggEarth2 = new Item { Id = 56, Name = "Egg Earth 2", TypeId = 140, EntityType = "egg", IsNFT = true, Url = "/items/egg/earth2.png" };
        public static Item EggDark2 = new Item { Id = 57, Name = "Egg Dark 2", TypeId = 141, EntityType = "egg", IsNFT = true, Url = "/items/egg/dark2.png" };
        public static Item EggLight2 = new Item { Id = 58, Name = "Egg Light 2", TypeId = 142, EntityType = "egg", IsNFT = true, Url = "/items/egg/light2.png" };
        public static Item EggGold3 = new Item { Id = 59, Name = "Egg Gold 3", TypeId = 143, EntityType = "egg", IsNFT = true, Url = "/items/egg/gold3.png" };
        public static Item EggWood3 = new Item { Id = 60, Name = "Egg Wood 3", TypeId = 144, EntityType = "egg", IsNFT = true, Url = "/items/egg/wood3.png" };
        public static Item EggWater3 = new Item { Id = 61, Name = "Egg Water 3", TypeId = 145, EntityType = "egg", IsNFT = true, Url = "/items/egg/water3.png" };
        public static Item EggFire3 = new Item { Id = 62, Name = "Egg Fire 3", TypeId = 146, EntityType = "egg", IsNFT = true, Url = "/items/egg/fire3.png" };
        public static Item EggEarth3 = new Item { Id = 63, Name = "Egg Earth 3", TypeId = 147, EntityType = "egg", IsNFT = true, Url = "/items/egg/earth3.png" };
        public static Item EggDark3 = new Item { Id = 64, Name = "Egg Dark 3", TypeId = 148, EntityType = "egg", IsNFT = true, Url = "/items/egg/dark3.png" };
        public static Item EggLight3 = new Item { Id = 65, Name = "Egg Light 3", TypeId = 149, EntityType = "egg", IsNFT = true, Url = "/items/egg/light3.png" };
        public static Item CommonEgg = new Item { Id = 66, Name = "Common Egg - 15 elements", TypeId = 150, EntityType = "egg", IsNFT = true, Url = "/items/egg/common.png" };
        public static Item RareEgg = new Item { Id = 67, Name = "Rare Egg - 15 elements", TypeId = 151, EntityType = "egg", IsNFT = true, Url = "/items/egg/rare.png" };
        public static Item UniqueEgg = new Item { Id = 68, Name = "Unique Egg - 15 elements", TypeId = 152, EntityType = "egg", IsNFT = true, Url = "/items/egg/unique.png" };
        public static Item LegendaryEgg = new Item { Id = 69, Name = "Legendary Egg - 15 elements", TypeId = 153, EntityType = "egg", IsNFT = true, Url = "/items/egg/legendary.png" };
        public static Item AncientMysteryEgg = new Item { Id = 70, Name = "Ancient Mystery Egg - 21 elements", TypeId = 154, EntityType = "egg", IsNFT = true, Url = "/items/egg/ancient.png" };

        public static Item WoodenKeyFragment = new Item { Id = 71, Name = "Wooden Key Fragment", TypeId = 5000, EntityType = "treasureKeyFragment", IsNFT = false, Url = "/items/treasureKeyFragment/wooden.png" };
        public static Item BronzeKeyFragment = new Item { Id = 72, Name = "Bronze Key Fragment", TypeId = 5001, EntityType = "treasureKeyFragment", IsNFT = false, Url = "/items/treasureKeyFragment/bronze.png" };
        public static Item SilverKeyFragment = new Item { Id = 73, Name = "Silver Key Fragment", TypeId = 5002, EntityType = "treasureKeyFragment", IsNFT = false, Url = "/items/treasureKeyFragment/silver.png" };
        public static Item GoldenKeyFragment = new Item { Id = 74, Name = "Golden Key Fragment", TypeId = 5003, EntityType = "treasureKeyFragment", IsNFT = false, Url = "/items/treasureKeyFragment/golden.png" };
        public static Item RubyKeyFragment = new Item { Id = 75, Name = "Ruby Key Fragment", TypeId = 5004, EntityType = "treasureKeyFragment", IsNFT = false, Url = "/items/treasureKeyFragment/ruby.png" };
        public static Item TopazKeyFragment = new Item { Id = 76, Name = "Topaz Key Fragment", TypeId = 5005, EntityType = "treasureKeyFragment", IsNFT = false, Url = "/items/treasureKeyFragment/topaz.png" };
        public static Item DiamondKeyFragment = new Item { Id = 77, Name = "Diamond Key Fragment", TypeId = 5006, EntityType = "treasureKeyFragment", IsNFT = false, Url = "/items/treasureKeyFragment/diamond.png" };
        public static Item VoidKeyFragment = new Item { Id = 78, Name = "Void Key Fragment", TypeId = 5007, EntityType = "treasureKeyFragment", IsNFT = false, Url = "/items/treasureKeyFragment/void.png" };
        public static Item RagKeyFragment = new Item { Id = 79, Name = "Rag Key Fragment", TypeId = 5008, EntityType = "treasureKeyFragment", IsNFT = false, Url = "/items/treasureKeyFragment/rag.png" };

        public static Item WoodenKey = new Item { Id = 80, Name = "Wooden Key", TypeId = 5000, EntityType = "treasureKey", IsNFT = true, Url = "/items/treasureKey/wooden.png" };
        public static Item BronzeKey = new Item { Id = 81, Name = "Bronze Key", TypeId = 5001, EntityType = "treasureKey", IsNFT = true, Url = "/items/treasureKey/bronze.png" };
        public static Item SilverKey = new Item { Id = 82, Name = "Silver Key", TypeId = 5002, EntityType = "treasureKey", IsNFT = true, Url = "/items/treasureKey/silver.png" };
        public static Item GoldenKey = new Item { Id = 83, Name = "Golden Key", TypeId = 5003, EntityType = "treasureKey", IsNFT = true, Url = "/items/treasureKey/golden.png" };
        public static Item RubyKey = new Item { Id = 84, Name = "Ruby Key", TypeId = 5004, EntityType = "treasureKey", IsNFT = true, Url = "/items/treasureKey/ruby.png" };
        public static Item TopazKey = new Item { Id = 85, Name = "Topaz Key", TypeId = 5005, EntityType = "treasureKey", IsNFT = true, Url = "/items/treasureKey/topaz.png" };
        public static Item DiamondKey = new Item { Id = 86, Name = "Diamond Key", TypeId = 5006, EntityType = "treasureKey", IsNFT = true, Url = "/items/treasureKey/diamond.png" };
        public static Item VoidKey = new Item { Id = 87, Name = "Void Key", TypeId = 5007, EntityType = "treasureKey", IsNFT = true, Url = "/items/treasureKey/void.png" };
        public static Item RagKey = new Item { Id = 88, Name = "Rag Key", TypeId = 5008, EntityType = "treasureKey", IsNFT = true, Url = "/items/treasureKey/rag.png" };

        public static Item IncubatorUncommonfragment = new Item { Id = 89, Name = "Incubator Uncommon fragment", TypeId = 5101, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item IncubatorCommonfragment = new Item { Id = 90, Name = "Incubator Common fragment", TypeId = 5102, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item IncubatorRarefragment = new Item { Id = 91, Name = "Incubator Rare fragment", TypeId = 5103, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item IncubatorUniquefragment = new Item { Id = 92, Name = "Incubator Unique fragment", TypeId = 5104, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item IncubatorLegendaryfragment = new Item { Id = 93, Name = "Incubator Legendary fragment", TypeId = 5105, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item IncubatorAncientfragment = new Item { Id = 94, Name = "Incubator Ancient fragment", TypeId = 5106, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item DefaultIncubator = new Item { Id = 95, Name = "Default Incubator", TypeId = 5100, EntityType = "building", IsNFT = true, Url = "/items/building/incubator/default.png" };
        public static Item IncubatorUncommon = new Item { Id = 96, Name = "Incubator Uncommon", TypeId = 5101, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item IncubatorCommon = new Item { Id = 97, Name = "Incubator Common", TypeId = 5102, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item IncubatorRare = new Item { Id = 98, Name = "Incubator Rare", TypeId = 5103, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item IncubatorUnique = new Item { Id = 99, Name = "Incubator Unique", TypeId = 5104, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item IncubatorLegendary = new Item { Id = 100, Name = "Incubator Legendary", TypeId = 5105, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item IncubatorAncient = new Item { Id = 101, Name = "Incubator Ancient", TypeId = 5106, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };

        public static Item LairUncommonfragment = new Item { Id = 102, Name = "Lair Uncommon fragment", TypeId = 5108, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item LairCommonfragment = new Item { Id = 103, Name = "Lair Common fragment", TypeId = 5109, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item LairRarefragment = new Item { Id = 104, Name = "Lair Rare fragment", TypeId = 5110, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item LairUniquefragment = new Item { Id = 105, Name = "Lair Unique fragment", TypeId = 5111, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item LairLegendaryfragment = new Item { Id = 106, Name = "Lair Legendary fragment", TypeId = 5112, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item LairAncientfragment = new Item { Id = 107, Name = "Lair Ancient fragment", TypeId = 5113, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item GoldLairRarefragment = new Item { Id = 108, Name = "Gold Lair Rare fragment", TypeId = 5114, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item GoldLairUniquefragment = new Item { Id = 109, Name = "Gold Lair Unique fragment", TypeId = 5115, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item GoldLairLegendaryfragment = new Item { Id = 110, Name = "Gold Lair Legendary fragment", TypeId = 5116, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item GoldLairAncientfragment = new Item { Id = 111, Name = "Gold Lair Ancient fragment", TypeId = 5117, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item WoodLairRarefragment = new Item { Id = 112, Name = "Wood Lair Rare fragment", TypeId = 5118, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item WoodLairUniquefragment = new Item { Id = 113, Name = "Wood Lair Unique fragment", TypeId = 5119, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item WoodLairLegendaryfragment = new Item { Id = 114, Name = "Wood Lair Legendary fragment", TypeId = 5120, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item WoodLairAncientfragment = new Item { Id = 115, Name = "Wood Lair Ancient fragment", TypeId = 5121, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item WaterLairRarefragment = new Item { Id = 116, Name = "Water Lair Rare fragment", TypeId = 5122, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item WaterLairUniquefragment = new Item { Id = 117, Name = "Water Lair Unique fragment", TypeId = 5123, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item WaterLairLegendaryfragment = new Item { Id = 118, Name = "Water Lair Legendary fragment", TypeId = 5124, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item WaterLairAncientfragment = new Item { Id = 119, Name = "Water Lair Ancient fragment", TypeId = 5125, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item FireLairRarefragment = new Item { Id = 120, Name = "Fire Lair Rare fragment", TypeId = 5126, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item FireLairUniquefragment = new Item { Id = 121, Name = "Fire Lair Unique fragment", TypeId = 5127, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item FireLairLegendaryfragment = new Item { Id = 122, Name = "Fire Lair Legendary fragment", TypeId = 5128, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item FireLairAncientfragment = new Item { Id = 123, Name = "Fire Lair Ancient fragment", TypeId = 5129, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item EarthLairRarefragment = new Item { Id = 124, Name = "Earth Lair Rare fragment", TypeId = 5130, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item EarthLairUniquefragment = new Item { Id = 125, Name = "Earth Lair Unique fragment", TypeId = 5131, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item EarthLairLegendaryfragment = new Item { Id = 126, Name = "Earth Lair Legendary fragment", TypeId = 5132, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item EarthLairAncientfragment = new Item { Id = 127, Name = "Earth Lair Ancient fragment", TypeId = 5133, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item DefaultLair = new Item { Id = 128, Name = "Default Lair", TypeId = 5107, EntityType = "building", IsNFT = true, Url = "/items/building/lair/default.png" };
        public static Item LairUncommon = new Item { Id = 129, Name = "Lair Uncommon", TypeId = 5108, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item LairCommon = new Item { Id = 130, Name = "Lair Common", TypeId = 5109, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item LairRare = new Item { Id = 131, Name = "Lair Rare", TypeId = 5110, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item LairUnique = new Item { Id = 132, Name = "Lair Unique", TypeId = 5111, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item LairLegendary = new Item { Id = 133, Name = "Lair Legendary", TypeId = 5112, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item LairAncient = new Item { Id = 134, Name = "Lair Ancient", TypeId = 5113, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item GoldLairRare = new Item { Id = 135, Name = "Gold Lair Rare", TypeId = 5114, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item GoldLairUnique = new Item { Id = 136, Name = "Gold Lair Unique", TypeId = 5115, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item GoldLairLegendary = new Item { Id = 137, Name = "Gold Lair Legendary", TypeId = 5116, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item GoldLairAncient = new Item { Id = 138, Name = "Gold Lair Ancient", TypeId = 5117, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item WoodLairRare = new Item { Id = 139, Name = "Wood Lair Rare", TypeId = 5118, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item WoodLairUnique = new Item { Id = 140, Name = "Wood Lair Unique", TypeId = 5119, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item WoodLairLegendary = new Item { Id = 141, Name = "Wood Lair Legendary", TypeId = 5120, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item WoodLairAncient = new Item { Id = 142, Name = "Wood Lair Ancient", TypeId = 5121, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item WaterLairRare = new Item { Id = 143, Name = "Water Lair Rare", TypeId = 5122, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item WaterLairUnique = new Item { Id = 144, Name = "Water Lair Unique", TypeId = 5123, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item WaterLairLegendary = new Item { Id = 145, Name = "Water Lair Legendary", TypeId = 5124, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item WaterLairAncient = new Item { Id = 146, Name = "Water Lair Ancient", TypeId = 5125, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item FireLairRare = new Item { Id = 147, Name = "Fire Lair Rare", TypeId = 5126, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item FireLairUnique = new Item { Id = 148, Name = "Fire Lair Unique", TypeId = 5127, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item FireLairLegendary = new Item { Id = 149, Name = "Fire Lair Legendary", TypeId = 5128, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item FireLairAncient = new Item { Id = 150, Name = "Fire Lair Ancient", TypeId = 5129, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item EarthLairRare = new Item { Id = 151, Name = "Earth Lair Rare", TypeId = 5130, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item EarthLairUnique = new Item { Id = 152, Name = "Earth Lair Unique", TypeId = 5131, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item EarthLairLegendary = new Item { Id = 153, Name = "Earth Lair Legendary", TypeId = 5132, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item EarthLairAncient = new Item { Id = 154, Name = "Earth Lair Ancient", TypeId = 5133, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };

        public static Item LoveZoneUncommonfragment = new Item { Id = 155, Name = "Love Zone Uncommon fragment", TypeId = 5135, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item LoveZoneCommonfragment = new Item { Id = 156, Name = "Love Zone Common fragment", TypeId = 5136, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item LoveZoneRarefragment = new Item { Id = 157, Name = "Love Zone Rare fragment", TypeId = 5137, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item LoveZoneUniquefragment = new Item { Id = 158, Name = "Love Zone Unique fragment", TypeId = 5138, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item LoveZoneLegendaryfragment = new Item { Id = 159, Name = "Love Zone Legendary fragment", TypeId = 5139, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item LoveZoneAncientfragment = new Item { Id = 160, Name = "Love Zone Ancient fragment", TypeId = 5140, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item DefaultLoveZone = new Item { Id = 161, Name = "Default Love Zone", TypeId = 5134, EntityType = "building", IsNFT = true, Url = "/items/building/loveZone/default.png" };
        public static Item LoveZoneUncommon = new Item { Id = 162, Name = "Love Zone Uncommon", TypeId = 5135, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item LoveZoneCommon = new Item { Id = 163, Name = "Love Zone Common", TypeId = 5136, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item LoveZoneRare = new Item { Id = 164, Name = "Love Zone Rare", TypeId = 5137, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item LoveZoneUnique = new Item { Id = 165, Name = "Love Zone Unique", TypeId = 5138, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item LoveZoneLegendary = new Item { Id = 166, Name = "Love Zone Legendary", TypeId = 5139, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item LoveZoneAncient = new Item { Id = 167, Name = "Love Zone Ancient", TypeId = 5140, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };

        public static Item WorldPortalUncommonfragment = new Item { Id = 168, Name = "World Portal Uncommon fragment", TypeId = 5142, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item WorldPortalCommonfragment = new Item { Id = 169, Name = "World Portal Common fragment", TypeId = 5143, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item WorldPortalRarefragment = new Item { Id = 170, Name = "World Portal Rare fragment", TypeId = 5144, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item WorldPortalUniquefragment = new Item { Id = 171, Name = "World Portal Unique fragment", TypeId = 5145, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item WorldPortalLegendaryfragment = new Item { Id = 172, Name = "World Portal Legendary fragment", TypeId = 5146, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item WorldPortalAncientfragment = new Item { Id = 173, Name = "World Portal Ancient fragment", TypeId = 5147, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item DefaultWorldPortal = new Item { Id = 174, Name = "Default World Portal", TypeId = 5141, EntityType = "building", IsNFT = true, Url = "/items/building/worldPortal/default.png" };
        public static Item WorldPortalUncommon = new Item { Id = 175, Name = "World Portal Uncommon", TypeId = 5142, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item WorldPortalCommon = new Item { Id = 176, Name = "World Portal Common", TypeId = 5143, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item WorldPortalRare = new Item { Id = 177, Name = "World Portal Rare", TypeId = 5144, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item WorldPortalUnique = new Item { Id = 178, Name = "World Portal Unique", TypeId = 5145, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item WorldPortalLegendary = new Item { Id = 179, Name = "World Portal Legendary", TypeId = 5146, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item WorldPortalAncient = new Item { Id = 180, Name = "World Portal Ancient", TypeId = 5147, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };

        public static Item ArenaUncommonfragment = new Item { Id = 181, Name = "Arena Uncommon fragment", TypeId = 5149, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item ArenaCommonfragment = new Item { Id = 182, Name = "Arena Common fragment", TypeId = 5150, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item ArenaPortalRarefragment = new Item { Id = 183, Name = "Arena Portal Rare fragment", TypeId = 5151, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item ArenaUniquefragment = new Item { Id = 184, Name = "Arena Unique fragment", TypeId = 5152, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item ArenaLegendaryfragment = new Item { Id = 185, Name = "Arena Legendary fragment", TypeId = 5153, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item ArenaAncientfragment = new Item { Id = 186, Name = "Arena Ancient fragment", TypeId = 5154, EntityType = "buildingFragment", IsNFT = false, Url = "/items/sealboxFragment/common.png" };
        public static Item DefaultArena = new Item { Id = 187, Name = "Default Arena", TypeId = 5148, EntityType = "building", IsNFT = true, Url = "/items/building/arena/default.png" };
        public static Item ArenaUncommon = new Item { Id = 188, Name = "Arena Uncommon", TypeId = 5149, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item ArenaCommon = new Item { Id = 189, Name = "Arena Common", TypeId = 5150, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item ArenaPortalRare = new Item { Id = 190, Name = "Arena Portal Rare", TypeId = 5151, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item ArenaUnique = new Item { Id = 191, Name = "Arena Unique", TypeId = 5152, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item ArenaLegendary = new Item { Id = 192, Name = "Arena Legendary", TypeId = 5153, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };
        public static Item ArenaAncient = new Item { Id = 193, Name = "Arena Ancient", TypeId = 5154, EntityType = "building", IsNFT = true, Url = "/items/sealboxFragment/common.png" };


        public static Item GoldTicket = new Item { Id = 210, Name = "GoldTicket", TypeId = 65000, EntityType = "ticket", IsNFT = true, Url = "/items/ticket/gold.png" };
        public static Item WoodTicket = new Item { Id = 211, Name = "WoodTicket", TypeId = 65001, EntityType = "ticket", IsNFT = true, Url = "/items/ticket/wood.png" };
        public static Item WaterTicket = new Item { Id = 212, Name = "WaterTicket", TypeId = 65002, EntityType = "ticket", IsNFT = true, Url = "/items/ticket/water.png" };
        public static Item FireTicket = new Item { Id = 213, Name = "FireTicket", TypeId = 65003, EntityType = "ticket", IsNFT = true, Url = "/items/ticket/fire.png" };
        public static Item EarthTicket = new Item { Id = 214, Name = "EarthTicket", TypeId = 65004, EntityType = "ticket", IsNFT = true, Url = "/items/ticket/earth.png" };
        public static Item DarkTicket = new Item { Id = 215, Name = "DarkTicket", TypeId = 65005, EntityType = "ticket", IsNFT = true, Url = "/items/ticket/dark.png" };
        public static Item LightTicket = new Item { Id = 216, Name = "LightTicket", TypeId = 65006, EntityType = "ticket", IsNFT = true, Url = "/items/ticket/light.png" };
        public static Item RandomTicket = new Item { Id = 217, Name = "RandomTicket", TypeId = 65007, EntityType = "ticket", IsNFT = true, Url = "/items/ticket/random.png" };
        public static Item RandomMystertyTicket = new Item { Id = 218, Name = "RandomMystertyTicket", TypeId = 65008, EntityType = "ticket", IsNFT = true, Url = "/items/ticket/random_mystery.png" };


        // public static Item Seal0 = new Item { Id = 230, Name = "Seal 0", TypeId = -1, EntityType = "seal", SealSlot = 0, IsNFT = true, Url = "/items/seal/0.png" };
        // public static Item Seal1 = new Item { Id = 231, Name = "Seal 1", TypeId = -1, EntityType = "seal", SealSlot = 1, IsNFT = true, Url = "/items/seal/1.png" };
        // public static Item Seal2 = new Item { Id = 232, Name = "Seal 2", TypeId = -1, EntityType = "seal", SealSlot = 2, IsNFT = true, Url = "/items/seal/2.png" };
        // public static Item Seal3 = new Item { Id = 233, Name = "Seal 3", TypeId = -1, EntityType = "seal", SealSlot = 3, IsNFT = true, Url = "/items/seal/3.png" };
        // public static Item Seal4 = new Item { Id = 234, Name = "Seal 4", TypeId = -1, EntityType = "seal", SealSlot = 4, IsNFT = true, Url = "/items/seal/4.png" };
        // public static Item Seal5 = new Item { Id = 235, Name = "Seal 5", TypeId = -1, EntityType = "seal", SealSlot = 5, IsNFT = true, Url = "/items/seal/5.png" };
        // public static Item Seal6 = new Item { Id = 236, Name = "Seal 6", TypeId = -1, EntityType = "seal", SealSlot = 6, IsNFT = true, Url = "/items/seal/6.png" };
        // public static Item Seal7 = new Item { Id = 237, Name = "Seal 7", TypeId = -1, EntityType = "seal", SealSlot = 7, IsNFT = true, Url = "/items/seal/7.png" };

        // NFT Box
        public static Item NftBoxW = new Item { Id = 250, Name = "W", TypeId = 1, EntityType = "nftbox", IsNFT = true, Url = "/items/nftbox/w.png" };
        public static Item NftBoxWPro = new Item { Id = 251, Name = "W-Pro", TypeId = 2, EntityType = "nftbox", IsNFT = true, Url = "/items/nftbox/wpro.png" };
        public static Item NftBoxWSaltu = new Item { Id = 252, Name = "Saltu", TypeId = 3, EntityType = "nftbox", IsNFT = true, Url = "/items/nftbox/saltu.png" };
        public static Item NftBoxOceanus = new Item { Id = 253, Name = "Oceanus", TypeId = 4, EntityType = "nftbox", IsNFT = true, Url = "/items/nftbox/oceanus.png" };
        public static Item NftBoxPahaad = new Item { Id = 254, Name = "Pahaad", TypeId = 5, EntityType = "nftbox", IsNFT = true, Url = "/items/nftbox/pahaad.png" };
        public static Item NftBoxDesertum = new Item { Id = 255, Name = "Desertum", TypeId = 6, EntityType = "nftbox", IsNFT = true, Url = "/items/nftbox/desertum.png" };
        public static Item NftBoxMors = new Item { Id = 256, Name = "Mors", TypeId = 7, EntityType = "nftbox", IsNFT = true, Url = "/items/nftbox/mors.png" };
        public static Item NftBoxSomnium = new Item { Id = 257, Name = "Somnium", TypeId = 8, EntityType = "nftbox", IsNFT = true, Url = "/items/nftbox/somnium.png" };

        // Fruit
        public static Item RareCreationFruit = new Item { Id = 258, Name = "Rare Creation", TypeId = 6000, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/rare_creation.png" };
        public static Item UniqueCreationFruit = new Item { Id = 259, Name = "Unique Creation", TypeId = 6001, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/unique_creation.png" };
        public static Item LegendaryCreationFruit = new Item { Id = 260, Name = "Legendary Creation", TypeId = 6002, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/legendary_creation.png" };
        public static Item RareBrilliantFruit = new Item { Id = 261, Name = "Rare Brilliant", TypeId = 6003, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/rare_brilliant.png" };
        public static Item UniqueBrilliantFruit = new Item { Id = 262, Name = "Unique Brilliant", TypeId = 6004, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/unique_brilliant.png" };
        public static Item LegendaryBrilliantFruit = new Item { Id = 263, Name = "Legendary Brilliant", TypeId = 6005, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/legendary_brilliant.png" };
        public static Item RareFlashFruit = new Item { Id = 264, Name = "Rare Flash", TypeId = 6006, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/rare_flash.png" };
        public static Item UniqueFlashFruit = new Item { Id = 265, Name = "Unique Flash", TypeId = 6007, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/unique_flash.png" };
        public static Item LegendaryFlashFruit = new Item { Id = 266, Name = "Legendary Flash", TypeId = 6008, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/legendary_flash.png" };
        public static Item RareIntellectFruit = new Item { Id = 267, Name = "Rare Intellect", TypeId = 6009, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/rare_intellect.png" };
        public static Item UniqueIntellectFruit = new Item { Id = 268, Name = "Unique Intellect", TypeId = 6010, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/unique_intellect.png" };
        public static Item LegendaryIntellectFruit = new Item { Id = 269, Name = "Legendary Intellect", TypeId = 6011, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/legendary_intellect.png" };
        public static Item RareSanguinaryFruit = new Item { Id = 270, Name = "Rare Sanguinary", TypeId = 6012, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/rare_sanguinary.png" };
        public static Item UniqueSanguinaryFruit = new Item { Id = 271, Name = "Unique Sanguinary", TypeId = 6013, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/unique_sanguinary.png" };
        public static Item LegendarySanguinaryFruit = new Item { Id = 272, Name = "Legendary Sanguinary", TypeId = 6014, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/legendary_sanguinary.png" };
        public static Item RareAtheneFruit = new Item { Id = 273, Name = "Rare Athene", TypeId = 6015, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/rare_athene.png" };
        public static Item UniqueAtheneFruit = new Item { Id = 274, Name = "Unique Athene", TypeId = 6016, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/unique_athene.png" };
        public static Item LegendaryAtheneFruit = new Item { Id = 275, Name = "Legendary Athene", TypeId = 6017, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/legendary_athene.png" };
        public static Item RareLaurusFruit = new Item { Id = 276, Name = "Rare Laurus", TypeId = 6018, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/rare_laurus.png" };
        public static Item UniqueLaurusFruit = new Item { Id = 277, Name = "Unique Laurus", TypeId = 6019, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/unique_laurus.png" };
        public static Item LegendaryLaurusFruit = new Item { Id = 278, Name = "Legendary Laurus", TypeId = 6020, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/legendary_laurus.png" };
        public static Item RareAraFruit = new Item { Id = 279, Name = "Rare Ara", TypeId = 6021, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/rare_ara.png" };
        public static Item UniqueAraFruit = new Item { Id = 280, Name = "Unique Ara", TypeId = 6022, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/unique_ara.png" };
        public static Item LegendaryAraFruit = new Item { Id = 281, Name = "Legendary Ara", TypeId = 6023, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/legendary_ara.png" };
        public static Item RarePommeFruit = new Item { Id = 282, Name = "Rare Pomme", TypeId = 6024, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/rare_pomme.png" };
        public static Item UniquePommeFruit = new Item { Id = 283, Name = "Unique Pomme", TypeId = 6025, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/unique_pomme.png" };
        public static Item LegendaryPommeFruit = new Item { Id = 284, Name = "Legendary Pomme", TypeId = 6026, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/legendary_pomme.png" };
        public static Item RareVitisFruit = new Item { Id = 285, Name = "Rare Vitis", TypeId = 6027, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/rare_vitis.png" };
        public static Item UniqueVitisFruit = new Item { Id = 286, Name = "Unique Vitis", TypeId = 6028, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/unique_vitis.png" };
        public static Item LegendaryVitisFruit = new Item { Id = 287, Name = "Legendary Vitis", TypeId = 6029, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/legendary_vitis.png" };
        public static Item RareHyperionFruit = new Item { Id = 288, Name = "Rare Hyperion", TypeId = 6030, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/rare_hyperion.png" };
        public static Item UniqueHyperionFruit = new Item { Id = 289, Name = "Unique Hyperion", TypeId = 6031, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/unique_hyperion.png" };
        public static Item LegendaryHyperionFruit = new Item { Id = 290, Name = "Legendary Hyperion", TypeId = 6032, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/legendary_hyperion.png" };
        public static Item RareOleaFruit = new Item { Id = 291, Name = "Rare Olea", TypeId = 6033, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/rare_olea.png" };
        public static Item UniqueOleaFruit = new Item { Id = 292, Name = "Unique Olea", TypeId = 6034, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/unique_olea.png" };
        public static Item LegendaryOleaFruit = new Item { Id = 293, Name = "Legendary Olea", TypeId = 6035, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/legendary_olea.png" };
        public static Item RareRosaFruit = new Item { Id = 294, Name = "Rare Rosa", TypeId = 6036, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/rare_rosa.png" };
        public static Item UniqueRosaFruit = new Item { Id = 295, Name = "Unique Rosa", TypeId = 6037, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/unique_rosa.png" };
        public static Item LegendaryRosaFruit = new Item { Id = 296, Name = "Legendary Rosa", TypeId = 6038, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/legendary_rosa.png" };
        public static Item RareDiasFruit = new Item { Id = 297, Name = "Rare Dias", TypeId = 6039, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/rare_dias.png" };
        public static Item UniqueDiasFruit = new Item { Id = 298, Name = "Unique Dias", TypeId = 6040, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/unique_dias.png" };
        public static Item LegendaryDiasFruit = new Item { Id = 299, Name = "Legendary Dias", TypeId = 6041, EntityType = "fruit", IsNFT = false, Url = "/items/fruit/legendary_dias.png" };


        public static Item EnergyItem = new Item { Id = 300, Name = "Energy", TypeId = 1, EntityType = "energy", IsNFT = false, Url = "/items/energy/energy.png" };
        public static Item RssTokenItem = new Item { Id = 301, Name = "Rss", TypeId = 1, EntityType = "rss", IsNFT = false, Url = "/items/rss/rss.png" };
        public static List<Item> ItemEnumList = new List<Item>
        {
            SCROLL, DUST,
            #region fragment
            DragonSealBoxCommonFragment,
            DragonSealBoxGiftFragment,
            DragonSealBoxFragment,
            DragonSealBoxMysteryFragment,
            DragonSealLunafragment,
            DragonSealBoxSolfragment,
            DragonSealBoxJadefragment,
            DragonSealBoxCrownfragment,
            DragonSealBoxZhengfragment,
            EggFragmentGold1,
            EggFragmentWood1,
            EggFragmentWater1,
            EggFragmentFire1,
            EggFragmentEarth1,
            EggFragmentDark1,
            EggFragmentLight1,
            EggFragmentGold2,
            EggFragmentWood2,
            EggFragmentWater2,
            EggFragmentFire2,
            EggFragmentEarth2,
            EggFragmentDark2,
            EggFragmentLight2,
            EggFragmentGold3,
            EggFragmentWood3,
            EggFragmentWater3,
            EggFragmentFire3,
            EggFragmentEarth3,
            EggFragmentDark3,
            EggFragmentLight3,
            CommonEggFragment,
            RareEggFragment,
            UniqueEggFragment,
            LegendaryEggFragment,
            AncientMysteryEggFragment,
            WoodenKeyFragment,
            BronzeKeyFragment,
            SilverKeyFragment,
            GoldenKeyFragment,
            RubyKeyFragment,
            TopazKeyFragment,
            DiamondKeyFragment,
            VoidKeyFragment,
            RagKeyFragment,
            IncubatorUncommonfragment,
            IncubatorCommonfragment,
            IncubatorRarefragment,
            IncubatorUniquefragment,
            IncubatorLegendaryfragment,
            IncubatorAncientfragment,
            LairUncommonfragment,
            LairCommonfragment,
            LairRarefragment,
            LairUniquefragment,
            LairLegendaryfragment,
            LairAncientfragment,
            GoldLairRarefragment,
            GoldLairUniquefragment,
            GoldLairLegendaryfragment,
            GoldLairAncientfragment,
            WoodLairRarefragment,
            WoodLairUniquefragment,
            WoodLairLegendaryfragment,
            WoodLairAncientfragment,
            WaterLairRarefragment,
            WaterLairUniquefragment,
            WaterLairLegendaryfragment,
            WaterLairAncientfragment,
            FireLairRarefragment,
            FireLairUniquefragment,
            FireLairLegendaryfragment,
            FireLairAncientfragment,
            EarthLairRarefragment,
            EarthLairUniquefragment,
            EarthLairLegendaryfragment,
            EarthLairAncientfragment,
            LoveZoneUncommonfragment,
            LoveZoneCommonfragment,
            LoveZoneRarefragment,
            LoveZoneUniquefragment,
            LoveZoneLegendaryfragment,
            LoveZoneAncientfragment,
            WorldPortalUncommonfragment,
            WorldPortalCommonfragment,
            WorldPortalRarefragment,
            WorldPortalUniquefragment,
            WorldPortalLegendaryfragment,
            WorldPortalAncientfragment,
            ArenaUncommonfragment,
            ArenaCommonfragment,
            ArenaPortalRarefragment,
            ArenaUniquefragment,
            ArenaLegendaryfragment,
            ArenaAncientfragment,
            #endregion

            #region original
            DragonSealCommonBox,
            DragonSealGiftBox,
            DragonSealBox,
            DragonMysterySealBox,
            DragonSealLunaBox,
            DragonSealSolBox,
            DragonSealJadeBox,
            DragonSealCrownBox,
            DragonSealZhengBox,
            MatingEgg,
            EggGold1,
            EggWood1,
            EggWater1,
            EggFire1,
            EggEarth1,
            EggDark1,
            EggLight1,
            EggGold2,
            EggWood2,
            EggWater2,
            EggFire2,
            EggEarth2,
            EggDark2,
            EggLight2,
            EggGold3,
            EggWood3,
            EggWater3,
            EggFire3,
            EggEarth3,
            EggDark3,
            EggLight3,
            CommonEgg,
            RareEgg,
            UniqueEgg,
            LegendaryEgg,
            AncientMysteryEgg,
            WoodenKey,
            BronzeKey,
            SilverKey,
            GoldenKey,
            RubyKey,
            TopazKey,
            DiamondKey,
            VoidKey,
            RagKey,
            DefaultIncubator,
            IncubatorUncommon,
            IncubatorCommon,
            IncubatorRare,
            IncubatorUnique,
            IncubatorLegendary,
            IncubatorAncient,
            DefaultLair,
            LairUncommon,
            LairCommon,
            LairRare,
            LairUnique,
            LairLegendary,
            LairAncient,
            GoldLairRare,
            GoldLairUnique,
            GoldLairLegendary,
            GoldLairAncient,
            WoodLairRare,
            WoodLairUnique,
            WoodLairLegendary,
            WoodLairAncient,
            WaterLairRare,
            WaterLairUnique,
            WaterLairLegendary,
            WaterLairAncient,
            FireLairRare,
            FireLairUnique,
            FireLairLegendary,
            FireLairAncient,
            EarthLairRare,
            EarthLairUnique,
            EarthLairLegendary,
            EarthLairAncient,
            DefaultLoveZone,
            LoveZoneUncommon,
            LoveZoneCommon,
            LoveZoneRare,
            LoveZoneUnique,
            LoveZoneLegendary,
            LoveZoneAncient,
            DefaultWorldPortal,
            WorldPortalUncommon,
            WorldPortalCommon,
            WorldPortalRare,
            WorldPortalUnique,
            WorldPortalLegendary,
            WorldPortalAncient,
            DefaultArena,
            ArenaUncommon,
            ArenaCommon,
            ArenaPortalRare,
            ArenaUnique,
            ArenaLegendary,
            ArenaAncient,

            GoldTicket,
            WoodTicket,
            WaterTicket,
            FireTicket,
            EarthTicket,
            DarkTicket,
            LightTicket,
            RandomTicket,
            RandomMystertyTicket,


            // Seal0,
            // Seal1,
            // Seal2,
            // Seal3,
            // Seal4,
            // Seal5,
            // Seal6,
            // Seal7,


            NftBoxW,
            NftBoxWPro,
            NftBoxWSaltu,
            NftBoxOceanus,
            NftBoxPahaad,
            NftBoxDesertum,
            NftBoxMors,
            NftBoxSomnium,

            #endregion
            #region fruit
            RareCreationFruit,
            UniqueCreationFruit,
            LegendaryCreationFruit,
            RareBrilliantFruit,
            UniqueBrilliantFruit,
            LegendaryBrilliantFruit,
            RareFlashFruit,
            UniqueFlashFruit,
            LegendaryFlashFruit,
            RareIntellectFruit,
            UniqueIntellectFruit,
            LegendaryIntellectFruit,
            RareSanguinaryFruit,
            UniqueSanguinaryFruit,
            LegendarySanguinaryFruit,
            RareAtheneFruit,
            UniqueAtheneFruit,
            LegendaryAtheneFruit,
            RareLaurusFruit,
            UniqueLaurusFruit,
            LegendaryLaurusFruit,
            RareAraFruit,
            UniqueAraFruit,
            LegendaryAraFruit,
            RarePommeFruit,
            UniquePommeFruit,
            LegendaryPommeFruit,
            RareVitisFruit,
            UniqueVitisFruit,
            LegendaryVitisFruit,
            RareHyperionFruit,
            UniqueHyperionFruit,
            LegendaryHyperionFruit,
            RareOleaFruit,
            UniqueOleaFruit,
            LegendaryOleaFruit,
            RareRosaFruit,
            UniqueRosaFruit,
            LegendaryRosaFruit,
            RareDiasFruit,
            UniqueDiasFruit,
            LegendaryDiasFruit,
            #endregion

            EnergyItem,
            RssTokenItem,
        };
    }
}






