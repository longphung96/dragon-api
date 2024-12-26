using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonAPI.Extensions;

namespace Dragon.Blueprints
{
    public class FamilyEnum
    {
        public static GenericEnum RONGOS = new GenericEnum(Id: 1, Code: "Dragon", Name: "Dragon");
        public static GenericEnum KUBERAOS = new GenericEnum(Id: 2, Code: "Kuberaos", Name: "Kuberaos");
        public static GenericEnum ARANYANIOS = new GenericEnum(Id: 3, Code: "Aranyanios", Name: "Aranyanios");
        public static GenericEnum VARUNAOS = new GenericEnum(Id: 4, Code: "Varunaos", Name: "Varunaos");
        public static GenericEnum AGNIOS = new GenericEnum(Id: 5, Code: "Agnios", Name: "Agnios");
        public static GenericEnum BHUMIOS = new GenericEnum(Id: 6, Code: "Bhumios", Name: "Bhumios");
        public static GenericEnum KALIOS = new GenericEnum(Id: 7, Code: "Kalios", Name: "Kalios");
        public static GenericEnum SURYAOS = new GenericEnum(Id: 8, Code: "Suryaos", Name: "Suryaos");
        public static GenericEnum SPIRIT = new GenericEnum(Id: 50, Code: "Spirit", Name: "Spirit");
        public static GenericEnum SLIME = new GenericEnum(Id: 51, Code: "Slime", Name: "Slime");
        public static GenericEnum SPECTRE = new GenericEnum(Id: 52, Code: "Spectre", Name: "Spectre");
        public static GenericEnum IMP = new GenericEnum(Id: 53, Code: "Imp", Name: "Imp");
        public static GenericEnum ELEMENTAL = new GenericEnum(Id: 54, Code: "Elemental", Name: "Elemental");
        public static GenericEnum ELEMENTSPIRIT = new GenericEnum(Id: 55, Code: "ElementSpirit", Name: "ElementSpirit");
        public static GenericEnum ELEMENTSPECTRE = new GenericEnum(Id: 56, Code: "ElementSpectre", Name: "ElementSpectre");
        public static List<GenericEnum> FamilyEnumList = new List<GenericEnum>
        {
            RONGOS,
            KUBERAOS,
            ARANYANIOS,
            VARUNAOS,
            AGNIOS,
            BHUMIOS,
            KALIOS,
            SURYAOS,
            SPIRIT,
            SLIME,
            SPECTRE,
            IMP,
            ELEMENTAL,
            ELEMENTSPIRIT,
            ELEMENTSPECTRE
        };
    }
}
